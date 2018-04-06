using System;

namespace Rede.PN.CondicaoComercial.SharePoint.Business
{
    /// <summary>
    /// Modelo para os dados da imagem da bandeira
    /// </summary>
    [Serializable]
    public class BandeiraImagem
    {
        public Int32 Codigo { get; set; }
        public String Descricao { get; set; }
        public String Url { get; set; }
    }
}
