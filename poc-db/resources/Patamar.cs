using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Sharepoint.Modelo
{
    [Serializable]
    public class Patamar
    {
        public Int32 CodCca { get; set; }
        public Int32 CodFeature { get; set; }
        public Int32 Prazo { get; set; }
        public Int32 SequenciaPatamar { get; set; }
        public Int32? PatamarInicial { get; set; }
        public Int32? PatamarFinal { get; set; }
        public Double? TaxaPatamar { get; set; }
    }
}
