using System;
using Redecard.PN.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class DadosCliente : BostonBasePage
    {

        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Cliente - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        MudarLogoOrigemCredenciamento();
                        MostraEscondeControlesPorTipoPessoa(DadosCredenciamento.TipoPessoa);
                        CarregarDadosControles();
                        CarregarDadosClienteDaSessao();
                    }
                }
                catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Cliente", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Cliente", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Cliente", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Cliente", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Muda a imagem e background do logotipo da Masterpage caso a origem seja diferente
        /// </summary>
        private void MudarLogoOrigemCredenciamento()
        {
            HiddenField hdnJsOrigem = (HiddenField)this.Master.FindControl("hdnJsOrigem");
            hdnJsOrigem.Value = String.Format("{0}-{1}", DadosCredenciamento.Canal, DadosCredenciamento.Celula);
        }

        /// <summary>
        /// Evento do clique no botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Cliente - Continuar"))
            {
                try
                {
                    Page.Validate("vgGeral");
                    if (!chkCorrEndComercial.Checked)
                        Page.Validate("vgEnderecoCorrespondencia");

                    if (Page.IsValid)
                    {
                        CarregarDadosClienteParaSessao();
                        
                        if (ValidaParticipacaoAcionaria())
                        {
                            // Caso seja uma nova proposta busca novo número de sequencia
                            if (DadosCredenciamento.NumeroSequencia == 0)
                                DadosCredenciamento.NumeroSequencia = Servicos.GetNumeroSequencia(DadosCredenciamento.TipoPessoa, DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong());

                            Int32 codRetorno = SalvarDados();

                            if (codRetorno == 0)
                                Response.Redirect("dadosbancarios.aspx", false);
                            else
                                base.ExibirPainelExcecao(MENSAGEM, codRetorno);
                        }
                    }
                }
                catch (FaultException<PNTransicoes.GeneralFault> fe)
                {
                    Logger.GravarErro("Boston - Dados Cliente", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.Codigo);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Cliente", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Cliente", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Cliente", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do clique no botão voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            if(DadosCredenciamento.Canal == 26)
                Response.Redirect("DadosIniciais.aspx?dados=W2kQvAu6568p5ijcECP4ScyjJcmdboFjjucHoI1oc5FIDNDMA3yhlJw1a59hJwzo");
            else
                Response.Redirect("DadosIniciais.aspx");
        }

        /// <summary>
        /// Evento de Databound da grid de proprietários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvProprietarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal ltlTipoPessoa = (Literal)e.Row.FindControl("ltlTipoPessoa");
                Literal ltlCNPJCPF = (Literal)e.Row.FindControl("ltlCNPJCPF");
                Literal ltlNomeProprietario = (Literal)e.Row.FindControl("ltlNomeProprietario");
                Literal ltlPartAcionaria = (Literal)e.Row.FindControl("ltlPartAcionaria");

                if (((Proprietario)e.Row.DataItem).TipoPessoa == 'J')
                    ltlTipoPessoa.Text = "Jurídica";
                else if (((Proprietario)e.Row.DataItem).TipoPessoa == 'F')
                    ltlTipoPessoa.Text = "Física";
                ltlCNPJCPF.Text = ((Proprietario)e.Row.DataItem).CPF_CNPJ;
                ltlNomeProprietario.Text = ((Proprietario)e.Row.DataItem).NomeProprietario;
                ltlPartAcionaria.Text = String.Format("{0:f2}%", ((Proprietario)e.Row.DataItem).PartAcionaria);
            }
        }

        /// <summary>
        /// Validação da consistência dos valores de telefone2 e DDD2
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvTelefone2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaTelefone(txtTelefone2DDD.Text, txtTelefone2Numero.Text);
        }

        /// <summary>
        /// Validação da consistência dos valores de Fax e DDD
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvFax_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaTelefone(txtFaxDDD.Text, txtFaxNumero.Text);
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega dados da sessão para os controles da página
        /// </summary>
        private void CarregarDadosClienteDaSessao()
        {
            if (DadosCredenciamento != null)
            {
                txtRazaoSocial.Text = DadosCredenciamento.RazaoSocial;
                txtNomeFantasia.Text = DadosCredenciamento.NomeFantasia;
                txtDataFundacao.Text = DadosCredenciamento.DataFundacao != DateTime.MinValue ? DadosCredenciamento.DataFundacao.ToString("dd/MM/yyyy") : String.Empty;

                gvProprietarios.DataSource = DadosCredenciamento.Proprietarios;
                gvProprietarios.DataBind();

                txtNomeContato.Text = DadosCredenciamento.NomeContato;
                txtTelefone1DDD.Text = DadosCredenciamento.DDDTelefone1;
                txtTelefone1Numero.Text = DadosCredenciamento.NumeroTelefone1;
                txtTelefone1Ramal.Text = DadosCredenciamento.RamalTelefone1;
                txtTelefone2DDD.Text = DadosCredenciamento.DDDTelefone2;
                txtTelefone2Numero.Text = DadosCredenciamento.NumeroTelefone2;
                txtTelefone2Ramal.Text = DadosCredenciamento.RamalTelefone2;
                txtFaxDDD.Text = DadosCredenciamento.DDDFax;
                txtFaxNumero.Text = DadosCredenciamento.NumeroFax;
                txtEmail.Text = DadosCredenciamento.Email;
                txtSite.Text = DadosCredenciamento.Site;

                if (DadosCredenciamento.EnderecoComercial != null)
                {
                    txtCEPComercial.Text = DadosCredenciamento.EnderecoComercial.CEP;
                    txtLogradouroComercial.Text = DadosCredenciamento.EnderecoComercial.Logradouro;
                    txtNumeroComercial.Text = DadosCredenciamento.EnderecoComercial.Numero;
                    txtComplementoComercial.Text = DadosCredenciamento.EnderecoComercial.Complemento;
                    txtCidadeComercial.Text = DadosCredenciamento.EnderecoComercial.Cidade;
                    ddlUFComercial.SelectedValue = DadosCredenciamento.EnderecoComercial.Estado;
                    txtBairroComercial.Text = DadosCredenciamento.EnderecoComercial.Bairro;
                }

                if (DadosCredenciamento.EnderecoCorrespondencia != null)
                {
                    txtCEPCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.CEP;
                    txtLogradouroCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.Logradouro;
                    txtNumeroCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.Numero;
                    txtComplementoCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.Complemento;
                    txtCidadeCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.Cidade;
                    ddlUFCorrespondencia.SelectedValue = DadosCredenciamento.EnderecoCorrespondencia.Estado;
                    txtBairroCorrespondencia.Text = DadosCredenciamento.EnderecoCorrespondencia.Bairro;
                }

                if (!String.IsNullOrEmpty(DadosCredenciamento.DiaFuncionamentoInicio))
                    ddlDiaFuncionamentoInicio.SelectedValue = DadosCredenciamento.DiaFuncionamentoInicio;

                if (!String.IsNullOrEmpty(DadosCredenciamento.DiaFuncionamentoFinal))
                    ddlDiaFuncionamentoFinal.SelectedValue = DadosCredenciamento.DiaFuncionamentoFinal;

                if (!String.IsNullOrEmpty(DadosCredenciamento.HorarioFuncionamentoInicio))
                    ddlHorarioFuncionamentoInicio.SelectedValue = DadosCredenciamento.HorarioFuncionamentoInicio;

                if (!String.IsNullOrEmpty(DadosCredenciamento.HorarioFuncionamentoFinal))
                    ddlHorarioFuncionamentoFinal.SelectedValue = DadosCredenciamento.HorarioFuncionamentoFinal;

                chkEstabShop.Checked = DadosCredenciamento.EstabelecimentoLocalizadoShoppingChecked;
                chkCorrEndComercial.Checked = DadosCredenciamento.EnderecoComercialIgualCorrespondenciaChecked;
            }
        }

        /// <summary>
        /// Carrega dados dos controles da página para sessão
        /// </summary>
        private void CarregarDadosClienteParaSessao()
        {
            DadosCredenciamento.DescricaoGrupoAtuacao = txtGrupoAtuacao.Text;
            DadosCredenciamento.DescricaoRamoAtividade = txtRamoAtividade.Text;
            DadosCredenciamento.RazaoSocial = txtRazaoSocial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.NomeFantasia = txtNomeFantasia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.DataFundacao = txtDataFundacao.Text.ToDate("dd/MM/yyyy");

            DadosCredenciamento.NomeContato = txtNomeContato.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.DDDTelefone1 = txtTelefone1DDD.Text;
            DadosCredenciamento.NumeroTelefone1 = txtTelefone1Numero.Text;
            DadosCredenciamento.RamalTelefone1 = txtTelefone1Ramal.Text;
            DadosCredenciamento.DDDTelefone2 = txtTelefone2DDD.Text;
            DadosCredenciamento.NumeroTelefone2 = txtTelefone2Numero.Text;
            DadosCredenciamento.RamalTelefone2 = txtTelefone2Ramal.Text;
            DadosCredenciamento.DDDFax = txtFaxDDD.Text;
            DadosCredenciamento.NumeroFax = txtFaxNumero.Text;
            DadosCredenciamento.Email = txtEmail.Text;
            DadosCredenciamento.Site = txtSite.Text;

            DadosCredenciamento.EnderecoComercial = new Endereco();
            DadosCredenciamento.EnderecoComercial.TipoEndereco = '1';
            DadosCredenciamento.EnderecoComercial.CEP = txtCEPComercial.Text;
            DadosCredenciamento.EnderecoComercial.Logradouro = txtLogradouroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.EnderecoComercial.Numero = txtNumeroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.EnderecoComercial.Complemento = txtComplementoComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.EnderecoComercial.Cidade = txtCidadeComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            DadosCredenciamento.EnderecoComercial.Estado = ddlUFComercial.SelectedValue.ToUpper();
            DadosCredenciamento.EnderecoComercial.Bairro = txtBairroComercial.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();

            DadosCredenciamento.EstabelecimentoLocalizadoShoppingChecked = chkEstabShop.Checked;
            DadosCredenciamento.EnderecoComercialIgualCorrespondenciaChecked = chkCorrEndComercial.Checked;

            DadosCredenciamento.DiaFuncionamentoInicio = ddlDiaFuncionamentoInicio.SelectedValue;
            DadosCredenciamento.DiaFuncionamentoFinal = ddlDiaFuncionamentoFinal.SelectedValue;
            DadosCredenciamento.HorarioFuncionamentoInicio = ddlHorarioFuncionamentoInicio.SelectedValue;
            DadosCredenciamento.HorarioFuncionamentoFinal = ddlHorarioFuncionamentoFinal.SelectedValue;

            DadosCredenciamento.EnderecoCorrespondencia = new Endereco();
            DadosCredenciamento.EnderecoInstalacao = new Endereco();
            DadosCredenciamento.EnderecoCorrespondencia.TipoEndereco = '2';
            DadosCredenciamento.EnderecoInstalacao.TipoEndereco = '4';
            if (!DadosCredenciamento.EnderecoComercialIgualCorrespondenciaChecked)
            {
                DadosCredenciamento.EnderecoCorrespondencia.CEP = txtCEPCorrespondencia.Text;
                DadosCredenciamento.EnderecoCorrespondencia.Logradouro = txtLogradouroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoCorrespondencia.Numero = txtNumeroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoCorrespondencia.Complemento = txtComplementoCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoCorrespondencia.Cidade = txtCidadeCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoCorrespondencia.Estado = ddlUFCorrespondencia.SelectedValue.ToUpper();
                DadosCredenciamento.EnderecoCorrespondencia.Bairro = txtBairroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();

                DadosCredenciamento.EnderecoInstalacao.CEP = txtCEPCorrespondencia.Text;
                DadosCredenciamento.EnderecoInstalacao.Logradouro = txtLogradouroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoInstalacao.Numero = txtNumeroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoInstalacao.Complemento = txtComplementoCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoInstalacao.Cidade = txtCidadeCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
                DadosCredenciamento.EnderecoInstalacao.Estado = ddlUFCorrespondencia.SelectedValue.ToUpper();
                DadosCredenciamento.EnderecoInstalacao.Bairro = txtBairroCorrespondencia.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().Trim().ToUpper();
            }
            else
            {
                DadosCredenciamento.EnderecoCorrespondencia.CEP = DadosCredenciamento.EnderecoComercial.CEP;
                DadosCredenciamento.EnderecoCorrespondencia.Logradouro = DadosCredenciamento.EnderecoComercial.Logradouro;
                DadosCredenciamento.EnderecoCorrespondencia.Numero = DadosCredenciamento.EnderecoComercial.Numero;
                DadosCredenciamento.EnderecoCorrespondencia.Complemento = DadosCredenciamento.EnderecoComercial.Complemento;
                DadosCredenciamento.EnderecoCorrespondencia.Cidade = DadosCredenciamento.EnderecoComercial.Cidade;
                DadosCredenciamento.EnderecoCorrespondencia.Estado = DadosCredenciamento.EnderecoComercial.Estado;
                DadosCredenciamento.EnderecoCorrespondencia.Bairro = DadosCredenciamento.EnderecoComercial.Bairro;

                DadosCredenciamento.EnderecoInstalacao.CEP = DadosCredenciamento.EnderecoComercial.CEP;
                DadosCredenciamento.EnderecoInstalacao.Logradouro = DadosCredenciamento.EnderecoComercial.Logradouro;
                DadosCredenciamento.EnderecoInstalacao.Numero = DadosCredenciamento.EnderecoComercial.Numero;
                DadosCredenciamento.EnderecoInstalacao.Complemento = DadosCredenciamento.EnderecoComercial.Complemento;
                DadosCredenciamento.EnderecoInstalacao.Cidade = DadosCredenciamento.EnderecoComercial.Cidade;
                DadosCredenciamento.EnderecoInstalacao.Estado = DadosCredenciamento.EnderecoComercial.Estado;
                DadosCredenciamento.EnderecoInstalacao.Bairro = DadosCredenciamento.EnderecoComercial.Bairro;
            }

            // Caso pessoa física incluir um proprietário com participação 100%
            if (DadosCredenciamento.TipoPessoa == 'F')
                DadosCredenciamento.Proprietarios = new List<Proprietario> { new Proprietario
                {
                    CPF_CNPJ = DadosCredenciamento.CPF_CNPJ,
                    NomeProprietario = DadosCredenciamento.RazaoSocial,
                    PartAcionaria = 100,
                    TipoPessoa = DadosCredenciamento.TipoPessoa
                }
            };
        }

        /// <summary>
        /// Carrega os valores dos controles da página
        /// </summary>
        private void CarregarDadosControles()
        {
            CarregarGrupoAtuacao();
            CarregarRamosAtividade(DadosCredenciamento.CodigoGrupoAtuacao, DadosCredenciamento.CodigoRamoAtividade);
            CarregarEstados();
        }

        /// <summary>
        /// Carrega combo box de estados
        /// </summary>
        private void CarregarEstados()
        {
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

            ddlUFComercial.DataSource = listaEstados;
            ddlUFComercial.DataBind();
            ddlUFCorrespondencia.DataSource = listaEstados;
            ddlUFCorrespondencia.DataBind();
        }

        /// <summary>
        /// Popula os valores da combobox do "Grupo de atuação"
        /// </summary>
        private void CarregarGrupoAtuacao()
        {
            txtGrupoAtuacao.Text = Servicos.GetNomeGrupoAtuacao(DadosCredenciamento.CodigoGrupoAtuacao);
        }

        /// <summary>
        /// Popula os valores da combobox de "Ramos de Atividades"
        /// </summary>
        private void CarregarRamosAtividade(Int32? codGrupoRamo, Int32? codRamoAtividade)
        {
            txtRamoAtividade.Text = Servicos.GetNomeRamoAtuacao(DadosCredenciamento.CodigoGrupoAtuacao, DadosCredenciamento.CodigoRamoAtividade);
        }

        /// <summary>
        /// Método de validação da consistência entre ddd e número de telefone
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
        /// Mostra / Esconde controles da página de acordo com o tipo de pessoa
        /// </summary>
        /// <param name="tipoPessoa"></param>
        private void MostraEscondeControlesPorTipoPessoa(Char tipoPessoa)
        {
            if (tipoPessoa == 'F')
            {
                ltlDataFundacao.Text = "Data de Nascimento*:";
                ltlRazaoSocial.Text = "Nome Completo*:";
                ltlEnderecoComercial.Text = "Endereço";
                ltlDiasFuncionamento.Text = "Dias de Vendas*: ";
                ltlHorarioFuncionamento.Text = "Horário de Vendas*: ";
                ltlTelefone2.Text = "Celular:";
                pnlProprietarios.Visible = false;
                pnlGrupoAtuacao.Visible = false;
                pnlEnderecoCorrespondencia.Visible = false;
                chkCorrEndComercial.Visible = false;
                txtRazaoSocial.Enabled = true;
                txtDataFundacao.Enabled = true;
            }
            else
            {
                if(String.IsNullOrEmpty(DadosCredenciamento.RazaoSocial.Trim()))
                    txtRazaoSocial.Enabled = true;

                if (DadosCredenciamento.DataFundacao == DateTime.MinValue)
                {
                    txtDataFundacao.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Valida a participação acionária dos proprietários
        /// </summary>
        /// <returns></returns>
        private Boolean ValidaParticipacaoAcionaria()
        {
            String msgErroParticipacao = String.Empty;
            Double participacaoTotal = (from p in DadosCredenciamento.Proprietarios
                                       select p.PartAcionaria).Sum();

            if (DadosCredenciamento.ExigeParticipacaoIntegral != null && DadosCredenciamento.ExigeParticipacaoIntegral == 'S')
            {
                if (participacaoTotal == 100)
                    return true;
                else
                    msgErroParticipacao = participacaoTotal < 100 ? "Total da participação acionária informada insuficiente – mínimo aceitável: 100%" : "Total da participação acionária informada não pode ser superior a 100%";
            }
            else
            {
                if (participacaoTotal >= 51 && participacaoTotal <= 100)
                    return true;
                else
                    msgErroParticipacao = participacaoTotal < 51 ? "Total da participação acionária informada insuficiente – mínimo aceitável: 51%" : "Total da participação acionária informada não pode ser superior a 100%";
            }

            base.ExibirPainelExcecao(msgErroParticipacao, CODIGO_ERRO);
            return false;
        }

        /// <summary>
        /// Salvar dados
        /// </summary>
        private Int32 SalvarDados()
        {
            return Servicos.TransicaoPasso2(DadosCredenciamento);
        }

        #endregion
    }
}
