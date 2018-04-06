using System;
using System.ComponentModel;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Model para cada item de validação
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class CustomRulesValidatorRule
    {
        /// <summary>
        /// Regex para validação
        /// </summary>
        public String RegexPattern { get; set; }

        /// <summary>
        /// Valor mínimo obrigatório
        /// </summary>
        public Double? MinimumValue { get; set; }

        /// <summary>
        /// Valor máximo obrigatório
        /// </summary>
        public Double? MaximumValue { get; set; }

        /// <summary>
        /// Mensagem de erro personalizada
        /// </summary>
        public String ErrorMessage { get; set; }
    }
}
