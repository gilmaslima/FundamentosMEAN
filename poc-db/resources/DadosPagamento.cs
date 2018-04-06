using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Redecard.PN.DataCash.Modelo
{
    [Serializable]
    public class DadosPagamento
    {
        /// <summary>
        /// Valor
        /// </summary>
        public Decimal Valor { get; set; }

        /// <summary>
        /// Valor formatado 
        /// </summary>
        public String ValorFormatado
        {
            get
            {
                return this.Valor.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Data de Vencimento
        /// </summary>
        public String DataVencimento { get; set; }

        /// <summary>
        /// Multa de Atraso
        /// </summary>
        public Double MultaAtraso { get; set; }

        /// <summary>
        /// Multa por Atraso formatado 
        /// </summary>
        public String MultaAtrasoFormatado
        {
            get
            {
                return this.MultaAtraso.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Juros ao Dia
        /// </summary>
        public Double JurosDia { get; set; }

        /// <summary>
        /// Valor dos juros formatado 
        /// </summary>
        public String JurosDiaFormatado
        {
            get
            {
                return this.JurosDia.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) + " %";
            }
        }

        /// <summary>
        /// Número do pedido
        /// </summary>
        public String NumeroPedido { get; set; }

        /// <summary>
        /// Nota
        /// </summary>
        public String Nota { get; set; }
    }
}
