using System;
using Redecard.PN.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using System.ServiceModel;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class DadosIniciais : BostonBasePage
    {

        #region [ Propriedades ]

        /// <summary>
        /// Origem do Credenciamento
        /// </summary>
        public String Origem
        {
            get
            {
                if (ViewState["Origem"] == null)
                    ViewState["Origem"] = String.Empty;

                return (String)ViewState["Origem"];
            }
            set
            {
                ViewState["Origem"] = value;
            }
        }

        #endregion

        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Iniciais - Page Load"))
            {
                try
                {
                    Response.RedirectPermanent("https://www.userede.com.br/nossos-produtos/mobile-rede/");

                    if (!Page.IsPostBack)
                    {
                        VerificaOrigemCredenciamento();
                        MudarLogoOrigemCredenciamento();
                        CarregaDadosControles();
                        CarregaDadosIniciaisDaSessao();
                    }
                }
                catch (FaultException<WFOrigem.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", ex);
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
            hdnJsOrigem.Value = Origem;
        }

        /// <summary>
        /// Evento do Clique no botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Iniciais - Continuar"))
            {
                try
                {
                    String validationGroup = GetValidationGroup(rbTipoPessoa.SelectedValue);

                    Page.Validate(validationGroup);

                    if (Page.IsValid)
                    {
                        DadosCredenciamento = new DadosCredenciamento();

                        CarregaDadosIniciaisParaSessao();
                        var dadosRetorno = Servicos.TransicaoPasso1(DadosCredenciamento);

                        if (dadosRetorno.CodigoRetorno == 0)
                        {
                            CarregarDadosSerasaEServicosParaSessao(dadosRetorno);
                            Response.Redirect("DadosCliente.aspx", false);
                        }
                        else
                        {
                            Logger.GravarErro("Boston - Dados Iniciais", new Exception(dadosRetorno.DescricaoRetorno));
                            SharePointUlsLog.LogErro(new Exception(dadosRetorno.DescricaoRetorno));
                            DadosCredenciamento.CodigoErro = dadosRetorno.CodigoRetorno.ToString();
                            Response.Redirect("ImpossivelContinuar.aspx");
                        }
                    }
                }
                catch (FaultException<WFProposta.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
                }
                catch (FaultException<GECanais.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
                }
                catch (FaultException<PNTransicoes.GeneralFault> fe)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.Codigo);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Iniciais", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Atualiza o captcha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkCliqueAqui_Click(object sender, EventArgs e)
        {
            imgCaptcha.ImageUrl = String.Format(@"captcha.aspx?guid={0}", Guid.NewGuid());
        }

        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCNPJ_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCNPJ.Text.IsValidCNPJ();
        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCPF_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCPF.Text.IsValidCPF();
        }

        /// <summary>
        /// Valida CPF ou CNPJ do Proprietário
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvProprietario_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtProprietario.Text.IsValidCPFCNPJ();
        }

        /// <summary>
        /// Validação do Captcha
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCaptcha_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidaCaptcha();
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega os valores dos controles da página
        /// </summary>
        private void CarregaDadosControles()
        {
            CarregaProfissoes();
            CarregaComoVoceConheceuProduto();
        }

        /// <summary>
        /// Popula os valores da combobox de "Profissões"
        /// </summary>
        private void CarregaProfissoes()
        {
            ddlProfissao.Items.Clear();

            var profissoes = Servicos.GetProfissoes('F', 15, null, null, "MPO");
            ddlProfissao.Items.AddRange(profissoes.ToArray());
        }

        /// <summary>
        /// Popula os valores da combobox "Como você conheceu o produto"
        /// </summary>
        private void CarregaComoVoceConheceuProduto()
        {
            ddlComoConheceu.Items.Clear();

            var comoConheceuLista = Servicos.GetComoConheceu(null, 'A');
            ddlComoConheceu.Items.AddRange(comoConheceuLista.ToArray());
        }

        /// <summary>
        /// Carrega os dados dos controles da tela para a sessão
        /// </summary>
        private void CarregaDadosIniciaisParaSessao()
        {
            DadosCredenciamento.Canal = 15;
            DadosCredenciamento.Celula = 482;
            if (Origem.Equals("VIVO", StringComparison.CurrentCultureIgnoreCase))
            {
                DadosCredenciamento.Canal = 26;
                DadosCredenciamento.Celula = 503;
            }

            DadosCredenciamento.EnderecoComercialIgualCorrespondenciaChecked = true;
            DadosCredenciamento.CCMExecutada = false;
            DadosCredenciamento.TipoPessoa = Char.Parse(rbTipoPessoa.SelectedValue);
            DadosCredenciamento.CPF_CNPJ = DadosCredenciamento.TipoPessoa == 'J' ? txtCNPJ.Text : txtCPF.Text;
            DadosCredenciamento.CPF_CNPJProprietario = txtProprietario.Text;
            DadosCredenciamento.DescricaoProfissao = ddlProfissao.SelectedItem.Text;
            DadosCredenciamento.CodigoProfissao = ddlProfissao.SelectedItem.Value;
            DadosCredenciamento.DescricaoComoConheceu = ddlComoConheceu.SelectedItem.Text;
            DadosCredenciamento.CodigoComoConheceu = ddlComoConheceu.SelectedItem.Value;

            if (DadosCredenciamento.TipoPessoa == 'F')
            {
                DadosCredenciamento.CodigoGrupoAtuacao = DadosCredenciamento.CodigoProfissao.Substring(0, 1).ToInt32();
                DadosCredenciamento.CodigoRamoAtividade = DadosCredenciamento.CodigoProfissao.Substring(1, 4).ToInt32();
            }

            // Recupera informação se canal exige participação integral
            DadosCredenciamento.ExigeParticipacaoIntegral = Servicos.GetExigeParticipacaoIntegral(null, CANAL, "=");
        }

        /// <summary>
        /// Carrega dados da sessão para os controles da tela
        /// </summary>
        private void CarregaDadosIniciaisDaSessao()
        {
            if (DadosCredenciamento != null)
            {
                rbTipoPessoa.SelectedValue = DadosCredenciamento.TipoPessoa.ToString();
                if (DadosCredenciamento.TipoPessoa == 'J')
                    txtCNPJ.Text = DadosCredenciamento.CPF_CNPJ;
                else
                    txtCPF.Text = DadosCredenciamento.CPF_CNPJ;

                txtProprietario.Text = DadosCredenciamento.CPF_CNPJProprietario;
                ddlProfissao.SelectedValue = DadosCredenciamento.CodigoProfissao;
                ddlComoConheceu.SelectedValue = DadosCredenciamento.CodigoComoConheceu;
            }
        }

        /// <summary>
        /// Carrega os dados retornados do SERASA e de Serviços para sessão
        /// </summary>
        /// <param name="dadosRetorno"></param>
        private void CarregarDadosSerasaEServicosParaSessao(PNTransicoes.RetornoGravarAtualizarPasso1 dadosRetorno)
        {
            DadosCredenciamento.CodTipoEstabelecimento = dadosRetorno.CodTipoEstabelecimento;
            DadosCredenciamento.NumPdvMatriz = dadosRetorno.NumPdvMatriz;
            DadosCredenciamento.NumeroSequencia = dadosRetorno.NumSequencia;
            DadosCredenciamento.IndMarketingDireto = dadosRetorno.IndMarketingDireto;

            if (dadosRetorno.DadosSerasa != null)
            {
                DadosCredenciamento.CNAE = dadosRetorno.DadosSerasa.CNAE;
                DadosCredenciamento.DataFundacao = dadosRetorno.DadosSerasa.DataFundacao;
                DadosCredenciamento.RazaoSocial = dadosRetorno.DadosSerasa.RazaoSocial;
                DadosCredenciamento.CodigoGrupoAtuacao = dadosRetorno.DadosSerasa.CodigoGrupoAtuacao;
                DadosCredenciamento.CodigoRamoAtividade = dadosRetorno.DadosSerasa.CodigoRamoAtividade;

                DadosCredenciamento.Proprietarios = new List<Proprietario>();
                foreach (var proprietario in dadosRetorno.DadosSerasa.Proprietarios)
                {
                    DadosCredenciamento.Proprietarios.Add(new Proprietario
                    {
                        CPF_CNPJ = proprietario.CPFCNPJ,
                        NomeProprietario = proprietario.Nome,
                        PartAcionaria = proprietario.Participacao.ToDouble(),
                        TipoPessoa = Char.Parse(proprietario.TipoPessoa)
                    });
                }
            }

            if (dadosRetorno.PropostaPendente != null)
            {
                DadosCredenciamento.CodTipoMovimento = dadosRetorno.PropostaPendente.CodTipoMovimento;
                DadosCredenciamento.EnderecoComercialIgualCorrespondenciaChecked = dadosRetorno.PropostaPendente.IndEnderecoIgualCom == 'S' ? true : false;
                DadosCredenciamento.NomeContato = dadosRetorno.PropostaPendente.PessoaContato;
                DadosCredenciamento.Email = dadosRetorno.PropostaPendente.NomeEmail;
                DadosCredenciamento.Site = dadosRetorno.PropostaPendente.NomeHomePage;
                DadosCredenciamento.NumOcorrencia = dadosRetorno.PropostaPendente.NumOcorrencia;
                DadosCredenciamento.NumPdv = dadosRetorno.PropostaPendente.NumPdv;
                DadosCredenciamento.DDDTelefone1 = dadosRetorno.PropostaPendente.NumDDD1;
                DadosCredenciamento.DDDTelefone2 = dadosRetorno.PropostaPendente.NumDDD2;
                DadosCredenciamento.DDDFax = dadosRetorno.PropostaPendente.NumDDDFax;
                DadosCredenciamento.NumeroTelefone1 = dadosRetorno.PropostaPendente.NumTelefone1.ToString();
                DadosCredenciamento.NumeroTelefone2 = dadosRetorno.PropostaPendente.NumTelefone2 != 0 ? dadosRetorno.PropostaPendente.NumTelefone2.ToString() : String.Empty;
                DadosCredenciamento.NumeroFax = dadosRetorno.PropostaPendente.NumTelefoneFax != 0 ? dadosRetorno.PropostaPendente.NumTelefoneFax.ToString() : String.Empty;
                DadosCredenciamento.RamalTelefone1 = dadosRetorno.PropostaPendente.Ramal1 != 0 ? dadosRetorno.PropostaPendente.Ramal1.ToString() : String.Empty;
                DadosCredenciamento.RamalTelefone2 = dadosRetorno.PropostaPendente.Ramal2 != 0 ? dadosRetorno.PropostaPendente.Ramal2.ToString() : String.Empty;
                DadosCredenciamento.EstabelecimentoLocalizadoShoppingChecked = dadosRetorno.PropostaPendente.IndRegiaoLoja == 'S' ? true : false;
                DadosCredenciamento.NomeFantasia = dadosRetorno.PropostaPendente.NomeFatura;

                if (DadosCredenciamento.TipoPessoa == 'F')
                {
                    DadosCredenciamento.RazaoSocial = dadosRetorno.PropostaPendente.RazaoSocial;
                    DadosCredenciamento.DataFundacao = dadosRetorno.PropostaPendente.DataFundacao;
                }
            }

            if (dadosRetorno.DadosTecnologia != null)
            {
                DadosCredenciamento.DiaFuncionamentoInicio = dadosRetorno.DadosTecnologia.DiaInicioFuncionamento.ToString();
                DadosCredenciamento.DiaFuncionamentoFinal = dadosRetorno.DadosTecnologia.DiaFimFuncionamento.ToString();
                DadosCredenciamento.HorarioFuncionamentoInicio = dadosRetorno.DadosTecnologia.HoraInicioFuncionamento.ToString();
                DadosCredenciamento.HorarioFuncionamentoFinal = dadosRetorno.DadosTecnologia.HoraFimFuncionamento.ToString();
            }

            if (dadosRetorno.EnderecoComercial != null)
            {
                DadosCredenciamento.EnderecoComercial = new Endereco();
                DadosCredenciamento.EnderecoComercial.TipoEndereco = dadosRetorno.EnderecoComercial.IndTipoEndereco;
                DadosCredenciamento.EnderecoComercial.CEP = String.Format("{0}-{1}", dadosRetorno.EnderecoComercial.CodigoCep, dadosRetorno.EnderecoComercial.CodComplementoCep);
                DadosCredenciamento.EnderecoComercial.Logradouro = dadosRetorno.EnderecoComercial.Logradouro;
                DadosCredenciamento.EnderecoComercial.Complemento = dadosRetorno.EnderecoComercial.ComplementoEndereco;
                DadosCredenciamento.EnderecoComercial.Numero = dadosRetorno.EnderecoComercial.NumeroEndereco;
                DadosCredenciamento.EnderecoComercial.Bairro = dadosRetorno.EnderecoComercial.Bairro;
                DadosCredenciamento.EnderecoComercial.Cidade = dadosRetorno.EnderecoComercial.Cidade;
                DadosCredenciamento.EnderecoComercial.Estado = dadosRetorno.EnderecoComercial.Estado;
            }

            if (dadosRetorno.EnderecoCorrespondencia != null)
            {
                DadosCredenciamento.EnderecoCorrespondencia = new Endereco();
                DadosCredenciamento.EnderecoCorrespondencia.TipoEndereco = dadosRetorno.EnderecoCorrespondencia.IndTipoEndereco;
                DadosCredenciamento.EnderecoCorrespondencia.CEP = String.Format("{0}-{1}", dadosRetorno.EnderecoCorrespondencia.CodigoCep, dadosRetorno.EnderecoCorrespondencia.CodComplementoCep);
                DadosCredenciamento.EnderecoCorrespondencia.Logradouro = dadosRetorno.EnderecoCorrespondencia.Logradouro;
                DadosCredenciamento.EnderecoCorrespondencia.Complemento = dadosRetorno.EnderecoCorrespondencia.ComplementoEndereco;
                DadosCredenciamento.EnderecoCorrespondencia.Numero = dadosRetorno.EnderecoCorrespondencia.NumeroEndereco;
                DadosCredenciamento.EnderecoCorrespondencia.Bairro = dadosRetorno.EnderecoCorrespondencia.Bairro;
                DadosCredenciamento.EnderecoCorrespondencia.Cidade = dadosRetorno.EnderecoCorrespondencia.Cidade;
                DadosCredenciamento.EnderecoCorrespondencia.Estado = dadosRetorno.EnderecoCorrespondencia.Estado;
            }

            if (dadosRetorno.DomicilioBancarioCredito != null)
            {
                DadosCredenciamento.CodigoBanco = dadosRetorno.DomicilioBancarioCredito.CodigoBanco.ToString();
                DadosCredenciamento.CodigoAgencia = dadosRetorno.DomicilioBancarioCredito.CodigoAgencia.ToString();
                DadosCredenciamento.ContaCorrente = dadosRetorno.DomicilioBancarioCredito.NumContaCorrente;
            }
        }

        /// <summary>
        /// Valida se o Captcha digitado é correto
        /// </summary>
        /// <returns></returns>
        private Boolean ValidaCaptcha()
        {
            bool retVal = false;
            String cookie = Convert.ToString(Session["cookie"]) ?? "";

            retVal = (txtCaptcha.Text.Equals(cookie, StringComparison.InvariantCultureIgnoreCase));
            return retVal;
        }

        /// <summary>
        /// Retorna o nome do grupo de validação de acordo com o tipo de pessoa (Física ou Jurídica)
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <returns></returns>
        private String GetValidationGroup(String tipoPessoa)
        {
            if (tipoPessoa == "F")
                return "vgPessoaFisica";
            else if (tipoPessoa == "J")
                return "vgPessoaJuridica";

            return String.Empty;
        }

        /// <summary>
        /// Verifica se o credenciamento está sendo feito por outro canal
        /// </summary>
        private void VerificaOrigemCredenciamento()
        {
            try
            {
                String query = Request["dados"];
                if (!String.IsNullOrEmpty(query))
                {
                    QueryStringSegura _query = new QueryStringSegura(query);

                    if (!String.IsNullOrEmpty(_query["origem"]))
                        Origem = _query["origem"];
                }
            }
            catch (Exception)
            {
                Origem = String.Empty;
            }
        }

        #endregion
    }
}
