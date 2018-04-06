using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Xml;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;

#region [Acesso ao diretório remoto]
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
#endregion

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LogIsRobo : ApplicationPageBaseAutenticadaWindows
    {
        #region [API de personificação remota]
        /// <summary>
        /// Personificação remota
        /// </summary>
        /// <param name="lpszUsername"></param>
        /// <param name="lpszDomain"></param>
        /// <param name="lpszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        /// <summary>
        /// Close the handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        #endregion

        /// <summary>
        /// Inicialização da página de consulta de Logs do ISRobô
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    txtServidor.Text = Environment.MachineName;
                    this.AtualizarStatusServico();

                    txtDataInicio.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    txtDataFim.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");

#if DEBUG
                    txtIPServidor.Text = @"\\vmware-host\Shared Folders\E\ISRoboLog";
#else
                    txtIPServidor.Text = @"\\CARD0032\D$\LogIsRobo";
#endif

                }
            }
            catch (InvalidOperationException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = ex.Message;
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = ex.Message;
            }
        }

        /// <summary>
        /// Atualiza o status do serviço no servidor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizarStatusServico_OnClick(object sender, EventArgs e)
        {
            this.AtualizarStatusServico();
        }

        /// <summary>
        /// Atualiza o status do serviço no servidor
        /// </summary>
        private void AtualizarStatusServico()
        {
            try
            {
                pnlErroStatus.Visible = false;
                ltErroStatus.Text = "";
                txtStatus.Text = "";

                if (txtDominioUsuario.Text.Split('\\').Length != 2)
                {
                    pnlErroStatus.Visible = true;
                    ltErroStatus.Text = @"Informe o usuário no formato Domínio\Usuário";
                    return;
                }

                if (txtSenhaUsuario.Text.Equals(""))
                {
                    pnlErroStatus.Visible = true;
                    ltErroStatus.Text = "Informe a senha do Usuário";
                    return;
                }

                String dominioServidorLog = txtDominioUsuario.Text.Split('\\')[0];
                String usuarioServidorLog = txtDominioUsuario.Text.Split('\\')[1];
                String senhaUsuarioServidorLog = txtSenhaUsuario.Text;

                SafeTokenHandle safeTokenHandle;

                //This parameter causes LogonUser to create a primary token. 
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                Boolean retorno = LogonUser(usuarioServidorLog, dominioServidorLog, senhaUsuarioServidorLog,
                                                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                                                out safeTokenHandle);
                //Logon falhou
                if (!retorno)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new Win32Exception(ret);
                }
                else
                {
                    using (safeTokenHandle)
                    {
                        //Usa o token handle retornado para o logon do usuário
                        using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                        {
                            //Realiza a impersonificação
                            using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                            {
                                // Check the identity.
                                //Console.WriteLine("After impersonation: "
                                //    + WindowsIdentity.GetCurrent().Name);

                                String servidor = txtServidor.Text;
                                String servico = txtServico.Text;

                                ServiceController scISRobo = new ServiceController(servico, servidor);

                                txtStatus.Text = scISRobo.Status.ToString();

                                switch (scISRobo.Status)
                                {
                                    case ServiceControllerStatus.Running:
                                        txtStatus.ForeColor = Color.Blue;
                                        break;
                                    case ServiceControllerStatus.ContinuePending:
                                    case ServiceControllerStatus.Paused:
                                    case ServiceControllerStatus.PausePending:
                                    case ServiceControllerStatus.StartPending:
                                        txtStatus.ForeColor = Color.Yellow;
                                        break;
                                    case ServiceControllerStatus.Stopped:
                                    case ServiceControllerStatus.StopPending:
                                        txtStatus.ForeColor = Color.Red;
                                        break;
                                }

                            }
                        }
                    }
                }
            }
            catch (Win32Exception ex)
            {
                pnlErroStatus.Visible = true;
                ltErroStatus.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                pnlErroStatus.Visible = true;
                ltErroStatus.Text = ex.Message;
            }
            catch (Exception ex)
            {
                pnlErroStatus.Visible = true;
                ltErroStatus.Text = ex.Message;
            }
        }

        /// <summary>
        /// Consulta os logs de acordo com os parâmetros selecionados
        /// </summary>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        protected void btnConsultar_OnClick(Object sender, EventArgs e)
        {
            try
            {

                List<String> logs = null;
                String nomeLog = "";
                DateTime dataInicio = DateTime.Now.Date;
                DateTime dataFim = DateTime.Now.Date;               

                SafeTokenHandle safeTokenHandle;

                //This parameter causes LogonUser to create a primary token. 
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;
                
                pnlDados.Visible = false;
                pnlErroFiltro.Visible = false;
                ltErroFiltro.Text = "";
                pnlErroResultado.Visible = false;
                ltErroResultado.Text = "";

                if (txtDominioUsuario.Text.Split('\\').Length != 2)
                {
                    pnlErroFiltro.Visible = true;
                    ltErroFiltro.Text = @"Informe o usuário no formato Domínio\Usuário";
                    return;
                }

                if (txtSenhaUsuario.Text.Equals(""))
                {
                    pnlErroFiltro.Visible = true;
                    ltErroFiltro.Text = @"Informe a senha do Usuário";
                    return;
                }

                String diretorioLog = txtIPServidor.Text;

                String dominioServidorLog = txtDominioUsuario.Text.Split('\\')[0];
                String usuarioServidorLog = txtDominioUsuario.Text.Split('\\')[1];
                String senhaUsuarioServidorLog = txtSenhaUsuario.Text;

                TimeSpan diasFiltro = dataFim - dataInicio;

                if (chkPorDia.Checked)
                {
                    if (!DateTime.TryParse(txtDataInicio.Text, out dataInicio))
                    {
                        pnlErroFiltro.Visible = true;
                        ltErroFiltro.Text = "Data de início inválida";
                        return;
                    }

                    if (!DateTime.TryParse(txtDataFim.Text, out dataFim))
                    {
                        pnlErroFiltro.Visible = true;
                        ltErroFiltro.Text = "Data de fim inválida";
                        return;
                    }

                    diasFiltro = dataFim - dataInicio;
                }

                Boolean retorno = LogonUser(usuarioServidorLog, dominioServidorLog, senhaUsuarioServidorLog,
                                                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                                                out safeTokenHandle);
                //Logon falhou
                if (!retorno)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new Win32Exception(ret);
                }
                else
                {
                    using (safeTokenHandle)
                    {
                        //Usa o token handle retornado para o logon do usuário
                        using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                        {
                            //Realiza a impersonificação
                            using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                            {
                                // Check the identity.
                                //Console.WriteLine("After impersonation: "
                                //    + WindowsIdentity.GetCurrent().Name);
                                                                
                                if (ddlTipoLog.SelectedValue.Equals("P")) //Carrega os logs de Processamento
                                {
                                    if (ddlTipoProcessamento.SelectedValue.Equals("E")) //Consulta nos logs de Entidade
                                    {
                                        nomeLog = "LOGEntidade{0}.txt";
                                    }
                                    else if (ddlTipoProcessamento.SelectedValue.Equals("U")) //Consulta nos logs de Usuário
                                    {
                                        nomeLog = "LOGUsuario{0}.txt";
                                    }

                                    logs = new List<string>();
                                    for (int d = diasFiltro.Days; d >= 0; d--)
                                    {
                                        DateTime data = dataInicio.AddDays(d);
                                        logs.Add(String.Format(nomeLog, data.ToString("yyyy-MM-dd")));
                                    }
                                }
                                else if (ddlTipoLog.SelectedValue.Equals("T")) //Carrega os logs de Tracing
                                {
                                    if (chkPorDia.Checked)
                                        nomeLog = "Rede.PN.ISRobo.{0}.1.log";
                                    else
                                        nomeLog = "Rede.PN.ISRobo.log";

                                    logs = new List<string>();

                                    for (int d = diasFiltro.Days; d >= 0; d--)
                                    {
                                        DateTime data = dataInicio.AddDays(d);

                                        if (!data.Equals(DateTime.Now.Date))
                                            logs.Add(String.Format(nomeLog, data.AddDays(1).ToString("yyyy-MM-dd")));
                                        else
                                            logs.Add("Rede.PN.ISRobo.log");
                                    }
                                }

                                List<LogProcessamento> logsProcessamentos = new List<LogProcessamento>();
                                List<LogTracing> logsTracing = new List<LogTracing>();

                                foreach (String log in logs)
                                {
                                    if (File.Exists(String.Format(@"{0}\{1}", diretorioLog, log)))
                                    {
                                        using (FileStream arquivoLog = new FileStream(String.Format(@"{0}\{1}", diretorioLog, log), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                        {

                                            if (ddlTipoLog.SelectedValue.Equals("P"))
                                            {
                                                LogProcessamento processamento = LogProcessamento.Load(arquivoLog);
                                                processamento.ArquivoLog = log;

                                                if (chkEntidade.Checked && !String.IsNullOrEmpty(txtEntidade.Text))
                                                {
                                                    Int32 codigoEntidade = 0;

                                                    if (Int32.TryParse(txtEntidade.Text, out codigoEntidade))
                                                        processamento.Entidades = processamento.Entidades.FindAll(i => i.CodigoEntidade == codigoEntidade);
                                                }

                                                if (chkStatusProcessamento.Checked && !ddlStatusProcessamento.SelectedValue.Equals("T"))
                                                {
                                                    processamento.Entidades = processamento.Entidades.FindAll(i => i.Processada == ddlStatusProcessamento.SelectedValue.Equals("P") ? true : false);
                                                }

                                                logsProcessamentos.Add(processamento);
                                            }
                                            else if (ddlTipoLog.SelectedValue.Equals("T"))
                                            {
                                                LogTracing tracing = new LogTracing();

                                                using (StreamReader arquivo = new StreamReader(arquivoLog))
                                                {    
                                                    tracing.TracesLog = new List<Modelo.Trace>();
                                                    tracing.TracesLog.AddRange(LogTracing.Load(arquivo));
                                                }

                                                tracing.ArquivoLog = log;

                                                if (ddlTipoProcessamentoTracing.SelectedValue.Equals("E"))
                                                {
                                                    tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Processamento == Modelo.Trace.TipoProcessamento.Entidade);
                                                }
                                                else if (ddlTipoProcessamentoTracing.SelectedValue.Equals("U"))
                                                {
                                                    tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Processamento == Modelo.Trace.TipoProcessamento.Usuario);
                                                }
                                                else if (ddlTipoProcessamentoTracing.SelectedValue.Equals("S"))
                                                {
                                                    tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Processamento == Modelo.Trace.TipoProcessamento.Servico);
                                                }

                                                if (chkEntidade.Checked)
                                                {
                                                    tracing.TracesLog = tracing.TracesLog.FindAll(i => i.CodigoEntidade == txtEntidade.Text.ToInt32());
                                                }

                                                if (chkSeveridade.Checked)
                                                {
                                                    if (ddlSeveridade.SelectedValue.Equals("I"))
                                                        tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Severidade == Modelo.Trace.TraceEventType.Information);
                                                    else if (ddlSeveridade.SelectedValue.Equals("A"))
                                                        tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Severidade == Modelo.Trace.TraceEventType.Warning);
                                                    else if (ddlSeveridade.SelectedValue.Equals("E"))
                                                        tracing.TracesLog = tracing.TracesLog.FindAll(i => i.Severidade == Modelo.Trace.TraceEventType.Error);
                                                }

                                                logsTracing.Add(tracing);
                                            }
                                        }
                                    }
                                }

                                if (logsProcessamentos.Count > 0 || logsTracing.Count > 0)
                                {
                                    pnlDados.Visible = true;
                                    if (ddlTipoLog.SelectedValue.Equals("P"))
                                        rptDados.DataSource = logsProcessamentos;
                                    else
                                        rptDados.DataSource = logsTracing;
                                    rptDados.DataBind();
                                }
                                else
                                {
                                    pnlDados.Visible = false;
                                    pnlErroResultado.Visible = true;
                                    ltErroResultado.Text = "Nenhum resultado encontrado";
                                }
                            }
                        }
                    }
                }
            }
            catch (Win32Exception ex)
            {
                pnlErroFiltro.Visible = true;
                ltErroFiltro.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroFiltro.Visible = true;
                ltErroFiltro.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Carrega o modelo de Repeater para cada arquivo de log encontrado para o período de busca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDados_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (!object.ReferenceEquals(e.Item.DataItem, null))
                    {
                        Label lblArquivoLog = (Label)e.Item.FindControl("lblArquivoLog");
                        Repeater rptLogProcessamento = (Repeater)e.Item.FindControl("rptLogProcessamento");
                        Repeater rtpLogTracing = (Repeater)e.Item.FindControl("rtpLogTracing");

                        if (ddlTipoLog.SelectedValue.Equals("P"))
                        {
                            LogProcessamento processamento = (LogProcessamento)e.Item.DataItem;

                            lblArquivoLog.Text = processamento.ArquivoLog;

                            rtpLogTracing.Visible = false;
                            rptLogProcessamento.Visible = true;
                            rptLogProcessamento.DataSource = processamento.Entidades;
                            rptLogProcessamento.DataBind();
                        }
                        else
                        {
                            LogTracing processamento = (LogTracing)e.Item.DataItem;
                            
                            lblArquivoLog.Text = processamento.ArquivoLog;
                                                        
                            rptLogProcessamento.Visible = false;
                            rtpLogTracing.Visible = true;
                            //processamento.TracesLog = processamento.TracesLog.Sort(i => i.DataHora,
                            rtpLogTracing.DataSource = processamento.TracesLog;
                            rtpLogTracing.DataBind();
                        }
                    }
                    else
                    {
                        pnlErroResultado.Visible = true;
                        ltErroResultado.Text = "Ocorreu um erro ao carregar os dados dos Logs.";
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Carrega os dados do arquivo de log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptLogProcessamento_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (!Object.ReferenceEquals(e.Item.DataItem, null))
                    {
                        Modelo.Entidade entidadeLog = (Modelo.Entidade)e.Item.DataItem;

                        Label lblProcessada = (Label)e.Item.FindControl("lblProcessada");
                        Label lblEntidade = (Label)e.Item.FindControl("lblEntidade");
                        Repeater rptDataHora = (Repeater)e.Item.FindControl("rptDataHora");
                        Repeater rptFluxos = (Repeater)e.Item.FindControl("rptFluxos");
                        Label lblRazaoSocial = (Label)e.Item.FindControl("lblRazaoSocial");
                        Label lblEmail = (Label)e.Item.FindControl("lblEmail");
                        Label lblPossuiKomerci = (Label)e.Item.FindControl("lblPossuiKomerci");
                        Label lblUsuario = (Label)e.Item.FindControl("lblUsuario");

                        lblProcessada.Text = entidadeLog.Processada.Equals(true) ? "Sim" : "Não";
                        lblEntidade.Text = entidadeLog.CodigoEntidade.ToString();

                        if (entidadeLog.DataHoraProcessamentos.Count > 10)
                        {
                            entidadeLog.DataHoraProcessamentos = entidadeLog.DataHoraProcessamentos.GetRange(0, 9);
                            entidadeLog.DataHoraProcessamentos.Add(entidadeLog.DataHoraProcessamentos[entidadeLog.DataHoraProcessamentos.Count - 1]);
                        }

                        rptDataHora.DataSource = entidadeLog.DataHoraProcessamentos;
                        rptDataHora.DataBind();                            

                        lblRazaoSocial.Text = entidadeLog.RazaoSocial;
                        lblEmail.Text = entidadeLog.Email;
                        lblPossuiKomerci.Text = entidadeLog.PossuiKomerci.Equals(true) ? "Sim" : "Não";

                        if (ddlTipoProcessamento.SelectedValue.Equals("U"))
                        {
                            if (entidadeLog.Usuario != null)
                                lblUsuario.Text = String.Format("Login: {0}", entidadeLog.Usuario.Login);

                            if (entidadeLog.FluxosProcessamento.Count > 10)
                            {
                                entidadeLog.FluxosProcessamento = entidadeLog.FluxosProcessamento.GetRange(0, 9);
                                entidadeLog.FluxosProcessamento.Add(entidadeLog.FluxosProcessamento[entidadeLog.FluxosProcessamento.Count -1]);
                            }

                            rptFluxos.DataSource = entidadeLog.FluxosProcessamento;
                            rptFluxos.DataBind();
                        }
                    }
                    else
                    {
                        pnlErroResultado.Visible = true;
                        ltErroResultado.Text = String.Format("Erro ao carregar o Log de Processamento");
                    }

                }
            }
            catch (ArgumentNullException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Carrega os horários de processamentos da Entidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDataHora_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (!Object.ReferenceEquals(e.Item.DataItem, null))
                    {
                        DateTime dataProcessamento = (DateTime)e.Item.DataItem;

                        Label lblDataHora = (Label)e.Item.FindControl("lblDataHora");
                        lblDataHora.Text = dataProcessamento.ToString();
                    }
                    else
                    {
                        pnlErroResultado.Visible = true;
                        ltErroResultado.Text = String.Format("Erro ao carregar o Log de Processamento");
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }

        }

        /// <summary>
        /// Carrega os fluxos de processamento da Entidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptFluxos_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (!Object.ReferenceEquals(e.Item.DataItem, null))
                    {
                        Int32 fluxo = (Int32)e.Item.DataItem;

                        Label lblFluxo = (Label)e.Item.FindControl("lblFluxo");
                        lblFluxo.Text = fluxo.ToString();
                    }
                    else
                    {
                        pnlErroResultado.Visible = true;
                        ltErroResultado.Text = String.Format("Erro ao carregar o Log de Processamento");
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rtpLogTracing_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        { 
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (!Object.ReferenceEquals(e.Item.DataItem, null))
                    {
                        Modelo.Trace traceLog = (Modelo.Trace)e.Item.DataItem;

                        Label lblDataHora = (Label)e.Item.FindControl("lblDataHora");
                        Label lblEntidade = (Label)e.Item.FindControl("lblEntidade");
                        Label lblMensagem = (Label)e.Item.FindControl("lblMensagem");
                        Label lblMaquina = (Label)e.Item.FindControl("lblMaquina");
                        Label lblSeveridade = (Label)e.Item.FindControl("lblSeveridade");

                        Color corServeridade = new Color();

                        switch(traceLog.Severidade)
                        {
                            case Modelo.Trace.TraceEventType.Warning:
                                corServeridade = Color.Yellow;
                                break;
                            case Modelo.Trace.TraceEventType.Error:
                                corServeridade = Color.Red;
                                break;
                            case Modelo.Trace.TraceEventType.Information:
                            default:
                                corServeridade = Color.Blue;
                                break;
                        }

                        lblDataHora.ForeColor = corServeridade;
                        lblDataHora.Text = traceLog.DataHora.ToString("dd/MM/yyyy HH:mm:ss.fffffff");

                        lblEntidade.ForeColor = corServeridade;
                        lblEntidade.Text = traceLog.CodigoEntidade.ToString();

                        lblMensagem.ForeColor = corServeridade;
                        lblMensagem.Text = traceLog.Mensagem;

                        lblMaquina.ForeColor = corServeridade;
                        lblMaquina.Text = traceLog.Maquina;

                        lblSeveridade.ForeColor = corServeridade;
                        lblSeveridade.Text = traceLog.Severidade.ToString();
                    }
                    else
                    {
                        pnlErroResultado.Visible = true;
                        ltErroResultado.Text = "Ocorreu um erro ao carregar os dados do Log de Tracing";
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                pnlErroResultado.Visible = true;
                ltErroResultado.Text = String.Format("{0}<br>{1}", ex.Message, ex.InnerException);
            }
        }

    }
}
