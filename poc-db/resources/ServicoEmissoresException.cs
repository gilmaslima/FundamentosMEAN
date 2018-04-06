using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Redecard.PN.Comum;

namespace Redecard.PN.Emissores.Servicos.Erro
{
    
    [Serializable]
    public class ServicoEmissoresException: PortalRedecardException
    {

        public ServicoEmissoresException(int codigo, string fonte) : base(codigo, fonte) { }
        public ServicoEmissoresException(int codigo, string fonte, Exception ex) : base(codigo, fonte, ex) { }
        public ServicoEmissoresException(int codigo, string fonte, string mensagem, Exception ex) : base(codigo, fonte,mensagem, ex) { }
        
                       
        /// <summary>
        /// Código do Erro.
        /// </summary>
        //[DataMember]
        //public int Codigo { get; set; }

        /// <summary>
        /// Arquivo/Fonte origem do Erro.
        /// </summary>
        //[DataMember]
        //public string Fonte { get; set; }
    }
}