using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
    /// </summary>
    public class ConsultarMotivoDebitoRetornoDTO
    {
        public string DataInclusao { get; set; }
        public int NumeroEstabelecimentoOrigem { get; set; }
        public string MotivoDebito { get; set; }
        public string ReferenciaProcessamento { get; set; }
        public string Resumo { get; set; }
        public string BandeiraResumoVendas { get; set; }
        public string DataVenda { get; set; }
        public string Cartao { get; set; }
        public string NumeroComprovanteVenda { get; set; }
        public string ValorConta { get; set; }
        public string Parcela { get; set; }
        public List<ConsultarMotivoDebitoFormaPagamentoRetornoDTO> FormasPagamento { get; set; }
    }
}
