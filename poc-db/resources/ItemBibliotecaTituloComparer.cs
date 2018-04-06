using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Redecard.Portal.Helper.DTO;

namespace Redecard.Portal.Helper.Comparadores
{
    /// <summary>
    /// Autor: Cristiano Martins Dias
    /// Data criação: 26/10/2010
    /// Descrição: Classe implementadora da interface IEqualityComparer&lt;ItemBiblioteca&gt; para integração com métodos do LINQ
    /// Link de apoio: http://nayyeri.net/icomparer-vs-iequalitycomparer
    /// </summary>
    public sealed class ItemBibliotecaTituloComparer : IEqualityComparer<ItemBiblioteca>
    {
        /// <summary>
        /// Executa comparação pelo título
        /// Com base nisso, quando utilizado pela função Distinct() do LINQ, os itens com mesmo Título serão eliminados
        /// </summary>
        public bool Equals(ItemBiblioteca x, ItemBiblioteca y)
        {
            return x.Titulo.ToUpper().Equals(y.Titulo.ToUpper());
        }

        public int GetHashCode(ItemBiblioteca item)
        {
            return base.GetHashCode();
        }
    }
}