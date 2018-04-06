using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Classe modelo de Avisos de Débito utilizada nas telas do módulo
    /// Request de 'Avisos de Débito - Crédito' e 'Avisos de Débito - Débito').
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS dos Books:<br/>
    /// - Book BXA770 / Programa XA770 / TranID IS67<br/>
    /// - Book BXD204CB / Programa XD204 / TranID XDS4
    /// </remarks>    
    [DataContract]
    public class AvisoDebito : ModeloBase
    {
        /// <summary>
        /// Número do Processo
        /// </summary>        
        [DataMember]
        public Decimal Processo { get; set; }

        /// <summary>
        /// Número do Resumo de Venda
        /// </summary>        
        [DataMember]
        public Decimal ResumoVenda { get; set; }

        /// <summary>
        /// Indicador de Request
        /// </summary>
        [DataMember]
        public Boolean IndicadorRequest { get; set; }

        /// <summary>
        /// Número do Cartão / NSU da Redecard / Número transação Acquirer
        /// </summary>        
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>
        /// Número do Ponto de Venda
        /// </summary>        
        [DataMember]
        public Decimal PontoVenda { get; set; }

        /// <summary>
        /// Número da Centralizadora
        /// </summary>        
        [DataMember]
        public Decimal Centralizadora { get; set; }

        /// <summary>
        /// Data da Venda
        /// </summary>        
        [DataMember]
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Valor da Venda
        /// </summary>        
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>
        /// Data de Cancelamento
        /// </summary>        
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>
        /// Valor Líquido do Cancelamento
        /// </summary>        
        [DataMember]
        public Decimal ValorLiquidoCancelamento { get; set; }

        /// <summary>
        /// Código do Motivo do Débito
        /// </summary>        
        [DataMember]
        public Int16  CodigoMotivoDebito { get; set; }

        /// <summary>
        /// Flag NSU Cartão
        /// </summary>        
        [DataMember]
        public Char? FlagNSUCartao { get; set; }

        /// <summary>
        /// Tipo do Cartão
        /// </summary>        
        [DataMember]
        public String TipoCartao { get; set; }

        /// <summary>
        /// Indicador de Parcela
        /// </summary>        
        [DataMember]
        public Boolean? IndicadorParcela { get; set; }
    }
}