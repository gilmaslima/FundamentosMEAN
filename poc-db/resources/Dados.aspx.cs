#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [30/08/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.ServiceModel;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System.Data;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public class DadosExecucao : SustentacaoApplicationPageBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlBancoDados;

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
        protected Repeater rptTables;

        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;

        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErro;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.ValidarChaveAcesso())
            {
                throw new SPException("Acesso negado");
            }

            if(!IsPostBack)
                CarregaComboBancoDados();
        }

        /// <summary>
        /// Realiza a chamada do serviço para executar o comando SQL
        /// </summary>
        /// <remarks>
        /// Realiza a chamada do serviço para executar o comando SQL
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ConsultarSQL(object sender, EventArgs e)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    string conexao = string.Format("{0}|", ddlBancoDados.SelectedValue);
                    if (String.Compare(ddlBancoDados.SelectedValue.ToLower(), "cnx", true) == 0)
                    {
                        conexao += txtStringConexao.Text;
                    }

                    DataTable[] tables = client.ConsultarSql(conexao, txtCommand.Text);
                    if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                    {
                        rptTables.DataSource = tables;
                        rptTables.DataBind();
                    }
                    pnlErro.Visible = false;
                    rptTables.Visible = true;
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

        /// <summary>
        /// Carrega o combo de banco de dados, de acordo com as connection strings configuradas no serviço.
        /// </summary>
        /// <remarks>
        /// Carrega o combo de banco de dados, de acordo com as connection strings configuradas no serviço.
        /// </remarks>
        private void CarregaComboBancoDados()
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    List<String> listaBancoDados = client.ListarConnectionStrings().ToList();
                    if (listaBancoDados != null && listaBancoDados.Count > 0)
                    {
                        ddlBancoDados.DataSource = listaBancoDados;
                        ddlBancoDados.DataBind();
                    }

                    ddlBancoDados.Items.Insert(0, new ListItem() 
                    { 
                        Text = "Selecione...",
                        Value = "",
                        Selected = true
                    });

                    ddlBancoDados.Items.Insert(ddlBancoDados.Items.Count, new ListItem()
                    {
                        Text = "Informar String de Conexão",
                        Value = "CNX",
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
    }
}