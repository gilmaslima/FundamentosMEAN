/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;

namespace Redecard.PN.FMS.Sharepoint.Helpers
{
   /// <summary>
    /// Este componente publica a classe XXX, que expõe métodos para manipular as exceções na camada de controle.
   /// </summary>
    public class ManipuladorExcecaoCamadaControle
    {
       /// <summary>
       /// Metodo centralizador dos tratamentos de exceções, verifica se o tipo de exceção é um tipo tratável e lança a exceção para aexibição na tela.
       /// </summary>
       /// <param name="e"></param>
       public static void TrataExcecao(Exception ex)
       {
           if (ex.GetType() == typeof(FaultException))
           {
               if (ex.InnerException.GetType() == typeof(GeneralFault))
               {

               }
           }
       }
    }
}
