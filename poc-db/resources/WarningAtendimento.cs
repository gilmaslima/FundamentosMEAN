using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business
{
    [Serializable]
    public class WarningAtendimento
    {
        public Int32 CodigoTipoAtividade { get; set; }
        public String Texto { get; set; }
        public String TextoBotao { get; set; }
        public String UrlDestino { get; set; }
        public Int32 DiasHistorico { get; set; }
        public Boolean Ativo { get; set; }
        public Boolean ExibirBotao { get; set; }
    }
}
