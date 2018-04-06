/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;
using System;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Xml;

namespace Rede.PN.FiltroAprovadoresConteudo.Layouts.CustomTaskForm
{
    /// <summary>
    /// Classe FiltroAprovadoresInitForm
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class FiltroAprovadoresInitForm : LayoutsPageBase
    {
        #region [Atributos e Propriedades]
        private SPListItem currentListItem;
        private SPList list;
        private SPWorkflowAssociation assocTemplate;
        private SPWorkflowTemplate baseTemplate;
        private String formData;
        private String workflowName;
        private string listItemUrl;
        #endregion

        #region [Eventos]
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.CarrgarDadosDeAssociacao();

        }

        protected void btnIniciar_Click(object sender, EventArgs e)
        {
            String redirectUrl = null;

            if (!SPUtility.DetermineRedirectUrl(this.list.DefaultViewUrl, SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEncodeUrl, this.Context, null, out redirectUrl))
            {
                redirectUrl = this.list.DefaultViewUrl;
            }

            using (var sPLongOperation = new SPLongOperation(this))
            {
                sPLongOperation.LeadingHTML = "Aguarde enquanto o fluxo é iniciado...";
                sPLongOperation.Begin();

                try
                {
                    this.IniciarAssociacaoFluxoDeTrabalho();
                }
                catch (UnauthorizedAccessException uEx)
                {
                    SPUtility.HandleAccessDenied(uEx);
                }
                catch (Exception ex)
                {
                    SPException spEx = ex as SPException;
                    string message;

                    if (spEx != null && spEx.ErrorCode == -2130575205)
                    {
                        message = SPResource.GetString("WorkflowFailedAlreadyRunningMessage", new object[0]);
                    }
                    else
                    {
                        if (spEx != null && spEx.ErrorCode == -2130575339)
                        {
                            message = SPResource.GetString("ListVersionMismatch", new object[0]);
                        }
                        else
                        {
                            if (spEx != null && (spEx.ErrorCode == -2130575338 || spEx.ErrorCode == -2130575306 || spEx.ErrorCode == -2130575282 || spEx.ErrorCode == -2130575258 || spEx.ErrorCode == -2130575223))
                            {
                                message = spEx.Message;
                            }
                            else
                            {
                                message = SPResource.GetString("WorkflowFailedStartMessage", new object[0]);
                            }
                        }
                    }

                    SPUtility.TransferToErrorPage(message);
                }

                if (base.IsDialogMode)
                {
                    base.SendResponseForPopUI();
                }
                else
                {
                    sPLongOperation.End(redirectUrl, SPRedirectFlags.Static, this.Context, null);
                }
            }

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            base.SendResponseForPopUI();
        }
        #endregion

        #region [Métodos]
        protected void CarrgarDadosDeAssociacao()
        {
            this.list = SPContext.Current.List;
            Guid associationId = new Guid(base.Request.Params["TemplateID"]);

            if (this.list != null)
            {
                this.currentListItem = this.list.GetItemById(Convert.ToInt32(base.Request.Params["ID"], CultureInfo.InvariantCulture));
                if (this.currentListItem != null)
                {
                    this.assocTemplate = SPWorkflowAssociationCollection.GetAssociationForListItemById(this.currentListItem, associationId);
                    base.PageTarget = this.currentListItem;
                }
            }
            else
            {
                this.assocTemplate = this.ObterAssociacaoFluxoDeTrabalho(associationId);
            }

            if (this.assocTemplate == null)
            {
                SPUtility.TransferToErrorPage("Erro ao iniciar Workflow, associação de fluxo não encontrada para a lista.");
            }

            this.baseTemplate = this.assocTemplate.BaseTemplate;
            this.workflowName = this.assocTemplate.Name;
            this.formData = this.assocTemplate.AssociationData;
        }

        private SPWorkflowAssociation ObterAssociacaoFluxoDeTrabalho(Guid idAssociacao)
        {
            SPWeb web = base.Web;

            if (web == null)
            {
                return null;
            }

            foreach (SPWorkflowAssociationCollection sPWorkflowAssociationCollection in new ArrayList { web.WorkflowAssociations })
            {
                SPWorkflowAssociation spWorkflowAssociation = null;
                if (idAssociacao != Guid.Empty)
                {
                    spWorkflowAssociation = sPWorkflowAssociationCollection[idAssociacao];
                }

                if (spWorkflowAssociation != null)
                {
                    return spWorkflowAssociation;
                }
            }
            return null;
        }

        protected void IniciarAssociacaoFluxoDeTrabalho()
        {
            XmlDocument docFormData = new XmlDocument();
            docFormData.LoadXml(this.formData);

            var xmlnsManager = new XmlNamespaceManager(docFormData.NameTable);
            xmlnsManager.AddNamespace("dfs", "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");
            xmlnsManager.AddNamespace("d", "http://schemas.microsoft.com/office/infopath/2009/WSSList/dataFields");

            XmlNode notificationMessageNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:NotificationMessage", xmlnsManager);
            notificationMessageNode.RemoveAll();
            notificationMessageNode.AppendChild(docFormData.CreateTextNode(this.txaSolicitacao.InnerText));

            this.formData = docFormData.OuterXml;

            if (this.currentListItem != null)
            {
                base.Web.Site.WorkflowManager.StartWorkflow(this.currentListItem, this.assocTemplate, this.formData);
                return;
            }

            base.Web.Site.WorkflowManager.StartWorkflow(null, this.assocTemplate, this.formData, SPWorkflowRunOptions.Synchronous);
        }
        #endregion             

    }
}
