using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Boston.Servicos
{
    [DataContract]
    public class RetornoGravarAtualizarPasso1
    {
        [DataMember]
        public Int32 CodigoRetorno { get; set; }

        [DataMember]
        public String DescricaoRetorno { get; set; }

        [DataMember]
        public DadosSerasa DadosSerasa { get; set; }

        [DataMember]
        public Int32 CodTipoEstabelecimento { get; set; }

        [DataMember]
        public Int32 NumPdvMatriz { get; set; }

        [DataMember]
        public Int32 NumSequencia { get; set; }

        [DataMember]
        public PropostaPendente PropostaPendente { get; set; }

        [DataMember]
        public Endereco EnderecoComercial { get; set; }

        [DataMember]
        public Endereco EnderecoCorrespondencia { get; set; }

        [DataMember]
        public DomicilioBancario DomicilioBancarioCredito { get; set; }

        [DataMember]
        public Tecnologia DadosTecnologia { get; set; }

        [DataMember]
        public Char IndMarketingDireto { get; set; }
    }
}
