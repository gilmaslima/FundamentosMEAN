using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class Proprietario
    {

        [DataMember]
        public TipoPessoa CodigoTipoPessoa { get; set; }

        [DataMember]
        public Int64 NumeroCNPJ { get; set; }

        [DataMember]
        public Int32 NumeroSequenciaProposta { get; set; }

        [DataMember]
        public TipoPessoa CodigoTipoPesssoaProprietario { get; set; }

        [DataMember]
        public Int64 NumeroCNPJCPFProprietario { get; set; }

        [DataMember]
        public String NomeProprietario { get; set; }

        [DataMember]
        public DateTime DataNascimentoProprietario { get; set; }

        [DataMember]
        public Double ParticipacaoAcionaria { get; set; }

        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }

        [DataMember]
        public TipoAcaoProprietario TipoAcaoProprietario { get; set; }

        [DataMember]
        public bool? DadosRetornadosSerasa { get; set; }
    }
}
