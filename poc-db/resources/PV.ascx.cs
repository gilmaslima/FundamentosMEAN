#region Histórico do Arquivo
/*
(c) Copyright [2015] Redecard S.A.
Autor       : [Rodrigo Rodrigues]
Empresa     : [Iteris]
Histórico   :
    [30/10/2015] – [Rodrigo Rodrigues] – [Criação]
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
using System.Security;
using Microsoft.SharePoint.Utilities;
using System.Collections;
using Microsoft.SharePoint.Navigation;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;

namespace Redecard.PN.Sustentacao.SharePoint
{
    public partial class PV : UserControlBase
    {
        private string _nomeGrupoSustentacao = "Ferramentas Portal PN";

        private DataTable DtPV
        {
            get { return ViewState["DtPV"] == null ? null : (DataTable)ViewState["DtPV"]; }
            set { ViewState["DtPV"] = value; }
        }

        private DataTable TbUsuarios
        {
            get { return ViewState["TbUsuarios"] == null ? null : (DataTable)ViewState["TbUsuarios"]; }
            set { ViewState["TbUsuarios"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected bool ValidarChaveAcesso()
        {
            
            bool possuiChaveAcesso = false;
            try
            {
                if (Request.QueryString["n"] != null)
                {
                    possuiChaveAcesso = Request.QueryString["n"].ToString() ==
                        _nomeGrupoSustentacao + "-" + DateTime.Today.Year.ToString("0000") + DateTime.Today.Month.ToString("00");
                }

            }
            catch (Exception ex)
            {
                SPUtility.TransferToErrorPage(ex.Message);
            }
            return possuiChaveAcesso;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarComando_Click(object sender, EventArgs e)
        {
            pnlAviso.Visible = false;
            btnAdicionarUsuario.Visible = false;
            ConsultarPV();
        }
        /// <summary>
        /// 
        /// </summary>
        protected void ConsultarPV()
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    string sql = string.Format("sp_cons_entidade {0}, {1}", txtNumPV.Text, ddlTipoEstabelecimento.SelectedValue);
                    DataTable[] tables = client.ConsultarSql("SQLServerPN", sql);
                    DataTable[] tables2 = ConsultarUsuario(null, txtNumPV.Text, ddlTipoEstabelecimento.SelectedValue);

                    int array1OriginalLength = tables.Length;
                    Array.Resize<DataTable>(ref tables, array1OriginalLength + tables2.Length);
                    Array.Copy(tables2, 0, tables, array1OriginalLength, tables2.Length);

                    if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                    {
                        tables[0].TableName = ddlTipoEstabelecimento.SelectedItem.Text;
                        this.DtPV = tables[0];
                        tables[1].TableName = "Usuários";
                        this.TbUsuarios = tables[1];

                        gdvPV.DataSource = this.DtPV;
                        gdvPV.DataBind();
                        gdvUsuarios.DataSource = this.TbUsuarios;
                        gdvUsuarios.DataBind();
                    }
                    pnlErro.Visible = false;
                    pnlGrid.Visible = true;
                    btnAdicionarUsuario.Visible = true;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    pnlGrid.Visible = false;
                    ltErro.Text = ex.Reason.ToString();
                    btnAdicionarUsuario.Visible = false;
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    pnlGrid.Visible = false;
                    ltErro.Text = ex.Message;
                    btnAdicionarUsuario.Visible = false;
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
        }

        protected void lnbExcluiUsuario_Click(object sender, CommandEventArgs e)
        {
            string pv = txtNumPV.Text;
            string codigoUsuario = (string)e.CommandArgument;

            ExcluirUsuario(codigoUsuario, pv, ddlTipoEstabelecimento.SelectedValue);
        }

        protected void lnbDesbloqueiaUsuario_Click(object sender, CommandEventArgs e)
        {
            string email = (string)e.CommandArgument;
            DesbloquearUsuario(email);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DesbloquearUsuario(string email)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    if (email == "")
                    {
                        ltAviso.Text = "ATENÇÃO: Não foi possível desbloquear o usuário porque seu email é nulo (NOM_EML_USR)";
                        pnlAviso.Visible = true;
                        return;
                    }
                    string sql = string.Format("sp_alt_usr_desbloq_all_pn '{0}'", email);
                    DataTable[] tables = client.ConsultarSql("SQLServerPN", sql);

                    if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                    {
                        if (tables[0].Rows.Count > 0)
                        {
                            if (tables[0].Rows[0].ToString() == "99")
                            {
                                ltAviso.Text = "Ocorreu um erro ao desbloquear o usuário";
                            }
                            else
                            {
                                //ConsultarUsuario(null, txtNumPV.Text);
                                ltAviso.Text = "Usuário desbloqueado com sucesso";
                            }
                            pnlAviso.Visible = true;
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExcluirUsuario(string codigoUsuario, string pv, String codGruEtd)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    //sp_exc_usuario 'rro', 1250191, 1, 'IS'
                    string sql = string.Format("sp_exc_usuario '{0}', '{1}', {2}, 'IS'", codigoUsuario, pv, codGruEtd);
                    DataTable[] tables = client.ConsultarSql("SQLServerPN", sql);

                    if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                    {
                        if (tables[0].Rows.Count > 0)
                        {
                            if (tables[0].Rows[0][0].ToString() == "0")
                            {
                                ConsultarPV();
                                ltAviso.Text = "Usuário excluído com sucesso";
                            }
                            else if (tables[0].Rows[0][0].ToString() == "2")
                            {
                                ltAviso.Text = "ATENÇÃO: Usuário não encontrado";
                            }
                            else
                            {
                                ltAviso.Text = "ATENÇÃO: Ocorreu um erro ao excluir o usuário";
                            }
                            pnlAviso.Visible = true;
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Message;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected DataTable[] ConsultarUsuario(string codigoUsuario, string pv, String codGruEtd)
        {
            DataTable[] tables = null;
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    if (codigoUsuario == null)
                    {
                        codigoUsuario = "null";
                    }
                    string sql = string.Format("sp_cons_usuario_pn {0}, {1}, {2}", codigoUsuario, pv, codGruEtd);
                    tables = client.ConsultarSql("SQLServerPN", sql);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    ltErro.Text = ex.Message;
                }
                return tables;
            }
        }

        protected void gdvUsuarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow row = ((DataRowView)e.Row.DataItem).Row;
                DataTable dt = row.Table.Clone();
                dt.ImportRow(row);
                GridView dg = e.Row.FindControl("gdvUsuariosDetalhes") as GridView;
                if (!object.ReferenceEquals(dt, null) && !object.ReferenceEquals(dg, null))
                {
                    dg.DataSource = dt;
                    dg.DataBind();
                }
            }
        }

    }
}
