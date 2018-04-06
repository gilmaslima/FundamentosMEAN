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
    /// Define os valores de response padrão, quando existe modelo a ser retornado.
    /// </summary>
    [DataContract]
    public class ResponseBaseItem<T> : ResponseBase
    {
        /// <summary>
        /// Define o modelo de retorno.
        /// </summary>
        [DataMember(Name = "item", EmitDefaultValue = false)]
        public T Item { get; set; }
    }
}