using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.DataCash.Servicos
{
    /// <summary>
    /// Dados do distribuidor
    /// </summary>
    [DataContract]
    public class Distribuidor
    {
        [DataMember]
        public int NumPdv { get; set; }
        [DataMember]
        public int NumPdvDistribuidor { get; set; }
        [DataMember]
        public string NomeDistribuidor { get; set; }
    }
    /// <summary>
    /// Retrna lista paginada de distribuidores
    /// </summary>
    [DataContract]
    public class RetornoDistribuidores
    {
        [DataMember]
        public Int32 QuantidadePaginas { get; set; }
        [DataMember]
        public List<Distribuidor> Distribuidores { get; set; }
    }

}