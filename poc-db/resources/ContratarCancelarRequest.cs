using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe ConciliadorRequest
    /// </summary>
    [DataContract]
    public class ContratarCancelarRequest
    {
        /// <summary>
        /// <para>1: 0206 - Conciliador</para>
        /// <para>2: 0207 - Retroativo</para>
        /// </summary>
        [DataMember]
        public Int32[] Servicos { get; set; }

        /// <summary>
        /// <para>1 - Contratação</para> 
        /// <para>2 - Cancelamento</para>
        /// </summary>
        [DataMember]
        public Int32 TipoSolicitacao { get; set; }
    }
}
