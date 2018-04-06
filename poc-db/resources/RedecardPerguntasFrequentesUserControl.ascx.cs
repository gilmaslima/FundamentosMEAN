using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Fechado.Model.Repository;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
using Redecard.Portal.Fechado.Model;
using Redecard.Portal.Helper;
using System.Linq;

namespace Redecard.Portal.Fechado.WebParts.RedecardPerguntasFrequentes
{
    public partial class RedecardPerguntasFrequentesUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardPerguntasFrequentes WebPart
        {
            get
            {
                return (RedecardPerguntasFrequentes)this.Parent;
            }
        }

        //private IList<DTOPerguntaFrequente> PerguntasFrequentes;

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void PopularPerguntasFrequentes()
        {
            /*IList<DTOPerguntaFrequente> itens = null;
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
            {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate
                {
                    //this.ddlArea.DataSource = repository.GetItems(i=> i).OrderBy(i => i.Ordem).Distinct().ToList();
                    //this.ddlArea.DataSource = repository.GetAllItems().OrderBy(i => i).Distinct().ToList();
                    itens = repository.GetAllItems();
                });
                this.rptAssunto.DataSource = itens;
                this.rptAssunto.DataBind();
            }*/
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void rptAssunto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            /*DTOPerguntaFrequente itens = e.Item.DataItem as DTOPerguntaFrequente;
            if (itens != null)  
                {
                    Repeater rptPerguntaResposta = e.Item.FindControl("rptPerguntaResposta") as Repeater;
                    if (rptPerguntaResposta != null)
                    {
                        //rptPerguntaResposta.DataSource = ;
                        //rptPerguntaResposta.DataBind();  
                    }
            }*/
        }

        #endregion

    }
}
