/*
(c) Copyright [2012] Redecard S.A.
Autor : [Lucas Nicoletto da Cunha]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/11/05 - Lucas Nicoletto da Cunha - Versão Inicial
*/
using System.Runtime.Serialization;
using System;
namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DadosProprietario
    {         
        [DataMember]
        public string Nome { get; set; }
        
        [DataMember]
        public DateTime DataNascimento { get; set; }

        [DataMember]
        public long CPF { get; set; }

        [DataMember]
        public decimal Percetual { get; set; }

        [DataMember]
        public string TipoPessoa { get; set; }

    }
}
