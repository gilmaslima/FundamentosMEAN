/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/18 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DadosSolicitacaoTecnologia
    {
        [DataMember]
        public int NumeroSolicitacao { get; set; }

        [DataMember]
        public DateTime Data { get; set; }

        [DataMember]
        public DateTime? DataInstalacao { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string TipoEquipamento { get; set; }
    }
}
