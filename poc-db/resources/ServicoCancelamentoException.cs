/*
(c) Copyright [2012] Redecard S.A.
Autor : [Guilherme Alves Brito]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/15 - Guilherme Alves Brito - Versão Inicial
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Cancelamento.Servicos.Erros
{
    [DataContract]
    public class ServicoCancelamentoException
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