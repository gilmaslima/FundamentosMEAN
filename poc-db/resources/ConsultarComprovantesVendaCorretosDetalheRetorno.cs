using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA797 - Resumo de vendas - CDC - CV's aceitos.
    /// </summary>
    [DataContract]
    public class ConsultarComprovantesVendaCorretosDetalheRetorno
    {
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public int Horas { get; set; }
        [DataMember]
        public decimal Numero { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public short Plano { get; set; }
        [DataMember]
        public short SubTipoTransacao { get; set; }
        [DataMember]
        public short TipoTransacao { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public decimal ValorComplementar { get; set; }
        [DataMember]
        public decimal ValorSaque { get; set; }
    }
}