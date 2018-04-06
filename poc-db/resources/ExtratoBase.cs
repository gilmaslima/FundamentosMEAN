using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Modelo
{
    public class ExtratoBase
    {
        /// <summary>
        /// Identificação do relatório.
        /// </summary>
        public String Identificacao { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Data Inicial.
        /// </summary>
        public DateTime DataInicial { get; set; }

        /// <summary>
        /// Data Final.
        /// </summary>
        public DateTime DataFinal { get; set; }
    }
}
