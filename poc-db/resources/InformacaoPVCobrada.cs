using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Emissores.Modelos
{
    //ZP381
    public class InformacaoPVCobrada
    {

        public String Tipo { get; set; }
        public Int32 NumeroPV { get; set; }
        public Decimal Cnpj { get; set; }
        public String DataInicial { get; set; }
        public String DataFinal { get; set; }
        public Int16 QuantidadeDias { get; set; }
        public Decimal ValorFaturamento { get; set; }
        public Decimal ValCobranca { get; set; }
        public String PVCentralizador { get; set; }
        public Int32 CodigoAgencia { get; set; }
        public String NumeroConta { get; set; }
        public String SiglaProduto { get; set; }

    }
}
