using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using System.Xml;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe utilizada para aplicar estilos direto nos elementos html, dado um CSS.<br/>
    /// Utilizado principalmente para envio de e-mails, pois já envia o HTML com estilos inline.
    /// </summary>
    public class CSSInliner
    {
        #region [ Propriedades ]

        /// <summary>Objeto de configuração</summary>
        private CSSInlinerConfig Config { get; set; }

        #endregion

        #region [ Construtores ]
        
        /// <summary>Construtor padrão</summary>
        /// <param name="config">Configurações</param>
        public CSSInliner(CSSInlinerConfig config)
        {
            if (config == null)
                throw new Exception("CSSInlinerConfig não pode ser nulo.");

            this.Config = config;
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>Executa rotina que aplica estilos inline</summary>
        /// <param name="htmlControle">Conteúdo HTML</param>
        /// <returns>
        /// Conteúdo HTML processado, contendo o mesmo HTML de entrada, 
        /// porém com estilos inline aplicados aos elementos
        /// </returns>
        public String Processar(String htmlControle)
        {
            return Processar(new String[] { htmlControle }.ToList());
        }

        /// <summary>Executa rotina que aplica estilos inline</summary>
        /// <param name="htmlControles">Conteúdos HTML. Na saída, serão concatenados.</param>
        /// <returns>
        /// Conteúdo HTML processado, contendo o mesmo HTML de entrada, 
        /// porém com estilos inline aplicados aos elementos
        /// </returns>
        public String Processar(List<String> htmlControles)
        {
            //Une os HTMLs, adicionando o separador HTML entre os controles
            String html = String.Join(Config.SeparadorHTML, htmlControles.ToArray());

            //Busca o CSS que será aplicado aos controles
            String css = Config.TextoCSS;

            //Lê o CSS e aplica as regras inline nos elementos
            html = ConverterParaEstiloEmLinha(html, css);

            //Se solicitado, converte URLs relativas para absolutas
            if (Config.ConverterURLsRelativas)
                html = ConverteParaURLsAbsolutas(html);

            //Se solicitado, remove as referências a arquivos CSS externos (<link type="text/css" rel="stylesheet"></link>)
            if (Config.RemoverReferenciasCss)
                html = RemoverReferenciasCssExterno(html);

            //Se solicitado, substitui controles por texto simples
            if (Config.SubstituirControlesPorTexto)
                html = ConverterControlesParaTexto(html);

            return html;
        }

        #endregion

        #region [ Conversão de CSS para Inline Style ]

        private static String ConverterParaEstiloEmLinha(String html, String stylesheet)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode rootNode = htmlDoc.DocumentNode;

            //limpa a folha de estilo
            stylesheet = Regex.Replace(stylesheet, @"[\r\n]", String.Empty); // remove newlines
            stylesheet = Regex.Replace(stylesheet, @"\s*(?!<\"")\/\*[^\*]+\*\/(?!\"")\s*", String.Empty); // remove comments

            //remove espaços desnecessários
            while (stylesheet.Contains("  "))
                stylesheet = stylesheet.Replace("  ", " ");

            //Ordena as regras de CSS de acordo com a especificidade
            MatchCollection cssRules = Regex.Matches(stylesheet, "([^{]*){([^}]*)}", RegexOptions.Singleline);
            List<Tuple<CSSSpecificity, String, String>> cssRulesList = new List<Tuple<CSSSpecificity, String, String>>();
            foreach (Match cssRule in cssRules)
            {
                String cssProperties = cssRule.Groups[2].Value.Trim();
                String[] cssSelectors = cssRule.Groups[1].Value.Split(',');
                foreach (String selector in cssSelectors)
                    cssRulesList.Add(new Tuple<CSSSpecificity, String, String>(
                        CSSSpecificity.Calculate(selector), selector, cssProperties));
            }
            cssRulesList = cssRulesList.OrderByDescending(cssRule => cssRule.Item1).ToList();
            
            //Itera sobre as regras CSS e aplica estilos em linha
            foreach (var cssRule in cssRulesList)
            {
                try
                {
                    String xpath = CSS2XPath.Transform(cssRule.Item2.Trim());
                    HtmlNodeCollection matchingNodes = rootNode.SelectNodes(xpath);
                    if (matchingNodes == null) continue;
                    foreach (HtmlNode node in matchingNodes)
                    {
                        //adiciona o estilo ao elemento
                        if (node.Attributes["style"] != null)
                            node.Attributes["style"].Value = cssRule.Item3 + ";" + node.Attributes["style"].Value;
                        else
                            node.Attributes.Add("style", cssRule.Item3);
                    }
                }
                catch (Exception ex) {
                    Logger.GravarErro("Erro CSS Inliner: " + cssRule.Item2, ex);
                }
            }
            return htmlDoc.DocumentNode.OuterHtml;
        }

        #endregion

        #region [ Conversão de Controles para Texto Simples ]

        private String ConverterControlesParaTexto(String html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode rootNode = htmlDoc.DocumentNode;


            String xpath = CSS2XPath.Transform("input[type=text][value]");
            HtmlNodeCollection matchingNodes = rootNode.SelectNodes(xpath);
            if (matchingNodes != null)
            {
                foreach (HtmlNode node in matchingNodes)
                {
                    node.Name = "span";
                    node.InnerHtml = node.Attributes["value"].Value;
                }
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }

        #endregion

        #region [ Conversão para URLs absolutas ]

        /// <summary>
        /// Converte as URLs relativas do conteúdo do HTML para URL absoluta
        /// </summary>
        /// <param name="html">HTML</param>
        /// <returns>HTML</returns>
        public static String ConverteParaURLsAbsolutas(String html)
        {
            if (String.IsNullOrEmpty(html))
                return html;

            const String htmlPattern = "(?<attrib>\\shref|\\ssrc|\\sbackground)\\s*?=\\s*?"
                                        + "(?<delim1>[\"'\\\\]{0,2})(?!#|http|ftp|mailto|javascript)"
                                        + "/(?<url>[^\"'>\\\\]+)(?<delim2>[\"'\\\\]{0,2})";
            const String cssPattern = "@import\\s+?(url)*['\"(]{1,2}"
                                        + "(?!http)\\s*/(?<url>[^\"')]+)['\")]{1,2}";

            var htmlRegex = new Regex(htmlPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = htmlRegex.Replace(html,
                m => htmlRegex.Replace(m.Value, "${attrib}=${delim1}" + ObterURLAbsoluta("~/" + m.Groups["url"].Value) + "${delim2}"));
            
            var cssRegex = new Regex(cssPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = cssRegex.Replace(html, m => cssRegex.Replace(m.Value, "@import url(" + ObterURLAbsoluta("~/" + m.Groups["url"].Value) + ")"));

            return html;
        }
        
        
        /// <summary>
        /// Extrai o caminho dos arquivos CSS do conteúdo HTML
        /// </summary>
        /// <param name="html">Conteúdo HTML</param>
        /// <returns>Lista de arquivos CSS</returns>
        public static List<String> RecuperarEstilosCss(String html)
        {
            List<String> arquivosCss = new List<String>();

            if (!String.IsNullOrEmpty(html))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlNode rootNode = htmlDoc.DocumentNode;

                HtmlNodeCollection nodesCss = rootNode.SelectNodes("//*/link[@rel='stylesheet'][@type='text/css'][@href]");
                if (nodesCss != null)
                    foreach (HtmlNode nodeCss in nodesCss)
                        arquivosCss.Add(nodeCss.Attributes["href"].Value.ToString());
            }
            return arquivosCss;
        }

        /// <summary>
        /// Se solicitado, remove as referências a arquivos 
        /// CSS externos (<link type="text/css" rel="stylesheet"></link>)
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static String RemoverReferenciasCssExterno(String html)
        {            
            if (!String.IsNullOrEmpty(html))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlNode rootNode = htmlDoc.DocumentNode;

                HtmlNodeCollection nodesCss = rootNode.SelectNodes("//*/link[@rel='stylesheet'][@type='text/css'][@href]");
                if (nodesCss != null)
                    foreach (HtmlNode nodeCss in nodesCss)
                        nodeCss.Remove();

                return rootNode.OuterHtml;
            }
            return html;
        }

        /// <summary>
        /// Obtém a URL absoluta
        /// </summary>
        /// <param name="relativeUrl">URL relativa</param>
        /// <returns>URL absoluta</returns>
        public static String ObterURLAbsoluta(String relativeUrl)
        {
            if (String.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }

        /// <summary>
        /// Prepara o HTML, substituindo imagens que estão em "_layouts" com "CID"
        /// </summary>
        /// <param name="html">HTML a ser processado</param>
        /// <returns>Dicionário contendo o CID e a URL completa</returns>
        public static Dictionary<String, String> PrepararImagensEmbutidas(ref String html)
        {
            var retorno = new Dictionary<String, String>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]");

            if (imgNodes != null)
            {
                foreach (var item in imgNodes)
                {
                    String imgSRC = item.Attributes["src"].Value;
                    String imgID = HttpUtility.UrlDecode(Path.GetFileName(imgSRC));

                    if (imgSRC.ToLower().Contains("_layouts"))
                    {
                        if (!retorno.ContainsKey(imgID))
                            retorno.Add(imgID, imgSRC);
                        item.Attributes["src"].Value = String.Format("cid:{0}", imgID.Replace(' ', '_'));
                    }
                }
            }
            html = doc.DocumentNode.OuterHtml;
            return retorno;
        }

        #endregion

        /// <summary>
        /// Embute as imagens como LinkedResource no e-mail
        /// </summary>        
        public static void EmbutirImagens(MailMessage mail)
        {
            String htmlContent = mail.Body;
            var cids = CSSInliner.PrepararImagensEmbutidas(ref htmlContent);
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);

            foreach (var cid in cids)
            {
                try
                {
                    Int32 indexOfLayouts = cid.Value.ToLower().IndexOf("_layouts");
                    String relativePath = cid.Value.Substring(indexOfLayouts + "_layouts".Length + 1);
                    byte[] imgData = ObterRecursoLayouts(relativePath);
                    MemoryStream imgStream = new MemoryStream(imgData);
                    LinkedResource imgResource = new LinkedResource(imgStream, MediaTypeNames.Image.Jpeg);
                    imgResource.ContentId = cid.Key.Replace(' ', '_');
                    altView.LinkedResources.Add(imgResource);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante leitura de imagem para envio de e-mail: " + cid.Key + ";" + cid.Value, ex);
                }
            }
            if (altView.LinkedResources.Count != 0)
                mail.AlternateViews.Add(altView);
        }

        /// <summary>
        /// Obtém o conteúdo de um arquivo físico dentro da pasta LAYOUTS.
        /// Ex: Se o arquivo desejado é "TEMPLATE\LAYOUTS\Redecard.Comum\IMAGES\arquivo.png",
        /// relativeFilePath deve ser "Redecard.Comum\Images\arquivo.png".
        /// </summary>                
        public static byte[] ObterRecursoLayouts(String relativeFilePath)
        {            
            String relativePath = String.Format(@"TEMPLATE\LAYOUTS\" + relativeFilePath);
            String imageFilePath = SPUtility.GetGenericSetupPath(relativePath.Replace('/', '\\'));
            byte[] imgData = File.ReadAllBytes(imageFilePath);
            return imgData;
        }

        /// <summary>
        /// Obtém o conteúdo de um arquivo físico dentro da pasta LAYOUTS.
        /// Ex: Se o arquivo desejado é "TEMPLATE\LAYOUTS\Redecard.Comum\arquivo.txt",
        /// relativeFilePath deve ser "Redecard.Comum\arquivo.txt".
        /// </summary>                
        public static String ObterConteudoLayouts(String relativeFilePath)
        {
            String relativePath = String.Format(@"TEMPLATE\LAYOUTS\" + relativeFilePath);
            String filePath = SPUtility.GetGenericSetupPath(relativePath.Replace('/', '\\'));
            String content = File.ReadAllText(filePath);
            return content;
        }
        
    }

    /// <summary>
    /// Configuração do CSS Inliner
    /// </summary>
    public class CSSInlinerConfig
    {
        public String SeparadorHTML { get; set; }
        public Boolean ConverterURLsRelativas { get; set; }
        public Boolean SubstituirControlesPorTexto { get; set; }
        public String TextoCSS { get; set; }
        public Boolean RemoverReferenciasCss { get; set; }

        public CSSInlinerConfig()
        {
            this.SeparadorHTML = "<br/>";
            this.ConverterURLsRelativas = true;
            this.SubstituirControlesPorTexto = false;
            this.TextoCSS = null;
        }
    }

    /// <summary>Implementação da classe Tuple para 3 elementos</summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    internal class Tuple<T1, T2, T3>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public Tuple(T1 Item1, T2 Item2, T3 Item3)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
        }
    }

    internal class CSSSpecificity : IComparable
    {
        /**** Referências
         * http://css-specificity.webapp-prototypes.appspot.com/
         * http://css-tricks.com/specifics-on-css-specificity/
         * http://specificity.keegan.st/
         ****/

        #region [ Propriedades ]

        /// <summary>Estilos em linha</summary>
        public Int32 A { get; set; }
        /// <summary>IDs</summary>
        public Int32 B { get; set; }
        /// <summary>Classes, Atributos e Pseudo-Classes</summary>
        public Int32 C { get; set; }
        /// <summary>Elementos e Pseudo-Elementos</summary>
        public Int32 D { get; set; }
        /// <summary>Seletor</summary>
        public String Seletor { get; set; }

        #endregion

        #region [ Constantes - Regex ]

        private const String attributeRegex = @"(\[[^\]]+\])";
        private const String idRegex = @"(#[^\s\+>~\.\[:]+)";
        private const String classRegex = @"(\.[^\s\+>~\.\[:]+)";
        private const String pseudoElementRegex = @"(::[^\s\+>~\.\[:]+|:first-line|:first-letter|:before|:after)";
        private const String pseudoClassRegex = @"(:[^\s\+>~\.\[:]+)";
        private const String elementRegex = @"([^\s\+>~\.\[:]+)";
        private const String notRegex = @":not\(([^\)]*)\)";

        #endregion

        #region [ Construtores ]

        public CSSSpecificity()
        {
            this.A = this.B = this.C = this.D = 0;
        }

        #endregion

        public static CSSSpecificity Calculate(String seletor)
        {
            String _seletor = seletor;

            //Remove a pseudo-classe de negação (:not), mas mantém o argumento
            //pois a especificidade utiliza este argumento para cálculo            
            Regex regexNot = new Regex(notRegex);
            if (regexNot.IsMatch(_seletor))
                foreach (Match match in regexNot.Matches(_seletor))
                    _seletor = _seletor.Replace(match.Value, match.Groups[1].Value);

            Int32 a = 0, b = 0, c = 0, d = 0;

            FindMatch(ref _seletor, attributeRegex, ref c);
            FindMatch(ref _seletor, idRegex, ref b);
            FindMatch(ref _seletor, classRegex, ref c);
            FindMatch(ref _seletor, pseudoElementRegex, ref d);
            FindMatch(ref _seletor, pseudoClassRegex, ref c);

            //Remove seletor universal e separadores
            _seletor = _seletor.Replace("*", " ").Replace("+", " ").Replace(">", " ").Replace("~", " ");

            //Remove pontos e cerquilhas que não possuem palavras            
            _seletor = _seletor.Replace("#", " ").Replace(".", " ");

            //Só restam os seletores de elementos
            FindMatch(ref _seletor, elementRegex, ref d);

            //Retorno
            return new CSSSpecificity
            {
                A = a,
                B = b,
                C = c,
                D = d,
                Seletor = seletor
            };
        }

        private static void FindMatch(ref String seletor, String strRegex, ref Int32 type)
        {
            Regex regex = new Regex(strRegex);
            if (regex.IsMatch(seletor))
            {
                var matches = regex.Matches(seletor);
                type += matches.Count;
                foreach (Match match in matches)
                {
                    //Substitui para não ser contado novamente
                    seletor = regex.Replace(seletor, String.Join(" ", new String[match.Length].ToArray()));
                }
            }
        }

        #region [ Comparadores e Sobrescritos ]

        public override string ToString()
        {
            return String.Format("{0} [{1},{2},{3},{4}]", this.Seletor, this.A, this.B, this.C, this.D);
        }

        public override bool Equals(object obj)
        {
            return obj is CSSSpecificity && this == (CSSSpecificity)obj && this.Seletor == ((CSSSpecificity)obj).Seletor;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Boolean operator ==(CSSSpecificity a, CSSSpecificity b)
        {
            return a.A == b.A && a.B == b.B && a.C == b.C && a.D == b.D;
        }

        public static Boolean operator !=(CSSSpecificity a, CSSSpecificity b)
        {
            return a.A != b.A || a.B != b.B || a.C != b.C || a.D != b.D;
        }

        public static Boolean operator >(CSSSpecificity a, CSSSpecificity b)
        {
            if (a.A > b.A) return true;
            else if (a.A == b.A)
                if (a.B > b.B) return true;
                else if (a.B == b.B)
                    if (a.C > b.C) return true;
                    else if (a.C == b.C)
                        if (a.D > b.D) return true;
                        else return false;
                    else return false;
                else return false;
            else return false;
        }

        public static Boolean operator <(CSSSpecificity a, CSSSpecificity b)
        {
            if (a.A < b.A) return true;
            else if (a.A == b.A)
                if (a.B < b.B) return true;
                else if (a.B == b.B)
                    if (a.C < b.C) return true;
                    else if (a.C == b.C)
                        if (a.D < b.D) return true;
                        else return false;
                    else return false;
                else return false;
            else return false;
        }

        public static Boolean operator >=(CSSSpecificity a, CSSSpecificity b)
        {
            if (a.A > b.A) return true;
            else if (a.A == b.A)
                if (a.B > b.B) return true;
                else if (a.B == b.B)
                    if (a.C > b.C) return true;
                    else if (a.C == b.C)
                        if (a.D >= b.D) return true;
                        else return false;
                    else return false;
                else return false;
            else return false;
        }

        public static Boolean operator <=(CSSSpecificity a, CSSSpecificity b)
        {
            if (a.A < b.A) return true;
            else if (a.A == b.A)
                if (a.B < b.B) return true;
                else if (a.B == b.B)
                    if (a.C < b.C) return true;
                    else if (a.C == b.C)
                        if (a.D <= b.D) return true;
                        else return false;
                    else return false;
                else return false;
            else return false;
        }

        #endregion

        public Int32 CompareTo(object obj)
        {
            CSSSpecificity a = this;
            CSSSpecificity b = (CSSSpecificity)obj;
            if (a > b) return 1;
            else if (a == b) return 0;
            else return -1;
        }
    }

    /// <summary>
    /// A static utility class for transforming CSS selectors to XPath selectors.
    /// </summary>
    /// <remarks>
    /// * C# port of css2xpath (JavaScript)    
    /// * Version 1.0
    /// * 
    /// * Original version by Andrea Giammarchi (http://webreflection.blogspot.com/2009/04/vice-versa-sub-project-css2xpath.html)
    /// * Original covered under 'MIT License'
    /// * 
    /// * [License for this port]:
    /// * 
    /// * The MIT License
    ///
    ///  Copyright (c) MostThingsWeb (http://mosttw.wordpress.com/)
    ///      
    ///  Permission is hereby granted, free of charge, to any person obtaining a copy
    ///  of this software and associated documentation files (the "Software"), to deal
    ///  in the Software without restriction, including without limitation the rights
    ///  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ///  copies of the Software, and to permit persons to whom the Software is
    ///  furnished to do so, subject to the following conditions:
    ///      
    ///  The above copyright notice and this permission notice shall be included in
    ///  all copies or substantial portions of the Software.
    /// </remarks>
    internal class CSS2XPath
    {
        private static List<Regex> patterns;
        private static List<Object> replacements;

        static CSS2XPath()
        {
            // Initalize list of patterns and replacements
            patterns = new List<Regex>();
            replacements = new List<Object>();

            // Generate all the rules

            // Attributes
            AddRule(new Regex(@"\[([^\]~\$\*\^\|\!]+)(=[^\]]+)?\]"), "[@$1$2]");

            // Multiple queries
            AddRule(new Regex(@"\s*,\s*"), "|");

            // Remove space around +, ~, and >
            AddRule(new Regex(@"\s*(\+|~|>)\s*"), "$1");

            //Handle *, ~, +, and >
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])~([a-zA-Z0-9_\-\*])"), "$1/following-sibling::$2");
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])\+([a-zA-Z0-9_\-\*])"), "$1/following-sibling::*[1]/self::$2");
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])>([a-zA-Z0-9_\-\*])"), "$1/$2");

            // Escaping
            AddRule(new Regex(@"\[([^=]+)=([^'|" + "\"" + @"][^\]]*)\]"), "[$1='$2']");

            // All descendant or self to //
            AddRule(new Regex(@"(^|[^a-zA-Z0-9_\-\*])(#|\.)([a-zA-Z0-9_\-]+)"), "$1*$2$3");
            AddRule(new Regex(@"([\>\+\|\~\,\s])([a-zA-Z\*]+)"), "$1//$2");
            AddRule(new Regex(@"\s+\/\/"), "//");

            // Handle :first-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):first-child"), "*[1]/self::$1");

            // Handle :last-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):last-child"), "$1[not(following-sibling::*)]");

            // Handle :only-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):only-child"), "*[last()=1]/self::$1");

            // Handle :empty
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):empty"), "$1[not(*) and not(normalize-space())]");

            // Handle :not
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):not\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                return m.Groups[1].Value + "[not(" + (new Regex("^[^\\[]+\\[([^\\]]*)\\].*$")).Replace(Transform(m.Groups[2].Value), "$1") + ")]";
            }));

            // Handle :nth-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):nth-child\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                String b = m.Groups[2].Value;
                String a = m.Groups[1].Value;

                switch (b)
                {
                    case "n":
                        return a;
                    case "even":
                        return "*[position() mod 2=0 and position()>=0]/self::" + a;
                    case "odd":
                        return a + "[(count(preceding-sibling::*) + 1) mod 2=1]";
                    default:
                        // Parse out the 'n'
                        b = ((new Regex("^([0-9])*n.*?([0-9])*$")).Replace(b, "$1+$2"));

                        // Explode on + (i.e 'nth-child(2n+0)' )
                        String[] b2 = new String[2];
                        String[] splitResult = b.Split('+');

                        // The first component will always be a number
                        b2[0] = splitResult[0];

                        int buffer = 0;

                        // The second component might be missing
                        if (splitResult.Length == 2)
                            if (!int.TryParse(splitResult[1], out buffer))
                                buffer = 0;

                        b2[1] = buffer.ToString();

                        return "*[(position()-" + b2[1] + ") mod " + b2[0] + "=0 and position()>=" + b2[1] + "]/self::" + a;
                }
            }));

            // Handle :contains
            AddRule(new Regex(@":contains\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                return "[contains(string(.),'" + m.Groups[1].Value + "')]";
            }));

            // != attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\|=([^\]]+)\]"), "[@$1=$2 or starts-with(@$1,concat($2,'-'))]");

            // *= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\*=([^\]]+)\]"), "[contains(@$1,$2)]");

            // ~= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)~=([^\]]+)\]"), "[contains(concat(' ',normalize-space(@$1),' '),concat(' ',$2,' '))]");

            // ^= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\^=([^\]]+)\]"), "[starts-with(@$1,$2)]");

            // $= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\$=([^\]]+)\]"), new MatchEvaluator((Match m) =>
            {
                String a = m.Groups[1].Value;
                String b = m.Groups[2].Value;
                return "[substring(@" + a + ",string-length(@" + a + ")-" + (b.Length - 3) + ")=" + b + "]";
            }));

            // != attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\!=([^\]]+)\]"), "[not(@$1) or @$1!=$2]");

            // ID and class
            AddRule(new Regex(@"#([a-zA-Z0-9_\-]+)"), "[@id='$1']");
            AddRule(new Regex(@"\.([a-zA-Z0-9_\-]+)"), "[contains(concat(' ',normalize-space(@class),' '),' $1 ')]");

            // Normalize filters
            AddRule(new Regex(@"\]\[([^\]]+)"), " and ($1)");
        }

        /// <summary>
        /// Adds a rule for transforming CSS to XPath.
        /// </summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A MatchEvaluator for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null.</exception>
        /// <example>
        /// <code>
        /// // Handle :contains selectors
        /// AddRule(new Regex(@":contains\(([^\)]*)\)"), new MatchEvaluator((Match m) => {
        ///     return "[contains(string(.),'" + m.Groups[1].Value + "')]";
        /// }));
        /// 
        /// // Note: Remember that m.Groups[1] refers to the first captured group; m.Groups[0] refers
        /// // to the entire match.
        /// </code>
        /// </example>
        private static void AddRule(Regex regex, MatchEvaluator replacement)
        {
            _AddRule(regex, replacement);
        }

        /// <summary>Adds a rule for transforming CSS to XPath.</summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A String for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null.</exception>
        /// <example>
        /// <code>
        /// // Replace commas (denotes multiple queries) with pipes (|)
        /// AddRule(new Regex(@"\s*,\s*"), "|");
        /// </code>
        /// </example>
        private static void AddRule(Regex regex, String replacement)
        {
            _AddRule(regex, replacement);
        }

        /// <summary>Adds a rule for transforming CSS to XPath. For internal use only.</summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A String or MatchEvaluator for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null, or if the replacement is neither a String nor a MatchEvaluator.</exception>
        private static void _AddRule(Regex regex, Object replacement)
        {
            if (regex == null)
                throw new ArgumentException("Must supply non-null Regex.", "regex");

            if (replacement == null || (!(replacement is String) && !(replacement is MatchEvaluator)))
                throw new ArgumentException("Must supply non-null replacement (either String or MatchEvaluator).", "replacement");

            patterns.Add(regex);
            replacements.Add(replacement);
        }

        /// <summary>Transforms the given CSS selector to an XPath selector.</summary>
        /// <param name="css">The CSS selector to transform into an XPath selector.</param>
        /// <returns>The resultant XPath selector.</returns>
        public static String Transform(String css)
        {
            int len = patterns.Count;

            for (int i = 0; i < len; i++)
            {
                Regex pattern = patterns[i];
                Object replacement = replacements[i];

                // Depending on what the replacement is, we need to cast it to either a String or a MatchEvaluator
                if (replacement is String)
                    css = pattern.Replace(css, (String)replacement);
                else
                    css = pattern.Replace(css, (MatchEvaluator)replacement);
            }

            return "//" + css;
        }

        /// <summary>
        /// Forces the CSS to XPath rules to be created. Not neccesary; the rules are created the first time Transform is called.
        /// </summary>
        /// <remarks>
        /// Perhaps you would want to use this in the initalization procedure of your application.
        /// </remarks>
        private static void PreloadRules()
        {
            //Empty by design:            
            //The static class initializer will be called the first time any static method, such
            //as this one, is called           
        }
    }
}
