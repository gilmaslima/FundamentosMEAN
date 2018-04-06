using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Aberto.WebParts.ListagemDownloads
{
    public partial class ListagemDownloadsUserControl : UserControlBase
    {
        #region Variáveis

        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listagemDownloads_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;
        
        private static string textoTodosSelecione = RedecardHelper.ObterResource("listagemDownloads_Todos");
        private static string valorTodosSelecione = "ALL";

        #endregion

        #region Propriedades
        /// <summary>
        /// Nome da âncora utilizada para posicionamento do usuário na página
        /// </summary>
        public string AncoraListagem
        {
            get
            {
                return "wptListagem";
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemDownloads WebPart {
            get {
                return this.Parent as ListagemDownloads;
            }
        }

        /// <summary>
        /// Mostra itens da página atual (de-até) e total de itens
        /// </summary>
        public string TotalDeDownloadsExibidos  
        {
            get;
            set;
        }

        /// <summary>
        /// Referência tipada ao user control de paginação
        /// </summary>
        private UserControlPaginador Paginador
        {
            get
            {
                return this.ucPaginadorHibrido as UserControlPaginador;
            }
        }

        /// <summary>
        /// Acesso a Area selecionada
        /// </summary>
        private string Area
        {
            get
            {
                //if (Request.Params[ChavesQueryString.Area] == null)
                //    return string.Empty;

                return this.slcArea.SelectedValue;
            }
        }

        /// <summary>
        /// Acesso ao Assunto selecionado
        /// </summary>
        private string Assunto
        {
            get
            {
                //if (Request.Params[ChavesQueryString.Assunto] == null)
                //    return string.Empty;

                return this.slcSubject.SelectedValue;
            }
        }

        /// <summary>
        /// Acesso a quantidade de itens por página selecioanda
        /// </summary>
        private int QtdeItensPagina
        {
            get
            {
                //if (Request.Params[ChavesQueryString.QtdeItens] == null)
                //    return string.Empty;

                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Acesso a ordenação selecionada
        /// </summary>
        private string Ordenacao
        {
            get
            {
                if (Request.Params[ChavesQueryString.Ordenar] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Ordenar].ToString());
            }
        }

        public bool AlgumFiltroSelecionado
        {
            get
            {
                return !this.slcArea.SelectedValue.Equals(valorPadraoSelecione) || !this.slcSubject.SelectedValue.Equals(valorPadraoSelecione);
            }
        }


        /// <summary>
        /// Acesso a Area selecionada
        /// </summary>
        private TipoDeDownload Tipo
        {
            get
            {
                if (Request.Params[ChavesQueryString.TipoDeDownload] == null)
                    return TipoDeDownload.None;

                return (TipoDeDownload)Enum.Parse(typeof(TipoDeDownload), Request.Params[ChavesQueryString.TipoDeDownload].ToString());
            }
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.CarregarCombos();

            this.CarregarDownloads(this.Area, this.Assunto);

            base.OnLoad(e);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            LinkButton lnkDownload = sender as LinkButton;

            string fileDown = lnkDownload.CommandName;

            //this.GravaDownload(lnkDownload.CommandArgument.ToInt());
           
            Response.Redirect(fileDown);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Atualiza no repositório a Lista de Downloads com a quantidade de downloads realizados
        /// </summary>
        /// <param name="lID">ID do Download</param>
        private void GravaDownload(int lID)
        {
            try
            {               
                DTODownload updDownload;

                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODownload, DownloadsItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        updDownload = repository.GetItems(download => download.Id.Equals(lID)).FirstOrDefault();

                        if (updDownload != null)
                        {
                            updDownload.NumeroDeDownloads++;

                            repository.Persist(updDownload);
                        }
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        private void CarregarCombos()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Area, Assunto e Página
//            string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Area.ToUpper())
//                                                    || gal.Trim().ToUpper().Equals(ChavesQueryString.Assunto.ToUpper())
//                                                    || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper())
//                                                    || gal.Trim().ToUpper().Equals(ChavesQueryString.QtdeItens.ToUpper())
//                                                    || gal.Trim().ToUpper().Equals(ChavesQueryString.Ordenar.ToUpper()));

//            //Monta Url usando UrlAtual + parametros 
//            string montaUrl = string.Format(@"window.location.href='{0}{1}{2}=' + window.encodeURIComponent({3}.options[{3}.selectedIndex].value) +
//                                            '{4}{5}=' + window.encodeURIComponent({6}.options[{6}.selectedIndex].value) +
//                                            '{7}{8}=' + window.encodeURIComponent({9}.options[{9}.selectedIndex].value) +
//                                            '{10}{11}=' + window.encodeURIComponent({12}.options[{12}.selectedIndex].value) +
//                                            '#{13}'; return false;",
//                                            urlAtual,
//                                            urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
//                                            ChavesQueryString.Area,
//                                            this.slcArea.UniqueID,
//                                            "&", //urlAtual.IndexOf('&') == -1 ? "&" : string.Empty,
//                                            ChavesQueryString.Assunto,
//                                            this.slcSubject.UniqueID,
//                                            "&", //urlAtual.IndexOf('&') == -1 ? "&" : string.Empty,
//                                            ChavesQueryString.QtdeItens,
//                                            this.slcVisualize.UniqueID,
//                                            "&", //urlAtual.IndexOf('&') == -1 ? "&" : string.Empty,
//                                            ChavesQueryString.Ordenar,
//                                            this.slcOrder.UniqueID,
//                                            this.AncoraListagem);

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            //this.btnOK.Attributes.Add("onclick", montaUrl);

            if (!Page.IsPostBack) {
                this.CarregarAreas();
                this.CarregarAssuntos();
            }

            //Seleciona a quantidade de itens por página atual, se informada
            //ListItem liQtdPaginas = this.slcVisualize.Items.FindByValue(this.QtdeItensPagina);

            //if (liQtdPaginas != null)
            //    liQtdPaginas.Selected = true;            
            //else
            //    this.slcVisualize.SelectedIndex = 0;

            ////Seleciona a ordenação, se informada
            //ListItem liOrdenacao = this.slcOrder.Items.FindByValue(this.Ordenacao);

            //if (liOrdenacao != null)
            //    liOrdenacao.Selected = true;
            //else
            //    this.slcOrder.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarAreas()
        {
            //Esvazia o controle
            this.slcArea.Items.Clear();

            //Obtém as áreas de download para carregamento
            IList<string> areas = this.ObterAreas();

            //Carrega o controle com as áreas
            if (areas != null)
                areas.ToList().ForEach(g => this.slcArea.Items.Add(new ListItem(g, g)));

            //Adiciona um primeiro item no controle
            this.slcArea.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));
            this.slcArea.Items.Insert(1, new ListItem(textoTodosSelecione, valorTodosSelecione));

            //Seleciona a área atual, se informada
            ListItem liArea = this.slcArea.Items.FindByValue(this.Area);

            if (liArea != null)
            {
                liArea.Selected = true;
            }
            else
                this.slcArea.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarAssuntos()
        {
            //Esvazia o controle
            this.slcSubject.Items.Clear();

            //Obtém os assuntos de download para carregamento
            IList<string> assuntos = this.ObterAssuntos();

            //Carrega o controle com os assuntos
            if (assuntos != null)
                assuntos.ToList().ForEach(g => this.slcSubject.Items.Add(new ListItem(g, g)));

            //Adiciona um primeiro item no controle
            this.slcSubject.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));
            this.slcSubject.Items.Insert(1, new ListItem(textoTodosSelecione, valorTodosSelecione));

            //Seleciona o assunto atual, se informada
            ListItem liAssunto = this.slcSubject.Items.FindByValue(this.Assunto);

            if (liAssunto != null)
            {
                liAssunto.Selected = true;
            }
            else
                this.slcSubject.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita as áreas ao Repositório de Downloads
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterAreas()
        {
            IList<string> areas = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODownload, DownloadsItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        areas = (from down in repository.GetAllItems()
                                    where down.TipoDeDownload == this.Tipo || this.Tipo == TipoDeDownload.None
                                    select down.Area.ToString()).OrderBy(r => r).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return areas;
        }

        /// <summary>
        /// Solicita os assuntos ao Repositório de Downloads
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterAssuntos()
        {
            IList<string> assuntos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODownload, DownloadsItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        assuntos = (from down in repository.GetAllItems()
                                    where down.TipoDeDownload == this.Tipo || this.Tipo == TipoDeDownload.None
                                    select down.Assunto.ToString()).OrderBy(r => r).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return assuntos;
        }   

        /// <summary>
        /// Carrega a lista de downloads com base no mes apontado no controle de lista
        /// </summary>
        /// <param name="data"></param>
        private void CarregarDownloads(string area, string assunto)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODownload, DownloadsItem>>())
                {
                    ListaPaginada<DTODownload> downloads;
                    int paginaAtual = 0;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        downloads = this.GetDownloadsDataSource(area, assunto, repository);
                        
                        //Pagina a lista de acordo com o item selecionado no combo de ordenação
                        //switch (this.Ordenacao)
                        //{
                        //    case "area":
                        //        downloads.OrderBy(o => o.Area);
                        //        break;
                        //    case "assunto":
                        //        downloads.OrderBy(o => o.Assunto);
                        //        break;
                        //    case "descricao":
                        //        downloads.OrderBy(o => o.Descricao);
                        //        break;
                        //    case "tipo":
                        //        downloads.OrderBy(o => o.Titulo);
                        //        break;
                        //    default:
                        //        downloads.OrderBy(o => o.ID);
                        //        break;
                        //}

                        if (!object.ReferenceEquals(downloads, null)) { // Estado inicial do controle
                            downloads.OrderBy(o => o.Area).OrderBy(o => o.Assunto);
                            this.pag.TotalPaginas = downloads.TotalPaginas;

                            if (Page.IsPostBack) {
                                if (downloads.Count < 1)
                                    this.TotalDeDownloadsExibidos = this.WebPart.MensagemNoitems;
                                else {
                                    this.TotalDeDownloadsExibidos = this.WebPart.MensagemHasitems.Replace("[0]", downloads.TotalItens.ToString());
                                }
                            }

                            this.rptListagemDownloads.DataSource = downloads;
                            this.rptListagemDownloads.DataBind();

                            //this.Paginador.MontarPaginador(downloads.TotalItens, this.QtdeItensPagina.ToInt(), this.AncoraListagem);

                            //Mostra o texto de quantos registros foram encontrados
                            //if (this.AlgumFiltroSelecionado)
                            //{
                            //    if (downloads.TotalItens > 0)
                            //        this.lblResultados.Text = string.Format(RedecardHelper.ObterResource("listagemDownloads_resultadosencontrados"), downloads.TotalItens);
                            //    else
                            //        this.lblResultados.Text = RedecardHelper.ObterResource("listagemDownloads_PesquisaSemResultado");
                            //}

                            #region [ Mostra itens De/Até e Total de Itens na paginação ]

                            //if (this.Paginador.Pagina == null && (!string.IsNullOrEmpty(this.Assunto) || !string.IsNullOrEmpty(this.Assunto)))
                            //{
                            //    if (!string.IsNullOrEmpty(this.Assunto) || !string.IsNullOrEmpty(this.Assunto))
                            //        paginaAtual = 1;
                            //}
                            //else if ((!string.IsNullOrEmpty(this.Assunto) || !string.IsNullOrEmpty(this.Assunto)))
                            //    paginaAtual = this.Paginador.Pagina.ToInt();


                            //if (paginaAtual > 0)
                            //{
                            //    this.TotalDeDownloadsExibidos = string.Format(RedecardHelper.ObterResource("listagemDownloads_downloads"),
                            //                                    (paginaAtual == 1 ? 1 : (this.QtdeItensPagina.ToInt() * (paginaAtual - 1))),
                            //                                    (this.QtdeItensPagina.ToInt() * paginaAtual) > downloads.TotalItens ? downloads.TotalItens :
                            //                                    (this.QtdeItensPagina.ToInt() * paginaAtual),
                            //                                    downloads.TotalPaginas);
                            //}

                            #endregion
                        }
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="assunto"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        private ListaPaginada<DTODownload> GetDownloadsDataSource(string area, string assunto, IRepository<DTODownload, DownloadsItem> repository) {
            ListaPaginada<DTODownload> downloads = null;
            // Área está preenchida e Assunto não está preenchido
            if (!String.IsNullOrEmpty(area) && String.IsNullOrEmpty(assunto)) {
                downloads = new ListaPaginada<DTODownload>(repository.GetItems(download =>
                                                            (download.TipoDeDownload.Equals(this.Tipo) || download.TipoDeDownload.Equals(TipoDeDownload.None)) &&
                                                            (download.Área.Equals(area) || area.Equals(valorTodosSelecione))), this.pag.PaginaAtual, this.QtdeItensPagina);
            }
            // Área não está preenchida e Assunto está preenchido
            else if (String.IsNullOrEmpty(area) && !String.IsNullOrEmpty(assunto)) {
                downloads = new ListaPaginada<DTODownload>(repository.GetItems(
                                                                download => (download.TipoDeDownload.Equals(this.Tipo) || this.Tipo.Equals(TipoDeDownload.None)) &&
                                                                            (download.Assunto.Equals(assunto) || assunto.Equals(valorTodosSelecione)))
                                                                .OrderBy(o => o.ID),
                                                                this.pag.PaginaAtual,
                                                                this.QtdeItensPagina);
            }
            // Área e Assunto estão preenchidos
            else if (!String.IsNullOrEmpty(area) && !String.IsNullOrEmpty(assunto)) {
                downloads = new ListaPaginada<DTODownload>(repository.GetItems(
                                                                download => (download.TipoDeDownload.Equals(this.Tipo) || this.Tipo.Equals(TipoDeDownload.None)) &&
                                                                            (download.Área.Equals(area) || area.Equals(valorTodosSelecione)) &&
                                                                            (download.Assunto.Equals(assunto) || assunto.Equals(valorTodosSelecione)))
                                                                .OrderBy(o => o.ID),
                                                                this.pag.PaginaAtual,
                                                                this.QtdeItensPagina);
            }
            // Nenhum dos dois está preenchido
            else 
                downloads = new ListaPaginada<DTODownload>(repository.GetItems(download => download.Id < 1), this.pag.PaginaAtual, this.QtdeItensPagina);
            return downloads;
        }
        #endregion
    }
}
