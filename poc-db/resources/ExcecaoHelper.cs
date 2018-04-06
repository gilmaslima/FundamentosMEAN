/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe ExcecaoHelper, que expõe métodos para manipular as exceções do Helper.
    /// </summary>
    public class ExcecaoHelper
    {
        private static string regexIdentificarCodigoCampo = @"\((\w+)\)\Z";
        private static string regexIdentificarCodigoEstabelecimentoInvalido = @"{(\w+)}";
        /// <summary>
        /// Este método é utilizado para obter cóodigo do campo de erro.
        /// </summary>
        /// <param name="corpoMensagemFMS"></param>
        /// <returns></returns>
        public static int ObterCodigoCampoErro(string corpoMensagemFMS)
        {
            string valorErro = Regex.Match(corpoMensagemFMS, regexIdentificarCodigoCampo).Groups[1].Value;
            int codigoErro = 0;

            if (valorErro != null)
            {
                codigoErro= int.Parse(valorErro);
            }
            return codigoErro;
        }
        /// <summary>
        /// Este método é utilizado para obter um estabelecimento inválido.
        /// </summary>
        /// <param name="corpoMensagemFMS"></param>
        /// <returns></returns>
        public static string ObterEstabelecimentoInvalido(string corpoMensagemFMS)
        {
            string valorErro = Regex.Match(corpoMensagemFMS, regexIdentificarCodigoEstabelecimentoInvalido).Groups[1].Value;
            string valorInvalido = "";

            if (valorErro != null)
            {
                valorInvalido = valorErro;
            }
            return valorInvalido;
        }
    }
}
