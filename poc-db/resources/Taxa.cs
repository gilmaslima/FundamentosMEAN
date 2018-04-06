using System;

namespace Rede.PN.CondicaoComercial.SharePoint.Business
{
    /// <summary>
    /// Modelo de Taxa
    /// </summary>
    public class Taxa
    {
        public String ModalidadeVenda { get; set; }
        public Decimal ValorTaxa { get; set; }
        public Decimal ValorTaxaEmissor { get; set; }
        public Decimal Tarifa { get; set; }
        public String Parcelas { get; set; }
        public String Prazo { get; set; }
        public Boolean Predatado { get; set; }
    }
}
