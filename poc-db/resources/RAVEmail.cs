/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Jun Watanabe]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Tiago Jun Watanabe - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.RAV.Servicos
{
    [DataContract]
    public class ModRAVEmail
    {
        [DataMember]
        public int Sequencia { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public EStatusEmail Status { get; set; }

        [DataMember]
        public EPeriodicidadeEmail Periodicidade { get; set; }

        [DataMember]
        public DateTime DataUltAlteracao { get; set; }

        [DataMember]
        public DateTime DataUltInclusao { get; set; }
    }

    public enum EStatusEmail
    { [EnumMember]
        None = 0,
        [EnumMember]
        Incluso = 'I',
        [EnumMember]
        Alterado = 'A',
        [EnumMember]
        Excluido = 'E' }

    public enum EPeriodicidadeEmail
    { Diario = 'D', Semanal = 'S', Quinzenal = 'Q', Mensal = 'M' }
}