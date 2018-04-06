using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business
{
    [Serializable]
    public class Warning
    {
        public List<String> Segmentos { get; set; }
        public List<Int32> Pvs { get; set; }
        public String Tipo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public String Texto { get; set; }
        public String UrlWarning { get; set; }
        public String UrlDestino { get; set; }
        public String TextoBotao { get; set; }
        public String IdentificadorAceite { get; set; }

    }
}
