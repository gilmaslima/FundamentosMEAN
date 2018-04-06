using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Modelo
{
    public class ProdutoVan
    {
        public Char CodTipoPessoa { get; set; }
        public Int64 NumCNPJ { get; set; }
        public Int32 NumSeqProp { get; set; }
        public Int32 CodCca { get; set; }
        public Char? IndTipoOperacaoProd { get; set; }
        public String Usuario { get; set; }
    }
}
