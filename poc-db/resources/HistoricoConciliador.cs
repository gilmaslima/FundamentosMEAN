using System;

namespace Redecard.PN.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public class HistoricoConciliador
    {
        /// <summary>
        /// 
        /// </summary>
        public String DataHoraEntrada { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String DataHoraSaida { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Servidor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int64 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Login { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoErro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String MensagemErro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int16 Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int16 Etapa { get; set; }
    }
}