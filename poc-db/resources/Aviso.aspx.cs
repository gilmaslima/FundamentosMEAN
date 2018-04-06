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
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using ComumControls = Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página que exibe o quadro de aviso conforma parâmetros enviados para a página
    /// </summary>
    public partial class Aviso : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// Exibe quadro de aviso personalizado conforma parâmetros passados
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                String dados = Request["dados"];

                if (String.IsNullOrEmpty(dados))
                {
                    this.CarregarMensagem("Atenção", "Sistema Indiposnível(-1)", PainelMensagemIcon.Erro, voltarPadrao: true);
                }
                else
                {
                    QueryStringSegura query = new QueryStringSegura(dados);

                    String titulo = Server.UrlDecode(query["titulo"]);
                    String mensagem = Server.UrlDecode(query["mensagem"]);
                    Int16 imagem = query["imagem"].ToInt16();
                    ComumControls.QuadroAviso.IconeMensagem icon = (ComumControls.QuadroAviso.IconeMensagem)imagem;

                    //Verifica se a mensagem possui o código de erro 1096: Usuário com acesso bloqueado por mais de 90 dias
                    if (mensagem.Contains("1096"))
                    {
                        Int32 iCodigoGrupoEntidade = query["codigoGrupoEntidade"].ToString().ToInt32();
                        Int32 iCodigoEntidade = query["codigoEntidade"].ToString().ToInt32();
                        String codigoNomeUsuario = query["codigoNomeUsuario"].ToString();

                        // Exibir tela de confirmação positiva
                        // Gravar dados do PV na sessão de confirmação positiva
                        InformacaoUsuario.Criar(iCodigoGrupoEntidade, iCodigoEntidade, codigoNomeUsuario, "M");

                        if (InformacaoUsuario.Existe())
                            InformacaoUsuario.Recuperar().SenhaExpirada = true;

                        this.CarregarMensagem(titulo, mensagem, ToPainelMensagemIcon(icon), voltarHome: true, confirmacaoPositiva: true);
                    }
                    else
                    {
                        this.CarregarMensagem(titulo, mensagem, ToPainelMensagemIcon(icon), voltarHome: true);
                    }
                }
            }
        }

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Carrega os dados no painel de mensagem
        /// </summary>
        /// <param name="titulo">Título da mensagem (ex: atenção)</param>
        /// <param name="mensagem">Descritivo da mensagem</param>
        /// <param name="icone">Ícone identificador (aviso, erro, sucesso)</param>
        /// <param name="voltarPadrao">Opcional: exibe o botão de voltar default [ex: history.back()]</param>
        /// <param name="voltarHome">Optional: exibe o botão de voltar para a home page</param>
        /// <param name="confirmacaoPositiva">Opcional: exibe o botão de prosseguir para a confirmação positiva</param>
        private void CarregarMensagem(
            string titulo, 
            string mensagem, 
            PainelMensagemIcon icone, 
            bool voltarPadrao = false,
            bool voltarHome = false,
            bool confirmacaoPositiva = false)
        {
            // definição da mensagem
            this.pnMensagem.Titulo = titulo;
            this.pnMensagem.Mensagem = mensagem;
            this.pnMensagem.TipoMensagem = icone;

            // visibilidade dos containers dos botões
            this.pnlVoltar.Visible = voltarPadrao || voltarHome;
            this.pnlConfirmacaoPositiva.Visible = confirmacaoPositiva;

            // evento de click dos botões
            if (voltarHome)
            {
                String url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
                this.btnVoltar.OnClientClick = String.Format("window.location.href = '{0}'; return false;", url);
            }
            else if (voltarPadrao)
            {
                this.btnVoltar.OnClientClick = "window.history.back(); return false;";
            }

            // aparência dos botões
            if (this.pnlVoltar.Visible && this.pnlConfirmacaoPositiva.Visible)
                this.btnVoltar.BotaoPrimario = false;
        }

        /// <summary>
        /// Converte o enumerator de icon do QuadroAviso para o do PainelMensagem
        /// </summary>
        /// <param name="legacyIcon">Tipo/enumerator de icon do QuadroAviso</param>
        /// <returns></returns>
        private PainelMensagemIcon ToPainelMensagemIcon(ComumControls.QuadroAviso.IconeMensagem legacyIcon)
        {
            switch (legacyIcon)
            {
                case ComumControls.QuadroAviso.IconeMensagem.Confirmacao:
                    return PainelMensagemIcon.Sucesso;
                case ComumControls.QuadroAviso.IconeMensagem.Erro:
                    return PainelMensagemIcon.Erro;
                case ComumControls.QuadroAviso.IconeMensagem.Aviso:
                default:
                    return PainelMensagemIcon.Aviso;
            }
        }

        #endregion
    }
}