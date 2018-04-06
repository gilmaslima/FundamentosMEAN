/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/23 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class Integrador
    {
        [DataMember]
        public string Codigo { get; set; }
        
        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public string Situacao { get; set; }

        [DataMember]
        public DateTime DataAtualizacao { get; set; }

        [DataMember]
        public int CodigoUltimaAtualizacao { get; set; }

        [DataMember]
        public DateTime DataAtualizacaoTabela { get; set; }

    }
}
