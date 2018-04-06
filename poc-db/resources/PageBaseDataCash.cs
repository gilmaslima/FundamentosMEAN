using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data;
using System.Globalization;
using Redecard.PN.Comum.TrataErroServico;
using Redecard.PN.DataCash.Base;

namespace Redecard.PN.DataCash.BasePage
{
    public class PageBaseDataCash : System.Web.UI.Page
    {
        public const Int32 CODIGO_ERRO = 300;
        public const String FONTE = "Redecard.PN.Web";

        public const String CAMINHO_CREDITO_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoCredito.ascx";
        public const String CAMINHO_CREDITO_COMPROVANTE = "~/controles/comprovantes/ComprovanteCredito.ascx";

        public const String CAMINHO_PREAUTORIZACAO_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoPreAutorizacao.ascx";
        public const String CAMINHO_PREAUTORIZACAO_COMPROVANTE = "~/controles/comprovantes/ComprovantePreAutorizacao.ascx";

        public const String CAMINHO_PAGAMENTORECORRENTE_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoPagamentoRecorrente.ascx";
        public const String CAMINHO_PAGAMENTORECORRENTE_COMPROVANTE = "~/controles/comprovantes/ComprovantePagamentoRecorrente.ascx";

        public const String CAMINHO_PREAUTORIZACAOAVS_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoPreAutorizacaoAVS.ascx";
        public const String CAMINHO_PREAUTORIZACAOAVS_COMPROVANTE = "~/controles/comprovantes/ComprovantePreAutorizacaoAVS.ascx";

        public const String CAMINHO_CREDITOAVS_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoCreditoAVS.ascx";
        public const String CAMINHO_CREDITOAVS_COMPROVANTE = "~/controles/comprovantes/ComprovanteCreditoAVS.ascx";
        
        public const String CAMINHO_IATA_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoIATA.ascx";
        public const String CAMINHO_IATA_COMPROVANTE = "~/controles/comprovantes/ComprovanteIATA.ascx";

        public const String CAMINHO_BOLETO_CONFIRMACAO = "~/controles/confirmacao/ConfirmacaoBoleto.ascx";
        public const String CAMINHO_BOLETO_COMPROVANTE = "~/controles/comprovantes/ComprovanteBoleto.ascx";

        public const String LISTA_PAISES = "~/controles/comprovantes/ComprovanteBoleto.ascx";

        public static CultureInfo ptBR { get { return new CultureInfo("pt-BR"); } }

        #region [ Variáveis de Sessão ]

        /// <summary>Dados da sessão</summary>
        protected Sessao SessaoAtual
        {
            get { return Session[Sessao.ChaveSessao] as Sessao; }
            private set { Session[Sessao.ChaveSessao] = value; }
        }
  
        /// <summary>Propriedade de Sessão utilizada na geração do documento em pdf.</summary>
        protected DataTable TabelaPDF
        {
            get { return (DataTable)Session["TabelaPDF"]; }
            set { Session["TabelaPDF"] = value; }
        }

        /// <summary>Propriedade de Sessão utilizada para registrar a última transação realizada</summary>
        protected String UltimaTransacaoRealizada
        {
            get { return (String)Session["UltimaTransacaoRealizada"]; }
            set { Session["UltimaTransacaoRealizada"] = value; }
        }
        #endregion

        #region [ Voltar ]

        /// <summary>Contador de posts realizados</summary>
        public Int32 QuantidadePost
        {
            get
            {
                if (ViewState["QuantidadePost"] == null)
                    ViewState["QuantidadePost"] = 0;
                return (Int32)ViewState["QuantidadePost"];
            }
            set { ViewState["QuantidadePost"] = value; }
        }

        /// <summary>Retorna para a página anterior</summary>
        protected void RetornarPaginaAnterior()
        {
            Int32 quantidade = (QuantidadePost + 1) * -1;
            RetornarPaginaAnterior(quantidade);
        }

        /// <summary>Retorna para uma página anterior</summary>
        protected void RetornarPaginaAnterior(Int32 quantidade)
        {
            Page.ClientScript.RegisterStartupScript(typeof(String), "RetornaPagina", 
                String.Format("history.go({0});", quantidade), true);
        }

        #endregion

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (!IsPostBack)
            {
                if (!Sessao.Contem())
                {
                    //Recupera os dados da sessão informada pela querystring
                    SessaoAtual = RecuperarSessaoAtual();
                }
            }
            
            if (!Sessao.Contem())
            {
                GeraExcecao("Acesso negado.");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack)
            {
                QuantidadePost++;
            }
        }

        public void ArmazenaTipoTransacao(String ultimaTransacaoRealizada)
        {
            this.UltimaTransacaoRealizada = ultimaTransacaoRealizada;
        }
    
        private Sessao RecuperarSessaoAtual()
        {
#if DEBUG
            return new Sessao
            {
                CNPJEntidade = "1425787000104",
                CodigoEntidade = 1250191,
                EmailEntidade = "ana.infant3223@redecard.com.br111",
                LoginUsuario = "redecard",
                NomeEntidade = "filial423423432",
                NomeUsuario = "Usuário do Estabelecimento de Teste",
                StatusPV = "A",
                GrupoEntidade = 1,
                Tecnologia = 20,
                UFEntidade = "sp"
            };
#else            
            //Verifica se foram passados os dados pela querystring
            if (String.IsNullOrEmpty(Request.QueryString["dados"]))
                GeraExcecao("QueryString ausente.");

            //Descriptografa os dados da querystring
            QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);

