using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA748 - Resumo de vendas - CDC / Cartões de débito - Ajustes a crédito / débito.
    /// </summary>
    [DataContract]
    public class ConsultarDadosAjustesDebitosDetalheRetorno
    {
        [DataMember]
        public short Codigo { get; set; }
        [DataMember]
        public DateTime DataReferencia { get; set; }
        [DataMember]
        public string DebitoDesagendamento { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public int Estabelecimento { get; set; }
        [DataMember]
        public string Referencia { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public decimal ValorDebito { get; set; }
    }
}