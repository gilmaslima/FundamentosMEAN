/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Vendas
{
    /// <summary>
    /// Enumeração auxiliar utilizada para customizar o retorno dos registros dos serviços
    /// de consulta do relatório de Vendas - Débito.
    /// </summary>
    /// <remarks>
    /// Relatório de Vendas - Débito - WACA1313 / WA1313 / ISHD
    /// </remarks>
    [DataContract]
    public enum DebitoTipoRegistro
    {
        /// <summary>Todos os registros</summary>
        [EnumMember]
        Todos = 0,
        /// <summary>Registros do tipo DT</summary>
        [EnumMember, Description("DT")]
        Detalhe,
        /// <summary>Registros do tipo A1</summary>
        [EnumMember, Description("A1")]
        AjusteComValor = 2,
        /// <summary>Registros do tipo A2</summary>
        [EnumMember, Description("A2")]
        AjusteSemValor = 3
    }
}