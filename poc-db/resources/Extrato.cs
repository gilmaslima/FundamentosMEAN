using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Servicos
{
    /// <summary>
    /// 2a Via do extrato de papel
    /// </summary>
    /// <remarks>
    ///Encapsula os registros retornadors pelo Serviço HIS do Book:<br/>
    /// - Book WACA742 / Programa WA742 / TranID IS28
    /// </remarks>
    public class Extrato
    { 
      
        /// <summary>
        /// Numero do box - WACA742-S-T1-NUM-BOX
        /// </summary>
        public String NumeroBox { get; set; }
        /// <summary>
        /// Linha texo do registro do extrato - WACA742-S-T1-LINHA
        /// </summary>
        public String LinhaExtrato { get; set; }
    }
}
