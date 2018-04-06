using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.IO;
using System.ServiceModel;

namespace Redecard.PN.Emissores.Servicos
{
    [MessageContract]
    public class DadosArquivo
    {
        [MessageBodyMember(Order = 1)]
        public System.IO.Stream Arquivo { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public String NomeArquivo { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public String CodigoEmissor { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public String Mes { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public String Ano { get; set; }
        [MessageHeader(MustUnderstand = true)]
        public DateTime DataCriacao { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public Int32 Tamanho { get; set; }
    }
}