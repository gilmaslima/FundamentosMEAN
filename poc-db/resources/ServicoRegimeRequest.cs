using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe ConsultaServicoRegimeRequest
    /// </summary>
    [DataContract]
    public class ServicoRegimeRequest
    {
        /// <summary>
        /// Define o Codigo do Servico
        /// </summary>
        [DataMember]
        public Int32 CodigoServico { get; set; }

    }
}
