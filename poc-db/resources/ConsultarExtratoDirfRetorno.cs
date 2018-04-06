using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarExtratoDirfRetorno
    {
        [DataMember]
        public string RazaoSocialEstabelecimento { get; set; }
        [DataMember]
        public string ComarcaEstabelecimento { get; set; }
        [DataMember]
        public string EnderecoEstabelecimento { get; set; }
        [DataMember]
        public string BairroEstabelecimento { get; set; }
        [DataMember]
        public string CidadeEstabelecimento { get; set; }
        [DataMember]
        public string EstadoEstabelecimento { get; set; }
        [DataMember]
        public int CepEstabelecimento { get; set; }
        [DataMember]
        public string CodigoMalaDiretaEstabelecimento { get; set; }
        [DataMember]
        public List<ConsultarExtratoDirfEstabelecimentoRetorno> Estabelecimentos { get; set; }
        [DataMember]
        public List<ConsultarExtratoDirfEmissorRetorno> Emissores { get; set; }
    }
}