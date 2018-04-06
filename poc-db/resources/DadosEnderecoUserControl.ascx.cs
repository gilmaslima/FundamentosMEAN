using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.DRCepServico;
using Redecard.PN.Credenciamento.Sharepoint.GEFiliaisZonas;
using Redecard.PN.Credenciamento.Sharepoint.WFEnderecos;
using System.Xml;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using Redecard.PN.Credenciamento.Sharepoint.WFTecnologia;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosEndereco
{
    public partial class DadosEnderecoUserControl : UserControlCredenciamentoBase
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
                if (Credenciamento.Fase < 3)
                    Credenciamento.Fase = 3;

                ListItem[] listaEstados = new ListItem[]{ new ListItem(String.Empty),
                        new ListItem("AC"),
                        new ListItem("AL"),
                        new ListItem("AP"),
                        new ListItem("AM"),
                        new ListItem("BA"),
                        new ListItem("CE"),
                        new ListItem("DF"),
                        new ListItem("ES"),
                        new ListItem("GO"),
                        new ListItem("MA"),
                        new ListItem("MT"),
                        new ListItem("MS"),
                        new ListItem("MG"),
                        new ListItem("PA"),
                        new ListItem("PB"),
                        new ListItem("PR"),
                        new ListItem("PE"),
                        new ListItem("PI"),
                        new ListItem("RJ"),
                        new ListItem("RN"),
                        new ListItem("RS"),
                        new ListItem("RO"),
                        new ListItem("RR"),
                        new ListItem("SC"),
                        new ListItem("SP"),
                        new ListItem("SE"),
                        new ListItem("TO")
                    };

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados Endereço";



                if (!Page.IsPostBack)
                {
                    ddlUFComercial.DataSource = listaEstados;
                    ddlUFComercial.DataBind();
                    ddlUFCorrespondencia.DataSource = listaEstados;
                    ddlUFCorrespondencia.DataBind();
                    ddlUFInstalacao.DataSource = listaEstados;
                    ddlUFInstalacao.DataBind();

                    // Carrega controles
                    if (!String.IsNullOrEmpty(Credenciamento.CEP))
                    {
                        txtCEPComercial.Text = Credenciamento.CEP;

                        String cep = txtCEPComercial.Text.Replace("-", "");
                        String endereco = String.Empty;
                        String bairro = String.Empty;
                        String cidade = String.Empty;
                        String uf = String.Empty;

                        BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                        txtEnderecoComercial.Text = endereco;

                        txtBairroComercial.Text = bairro;
                        txtBairroComercial.Enabled = String.IsNullOrEmpty(bairro.Trim());

                        txtCidadeComercial.Text = cidade;
                        txtCidadeComercial.Enabled = String.IsNullOrEmpty(cidade.Trim());

                        ddlUFComercial.SelectedValue = uf.ToUpper();
                        ddlUFComercial.Enabled = String.IsNullOrEmpty(uf.Trim());

                        if (Credenciamento.CEP == Credenciamento.EnderecoComercial.CEP)
                        {
                            if (Credenciamento.EnderecoComercial.Logradouro != null && !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Logradouro.Trim()))
                                txtEnderecoComercial.Text = Credenciamento.EnderecoComercial.Logradouro;

                            txtNumeroComercial.Text = Credenciamento.EnderecoComercial.Numero != null && !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Numero.Trim()) ? Credenciamento.EnderecoComercial.Numero : String.Empty;
                            txtComplementoComercial.Text = Credenciamento.EnderecoComercial.Complemento != null && !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Complemento.Trim()) ? Credenciamento.EnderecoComercial.Complemento : String.Empty;
                        }
                    }

                    if (!String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.CEP))
                    {
                        txtCEPCorrespondencia.Text = Credenciamento.EnderecoCorrespondencia.CEP;

                        String cep = txtCEPCorrespondencia.Text.Replace("-", "");
                        String endereco = String.Empty;
                        String bairro = String.Empty;
                        String cidade = String.Empty;
                        String uf = String.Empty;

                        BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                        txtEnderecoCorrespondencia.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Logradouro) ? Credenciamento.EnderecoCorrespondencia.Logradouro : String.Empty;
                        //txtEnderecoCorrespondencia.Enabled = String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Logradouro) && String.IsNullOrEmpty(endereco) ? true : false;

                        txtNumeroCorrespondencia.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Numero) ? Credenciamento.EnderecoCorrespondencia.Numero : String.Empty;
                        txtComplementoCorrespondencia.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Complemento) ? Credenciamento.EnderecoCorrespondencia.Complemento : String.Empty;

                        txtBairroCorrespondencia.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Bairro) ? Credenciamento.EnderecoCorrespondencia.Bairro : String.Empty;
                        txtBairroCorrespondencia.Enabled = String.IsNullOrEmpty(bairro.Trim());

                        txtCidadeCorrespondencia.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Cidade) ? Credenciamento.EnderecoCorrespondencia.Cidade : String.Empty;
                        txtCidadeCorrespondencia.Enabled = String.IsNullOrEmpty(cidade.Trim());

                        ddlUFCorrespondencia.SelectedValue = !String.IsNullOrEmpty(Credenciamento.EnderecoCorrespondencia.Estado) ? Credenciamento.EnderecoCorrespondencia.Estado.ToUpper() : String.Empty;
                        ddlUFCorrespondencia.Enabled = String.IsNullOrEmpty(uf.Trim());
                    }

                    if (!String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.CEP))
                    {
                        txtCEPInstalacao.Text = Credenciamento.EnderecoInstalacao.CEP;

                        String cep = txtCEPInstalacao.Text.Replace("-", "");
                        String endereco = String.Empty;
                        String bairro = String.Empty;
                        String cidade = String.Empty;
                        String uf = String.Empty;

                        BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                        txtEnderecoInstalacao.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Logradouro) ? Credenciamento.EnderecoInstalacao.Logradouro : String.Empty;
                        //txtEnderecoInstalacao.Enabled = String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Logradouro) && String.IsNullOrEmpty(endereco) ? true : false;

                        txtNumeroInstalacao.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Numero) ? Credenciamento.EnderecoInstalacao.Numero : String.Empty;
                        txtComplementoInstalacao.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Complemento) ? Credenciamento.EnderecoInstalacao.Complemento : String.Empty;

                        txtBairroInstalacao.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Bairro) ? Credenciamento.EnderecoInstalacao.Bairro : String.Empty;
                        txtBairroInstalacao.Enabled = String.IsNullOrEmpty(bairro.Trim());

                        txtCidadeInstalacao.Text = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Cidade) ? Credenciamento.EnderecoInstalacao.Cidade : String.Empty;
                        txtCidadeInstalacao.Enabled = String.IsNullOrEmpty(cidade.Trim());

                        ddlUFInstalacao.SelectedValue = !String.IsNullOrEmpty(Credenciamento.EnderecoInstalacao.Estado) ? Credenciamento.EnderecoInstalacao.Estado.ToUpper() : String.Empty;
                        ddlUFInstalacao.Enabled = String.IsNullOrEmpty(uf.Trim());

                        if (Credenciamento.DiaInicioFuncionamento != 0)
                            ddlDiaFuncionamentoInicio.SelectedValue = Credenciamento.DiaInicioFuncionamento.ToString();

                        if (Credenciamento.DiaFimFuncionamento != 0)
                            ddlDiaFuncionamentoFinal.SelectedValue = Credenciamento.DiaFimFuncionamento.ToString();

                        ddlHorarioFuncionamentoInicio.SelectedValue = Credenciamento.HoraInicioFuncionamento.ToString();
                        ddlHorarioFuncionamentoFinal.SelectedValue = Credenciamento.HoraFimFuncionamento.ToString();

                        //if (Credenciamento.DiaInicioInstalacao != 0)
                        //    ddlDiaInstalacaoInicio.SelectedValue = Credenciamento.DiaInicioInstalacao.ToString();

                        //if (Credenciamento.DiaFimInstalacao != 0)
                        //    ddlDiaInstalacaoFinal.SelectedValue = Credenciamento.DiaFimInstalacao.ToString();

                        //if (Credenciamento.HoraInicioInstalacao != 0)
                        //    ddlHorarioInstalacaoInicio.SelectedValue = Credenciamento.HoraInicioInstalacao.ToString();

                        //if (Credenciamento.HoraFimInstalacao != 0)
                        //    ddlHorarioInstalacaoFinal.SelectedValue = Credenciamento.HoraFimInstalacao.ToString();
                    }

                    if (!String.IsNullOrEmpty(Credenciamento.NomeContatoInstalacao))
                        txtNome.Text = Credenciamento.NomeContatoInstalacao;
                    else
                        txtNome.Text = Credenciamento.PessoaContato.Trim();

                    if (!String.IsNullOrEmpty(Credenciamento.NumDDDInstalacao))
                        txtDDD.Text = Credenciamento.NumDDDInstalacao;
                    else
                        txtDDD.Text = Credenciamento.NumDDD1;

                    if (Credenciamento.NumTelefoneInstalacao != 0)
                        txtNumero.Text = Credenciamento.NumTelefoneInstalacao.ToString();
                    else
                        txtNumero.Text = Credenciamento.NumTelefone1.ToString().Trim();

                    if (Credenciamento.RamalInstalacao != 0)
                        txtRamal.Text = Credenciamento.RamalInstalacao.ToString();
                    else
                        txtRamal.Text = Credenciamento.Ramal1.ToString().Trim();

                    if (!String.IsNullOrEmpty(Credenciamento.Observacao))
                    {
                        //int obsIndex = Credenciamento.Observacao.LastIndexOf("#OBS:");
                        //if (obsIndex != -1)
                        //{
                        //    txtPontoReferencia.Text = Credenciamento.Observacao.Substring(0, obsIndex).Trim().Replace("#PTREF:", "");
                        //    txtObs.Text = Credenciamento.Observacao.Substring(obsIndex).Replace("#OBS:", "");
                        //}
                        //else
                        txtObs.Text = Credenciamento.Observacao;
                    }

                    if ((!String.IsNullOrEmpty(txtCEPCorrespondencia.Text) && txtCEPComercial.Text != txtCEPCorrespondencia.Text) || txtNumeroComercial.Text != txtNumeroCorrespondencia.Text)
                    {
                        chkCorrEndComercial.Checked = false;
                        pnlCorrespondencia.Visible = !pnlCorrespondencia.Visible;
                    }

                    if (Credenciamento.EndInstIgualComer == 'N')
                    {
                        chkInstEndComercial.Checked = false;
                        pnlInstalacao.Visible = !pnlInstalacao.Visible;
                    }

                    chkEstabShop.Checked = Credenciamento.IndRegiaoLoja == 'S' ? true : false;

                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        chkInstEndComercial.Enabled = false;
                        txtNome.Enabled = false;
                        txtDDD.Enabled = false;
                        txtNumero.Enabled = false;
                        txtRamal.Enabled = false;
                        txtObs.Enabled = false;
                        //txtPontoReferencia.Enabled = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
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
                Response.Redirect("pn_dadosoperacionais.aspx", false);
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
            Response.Redirect("pn_dadosnegocio.aspx", false);
        }

        /// <summary>
        /// Esconde / Mostra painel de Endereço de Correspondência
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkCorrEndComercial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pnlCorrespondencia.Visible = !pnlCorrespondencia.Visible;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Esconde / Mostra painel de Endereço de Instalação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkInstEndComercial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pnlInstalacao.Visible = !pnlInstalacao.Visible;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP Comercial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCEPComercial_TextChanged(object sender, EventArgs e)
        {
            try
            {
                String cep = txtCEPComercial.Text.Replace("-", "");
                String endereco = String.Empty;
                String bairro = String.Empty;
                String cidade = String.Empty;
                String uf = String.Empty;

                BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                txtEnderecoComercial.Text = endereco;
                //txtEnderecoComercial.Enabled = String.IsNullOrEmpty(endereco);

                txtBairroComercial.Text = bairro;
                txtBairroComercial.Enabled = String.IsNullOrEmpty(bairro);

                txtCidadeComercial.Text = cidade;
                txtCidadeComercial.Enabled = String.IsNullOrEmpty(cidade);

                ddlUFComercial.SelectedValue = uf.ToUpper();
                ddlUFComercial.Enabled = String.IsNullOrEmpty(uf);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP Correspondência
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCEPCorrespondencia_TextChanged(object sender, EventArgs e)
        {
            try
            {
                String cep = txtCEPCorrespondencia.Text.Replace("-", "");
                String endereco = String.Empty;
                String bairro = String.Empty;
                String cidade = String.Empty;
                String uf = String.Empty;

                BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                txtEnderecoCorrespondencia.Text = endereco;
                //txtEnderecoCorrespondencia.Enabled = String.IsNullOrEmpty(endereco);

                txtBairroCorrespondencia.Text = bairro;
                txtBairroCorrespondencia.Enabled = String.IsNullOrEmpty(bairro);

                txtCidadeCorrespondencia.Text = cidade;
                txtCidadeCorrespondencia.Enabled = String.IsNullOrEmpty(cidade);

                ddlUFCorrespondencia.SelectedValue = uf;
                ddlUFCorrespondencia.Enabled = String.IsNullOrEmpty(uf);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP Instalação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCEPInstalacao_TextChanged(object sender, EventArgs e)
        {
            try
            {
                String cep = txtCEPInstalacao.Text.Replace("-", "");
                String endereco = String.Empty;
                String bairro = String.Empty;
                String cidade = String.Empty;
                String uf = String.Empty;

                BuscarLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

                txtEnderecoInstalacao.Text = endereco;
                //txtEnderecoInstalacao.Enabled = String.IsNullOrEmpty(endereco);

                txtBairroInstalacao.Text = bairro;
                txtBairroInstalacao.Enabled = String.IsNullOrEmpty(bairro);

                txtCidadeInstalacao.Text = cidade;
                txtCidadeInstalacao.Enabled = String.IsNullOrEmpty(cidade);

                ddlUFInstalacao.SelectedValue = uf;
                ddlUFInstalacao.Enabled = String.IsNullOrEmpty(uf);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca informações do logradouro de acordo com o CEP
        /// </summary>
        private void BuscarLogradouro(String cep, ref String endereco, ref String bairro, ref String cidade, ref String uf)
        {
            Int32 retorno = 0;

            using (var log = Logger.IniciarLog("Carregar Dados Credenciamento - Buscar Logradouro"))
            {

                log.GravarLog(EventoLog.ChamadaServico, new { cep, endereco, bairro, cidade, uf });
                SharePointUlsLog.LogMensagem(String.Format("Credenciamento, Buscar Logradouro - Parâmetros de entrada:"));
                SharePointUlsLog.LogMensagem(String.Format("CEP: {0}", cep));
                SharePointUlsLog.LogMensagem(String.Format("Endereço: {0}", endereco));
                SharePointUlsLog.LogMensagem(String.Format("Bairro: {0}", bairro));
                SharePointUlsLog.LogMensagem(String.Format("Cidade: {0}", cidade));
                SharePointUlsLog.LogMensagem(String.Format("Uf: {0}", uf));

                try
                {
                    using (var contexto = new ContextoWCF<DRCepServicoClient>())
                    {
                        retorno = contexto.Cliente.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
                    }
                }
                catch (FaultException<DRCepServico.GeneralFault> fe)
                {
                    Logger.GravarErro("Credenciamento - Dados Endereço", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Message, "");
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Credenciamento - Dados Endereço", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(te.Message, 300);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Credenciamento - Dados Endereço", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(ce.Message, 300);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, endereco, bairro, cidade, uf });
                SharePointUlsLog.LogMensagem(String.Format("Credenciamento, Buscar Logradouro - Parâmetros de saída:"));
                SharePointUlsLog.LogMensagem(String.Format("Retorno: {0}", retorno));
                SharePointUlsLog.LogMensagem(String.Format("Endereço: {0}", endereco));
                SharePointUlsLog.LogMensagem(String.Format("Bairro: {0}", bairro));
                SharePointUlsLog.LogMensagem(String.Format("Cidade: {0}", cidade));
                SharePointUlsLog.LogMensagem(String.Format("Uf: {0}", uf));
            }
        }

        /// <summary>
        /// Buscar dados de filiais
        /// </summary>
        private void BuscarDadosFiliais()
        {
            ServicoPortalGEFiliaisZonasClient client = new ServicoPortalGEFiliaisZonasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Dados Endereço - Buscar Dados Filiais"))
                {
                    Char tipoOperacao = 'F';
                    String codCEP = txtCEPComercial.Text.Replace("-", "");
                    Char codTipoCep = chkEstabShop.Checked ? 'S' : 'R';
                    Int32? codFilial = null;

                    FiliaisZonasListaDadosCadastrais[] filiais = client.ListaDadosCadastrais(tipoOperacao, codCEP, codTipoCep, codFilial);
                    client.Close();

                    if (filiais.Length > 0)
                    {
                        Credenciamento.CodFilial = filiais[0].CodFilial;
                        Credenciamento.NomeFilial = filiais[0].NomeFilial;
                        Credenciamento.CodZonaVenda = filiais[0].CodZonaVenda;
                        Credenciamento.NomeZonaVenda = filiais[0].NomeZonaVenda;
                    }

                }
            }
            catch (FaultException<GEFiliaisZonas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Endereço", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Message, "");
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Endereço", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Endereço", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da quarta tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso4()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Iniciais"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    PNTransicoesServico.Tecnologia tecnologia = PreencheTecnologia();
                    PNTransicoesServico.Endereco enderecoComercial = PreencheEndereco(Credenciamento.EnderecoComercial);
                    PNTransicoesServico.Endereco enderecoCorrespondencia = PreencheEndereco(Credenciamento.EnderecoCorrespondencia);
                    PNTransicoesServico.Endereco enderecoInstalacao = PreencheEndereco(Credenciamento.EnderecoInstalacao);

                    Int32 retorno = client.GravarAtualizarPasso4(proposta, tecnologia, enderecoComercial, enderecoCorrespondencia, enderecoInstalacao);
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
                    BuscarDadosFiliais();

                    Credenciamento.DiaInicioFuncionamento = ddlDiaFuncionamentoInicio.SelectedValue.ToInt32();
                    Credenciamento.DiaFimFuncionamento = ddlDiaFuncionamentoFinal.SelectedValue.ToInt32();
                    Credenciamento.HoraInicioFuncionamento = ddlHorarioFuncionamentoInicio.SelectedValue.ToInt32();
                    Credenciamento.HoraFimFuncionamento = ddlHorarioFuncionamentoFinal.SelectedValue.ToInt32();
                    //Credenciamento.DiaInicioInstalacao = ddlDiaInstalacaoInicio.SelectedValue.ToInt32();
                    //Credenciamento.DiaFimInstalacao = ddlDiaInstalacaoFinal.SelectedValue.ToInt32();
                    //Credenciamento.HoraInicioInstalacao = ddlHorarioInstalacaoInicio.SelectedValue.ToInt32();
                    //Credenciamento.HoraFimInstalacao = ddlHorarioInstalacaoFinal.SelectedValue.ToInt32();
                    Credenciamento.NomeContatoInstalacao = txtNome.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.NumDDDInstalacao = txtDDD.Text;
                    Credenciamento.NumTelefoneInstalacao = txtNumero.Text.ToInt32();
                    Credenciamento.RamalInstalacao = txtRamal.Text.ToInt32();
                    //Credenciamento.Observacao = String.Format("#PTREF:{0} #OBS:{1}", txtPontoReferencia.Text.Trim(), txtObs.Text.Trim());
                    Credenciamento.Observacao = txtObs.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim();
                    Credenciamento.IndRegiaoLoja = chkEstabShop.Checked ? 'S' : 'R';

                    Credenciamento.EnderecoComercial = new Modelo.Endereco();
                    Credenciamento.EnderecoComercial.IndTipoEndereco = '1';
                    Credenciamento.EnderecoComercial.CEP = txtCEPComercial.Text;
                    Credenciamento.EnderecoComercial.Logradouro = txtEnderecoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.EnderecoComercial.Complemento = txtComplementoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.EnderecoComercial.Numero = txtNumeroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                    Credenciamento.EnderecoComercial.Bairro = txtBairroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.EnderecoComercial.Cidade = txtCidadeComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    Credenciamento.EnderecoComercial.Estado = ddlUFComercial.SelectedValue.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();

                    Credenciamento.EnderecoCorrespondencia = new Modelo.Endereco();
                    Credenciamento.EnderecoCorrespondencia.IndTipoEndereco = '2';
                    if (!chkCorrEndComercial.Checked)
                    {
                        Credenciamento.EnderecoCorrespondencia.CEP = txtCEPCorrespondencia.Text;
                        Credenciamento.EnderecoCorrespondencia.Logradouro = txtEnderecoCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Complemento = txtComplementoCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Numero = txtNumeroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.EnderecoCorrespondencia.Bairro = txtBairroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Cidade = txtCidadeCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Estado = ddlUFCorrespondencia.SelectedValue.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    }
                    else
                    {
                        Credenciamento.EnderecoCorrespondencia.CEP = txtCEPComercial.Text;
                        Credenciamento.EnderecoCorrespondencia.Logradouro = txtEnderecoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Complemento = txtComplementoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Numero = txtNumeroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.EnderecoCorrespondencia.Bairro = txtBairroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Cidade = txtCidadeComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoCorrespondencia.Estado = ddlUFComercial.SelectedValue.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    }

                    Credenciamento.EnderecoInstalacao = new Modelo.Endereco();
                    Credenciamento.EnderecoInstalacao.IndTipoEndereco = '4';
                    if (!chkInstEndComercial.Checked)
                    {
                        Credenciamento.EndInstIgualComer = 'N';
                        Credenciamento.EnderecoInstalacao.CEP = txtCEPInstalacao.Text;
                        Credenciamento.EnderecoInstalacao.Logradouro = txtEnderecoInstalacao.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Complemento = txtComplementoInstalacao.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Numero = txtNumeroInstalacao.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.EnderecoInstalacao.Bairro = txtBairroInstalacao.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Cidade = txtCidadeInstalacao.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Estado = ddlUFInstalacao.SelectedValue.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    }
                    else
                    {
                        Credenciamento.EndInstIgualComer = 'S';
                        Credenciamento.EnderecoInstalacao.CEP = txtCEPComercial.Text;
                        Credenciamento.EnderecoInstalacao.Logradouro = txtEnderecoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Complemento = txtComplementoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Numero = txtNumeroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.EnderecoInstalacao.Bairro = txtBairroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Cidade = txtCidadeComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                        Credenciamento.EnderecoInstalacao.Estado = ddlUFComercial.SelectedValue.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces();
                    }
                    return GravarAtualizarPasso4();
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
                Logger.GravarErro("Credenciamento - Dados Endereço", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
                return (Int32)fe.Detail.CodErro;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ce);
                SharePointUlsLog.LogErro(ce);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Endereço", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        #endregion
    }
}
