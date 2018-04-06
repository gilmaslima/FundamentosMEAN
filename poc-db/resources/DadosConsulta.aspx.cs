#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
- [03/08/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.ServiceModel;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;
using System.Data;
using System.Xml.Linq;
using System.Collections.Generic;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public class DadosConsulta : SustentacaoApplicationPageBase
    {
        public String UsuarioLogado
        {
            get
            {
                return ViewState["UsuarioLogado"] == null ? null : (String)ViewState["UsuarioLogado"];
            }
            set
            {
                ViewState["UsuarioLogado"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlProcedures;

        /// <summary>
        /// 
        /// </summary>
        protected HyperLink linkListaProcs;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlDados;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlBanco;

        /// <summary>
        /// 
        /// </summary>
        protected List<string> listaProcs;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtStringConexao;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox txtCommand;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblResultado;

        /// <summary>
        /// 
        /// </summary>
        protected Repeater rptTables;

        /// <summary>
        /// 
        /// </summary>
        protected Repeater rptParameters;

        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;

        /// <summary>
        /// 
        /// </summary>
        protected Literal ltSignificado;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErro;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlSelect;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlProcedure;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboBancoDados();
                CarregarProcedures();
            }
        }

        private void CarregaComboBancoDados()
        {
            string[] bancosPermitidos = new string[] { "SQLServerPN", "SQLServerLog", "SQLServerAC", "SQLServerGSEDM", "SQLServerIS" };

            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    List<String> listaBancoDados = client.ListarConnectionStrings().ToList();
                    List<String> listaBancosPermitidos = new List<string>();

                    foreach (string banco in listaBancoDados)
                    {
                        if (bancosPermitidos.Contains(banco))
                        {
                            listaBancosPermitidos.Add(banco);
                        }
                    }

                    if (listaBancoDados != null && listaBancoDados.Count > 0)
                    {
                        ddlBanco.DataSource = listaBancosPermitidos;
                        ddlBanco.DataBind();
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ConsultarSQL(object sender, EventArgs e)
        {
            string script = "";

            if (pnlSelect.Visible)
            {
                if (ValidarQuery())
                {
                    script = txtCommand.Text;
                    ConsultarQuerySQLServer(script);
                }
                else
                {
                    lblResultado.Text = "";
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    Response.Write("<script type='text/javascript'>alert('Apenas comandos SELECT são permitidos para realizar queries na base. Comandos como DROP, DELETE, ALTER, SELECT INTO, INSERT, UPDATE etc não são permitidos.');</script>");
                    ltErro.Text = "Apenas comandos SELECT são permitidos para realizar queries na base. Comandos como DROP, DELETE, ALTER, SELECT INTO, INSERT, UPDATE etc não são permitidos.";
                }
            }
            else
            {
                script = ddlProcedures.SelectedItem.Text += " ";

                foreach (RepeaterItem item in rptParameters.Items)
                {
                    TextBox txtParametro = ((TextBox)rptParameters.Items[item.ItemIndex].Controls[1]);
                    if (!String.IsNullOrEmpty(txtParametro.Text))
                        script += " " + txtParametro.Text + ",";
                }

                script = script.Remove(script.Length - 1, 1);
                ConsultarQuerySQLServer(script);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidarQuery()
        {
            bool queryValida = true;

            if (!txtCommand.Text.ToUpper().Contains("UPDATE") && !txtCommand.Text.ToUpper().Contains("CREATE") && !txtCommand.Text.ToUpper().Contains("DELETE") && !txtCommand.Text.ToUpper().Contains("INSERT") && !txtCommand.Text.ToUpper().Contains("EXECUTE") && !txtCommand.Text.ToUpper().Contains("DROP") && !txtCommand.Text.ToUpper().Contains("TRUNCATE"))
            {
                if (txtCommand.Text.ToUpper().Contains("SELECT") && txtCommand.Text.ToUpper().Contains("INTO"))
                {
                    queryValida = false;
                }
                else
                {
                    listaProcs = ddlProcedures.Items.Cast<ListItem>().Select(i => i.Text).ToList();

                    for (int i = 0; i < listaProcs.Count; i++)
                    {
                        if (txtCommand.Text.ToUpper().Contains(listaProcs[i]) || txtCommand.Text.Contains(listaProcs[i]))
                            queryValida = false;
                    }
                }
            }
            else
            {
                queryValida = false;
            }

            return queryValida;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarProcedures();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlDados_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlDados.SelectedValue)
            {
                case "1":
                    pnlProcedure.Visible = false;
                    pnlErro.Visible = false;
                    rptTables.Visible = false;
                    pnlSelect.Visible = true;
                    break;

                case "2":
                    pnlSelect.Visible = false;
                    pnlErro.Visible = false;
                    rptTables.Visible = false;
                    pnlProcedure.Visible = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarProcedures()
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    DataTable[] tables = client.ConsultarQuerySQLServer("SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES  WHERE ROUTINE_TYPE = 'PROCEDURE'", this.UsuarioLogado, "Consulta de Procedures via Sistema", ddlBanco.SelectedItem.Text);
                    List<string> listaProcedures = new List<string>();

                    foreach (DataRow row in tables[0].Rows)
                        listaProcedures.Add(row[0].ToString());

                    if (listaProcedures != null && listaProcedures.Count > 0)
                    {
                        VerificarSignificados(listaProcedures.OrderBy(q => q).ToList());

                        ddlProcedures.DataSource = listaProcedures.OrderBy(q => q).ToList();
                        ddlProcedures.DataBind();
                    }

                    ddlProcedures.Items.Insert(0, new ListItem()
                    {
                        Text = "Selecione...",
                        Value = "",
                        Selected = true
                    });
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listaProcedures"></param>
        private void VerificarSignificados(List<string> listaProcedures)
        {
            using (SPWeb mySite = SPContext.Current.Web)
            {
                SPListCollection listas = mySite.Lists;
                SPList listaProcs = listas.TryGetList("Procedures");

                if (Request.Url.ToString().Contains("pi"))
                    linkListaProcs.NavigateUrl = Request.Url.ToString().Replace("/_layouts/Sustentacao/dadosconsulta.aspx", "/Lists/Procedures/AllItems.aspx");
                else
                    linkListaProcs.NavigateUrl = Request.Url.ToString().Replace("/_layouts/Sustentacao/dadosconsulta.aspx", "/sites/fechado/Lists/Procedures/AllItems.aspx");
                
                if (listaProcs == null)
                    CriarListaSignificados();

                listaProcs = listas.TryGetList("Procedures");

                foreach (string proc in listaProcedures)
                {
                    SPQuery query = new SPQuery();
                    query.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + proc + "</Value></Eq></Where>";
                    SPListItemCollection found = listaProcs.GetItems(query);

                    if (found.Count <= 0)
                    {
                        SPListItemCollection listItems = listaProcs.Items;
                        SPListItem item = listItems.Add();
                        item["Title"] = proc;
                        item["Banco"] = ddlBanco.SelectedItem.Text;
                        item.Update();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CriarListaSignificados()
        {
            using (SPWeb mySite = SPContext.Current.Web)
            {
                bool allowUnsafeUpdates = mySite.AllowUnsafeUpdates;
                SPListCollection listas = mySite.Lists;
                mySite.AllowUnsafeUpdates = true;
                listas.Add("Procedures", "Significado das Procedures", SPListTemplateType.GenericList);
                SPList listaProcs = listas["Procedures"];
                listaProcs.Fields.Add("Significado", SPFieldType.Text, true);
                listaProcs.Fields.Add("Banco", SPFieldType.Text, true);
                mySite.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomeProcedure"></param>
        private void EncontrarSignificadoLista(string nomeProcedure)
        {
            try
            {
                using (SPWeb mySite = SPContext.Current.Web)
                {
                    SPListCollection listas = mySite.Lists;
                    SPList listaProcs = listas.TryGetList("Procedures");
                    SPQuery query = new SPQuery();
                    query.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + nomeProcedure + "</Value></Eq></Where>";
                    SPListItem item = listaProcs.GetItems(query).Cast<SPListItem>().FirstOrDefault();
                    ltSignificado.Text = item["Significado"].ToString();
                }
            }
            catch (Exception ex)
            {
                ltSignificado.Text = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BuscarParametrosProcedure(object sender, EventArgs e)
        {
            if (ddlProcedures.SelectedValue != "0")
            {
                using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
                {
                    try
                    {
                        EncontrarSignificadoLista(ddlProcedures.SelectedItem.Text);

                        DataTable[] tables = client.ConsultarQuerySQLServer(String.Format("SELECT PARAMETER_NAME + ' (' + DATA_TYPE + ')' FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = '{0}'", ddlProcedures.SelectedItem.Text), this.UsuarioLogado, "Consulta de parâmetros da PROC via Sistema", ddlBanco.SelectedItem.Text);
                        List<string> listaParametros = new List<string>();

                        foreach (DataRow row in tables[0].Rows)
                            listaParametros.Add(row[0].ToString());

                        if (listaParametros.Count > 0)
                        {
                            rptParameters.DataSource = listaParametros;
                            rptParameters.DataBind();
                        }
                        else
                        {
                            rptParameters.DataSource = null;
                            rptParameters.DataBind();
                        }
                    }
                    catch (FaultException<GeneralFault> ex)
                    {
                        lblResultado.Text = "";
                        pnlErro.Visible = true;
                        rptTables.Visible = false;
                        ltErro.Text = ex.Reason.ToString();
                    }
                    catch (Exception ex)
                    {
                        lblResultado.Text = "";
                        pnlErro.Visible = true;
                        rptTables.Visible = false;
                        ltErro.Text = ex.Message;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        protected void ConsultarQuerySQLServer(string script)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    string infoOperacao = string.Format("Executada query no banco {0}: {1}", ddlBanco.SelectedItem.Text, script);

                    DataTable[] tables = client.ConsultarQuerySQLServer(script, this.UsuarioLogado, infoOperacao, ddlBanco.SelectedItem.Text);

                    if (tables == null)
                    {
                        lblResultado.Text = "Operação executada com sucesso.";

                        pnlErro.Visible = false;
                        rptTables.Visible = false;
                    }
                    else
                    {
                        if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                        {
                            rptTables.DataSource = tables;
                            rptTables.DataBind();
                        }

                        lblResultado.Text = "";
                        pnlErro.Visible = false;
                        rptTables.Visible = true;
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    lblResultado.Text = "";
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    lblResultado.Text = "";
                    pnlErro.Visible = true;
                    rptTables.Visible = false;
                    ltErro.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TablesDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataTable dt = e.Item.DataItem as DataTable;
                GridView dg = e.Item.FindControl("dGrid") as GridView;

                if (!object.ReferenceEquals(dt, null) && !object.ReferenceEquals(dg, null))
                {
                    dg.DataSource = dt;
                    dg.DataBind();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dGrid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell item in e.Row.Cells)
                {
                    try
                    {
                        var testexml = XDocument.Parse(Server.HtmlDecode(item.Text).Trim());

                        LinkButton lnkView = new LinkButton();
                        lnkView.ID = "lnkView";
                        lnkView.Text = "Ver XML";
                        lnkView.Attributes.Add("style", "color: blue; text-decoration: underline");
                        lnkView.Attributes.Add("xml", FormatXml(item.Text));
                        lnkView.OnClientClick = "return ClickMostrarXML(this); return false;";

                        if (Server.HtmlDecode(item.Text).Trim() != "")
                        {
                            lnkView.Visible = true;
                        }
                        else
                        {
                            lnkView.Visible = false;
                        }

                        item.Controls.Add(lnkView);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptParameters_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            var textbox = e.Item.FindControl("txtValor");
            textbox.ID = "txtValor" + (e.Item.ItemIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception ex)
            {
                return xml;
            }
        }
    }
}