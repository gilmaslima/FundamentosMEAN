/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;

namespace Redecard.PN.OutrasEntidades
{
    /// <summary>
    /// Enumerador para o tipo da mensagem de validação
    /// </summary>
    public enum TipoMensagemNovoAcesso
    {
        /// <summary>
        /// Sucesso
        /// </summary>
        [Description("check")]
        Sucesso,

        /// <summary>
        /// Erro
        /// </summary>
        [Description("erro")]
        Erro,

        /// <summary>
        /// Aviso
        /// </summary>
        [Description("info")]
        Aviso
    }
}