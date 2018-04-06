/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.OutrosServicos.ContratoDados
{
    /// <summary>
    /// Define os valores de response de lista padrão.
    /// </summary>
    [DataContract]
    public class ResponseBaseList<T> : ResponseBase
    {
        /// <summary>
        /// Define os itens de retorno do tipo T
        /// </summary>
        [DataMember(Name = "itens", EmitDefaultValue = false)]
        public T[] Itens { get; set; }
    }
}