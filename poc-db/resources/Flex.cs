using System;

namespace Rede.PN.CondicaoComercial.SharePoint.Business
{
    /// <summary>
    /// Modelo flex
    /// </summary>
    public class Flex
    {
        public String RecebimentoAntecipado { get; set; }
        public String Parcelas { get; set; }
        public String Fator1 { get; set; }
        public String Fator2 { get; set; }
        public Int32 Prazo { get; set; }
        public String Taxa { get; set; }
    }
}
