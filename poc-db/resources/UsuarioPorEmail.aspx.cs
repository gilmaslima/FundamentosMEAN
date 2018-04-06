using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.Sustentacao.AdministracaoDados;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Sustentacao.SharePoint.Layouts.sustentacao
{
    public partial class UsuarioPorEmail : SustentacaoApplicationPageBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarComando_Click(Object sender, EventArgs e)
        {
            String email = String.Empty;
            Int32 pv = 0;
            Int32 grupo = 0;

            if (rdbEmail.Checked)
            {
                if (txtEmail.Text == String.Empty || txtEmail.Text == null)
                {
                    ltAviso.Text = "ATENÇÃO: É necessário preencher o email para realizar a consulta.";
                    pnlAviso.Visible = true;
                    return;
                }
                else
                {
                    email = txtEmail.Text;
                    pv = 0;
                    grupo = 0;
                }
            }

            if (rdbPV.Checked)
            {
                if (txtNumPV.Text == String.Empty || txtNumPV.Text == null)
                {
                    ltAviso.Text = "ATENÇÃO: É necessário preencher o PV para realizar a consulta.";
                    pnlAviso.Visible = true;
                    return;
                }
                else
                {
                    email = String.Empty;
                    pv = StringToInt(txtNumPV.Text);
                    grupo = Convert.ToInt32(ddlTipoEstabelecimento.SelectedValue);
                }
            }

            pnlAviso.Visible = false;
            ConsultarUsuariosPorEmail(email, pv, grupo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDesbloquear_Click(Object sender, EventArgs e)
        {
            pnlAviso.Visible = false;
            for (int i = 0; i < gdvUsuarios.Rows.Count; i++)
            {
                CheckBox chkDesbloquear = new CheckBox();
                chkDesbloquear = (CheckBox)gdvUsuarios.Rows[i].FindControl("chkItem");

                if (chkDesbloquear.Checked)
                {
                    IOrderedDictionary rowGrid = GetValues(gdvUsuarios.Rows[i]);
                    DesbloquearUsuario(Convert.ToInt32(rowGrid["ID"]), Convert.ToInt32(rowGrid["CodigoPV"]), Convert.ToString(rowGrid["Email"]));
                }
            }

            if (rdbEmail.Checked)
            {
                ConsultarUsuariosPorEmail(txtEmail.Text, 0, 0);
            }

            if (rdbPV.Checked)
            {
                ConsultarUsuariosPorEmail(String.Empty, StringToInt(txtNumPV.Text), StringToInt(ddlTipoEstabelecimento.SelectedValue));
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "FecharAguarde", "FechaAguarde()");
        }

        private Int32 StringToInt(String pv)
        {
            Int32 retorno = 0;

            try
            {
                retorno = Convert.ToInt32(pv);
            }
            catch (FormatException)
            {
                pnlErro.Visible = true;
                pnlGrid.Visible = false;
                ltErro.Text = "Digite um número válido!";
            }
            catch (Exception ex)
            {
                pnlErro.Visible = true;
                pnlGrid.Visible = false;
                ltErro.Text = ex.Message;
            }

            return retorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcluir_Click(Object sender, EventArgs e)
        {
            pnlAviso.Visible = false;
            if (rdbEmail.Checked)
            {
                for (int i = 0; i < gdvUsuarios.Rows.Count; i++)
                {
                    CheckBox chkDesbloquear = new CheckBox();
                    chkDesbloquear = (CheckBox)gdvUsuarios.Rows[i].FindControl("chkItem");

                    if (chkDesbloquear.Checked)
                    {
                        IOrderedDictionary rowGrid = GetValues(gdvUsuarios.Rows[i]);
                        ExcluirUsuario(Convert.ToInt32(rowGrid["ID"]), Convert.ToString(rowGrid["Email"]));
                    }
                }
            }

            if (rdbPV.Checked)
            {
                for (int i = 0; i < gdvUsuarios.Rows.Count; i++)
                {
                    CheckBox chkDesbloquear = new CheckBox();
                    chkDesbloquear = (CheckBox)gdvUsuarios.Rows[i].FindControl("chkItem");

                    if (chkDesbloquear.Checked)
                    {
                        IOrderedDictionary rowGrid = GetValues(gdvUsuarios.Rows[i]);
                        ExcluirRelacionamento(Convert.ToInt32(rowGrid["ID"]), Convert.ToInt32(rowGrid["CodigoPV"]), Convert.ToInt32(ddlTipoEstabelecimento.SelectedValue), Convert.ToString(rowGrid["Email"]));
                    }
                }
            }

            if (rdbEmail.Checked)
            {
                ConsultarUsuariosPorEmail(txtEmail.Text, 0, 0);
            }

            if (rdbPV.Checked)
            {
                ConsultarUsuariosPorEmail(String.Empty, StringToInt(txtNumPV.Text), StringToInt(ddlTipoEstabelecimento.SelectedValue));
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "FecharAguarde", "FechaAguarde()");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rdb_CheckedChanged(Object sender, EventArgs e)
        {
            if (rdbEmail.Checked)
            {
                pnlGrid.Visible = false;
                pnlAviso.Visible = false;
                pnlErro.Visible = false;
                pnlConsultaPorEmail.Visible = true;
                pnlConsultaPorPV.Visible = false;
                txtNumPV.Text = String.Empty;
            }

            if (rdbPV.Checked)
            {
                pnlGrid.Visible = false;
                pnlAviso.Visible = false;
                pnlErro.Visible = false;
                pnlConsultaPorEmail.Visible = false;
                pnlConsultaPorPV.Visible = true;
                txtEmail.Text = String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridViewRow"></param>
        /// <returns></returns>
        private IOrderedDictionary GetValues(GridViewRow gridViewRow)
        {
            IOrderedDictionary values = new OrderedDictionary();

            foreach (DataControlFieldCell cell in gridViewRow.Cells)
            {
                if (cell.Visible)
                {
                    cell.ContainingField.ExtractValuesFromCell(values, cell, gridViewRow.RowState, true);
                }
            }
            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConsultarUsuariosPorEmail(String email, Int32 pv, Int32 grupo)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    UsuarioPV[] listaUsuarios = client.ConsultarUsuariosPorEmail(email, pv, grupo);

                    gdvUsuarios.DataSource = listaUsuarios;
                    gdvUsuarios.DataBind();

                    pnlErro.Visible = false;
                    pnlGrid.Visible = true;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    pnlErro.Visible = true;
                    pnlGrid.Visible = false;
                    ltErro.Text = ex.Reason.ToString();
                }
                catch (Exception ex)
                {
                    pnlErro.Visible = true;
                    pnlGrid.Visible = false;
                    ltErro.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DesbloquearUsuario(Int32 codUsrId, Int32 codEntidade, String nomEmlUsr)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    client.DesbloquearUsuario(codUsrId, codEntidade, this.UsuarioLogado, nomEmlUsr);
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
        /// <param name="codigoUsuario"></param>
        /// <param name="pv"></param>
        protected void ExcluirUsuario(Int32 codigoUsuario, String nomEmlUsr)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    client.ExcluirUsuario(codigoUsuario, this.UsuarioLogado, nomEmlUsr);
                    Historico.ExclusaoUsuario(0, "", "", "", 0, this.UsuarioLogado, codigoUsuario, "", nomEmlUsr, "");
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

        private void ExcluirRelacionamento(Int32 codigoUsuario, Int32 codigoPV, Int32 grupo, String nomEmlUsr)
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    client.ExcluirRelEtd(codigoUsuario, codigoPV, grupo, this.UsuarioLogado, nomEmlUsr);
                    Historico.ExclusaoUsuario(0, "", "", "", codigoPV, this.UsuarioLogado, codigoUsuario, "", nomEmlUsr, "");
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
    }
}
