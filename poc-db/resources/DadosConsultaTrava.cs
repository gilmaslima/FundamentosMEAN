/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/31 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Modelos
{
    public class DadosConsultaTrava
    {
        public decimal FaixaInicialFaturamento { get; set; }

        public decimal FaixaFinalFaturamento { get; set; }

        public decimal FatorMultiplicado { get; set; }

        public int QuantidadePVs { get; set; }

        public decimal QuantidadeDias { get; set; }

        public decimal TotalFaturamentoMasterCard { get; set; }

        public decimal TotalFaturamentoVisa { get; set; }

        public decimal TotalFaturamento { get; set; }

        public decimal TotalCobrancaMasterCard { get; set; }

        public decimal TotalCobrancaVisa { get; set; }

        public decimal TotalCobranca { get; set; }


    }
}
