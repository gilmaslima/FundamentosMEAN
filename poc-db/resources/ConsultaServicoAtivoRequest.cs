using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe ConsultaServicoAtivoRequest
    /// </summary>
    [DataContract]
    public class ConsultaServicoAtivoRequest
    {
        /// <summary>
        /// Define o Numero do PV 
        /// </summary>
        [DataMember]
        public Int64 NumeroPV { get; set; }

        /// <summary>
        /// Define o Codigo do Servico
        /// </summary>
        [DataMember]
        public Int32 CodigoServico { get; set; }

        /// <summary>
        /// Define o Usuario
        /// </summary>
        [DataMember]
        public String Usuario { get; set; } 
    }
}
