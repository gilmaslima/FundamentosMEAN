/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Enumerador para a forma de recuperação de senha
    /// </summary>
    public enum FormaRecuperacao
    {
        /// <summary>
        /// E-mail principal
        /// </summary>
        [Description("E-mail principal")]
        EmailPrincipal = 1,

        /// <summary>
        /// E-mail secundário
        /// </summary>
        [Description("E-mail secundário")]
        EmailSecundario = 2,

        /// <summary>
        /// SMS
        /// </summary>
        [Description("SMS")]
        Sms = 3
    }
}
