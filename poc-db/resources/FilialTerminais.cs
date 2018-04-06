using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Filial Terminais
    /// </summary>
    public class FilialTerminais
    {
        /// <summary>
        /// Filial
        /// </summary>
        public Filial Filial { get; set; }

        /// <summary>
        /// Terminais da Filial
        /// </summary>
        public List<TerminalContaCerta> Terminais { get; set; }
    }
}
