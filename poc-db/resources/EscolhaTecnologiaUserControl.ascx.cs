using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.TGForSoftware;
using Redecard.PN.Credenciamento.Sharepoint.TGEventosEsp;
using Redecard.PN.Credenciamento.Sharepoint.TGAcaoCom;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.WFTecnologia;
using Redecard.PN.Credenciamento.Sharepoint.TGFabHardware;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.EscolhaTecnologia
{
    public partial class EscolhaTecnologiaUserControl : UserControlCredenciamentoBase
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
                if (Credenciamento.Fase < 6)
                    Credenciamento.Fase = 6;
                Page.MaintainScrollPositionOnPostBack = true;

                if (!Page.IsPostBack)
                {
                    txtTipoEquipamento.Text = Credenciamento.TipoEquipamento;
                    txtValorAluguel.Text = String.Format("{0:C}", Credenciamento.ValorAluguelTela);

                    CarregarEventos();
                    CarregarAcaoComercial();

                    // Carregar dados já salvos
                    txtQuantidade.Text = Credenciamento.QtdeTerminaisSolicitados != 0 ? Credenciamento.QtdeTerminaisSolicitados.ToString() : txtQuantidade.Text;
                    ddlEvento.SelectedValue = !String.IsNullOrEmpty(Credenciamento.CodEvento) ? Credenciamento.CodEvento : ddlEvento.SelectedValue;

                    if (String.Compare(Credenciamento.TipoEquipamento, "PDV") == 0)
                    {
                        CarregarSoftwareTEF();
                        CarregarMarcaPDV();
                        pnlPDV.Visible = true;

                        txtRenpac.Text = Credenciamento.NroRenpac != 0 ? Credenciamento.NroRenpac.ToString() : txtRenpac.Text;
                        ddlMarcaPDV.SelectedValue = !String.IsNullOrEmpty(Credenciamento.CodMarcaPDV) ? Credenciamento.CodMarcaPDV : ddlMarcaPDV.SelectedValue;
                        ddlSoftwareTEF.SelectedValue = !String.IsNullOrEmpty(Credenciamento.CodSoftwareTEF) ? Credenciamento.CodSoftwareTEF : ddlSoftwareTEF.SelectedValue;
                    }

                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        txtQuantidade.Text = "1";
                        txtQuantidade.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Int32 codRetorno = SalvarDados();
            if (codRetorno == 0)
                Response.Redirect("pn_contracaoservicos.aspx", false);
            else if (codRetorno != 399)
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
            Response.Redirect("pn_dadosbancarios.aspx", false);
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de eventos e carrega drop down
        /// </summary>
        private void CarregarEventos()
        {
            ServicoPortalTGEventosEspeciaisClient client = new ServicoPortalTGEventosEspeciaisClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Eventos"))
                {
                    EventosEspeciais[] eventos = client.ListaDadosCadastrais(null, 'A', null);
                    client.Close();

                    ddlEvento.Items.Clear();
                    ddlEvento.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (EventosEspeciais evento in eventos)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", evento.CodEventoEspecial, evento.NomeEventoEspecial), evento.CodEventoEspecial);
                        ddlEvento.Items.Add(item);
                    }

                }
            }
            catch (FaultException<TGEventosEsp.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de softwares TEF e carrega drop down
        /// </summary>
        private void CarregarSoftwareTEF()
        {
            ServicoPortalTGFornecedorSoftwareClient client = new ServicoPortalTGFornecedorSoftwareClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Softwares TEF"))
                {
                    FornecedorSoftware[] softwares = client.ListaDadosCadastrais(null, 'A');
                    client.Close();

                    ddlSoftwareTEF.Items.Clear();
                    ddlSoftwareTEF.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (FornecedorSoftware software in softwares)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", software.CodFornecedorSoftware, software.NomeFornecedorSoftware), software.CodFornecedorSoftware);
                        ddlSoftwareTEF.Items.Add(item);
                    }

                }
            }
            catch (FaultException<TGForSoftware.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Marcas e carrega drop down
        /// </summary>
        private void CarregarMarcaPDV()
        {
            ServicoPortalTGFabricanteHardwareClient client = new ServicoPortalTGFabricanteHardwareClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Softwares TEF"))
                {
                    FabricanteHardware[] hardwares = client.ListaDadosCadastrais(null, 'A');
                    client.Close();

                    ddlMarcaPDV.Items.Clear();
                    ddlMarcaPDV.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (FabricanteHardware hardware in hardwares)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", hardware.CodFabricanteHardware, hardware.NomeFabricanteHardware), hardware.CodFabricanteHardware);
                        ddlMarcaPDV.Items.Add(item);
                    }

                }
            }
            catch (FaultException<TGFabHardware.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de ações comerciais e carrega drop down
        /// </summary>
        private void CarregarAcaoComercial()
        {
            ServicoPortalTGAcaoComercialClient client = new ServicoPortalTGAcaoComercialClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ação Comercial"))
                {
                        ListaDadosCadastrais[] acoes = client.ListaDadosCadastrais(null, 'A');
                        client.Close();

                        ddlAcaoComercial.Items.Clear();
                        ddlAcaoComercial.Items.Add(new ListItem(String.Empty, String.Empty));
                        foreach (ListaDadosCadastrais acao in acoes)
                        {
                            ListItem item = new ListItem(String.Format("{0} - {1}", acao.CodAcaoComercial, acao.DescAcaoComercial), acao.CodAcaoComercial.ToString());
                            ddlAcaoComercial.Items.Add(item);
                        }
                }
            }
            catch (FaultException<TGAcaoCom.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Consulta valor da Venda Digitada
        /// </summary>
        private void ConsultaVendaDigitadaPorRamoAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Venda Digitada Por Ramo Atividade"))
                {
                        Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                        Int32 codRamoAtivididade = Credenciamento.RamoAtividade;
                        Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];

                        ConsultaVendaDigitadaPorRamoAtividade[] vendasDigitada = client.ConsultaVendaDigitadaPorRamoAtividade(codGrupoRamo, codRamoAtivididade, codTipoPessoa);
                        client.Close();

                        if (vendasDigitada.Length > 0)
                            Credenciamento.IndVendaDigitada = vendasDigitada[0].IndVendaDigitada;
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da sétima tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso7()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Escolha Tecnologia"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    PNTransicoesServico.Tecnologia tecnologia = PreencheTecnologia();

                    Int32 retorno = client.GravarAtualizarPasso7(proposta, tecnologia);
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
                    Credenciamento.QtdeTerminaisSolicitados = txtQuantidade.Text.ToInt32();
                    Credenciamento.CodEvento = ddlEvento.SelectedValue;
                    Credenciamento.AcaoComercial = !String.IsNullOrEmpty(ddlAcaoComercial.SelectedValue) ? ddlAcaoComercial.SelectedValue.ToInt32Null() : null;

                    ConsultaVendaDigitadaPorRamoAtividade();

                    if (String.Compare(Credenciamento.TipoEquipamento, "PDV") == 0)
                    {
                        Credenciamento.CodMarcaPDV = ddlMarcaPDV.SelectedValue;
                        Credenciamento.NomeMarcaPDV = ddlMarcaPDV.SelectedItem.Text;
                        Credenciamento.CodSoftwareTEF = ddlSoftwareTEF.SelectedValue;
                        Credenciamento.NomeSoftwareTEF = ddlSoftwareTEF.SelectedItem.Text;
                        Credenciamento.NroRenpac = !String.IsNullOrEmpty(txtRenpac.Text) ? txtRenpac.Text.ToInt32() : 0;
                    }

                    return GravarAtualizarPasso7();
                }

                return 399;
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                return fe.Detail.Codigo;
            }
            catch (FaultException<GETaxaFiliacao.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
                return (Int32)fe.Detail.CodErro;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Escolha Tecnologia", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        #endregion
    }
}