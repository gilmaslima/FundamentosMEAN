using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    /// <summary>
    /// Classe Modelo Response ConciliadorResponse
    /// </summary>
    [DataContract]
    public class ServicoAtivoResponse
    {
        /// <summary>
        ///  Define o Codigo do Servico
        /// </summary>
        [DataMember]
        public Int32 CodigoServico { get; set; }

        /// <summary>
        ///  Define a Descrição do Serviço
        /// </summary>
        [DataMember]
        public string DescricaoServico { get; set; }

        /// <summary>
        ///  Define a Código de Status do Serviço
        /// </summary>
        [DataMember]
        public char StatusServico { get; set; }

        /// <summary>
        ///  Define o Código do Regime
        /// </summary>
        [DataMember]
        public Int32 CodigoRegime { get; set; }

        /// <summary>
        ///  Define uma lista de Patamares do Servico
        /// </summary>
        [DataMember]
        public List<ConciliadorPatamarResponse> Patamares { get; set; }

        /// <summary>
        /// Data de Inclusão do Serviço
        /// </summary>
        [DataMember]
        public DateTime? DataInclusaoServico { get; set; }

        /// <summary>
        /// Data de Cancelamento do Serviço
        /// </summary>
        [DataMember]
        public DateTime? DataCancelamentoServico { get; set; }

        /// <summary>
        /// Data de Reativação do Serviço
        /// </summary>
        [DataMember]
        public DateTime? DataReativacaoServico { get; set; }
    }
}
