using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Turquia - Equipamento 
    /// </summary>
    public class Equipamento
    {
        /// <summary>
        /// Tipo de Equipamento do pacote
        /// </summary>
        public String Tipo { get; set; }

        /// <summary>
        /// Quantidade de terminais do tipo.
        /// </summary>
        public Int16 QtdTerminais { get; set; }

        /// <summary>
        /// Valor da tecnologia.
        /// </summary>
        public Decimal Valor { get; set; }
    }
}
