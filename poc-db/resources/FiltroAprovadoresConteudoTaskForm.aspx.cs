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
using System.Security.Permissions;
using System.Xml;

namespace Rede.PN.FiltroAprovadoresConteudo.Layouts.CustomTaskForm
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class FiltroAprovadoresConteudoTaskForm : LayoutsPageBase
    {
        #region [Atributos e Prorpriedades]
        protected new SPWeb Web { get; set; }

        private SPList list;

        protected SPList List
        {
            get
            {
                if (this.list == null)
                {
                    throw new SPException("Erro ao obter lista do Context");
                }
                return this.list;
            }
            set
            {
                this.list = value;
            }
        }

        protected SPListItem Tarefa { get; set; }        
        #endregion

        #region [Eventos]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Boolean.Parse(this.Tarefa["Completed"].ToString()) == true)
            {
                this.spnBotoes.Visible = false;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Web = SPContext.Current.Web;
            this.List = SPContext.Current.List;
            this.Tarefa = (SPListItem)SPContext.Current.Item;
        }

        protected void btnAprovar_Click(object sender, EventArgs e)
        {
            AlterarTarefa("Aprovado");
        }

        protected void btnRejeitar_Click(object sender, EventArgs e)
        {
            AlterarTarefa("Rejeitado");
        }
        #endregion

        #region [Métodos]
        private void AlterarTarefa(String resultado)
        {
            String solicitacao = (String)this.Tarefa[SPBuiltInFieldId.Body];
            Boolean validarAprovador = true;
            Guid workflowInstanceId = new Guid((string)this.Tarefa[SPBuiltInFieldId.WorkflowInstanceID]);
            SPWorkflow spWorkflow = new SPWorkflow(this.Web, workflowInstanceId);
            SPWorkflowAssociation parentAssociation = spWorkflow.ParentAssociation;

            XmlDocument docAssociationData = new XmlDocument();
            docAssociationData.LoadXml(parentAssociation.AssociationData);

            var xmlnsManager = new XmlNamespaceManager(docAssociationData.NameTable);
            xmlnsManager.AddNamespace("dfs", "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");
            xmlnsManager.AddNamespace("d", "http://schemas.microsoft.com/office/infopath/2009/WSSList/dataFields");

            XmlNode validarAprovadorNode = docAssociationData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:ValidarAprovador", xmlnsManager);
            if (validarAprovadorNode != null && validarAprovadorNode.HasChildNodes)
            {
                validarAprovador = Convert.ToBoolean(validarAprovadorNode.FirstChild.Value);
            }

            if (validarAprovador)
            {
                String urlArquivo = this.Tarefa[SPBuiltInFieldId.WorkflowLink].ToString().Split(',')[0];
                SPFile arquivo = this.Web.GetFile(urlArquivo);

                if (arquivo.ModifiedBy.ID == this.Web.CurrentUser.ID)
                {
                    SPUtility.TransferToErrorPage("Erro ao alterar tarefa: O usuário aprovador não pode ser o mesmo usuário editor.");
                    return;
                }
            }

            var htDados = new Hashtable();
            htDados["WorkflowOutcome"] = resultado;
            htDados["PercentComplete"] = 1.0f;
            htDados["Status"] = "Concluída";
            htDados["Completed"] = true;
            htDados["TaskStatus"] = resultado;
            htDados["ows_FieldName_Comments "] = this.txaComentarios.InnerText;

            String redirectUrl = null;

            if (!SPUtility.DetermineRedirectUrl(this.List.DefaultViewUrl, SPRedirectFlags.UseSource | SPRedirectFlags.DoNotEncodeUrl, this.Context, null, out redirectUrl))
            {
                redirectUrl = this.List.DefaultViewUrl;
            }

            using (var sPLongOperation = new SPLongOperation(this))
            {
                sPLongOperation.LeadingHTML = "Aguarde enquanto a tarefa é concluída...";
                //sPLongOperation.TrailingHTML = string.Format(CultureInfo.CurrentCulture, "Isso pode levar alguns minutos.", new object[]
                //{
                //    redirectUrl
                //});

                sPLongOperation.Begin();

                if (!SPWorkflowTask.AlterTask(this.Tarefa, htDados, true))
                {
                    SPUtility.TransferToErrorPage("Erro ao alterar tarefa. Favor verificar o histórico do fluxo de trabalho.");
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
        #endregion       
    }
}
