using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    public class TerminalContaCerta
    {
        /// <summary>
        /// Terminal Bancário
        /// </summary>
        public TerminalBancario TerminalBancario { get; set; }

        /// <summary>
        /// Indicador Conta Certa
        /// </summary>
        public Boolean IndicadorContaCerta { get; set; }

        /// <summary>
        /// Indicador Flex
        /// </summary>
        public Boolean IndicadorFlex { get; set; }
    }
}
