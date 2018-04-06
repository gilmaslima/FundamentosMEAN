#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [23/10/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.IdentityModel;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using System.Web.Security;
using System.Security;
using Microsoft.IdentityModel.Configuration;
using System.Xml;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Controle de login do Portal Redecard
    /// </summary>
    public partial class LoginFrame : UserControlBase
    {
        /// <summary>
        /// Carregamento da Página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(Object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregando página"))
            {
                try
                {
                    String url = String.Empty;
                    String urlEstab = String.Empty;

                    url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/Login.aspx", SPUrlZone.Default);
                    urlEstab = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/LoginSelecionaEstabelecimento.aspx", SPUrlZone.Default);
                    ltLoginUrl.Text = url;
                    ltLoginUrlEstab.Text = urlEstab;

                    //if (!Page.IsPostBack)
                    //    ListarGrupoEntidades();
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErroTela(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErroTela(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir painel de erro
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        protected void ExibirErroTela(String fonte, Int32 codigo)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            pnlServices.Visible = false;
            pnlErro.Visible = true;
            ltErro.Text = mensagem;
        }

        /// <summary>
        /// Consulta o cache do site para retornar a lista de grupos entidades
        /// </summary>
        /// <returns>Listagem dos grupos de entidades ativos</returns>
        protected EntidadeServico.GrupoEntidade[] ObterGrupoEntidadesCache()
        {
            using (Logger Log = Logger.IniciarLog("Consulta grupo de entidades em cache"))
            {
                Int32 codigoRetorno;
                using (EntidadeServicoClient client = new EntidadeServicoClient())
                {
                    return client.ConsultarGrupo(out codigoRetorno, null);
                }
            }
        }

        /// <summary>
        /// Carrega os grupos de entidade no combo box de seleção
        /// </summary>
        protected void ListarGrupoEntidades()
        {
            EntidadeServico.GrupoEntidade[] grupoEntidades = this.ObterGrupoEntidadesCache();

            if (grupoEntidades.Length > 0)
            {
                ddlYouAre.DataSource = grupoEntidades;
                ddlYouAre.DataTextField = "Descricao";
                ddlYouAre.DataValueField = "Codigo";
                ddlYouAre.DataBind();
            }
        }
    }
}
