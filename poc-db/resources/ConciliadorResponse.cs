using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    /// <summary>
    /// Classe Modelo Response ConciliadorResponse
    /// </summary>
    [DataContract]
    public class ConciliadorResponse
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
        ///  Define o Código Sequencial do Regime
        /// </summary>
        [DataMember]
        public Int32 SequencialRegime { get; set; }

        /// <summary>
        ///  Define o Patamar Inicial
        /// </summary>
        [DataMember]
        public Int32 PatamarInicial { get; set; }

        /// <summary>
        ///  Define o Patamar Final
        /// </summary>
        [DataMember]
        public Int32 PatamarFinal { get; set; }

        /// <summary>
        ///  Define o Valor da Franquia
        /// </summary>
        [DataMember]
        public Double ValorFranquia { get; set; }

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
