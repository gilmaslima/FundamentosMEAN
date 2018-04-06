using Redecard.PN.DadosCadastrais.Modelo;
using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Modelo.Mensagens
{
    [DataContract]
    public class ConfirmacaoPositivaPrimeiroAcessoRequest
    {
        [DataMember]
        public string Banco { get; set; }

        [DataMember]
        public string Agencia { get; set; }

        [DataMember]
        public string ContaCorrente { get; set; }

        [DataMember]
        public EntidadeServicoModel[] EntidadesPNSelecionadas { get; set; }

        [DataMember]
        public long? CpfProprietario { get; set; }

        [DataMember]
        public long? CnpjEstabelecimento { get; set; }

        [DataMember]
        public long? CpfCnpjSocio { get; set; }
    }
}
