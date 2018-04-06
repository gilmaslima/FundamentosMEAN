using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using System.IO;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
using System.Web;
using System.Globalization;
using Redecard.PN.Extrato.SharePoint.WebParts.RelatoriosDetalhe;
using Microsoft.SharePoint.Utilities;
using ConsultarVendas = Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasListagem;

namespace Redecard.PN.Extrato.SharePoint
{
    public class BaseUserControl : UserControlBase
    {
        /// <summary>
        /// Linhas máximas para download
        /// </summary>
        public const Int32 MAX_LINHAS_DOWNLOAD = Constantes.MAX_LINHAS_DOWNLOAD;

        /// <summary>
        /// Culture-Info PT-BR
        /// </summary>
        protected static CultureInfo PtBR
        {
            get { return new CultureInfo("pt-BR"); }
        }

        public new void ExibirPainelExcecao(String fonte, Int32 codigo)
        {
            //Oculta os controles com os dados do relatório, visto que houve uma exceção durante a consulta
            this.VerificaControlesVisiveis(0, null, null);
            switch (codigo)
            {
                case 10:
                    if (fonte.Equals("ConsultarCreditoAG.ConsultarCredito") || fonte.Equals("ConsultarDebitoAG.ConsultarDebito") || fonte.Equals("ConsultarDebitoAG.ConsultarDebitoCdcConstrucard")
                        || fonte.Equals("ConsultarOrdensCreditoAG.ConsultarOrdensCreditoEnviadosAoBanco") || fonte.Equals("ConsultarRAVAG.ConsultarAntecipacao")
                        || fonte.Equals("ConsultarCreditoAG.ConsultarCreditoValoresPagos") || fonte.Equals("ConsultarDebitoAG.ConsultarValoresPagosCartaoDebito")
                        || fonte.Equals("ConsultarRAVAG.ConsultarAntecipacaoLancamentosFuturos") || fonte.Equals("ConsultarCreditoAG.ConsultarMovimentosFinanceirosLancamentosFuturos")
                        || fonte.Equals("ConsultaHomepageAG.ConsultarTransacoesCreditoDebito") || fonte.Equals("ConsultarOrdensCreditoAG.ConsultarOrdensCredito"))
                        return;
                    else if (new[] {
                        "AntecipacaoRAVAG", "ConsultaTransacaoAG", "LancamentosFuturosAG", "OrdensCreditoAG", 
                        "ResumoVendasAG", "RecargaCelularAG", "ServicosAG", "ValoresPagosAG", "VendasAG" }
                            .Any(f => fonte.StartsWith(f + ".")))
                        return;
                    else
                        break;
                case 20:
                    if (new[] {
                        "ConsultaTransacaoBLL" }.Any(f => fonte.StartsWith(f + ".")))
                        return;
                    break;
                case 30:
                    if (fonte.Equals("ConsultarExtratosAG.ConsultarConsolidadoDebitosEDesagendamento"))
                        return;
                    else
                        break;
                case 40:
                    if (fonte.Equals("ConsultarDebitoAG.ConsultarTransacoesDebito") ||
                        fonte.Equals("Redecard.PN.Extrato.Agentes.ConsultarDebitoAG"))
                        return;
                    else
                        break;
                case 50:
                    if (fonte.Equals("ConsultarCreditoAG.ConsultarTransacoesCredito"))
                        return;
                    else
                        break;
                case 60:
                    if (String.Compare("ResumoVendasAG.ConsultarRecargaCelularResumo", fonte, true) == 0)
                    {
                        break;
                    }
                    else if (fonte.StartsWith("AntecipacaoRAVAG")
                        || fonte.StartsWith("ConsultaTransacaoAG")
                        || fonte.StartsWith("LancamentosFuturosAG")
                        || fonte.StartsWith("OrdensCreditoAG")
                        || fonte.StartsWith("ResumoVendasAG")
                        || fonte.StartsWith("ServicosAG")
                        || fonte.StartsWith("ValoresPagosAG")
                        || fonte.StartsWith("VendasAG")
                        || fonte.StartsWith("RecargaCelularAG"))
                    {
                        return;
                    }
                    else
                        break;
            }

            base.ExibirPainelExcecao(fonte, codigo);
        }

        public new void ExibirPainelExcecao(String mensagem, String codigo)
        {
            //Oculta os controles com os dados do relatório, visto que houve uma exceção durante a consulta
            this.VerificaControlesVisiveis(0, null, null);

            base.ExibirPainelExcecao(mensagem, codigo);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                CriarHistoricoNavegacao(new BuscarDados(), false);
            }
            else
            {
                //Conta a quantidade de posts
                QuantidadePost++;
            }
        }


