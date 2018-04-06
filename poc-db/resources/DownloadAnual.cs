using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Emissores.Modelos
{
    
    public class DownloadAnual
    {
        
        public int Ano { get; set; }

        
        public List<DownloadMes> Meses { get; set; }

    }
}
