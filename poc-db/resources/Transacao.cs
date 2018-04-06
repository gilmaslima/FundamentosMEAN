using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.DataCash.Servicos
{
    [DataContract]
    public class TotalTransacoes : MensagemErro
    {
        [DataMember]
        public Int32 TotalTransacoesAprovadas { get; set; }

        [DataMember]
        public Int32 TotalTransacoesReprovadas { get; set; }
    }
}