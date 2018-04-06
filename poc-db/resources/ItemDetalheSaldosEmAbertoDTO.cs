using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Extrato.Modelo
{
    public class ItemDetalheSaldosEmAbertoDTO : BasicDTO
    {
        public DateTime DataReferencia { get; set; }
        public Int16 CodigoBandeira { get; set; }
        public Int16 CodigoBanco { get; set; }
        public Int16 CodigoAgencia { get; set; }
        public String ContaCorrente { get; set; }
        public Int32 CodigoEstabelecimento { get; set; }
        public Decimal ValorBruto { get; set; }
        public Decimal ValorLiquido { get; set; }
        
    }
}