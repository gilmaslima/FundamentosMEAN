using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Helper;
using System;

namespace Redecard.Portal.Aberto.WebParts.RedecardFlashViewer {

    /// <summary>
    /// Apresenta conteúdo de mídia no portal da Redecard, utilizada para contornar um tratamento do editor de
    /// conteúdo padrão do SharePoint, quando tentamos renderizar uma tag OBJECT no editor, ele faz a alteração de alguns
    /// atributod, impossibilitando a exibição da mídia.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class RedecardFlashViewer : WebPart {

        /// <summary>
        /// Armazena o endereço do conteúdo flash
        /// </summary>
        private string _midiaFlashUrl = string.Empty;

        /// <summary>
        /// Armazena as variáveis do flash
        /// </summary>
        private string _midiaFlashVars = string.Empty;


        /// <summary>
        /// 
        /// </summary>
        private string _containerDivClass = string.Empty;

        /// <summary>
        /// Armazena o tamanho horizontal (Width) do objeto
        /// </summary>
        private string _horizontalLength = "100";

        /// <summary>
        /// Armazena o tamanho vertical (Heigth) do objeto
        /// </summary>
        private string _verticalLength = "100";

        /// <summary>
        /// Endereço do arquivo de mídia Macromedia Flash
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Endereço do arquivo de mídia")]
        [Personalizable(PersonalizationScope.Shared)]
        public string FlashUrl {
            get {
                return _midiaFlashUrl;
            }
            set {
                _midiaFlashUrl = value;
            }
        }

        /// <summary>
        /// Variáveis do arquivo de mídia Macromedia Flash
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Variáveis do arquivo de mídia")]
        [Personalizable(PersonalizationScope.Shared)]
        public string FlashVars {
            get {
                return _midiaFlashVars;
            }
            set {
                _midiaFlashVars = value;
            }
        }

        /// <summary>
        /// Nome da classe que será aplicado na DIV que contém o controle de mídia
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Nome da classe do container")]
        [Personalizable(PersonalizationScope.Shared)]
        public string ContainerDivClassName {
            get {
                return _containerDivClass;
            }
            set {
                _containerDivClass = value;
            }
        }

        /// <summary>
        /// Tamanho horizontal do objeto de mídia
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Tamanho Horizontal")]
        [Personalizable(PersonalizationScope.Shared)]
        public string HorizontalLength {
            get {
                return _horizontalLength;
            }
            set {
                _horizontalLength = value;
            }
        }

        /// <summary>
        /// Tamanho vertical do objeto de mídia
        /// </summary>
        [WebBrowsable(true)]
        [Category(RedecardHelper._webPartsPropertiesConfigCategory)]
        [WebDisplayName("Tamanho Vertical")]
        [Personalizable(PersonalizationScope.Shared)]
        public string VerticalLength {
            get {
                return _verticalLength;
            }
            set {
                _verticalLength = value;
            }
        }

        //Exemplo de Renderização da Web Part
        //-------------------------------------------------------------------------------------------------------
        //<object type="application/x-shockwave-flash" data="flash_flags.swf" width="300" height="300"> 
        //    <param name="movie" value="flash_flags.swf" /> 
        //    <param name="wmode" value="transparent" /> 
        //    <param name="menu" value="false" /> 
        //    <param name="flashvars" value="urlXml=util/xml/tvflash.xml"/>
        //</object>

        /// <summary>
        /// Identifica se o DIV container será renderizado
        /// </summary>
        /// <returns></returns>
        private bool RenderContainer() {
            return !String.IsNullOrEmpty(_containerDivClass);
        }

        /// <summary>
        /// Renderização da TAG para exibição da mídia de flash.
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer) {
            if (!String.IsNullOrEmpty(this.FlashUrl)) {
                if (this.RenderContainer()) {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, this.ContainerDivClassName);
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "application/x-shockwave-flash");
                writer.AddAttribute(HtmlTextWriterAttribute.Width, this.HorizontalLength);
                writer.AddAttribute(HtmlTextWriterAttribute.Height, this.VerticalLength);
                writer.AddAttribute("data", this.FlashUrl);
                writer.RenderBeginTag(HtmlTextWriterTag.Object);
                // renderização dos paramêtros
                this.RenderParam(writer, "movie", this.FlashUrl);
                this.RenderParam(writer, "wmode", "transparent");
                this.RenderParam(writer, "menu", "false");
                this.RenderParam(writer, "flashvars", this.FlashVars);
            }
        }

        /// <summary>
        /// Renderiza uma tag de param para o objeto mídia
        /// </summary>
        /// <param name="writer"></param>
        private void RenderParam(HtmlTextWriter writer, string name, string value) {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
            writer.AddAttribute(HtmlTextWriterAttribute.Value, value);
            writer.RenderBeginTag(HtmlTextWriterTag.Param);
            writer.RenderEndTag();
        }

        /// <summary>
        /// Finaliza a renderização do objeto de mídia
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer) {
            if (!String.IsNullOrEmpty(this.FlashUrl)) {
                if (this.RenderContainer())
                    writer.RenderEndTag();
                writer.RenderEndTag();
            }
        }
    }
}