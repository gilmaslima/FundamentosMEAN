using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.DicasMaisRecentes
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 10/10/2010
    /// Descrição: Composição do WebPart de Filtragem de Dicas
    /// </summary>
    public partial class DicasMaisRecentesUserControl : UserControlBase
    {
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


        private Redecard.Portal.Aberto.Model.TipoDaDica TipoDaDica
        {
            get
            {
                return this.WebPart.TipoDaDica;
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private DicasMaisRecentes WebPart
        {
            get
            {
                return this.Parent as DicasMaisRecentes;
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
            this.CarregarCategoriaDicas();

            base.OnLoad(e);
        }

        #endregion        

        #region Métodos

        /// <summary>
        /// Carrega a lista de dicas por Tipo de Dica
        /// </summary>        
        private void CarregarCategoriaDicas()
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODica, DicasItem>>())
                {
                    ListaPaginada<DTODica> dicas;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        dicas = new ListaPaginada<DTODica>(repository.GetItems(dica => dica.TipoDaDica.Equals(TipoDaDica)).OrderByDescending(dica => dica.NumeroDeExibicoes), this.Paginador.Pagina, this.ItensPorPagina);
                        this.rptListagemDicas.DataSource = dicas;
                        this.rptListagemDicas.DataBind();

                        this.Paginador.MontarPaginador(this.ItensPorPagina, this.ItensPorPagina, this.AncoraListagem);                        
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
