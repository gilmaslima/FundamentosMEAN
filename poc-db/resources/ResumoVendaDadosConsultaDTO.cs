using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.SharePoint
{
    [Serializable]
    public class ResumoVendaDadosConsultaDTO
    {
        public int NumeroEstabelecimento { get; set; }
        public int NumeroResumoVenda { get; set; }
        public DateTime DataApresentacao { get; set; }
    }
}
