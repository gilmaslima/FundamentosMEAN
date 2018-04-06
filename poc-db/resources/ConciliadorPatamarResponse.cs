using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    /// <summary>
    /// Classe Response ConciliadorPatamarResponse
    /// </summary>
    [DataContract]
    public class ConciliadorPatamarResponse
    {
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
