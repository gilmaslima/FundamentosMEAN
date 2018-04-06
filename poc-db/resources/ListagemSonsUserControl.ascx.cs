using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper.Web;
using System.Collections.Generic;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model;

namespace Redecard.Portal.Aberto.WebParts.ListagemSons
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 05/10/2010
    /// Descrição: Composição do WebPart de Listagem de Sons
    /// </summary>
    public partial class ListagemSonsUserControl : UserControlBase
    {
        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.CarregarTiposDeSons();

            this.CarregarSons(this.TipoSom);

            base.OnLoad(e);
        }
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
        private ListagemSons WebPart
        {
            get
            {
                return this.Parent as ListagemSons;
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
        /// Acesso ao Tipo de Som selecionado
        /// </summary>
        private string TipoSom
        {
            get
            {
                if (Request.Params[ChavesQueryString.Galeria] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Galeria].ToString());
            }
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Monta uma string JavaScript de redirecionamento com base nos parâmetros informados
        /// </summary>
        /// <param name="urlAtual"></param>
        /// <param name="nomeParametro"></param>
        /// <param name="NomeAncora"></param>
        /// <returns></returns>
        protected string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora)
        {
            return string.Format("window.location.href='{0}{1}{2}=' + window.encodeURIComponent(this.options[this.selectedIndex].value) + '#" + nomeAncora + "';",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro);
        }

        /// <summary>
        /// Carrega a DropDownList de Tipos de Sons
        /// Os Tipos de Sons são obtidos a partir da listagem de sons existentes no repositório, e as duplicatas são eliminadas.
        /// Por último a coleção é ordenada alfabeticamente
        /// </summary>
        private void CarregarTiposDeSons()
        {
            string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Galeria.ToUpper()) || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            this.slcTypeCardBenefits.Items.Clear();

            this.slcTypeCardBenefits.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Galeria, this.AncoraListagem));

            IList<string> tiposSom = this.ObterTiposSom();

            if (tiposSom != null)
                tiposSom.ToList().ForEach(g => this.slcTypeCardBenefits.Items.Add(new ListItem(g, g)));

            this.slcTypeCardBenefits.Items.Insert(0, new ListItem(RedecardHelper.ObterResource("listagemSons_SelecioneUmaOpcao"), string.Empty));

            //Seleciona a galeria atual, se informada
            ListItem liTipoSom = this.slcTypeCardBenefits.Items.FindByValue(this.TipoSom);
            if (liTipoSom != null)
                liTipoSom.Selected = true;
            else
                this.slcTypeCardBenefits.SelectedValue = string.Empty;
        }

        /// <summary>
        /// Solicita os tipos de sons com o Repositório de Sons
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterTiposSom()
        {
            IList<string> tipos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOSom, SonsItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        tipos = (from gal in repository.GetAllItems()
                                    select gal.TipoDoSom).OrderBy(g => g).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return tipos;
        }

        /// <summary>
        /// Carrega a lista de sons com base no tipo de som apontado no controle de lista
        /// </summary>
        /// <param name="galeria"></param>
        private void CarregarSons(string tipoSom)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOSom, SonsItem>>())
                {
                    ListaPaginada<DTOSom> sons;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {                        
                        sons = new ListaPaginada<DTOSom>(repository.GetItems(som => som.TipoDoSom.Equals(tipoSom)).OrderByDescending(som => som.ID), this.Paginador.Pagina, this.ItensPorPagina);                      

                        this.rptListagemSons.DataSource = sons;
                        this.rptListagemSons.DataBind();

                        this.Paginador.MontarPaginador(sons.TotalItens, this.ItensPorPagina, this.AncoraListagem);
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