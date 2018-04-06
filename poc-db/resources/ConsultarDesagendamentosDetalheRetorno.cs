using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA616 - Resumo de vendas - Cartões de débito - Desagendamentos.
    /// </summary>
    [DataContract]
    public class ConsultarDesagendamentosDetalheRetorno
    {
        [DataMember]
        public short Codigo { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public DateTime DataCarta { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public string NumeroProcesso { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
    }
}