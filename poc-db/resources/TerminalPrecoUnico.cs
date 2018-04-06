using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de PrecoUnico.
    /// </summary>
    public class TerminalPrecoUnico
    {
        /// <summary>
        /// Define um ValorFaturamentoContrato.
        /// </summary>
        public Decimal ValorFaturamentoContrato { get; set; }

        /// <summary>
        /// Define uma QuantidadeEquipamento.
        /// </summary>
        public String QuantidadeEquipamento { get; set; }

        /// <summary>
        /// Define um TipoEquipamento.
        /// </summary>
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Define um ValorPrecoUnicoSemFlex.
        /// </summary>
        public Decimal ValorPrecoUnicoSemFlex { get; set; }

        /// <summary>
        /// Define um ValorPrecoUnicoComFlexA.
        /// </summary>
        public Decimal ValorPrecoUnicoComFlex { get; set; }
    }
}
