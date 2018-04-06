using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Data;
using System.Collections.Generic;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using Redecard.PN.Credenciamento.Sharepoint.GEEmpVenDir;
using Redecard.PN.Credenciamento.Sharepoint.WFSerasa;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.WFProprietarios;
using System.ServiceModel;
using System.Linq;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using Redecard.PN.Credenciamento.Sharepoint.GEPontoVen;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosCliente
{
    public partial class DadosClienteUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Lista de Proprietários
        /// </summary>
        public List<Modelo.Proprietario> Proprietarios
        {
            get
            {
                if (ViewState["Proprietarios"] == null)
                    ViewState["Proprietarios"] = new List<Modelo.Proprietario>();

                return (List<Modelo.Proprietario>)ViewState["Proprietarios"];
            }
            set
            {
                ViewState["Proprietarios"] = value;
            }
        }

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
                if (Credenciamento.Fase < 1)
                    Credenciamento.Fase = 1;

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados do Cliente";
                Page.Culture = "en-GB";

                if (!Page.IsPostBack)
                {
                    Page.DataBind();
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                    {
                        pnlPessoaFisica.Visible = false;
                        pnlPessoaJuridica.Visible = true;

                        CarregarSerasa();
                    }
                    else
                    {
                        pnlPessoaFisica.Visible = true;
                        pnlPessoaJuridica.Visible = false;
                        pnlProprietarios.Visible = false;
                    }

                    if (String.Compare(Credenciamento.TipoComercializacao, "00000") == 0 &&
                        !String.IsNullOrEmpty(Credenciamento.CNAE) && String.Compare(Credenciamento.RetornoSerasa, "00") == 0)
                    {
                        CarregarNroRamo();
                    }

                    CarregarRamoAtuacao();

                    if (String.Compare(Credenciamento.RetornoSerasa, "00") != 0 || String.Compare(Credenciamento.TipoComercializacao, "00000") != 0)
                    {
                        if (String.Compare(Credenciamento.TipoComercializacao, "00000") != 0)
                        {
                            Credenciamento.GrupoRamo = Credenciamento.TipoComercializacao.Substring(0, 1).ToInt32();
                            Credenciamento.RamoAtividade = Credenciamento.TipoComercializacao.Substring(1, 4).ToInt32();
                        }

                        if (Credenciamento.GrupoRamo != 0)
                        {
                            ddlRamoAtuacao.SelectedValue = Credenciamento.GrupoRamo.ToString();
                            CarregarRamoAtividade();
                        }
                        if (Credenciamento.RamoAtividade != 0)
                        {
                            ddlRamoAtividade.SelectedValue = String.Format(@"{0:0000}", Credenciamento.RamoAtividade);
                        }

                        if (String.Compare(Credenciamento.TipoComercializacao, "00000") != 0)
                        {
                            ddlRamoAtuacao.Enabled = false;
                            ddlRamoAtividade.Enabled = false;
                        }

                        if (Credenciamento.RamoAtividade != 0 && Credenciamento.GrupoRamo != 0)
                            MontaNroRamo();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Credenciamento.CNAE))
                            CarregarNroRamo();

                        if (!String.IsNullOrEmpty(txtNroRamo.Text))
                        {
                            if (Credenciamento.GrupoRamo == 0 && Credenciamento.RamoAtividade == 0)
                            {
                                ddlRamoAtuacao.SelectedValue = txtNroRamo.Text.Substring(0, 1);
                                //ddlRamoAtuacao.Enabled = false;
                                CarregarRamoAtividade();
                                ddlRamoAtividade.SelectedValue = txtNroRamo.Text.Substring(1, 4);
                            }
                            else
                            {
                                ddlRamoAtuacao.SelectedValue = Credenciamento.GrupoRamo.ToString();
                                //ddlRamoAtuacao.Enabled = false;
                                CarregarRamoAtividade(Credenciamento.GrupoRamo, null);
                                ddlRamoAtividade.SelectedValue = String.Format(@"{0:0000}", Credenciamento.RamoAtividade);
                            }
                        }
                    }

                    CarregarEDV();
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                    {
                        txtRazaoSocial.Text = !String.IsNullOrEmpty(Credenciamento.RazaoSocial) ? Credenciamento.RazaoSocial.Trim() : txtRazaoSocial.Text;
                        txtDataFundacao.Text = Credenciamento.DataFundacao != default(DateTime) && Credenciamento.DataFundacao != new DateTime(1900, 1, 1) ? Credenciamento.DataFundacao.ToString("dd/MM/yyyy") : txtDataFundacao.Text;

                        if (Credenciamento.RecuperadaGE)
                        {
                            txtRazaoSocial.Enabled = false;
                        }
                    }
                    else
                    {
                        txtNomeCompleto.Text = !String.IsNullOrEmpty(Credenciamento.NomeCompleto) ? Credenciamento.NomeCompleto.Trim() : txtNomeCompleto.Text;
                        txtDataNascimento.Text = Credenciamento.DataNascimento != default(DateTime) && Credenciamento.DataNascimento != new DateTime(1900, 1, 1) ? Credenciamento.DataNascimento.ToString("dd/MM/yyyy") : txtDataNascimento.Text;

                        if (Credenciamento.RecuperadaGE)
                        {
                            txtNomeCompleto.Enabled = false;
                        }
                    }

                    txtCNAE.Text = !String.IsNullOrEmpty(Credenciamento.CNAE) ? Credenciamento.CNAE : txtCNAE.Text;
                    txtNome.Text = !String.IsNullOrEmpty(Credenciamento.PessoaContato) ? Credenciamento.PessoaContato.Trim() : txtNome.Text;
                    txtEmail.Text = !String.IsNullOrEmpty(Credenciamento.NomeEmail) ? Credenciamento.NomeEmail.Trim() : txtEmail.Text;
                    txtSite.Text = !String.IsNullOrEmpty(Credenciamento.NomeHomePage) ? Credenciamento.NomeHomePage.Trim() : txtSite.Text;
                    txtTel1DDD.Text = !String.IsNullOrEmpty(Credenciamento.NumDDD1) ? Credenciamento.NumDDD1 : txtTel1DDD.Text;
                    txtTel1Numero.Text = Credenciamento.NumTelefone1 != null && Credenciamento.NumTelefone1 != 0 ? Credenciamento.NumTelefone1.ToString() : txtTel1Numero.Text;
                    txtTel1Ramal.Text = Credenciamento.Ramal1 != null && Credenciamento.Ramal1 != 0 ? Credenciamento.Ramal1.ToString() : txtTel1Ramal.Text;
                    txtFaxDDD.Text = !String.IsNullOrEmpty(Credenciamento.NumDDDFax) ? Credenciamento.NumDDDFax : txtFaxDDD.Text;
                    txtFaxNumero.Text = Credenciamento.NumTelefoneFax != null && Credenciamento.NumTelefoneFax != 0 ? Credenciamento.NumTelefoneFax.ToString() : txtFaxNumero.Text;
                    txtTel2DDD.Text = !String.IsNullOrEmpty(Credenciamento.NumDDD2) ? Credenciamento.NumDDD2 : txtTel2DDD.Text;
                    txtTel2Numero.Text = Credenciamento.NumTelefone2 != null && Credenciamento.NumTelefone2 != 0 ? Credenciamento.NumTelefone2.ToString() : txtTel2Numero.Text;
                    txtTel2Ramal.Text = Credenciamento.Ramal2 != null && Credenciamento.Ramal2 != 0 ? Credenciamento.Ramal2.ToString() : txtTel2Ramal.Text;
                    if (Credenciamento.EDV != null && Credenciamento.EDV != 0)
                    {
                        ddlEDV.Enabled = true;
                        ddlEDV.SelectedValue = Credenciamento.EDV.ToString();
                        chkEDV.Checked = true;
                    }

                    chkAtivarExtratoEmail.Checked = rfvEmail.Visible = Credenciamento.IndExtratoEmail != null && Credenciamento.IndExtratoEmail == 'S' ? true : false;

                    if (Credenciamento.RecuperadaGE || String.Compare(Credenciamento.RetornoSerasa, "00") == 0)
                    {
                        btnAdicionarProprietario.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
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
                Response.Redirect("pn_dadosnegocio.aspx", false);
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

        /// <summary>
        /// Evento do botão Incluir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("vgSocio");

                if (Page.IsValid)
                {
                    Int32 codigoErro = 0;

                    if ((ddlTipoPessoa.SelectedValue == "J" && Proprietarios.FirstOrDefault(p => String.Compare(p.CPF_CNPJ, txtCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", "")) == 0) != null)
                        || (ddlTipoPessoa.SelectedValue == "F" && Proprietarios.FirstOrDefault(p => String.Compare(p.CPF_CNPJ, txtCPF.Text.Replace(".", "").Replace("-", "")) == 0) != null))
                        codigoErro = 300;

                    if (codigoErro == 0)
                    {
                        Proprietarios.Add(new Modelo.Proprietario()
                        {
                            TipoPessoa = ddlTipoPessoa.SelectedValue,
                            CPF_CNPJ = String.Compare(ddlTipoPessoa.SelectedValue, "J") == 0 ? txtCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", "") : txtCPF.Text.Replace(".", "").Replace("-", ""),
                            Participacao = txtPartAcionaria.Text,
                            Nome = txtNomeProprietario.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper(),
                            Relato = ""
                        });

                        gvDadosProprietarios.DataSource = Proprietarios;
                        gvDadosProprietarios.DataBind();

                        //Limpa controles
                        txtCNPJ.Text = String.Empty;
                        txtCPF.Text = String.Empty;
                        txtPartAcionaria.Text = String.Empty;
                        txtNomeProprietario.Text = String.Empty;

                        pnlInclusaoSocio.Visible = !pnlInclusaoSocio.Visible;
                        pnlAdicionarBtn.Visible = !pnlAdicionarBtn.Visible;
                    }
                    else
                        base.ExibirPainelExcecao("Proprietário já incluído na proposta.", codigoErro.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

        }

        /// <summary>
        /// Evento do botão cancelar inclusão do proprietário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                pnlInclusaoSocio.Visible = !pnlInclusaoSocio.Visible;
                pnlAdicionarBtn.Visible = !pnlAdicionarBtn.Visible;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_dadosiniciais.aspx", false);
        }

        /// <summary>
        /// Evento do botão Adicionar Proprietários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdicionarProprietario_Click(object sender, EventArgs e)
        {
            try
            {
                pnlInclusaoSocio.Visible = !pnlInclusaoSocio.Visible;
                pnlAdicionarBtn.Visible = !pnlAdicionarBtn.Visible;

            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento do botão excluir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (Proprietarios.Count > 1)
                {
                    ImageButton btn = (ImageButton)sender;
                    GridViewRow gvRow = (GridViewRow)btn.NamingContainer;

                    WFProprietarios.RetornoErro erro = new WFProprietarios.RetornoErro();
                    erro.CodigoErro = 0;

                    if (Credenciamento.NumSequencia != null)
                        erro = ExcluirProprietario(Proprietarios[gvRow.RowIndex]);

                    if (erro.CodigoErro == 0 || erro.CodigoErro == 30003)
                    {
                        Proprietarios.RemoveAt(gvRow.RowIndex);
                        gvDadosProprietarios.DataSource = Proprietarios;
                        gvDadosProprietarios.DataBind();
                    }
                    else
                        base.ExibirPainelExcecao(erro.DescricaoErro, erro.CodigoErro.ToString());
                }
                else
                    base.ExibirPainelExcecao("Não é permitido excluir proprietário caso ele seja único", "300");
            }
            catch (FaultException<WFProprietarios.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento que muda o campo de cpf para cnpj de acordo com o tipo de pessoa escolhida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTipoPessoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.Compare(((DropDownList)sender).SelectedValue, "F") == 0)
            {
                txtCPF.Visible = true;
                cvCPF.Visible = true;
                rfvCPF.Visible = true;

                txtCNPJ.Visible = false;
                cvCNPJ.Visible = false;
                rfvCNPJ.Visible = false;
            }
            else
            {
                txtCPF.Visible = false;
                cvCPF.Visible = false;
                rfvCPF.Visible = false;

                txtCNPJ.Visible = true;
                cvCNPJ.Visible = true;
                rfvCNPJ.Visible = true;
            }
        }

        /// <summary>
        /// Evento que habilita e desabilita 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkEDV_CheckedChanged(object sender, EventArgs e)
        {
            ddlEDV.Enabled = !ddlEDV.Enabled;
            rfvEDV.Enabled = !rfvEDV.Enabled;
        }

        /// <summary>
        /// Preenche grid de proprietários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvDadosProprietarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (String.Compare(((Modelo.Proprietario)e.Row.DataItem).TipoPessoa, "J") == 0)
                {
                    ((Label)e.Row.FindControl("lblTipoPessoa")).Text = "Júridica";
                    ((Label)e.Row.FindControl("lblCNPJ")).Text = String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(((Modelo.Proprietario)e.Row.DataItem).CPF_CNPJ));
                    ((Label)e.Row.FindControl("lblCPF")).Visible = false;
                }
                else
                {
                    ((Label)e.Row.FindControl("lblTipoPessoa")).Text = "Física";
                    ((Label)e.Row.FindControl("lblCPF")).Text = String.Format(@"{0:000\.000\.000\-00}", long.Parse(((Modelo.Proprietario)e.Row.DataItem).CPF_CNPJ));
                    ((Label)e.Row.FindControl("lblCNPJ")).Visible = false;
                }

                ((Label)e.Row.FindControl("lblNomeProprietario")).Text = ((Modelo.Proprietario)e.Row.DataItem).Nome;
                ((Label)e.Row.FindControl("lblPartAcionaria")).Text = ((Modelo.Proprietario)e.Row.DataItem).Participacao;
                //((Label)e.Row.FindControl("lblRelato")).Text = ((Modelo.Proprietario)e.Row.DataItem).Relato;

                if (Credenciamento.RecuperadaGE || String.Compare(Credenciamento.RetornoSerasa, "00") == 0 || Credenciamento.CodTipoEstabelecimento == 1)
                {
                    ((ImageButton)e.Row.FindControl("btnExcluir")).Enabled = false;
                }
            }
        }

        /// <summary>
        /// Monta o número de Ramo ao trocar o valor do Ramo de Atividade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRamoAtividade_SelectedIndexChanged(object sender, EventArgs e)
        {
            MontaNroRamo();
        }

        /// <summary>
        /// Ramo de Atuação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRamoAtuacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNroRamo.Text = String.Empty;
            CarregarRamoAtividade();
        }

        /// <summary>
        /// Ativa e desativa validação do campo email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkAtivarExtratoEmail_CheckedChanged(object sender, EventArgs e)
        {
            if (rfvEmail.Visible)
                rfvEmail.Visible = false;
            else
                rfvEmail.Visible = true;
        }

        /// <summary>
        /// Caso o usuário digite o email ativa o checkbox extrato email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(((TextBox)sender).Text))
            {
                chkAtivarExtratoEmail.Checked = true;
                rfvEmail.Visible = true;
            }
        }

        /// <summary>
        /// Valida Dados Telefone 2
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvTel2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaTelefone(txtTel2DDD.Text, txtTel2Numero.Text);
        }

        /// <summary>
        /// Valida Dados Fax
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvFax_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaTelefone(txtFaxDDD.Text, txtFaxNumero.Text);
        }

        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCNPJ_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCNPJ.Text.Replace(".", "").Replace("-", "").Replace("/", "").IsValidCNPJ();
        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCPF_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCPF.Text.Replace(".", "").Replace("-", "").IsValidCPF();
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de Ramos de Atuação e carrega dropdown
        /// </summary>
        private void CarregarRamoAtuacao()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ramos de Atuação"))
                {
                    RamosAtividadesListaDadosCadastraisGruposRamosAtividades[] ramos = client.ListaDadosCadastraisGruposRamosAtividades();
                    client.Close();

                    ddlRamoAtuacao.Items.Clear();
                    ddlRamoAtuacao.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (RamosAtividadesListaDadosCadastraisGruposRamosAtividades ramo in ramos.OrderBy(r => r.DescrRamoAtividade))
                    {
                        ListItem item = new ListItem(ramo.DescrRamoAtividade, ramo.CodGrupoRamoAtividade.ToString());
                        ddlRamoAtuacao.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Ramos de Atividade e carrega o dropdown
        /// </summary>
        private void CarregarRamoAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ramos de Atividades"))
                {
                    Int32? codGrupoRamo = !String.IsNullOrEmpty(txtNroRamo.Text) ? txtNroRamo.Text.Substring(0, 1).ToInt32Null() : ddlRamoAtuacao.SelectedValue.ToInt32Null();
                    Int32? codRamoAtividade = !String.IsNullOrEmpty(txtNroRamo.Text) ? txtNroRamo.Text.Substring(1).ToInt32Null() : null;

                    RamosAtividadesListaDadosCadastraisRamosAtividades[] ramos = client.ListaDadosCadastraisRamosAtividades(codGrupoRamo, codRamoAtividade);
                    client.Close();

                    //ddlRamoAtividade.Enabled = !String.IsNullOrEmpty(txtNroRamo.Text) ? false : true;
                    ddlRamoAtividade.Items.Clear();
                    ddlRamoAtividade.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (RamosAtividadesListaDadosCadastraisRamosAtividades ramo in ramos.OrderBy(r => r.DescrRamoAtividade))
                    {
                        // TODO Lógica deve ir para serviço
                        if (ramo.SituacaoRamoAtividade == 'A')
                        {
                            ListItem item = new ListItem(ramo.DescrRamoAtividade, String.Format(@"{0:0000}", ramo.CodRamoAtivididade));
                            ddlRamoAtividade.Items.Add(item);
                        }
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Ramos de Atividade e carrega o dropdown
        /// </summary>
        private void CarregarRamoAtividade(Int32? codGrupoRamo, Int32? codRamoAtividade)
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ramos de Atividades"))
                {
                    RamosAtividadesListaDadosCadastraisRamosAtividades[] ramos = client.ListaDadosCadastraisRamosAtividades(codGrupoRamo, codRamoAtividade);
                    client.Close();

                    //ddlRamoAtividade.Enabled = !String.IsNullOrEmpty(txtNroRamo.Text) ? false : true;
                    ddlRamoAtividade.Items.Clear();
                    ddlRamoAtividade.Items.Add(new ListItem(String.Empty, String.Empty));
                    foreach (RamosAtividadesListaDadosCadastraisRamosAtividades ramo in ramos.OrderBy(r => r.DescrRamoAtividade))
                    {
                        // TODO Lógica deve ir para serviço
                        if (ramo.SituacaoRamoAtividade == 'A')
                        {
                            ListItem item = new ListItem(ramo.DescrRamoAtividade, String.Format(@"{0:0000}", ramo.CodRamoAtivididade));
                            ddlRamoAtividade.Items.Add(item);
                        }
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Ramos de Atividade e carrega o dropdown
        /// </summary>
        private void CarregarNroRamo()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ramos de Atividades Por CNAE"))
                {
                    ListaRamosAtividadesPorCnae[] ramos = client.ListaRamosAtividadesPorCnae(Credenciamento.CNAE);
                    client.Close();

                    if (ramos.Length > 0 && ramos[0].CodGrupoRamo != null && ramos[0].CodRamoAtivididade != null)
                    {
                        txtNroRamo.Text = String.Format(@"{0}{1:0000}", ramos[0].CodGrupoRamo, ramos[0].CodRamoAtivididade);
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de EDVs e carrega dropdown
        /// </summary>
        private void CarregarEDV()
        {
            ServicoPortalGEEmpVendaDiretaClient client = new ServicoPortalGEEmpVendaDiretaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Lista de EDVs"))
                {
                    ddlEDV.Items.Clear();

                    EmpVendaDiretaListaDadosCadastrais[] edvs = client.ListaDadosCadastrais(null);
                    client.Close();

                    ddlEDV.Items.Add("");
                    foreach (EmpVendaDiretaListaDadosCadastrais edv in edvs)
                    {
                        ListItem item = new ListItem(edv.NomeRazaoSocialEmpVendaDireta, edv.CodEmpVendaDireta.ToString());
                        ddlEDV.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GEEmpVenDir.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Consulta dados do Serasa para pessoas físca ou jurídica
        /// </summary>
        private void CarregarSerasa()
        {
            try
            {
                if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                {
                    if (!String.IsNullOrEmpty(Credenciamento.RazaoSocial))
                    {
                        txtRazaoSocial.Text = Credenciamento.RazaoSocial;
                        if (String.Compare(Credenciamento.RetornoSerasa, "00") == 0)
                            txtRazaoSocial.Enabled = false;
                    }
                    if (Credenciamento.DataFundacao != DateTime.MinValue && Credenciamento.DataFundacao != new DateTime(1900, 1, 1))
                    {
                        txtDataFundacao.Text = Credenciamento.DataFundacao.ToString("dd/MM/yyyy");
                        if (String.Compare(Credenciamento.RetornoSerasa, "00") == 0)
                        {
                            txtDataFundacao.Enabled = false;
                            txtDataFundacao.CssClass = txtDataFundacao.CssClass.Replace("dtPicker", String.Empty);
                            txtDataFundacao.Attributes.CssStyle.Add("width", "120px");
                        }
                    }
                    if (!String.IsNullOrEmpty(Credenciamento.CNAE))
                    {
                        txtCNAE.Text = Credenciamento.CNAE;
                        //if (String.Compare(Credenciamento.RetornoSerasa, "00") == 0)
                        //    txtCNAE.Enabled = false;
                    }

                    Proprietarios = Credenciamento.Proprietarios;

                    gvDadosProprietarios.DataSource = Proprietarios;
                    gvDadosProprietarios.DataBind();
                }
                else
                {
                    //TODO preencher campos PF
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida Canal Tipo de Pessoa
        /// </summary>
        /// <returns></returns>
        private Boolean ValidaCanalTipoPessoa()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();
            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Ramos de Atividades Por CNAE"))
                {
                    Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtivididade = Credenciamento.RamoAtividade;
                    Int32 codCanal = Credenciamento.Canal;

                    RamosAtividadesValidaCanalTipoPessoa[] ramos = client.ValidaCanalTipoPessoa(codGrupoRamo, codRamoAtivididade, codCanal, tipoPessoa);
                    client.Close();

                    if (ramos[0].IndPermissao == 'S')
                        return true;

                    return false;
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Verifica se IndRamoTarget = S para continuar com o Credenciamento
        /// </summary>
        /// <returns></returns>
        private void ListaDadosComplementaresRamosAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Complementares Ramos de Atividades"))
                {
                    Int32 codGrupoRamo = txtNroRamo.Text.Substring(0, 1).ToInt32();
                    Int32 codRamoAtividade = txtNroRamo.Text.Substring(1).ToInt32();

                    RamosAtividadesListaDadosComplementaresRamosAtividades[] ramosAtividade = client.ListaDadosComplementaresRamosAtividades(codGrupoRamo, codRamoAtividade);
                    client.Close();

                    if (ramosAtividade.Length > 0)
                    {
                        Credenciamento.PermIATA = ramosAtividade[0].IndPermIata;
                        Credenciamento.IndRamoTarget = ramosAtividade[0].IndRamoTarget;
                        Credenciamento.IndPermMaquineta = ramosAtividade[0].IndPermMaquineta;
                        Credenciamento.IndPermDuplCNPJ = ramosAtividade[0].IndPermDuplCNPJ;
                        Credenciamento.IndPermDuplCPF = ramosAtividade[0].IndPermDuplCPF;
                        Credenciamento.IndTipoMoeda = ramosAtividade[0].IndTipoMoeda;
                        Credenciamento.IndMarketingDireto = ramosAtividade[0].IndMarketingDireto;
                        Credenciamento.IndPermCentMsmRamo = ramosAtividade[0].IndPermCentMsmRamo;
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da segunda tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso2()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Cliente"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    List<PNTransicoesServico.Proprietario> proprietarios = PreencheListaProprietarios();
                    PNTransicoesServico.Endereco enderecoComercial = PreencheEndereco(Credenciamento.EnderecoComercial);
                    PNTransicoesServico.Endereco enderecoCorrespondencia = PreencheEndereco(Credenciamento.EnderecoCorrespondencia);
                    PNTransicoesServico.Endereco enderecoInstalacao = PreencheEndereco(Credenciamento.EnderecoInstalacao);
                    PNTransicoesServico.DomicilioBancario domCredito = null;
                    if (Credenciamento.CodTipoEstabelecimento == 1)
                        domCredito = PreencheDomicilioBancario(1, Credenciamento.CodBancoCredito,
                            Credenciamento.NomeBancoCredito, Credenciamento.AgenciaCredito, Credenciamento.ContaCredito);

                    List<PNTransicoesServico.Produto> produtosCredito = PreencheListaProdutos(Credenciamento.ProdutosCredito);
                    List<PNTransicoesServico.Produto> produtosDebito = PreencheListaProdutos(Credenciamento.ProdutosDebito);
                    List<PNTransicoesServico.Produto> produtosConstrucard = PreencheListaProdutos(Credenciamento.ProdutosConstrucard);
                    List<PNTransicoesServico.Patamar> patamares = PreencheListaPatamares(Credenciamento.Patamares);

                    Int32 retorno = client.GravarAtualizarPasso2(proposta, proprietarios, enderecoComercial, enderecoCorrespondencia, enderecoInstalacao, domCredito, 
                        produtosCredito, produtosDebito, produtosConstrucard, patamares);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault>)
            {
                client.Abort();
                throw;
            }
            catch (FaultException<GERegimes.ModelosErroServicos> fe)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Excluí um proprietário
        /// </summary>
        private WFProprietarios.RetornoErro ExcluirProprietario(Modelo.Proprietario proprietario)
        {
            ServicoPortalWFProprietariosClient client = new ServicoPortalWFProprietariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Excluí dados do proprietário"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                    Char codTipoPesProprietario = proprietario.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJCPFProprietario = 0;
                    if (codTipoPesProprietario == 'J')
                        Int64.TryParse(proprietario.CPF_CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJCPFProprietario);
                    else
                        Int64.TryParse(proprietario.CPF_CNPJ.Replace(".", "").Replace("-", ""), out numCNPJCPFProprietario);

                    String nomeProprietario = proprietario.Nome;
                    DateTime? dataNascProprietario = null;
                    Double? participacaoAcionaria = null;
                    String usuarioUltimaAtualizacao = String.Empty;

                    WFProprietarios.RetornoErro[] retorno = client.ExclusaoProprietarios(codTipoPessoa, numCNPJ, numSeqProp, codTipoPesProprietario, numCNPJCPFProprietario, nomeProprietario, dataNascProprietario, participacaoAcionaria, usuarioUltimaAtualizacao);
                    client.Close();

                    return retorno[0];
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Monta o valor do número do ramo
        /// </summary>
        private void MontaNroRamo()
        {
            if (ddlRamoAtuacao.SelectedValue != null && ddlRamoAtividade.SelectedValue != null)
                txtNroRamo.Text = String.Format(@"{0}{1:0000}", ddlRamoAtuacao.SelectedValue, ddlRamoAtividade.SelectedValue.ToInt32());
            else
                txtNroRamo.Text = String.Empty;
        }

        /// <summary>
        /// Salva dados da Tela
        /// </summary>
        /// <returns></returns>
        private Int32 SalvarDados()
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    //Grava dados do Credenciamento na sessão
                    if (String.Compare(Credenciamento.TipoPessoa, "F") == 0)
                    {
                        Credenciamento.NomeCompleto = txtNomeCompleto.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.DataNascimento = txtDataNascimento.Text.ToDate();
                        Credenciamento.Proprietarios = new List<Modelo.Proprietario>{
                            new Modelo.Proprietario {
                                CPF_CNPJ = Credenciamento.CPF,
                                Nome = Credenciamento.NomeCompleto.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper(),
                                Participacao = "100",
                                TipoPessoa = "F"
                            }
                        };
                    }
                    else
                    {

                        Credenciamento.Proprietarios = Proprietarios;
                        Credenciamento.PermitePatamar = true;
                        if (Credenciamento.GrupoRamo != txtNroRamo.Text.Substring(0, 1).ToInt32() || Credenciamento.RamoAtividade != txtNroRamo.Text.Substring(1).ToInt32())
                        {
                            if (Credenciamento.GrupoRamo != 0 && Credenciamento.RamoAtividade != 0)
                                Credenciamento.RefazerNegociacao = true;
                            if (Credenciamento.Patamares != null)
                                Credenciamento.Patamares.Clear();
                        }
                        else
                            if (Credenciamento.CodTipoEstabelecimento == 1)
                                Credenciamento.PermitePatamar = false;


                        Credenciamento.GrupoRamo = txtNroRamo.Text.Substring(0, 1).ToInt32();
                        Credenciamento.RamoAtividade = txtNroRamo.Text.Substring(1).ToInt32();
                        Credenciamento.RazaoSocial = txtRazaoSocial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                        Credenciamento.DataFundacao = txtDataFundacao.Text.ToDate();
                        Credenciamento.CNAE = txtCNAE.Text;
                    }

                    Credenciamento.PessoaContato = txtNome.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
                    Credenciamento.NomeEmail = txtEmail.Text;
                    Credenciamento.NomeHomePage = txtSite.Text;
                    Credenciamento.NumDDD1 = txtTel1DDD.Text;
                    Credenciamento.NumTelefone1 = txtTel1Numero.Text.ToInt32Null();
                    Credenciamento.Ramal1 = txtTel1Ramal.Text.ToInt32Null();
                    Credenciamento.NumDDDFax = txtFaxDDD.Text;
                    Credenciamento.NumTelefoneFax = txtFaxNumero.Text.ToInt32Null();
                    Credenciamento.NumDDD2 = txtTel2DDD.Text;
                    Credenciamento.NumTelefone2 = txtTel2Numero.Text.ToInt32Null();
                    Credenciamento.Ramal2 = txtTel2Ramal.Text.ToInt32Null();
                    Credenciamento.EDV = chkEDV.Checked ? ddlEDV.SelectedValue.ToInt32Null() : null;
                    Credenciamento.IndExtratoEmail = chkAtivarExtratoEmail.Checked ? 'S' : 'N';

                    // Valida se a soma da participação dos proprietários é válida
                    Boolean validaParticipacao = false;
                    String msgErroParticipacao = String.Empty;

                    Decimal participacao = (from p in Proprietarios select p.Participacao.Replace(",", ".").ToDecimal()).Sum();
                    if (Credenciamento.ExigeParticipacaoIntegral != null && Credenciamento.ExigeParticipacaoIntegral == 'S')
                    {
                        validaParticipacao = participacao == 100 ? true : false;
                        msgErroParticipacao = participacao < 100 ? "Total da participação acionária informada insuficiente – mínimo aceitável: 100%" : "Total da participação acionária informada não pode ser superior a 100%";
                    }
                    else
                    {
                        validaParticipacao = participacao >= 51 && participacao <= 100 ? true : false;
                        msgErroParticipacao = participacao < 51 ? "Total da participação acionária informada insuficiente – mínimo aceitável: 51%" : "Total da participação acionária informada não pode ser superior a 100%";
                    }

                    if (ValidaCanalTipoPessoa())
                    {
                        ListaDadosComplementaresRamosAtividade();

                        if (Credenciamento.IndRamoTarget != null && Credenciamento.IndRamoTarget != 'N')
                        {
                            Boolean pvAtivos = ListaCadastroReduzidoPorCNPJ();
                            if (!(pvAtivos && String.Compare(Credenciamento.TipoPessoa, "J") == 0 && Credenciamento.IndPermDuplCNPJ == 'N' ||
                                pvAtivos && String.Compare(Credenciamento.TipoPessoa, "F") == 0 && Credenciamento.IndPermDuplCPF == 'N'))
                            {
                                if (Credenciamento.TipoPessoa == "F" || validaParticipacao)
                                {
                                    return GravarAtualizarPasso2();
                                }
                                else
                                    base.ExibirPainelExcecao(msgErroParticipacao, "300");
                            }
                            else
                                base.ExibirPainelExcecao("Ramo não permite duplicação – impossível continuar com o credenciamento", "300");
                        }
                        else
                            base.ExibirPainelExcecao("Ramo não TARGET – impossível continuar com o credenciamento", "300");
                    }
                    else
                        base.ExibirPainelExcecao("Ramo de Atividade não permitido para esse Canal e Tipo de Pessoa.", "300");
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
            catch (FaultException<GERegimes.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
                return (Int32)fe.Detail.CodErro;
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
                return pe.Codigo;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        /// <summary>
        /// Valida Telefone e DDD
        /// </summary>
        /// <param name="ddd"></param>
        /// <param name="telefone"></param>
        /// <returns></returns>
        private Boolean ValidaTelefone(String ddd, String telefone)
        {
            if (String.IsNullOrEmpty(ddd) && !String.IsNullOrEmpty(telefone) ||
                !String.IsNullOrEmpty(ddd) && String.IsNullOrEmpty(telefone))
                return false;

            return true;
        }

        /// <summary>
        /// Lista cadastro reduzido por CNPJ
        /// </summary>
        /// <returns></returns>
        private Boolean ListaCadastroReduzidoPorCNPJ()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 numCNPJ = 0;

                PontoVendaListaCadastroReduzidoPorCNPJ[] retorno = client.ListaCadastroReduzidoPorCNPJ(codTipoPessoa, numCNPJ);
                client.Close();

                if (retorno.Length > 0)
                    return true;

                return false;
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }
        #endregion
    }
}