using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Redecard.PN.Comum;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.RedirecionarEadquirencia
{
    public partial class RedirecionarEadquirenciaUserControl : WebpartBase
    {
        public RedirecionarEadquirencia ParentWebPart
        {
            get
            {
                return (this.Parent as RedirecionarEadquirencia);
            }
        }

        #region [Eventos]

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ExecucaoTratada("RedirecionarEadquirenciaUserControl - Page_Load", () =>
                {
#if !DEBUG
                    if (!Sessao.Contem())
                        throw new Exception("Falha ao obter sessão.");
#endif
                    // Carregar url para o iframe
                    String iframe =
                        String.Format("<iframe id='ifrEadquirencia' src = '{0}' width='{1}' height='{2}' border='0' frameborder='0' scrolling='no'></iframe>"
                                        , this.MontarUrl()
                                        , ParentWebPart.Largura
                                        , ParentWebPart.Altura);

                    // Habilitar transação
                    HabilitarTransacao();

                    this.Controls.Add(new LiteralControl(iframe));
                });
            }
        }

        #endregion [Fim - Eventos]

        #region [Métodos]

        /// <summary>
        /// Montar a URL de envio para o Eadquirencia
        /// </summary>
        protected String MontarUrl()
        {
            QueryStringSegura dados = ObterQueryString();

            return String.Format("{0}?dados={1}", this.ParentWebPart.URLEadquirencia, dados.ToString());
        }

        /// <summary>
        /// Obter query string
        /// </summary>
        /// <returns>retorna query string</returns>
        private QueryStringSegura ObterQueryString()
        {
            //Gera guid para vínculo do objeto de sessão no cache
            Int32 codigoGrupoRamo = 0;
            Int32 codigoRamoAtividade = 0;
            Int32 codigoEntidade = 0;
            String nomeEntidade = String.Empty;
            Int32 codigoIdUsuario = 0;

            codigoGrupoRamo         = SessaoAtual.CodigoGrupoRamo;
            codigoRamoAtividade     = SessaoAtual.CodigoRamoAtividade;
            codigoEntidade          = SessaoAtual.CodigoEntidade;
            codigoIdUsuario         = SessaoAtual.CodigoIdUsuario;
            nomeEntidade            = SessaoAtual.NomeEntidade;

            //Monta querystring
            QueryStringSegura dados = new QueryStringSegura();
            dados.Add("id", Guid.NewGuid().ToString("N"));
            dados.Add("parentUrl", Request.Url.GetLeftPart(UriPartial.Path));
            dados.Add("CodigoGrupoRamo", codigoGrupoRamo.ToString());
            dados.Add("CodigoRamoAtividade", codigoRamoAtividade.ToString());
            dados.Add("CodigoEntidade", codigoEntidade.ToString());
            dados.Add("NomeEntidade", nomeEntidade.ToString());
            dados.Add("CodigoUsuario", codigoIdUsuario.ToString());

            CarregarDadosEspecificosQueryString(dados);

            //dados.ExpireTime = DateTime.Now.AddMinutes(1);

            if (Request.QueryString["dados"] != null)
            {
                QueryStringSegura dadosWebPart = new QueryStringSegura(Request.QueryString["dados"]);
                foreach (String key in dadosWebPart.Keys)
                {
                    dados.Add(key, dadosWebPart[key]);
                }
            }
            return dados;
        }

        /// <summary>
        /// Carrega as informações específicas da query string
        /// </summary>
        /// <param name="queryStringSegura">Query string com as informações específicas</param>
        private void CarregarDadosEspecificosQueryString(QueryStringSegura queryStringSegura)
        {
            switch (ParentWebPart.ConfiguracaoRedirecionamento)
            {
                case TipoRedirecionamento.FacaSuaVenda:
                    CarregarDadosEspecificosFacaSuaVenda(queryStringSegura);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Carrega as informações específicas do faça sua venda
        /// </summary>
        /// <param name="queryStringSegura"></param>
        private void CarregarDadosEspecificosFacaSuaVenda(QueryStringSegura queryStringSegura)
        {
            queryStringSegura.Add("NumeroParcelas", ObterNumeroParcelas().ToString());
        }

        /// <summary>
        /// Habilita a transação entre o PN e o front-end EC
        /// </summary>
        /// <returns></returns>
        protected void HabilitarTransacao()
        {
            Int32 numeroPv;

            if (!this.SessaoAtual.PossuiEadquirencia)
                throw new Exception("Este PV não possui o produto E-rede adquirência, verifique os serviços e as tecnologias cadastradas.");

            numeroPv = SessaoAtual.CodigoEntidade;
            using (ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient> contexto = new ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient>())
            {
                contexto.Cliente.InserirTimeoutLoginAdquirencia(numeroPv);
            }
        }

        /// <summary>
        /// Obter número das parcelas
        /// </summary>
        private Int32 ObterNumeroParcelas()
        {
            Int32 numeroParcelas;

            using (Logger log = Logger.IniciarLog(String.Format("Consultar número parcelas: ConsultarNumeroParcelas(CodigoGrupoRamo:{0}, CodigoRamoAtividade:{1})", SessaoAtual.CodigoGrupoRamo, SessaoAtual.CodigoRamoAtividade)))
            {
                using (ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient> contexto = new ContextoWCF<EadquirenciaServico.ServicoEAdquirenciaClient>())
                {
                    numeroParcelas = contexto.Cliente.ConsultarNumeroParcelas(SessaoAtual.CodigoGrupoRamo, SessaoAtual.CodigoRamoAtividade);
                }
            }

            return numeroParcelas;
        }

        #endregion [Fim - Métodos]
    }
}
