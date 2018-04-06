using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    /// <summary>
    /// Classe Response ServicoRegimeResponse
    /// </summary>
    [DataContract]
    public class ServicoRegimeResponse
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
    }
}
