using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
	/// <summary>
	/// Define os valores de response do relatório de Estorno.
	/// </summary>
	[DataContract]
	public class RelatorioEstornoResponse
    {
        public RelatorioEstornoResponse()
        {
            this.StatusRelatorio = new StatusRetorno();
            this.StatusTotalizador = new StatusRetorno();
        }

        /// <summary>
        /// Define a Quantidade Total Registros.
        /// </summary>
        [DataMember]
        public Int32 QuantidadeTotalRegistros { get; set; }

        /// <summary>
        /// Define o Totalizador.
        /// </summary>
        [DataMember]
        public EstornoTotalizador Totalizador { get; set; }

        /// <summary>
        /// Define os Registros.
        /// </summary>
        [DataMember]
        public List<EstornoD> Registros { get; set; }

        /// <summary>
        /// Define o Status do Totalizador.
        /// </summary>
        [DataMember]
        public StatusRetorno StatusTotalizador { get; set; }

        /// <summary>
        /// Define o Status do Relatorio.
        /// </summary>
        [DataMember]
        public StatusRetorno StatusRelatorio { get; set; }

        /// <summary>
        /// Define a mensagem de retorno.
        /// </summary>
        [DataMember]
        public String Mensagem { get; set; }
    }
}