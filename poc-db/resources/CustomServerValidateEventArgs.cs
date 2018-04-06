using System;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Provides data for the System.Web.UI.WebControls.CustomValidator.EmailServerValidateEventArgs
    /// event of the System.Web.UI.WebControls.CustomValidator control. This class cannot
    /// be inherited.
    /// </summary>
    public class CustomServerValidateEventArgs : ServerValidateEventArgs
    {
        /// <summary>
        /// Construtor do EventArgs
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isValid"></param>
        public CustomServerValidateEventArgs(string value, bool isValid)
            : base(value, isValid)
        {
        }

        /// <summary>
        /// Mensagem a ser exibida caso nao for Valido
        /// </summary>
        public String ErrorMessage { get; set; }

    }
}
