using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
    /// <summary>
    /// Classe que representa um item de informação no QuadroInformacao
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class QuadroInformacaoItem
    {
        /// <summary>
        /// Define o título a ser exibido
        /// </summary>
        [NotifyParentProperty(true)]
        public String Descricao { get; set; }

        /// <summary>
        /// Define o título a ser exibido
        /// </summary>
        [NotifyParentProperty(true)]
        public String Valor { get; set; }

    }
}
