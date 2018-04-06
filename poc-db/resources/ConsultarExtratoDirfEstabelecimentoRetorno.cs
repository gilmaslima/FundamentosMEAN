using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarExtratoDirfEstabelecimentoRetorno
    {
        [DataMember]
        public decimal ValorCobrado { get; set; }
        [DataMember]
        public decimal ValorIrRecebido { get; set; }
        [DataMember]
        public decimal ValorRecebido { get; set; }
        [DataMember]
        public decimal ValorRepassadoEmissor { get; set; }
    }
}