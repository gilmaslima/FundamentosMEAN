using System;

namespace Redecard.PN.RAV.Sharepoint
{
    /// <summary>
    /// Classe com métodos comum do projeto RAV
    /// </summary>
    public class RAVComum
    {
        /// <summary>
        /// Retorna alias do campo Produto Antecipação do projeto cessão de crédito
        /// </summary>
        /// <param name="nome">Nome original(mainframe)</param>
        /// <returns>Nome formatado</returns>
        public static String RetornaAliasProdutoAntecipacao(String nome)
        {
            if (!String.IsNullOrEmpty(nome))
            {
                switch (nome)
                {
                    case "CESSAO DE CREDITO":
                        return "Cess&atilde;o de Cr&eacute;dito"; //return HttpUtility.HtmlEncode("Cessão de Crédito");
                        
                    default:
                        return nome;
                }
            }
            else
                return String.Empty;
        }
    }
}
