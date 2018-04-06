using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{
    public class MensagemErro
    {
        /// <summary>
        /// Código do Erro
        /// 1 - OK
        /// 2 - Erro no Banco de Dados
        /// </summary>
        public Int32 CodigoRetorno { get; set; }        
      
        public String MensagemRetorno { get; set; }
        public String DataExecucao { get; set; }
        public String Metodo { get; set; }
        public String ServidorExecucao { get; set; }
        public String StackTrace { get; set; }
    }
}
