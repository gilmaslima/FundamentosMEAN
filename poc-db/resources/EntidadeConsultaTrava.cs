/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/31 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Modelos
{

    public class EntidadeConsultaTrava
    {

        public decimal ValorTotalFaixaMasterCard { get; set; }
        public decimal ValorTotalFaixaVisa { get; set; }
        public decimal ValorTotalFaixas { get; set; }
        public decimal ValorTotalCobradoFaixaMasterCard { get; set; }
        public decimal ValorTotalCobradoFaixaVisa { get; set; }
        public decimal ValorTotalCobradoFaixas { get; set; }
        public decimal ValorTotalCobrancaMasterCard { get; set; }
        public decimal ValorTotalCobrancaVisa { get; set; }
        public int TotalOcrrenciasRetornada { get; set; }

        public decimal ValorTotalCobranca { get; set; }


        public List<DadosConsultaTrava> DadosConsultaTravas { get; set; }
    }
}
