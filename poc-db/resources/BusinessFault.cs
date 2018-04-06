using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.FMS.Servico
{
    [DataContract]
    public class BusinessFault
    {
        [DataMember]
        public Int32 Codigo { get; private set; }
        [DataMember]
        public String Mensagem { get; private set; }

        public BusinessFault(Int32 codigo, String mensagem)
        {
            this.Codigo = codigo;
            this.Mensagem = mensagem;
        }
    }
}