            //Lê os dados passados pela querystring
            String guidSessao = qs["id"];
            if (String.IsNullOrEmpty(guidSessao))
                GeraExcecao("QueryString inválida.");

            Sessao sessao = null;
            using (Logger Log = Logger.IniciarLog("RecuperarSessaoAtual - DataCash"))
            {
                try
                {
                    //Recupera dados da sessão pelo ID
                    sessao = CacheAdmin.Recuperar<Sessao>(Comum.Cache.DataCashIntegracao, guidSessao);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    ExibirPainelExcecao(ex, "/");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    ExibirPainelExcecao(FONTE, CODIGO_ERRO, "/");
                }
            }

            //Se encontrou dados da sessão em Cache
            if (sessao != null)
            {
                //Remove objeto de dados de sessão do cache por segurança
                CacheAdmin.Remover(Comum.Cache.DataCashIntegracao, guidSessao);
            }
            else
                GeraExcecao("Sessão não encontrada: " + guidSessao);

            //Retorna dados da sessão recuperada
            return sessao;              
#endif
        }

        [Obsolete("Utilizar GeraPainelExcecao(String,String,Int32,String)")]
        public void GeraPainelErro(String mensagem, Int32 codigo, String redirecionamento)
        {
            this.GeraPainel("Erro", mensagem, codigo, redirecionamento);
        }

        [Obsolete("Utilizar GeraPainelExcecao(String,String,Int32,String)")]
        public void GeraPainelMensagem(String titulo, String mensagem, Int32 codigo)
        {
            this.GeraPainel(titulo, mensagem, codigo, String.Empty);            
        }

        [Obsolete("Utilizar GeraPainelExcecao(String,String,Int32,String)")]
        public void GeraPainelMensagem(String titulo, String mensagem)
        {
            this.GeraPainel(titulo, mensagem, null, String.Empty);
        }
        
        [Obsolete("Remover método: não faz nada")]
        public void AtualizaSession()
        {
            if (Request.QueryString["dados"] != null)
            {
                //QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
                //Session["urlParent"] = qs["url"];
            }
        }

        /// <summary>
        /// Gera popup de mensagens de aviso
        /// </summary>
        /// <param name="titulo">Título</param>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="codigo">Código</param>
        /// <param name="redirecionamento">URL de redirecionamento</param>
        private void GeraPainel(String titulo, String mensagem, Int32? codigo, String redirecionamento)
        {
            String script = String.Format("exibirMensagem('{0}', '{1}', '{2}', '{3}');",
                redirecionamento, mensagem, codigo == null ? String.Empty : codigo.ToString(), titulo);
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        #region [ Exibir Painel de Exceção ]

        /// <summary>
        /// Faça Sua Venda: Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        [Obsolete("Utilizar GeraPainelExcecao(String,String,Int32,String)")]
        public void GeraPainelExcecao(String mensagem, String codigo)
        {
            String script = String.Format("exibirMensagem('{0}', '{1}', '{2}');", "pn_FacaSuaVenda.aspx", mensagem, codigo);
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        /// <param name="titulo">Título</param>
        /// <param name="urlRedirecionamento">URL de redirecionamento</param>
        protected void ExibirPainelExcecao(String titulo, String fonte, Int32 codigo, String urlRedirecionamento)
        {
            PainelMensagem.ExibirPainelExcecao(this, titulo, fonte, codigo, urlRedirecionamento);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        /// <param name="urlRedirecionamento">URL de redirecionamento</param>
        protected void ExibirPainelExcecao(String fonte, Int32 codigo, String urlRedirecionamento)
        {
            PainelMensagem.ExibirPainelExcecao(this, fonte, codigo, urlRedirecionamento);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        protected void ExibirPainelExcecao(String fonte, Int32 codigo)
        {
            PainelMensagem.ExibirPainelExcecao(this, fonte, codigo);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="ex">Exceção contendo a mensagem e código do erro</param>
        protected void ExibirPainelExcecao(PortalRedecardException ex)
        {
            PainelMensagem.ExibirPainelExcecao(this, ex);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="ex">Exceção contendo a mensagem e código do erro</param>
        /// <param name="urlRedirecionamento">URL de redirecionamento</param>
        protected void ExibirPainelExcecao(PortalRedecardException ex, String urlRedirecionamento)
        {
            PainelMensagem.ExibirPainelExcecao(this, ex, urlRedirecionamento);
        }

        #endregion

        /// <summary>
        /// Redirects a client to a new URL. Specifies the new URL and whether execution
        /// of the current page should terminate.
        /// </summary>
        /// <param name="url">The location of the target.</param>
        public void Redirecionar(String url)
        {
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Método para tratar erro de session
        /// </summary>
        /// <param name="mensagem">Mensagem que deve ser exibida</param>
        public void GeraExcecao(String mensagem)
        {
            ExibirPainelExcecao(mensagem, FONTE, CODIGO_ERRO, "/sites/fechado/Ecommerce/_layouts/Redecard.Comum/SessaoExpirada.aspx");
        }
    }
}