        /// <summary>
        /// Cria e mantem o historico de navegação para o botão voltar funcionar e recarregar a consulta realizada
        /// </summary>
        /// <param name="BuscarDados">Parametros da consulta realizada</param>
        /// <param name="ConsultaEfetuada">Se a consulta foi realizada</param>
        public void CriarHistoricoNavegacao(BuscarDados BuscarDados, bool ConsultaEfetuada)
        {
            List<PaginaVisitada> objList = Session["UltimasPaginasVisitadas"] == null ? new List<PaginaVisitada>() : Session["UltimasPaginasVisitadas"] as List<PaginaVisitada>;
            PaginaVisitada objPagina = objList.Find(x => x.Nome.Contains(Request.ServerVariables["PATH_INFO"]));
            if (objPagina != null)
            {
                if (!objPagina.ConsultaEfetuada || ConsultaEfetuada)
                {
                    objPagina.BuscarDados = BuscarDados;
                    objPagina.ConsultaEfetuada = ConsultaEfetuada;
                }
                objPagina.Nome = Request.ServerVariables["PATH_INFO"];
                objPagina.UltimoAcesso = DateTime.Now;
            }
            else
            {
                objList.Add(new PaginaVisitada() { Nome = Request.ServerVariables["PATH_INFO"], ConsultaEfetuada = ConsultaEfetuada, BuscarDados = BuscarDados, UltimoAcesso = DateTime.Now });
            }

            //mantem apenas 2 registros
            if (objList.Count > 2)
            {
                objList = objList.OrderBy(x => x.UltimoAcesso).ToList();
                objList.RemoveRange(0, objList.Count - 2);
            }

            Session["UltimasPaginasVisitadas"] = objList;
        }

        private static Dictionary<String, List<SPListaPadrao>> _listaSP;
        private static Dictionary<String, List<SPListaPadrao>> ListaSP
        {
            get { return _listaSP ?? (_listaSP = new Dictionary<String, List<SPListaPadrao>>()); }
        }

        /// <summary>
        /// Recupera os dados de uma lista padrão do SP, é obrigatorio a Lista ter os campos "Título" e "Valor"
        /// </summary>
        /// <param name="NomeLista">Nome da Lista do Site</param>
        /// <returns>A Lista em formato de VO</returns>
        public List<SPListaPadrao> GetListaSP(string NomeLista)
        {
            //Variável de retorno             
            List<SPListaPadrao> objList = null;

            try
            {
                if (ListaSP.ContainsKey(NomeLista))
                    objList = ListaSP[NomeLista];
                else
                {
                    objList = new List<SPListaPadrao>();
                    SPList objListPeriodo = SPContext.Current.Web.Lists[NomeLista];
                    if (objListPeriodo != null)
                    {
                        //Converte SPListItemCollection para uma List<SPListaPadrao>
                        //recuperar coluna valor
                        foreach (SPItem item in objListPeriodo.Items)
                        {
                            SPListaPadrao objItem = new SPListaPadrao();
                            objItem.Titulo = item["Título"].ToString();
                            objItem.Valor = item["Valor"].ToString();
                            objItem.Descricao = item.Fields.ContainsField("Descricao") ? item["Descricao"].ToString() : String.Empty;
                            objList.Add(objItem);
                        }
                    }
                    ListaSP.Add(NomeLista, objList);
                }
            }
            catch (Exception exc)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.GetListaSP[" + NomeLista + "]");
                SharePointUlsLog.LogErro(exc);
            }

