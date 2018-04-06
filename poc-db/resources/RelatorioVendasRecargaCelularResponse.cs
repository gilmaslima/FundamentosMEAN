/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Extrato.Servicos.Vendas;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Response do relatório de vendas - recarga celular
    /// </summary>
    [DataContract]
    public class RelatorioVendasRecargaCelularResponse : RelatorioResponseBase
    {
        /// <summary>
        /// Totalizador do relátorio
        /// </summary>
        [DataMember(Name = "totalizador")]
        public RecargaCelularTotalizador Totalizador { get; set; }
        /// <summary>
        /// dados do relatorio
        /// </summary>
        [DataMember(Name = "registros")]
        public RecargaCelular[] Registros { get; set; }
    }
}