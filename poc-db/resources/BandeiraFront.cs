using System;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Essa classe é usada no dados bandarios para renderizar as bandeiras dentro do quadro de bancos do estabelecimento
    /// </summary>
    public class BandeiraFront
    {
        public String SiglaProduto { get; set; }
        public Boolean Trava { get; set; }
        public String DescricaoBandeira { get; set; }
        public Char TipoTransacao { get; set; }
        public Boolean ErroValidacao { get; set; }

    }
}
