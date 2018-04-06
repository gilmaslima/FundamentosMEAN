using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

using System.Web;
using Microsoft.SharePoint.Utilities;

using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DataCash.SharePoint.DataCashService;
using System.Text;
using System.ServiceModel;

namespace Redecard.PN.DataCash.SharePoint.WebParts.UsuariosEcommerce
{
    public partial class UsuariosEcommerceUserControl : UserControlBaseDataCash
    {
        #region [ Atributos / Propriedades / Variáveis ]

        /// <summary>
        /// Senha do usuário guardada no ViewState, aguardando Confirmação
        /// </summary>
        private String Senha
        {
            get
            {
                if (object.ReferenceEquals(ViewState["SenhaUsuario"], null))
                    this.Senha = String.Empty;
                return (String)ViewState["SenhaUsuario"];
            }
            set
            {
                ViewState["SenhaUsuario"] = value;
            }
        }

        /// <summary>
        /// Sucesso na efetivação da Troca de Senha
        /// </summary>
        public Boolean SucessoTrocaSenha
        {
            get { return (Boolean) (ViewState["SucessoTrocaSenha"] ?? (ViewState["SucessoTrocaSenha"] = false)); }
            set { ViewState["SucessoTrocaSenha"] = value; }
        }
       
        #endregion

        #region [ Eventos da Página ]
        
        /// <summary>
        /// Page Load
        /// </summary>        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = btnContinuar.UniqueID;
            
            if (!Page.IsPostBack)
            {
                using (Logger Log = Logger.IniciarLog("Inicializando a página"))
                {
                    try
                    {
                        CarregarEdicao();
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento Botão de Continuação
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Continua alteração/gravação do Usuário"))
            {
                try
                {                                                                                
                    this.Senha = txtSenha.Text;
                    this.SucessoTrocaSenha = Salvar();

                    //Em caso de sucesso, ativa a View referente ao quadro de aviso de sucesso
                    if (this.SucessoTrocaSenha)
                        mvwEdicaoSenha.SetActiveView(vwEfetivacao);                                      
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento Botão Voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Voltar passo anterior Usuário"))
            {
                try
                {
                    String url = SPUtility.GetPageUrlPath(HttpContext.Current);
                    Response.Redirect(url, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Carrega dados para edição de usuário
        /// </summary>
        private void CarregarEdicao()
        {
            using (Logger Log = Logger.IniciarLog("Carregando Edição do Usuário")) 
            {
                try
                {
                    (qdAviso as QuadroAviso).CarregarMensagem();
                    txtSenha.Text = String.Empty;
                    txtConfirmacaoSenha.Text = String.Empty;                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
                
        /// <summary>
        /// Atualiza a senha do usuário
        /// </summary>
        private Boolean Salvar()
        {
            using (Logger Log = Logger.IniciarLog("Confirmação de alteração/cadastro de Usuário"))
            {
                try
                {
                    Boolean retorno = true;
                    MensagemErro mensagemErro;                    
                    using (var contexto = new ContextoWCF<DataCashServiceClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { base.SessaoAtual.CodigoEntidade });
                        retorno = contexto.Cliente.TrocarSenha(out mensagemErro, base.SessaoAtual.CodigoEntidade, this.Senha);
                        Log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
                    }
                    
                    if (mensagemErro != null && mensagemErro.CodigoRetorno != 1)
                    {
                        base.ExibirPainelExcecao("DataCashService.TrocarSenha", mensagemErro.CodigoRetorno);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        #endregion
    }
}