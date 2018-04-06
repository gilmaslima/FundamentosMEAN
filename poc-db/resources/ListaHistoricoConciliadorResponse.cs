using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Servicos.Modelos
{
    /// <summary>
    /// Lista de históricos de logs do Conciliador
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [DataContract]
    public class ListaHistoricoConciliadorResponse<T>
    {
        /// <summary>
        /// Lista de Históricos
        /// </summary>
        [DataMember]
        public List<T> Historicos { get; set; }
    }
}