            return objList ?? new List<SPListaPadrao>();
        }

        /// <summary>
        /// Recupera a Linha pela Coluna Valor
        /// </summary>
        /// <param name="Valor">Valor</param>
        /// <returns>O objeto econtrado</returns>
        public Predicate<SPListaPadrao> SPListaPadraoPorValor(string Valor)
        {
            return delegate(SPListaPadrao obj)
            {
                return obj.Valor == Valor;
            };
        }

        private List<SPListaPadrao> _SPListaBanco = null;
        protected List<SPListaPadrao> SPListaBanco { get { return _SPListaBanco ?? (_SPListaBanco = GetListaSP(Constantes.Extrato_Lista_Banco)); } }

        protected String ObterNomeBanco(Int32 bancoCredito)
        {
            SPListaPadrao listaBanco = SPListaBanco.Find(SPListaPadraoPorValor(bancoCredito.ToString()));
            return listaBanco == null ? bancoCredito.ToString() : listaBanco.Titulo.PadLeft(10, ' ').Substring(0, 10).TrimStart(' ');
        }

        /// <summary>
        /// Monta as Bandeiras em linha horizontal em HTML
        /// </summary>
        /// <param name="Bandeiras">Lista das Bandeiras</param>
        public void MontaBandeira(List<BandeiraPadrao> Bandeiras, HtmlTable TabelaDestino)
        {
            HtmlTableRow objTbRow1 = new HtmlTableRow();
            objTbRow1.Attributes.Add("class", "trBandeira1");
            HtmlTableRow objTbRow2 = new HtmlTableRow();
            objTbRow2.Attributes.Add("class", "trBandeira2");
            HtmlTableRow objTbRow3 = new HtmlTableRow();
            objTbRow3.Attributes.Add("class", "trBandeira3");
            HtmlTableCell objTbCell1, objTbCell2;
            Image objImage;
            HtmlGenericControl objSpan1, objSpan2;

            //monta resultados das bandeiras
            for (int i = 0; i < Bandeiras.Count; i++)
            {
                //montando a celula da imagem
                objTbCell1 = new HtmlTableCell();
                objImage = new Image();
                objImage.ImageUrl = "/_layouts/Redecard.PN.Extrato.SharePoint/Styles/ico_" + Bandeiras[i].Bandeira.Trim() + ".jpg";
                objImage.CssClass = "imgBandeira";
                objTbCell1.Attributes.Add("class", "tdImgBandeira");
                objTbCell1.Controls.Add(objImage);

                //celula do nome e valor
                objTbCell2 = new HtmlTableCell();
                objSpan1 = new HtmlGenericControl("span");
                objSpan1.Attributes.Add("class", "spanNomeBandeira");
                objSpan1.InnerText = Bandeiras[i].Bandeira;
                objSpan1.Style.Add("font-weight", "bold");
                objTbCell2.Controls.Add(objSpan1);
                //objTbCell2.Controls.Add(new HtmlGenericControl("br"));
                objTbCell2.Attributes.Add("class", "tdDadosBandeira");
                objSpan2 = new HtmlGenericControl("span");
                objSpan2.Attributes.Add("class", "spanValorBandeira");
                objSpan2.InnerText = Bandeiras[i].Valor;
                objTbCell2.Controls.Add(objSpan2);

                if (i % 2 == 0)
                {
                    objTbRow1.Cells.Add(objTbCell1);
                    objTbRow1.Cells.Add(objTbCell2);
                }
                else
                {
                    objTbRow2.Cells.Add(objTbCell1);
                    objTbRow2.Cells.Add(objTbCell2);
                }
            }

            TabelaDestino.Rows.Add(objTbRow1);
            TabelaDestino.Rows.Add(objTbRow2);
        }

        public Guid GuidUsuario()
        {
            Guid? guid = Session["GuidUsuario"] as Guid?;
            if (guid == null)
            {
                guid = Guid.NewGuid();
                Session["GuidUsuario"] = guid;
            }
            return (Guid)guid;
        }


        public Guid GuidPesquisa()
        {
            Guid? guid = ViewState["guidPesquisa"] as Guid?;

            //cria ou recupera os guids de consulta
            if (guid == null)
            {
                guid = Guid.NewGuid();
                ViewState["guidPesquisa"] = guid;
            }
            return (Guid)ViewState["guidPesquisa"];
        }

        public void GuidPesquisa(Guid guid)
        {
            ViewState["guidPesquisa"] = guid;
        }

        public Guid GuidPesquisaTotalizador()
        {
            Guid? guid = ViewState["guidPesquisaTotalizador"] as Guid?;

            //cria ou recupera os guids de consulta
            if (guid == null)
            {
                guid = Guid.NewGuid();
                ViewState["guidPesquisaTotalizador"] = guid;
            }
            return (Guid)ViewState["guidPesquisaTotalizador"];
        }

        public void GravarBuscarDados(BuscarDados buscarDados)
        {
            GravarBuscarDados<BuscarDados>(buscarDados);
        }

        public void GravarBuscarDados<T>(T buscarDados)
        {
            ViewState["dataBuscarDados"] = buscarDados;
        }

        public BuscarDados ObterBuscarDados()
        {
            return ObterBuscarDados<BuscarDados>();
        }

        public String _QSDados
        {
            get { return (String)ViewState["__QSDados__"]; }
            set { ViewState["__QSDados__"] = value; }
        }

        public QueryStringSegura QS
        {
            get
            {
                if (_QSDados == null)
                    if (Request.QueryString["dados"] != null)
                        _QSDados = Request.QueryString["dados"];
                return _QSDados != null ? new QueryStringSegura(_QSDados) : null;
            }
        }

        /// <summary>
        /// Recupera da session uma informacao de transiçao entre páginas e exclui este valor da session.
        /// Este método é utilizado para passar informaçoes entre as paginas, e é baseado no tipo passado.
        /// Como a idéia é que o mesmo seja totalmente transitório, o método é projetado para armazenar um valor por vez, e que assim que navegaçao chegar na próxima pagina,
        /// o dado deve ser removido da session e armazenado no ViewState.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Valor armazenado ou null caso nao haja informacao deste tipo na session</returns>
        public T RetiraInformacaoTransicaoSession<T>(String key)
        {

            String chave = "informacaoTransicao_" + key + "_" + typeof(T).Name;
            T valor = (T)Session[chave];
            if (valor != null)
            {
                Session.Remove(chave);
            }
            return valor;

        }

        public static T RetiraInformacaoTransicaoSession<T>(String key, System.Web.SessionState.HttpSessionState session)
        {
            String chave = "informacaoTransicao_" + key + "_" + typeof(T).Name;
            T valor = (T)session[chave];
            if (valor != null)
            {
                session.Remove(chave);
            }
            return valor;
        }

        /// <summary>
        /// Armazena na session uma informacao de transiçao entre páginas.
        /// Este método é utilizado para passar informaçoes entre as paginas, e é baseado no tipo do dado a ser transferido.
        /// Como a idéia é que o mesmo seja totalmente transitório, o método é projetado para armazenar um valor por vez, e que assim que navegaçao chegar na próxima pagina,
        /// o dado deve ser removido da session e armazenado no ViewState.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void ArmazenaInformacaoTransicaoSession<T>(String key, T valor)
        {
            String chave = "informacaoTransicao_" + key + "_" + typeof(T).Name;
            Session[chave] = valor;
        }

        public static void ArmazenaInformacaoTransicaoSession<T>(String key, T valor, System.Web.SessionState.HttpSessionState session)
        {
            String chave = "informacaoTransicao_" + key + "_" + typeof(T).Name;
            session[chave] = valor;
        }

        public T ObterBuscarDados<T>()
        {
            T buscarDados = (T)ViewState["dataBuscarDados"];
            if (buscarDados == null)
            {
                buscarDados = (T)ViewState["BuscarDados"];
            }

            if (buscarDados == null)
            {
                throw new Exception("Buscar dados nao encontrado");
            }
            return buscarDados;
        }

        protected HyperLink ObterHyperLinkResumoVenda(
            String tipoVenda,
            Int32 numeroResumoVenda,
            Int32 numeroEstabelecimento,
            DateTime dataApresentacao)
        {
            var queryString = new QueryStringSegura();
            queryString["tipoVenda"] = tipoVenda;
            queryString["numeroResumoVenda"] = numeroResumoVenda.ToString();

            queryString["tipoBusca"] = ((int)ConsultarVendas.ConsultarVendasListagemUserControl.TipoBusca.ResumoVendas).ToString();

            switch (tipoVenda)
            {
                case "D":
                    queryString["tipoVenda"] = ((int)ConsultarVendas.ConsultarVendasListagemUserControl.TipoVenda.Debito).ToString();
                    break;
                case "CDC":
                    queryString["tipoVenda"] = ((int)ConsultarVendas.ConsultarVendasListagemUserControl.TipoVenda.Construcard).ToString();
                    break;
                case "RC":
                    queryString["tipoVenda"] = ((int)ConsultarVendas.ConsultarVendasListagemUserControl.TipoVenda.Recarga).ToString();
                    break;
                case "C":
                default:
                    queryString["tipoVenda"] = ((int)ConsultarVendas.ConsultarVendasListagemUserControl.TipoVenda.Credito).ToString();
                    break;
            }

            queryString["numero"] = numeroResumoVenda.ToString();
            queryString["numeroEstabelecimento"] = numeroEstabelecimento.ToString();
            queryString["dataApresentacao"] = dataApresentacao.ToString(Constantes.Formatador.FormatoDataPadrao);
			queryString["dataInicial"] = dataApresentacao.ToString(Constantes.Formatador.FormatoDataPadrao);

            String url = String.Format("/sites/fechado/extrato/paginas/pn_ConsultarVendasResumo.aspx?dados={0}", queryString.ToString());

            HyperLink result = new HyperLink();
            result.NavigateUrl = url;
            result.Text = numeroResumoVenda.ToString();
            result.Attributes.Add("onclick", "blockUI();");
            return result;
        }

        protected String ObterUrlLinkDetalhe(QueryStringSegura queryString)
        {
            BuscarDados dados = ObterBuscarDados();

            //Gera guid do objeto de busca que será armazenado em sessão, 
            //e recuperado posteriormente na tela de detalhe
            String guidBuscarDados = Guid.NewGuid().ToString();
            queryString["guidBuscarDados"] = guidBuscarDados;
            ArmazenaInformacaoTransicaoSession(guidBuscarDados, dados);

            String webURL = "";
            //Se SPContext for nulo, cria contexto
            try
            {
                if (SPContext.Current == null)
                {
                    String requestUrl = Request.Url.ToString();
                    requestUrl = requestUrl.Substring(0, requestUrl.LastIndexOf("/"));

                    using (SPSite site = new SPSite(requestUrl))
                    using (SPWeb web = site.OpenWeb())
                        webURL = web.Url;
                }
                else
                    webURL = SPContext.Current.Web.Url;

                return String.Format("{0}/Paginas/pn_RelatoriosDetalhe.aspx?dados={1}", webURL, queryString.ToString());
            }
            catch (Exception e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlPaginaDetalhe");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
        }

        protected void RedirecionarRelatorio(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda,
            DateTime dataInicial, DateTime dataFinal, Boolean pesquisar)
        {
            SPUtility.Redirect(this.ObterUrlLinkRelatorio(tipoRelatorio, tipoVenda,
                dataInicial, dataFinal, pesquisar), SPRedirectFlags.CheckUrl, this.Context);
        }

        protected String ObterUrlLinkRelatorio(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda,
            DateTime dataInicial, DateTime dataFinal, Boolean pesquisar)
        {
            String webURL = "";
            try
            {
                String requestUrl = Request.Url.ToString();
                requestUrl = requestUrl.Substring(0, requestUrl.LastIndexOf("/"));

                using (SPSite site = new SPSite(requestUrl))
                using (SPWeb web = site.AllWebs["extrato"])
                    webURL = web.Url;

                QueryStringSegura qs = new QueryStringSegura();
                qs["DataInicial"] = dataInicial.ToString("dd/MM/yyyy");
                qs["DataFinal"] = dataFinal.ToString("dd/MM/yyyy");
                qs["TipoRelatorio"] = ((Int32)tipoRelatorio).ToString();
                qs["TipoVenda"] = ((Int32)tipoVenda).ToString();
                qs["Pesquisar"] = pesquisar ? Boolean.TrueString : Boolean.FalseString;

                return String.Format("{0}/Paginas/pn_Relatorios.aspx?dados={1}", webURL, qs.ToString());
            }
            catch (QueryStringExpiradaException e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
            catch (QueryStringInvalidaException e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
            catch (Exception e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
        }

        protected String ObterUrlLinkDetalhe(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda, QueryStringSegura queryString)
        {
            return ObterUrlLinkDetalhe(tipoRelatorio, tipoVenda, queryString, "pn_RelatoriosDetalhe.aspx");
        }

        protected String ObterUrlLinkDetalhe(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda, QueryStringSegura queryString, String nomePaginaDetalhe)
        {
            BuscarDados dados = new BuscarDados();
            dados.IDRelatorio = (Int32)tipoRelatorio;
            dados.IDTipoVenda = (Int32)tipoVenda;

            //Gera guid do objeto de busca que será armazenado em sessão, 
            //e recuperado posteriormente na tela de detalhe
            String guidBuscarDados = Guid.NewGuid().ToString();
            queryString["guidBuscarDados"] = guidBuscarDados;
            ArmazenaInformacaoTransicaoSession(guidBuscarDados, dados);

            String webURL = "";
            //Se SPContext for nulo, cria contexto
            try
            {
                if (SPContext.Current == null)
                {
                    String requestUrl = Request.Url.ToString();
                    requestUrl = requestUrl.Substring(0, requestUrl.LastIndexOf("/"));

                    using (SPSite site = new SPSite(requestUrl))
                    using (SPWeb web = site.OpenWeb())
                        webURL = web.Url;
                }
                else
                    webURL = SPContext.Current.Web.Url;

                return String.Format("{0}/Paginas/{1}?dados={2}", webURL, nomePaginaDetalhe, queryString.ToString());
            }
            catch (PortalRedecardException e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlPaginaDetalhe");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
            catch (Exception e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlPaginaDetalhe");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
        }

        /// <summary>
        /// Processa um Control e recupera dele o conteúdo em HTML para download ou envio de email.
        /// Este Control pode ser um placeholder com vários controles dentro.
        /// A única observaçao é que existem alguns controles (ex: gridview) que nao funcionam aqui, pois dependem de um FORM.
        /// Entao as paginas precisam retornar controles que nao sejam deste tipo (no caso do gridview, o table de dentro do gridview já funciona)
        /// </summary>
        /// <param name="control">O controle a ser renderizado em tabela. Normalmente é um HTMLTable. Pode ser um PlaceHolder com vários controles dentro.</param>
        /// <returns></returns>
        protected String ObterHTMLControle(Control control)
        {
            if (control == null)
            {
                return String.Empty;
            }
            List<Control> controls = new List<Control>();
            if (control is GridView)
            {
                // Se foi passado um gridview diretamente, obtem a tabela de dentro dele
                if (control.Controls.Count > 0)
                {
                    controls.Add(control.Controls[0]);
                }
            }
            else if (control is HtmlTable)
            {
                // Se foi passado um HTMLTable
                controls.Add(control);
            }
            else if (control is PlaceHolder)
            {
                // Se foi passado um PlaceHolder, obtem os controles de dentro para renderizar um por um 
                controls = ObterControles(control);
            }
            else
            {
                // Usa o próprio controle fornecido
                controls.Add(control);
            }

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            foreach (Control c in controls)
            {
                c.RenderControl(htmlWrite);
            }

            return stringWrite.ToString();
        }

        private List<Control> ObterControles(Control control)
        {
            List<Control> result = new List<Control>();
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control c = control.Controls[i];
                result.Add(c);
            }
            return result;
        }

        /* Não Apagar*/
        protected String ObterUrlLinkResumoVenda(string tipoVenda, int numeroResumoVenda, int numeroEstabelecimento, DateTime dataApresentacao)
        {
            Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
            queryString["tipoVenda"] = tipoVenda;
            queryString["numeroResumoVenda"] = numeroResumoVenda.ToString();
            queryString["numeroEstabelecimento"] = numeroEstabelecimento.ToString();
            queryString["dataApresentacao"] = dataApresentacao.ToString(Constantes.Formatador.FormatoDataPadrao);

            String webURL = "";

            //Se SPContext for nulo, cria contexto
            try
            {
                if (SPContext.Current == null)
                {
                    String requestUrl = Request.Url.ToString();
                    requestUrl = requestUrl.Substring(0, requestUrl.LastIndexOf("/"));

                    using (SPSite site = new SPSite(requestUrl))
                    using (SPWeb web = site.OpenWeb())
                        webURL = string.Format("/Paginas/pn_ResumoVendas.aspx?dados={0}", queryString.ToString());
                }
                else
                    webURL = SPContext.Current.Web.Url;

                return String.Format("{0}/Paginas/pn_ResumoVendas.aspx?dados={1}", webURL, queryString.ToString());
            }
            catch (Exception e)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkResumoVenda");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
        }

        /// <summary>
        /// Redireciona para página fechada a partir das páginas cadastradas
        /// para o usuário.
        /// </summary>
        /// <param name="site">Opcional. Identificação do site (ex: servicos, minhaconta, extrato, ...).
        /// Se nulo, busca em todos os sites.</param>
        /// <param name="urlPagina">URL da página</param>
        /// <param name="queryString">QueryString customizada</param>
        /// <returns>True: se redirecionou; false, caso contrário</returns>
        public Boolean RedirecionarPaginaPN(String site, String urlPagina, QueryStringSegura queryString)
        {
            String url = ProcurarUrlPaginaPN(SessaoAtual, site, urlPagina).FirstOrDefault();
            if (!String.IsNullOrEmpty(url))
            {
                url = String.Format("{0}?dados={1}", url, queryString.ToString());
                Response.Redirect(url, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Recupera a URL de uma página a partir das páginas cadastradas
        /// para o usuário, através da URL da mesma.
        /// </summary>
        /// <param name="site">Opcional. Identificação do site (ex: servicos, minhaconta, extrato, ...).
        /// Se nulo, busca em todos os sites.</param>
        /// <param name="urlPagina">URL da página</param>
        /// <returns>URLs das páginas encontradas</returns>
        public static List<String> ProcurarUrlPaginaPN(Sessao sessaoAtual, String site, String urlPagina)
        {
            if (Sessao.Contem())
            {
                //Seleciona as páginas que atendem aos critérios
                List<String> paginas = sessaoAtual.Paginas
                    .Where(pagina =>
                    {
                        String[] tokens = pagina.Url.Split('/');
                        return
                            pagina.Url.StartsWith("/")
                            && tokens.Last().IndexOf(urlPagina, StringComparison.OrdinalIgnoreCase) >= 0
                            && (String.IsNullOrEmpty(site) ||
                            String.Compare(site, tokens.Reverse().Skip(2).FirstOrDefault(), true) == 0);
                    }).Select(pagina => pagina.Url)
                    .ToList();

                return paginas;
            }
            else
                return new List<String>(new[] { String.Concat("/sites/fechado/", site, "/Paginas/", urlPagina) });
        }

        /// <summary>
        /// Carrega atalhos da Home, verificando permissões do usuário
        /// </summary>
        /// <returns>Boolean indicando se algum Atalho foi adicionado ao controle</returns>
        protected Boolean CarregarAtalhosHome(Repeater repeater, String configuracaoAtalhos)
        {
            var lista = new List<KeyValuePair<String, String>>();

            if (configuracaoAtalhos != null)
            {
                String[] atalhos = configuracaoAtalhos.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String atalho in atalhos)
                {
                    String urlAtalho = default(String);
                    String textoAtalho = default(String);

                    //Verifica se usuário possui acesso à página
                    if (VerificarPermissaoAtalhosHomeAreaFechada(atalho, out textoAtalho, out urlAtalho))
                        lista.Add(new KeyValuePair<string, string>(textoAtalho, urlAtalho));
                }
            }

            repeater.ItemDataBound += (s, ev) =>
            {
                if (ev.Item.ItemType == ListItemType.Item || ev.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var item = (KeyValuePair<String, String>)ev.Item.DataItem;
                    var lnkAtalho = ev.Item.FindControl("lnkAtalho") as HyperLink;
                    lnkAtalho.Text = item.Key.Trim();
                    lnkAtalho.NavigateUrl = item.Value.Trim();
                }
            };
            repeater.DataSource = lista;
            repeater.DataBind();

            //Retorna indicando se algum controle foi incluído no controle
            return lista.Count > 0;
        }

        /// <summary>
        /// Verifica se o usuário possui permissão de acesso à determinada URL<br/>
        /// </summary>
        /// <param name="configuracaoAtalho">TextoBotao;URL</param>
        protected Boolean VerificarPermissaoAtalhosHomeAreaFechada(String configuracaoAtalho, out String texto, out String url)
        {
            //Recupera o texto e a URL do atalho "texto;url"
            String[] tokens = configuracaoAtalho.Split(';');
            if (tokens.Length > 1)
            {
                texto = tokens[0];
                url = tokens[1];

                if (!String.IsNullOrEmpty(texto) && !String.IsNullOrEmpty(url))
                {
                    if (Sessao.Contem())
                        return !url.StartsWith("/sites/fechado/") || base.ValidarPagina(url);
                    else
                        return true;
                }
            }

            //Se não possui permissão, anula as variáveis output
            texto = null;
            url = null;
            return false;
        }

        /// <summary>
        /// Valida o acesso a determinado serviço especificado no paramêtro "codigoServico" de 
        /// acordo com a relação de permissões do usuário
        /// </summary>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Serviço disponível para o usuário</returns>
        public new Boolean ValidarServico(Int32 codigoServico)
        {
            if (Sessao.Contem())
            {
                var servicos = this.SessaoAtual.Servicos.Flatten(servico => servico.Items);
                return servicos.Any(servico => servico.Codigo == codigoServico);
            }
            else
                return SPContext.Current.Web.CurrentUser != null;
        }

        /// <summary>Contador de posts realizados</summary>
        public int QuantidadePost
        {
            get
            {
                if (ViewState["QuantidadePost"] == null)
                    ViewState["QuantidadePost"] = 0;
                return Convert.ToInt32(ViewState["QuantidadePost"]);
            }
            set { ViewState["QuantidadePost"] = value; }
        }

        /// <summary>Retorna para a página anterior</summary>
        protected void RetornarPaginaAnterior()
        {
            RetornarPaginaAnterior((QuantidadePost + 1) * -1);
        }

        protected void RetornarPaginaAnterior(Int32 quantidade)
        {
            String retorno = quantidade.ToString();
            Page.ClientScript.RegisterStartupScript(typeof(string), "RetornaPagina", string.Format("history.go({0});", retorno), true);
        }
        /// <summary>
        /// Indica se o relatório foi chamado pela tela de emissores
        /// </summary>
        public string OrigemEmissores
        {
            get
            {
                if (ViewState["OrigemEmissores"] == null)
                    ViewState["OrigemEmissores"] = String.Empty;
                return ViewState["OrigemEmissores"].ToString();
            }
            set { ViewState["OrigemEmissores"] = value; }
        }
        #region [ Aviso de Relatório sem dados ]
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl dvRelatorio;
        protected global::System.Web.UI.WebControls.Panel pnlQuadroAviso;
        protected global::Redecard.PN.Extrato.Core.Web.Controles.Portal.QuadroAviso qdAvisoSemRegistros;

        /// <summary>Verifica os controles que devem estar visíveis</summary>
        /// <param name="qtdRegistros">Quantidade de registros da página</param>
        protected void VerificaControlesVisiveis(int qtdRegistros, string titulo, string mensagem)
        {
            if (qtdRegistros > 0)
            {
                if (pnlQuadroAviso != null)
                    pnlQuadroAviso.Visible = false;
                if (dvRelatorio != null)
                    dvRelatorio.Visible = true;
            }
            else
            {
                //Define o título da mensagem
                if (titulo == null)
                {
                    titulo = "Aviso";
                    HtmlGenericControl spnTituloRelatorio = this.FindControl("spnTituloRelatorio") as HtmlGenericControl;
                    if (spnTituloRelatorio != null)
                        titulo += " - " + spnTituloRelatorio.InnerText;
                }

                if (pnlQuadroAviso != null)
                    pnlQuadroAviso.Visible = true;
                if (dvRelatorio != null)
                    dvRelatorio.Visible = false;

                if (qdAvisoSemRegistros != null)
                {
                    qdAvisoSemRegistros.TipoQuadro = Core.Web.Controles.Portal.TipoQuadroAviso.Aviso;
                    qdAvisoSemRegistros.Mensagem = mensagem ?? "não há movimento para o período informado!";
                }
            }
        }
        #endregion

        #region [ Renderização de Controles ]

        /// <summary>Renderização de controles para obtenção do HTML renderizado</summary>
        /// <param name="controle">Controle a ser renderizado</param>
        /// <param name="controles">Lista de controles a serem renderizados</param>
        /// <param name="desativarControles">Se TRUE, renderiza todos os controles como desativados/desabilitados</param>        
        /// <returns>HTML representando o controle renderizado</returns>
        protected String RenderizarControles(Boolean desativarControles, Control controle, params Control[] controles)
        {
            StringBuilder sb = new StringBuilder();

            List<Control> listaControles = new List<Control>();
            if (controle != null)
                listaControles.Add(controle);
            if (controles != null && controles.Length > 0)
                listaControles.AddRange(controles);

            using (StringWriter writer = new StringWriter(sb))
            using (HtmlTextWriter hwriter = new HtmlTextWriter(writer))
            {
                Control controleAtual;
                for (Int32 iControle = 0, total = listaControles.Count; iControle < total; iControle++)
                {
                    controleAtual = listaControles[iControle];
                    if (desativarControles)
                        DesativarControles(controleAtual);
                    controleAtual.RenderControl(hwriter);
                }
                return HttpUtility.HtmlDecode(sb.ToString());
            }
        }

        /// <summary>Desativa o controle e todos os seus controles filhos recursivamente</summary>
        /// <param name="controle">Controle a ser desativado</param>
        private void DesativarControles(Control controle)
        {
            //Desativa o controle atual, de acordo com seu tipo
            if (controle is HtmlAnchor)
            {
                (controle as HtmlAnchor).Disabled = true;
                (controle as HtmlAnchor).HRef = "";
            }
            else if (controle is HtmlLink)
                (controle as HtmlLink).Disabled = true;
            else if (controle is HyperLink)
            {
                (controle as HyperLink).Enabled = false;
                (controle as HyperLink).Attributes.Remove("href");
            }
            else if (controle is LinkButton)
                (controle as LinkButton).Enabled = false;
            else if (controle is HtmlButton)
                (controle as HtmlButton).Disabled = true;
            else if (controle is HtmlInputControl)
                (controle as HtmlInputControl).Disabled = true;
            else if (controle is HtmlSelect)
                (controle as HtmlSelect).Disabled = true;
            else if (controle is TextBox)
                (controle as TextBox).Enabled = false;
            else if (controle is CheckBox)
                (controle as CheckBox).Enabled = false;

            //Aplica desativação de controles recursivamente para os controles filhos
            Int32 totalFilhos = controle.Controls.Count;
            for (Int32 iControleFilho = 0; iControleFilho < totalFilhos; iControleFilho++)
                DesativarControles(controle.Controls[iControleFilho]);
        }

        #endregion

        /// <summary>
        /// Dicionário com os motivos de crédito customizados
        /// </summary>
        private Dictionary<string, SPListaMotivosCreditoDebito> dicMotivosCreditoDebito = null;
        protected Dictionary<string, SPListaMotivosCreditoDebito> DicMotivosCreditoDebito
        {
            get
            {
                return dicMotivosCreditoDebito ?? (dicMotivosCreditoDebito = GetListaMotivosCreditoDebito());
            }
        }

        /// <summary>
        /// Constroi um dicionário com os motivos de crédito/débito customizados, persistidos na lista do Sharepoint
        /// </summary>
        /// <returns>Lista com os motivos de crédito/débito customizados</returns>
        protected Dictionary<string, SPListaMotivosCreditoDebito> GetListaMotivosCreditoDebito()
        {
            string nomeLista = "Motivos de Crédito e Débito Customizados";

            //Variável de retorno             
            Dictionary<string, SPListaMotivosCreditoDebito> objList = null;

            try
            {
                objList = new Dictionary<string, SPListaMotivosCreditoDebito>();

                SPList objListMotivos = SPContext.Current.Web.Lists[nomeLista];
                if (objListMotivos != null)
                {
                    // Converte SPListItemCollection para uma List<SPListaMotivosCreditoDebito>
                    foreach (SPItem item in objListMotivos.Items)
                    {
                        SPListaMotivosCreditoDebito objItem = new SPListaMotivosCreditoDebito
                        {
                            CodigoID = Convert.ToString(item["CodigoID"]),
                            CodigoOriginal = Convert.ToString(item["CodigoOriginal"]),
                            DescritivoCustomizado = Convert.ToString(item["DescritivoCustomizado"]),
                            TituloCustomizado = Convert.ToString(item["TituloCustomizado"])
                        };

                        if (!objList.ContainsKey(objItem.CodigoOriginal))
                            objList.Add(objItem.CodigoOriginal, objItem);
                    }
                }
            }
            catch (Exception exc)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.GetListaSP[" + nomeLista + "]");
                SharePointUlsLog.LogErro(exc);
            }

            return objList ?? new Dictionary<string, SPListaMotivosCreditoDebito>();
        }

        /// <summary>
        /// Consulta no dicionário de motivos de crédito/débito se há algum com o código informado
        /// </summary>
        /// <param name="codigoOriginal"></param>
        /// <returns>Retorna o títuo customizado, se existir</returns>
        protected string GetTituloMotivoCreditoDebitoCustomizado(string codigoMotivo, string tituloDefault = "")
        {
            SPListaMotivosCreditoDebito objMotivoCreditoDebito = null;
            if (this.DicMotivosCreditoDebito.ContainsKey(codigoMotivo))
            {
                objMotivoCreditoDebito = this.DicMotivosCreditoDebito[codigoMotivo];
                if (objMotivoCreditoDebito != null && !string.IsNullOrEmpty(objMotivoCreditoDebito.TituloCustomizado))
                    return objMotivoCreditoDebito.TituloCustomizado;
            }

            if (string.IsNullOrEmpty(tituloDefault))
                tituloDefault = codigoMotivo;

            return tituloDefault;
        }

        /// <summary>
        /// Consulta no dicionário de motivos de crédito/débito se há algum com o código informado
        /// </summary>
        /// <param name="codigoOriginal"></param>
        /// <returns>Retorna o títuo customizado, se existir</returns>
        protected string GetDescritivoMotivoCreditoDebitoCustomizado(string codigoMotivo, string descritivoDefault = "")
        {
            SPListaMotivosCreditoDebito objMotivoCreditoDebito = null;
            if (this.DicMotivosCreditoDebito.ContainsKey(codigoMotivo))
            {
                objMotivoCreditoDebito = this.DicMotivosCreditoDebito[codigoMotivo];
                if (objMotivoCreditoDebito != null && !string.IsNullOrEmpty(objMotivoCreditoDebito.DescritivoCustomizado))
                    return objMotivoCreditoDebito.DescritivoCustomizado;
            }

            if (string.IsNullOrEmpty(descritivoDefault))
                descritivoDefault = codigoMotivo;

            return descritivoDefault;
        }
    }
}
