using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.GEGerencias;
using Redecard.PN.Credenciamento.Sharepoint.GECarteiras;
using Redecard.PN.Credenciamento.Sharepoint.GEDomBancario;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using Redecard.PN.Credenciamento.Sharepoint.GEGrpComGer;
using System.Text.RegularExpressions;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosOperacionais
{
    public partial class DadosOperacionaisUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Credenciamento.Fase < 4)
                    Credenciamento.Fase = 4;

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados Operacionais";
                Page.Culture = "en-GB";

                if (!Page.IsPostBack)
                {
                    Page.DataBind();

                    CarregaGerencias();
                    ddlGerencia.SelectedValue = Credenciamento.CodGerencia != null && !Char.IsWhiteSpace((Char)Credenciamento.CodGerencia) ? Credenciamento.CodGerencia.ToString() : "V";
                    CarregaCarteiras();

                    if (Credenciamento.PermIATA != null && Credenciamento.PermIATA == 'S')
                    {
                        ddlParcelamentoIATA.Enabled = true;
                    }

                    if (String.Compare(Credenciamento.TipoPessoa, "F") == 0)
                        ddlTipoEstabelecimento.SelectedValue = "0";
                    else
                        if (Credenciamento.CodTipoEstabelecimento == null)
                            ddlTipoEstabelecimento.SelectedValue = "2";
                        else
                            ddlTipoEstabelecimento.SelectedValue = Credenciamento.CodTipoEstabelecimento.ToString();

                    // Carrega dados já salvos
                    rbHorarioFuncionamento.SelectedValue = Credenciamento.HorarioFuncionamento != null ? Credenciamento.HorarioFuncionamento.ToString() : rbHorarioFuncionamento.SelectedValue;
                    txtNomeFatura.Text = !String.IsNullOrEmpty(Credenciamento.NomeFatura) ? Credenciamento.NomeFatura.Trim() : txtNomeFatura.Text;
                    txtNroGrupoComercial.Text = Credenciamento.GrupoComercial != null && Credenciamento.GrupoComercial != 0 ? Credenciamento.GrupoComercial.ToString() : txtNroGrupoComercial.Text;
                    txtNroGrupoGerencial.Text = Credenciamento.GrupoGerencial != null && Credenciamento.GrupoGerencial != 0 ? Credenciamento.GrupoGerencial.ToString() : txtNroGrupoGerencial.Text;
                    ddlLocalPagamento.SelectedValue = Credenciamento.LocalPagamento != null ? Credenciamento.LocalPagamento.ToString() : ddlLocalPagamento.SelectedValue;
                    ddlLocalPagamento.Enabled = Credenciamento.CodTipoEstabelecimento == 1 ? true : false;
                    if (String.Compare(ddlLocalPagamento.SelectedValue, "2") == 0)
                    {
                        pnlCentralizadora.Visible = true;
                        txtCentralizadora.Enabled = true;
                    }
                    txtCentralizadora.Text = Credenciamento.Centralizadora != null && Credenciamento.Centralizadora != 0 ? Credenciamento.Centralizadora.ToString() : txtCentralizadora.Text;
                    txtDataAssProposta.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txtCPFVendedor.Text = Credenciamento.CPFVendedor != null && Credenciamento.CPFVendedor != 0 ? String.Format(@"{0:000\.000\.000\-00}", Credenciamento.CPFVendedor) : txtCPFVendedor.Text;
                    ddlCarteira.SelectedValue = Credenciamento.CodCarteira != null ? Credenciamento.CodCarteira.ToString() : ddlCarteira.SelectedValue;

                    if (Credenciamento.CodTipoEstabelecimento != null && Credenciamento.CodTipoEstabelecimento == 1)
                        txtNroGrupoComercial.Enabled = false;

                    if (Credenciamento.Canal == 4)
                        ltlCPFVendedor.Text = "CPF do Promotor*:";

                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        rfvCPFVendedor.Enabled = false;
                        rfvCPFVendedor.Visible = false;

                        //ddlParcelamentoIATA.SelectedValue = "Sim";
                    }

                    if (Credenciamento.TipoComercializacao.Equals("80105"))
                    {
                        var grupoGerencialBNDES = ConsultaGrupoGerencialBNDES();
                        txtNroGrupoGerencial.Text = grupoGerencialBNDES.ToString();
                        txtNroGrupoGerencial.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        private Int32 ConsultaGrupoGerencialBNDES()
        {
            Int32 grupoGerencialBNDES;

            using (var log = Logger.IniciarLog("Carrega Grupo Gerencial BNDES"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    
                });

                using (var context = new ContextoWCF<ServicoPortalGEGruposComerciaisGerenciaisClient>())
                {
                    grupoGerencialBNDES = context.Cliente.ConsultaGrupoGerencialBNDES();
                }

                log.GravarLog(EventoLog.RetornoServico, new { 
                    grupoGerencialBNDES
                });
            }

            return grupoGerencialBNDES;
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Int32 codRetorno = SalvarDados();
            if (codRetorno == 0)
                Response.Redirect("pn_dadosbancarios.aspx", false);
            else if (codRetorno != -1)
                base.ExibirPainelExcecao("Redecard.PN.Credenciamento.Servicos", codRetorno);
        }

        /// <summary>
        /// Evento do botão Parar e Salvar Proposta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();

            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx", false);
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_dadosendereco.aspx", false);
        }

        /// <summary>
        /// Carrega dropdown de Carteiras ao selecionar uma Gerência
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlGerencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregaCarteiras();
        }

        /// <summary>
        /// Mostra ou esconde informações da centralizadora de acordo com o local de pagamento selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlLocalPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pnlCentralizadora.Visible = !pnlCentralizadora.Visible;
                txtCentralizadora.Enabled = !txtCentralizadora.Enabled;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida Nro do Grupo Comercial
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvNroGrupoComercial_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                GEGrpComGer.CodErroDescricaoErro retorno = ValidaGrupoComercial(txtNroGrupoComercial.Text.ToInt32());
                if (retorno.CodErro == 0)
                    args.IsValid = true;
                //else
                //    base.ExibirPainelExcecao(retorno.DescricaoErro, retorno.CodErro.ToString());
            }
            catch (FaultException<GEGrpComGer.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida Nro do Grupo Gerencial
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvNroGrupoGerencial_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                GEGrpComGer.CodErroDescricaoErro retorno = ValidaGrupoGerencial(txtNroGrupoGerencial.Text.ToInt32());
                if (retorno.CodErro == 0)
                    args.IsValid = true;
                //else
                //    base.ExibirPainelExcecao(retorno.DescricaoErro, retorno.CodErro.ToString());
            }
            catch (FaultException<GEGrpComGer.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCPFVendedor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCPFVendedor.Text.Replace(".", "").Replace("-", "").IsValidCPF();
        }

        /// <summary>
        /// Valida PV Centralizador
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCentralizadora_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaCentralizadora();
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de Carteiras e preenche o dropdown
        /// </summary>
        private void CarregaCarteiras()
        {
            ServicoPortalGECarteirasClient client = new ServicoPortalGECarteirasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Carteiras"))
                {
                    Char codGerencia = ddlGerencia.SelectedValue.ToCharArray()[0];

                    ddlCarteira.Items.Clear();

                    CarteirasListaDadosCadastrais[] carteiras = client.ListaDadosCadastrais(codGerencia, null, String.Empty);
                    client.Close();

                    foreach (CarteirasListaDadosCadastrais carteira in carteiras)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", carteira.CodCarteira, carteira.NomeCarteira), carteira.CodCarteira.ToString());
                        ddlCarteira.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GECarteiras.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Gerências e preenche o dropdown
        /// </summary>
        private void CarregaGerencias()
        {
            ServicoPortalGEGerenciaClient client = new ServicoPortalGEGerenciaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Gerencias"))
                {
                    GerenciasListaDadosCadastrais[] gerencias = client.ListaDadosCadastrais();
                    client.Close();

                    foreach (GerenciasListaDadosCadastrais gerencia in gerencias)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", gerencia.CodGerencia, gerencia.NomeGerencia), gerencia.CodGerencia.ToString());
                        ddlGerencia.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GEGerencias.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da quinta tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso5()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Operacionais"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();

                    Int32 retorno = client.GravarAtualizarPasso5(proposta);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;

            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Salva dados da tela
        /// </summary>
        /// <returns></returns>
        private Int32 SalvarDados()
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    Credenciamento.HorarioFuncionamento = rbHorarioFuncionamento.SelectedValue.ToInt32Null();
                    Credenciamento.NomeFatura = txtNomeFatura.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.GrupoComercial = txtNroGrupoComercial.Text.ToInt32Null();
                    Credenciamento.GrupoGerencial = txtNroGrupoGerencial.Text.ToInt32Null();
                    Credenciamento.LocalPagamento = ddlLocalPagamento.SelectedValue.ToInt32Null();
                    Credenciamento.Centralizadora = !String.IsNullOrEmpty(txtCentralizadora.Text) ? txtCentralizadora.Text.ToInt32Null() : 0;
                    Credenciamento.DataCadastroProposta = txtDataAssProposta.Text.ToDate();
                    Credenciamento.CPFVendedor = !String.IsNullOrEmpty(txtCPFVendedor.Text) ? Int64.Parse(txtCPFVendedor.Text.Replace("-", "").Replace(".", "")) : 0;
                    Credenciamento.CodGerencia = ddlGerencia.SelectedValue.ToCharArray()[0];
                    Credenciamento.CodCarteira = ddlCarteira.SelectedValue.ToInt32Null();
                    Credenciamento.CodTipoEstabelecimento = ddlTipoEstabelecimento.SelectedValue.ToInt32Null();

                    return GravarAtualizarPasso5();
                }
                return -1;
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                return fe.Detail.Codigo;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Operacionais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        /// <summary>
        /// Método de validação do número do grupo comercial
        /// </summary>
        /// <param name="grupoComercial"></param>
        /// <returns></returns>
        private GEGrpComGer.CodErroDescricaoErro ValidaGrupoComercial(Int32 grupoComercial)
        {
            ServicoPortalGEGruposComerciaisGerenciaisClient client = new ServicoPortalGEGruposComerciaisGerenciaisClient();

            try
            {
                GEGrpComGer.CodErroDescricaoErro retorno = client.ValidaGrupoComercial(grupoComercial);
                client.Close();

                return retorno;
            }
            catch (FaultException<GEGrpComGer.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Método de validação do número do grupo gerencial
        /// </summary>
        /// <param name="grupoGerencial"></param>
        /// <returns></returns>
        private GEGrpComGer.CodErroDescricaoErro ValidaGrupoGerencial(Int32 grupoGerencial)
        {
            ServicoPortalGEGruposComerciaisGerenciaisClient client = new ServicoPortalGEGruposComerciaisGerenciaisClient();

            try
            {
                GEGrpComGer.CodErroDescricaoErro retorno = client.ValidaGrupoGerencial(grupoGerencial);
                client.Close();

                return retorno;
            }
            catch (FaultException<GEGrpComGer.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Chamada ao serviço de validação da centralizadora
        /// </summary>
        /// <returns></returns>
        private Boolean ValidaCentralizadora()
        {
            ServicoPortalGEDomicilioBancarioClient client = new ServicoPortalGEDomicilioBancarioClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Valida PV Centralizador"))
                {
                    Int32 numCentralizadora = txtCentralizadora.Text.ToInt32();
                    Int64 numCNPJ;
                    if (Credenciamento.TipoPessoa == "J")
                        numCNPJ = Credenciamento.CNPJ.CpfCnpjToLong();
                    else
                        numCNPJ = Credenciamento.CPF.CpfCnpjToLong();

                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtividade = Credenciamento.RamoAtividade;
                    Int32 codBanco = Credenciamento.CodBancoCredito;
                    Int32 codAgencia = Credenciamento.AgenciaCredito;
                    String contaCorrente = Credenciamento.ContaCredito;

                    GEDomBancario.CodErroDescricaoErro retorno = client.ValidacaoCentralizadoraPagamento(numCentralizadora, numCNPJ, codGrupoRamo, codRamoAtividade, codBanco, codAgencia, contaCorrente);
                    client.Close();

                    if (retorno.CodErro == 0)
                        return true;

                    return false;
                }
            }
            catch (FaultException<GEDomBancario.ModelosErroServicos>)
            {
                client.Abort();
                return false;
            }
            catch (TimeoutException)
            {
                client.Abort();
                return false;
            }
            catch (CommunicationException)
            {
                client.Abort();
                return false;
            }
            catch (Exception)
            {
                client.Abort();
                return false;
            }
        }

        #endregion
    }
}
