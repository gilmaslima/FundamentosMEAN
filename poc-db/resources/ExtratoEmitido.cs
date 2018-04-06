using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Modelo
{
    /// <summary>
    /// Classe retorno book WACA740 
    /// </summary>
    public class ExtratoEmitido : ExtratoBase
    {
        /// <summary>
        /// Indica extrato recuperado quando = "R"
        /// </summary>
        public String Recuperado { get; set; }
    }
}
