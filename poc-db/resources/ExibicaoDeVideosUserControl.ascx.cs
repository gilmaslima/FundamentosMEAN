using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace Redecard.Portal.Aberto.WebParts.ExibicaoDeVideos
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 21/10/2010
    /// Descrição: Player de exibição de Videos
    /// </summary>
    public partial class ExibicaoDeVideosUserControl : UserControl
    {
        private static string UrlMediaPlayer = string.Format("{0}/{1}", SPContext.Current.Site.Url, "Style%20Library/redecard/media/swf/yt.swf");

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ExibicaoDeVideos WebPart
        {
            get
            {
                return this.Parent as ExibicaoDeVideos;
            }
        }

        /// <summary>
        /// Carregamento da WebPart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.RegistrarScriptsInicializacao();

            base.OnLoad(e);
        }

        /// <summary>
        /// Registra na variável auxiliar exibirInfoVideo (javascript, arquivo core.js) o valor true ou false baseado na configuração da Web Part
        /// Registra o caminho do player interno que o jquery media utiliza para reprodução de conteúdo
        /// </summary>
        private void RegistrarScriptsInicializacao()
        {
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_exibicaoInfoVideos", string.Format("exibirInfoVideo = {0};", this.WebPart.ExibirInfoVideo.ToString().ToLower()), true);

            //Indica o caminho do player interno que o jquery media utiliza para reproduzir os conteúdos (as variáveis $.fn.media.defaults.flvPlayer e $.fn.media.defaults.mp3Player se encontram dentro do arquivo jquery.media.js)
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_flvPlayer", string.Format("$.fn.media.defaults.flvPlayer = '{0}';", ExibicaoDeVideosUserControl.UrlMediaPlayer), true);
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_mp3Player", string.Format("$.fn.media.defaults.mp3Player = '{0}';", ExibicaoDeVideosUserControl.UrlMediaPlayer), true);
        }
    }
}
