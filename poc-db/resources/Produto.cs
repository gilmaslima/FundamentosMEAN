using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Boston.Modelo
{
    public class Produto
    {
        public Char CodTipoPessoa { get; set; }
        public Int64 NumCNPJ { get; set; }
        public Int32 NumSeqProp { get; set; }
        public Int32 CodCca { get; set; }
        public Char IndTipoOperacaoProd { get; set; }
        public Int32 CodFeature { get; set; }
        public Char TipoRegimeNegociado { get; set; }
        public Int32 CodRegimePadrao { get; set; }
        public Int32 PrazoPadrao { get; set; }
        public Double TaxaPadrao { get; set; }
        public Int32 CodRegimeMinimo { get; set; }
        public Int32 PrazoMinimo { get; set; }
        public Double TaxaMinimo { get; set; }
        public Char IndAceitaFeature { get; set; }
        public String Usuario { get; set; }
        public Double ValorLimiteParcela { get; set; }
        public Char IndFormaPagamento { get; set; }
    }
}
