using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Microsoft.SharePoint;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Aberto.WebParts.ExibicaoDeSons
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data da criação: 15/10/2010
    /// Descrição: Player de exibição de sons
    /// </summary>
    public partial class ExibicaoDeSonsUserControl : UserControlBase
    {
        private static string UrlMediaPlayer = string.Format("{0}/{1}", SPContext.Current.Site.Url, "Style%20Library/redecard/media/swf/mediaplayer.swf");

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ExibicaoDeSons WebPart
        {
            get
            {
                return this.Parent as ExibicaoDeSons;
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
        /// Registra na variável auxiliar exibirInfoSom (javascript, arquivo core.js) o valor true ou false baseado na configuração da Web Part
        /// Registra o caminho do player interno que o jquery media utiliza para reprodução de conteúdo
        /// </summary>
        private void RegistrarScriptsInicializacao()
        {
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_exibicaoInfoSons", string.Format("exibirInfoSom = {0};", this.WebPart.ExibirInfoSom.ToString().ToLower()), true);

            //Indica o caminho do player interno que o jquery media utiliza para reproduzir os conteúdos (as variáveis $.fn.media.defaults.flvPlayer e $.fn.media.defaults.mp3Player se encontram dentro do arquivo jquery.media.js)
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_flvPlayer", string.Format("$.fn.media.defaults.flvPlayer = '{0}';", ExibicaoDeSonsUserControl.UrlMediaPlayer), true);
            this.Page.ClientScript.RegisterStartupScript(typeof(Page), "_mp3Player", string.Format("$.fn.media.defaults.mp3Player = '{0}';", ExibicaoDeSonsUserControl.UrlMediaPlayer), true);
        }
    }
}