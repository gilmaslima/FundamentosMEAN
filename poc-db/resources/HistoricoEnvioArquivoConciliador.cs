using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos.Modelos
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class HistoricoEnvioArquivoConciliador
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String DataHoraEntrada { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String DataHoraSaida { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String NomeArquivo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int64 Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int32 CodigoErro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String MensagemErro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int16 Status { get; set; }
    }
}