using System;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Essa classe é usada no dados bandarios para renderizar o quadro de bancos do estabelecimento
    /// </summary>
    public class BancoFront
    {
        public String NomeBanco { get; set; }
        public int CodigoBanco { get; set; }
        public String CodigoAgencia { get; set; }
        public String NumeroConta { get; set; }
        public List<BandeiraFront> BandeirasCredito { get; set; }
        public List<BandeiraFront> BandeirasDebito { get; set; }
        public Boolean ConfirmacaoEletronica { get; set; }
    }
}
