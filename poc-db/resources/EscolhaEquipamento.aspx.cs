using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.Base;
using System.Web;
using System.Web.Services;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Web.Configuration;
using System.ServiceModel;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class EscolhaEquipamento : BostonBasePage
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Escolha Equipamento - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        MudarLogoOrigemCredenciamento();
                        CarregarDadosControles();
                        CarregarDadosDaSessao();
                    }
                }
                catch (FaultException<WFAdministracao.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", ex);
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
        /// Evento do clique no botão de continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Escolha Equipamento - Continuar"))
            {
                try
                {
                    String mensagemRetorno = String.Empty;
                    Int32 codigoRetorno = 0;

                    if (rbSolicitarLeitor.Checked)
                    {
                        
                        CarregarDadosParaSessao();
                        codigoRetorno = SalvarDados(out mensagemRetorno);

                        if (codigoRetorno == 0)
                            if(!DadosCredenciamento.CCMExecutada)
                                MontaIFramePagamento();
                            else
                                base.ExibirPainelExcecao("Equipamento já adquirido.", 301);
                        else
                        {
                            Logger.GravarErro("Boston - Escolha Equipamento", new Exception(mensagemRetorno));
                            SharePointUlsLog.LogErro(new Exception(mensagemRetorno));
                            base.ExibirPainelExcecao(MENSAGEM, codigoRetorno);
                        }
                    }
                    else
                        Response.Redirect("Comprovante.aspx", false);
                }
                catch (FaultException<FEToken.GeneralFault> fe)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.Codigo);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Escolha Equipamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega dados da tela para sessão
        /// </summary>
        private void CarregarDadosParaSessao()
        {
            DadosCredenciamento.TaxaAtivacao = Decimal.Parse(ltlTaxaAtivacao.Text, NumberStyles.Currency);
        }

        /// <summary>
        /// Salva dados na base
        /// </summary>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        private Int32 SalvarDados(out String mensagemRetorno)
        {
            return Servicos.AtualizaTaxaAtivacaoPropostaMPOS(DadosCredenciamento.TipoPessoa, DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong(), DadosCredenciamento.NumeroSequencia,
                DadosCredenciamento.TaxaAtivacao, 6, DadosCredenciamento.Usuario, out mensagemRetorno);
        }

        /// <summary>
        /// Evento do clique no botão de voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("DadosBancarios.aspx");
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega dados dos controles da página
        /// </summary>
        private void CarregarDadosControles()
        {
            ltlTaxaAtivacao.Text = Servicos.GetTaxaAtivacao(DadosCredenciamento.Canal, DadosCredenciamento.Celula, DadosCredenciamento.TipoPessoa,
                DadosCredenciamento.CodEquipamento, DadosCredenciamento.CodigoGrupoAtuacao, DadosCredenciamento.CodigoRamoAtividade,
                null, 30);
        }

        /// <summary>
        /// Carrega dados da sessão para os controles da página
        /// </summary>
        private void CarregarDadosDaSessao()
        {
            ltlLogradouro.Text = String.Format("{0}, {1} - {2}", DadosCredenciamento.EnderecoCorrespondencia.Logradouro, DadosCredenciamento.EnderecoCorrespondencia.Numero, DadosCredenciamento.EnderecoCorrespondencia.Complemento);
            ltlBairro.Text = DadosCredenciamento.EnderecoCorrespondencia.Bairro;
            ltlCidade.Text = DadosCredenciamento.EnderecoCorrespondencia.Cidade;
            ltlEstado.Text = DadosCredenciamento.EnderecoCorrespondencia.Estado;
            ltlCEP.Text = DadosCredenciamento.EnderecoCorrespondencia.CEP;
            ltlPais.Text = "Brasil";
        }

        /// <summary>
        /// Retorna token do datacash
        /// </summary>
        /// <returns></returns>
        private String GetToken()
        {
            String cpfCnpj = DadosCredenciamento.CPF_CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
            String numPdv = DadosCredenciamento.Canal == 26 && DadosCredenciamento.Celula == 503 ?
                WebConfigurationManager.AppSettings["numPdvEstabelecimentoVivo"].ToString() :
                WebConfigurationManager.AppSettings["numPdvEstabelecimento"].ToString();

            Decimal valorTransacao = DadosCredenciamento.TaxaAtivacao;
            Random random = new Random();
            String numPedido = String.Format(@"{0}-{1}", DadosCredenciamento.NumPdv, random.Next(100000, 999999));
            Int32 qtdParcela = WebConfigurationManager.AppSettings["qtdParcela"].ToInt32();
            String urlRetorno = String.Format("{0}/{1}", SPContext.Current.Web.Url, WebConfigurationManager.AppSettings["urlRetorno"]);
            var contato = DadosCredenciamento.NomeContato.Split(' ');
            String nome = contato[0];
            String sobrenome = contato[contato.Length - 1];
            String email = DadosCredenciamento.Email;
            DateTime dataFundacao = DadosCredenciamento.DataFundacao;
            String telefone1 = String.Format("{0}{1}", DadosCredenciamento.DDDTelefone1, DadosCredenciamento.NumeroTelefone1);
            String telefone2 = String.Format("{0}{1}", DadosCredenciamento.DDDTelefone2, DadosCredenciamento.NumeroTelefone2);

            String token = Servicos.GetTokenAnaliseRisco(cpfCnpj, nome, sobrenome, dataFundacao, email, telefone1, telefone2, numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, new Int32[]{1, 602}, DadosCredenciamento.EnderecoComercial, DadosCredenciamento.EnderecoInstalacao, DadosCredenciamento.EnderecoComercial);
            return token;
        }

        /// <summary>
        /// WebMethod que carrega os dados do comprovante para sessão
        /// </summary>
        /// <param name="nsu"></param>
        /// <param name="tid"></param>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="nomeEstabelecimento"></param>
        /// <param name="dataPagamento"></param>
        /// <param name="horaPagamento"></param>
        /// <param name="numeroAutorizacao"></param>
        /// <param name="tipoTransacao"></param>
        /// <param name="formaPagamento"></param>
        /// <param name="bandeira"></param>
        /// <param name="nomePortador"></param>
        /// <param name="numeroCartao"></param>
        /// <param name="valor"></param>
        /// <param name="numeroParcelas"></param>
        /// <param name="numeroPedido"></param>
        [WebMethod]
        public static String CarregaDadosComprovanteParaSessao(String nsu, String tid, String numeroEstabelecimento, 
            String nomeEstabelecimento, String dataPagamento, String horaPagamento, String numeroAutorizacao,
            String tipoTransacao, String formaPagamento, String bandeira, String nomePortador, String numeroCartao, 
            String valor, String numeroParcelas, String numeroPedido)
        {
            QueryStringSegura queryString = new QueryStringSegura();
            queryString.Add("nsu", HttpUtility.UrlEncode(nsu));
            queryString.Add("tid", HttpUtility.UrlEncode(tid));
            queryString.Add("numeroEstabelecimento", HttpUtility.UrlEncode(numeroEstabelecimento));
            queryString.Add("nomeEstabelecimento", HttpUtility.UrlEncode(nomeEstabelecimento));
            queryString.Add("dataPagamento", HttpUtility.UrlEncode(dataPagamento));
            queryString.Add("horaPagamento", HttpUtility.UrlEncode(horaPagamento));
            queryString.Add("numeroAutorizacao", HttpUtility.UrlEncode(numeroAutorizacao));
            queryString.Add("tipoTransacao", HttpUtility.UrlEncode(tipoTransacao));
            queryString.Add("formaPagamento", HttpUtility.UrlEncode(formaPagamento));
            queryString.Add("bandeira", HttpUtility.UrlEncode(bandeira));
            queryString.Add("nomePortador", HttpUtility.UrlEncode(nomePortador));
            queryString.Add("numeroCartao", HttpUtility.UrlEncode(numeroCartao));
            queryString.Add("valor", HttpUtility.UrlEncode(valor));
            queryString.Add("numeroParcelas", HttpUtility.UrlEncode(numeroParcelas));
            queryString.Add("numeroPedido", HttpUtility.UrlEncode(numeroPedido));
            
            return queryString.ToString();
        }

        /// <summary>
        /// Monta e exibe IFrame de Pagamento
        /// </summary>
        private void MontaIFramePagamento()
        {
            String titulo = "Pagamento";
            String src = WebConfigurationManager.AppSettings["urlDataCash"].ToString();
#if !DEBUG
                    String token = GetToken();
                    src = String.Format(@"{0}?token={1}", src, token);
#endif
            String script = String.Format("exibirIFramePagamento('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(src));
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        #endregion
    }
}
