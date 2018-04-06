/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.OutrosServicos.Modelo.Corban
{
    /// <summary>
    /// Enumerador com os tipos de Forma de Pagamento Corban para filtro
    /// </summary>
    public enum FormaPagemento : short
    {
        /// <summary>
        /// Todos os tipos de pagamento
        /// </summary>
        Todos = 0,

        /// <summary>
        /// Pagamento em Crédito
        /// </summary>
        Credito = 1,

        /// <summary>
        /// Pagamento em Débito
        /// </summary>
        Debito = 2,

        /// <summary>
        /// Pagamento em Dinheiro
        /// </summary>
        Dinheiro = 3
    }
}
