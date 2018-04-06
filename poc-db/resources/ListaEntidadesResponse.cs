using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    /// <summary>
    /// Classe de Listagem de Entidades.
    /// </summary>
    [DataContract]
    public class ListaEntidadesResponse
    {
        /// <summary>
        /// Listagem de regimes.
        /// </summary>
        [DataMember]
        public ItemEntidadeResponse[] Itens { get; set; }

        /// <summary>
        /// Total de itens.
        /// </summary>
        [DataMember]
        public Int32 TotalItens { get; set; }
    }
}
