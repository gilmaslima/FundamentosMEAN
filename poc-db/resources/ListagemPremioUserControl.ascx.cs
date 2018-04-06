using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;
using Redecard.Portal.Helper.Web.Controles;

namespace Redecard.Portal.Aberto.WebParts.ListagemPremio
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 12/10/2010
    /// Descrição: Composição do WebPart de Listagem de Prêmios
    /// </summary>
    public partial class ListagemPremioUserControl : UserControlBase
    {
        #region Variáveis

        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listemPremios_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;

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
        /// Obtém os itens no página estipulados no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private int ItensPorPagina
        {
            get
            {
                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemPremio WebPart
        {
            get
            {
                return this.Parent as ListagemPremio;
            }
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
        /// Acesso a categoria selecionada
        /// </summary>
        private string Categoria
        {
            get
            {
                if (Request.Params[ChavesQueryString.Categoria] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Categoria].ToString());
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
            this.CarregarCategorias();

            this.CarregarPremios(this.Categoria);

            base.OnLoad(e);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //int iCount = 0;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ItemDataBound(object sender, RepeaterItemEventArgs e) {
        //    if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item) {
        //        if (iCount == 2) {
        //            Literal liItem = e.Item.FindControl("liItem") as Literal;
        //            if (!object.ReferenceEquals(liItem, null))
        //                liItem.Text = "<li class=\"last\">";
        //            iCount = 0;
        //        }
        //        else
        //            iCount++;
        //    }
        //}
        #endregion

        #region Métodos

        /// <summary>
        /// Monta uma string JavaScript de redirecionamento com base nos parâmetros informados
        /// </summary>
        /// <param name="urlAtual"></param>
        /// <param name="nomeParametro">Nome do parâmetro que será utilizado para o valor do item selecionado no controle. Exemplo: PAGINA=1</param>
        /// <param name="nomeAncora"></param>
        /// <param name="objeto"></param>
        /// <returns></returns>
        protected virtual string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora)
        {
            return string.Format("window.location.href='{0}{1}{2}=' + window.encodeURIComponent(this.options[this.selectedIndex].value) + '#{3}'; return false;",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro,
                                nomeAncora);
        }

        /// <summary>
        /// Carrega a DropDownList de Categorias de Prêmios
        /// As categorias são obtidos a partir da listagem de prêmios existentes no repositório, e as duplicatas são eliminadas.
        /// Por último a coleção é ordenada alfabeticamente
        /// </summary>
        private void CarregarCategorias()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Categoria e Página
            string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Categoria.ToUpper()) || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcCategoriaPremio.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item            
            this.slcCategoriaPremio.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Categoria, this.AncoraListagem));

            //Obtém as categorias de prêmio para carregamento
            IList<string> categorias = this.ObterCategorias();

            //Carrega o controle com as categorias
            if (categorias != null)
                categorias.ToList().ForEach(g => this.slcCategoriaPremio.Items.Add(new ListItem(g, g)));

            //Adiciona um primeiro item no controle
            this.slcCategoriaPremio.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona a categoria atual, se informado
            ListItem liCategoria = this.slcCategoriaPremio.Items.FindByValue(this.Categoria);

            if (liCategoria != null)
                liCategoria.Selected = true;
            else
                this.slcCategoriaPremio.SelectedValue = valorPadraoSelecione;

            this.lblPremio.Text = RedecardHelper.ObterResource("listemPremios_vejaoutrospremios");
        }

        /// <summary>
        /// Solicita as categorias de prêmios com o Repositório de Prêmios
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterCategorias()
        {
            IList<string> premios = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPremio, PrêmiosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        premios = (from pre in repository.GetAllItems()
                                   select pre.Categoria).OrderBy(p => p).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return premios;
        }

        /// <summary>
        /// Carrega a lista de prêmios com base na categoria apontada no controle de lista
        /// </summary>
        /// <param name="data"></param>
        private void CarregarPremios(string categoria)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPremio, PrêmiosItem>>())
                {
                    ListaPaginada<DTOPremio> premios;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        premios = new ListaPaginada<DTOPremio>(repository.GetItems(premio => premio.Categoria.Equals(categoria)).OrderByDescending(premio => premio.ID), this.Paginador.Pagina, this.ItensPorPagina);
                        this.rptListagemPremios.DataSource = premios;
                        this.rptListagemPremios.DataBind();

                        this.Paginador.MontarPaginador(premios.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }
        #endregion
    }
}