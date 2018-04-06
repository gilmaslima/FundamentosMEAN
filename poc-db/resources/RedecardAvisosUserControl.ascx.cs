using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Fechado.Model.Repository;
using Redecard.Portal.Fechado.Model;
using Redecard.Portal.Helper;
using Microsoft.SharePoint;
using Microsoft.Office.Server;
using Microsoft.Office.Server.WebControls;
using Microsoft.Office.Server.Audience;

namespace Redecard.Portal.Fechado.WebParts.RedecardAvisos
{
    public partial class RedecardAvisosUserControl : UserControl
    {
        #region Propriedades__________________

        private RedecardAvisos WebPart
        {
            get
            {
                return (RedecardAvisos)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________
        
        /// <summary>
        /// Solicita o carregamento de Avisos
        /// </summary>
        private void CarregarAvisos()
        {
            IList<DTOAviso> avisos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOAviso, AvisosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        //Faz a busca no repositório, valida audiência e ordena aleatóriamente
                        avisos = AvisosAudienciaUsuario(repository.GetAllItems()).OrderBy(p => new Random().Next()).ToList();
                    });

                    if (avisos.Count > 0)
                    {
                        rptAvisos.DataSource = avisos;
                        rptAvisos.DataBind();
                    }
                    else
                    {
                        msgErro.Text = RedecardHelper.ObterResourceFechado("_RedecardAvisos_NenhumItem");
                        msgErro.Visible = true;
                    }
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }   
        }

        /// <summary>
        /// Filtra os avisos que o usuário pode visualizar (Audiências)
        /// </summary>
        /// <param name="listAvisos"></param>
        /// <returns></returns>
        private IList<DTOAviso> AvisosAudienciaUsuario(IList<DTOAviso> listAvisos)
        {
            IList<DTOAviso> listReturn = new List<DTOAviso>();

            try
            {
                AudienceLoader audienceLoader = AudienceLoader.GetAudienceLoader();

                foreach (DTOAviso listItem in listAvisos)
                {
                    if (AudienceManager.IsCurrentUserInAudienceOf(audienceLoader, listItem.Audiencia, false))
                    {
                        listReturn.Add(listItem);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return listReturn;
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarAvisos();
        }


        #endregion
    }
}
