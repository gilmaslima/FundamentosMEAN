using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.DataCash
{
    /// <summary>
    /// Métodos de Extensão para o namespace Redecard.PN.DataCash
    /// </summary>
    public static class MetodosExtensao
    {
        /// <summary>
        /// Se String for nula, vazia ou contém apenas espaços em branco, retorna o valor do parâmetro "substituto"
        /// </summary>
        /// <param name="valor">String a ser verificada</param>
        /// <param name="substituto">Valor que irá substituir a String que seja nula, 
        /// vazia ou que contenha apenas espaços em branco</param>
        /// <returns>String</returns>
        public static String IfNullOrEmpty(this String valor, String substituto)
        {
            if (String.IsNullOrEmpty(valor) || String.IsNullOrWhiteSpace(valor))
                return substituto;
            else
                return valor;
        }
    }
}