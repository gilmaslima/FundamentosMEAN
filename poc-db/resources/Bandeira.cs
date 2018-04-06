using System;
using System.Collections.Generic;

namespace Rede.PN.CondicaoComercial.SharePoint.Business
{
    /// <summary>
    /// Modelo de bandeira
    /// </summary>
    public class Bandeira
    {
        public String Nome { get; set; }
        public Int32 Codigo { get; set; }

        #region Itens para remover

        public String NomeBanco { get; set; }
        public String NomeAgencia { get; set; }
        public String CodigoAgencia { get; set; }
        public String ContaAtualizada { get; set; }

        #endregion

        public List<Taxa> Taxas { get; set; }
        public List<Flex> Flex { get; set; }
    }
}
