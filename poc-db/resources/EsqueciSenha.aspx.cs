#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [05/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página que realiza a confirmação positiva do usuário antes do login
    /// </summary>
    public class EsqueciSenha : ApplicationPageBaseAnonima
    {
        #region Controles
        /// <summary>
        /// 
        /// </summary>
        protected SharePoint.RecuperarDados ctrlRecuperarDados;
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErroPrincipal;
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlBotaoErro;
        /// <summary>
        /// 
        /// </summary>
        protected QuadroAviso quadroAviso;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltUrl;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlRecuperarDados.EnviarClick += new SharePoint.RecuperarDados.EnviarClickHandle(ProximaPagina);
            ltUrl.Text = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/EsqueciSenha.aspx", SPUrlZone.Internet);
        }

        /// <summary>
        /// 
        /// </summary>
        void ProximaPagina(object sender, EventArgs e)
        {
            try
            {
                Int32 retorno = ctrlRecuperarDados.Validar();

                if (retorno != 0)
                {
                    if ((retorno >= 1048 && retorno <= 1049) || retorno == 1069)
                        // Erro proveniente da recuperação de dados.
                        this.ExibirErro("SharePoint.RecuperarDados", retorno);
                    else
                        this.ExibirErro("EntidadeServico.Consultar", retorno);
                }
                else
                {
                    Response.Redirect("RecuperarDadosConfirmacao.aspx");
                }
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            pnlBotaoErro.Visible = true;
            ctrlRecuperarDados.pnlPagina1.Visible = false;
        }
    }
}