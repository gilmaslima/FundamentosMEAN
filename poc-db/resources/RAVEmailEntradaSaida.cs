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
    public class ModRAVEmailEntradaSaida
    {
        public ModRAVEmailEntradaSaida()
        { ListaEmails = new List<ModRAVEmail>(); }

        [DataMember]
        public long NumeroPDV { get; set; }

        [DataMember]
        public char IndEnviaEmail { get; set; }

        [DataMember]
        public char IndEnviaFluxoCaixa { get; set; }

        [DataMember]
        public char IndEnviaValoresPV { get; set; }

        [DataMember]
        public char IndEnviaResumoOperacao { get; set; }

        [DataMember] 
        public char IndRegistro { get; set; }

        [DataMember] 
        public IList<ModRAVEmail> ListaEmails { get; set; }
    }
}