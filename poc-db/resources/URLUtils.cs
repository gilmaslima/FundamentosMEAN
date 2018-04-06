using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Redecard.Portal.Helper.Web
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe utilitária para manipulações de URL
    /// </summary>
    public static class URLUtils
    {
        /// <summary>
        /// Objeto para concatenação da URL a ser desmembrada e remontada
        /// </summary>
        public static StringBuilder sbURL = null;

        /// <summary>
        /// Codifica caracteres para uso em ambiente web (para uso em URL's por exemplo)
        /// </summary>
        /// <param name="informacao"></param>
        /// <returns></returns>
        public static string URLEncode(string informacao)
        {
            return HttpContext.Current.Server.UrlEncode(informacao);
        }

        /// <summary>
        /// Decodifica caracteres para uso em ambiente web
        /// </summary>
        /// <param name="informacao"></param>
        /// <returns></returns>
        public static string URLDecode(string informacao)
        {
            // Verificar se o contexto é nulo, este método é chamado diversas vezes
            // pelo Paginador Híbrido, acontece que quando usamos o AnonymousContext do SharePoint,
            // ele igual o contexto a nulo, neste caso, ocorre um erro de NullReferenceException
            // no Portal.
            if (!object.ReferenceEquals(HttpContext.Current, null)) {
                return HttpContext.Current.Server.UrlDecode(informacao);
            }
            return informacao;
        }

        /// <summary>
        /// Desmembra a URL inteira informada e remonta desprezando parâmetros que podem ser informados através do parâmetro funcaoExcecaoParametro
        /// </summary>
        /// <param name="funcaoExcecaoParametro"></param>
        /// <returns></returns>
        public static string ObterURLAtual(Predicate<string> funcaoExcecaoParametro)
        {
            string s = string.Empty;

            if (!object.ReferenceEquals(HttpContext.Current, null))
            {
                string url = HttpContext.Current.Request.Url.ToString();

                string[] fragmentos = url.Split('?'); //quebra por exemplo http://xpto.123.com?id=5 em duas partes: http://xpto.123.com e id=5
                bool contemParametros = fragmentos.GetLength(0) > 1 && !fragmentos[1].Trim().Equals(string.Empty); //verifica após a quebra se contém parâmetros(querystrings)

                sbURL = new StringBuilder(fragmentos[0]);

                if (contemParametros) {
                    sbURL.Append("?");

                    string[] paresChaveValor = fragmentos[1].Split('&'); //quebra por exemplo id=5&g=75 ==>> id=5 e g=75 

                    foreach (string parChaveValor in paresChaveValor) {
                        string[] separacaoChaveDoValor = parChaveValor.Split('='); //quebra por exemplo id=5  ==>> id e 5

                        //Executa a função de condição que , se verdadeiro, ignora a montagem da URL nesta iteração
                        //Neste caso, quando a chave-parâmetro é igual a algum nome informado na função, é ignorado
                        if (funcaoExcecaoParametro(separacaoChaveDoValor[0]))
                            continue;

                        string valorTratado = string.Empty;

                        if (separacaoChaveDoValor.GetLength(0) > 1)
                            valorTratado = URLUtils.URLEncode(separacaoChaveDoValor[1]);

                        sbURL.AppendFormat("{0}={1}&", separacaoChaveDoValor[0], valorTratado);
                    }
                }
                
            }
            
            if (!object.ReferenceEquals(sbURL, null))
                s = sbURL.ToString();

            return s;
        }

        /// <summary>
        /// Retorna um pattern para URL's
        /// </summary>
        public static string ExpressaoRegular_URL
        {
            get
            {
                return @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            }
        }

        /// <summary>
        /// Retorna se um dado URL é válido
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static bool URLValido(string URL)
        {
            return Regex.IsMatch(URL, URLUtils.ExpressaoRegular_URL);
        }
    }
}