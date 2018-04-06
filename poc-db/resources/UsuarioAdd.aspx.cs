using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using Redecard.PN.Comum;
using System.Data;
using System.ServiceModel;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    public partial class UsuarioAdd : SustentacaoApplicationPageBase
    {
        private String _nomeGrupoSustentacao = "Ferramentas Portal PN";
        private String _prefixoSustentacao = "operacional";

        private String NumPV
        {
            get { return Request.QueryString["numPV"] == null ? null : (String)Request.QueryString["numPV"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }
            lblExtensaoNomeUsuario.Text = _prefixoSustentacao;
        }

        protected void btnSalvarNovoUsuario_Click(object sender, EventArgs e)
        {
            AdicionarUsuario();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AdicionarUsuario()
        {
            using (SustentacaoAdministracaoServicoClient client = new SustentacaoAdministracaoServicoClient())
            {
                try
                {
                    if (txtSenhaUsuario.Text.Trim() != txtSenhaUsuarioConfirmacao.Text.Trim())
                    {
                        this.Alerta("As senhas devem coincidir!", true);
                        return;
                    }

                    String senhaUsuario = txtSenhaUsuario.Text.Trim();
                    senhaUsuario = EncriptadorSHA1.EncryptString(senhaUsuario);
                    String usuario = _prefixoSustentacao;
                    //sp_ins_usu_ent_pn 0, 1, 23068165, '', 'teste2', 'teste2', 'M', 'E0F68134D29DC326D115DE4C8FAB870A3C4B02'
                    String sql = string.Format("sp_ins_usu_ent_pn 0, 1, {0}, '', '{1}', '{2}', 'M', '{3}', '{4}'", this.NumPV, usuario, usuario, senhaUsuario, txtEmailUsuario.Text);
                    DataTable[] tables = client.ConsultarSql("SQLServerPN", sql);

                    if (!object.ReferenceEquals(tables, null) && tables.Length > 0)
                    {
                        if (tables[0].Rows.Count > 0)
                        {
                            if (tables[0].Rows[0][0].ToString() == "0")
                            {
                                this.Alerta("Usuário criado com sucesso", true);
                            }
                            else if (tables[0].Rows[0][0].ToString() == "1")
                            {
                                this.Alerta("ATENÇÃO: Não é possível criar usuários com o mesmo email (NOM_EML_USR)");
                            }
                            else if (tables[0].Rows[0][0].ToString() == "10")
                            {
                                this.Alerta("ATENÇÃO: Não é possível criar usuários com a mesma senha");
                            }
                            else
                            {
                                this.Alerta("ATENÇÃO: Ocorreu um erro ao criar o usuário");
                            }
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    this.Alerta(ex.Reason.ToString());
                }
                catch (Exception ex)
                {
                    this.Alerta(ex.Message);
                }
            }
        }

    }
}
