/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using Redecard.PN.Sustentacao.SharePoint.Helpers;
using System.Diagnostics;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public partial class ServicosConsulta : SustentacaoApplicationPageBase
    {
        #region [ Attributes and Properties ]

        /// <summary>
        /// Default Text Identity.
        /// </summary>
        private const Int32 defaultTextIdentity = 5;

        /// <summary>
        /// Dicionário de todos os assemblies gerados, que possui como chave a url do serviço.
        /// </summary>
        public List<ServiceSession> CurrentServices
        {
            get { return HttpContext.Current.Session[SessionVariables.CurrentServices] != null ? 
                (List<ServiceSession>)HttpContext.Current.Session[SessionVariables.CurrentServices] : new List<ServiceSession>(); }
            
            set { HttpContext.Current.Session[SessionVariables.CurrentServices] = value; }
        }

        /// <summary>
        /// Lista de parâmetros do serviço selecionado atualmente, em session.
        /// </summary>        
        public List<Field> CurrentsParameters
        {
            get { return Session[SessionVariables.CurrentParameters] != null ? (List<Field>)Session[SessionVariables.CurrentParameters] : new List<Field>(); }
            set { Session[SessionVariables.CurrentParameters] = value; }
        }

        /// <summary>
        /// Lista dos campos do resultado atual, em session.
        /// </summary>
        public List<Field> CurrentsResults
        {
            get { return HttpContext.Current.Session[SessionVariables.CurrentsResults] != null ? 
                (List<Field>)HttpContext.Current.Session[SessionVariables.CurrentsResults] : new List<Field>(); }

            set { HttpContext.Current.Session[SessionVariables.CurrentsResults] = value; }
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Evento Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!this.ValidarChaveAcesso())
            {                
                throw new SPException("Access denied");
            }

            if (!IsPostBack)
            {
                this.CurrentServices = new List<ServiceSession>();
                this.CurrentsParameters = new List<Field>();
                this.CurrentsResults = new List<Field>();
            }
        }

        /// <summary>
        /// Evento click do botão btnAddService
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddService_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode treeNodeRoot;

                TreeNode treeNodeChild = new TreeNode(txtUrl.Text);
                this.AddServiceNode(treeNodeChild);

                if (this.trvMethods.Nodes != null && this.trvMethods.Nodes.Count > 0)
                {
                    treeNodeRoot = this.trvMethods.Nodes[0];
                    treeNodeRoot.ChildNodes.Add(treeNodeChild);
                }
                else
                {
                    treeNodeRoot = new TreeNode("My Services");
                    treeNodeRoot.ChildNodes.Add(treeNodeChild);
                    this.trvMethods.Nodes.Add(treeNodeRoot);
                }

                treeNodeRoot.Expand();
                treeNodeChild.Expand();
            }
            catch (FileNotFoundException fEx)
            {
                DisplayError(fEx);
            }
            catch (NullReferenceException nrfEx)
            {
                DisplayError(nrfEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento SelectedNodeChanged da tree view trvMethods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void trvMethods_SelectedNodeChanged(Object sender, EventArgs e)
        {
            MakeGridParameters();
        }

        /// <summary>
        /// Evento RowDataBound do gridView grvRequest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvRequest_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.DataItem != null)
                {
                    Field item = e.Row.DataItem as Field;
                    var txtName = e.Row.FindControl("txtName") as TextBox;
                    txtName.Attributes.Add("style", String.Format("text-indent:{0}%", item.TextIdentity));

                    var ddlValue = e.Row.FindControl("ddlValue") as DropDownList;
                    var txtValue = e.Row.FindControl("txtValue") as TextBox;
                    var txtValueArray = e.Row.FindControl("txtValueArray") as TextBox;

                    if (item.EditorType == EditorType.TextBox)
                    {
                        txtValue.Text = CurrentsParameters[e.Row.DataItemIndex].Value;
                        txtValue.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
                        txtValue.Visible = true;
                        ddlValue.Visible = false;
                        txtValueArray.Visible = false;
                    }
                    else if (item.EditorType == EditorType.TextBoxArray)
                    {
                        txtValueArray.Text = CurrentsParameters[e.Row.DataItemIndex].Value;
                        txtValueArray.Visible = true;
                        txtValue.Visible = false;
                        ddlValue.Visible = false;
                    }
                    else
                    {
                        if (String.Compare(item.FriendlyTypeName, "System.Boolean") == 0)
                        {
                            ddlValue.Items.Add(new ListItem()
                            {
                                Text = Boolean.FalseString,
                                Value = Boolean.FalseString,
                                Selected = String.Compare(CurrentsParameters[e.Row.DataItemIndex].Value, Boolean.FalseString) == 0
                            });

                            ddlValue.Items.Add(new ListItem()
                            {
                                Text = Boolean.TrueString,
                                Value = Boolean.TrueString,
                                Selected = String.Compare(CurrentsParameters[e.Row.DataItemIndex].Value, Boolean.TrueString) == 0
                            });
                        }
                        else
                        {

                            ddlValue.Items.Add(new ListItem() { Text = "(null)", Value = null });

                            ddlValue.Items.Add(new ListItem()
                            {
                                Text = item.FriendlyTypeName,
                                Value = item.FriendlyTypeName,
                                Selected = String.Compare(CurrentsParameters[e.Row.DataItemIndex].Value, "(null)") != 0
                            });
                        }

                        ddlValue.Visible = true;
                        txtValue.Visible = false;
                        txtValueArray.Visible = false;
                    }

                }
            }
            catch (IndexOutOfRangeException iEx)
            {
                DisplayError(iEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento RowDataBound do gridView grvResponse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvResponse_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.DataItem != null)
                {
                    Field item = e.Row.DataItem as Field;
                    var divNameResponse = e.Row.FindControl("divResponseName") as HtmlGenericControl;
                    if (divNameResponse != null)
                        divNameResponse.Attributes.Add("style", String.Format("padding-left:{0}%", item.TextIdentity));

                    if ((item.EditorType == EditorType.TextBoxArray || item.EditorType == EditorType.DropDownBox) &&
                        String.Compare(item.FriendlyTypeName, "System.Boolean") != 0)
                    {
                        var lnkExpandLevel = e.Row.FindControl("lnkExpandLevel") as LinkButton;
                        var lnkCollapseAll = e.Row.FindControl("lnkCollapseAll") as LinkButton;

                        if (lnkExpandLevel != null)
                            lnkExpandLevel.Visible = item.IsCollapsed;

                        if (lnkCollapseAll != null)
                            lnkCollapseAll.Visible = !item.IsCollapsed;
                    }
                }
            }
            catch (InvalidOperationException iOex)
            {
                DisplayError(iOex);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento TextChanged do TextBox txtValueArray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtValueArray_TextChanged(Object sender, EventArgs e)
        {
            try
            {
                var txtValue = sender as TextBox;
                var row = (GridViewRow)txtValue.Parent.Parent;

                if (CurrentsParameters[row.DataItemIndex] != null)
                {
                    UpdateCurrentParameters((GridView)row.Parent.Parent);
                    Field currentField = CurrentsParameters[row.DataItemIndex];

                    if (!String.IsNullOrWhiteSpace(currentField.Value))
                    {
                        String pattern = @"^length=\d{1,9}$";
                        Boolean lengthIsValid = Regex.IsMatch(txtValue.Text, pattern, RegexOptions.IgnoreCase);

                        if (!lengthIsValid)
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), String.Format("Erro_{0}", Guid.NewGuid().ToString()),
                                "alert('Valor inválido para a propriedade. Digite: length=[Num. Itens]');", true);
                        }
                        else
                        {
                            Field[] childFields = currentField.GetChildFields();

                            foreach (Field field in childFields)
                            {
                                field.TextIdentity = currentField.TextIdentity + defaultTextIdentity;
                            }

                            CurrentsParameters.RemoveAll(i => i.Parent == currentField); //Remove todos os itens atuais, atualiza utilizando os novos e mantendo os atuais.
                            CurrentsParameters.InsertRange(row.RowIndex + 1, childFields);
                        }
                    }
                    else
                    {
                        RemoveAllDescendants(currentField, CurrentsParameters, true);
                    }
                }

                ClearResponseGrid();
                grvRequest.DataSource = CurrentsParameters;
                grvRequest.DataBind();
            }
            catch (ArgumentNullException anEx)
            {
                DisplayError(anEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento SelectedIndexChanged do DropDown ddlValue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlValue_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try
            {
                var childFields = new Field[] { };
                var ddl = sender as DropDownList;
                var row = (GridViewRow)ddl.Parent.Parent;

                UpdateCurrentParameters((GridView)row.Parent.Parent);
                Field currentField = CurrentsParameters[row.DataItemIndex];

                if (String.Compare(currentField.FriendlyTypeName, "System.Boolean") != 0)
                {
                    if (String.Compare((sender as DropDownList).SelectedValue, "(null)") != 0)
                    {
                        childFields = currentField.GetChildFields();

                        foreach (Field field in childFields)
                        {
                            field.TextIdentity = currentField.TextIdentity + defaultTextIdentity;
                        }

                        CurrentsParameters.InsertRange(row.RowIndex + 1, childFields);
                    }
                    else
                    {
                        RemoveAllDescendants(currentField, CurrentsParameters, true);
                    }
                }

                ClearResponseGrid();
                grvRequest.DataSource = CurrentsParameters;
                grvRequest.DataBind();
            }
            catch (IndexOutOfRangeException iOEx)
            {
                DisplayError(iOEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        ///  Evento SelectedIndexChanged do DropDown ddlValueResponse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlValueResponse_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try
            {
                var childFields = new Field[] { };
                var ddl = sender as DropDownList;
                var row = (GridViewRow)ddl.Parent.Parent;

                //UpdateCurrentsResults((GridView)row.Parent.Parent);
                Field currentField = CurrentsResults[row.DataItemIndex];

                if (String.Compare(currentField.FriendlyTypeName, "System.Boolean") != 0)
                {
                    if (String.Compare((sender as DropDownList).SelectedValue, "(null)") != 0)
                    {
                        childFields = currentField.GetChildFields();

                        foreach (Field field in childFields)
                        {
                            field.TextIdentity = currentField.TextIdentity + defaultTextIdentity;
                        }

                        CurrentsResults.InsertRange(row.RowIndex + 1, childFields);
                    }
                    else
                    {
                        RemoveAllDescendants(currentField, CurrentsResults, false);
                    }
                }

                grvResponse.DataSource = CurrentsResults;
                grvResponse.DataBind();
            }
            catch (IndexOutOfRangeException ioEx)
            {
                DisplayError(ioEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento click do botão btnInvoke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInvoke_Click(Object sender, EventArgs e)
        {
            try
            {
                this.grvRequest = ((sender as Button).Parent.Parent.FindControl("grvRequest") as GridView);
                UpdateCurrentParameters(this.grvRequest);

                String endpointAddress = this.trvMethods.SelectedNode.Parent.Parent.Value;
                String operationContractAndEndpointName = this.trvMethods.SelectedNode.Parent.Value;
                String methodName = this.trvMethods.SelectedValue.Replace("()", String.Empty);
                AppDomain appDomain = null;

                ServiceMethodInfo method = GetServiceMethodInfo(endpointAddress, operationContractAndEndpointName, methodName, out appDomain);

                List<Field> inputParameters = (CurrentsParameters.ToArray().Clone() as Field[]).ToList();

                foreach (Field field in CurrentsParameters)
                {
                    if (field.IsExpandable())
                    {
                        RemoveAllDescendants(field, inputParameters, false);
                    }
                }

                var serviceInputs = new ServiceInvocationInputs(inputParameters.ToArray(), method, appDomain, this.chkStartNewProxy.Checked);

                ServiceInvocationOutputs serviceOutputs = ServiceExecutor.ExecuteInClientDomain(serviceInputs);

                if (serviceOutputs.ServiceInvocationResult != null && serviceOutputs.ServiceInvocationResult.Count() > 0 &&
                    serviceOutputs.HasError == false)
                {
                    if (serviceOutputs.ServiceInvocationResult[0].FriendlyTypeName.Contains("System.Data.DataTable"))
                    {
                        this.divResponse.Visible = false;
                        this.divError.Visible = false;
                        this.divDataTable.Visible = true;
                        Field[] childFields = serviceOutputs.ServiceInvocationResult.First().GetChildFields();
                        this.grvDataTable.DataSource = (childFields.First() as DataTableField).DataTableValue;
                        this.grvDataTable.DataBind();
                    }
                    else
                    {
                        this.divResponse.Visible = true;
                        this.divDataTable.Visible = false;
                        this.divError.Visible = false;

                        this.CurrentsResults.Clear();
                        this.CurrentsResults.InsertRange(0, serviceOutputs.ServiceInvocationResult);

                        ExpandLevel(serviceOutputs.ServiceInvocationResult.FirstOrDefault());

                        grvResponse.DataSource = this.CurrentsResults;
                        grvResponse.DataBind();
                    }
                }
                else if (serviceOutputs.HasError)
                {
                    DisplayError(serviceOutputs.ErrorMessage);
                }
            }
            catch (FileNotFoundException fEx)
            {
                DisplayError(fEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento click do Link Buttom lnkExpandLevel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkExpandLevel_Click(Object sender, EventArgs e)
        {
            try
            {
                var lnkExpandLevel = sender as LinkButton;
                var row = (GridViewRow)lnkExpandLevel.Parent.Parent.Parent;

                if (CurrentsResults[row.DataItemIndex] != null)
                {
                    Field currentField = CurrentsResults[row.DataItemIndex];
                    ExpandLevel(currentField);
                }

                grvResponse.DataSource = CurrentsResults;
                grvResponse.DataBind();
            }
            catch (IndexOutOfRangeException fEx)
            {
                DisplayError(fEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento click do Link Buttom lnkCollapseAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkCollapseAll_Click(Object sender, EventArgs e)
        {
            try
            {
                var lnkExpandAll = sender as LinkButton;
                var row = (GridViewRow)lnkExpandAll.Parent.Parent.Parent;

                if (CurrentsResults[row.DataItemIndex] != null)
                {
                    Field currentField = CurrentsResults[row.DataItemIndex];
                    RemoveAllDescendants(currentField, CurrentsResults, false);
                }

                grvResponse.DataSource = CurrentsResults;
                grvResponse.DataBind();
            }
            catch (IndexOutOfRangeException fEx)
            {
                DisplayError(fEx);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// Evento click do botão btnImportRequest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportRequest_Click(object sender, EventArgs e)
        {
            if (this.fluImportRequest.HasFile)
            {
                String fileName = Path.GetFileName(this.fluImportRequest.FileName);
                String importRequestPath = Path.Combine(Path.GetTempPath(), "PN_WCFTestWeb\\ImportRequest");
                String importRequestFileFullPath = Path.Combine(importRequestPath, fileName);
                String currentMethodName = this.trvMethods.SelectedValue;

                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    try
                    {
                        if (!Directory.Exists(importRequestPath))
                            Directory.CreateDirectory(importRequestPath);

                        this.fluImportRequest.SaveAs(importRequestFileFullPath);
                        this.fluImportRequest.FileContent.Dispose();

                        var serializer = new DataContractSerializer(typeof(Dictionary<String, List<Field>>));

                        using (Stream fs = new FileStream(importRequestFileFullPath, FileMode.Open))
                        {
                            using (XmlReader reader = new XmlTextReader(fs))
                            {
                                var importedParameters = serializer.ReadObject(reader) as Dictionary<String, List<Field>>;

                                reader.Close();
                                reader.Dispose();

                                if (importedParameters != null && importedParameters.Count > 0)
                                {
                                    if (String.Compare(importedParameters.Keys.FirstOrDefault(), currentMethodName) != 0)
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(),
                                            String.Format("Erro_{0}", Guid.NewGuid().ToString()), "alert('Arquivo inválido para o método selecionado.');", true);
                                        return;
                                    }

                                    this.CurrentsParameters.Clear();
                                    ClearResponseGrid();
                                    this.CurrentsParameters.AddRange(importedParameters.Values.FirstOrDefault());
                                    this.grvRequest.DataSource = this.CurrentsParameters;
                                    this.grvRequest.DataBind();
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), 
                                        String.Format("Erro_{0}", Guid.NewGuid().ToString()), "alert('Formato de arquivo inválido.');", true);
                                    return;
                                }
                            };
                        };

                    }
                    catch (FileNotFoundException fEx)
                    {
                        DisplayError(fEx);
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex);
                    }
                    finally
                    {
                        File.Delete(importRequestFileFullPath);
                        if (File.Exists(importRequestFileFullPath))
                        {
                            DisplayError(String.Format("Não foi possível apagar o arquivo temporário: {0}", importRequestFileFullPath));
                        }
                    }
                });
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    String.Format("Erro_{0}", Guid.NewGuid().ToString()), "alert('Selecione um arquivo.');", true);                
            }
        }

        /// <summary>
        /// Evento click do botão btnExportRequest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportRequest_Click(object sender, EventArgs e)
        {
            this.UpdateCurrentParameters(this.grvRequest);
            String exportRequestPath = Path.Combine(Path.GetTempPath(), "PN_WCFTestWeb\\ExportRequest");
            String fileName = String.Format("{0}.xml", this.trvMethods.SelectedValue.Replace("()", String.Empty));
            String exportRequestFileFullPath = Path.Combine(exportRequestPath, fileName);

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {
                    if (!Directory.Exists(exportRequestPath))
                        Directory.CreateDirectory(exportRequestPath);
                    
                    var serializer = new DataContractSerializer(typeof(Dictionary<String, List<Field>>));
                    var exportParameters = new Dictionary<String, List<Field>>();
                    exportParameters.Add(this.trvMethods.SelectedValue, this.CurrentsParameters);


                    using (Stream fs = new FileStream(exportRequestFileFullPath, FileMode.OpenOrCreate))
                    {
                        using (XmlWriter writer = new XmlTextWriter(fs, Encoding.UTF8))
                        {
                            serializer.WriteObject(writer, exportParameters);
                            writer.Close();
                            writer.Dispose();
                        };
                    };

                    String exportFileContent = null;
                    using (var sr = new StreamReader(exportRequestFileFullPath))
                    {
                        exportFileContent = sr.ReadToEnd();
                        sr.Close();

                        String attachment = String.Format("attachment; filename={0}", fileName);
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/xml";
                        Response.AddHeader("content-disposition", attachment);
                        Response.AddHeader("content-Length", exportFileContent.Length.ToString());
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Write(exportFileContent);
                        Response.Flush();

                    }

                }
                catch (FileLoadException fEx)
                {
                    DisplayError(fEx);
                }
                catch (SerializationException sEx)
                {
                    DisplayError(sEx);
                }
                catch (ThreadAbortException thrdEx)
                {
                    DisplayError(thrdEx);
                }
                catch (Exception ex)
                {
                    DisplayError(ex);
                }
                finally
                {
                    File.Delete(exportRequestFileFullPath);
                    if (File.Exists(exportRequestFileFullPath))
                    {
                        DisplayError(String.Format("Não foi possível apagar o arquivo temporário: {0}", exportRequestFileFullPath));
                    }
                }
            });
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Adiciona um novo nó de serviço.
        /// </summary>
        /// <param name="serviceNode">Objeto TreeNode.</param>
        private void AddServiceNode(TreeNode serviceNode)
        {
            this.divError.Visible = false;
            String endpointAddress = serviceNode.Value;
            AppDomain appDomain = null;

            List<ClientEndpointInfo> endpoints = GetClientEndpointInfoList(endpointAddress, out appDomain);

            foreach (ClientEndpointInfo current in endpoints)
            {
                TreeNode treeNode = new TreeNode(current.ToString());
                serviceNode.ChildNodes.Add(treeNode);

                foreach (ServiceMethodInfo method in current.Methods)
                {
                    TreeNode methodTreeNode = new TreeNode(method.MethodName + "()");
                    treeNode.ChildNodes.Add(methodTreeNode);
                }
            }
        }

        /// <summary>
        /// Monta o grid para preenchimento dos parâmetros do método selecionado na treeView
        /// </summary>
        private void MakeGridParameters()
        {
            if (this.trvMethods.SelectedValue.Contains("()"))
            {
                this.CurrentsParameters.Clear();
                this.CurrentsResults.Clear();
                this.divDataTable.Visible = false;
                this.spnImportExport.Visible = true;
                this.divResponse.Visible = true;
                this.divError.Visible = false;

                String endpointAddress = this.trvMethods.SelectedNode.Parent.Parent.Value;
                String operationContractAndEndpointName = this.trvMethods.SelectedNode.Parent.Value;
                String methodName = this.trvMethods.SelectedValue.Replace("()", String.Empty);
                AppDomain appDomain;

                ServiceMethodInfo method = GetServiceMethodInfo(endpointAddress, operationContractAndEndpointName, methodName, out appDomain);

                if (method != null)
                {
                    this.ltrLegendMethod.Text = methodName;
                    Field[] fields = method.GetFields();

                    this.spnImportExport.Visible = (fields != null && fields.Count() > 0);

                    this.CurrentsParameters.AddRange(fields);
                    this.grvRequest.DataSource = this.CurrentsParameters;
                    this.grvRequest.DataBind();

                    this.grvResponse.DataSource = this.CurrentsResults;
                    this.grvResponse.DataBind();
                }
            }
        }

        /// <summary>
        /// Atualiza a Session com os valores preenchidos no grid de parâmetros.
        /// </summary>
        /// <param name="grid">Objeto GridVeiw</param>
        private void UpdateCurrentParameters(GridView grid)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                var txtValue = (row.FindControl("txtValue") as TextBox);
                var txtValueArray = (row.FindControl("txtValueArray") as TextBox);
                var ddlValue = (row.FindControl("ddlValue") as DropDownList);

                if (txtValue.Visible == true)
                {
                    CurrentsParameters[row.DataItemIndex].Value = txtValue.Text;
                }
                else if (txtValueArray.Visible == true)
                {
                    CurrentsParameters[row.DataItemIndex].Value = txtValueArray.Text;
                }
                else if (ddlValue.Visible == true)
                {
                    CurrentsParameters[row.DataItemIndex].Value = ddlValue.SelectedValue;
                }
            }

        }

        /// <summary>
        /// Obtem a lista de endpoints baseado no Addreess e no client domain 
        /// </summary>
        /// <param name="endpointAddress">URL com o Address do endpoint.</param>
        /// <param name="appDomain">Client Domain</param>
        /// <returns>Lista de endpoints.</returns>
        private List<ClientEndpointInfo> GetClientEndpointInfoList(String endpointAddress, out AppDomain appDomain)
        {
            List<ServiceSession> listServiceSession = this.CurrentServices;

            ServiceSession serviceSession = listServiceSession.Where(i => String.Compare(i.EndpointAddress, endpointAddress) == 0).FirstOrDefault();

            ServiceProject serviceProject = null;

            if (serviceSession == null)
            {
                serviceSession = new ServiceSession { EndpointAddress = endpointAddress };
                serviceProject = ServiceAnalyzer.AnalyzeService(endpointAddress, serviceSession);
                listServiceSession.Add(serviceSession);
            }
            else
            {
                serviceProject = ServiceAnalyzer.AnalyzeService(endpointAddress, serviceSession);
            }

            this.CurrentServices = listServiceSession;

            if (serviceProject != null && serviceProject.Endpoints != null)
            {
                appDomain = serviceProject.Domain;
                return serviceProject.Endpoints.ToList();
            }

            appDomain = null;
            return null;
        }

        /// <summary>
        /// Obtem as informações do método.
        /// </summary>
        /// <param name="endpointAddress">URL com o address do endpoint.</param>
        /// <param name="operationContractAndEndpointName">Nome do operation contract concatenado com o nome do endpoint.</param>
        /// <param name="methodName">Nome do método</param>
        /// <param name="appDomain">Client domain.</param>
        /// <returns></returns>
        private ServiceMethodInfo GetServiceMethodInfo(String endpointAddress, String operationContractAndEndpointName, String methodName, out AppDomain appDomain)
        {
            List<ClientEndpointInfo> endpoints = GetClientEndpointInfoList(endpointAddress, out appDomain);

            if (endpoints != null)
            {
                ClientEndpointInfo currentEndPoint = endpoints.Where(i => String.Compare(i.ToString(), operationContractAndEndpointName) == 0).FirstOrDefault();

                if (currentEndPoint != null)
                {
                    ServiceMethodInfo method = currentEndPoint.Methods.Where(i => String.Compare(i.MethodName, methodName) == 0).FirstOrDefault();
                    return method;
                }
            }

            appDomain = null;
            return null;
        }

        /// <summary>
        /// Remove o campo e todos os campos filhos.
        /// </summary>
        /// <param name="field">Campo raíz.</param>
        /// <param name="listFields">Lista da qual será removida</param>
        /// <param name="setNullChildFieldsProperty">Flag que indica se deve setar a lista ChildFields para null.</param>
        private void RemoveAllDescendants(Field field, List<Field> listFields, Boolean setNullChildFieldsProperty)
        {
            Field[] chieldFields = listFields.Where(i => i.Parent == field).ToArray();

            foreach (Field childField in chieldFields)
            {
                RemoveAllDescendants(childField, listFields, setNullChildFieldsProperty);
            }

            listFields.RemoveAll(i => i.Parent == field);

            if (setNullChildFieldsProperty)
                field.SetChildFields(null);

            field.IsCollapsed = true;
        }

        /// <summary>
        /// Expande um nível do campo específicado para exibição no grid.
        /// </summary>
        /// <param name="currentField">Campo que será expandido.</param>
        private void ExpandLevel(Field currentField)
        {
            var childFields = new Field[] { };

            if (currentField != null && String.Compare(currentField.FriendlyTypeName, "System.Boolean") != 0)
            {
                childFields = currentField.GetChildFields();

                if (childFields != null && childFields.Count() > 0)
                {
                    foreach (Field field in childFields)
                    {
                        field.TextIdentity = currentField.TextIdentity + defaultTextIdentity;
                        field.Parent = currentField;
                    }

                    CurrentsResults.InsertRange(CurrentsResults.IndexOf(currentField) + 1, childFields);
                }

                currentField.IsCollapsed = false;
            }
        }

        /// <summary>
        /// Limpa o grid de resultados.
        /// </summary>
        private void ClearResponseGrid()
        {
            CurrentsResults.Clear();
            grvResponse.DataSource = CurrentsResults;
            grvResponse.DataBind();
        }

        /// <summary>
        /// Formata e exibe um Exception na tela.
        /// </summary>
        /// <param name="error">Objeto Exception.</param>
        private void DisplayError(Exception error)
        {
            String errorMessage = Util.FormatExceptionMessage(error);
            this.txaError.InnerText = errorMessage;
            this.divResponse.Visible = false;
            this.divError.Visible = true;
        }

        /// <summary>
        /// Exibe painel de erro
        /// </summary>
        /// <param name="errorMessage">Mensagem de erro a ser exibida.</param>
        private void DisplayError(String errorMessage)
        {
            this.txaError.InnerText = errorMessage;
            this.divResponse.Visible = false;
            this.divError.Visible = true;
        }

        #endregion                         

    }
}

