using Redecard.PN.Extrato.Servicos.AntecipacaoRAV;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// Response do relatório de Antecipados
    /// </summary>
    [DataContract]
    public class RelatorioAntecipadosResponse : RelatorioResponseBase
    {
        /// <summary>
        /// Totalizador do relátorio
        /// </summary>
        [DataMember(Name = "totalizador")]
        public RAVTotalizador Totalizador { get; set; }
        /// <summary>
        /// dados do relatorio
        /// </summary>
        [DataMember(Name = "registros")]
        public RAV[] Registros { get; set; }
    }
}