using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
    /// </summary>
    [DataContract]
    public class ConsultarMotivoDebitoRetorno
    {
        [DataMember]
        public string DataInclusao { get; set; }
        [DataMember]
        public int NumeroEstabelecimentoOrigem { get; set; }
        [DataMember]
        public string MotivoDebito { get; set; }
        [DataMember]
        public string ReferenciaProcessamento { get; set; }
        [DataMember]
        public string Resumo { get; set; }
        [DataMember]
        public string BandeiraResumoVendas { get; set; }
        [DataMember]
        public string DataVenda { get; set; }
        [DataMember]
        public string Cartao { get; set; }
        [DataMember]
        public string NumeroComprovanteVenda { get; set; }
        [DataMember]
        public string ValorConta { get; set; }
        [DataMember]
        public string Parcela { get; set; }
        [DataMember]
        public List<ConsultarMotivoDebitoFormaPagamentoRetorno> FormasPagamento { get; set; }
    }
}