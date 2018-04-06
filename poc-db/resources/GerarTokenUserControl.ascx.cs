using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Redecard.PN.Comum;
using System;
using System.ServiceModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.GerarToken
{
    public partial class GerarTokenUserControl : WebpartBase
    {
        #region [Eventos]

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack)
                return;

            // Carrega a página conforme o tipo de geração de Token
            CarregarPagina();
        }

        /// <summary>
        /// Evento do botão Gerar Token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGerarToken_Click(object sender, EventArgs e)
        {
            Boolean ocorreuErro = false;
            Boolean possuiPrimeiroToken = VerificarPrimeiroToken(out ocorreuErro);

            if (ocorreuErro)
                return;

            if (possuiPrimeiroToken)
                ExibirPopupConfirmacao();
            else
                GerarToken();
        }

        /// <summary>
        /// Evento do botão Novo Token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNovoToken_Click(object sender, EventArgs e)
        {
            divConfirmarToken.Visible = false;
            GerarToken();
        }

        /// <summary>
        /// Cancelar a geraçao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNao_Click(object sender, EventArgs e)
        {
            CarregarPagina();
        }
        #endregion [Fim - Eventos]

        #region [Métodos]

        /// <summary>
        /// Carregar página
        /// </summary>
        protected void CarregarPagina()
        {
            //configura campos padrão
            divSucesso.Visible = false;
            divConfirmarToken.Visible = false;
        }

        /// <summary>
        /// Exibir popup de confirmação
        /// </summary>
        protected void ExibirPopupConfirmacao()
        {
            divConfirmarToken.Visible = true;
            StringBuilder sb = new StringBuilder();
            this.ExibirPainelHtml(sb.ToString());
        }

        /// <summary>
        /// Método para gerar token
        /// </summary>
        /// <returns></returns>
        protected void GerarToken()
        {
            using (Logger log = Logger.IniciarLog("Geração de Token."))
            {
                try
                {
                    if (!Sessao.Contem())
                        throw new Exception("Falha ao obter sessão.");

                    Int32 numeroPv = SessaoAtual.CodigoEntidade;
                    String token = String.Empty;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                        token = ctx.Cliente.GeraToken(numeroPv);

                    litToken.Text = token;
                    divSucesso.Visible = true;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro(ex.Message);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro();
                }
            }


        }

        /// <summary>
        /// Exibir mensagem de erro padrão
        /// </summary>
        protected void ExibirMensagemErro(String msg = "Não foi possível gerar o token. Por favor, entre em contato com a Central de Atendimento Rede.")
        {
            divConfirmarToken.Visible = false;
            divSucesso.Visible = false;
            base.ExibirPainelMensagem(msg);
        }

        #endregion [Fim - Métodos]

        #region [Consultas WCF]

        /// <summary>
        /// Verificar se é primeiro Token
        /// </summary>
        /// <returns></returns>
        protected Boolean VerificarPrimeiroToken(out Boolean ocorreuErro)
        {
            Boolean primeiroToken = true;
            ocorreuErro = false;

            using (Logger log = Logger.IniciarLog("Consulta verifica se é primeiro Token."))
            {
                try
                {
                    if (!Sessao.Contem())
                        throw new Exception("Falha ao obter sessão.");

                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                        primeiroToken = ctx.Cliente.VerificaPossuiToken(numeroPv);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    ocorreuErro = true;
                    log.GravarErro(ex);
                    ExibirMensagemErro();
                }
                catch (Exception ex)
                {
                    ocorreuErro = true;
                    log.GravarErro(ex);
                    ExibirMensagemErro();
                }
            }

            return primeiroToken;
        }

        #endregion [Fim - Consultas WCF]
    }
}
