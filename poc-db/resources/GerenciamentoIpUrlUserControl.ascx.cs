using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Collections.Generic;
using Redecard.PN.DataCash.SharePoint.DataCashService;
using System.ServiceModel;
using System.Linq;

namespace Redecard.PN.DataCash.SharePoint.WebParts.GerenciamentoIpUrl
{
    public partial class GerenciamentoIpUrlUserControl : UserControlBaseDataCash
    {
        #region [ Propriedades/Constantes ]

        /// <summary>
        /// Lista de IPs
        /// </summary>
        protected List<String> IPs
        {
            get 
            {
                if (ViewState["IPs"] == null)
                    IPs = new List<String>();
                return (List<String>)ViewState["IPs"];
            }
            set { ViewState["IPs"] = value; }
        }

        #endregion

        #region [ Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                mvwGerenciamentoIPUrlBack.SetActiveView(vwDadosFerramenta);
            }
        }

        #endregion

        #region [ Eventos de Botões ]

        /// <summary>
        /// Evento de botão para Exclusão de um IP do controle (lista de IPs)
        /// </summary>
        protected void btnDadosFerramentaExcluirIP_Click(object sender, ImageClickEventArgs e)
        {
            IButtonControl btnDadosFerramentaExcluirIP = (IButtonControl)sender;
            Int32 itemIndex = btnDadosFerramentaExcluirIP.CommandArgument.ToInt32(-1);
            if (IPs.Count > itemIndex)
            {
                IPs.RemoveAt(itemIndex);
                IPs.Sort();
                rptDadosFerramentaIPs.Visible = IPs.Count > 0;
                rptDadosFerramentaIPs.DataSource = IPs;
                rptDadosFerramentaIPs.DataBind();
            }
        }

        /// <summary>
        /// Evento de botão para Inclusão de um IP no controle (lista de IPs)
        /// </summary>
        protected void btnDadosFerramentaAdicionarIP_Click(object sender, EventArgs e)
        {
            String ip = txtIP.Text.EmptyToNull();

            if (ip != null)
            {
                { //Formata IP, quando composto corretamente por 4 octetos
                    var ipOctetos = ip.Split('.');
                    if (ipOctetos.Length == 4)
                        ip = String.Join(".", ipOctetos.Select(octeto => octeto.ToInt32(0).ToString()).ToArray());
                }

                //Verifica se IP já está na lista temporária e se é válido
                if (!IPs.Contains(ip))
                {                    
                    IPs.Add(ip);
                    IPs.Sort();

                    txtIP.Text = String.Empty; //limpa campo de IP, pois é válido e já foi incluído com sucesso na lista temporária
                }

                rptDadosFerramentaIPs.Visible = IPs.Count > 0;
                rptDadosFerramentaIPs.DataSource = IPs;
                rptDadosFerramentaIPs.DataBind();
            }
        }

        /// <summary>
        /// Evento de botão para continuação
        /// </summary>
        protected void btnDadosFerramentaContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Validação Gerenciamento IP e UrlBack"))
            {
                //Recupera dados
                String urlHPS = txtHPS.Text;
                String urlExpiracao = txtExpiracao.Text;
                String urlRiscoOffline = txtRiscoOffline.Text;
                List<String> listaIps = IPs;
                String mensagemValidacao = String.Empty;

                //Valida dados de entrada
                if (ValidarDadosFerramenta(urlHPS, urlExpiracao, urlRiscoOffline, listaIps, out mensagemValidacao))
                {
                    //Altera View de Edição para Confirmação
                    mvwGerenciamentoIPUrlBack.SetActiveView(vwConfirmacao);
                    ExibirMensagemValidacaoDadosFerramenta(false, null);
                }
                else
                {
                    ExibirMensagemValidacaoDadosFerramenta(true, mensagemValidacao);
                }
            }
        }

        /// <summary>
        /// Evento de botão para confirmação dos dados para atualização
        /// </summary>
        protected void btnConfirmacaoContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Gravando Gerenciamento IP e UrlBack"))
            {
                //Recupera dados
                String urlHPS = lblConfirmacaoHPS.Text;
                String urlExpiracao = lblConfirmacaoExpiracao.Text;
                String urlRiscoOffline = lblConfirmacaoRiscoOffline.Text;
                List<String> listaIps = IPs;
                String mensagemValidacao = String.Empty;

                //Valida dados confirmados
                if (ValidarDadosFerramenta(urlHPS, urlExpiracao, urlRiscoOffline, listaIps, out mensagemValidacao))
                {
                    //Grava dados e altera View de Confirmação para Comprovante                
                    Boolean sucesso = GravarDadosFerramenta(SessaoAtual.CodigoEntidade, urlHPS, urlExpiracao, urlRiscoOffline, listaIps);

                    if (sucesso)
                    {
                        //Altera view de Confirmação para Comprovante
                        mvwGerenciamentoIPUrlBack.SetActiveView(vwComprovante);
                        ExibirMensagemValidacaoConfirmacao(false, null);
                    }
                }
                else
                {
                    ExibirMensagemValidacaoConfirmacao(true, mensagemValidacao);
                }
            }
        }

        /// <summary>
        /// Evento de botão para Voltar para passo inicial
        /// </summary>
        protected void btnComprovanteVoltar_Click(object sender, EventArgs e)
        {                                    
            //Altera para View inicial de Edição
            mvwGerenciamentoIPUrlBack.SetActiveView(vwDadosFerramenta);          
        }

        #endregion

        #region [ MultiView ]

        protected void mvwGerenciamentoIPUrlBack_ActiveViewChanged(object sender, EventArgs e)
        {
            //Atualiza exibição de valores dos controles
            var activeView = mvwGerenciamentoIPUrlBack.GetActiveView();

            if (activeView == vwDadosFerramenta)
            {
                //Carrega os dados iniciais para os controles de edição
                CarregarDadosFerramenta();
            }
            else if (activeView == vwConfirmacao)
            {
                //Replica os dados dos controles de edição para os controles de confirmação
                ReplicarDadosFerramentaParaConfirmacao();
            }
            else if (activeView == vwComprovante)
            {
                //Replica os dados dos controles de confirmação para os controles de comprovante
                ReplicarConfirmacaoParaComprovante();
            }

            //Atualiza o passo atual no processo
            AtualizaCabecalhoPassoAtual();
        }

        /// <summary>
        /// Atualiza cabeçalho com dados do Passo Atual
        /// </summary>
        private void AtualizaCabecalhoPassoAtual()
        {
            View activeView = mvwGerenciamentoIPUrlBack.GetActiveView();
            if (activeView == vwDadosFerramenta)
            {
                ucHeaderPassos.AtivarPasso(0);
            }
            else if (activeView == vwConfirmacao)
            {
                ucHeaderPassos.AtivarPasso(1);
            }
            else if (activeView == vwComprovante)
            {
                ucHeaderPassos.AtivarPasso(2);
            }
        }

        /// <summary>
        /// Carrega dados iniciais
        /// </summary>
        private void CarregarDadosFerramenta()
        {
            using (Logger Log = Logger.IniciarLog("Carregar Gerenciamento IP e UrlBack"))
            {
                //Carrega dados iniciais
                {
                    GerenciamentoPV dadosFerramenta = ConsultarDadosFerramenta(SessaoAtual.CodigoEntidade);
                    Boolean retornoOK = dadosFerramenta != null;

                    if (retornoOK)
                    {
                        txtExpiracao.Text = dadosFerramenta.UrlExpirado;
                        txtHPS.Text = dadosFerramenta.UrlHps;
                        txtRiscoOffline.Text = dadosFerramenta.UrlOffLine;
                        txtIP.Text = String.Empty;

                        IPs = dadosFerramenta.Ips;
                        IPs.Sort();

                        rptDadosFerramentaIPs.Visible = IPs.Count > 0;
                        rptDadosFerramentaIPs.DataSource = IPs;
                        rptDadosFerramentaIPs.DataBind();
                    }

                    //(des)bloqueia controles, de acordo com consulta com erro ou sucesso                  
                    txtExpiracao.Enabled = retornoOK;
                    txtHPS.Enabled = retornoOK;
                    txtRiscoOffline.Enabled = retornoOK;
                    txtIP.Enabled = retornoOK;
                    btnDadosFerramentaAdicionarIP.Enabled = retornoOK;
                    btnDadosFerramentaContinuar.Enabled = retornoOK;
                }

                //Limpa os campos de confirmação e comprovante
                lblComprovanteExpiracao.Text = String.Empty;
                lblComprovanteHPS.Text = String.Empty;
                lblComprovanteRiscoOffline.Text = String.Empty;
                lblConfirmacaoExpiracao.Text = String.Empty;
                lblConfirmacaoHPS.Text = String.Empty;
                lblConfirmacaoRiscoOffline.Text = String.Empty;
            }
        }

        /// <summary>
        /// Replica dados informados pelo usuário para passo de confirmação
        /// </summary>
        private void ReplicarDadosFerramentaParaConfirmacao()
        {
            lblConfirmacaoExpiracao.Text = txtExpiracao.Text;
            lblConfirmacaoHPS.Text = txtHPS.Text;
            lblConfirmacaoRiscoOffline.Text = txtRiscoOffline.Text;

            rptConfirmacaoIPs.DataSource = IPs;
            rptConfirmacaoIPs.DataBind();
        }

        /// <summary>
        /// Replica dados da confirmação para passo de comprovante
        /// </summary>
        private void ReplicarConfirmacaoParaComprovante()
        {
            lblComprovanteExpiracao.Text = lblConfirmacaoExpiracao.Text;
            lblComprovanteHPS.Text = lblConfirmacaoHPS.Text;
            lblComprovanteRiscoOffline.Text = lblConfirmacaoRiscoOffline.Text;

            rptComprovanteIPs.DataSource = IPs;
            rptComprovanteIPs.DataBind();
        }

        /// <summary>
        /// Seta exibição de mensagem de validação no passo de edição de dados
        /// </summary>        
        private void ExibirMensagemValidacaoDadosFerramenta(Boolean exibir, String mensagem)
        {
            lblDadosFerramentaMensagem.Visible = exibir;
            lblDadosFerramentaMensagem.Text = mensagem;
        }

        /// <summary>
        /// Exibe mensagem de validação da confirmação
        /// </summary>
        private void ExibirMensagemValidacaoConfirmacao(Boolean exibir, String mensagem)
        {
            lblConfirmacaoMensagem.Visible = exibir;
            lblConfirmacaoMensagem.Text = mensagem;
        }
        
        #endregion

        #region [ Operações / Validações ]

        /// <summary>
        /// Atualiza os dados de configuração de IP e URLBack
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="urlHPS">Url HPS</param>
        /// <param name="urlExpiracao">Url Expiração</param>
        /// <param name="urlRiscoOffline">Url Risco Offline</param>
        /// <param name="listaIPs">Lista de IPs</param>
        /// <returns>Booleano de retorno da operação</returns>
        private Boolean GravarDadosFerramenta(Int32 codigoEntidade, String urlHPS, String urlExpiracao, String urlRiscoOffline, List<String> listaIPs)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Gravar dados Ferramenta"))
            {
                try
                {
                    //Monta objeto com dados a serem gravados
                    var gerenciamentoPV = new GerenciamentoPV();
                    gerenciamentoPV.Ips = listaIPs;
                    gerenciamentoPV.UrlExpirado = urlExpiracao;
                    gerenciamentoPV.UrlHps = urlHPS;
                    gerenciamentoPV.UrlOffLine = urlRiscoOffline;
                    gerenciamentoPV.PV = codigoEntidade;

                    MensagemErro mensagemErro;

                    using (var contexto = new ContextoWCF<DataCashServiceClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { gerenciamentoPV });
                        retorno = contexto.Cliente.ManutencaoGerenciamentoPV(out mensagemErro, gerenciamentoPV);
                        Log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
                    }

                    if (mensagemErro != null && mensagemErro.CodigoRetorno != 1)
                    {
                        base.ExibirPainelExcecao("DataCashService.ManutencaoGerenciamentoPV", mensagemErro.CodigoRetorno);
                        retorno = false;
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Consulta dados de configuração de IP e URLBack
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Dados de configuração do IP e URLBack</returns>
        private GerenciamentoPV ConsultarDadosFerramenta(Int32 codigoEntidade)
        {
            GerenciamentoPV retorno = null;

            using (Logger Log = Logger.IniciarLog("Consultar dados ferramenta"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<DataCashServiceClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { codigoEntidade });
                        retorno = contexto.Cliente.ConsultaGerencimentoPV(codigoEntidade);
                        Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    }

                    if (retorno != null && retorno.CodigoRetorno != 1)
                    {
                        base.ExibirPainelExcecao("DataCashService.ConsultaGerenciamentoPV", retorno.CodigoRetorno);
                        return null;
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Validação de IP
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="ip">Número do IP</param>
        /// <returns>Booleano indicando resultado da validação</returns>
        [Obsolete("TODO: Método não utilizado - Remover")]
        private Boolean ValidarIP(Int32 codigoEntidade, String ip)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Validação de IP"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<DataCashServiceClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { codigoEntidade, ip });
                        //retorno = contexto.Cliente.ValidaIp(codigoEntidade, ip);
                        Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    }                    
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Validação server-side dos dados informados pelo usuário.
        /// </summary>
        /// <param name="urlHPS">Url HPS</param>
        /// <param name="urlExpiracao">Url Expiração</param>
        /// <param name="urlRiscoOffline">Url Risco Offline</param>
        /// <param name="listaIPs">Lista de IPs</param>
        /// <param name="mensagemValidacao">Mensagem de validação</param>
        /// <returns>Booleano do resultado da zvalidação</returns>
        private static Boolean ValidarDadosFerramenta(String urlHPS, String urlExpiracao, String urlRiscoOffline, List<String> listaIPs, out String mensagemValidacao)
        {
            if (urlHPS.EmptyToNull() == null)
            {
                mensagemValidacao = "Urlback HPS é obrigatório.";
                return false;
            }
            else if (urlExpiracao.EmptyToNull() == null)
            {
                mensagemValidacao = "Urlback Expiração é obrigatório.";
                return false;
            }
            else if (urlRiscoOffline.EmptyToNull() == null)
            {
                mensagemValidacao = "Urlback Risco Off-line é obrigatório.";
                return false;
            }
            else if (listaIPs == null || listaIPs.Count == 0)
            {
                mensagemValidacao = "IP é obrigatório.";
                return false;
            }

            mensagemValidacao = null;
            return true;
        }

        #endregion
    }
}
