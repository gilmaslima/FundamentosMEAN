/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.RAV.Servicos
{
    [DataContract]
    public class ServicoRAVException
    {
        /// <summary>
        /// Código do Erro.
        /// </summary>
        [DataMember]
        public int Codigo { get; set; }

        /// <summary>
        /// Arquivo/Fonte origem do Erro.
        /// </summary>
        [DataMember]
        public string Fonte { get; set; }
    }
}