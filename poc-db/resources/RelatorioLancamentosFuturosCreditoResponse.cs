/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Extrato.Servicos.LancamentosFuturos;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Response do relatório de lancamentos futuros - credito
    /// </summary>
    [DataContract]
    public class RelatorioLancamentosFuturosCreditoResponse : RelatorioResponseBase
    {
        /// <summary>
        /// Totalizador do relátorio
        /// </summary>
        [DataMember(Name = "totalizador")]
        public CreditoTotalizador Totalizador { get; set; }
        /// <summary>
        /// dados do relatorio
        /// </summary>
        [DataMember(Name = "registros")]
        public Credito[] Registros { get; set; }
    }
}