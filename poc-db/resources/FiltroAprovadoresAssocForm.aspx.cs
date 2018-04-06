/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.Office.Workflow;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Security.Permissions;
using System.Xml;

namespace Rede.PN.FiltroAprovadoresConteudo.Layouts.CustomTaskForm
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class FiltroAprovadoresAssocForm : WrkAssocPage
    {

        #region [Atributos e Propriedades]
        private bool isWorkflowOnSharePoint;        
        #endregion

        #region [Eventos]
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);

            Console.WriteLine(Request.Params.Count);

            this.isWorkflowOnSharePoint = (base.Request.Params["WF4"] == null || base.Request.Params["WF4"] == "0");
            if (this.isWorkflowOnSharePoint)
            {
                this.CarregarDadosDeAssociacao(ea);
                return;
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            AssociarFluxoDeTrabalho();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            string url = "WrkSetng.aspx" + this.m_strQueryParams;
            SPUtility.Redirect(url, SPRedirectFlags.RelativeToLayoutsPage, this.Context);
        }        
        #endregion

        #region [Métodos]
        private void CarregarDadosDeAssociacao(EventArgs ea)
        {
            base.AssociationOnLoad(ea);
            //if (this.m_assocTemplate == null)
            //{
            //    this.m_workflowManager = new WorkflowServicesManager(base.Web);
            //    if (this.m_workflowManager.IsConnected)
            //    {
            //        this.CheckDupWf4Name();
            //    }
            //}            
            if (this.m_assocTemplate == null)
            {
                this.m_formData = this.m_baseTemplate.AssociationData;
            }
            else
            {
                this.m_formData = this.m_assocTemplate.AssociationData;
            }

            if (string.IsNullOrEmpty(this.m_formData))
            {
                this.m_formData = null;
            }

            base.PreservePostbackParameters();

        }

        private void AssociarFluxoDeTrabalho()
        {
            XmlDocument docFormData = new XmlDocument();
            docFormData.LoadXml(this.m_formData);

            var xmlnsManager = new XmlNamespaceManager(docFormData.NameTable);

            xmlnsManager.AddNamespace("dfs", "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");
            xmlnsManager.AddNamespace("d", "http://schemas.microsoft.com/office/infopath/2009/WSSList/dataFields");
            xmlnsManager.AddNamespace("pc", "http://schemas.microsoft.com/office/infopath/2007/PartnerControls");

            XmlNode assigneeNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:Approvers/d:Assignment/d:Assignee", xmlnsManager);
            XmlElement personElement = docFormData.CreateElement("pc", "Person", xmlnsManager.LookupNamespace("pc"));

            if (peoplePicker.ResolvedEntities.Count > 0)
            {
                assigneeNode.AppendChild(personElement);

                foreach (PickerEntity aprovador in peoplePicker.ResolvedEntities)
                {
                    XmlElement displayNameElement = docFormData.CreateElement("pc", "DisplayName", xmlnsManager.LookupNamespace("pc"));
                    displayNameElement.AppendChild(docFormData.CreateTextNode(aprovador.DisplayText));
                    personElement.AppendChild(displayNameElement);

                    XmlElement accountIdElement = docFormData.CreateElement("pc", "AccountId", xmlnsManager.LookupNamespace("pc"));
                    accountIdElement.AppendChild(docFormData.CreateTextNode(aprovador.Key));
                    personElement.AppendChild(accountIdElement);

                    XmlElement accountTypeElement = docFormData.CreateElement("pc", "AccountType", xmlnsManager.LookupNamespace("pc"));
                    accountTypeElement.AppendChild(docFormData.CreateTextNode(aprovador.EntityType));
                    personElement.AppendChild(accountTypeElement);
                }
            }

            XmlNode assignmentTypeNode =
                docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:Approvers/d:Assignment/d:AssignmentType", xmlnsManager);
            assignmentTypeNode.RemoveAll();
            assignmentTypeNode.AppendChild(docFormData.CreateTextNode(this.ddlTipoAtribuicaoTarefa.SelectedValue));

            XmlNode expandGroupsNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:ExpandGroups", xmlnsManager);
            expandGroupsNode.RemoveAll();
            expandGroupsNode.AppendChild(docFormData.CreateTextNode(this.chkExpandirGrupos.Checked.ToString().ToLower()));

            XmlNode terminarRejeicaoNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:CancelonRejection", xmlnsManager);
            terminarRejeicaoNode.RemoveAll();
            terminarRejeicaoNode.AppendChild(docFormData.CreateTextNode(this.chkTerminaRejeicao.Checked.ToString().ToLower()));

            XmlNode terminarAlteracaoNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:CancelonChange", xmlnsManager);
            terminarAlteracaoNode.RemoveAll();
            terminarAlteracaoNode.AppendChild(docFormData.CreateTextNode(this.chkTerminaAlteracao.Checked.ToString().ToLower()));

            XmlNode habilitarAprovacaoConteudoNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:EnableContentApproval", xmlnsManager);
            habilitarAprovacaoConteudoNode.RemoveAll();
            habilitarAprovacaoConteudoNode.AppendChild(docFormData.CreateTextNode(this.chkHabilitarAprovacaoConteudo.Checked.ToString().ToLower()));

            XmlNode validarAprovadorNode = docFormData.SelectSingleNode("descendant::dfs:dataFields/d:SharePointListItem_RW/d:ValidarAprovador", xmlnsManager);
            validarAprovadorNode.RemoveAll();
            validarAprovadorNode.AppendChild(docFormData.CreateTextNode(this.chkValidarAprovador.Checked.ToString().ToLower()));

            this.m_formData = docFormData.OuterXml;
            this.PerformAssociation();

            string url = "WrkSetng.aspx" + this.m_strQueryParams;
            SPUtility.Redirect(url, SPRedirectFlags.RelativeToLayoutsPage, this.Context);
        }
        #endregion
    }
}
