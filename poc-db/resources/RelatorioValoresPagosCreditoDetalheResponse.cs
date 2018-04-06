/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Extrato.Servicos.ValoresPagos;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{

    /// <summary>
    /// Response do relatório de valores pagos - detalhe crédito
    /// </summary>
    [DataContract]
    public class RelatorioValoresPagosCreditoDetalheResponse : RelatorioResponseBase
    {
        /// <summary>
        /// Totalizador do relátorio
        /// </summary>
        [DataMember(Name = "totalizador")]
        public CreditoDetalheTotalizador Totalizador { get; set; }
        /// <summary>
        /// dados do relatorio
        /// </summary>
        [DataMember(Name = "registros")]
        public CreditoDetalhe[] Registros { get; set; }

    }
}