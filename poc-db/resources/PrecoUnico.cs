using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    [Serializable]
    public class PrecoUnico
    {
        public Decimal ValorFaturamentoContrato { get; set; }
        public String QuantidadeEquipamento { get; set; }
        public String TipoEquipamento { get; set; }
        public Decimal ValorPrecoUnicoSemFlex { get; set; }
        public Decimal ValorPrecoUnicoComFlex { get; set; }
    }
}
