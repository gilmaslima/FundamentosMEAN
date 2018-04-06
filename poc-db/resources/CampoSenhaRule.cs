using System;
using System.ComponentModel;

namespace Redecard.PNCadastrais.Core.Web.Controles.Portal
{
    /// <summary>
    /// Model para cada item de validação
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class CampoSenhaRule
    {
        /// <summary>
        /// Descrição da validação
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Mensagem de erro personalizada
        /// </summary>
        public String ErrorMessage { get; set; }

        /// <summary>
        /// Regex para validação
        /// </summary>
        public String RegexPattern { get; set; }

        /// <summary>
        /// Define se a descrição será oculta na listagem: validação implícita
        /// </summary>
        public Boolean HideOnList { get; set; }

        /// <summary>
        /// Define a CSS Class para o item da lista com erro
        /// </summary>
        public String ErrorCssClass { get; set; }
    }
}
