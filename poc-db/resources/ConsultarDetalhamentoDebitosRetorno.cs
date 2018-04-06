using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDetalhamentoDebitosRetorno
    {
        [DataMember]
        public List<ConsultarDetalhamentoDebitosDetalheRetorno> Registros { get; set; }
        [DataMember]
        public ConsultarDetalhamentoDebitosTotaisRetorno Totais { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }
    }
}