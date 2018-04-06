using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe auxiliar para geração de CSV
    /// </summary>
    public abstract class CSVExporter
    {
        #region [ HTML para CSV ]

        /// <summary>
        /// Gera conteúdo binário CSV a partir de uma tabela HTML (table.dadosTabela.dados)
        /// </summary>
        /// <param name="htmlTabela">HTML da tabela</param>
        /// <param name="encoding">Codificação a ser utilizada na geração do conteúdo binário</param>
        /// <returns>Conteúdo CSV binário</returns>
        public static byte[] GerarCSVBinario(String htmlTabela, Encoding encoding)
        {
            String csv = GerarCSV(htmlTabela);
            return encoding.GetBytes(csv);
        }

        /// <summary>
        /// Gera o conteúdo CSV a partir de uma tabela HTML simples: table -> tr -> td|th.<br/>
        /// Considera o atributo "colspan" para geração do arquivo.
        /// </summary>
        /// <param name="htmlTabela">HTML da tabela</param>
        /// <returns>Conteúdo CSV</returns>
        public static String GerarCSV(String htmlTabela)
        {
            return GerarCSV(htmlTabela, ",");
        }

        /// <summary>
        /// Gera o conteúdo CSV a partir de uma tabela HTML simples: table -> tr -> td|th.<br/>
        /// Considera o atributo "colspan" para geração do arquivo.
        /// </summary>
        /// <param name="htmlTabela">HTML da tabela</param>
        /// <param name="delimiter">Delimitador CSV</param>
        /// <param name="encoding">Codificação a ser utilizada na geração do conteúdo binário</param>
        /// <returns>Conteúdo CSV binário</returns>
        public static Byte[] GerarCSVBinario(String htmlTabela, String delimiter, Encoding encoding)
        {
            String csv = GerarCSV(htmlTabela, delimiter);
            return encoding.GetBytes(csv);
        }

        /// <summary>
        /// Gera o conteúdo CSV a partir de uma tabela HTML simples: table -> tr -> td|th.<br/>
        /// Considera o atributo "colspan" para geração do arquivo.
        /// </summary>
        /// <param name="htmlTabela">HTML da tabela</param>
        /// <param name="delimiter">Delimitador CSV</param>
        /// <returns>Conteúdo CSV</returns>
        public static String GerarCSV(String htmlTabela, String delimiter)
        {
            //Carrega o HTML para processamento
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlTabela);

            //Node
            HtmlNode rootNode = htmlDoc.DocumentNode;

            //Monta seletor de tabelas (table)
            String xpathClass = "//table[contains(concat(' ', normalize-space(@class), ' '), ' tabelaDados ')][contains(concat(' ', normalize-space(@class), ' '), ' dados ')]";

            //Seleciona as tabelas
            HtmlNodeCollection tables = rootNode.SelectNodes(xpathClass);
            if (tables == null) return String.Empty;

            //StringBuilder para montagem do conteúdo CSV
            StringBuilder csv = new StringBuilder();

            //Percorre as tabelas encontradas
            foreach (HtmlNode table in tables)
            {
                //Busca as linhas da tabela (tr)
                HtmlNodeCollection rows = table.SelectNodes(".//tr");
                if (rows == null) continue;

                //Percorre as linhas da tabela
                foreach (HtmlNode row in rows)
                {
                    //Busca as colunas da linha
                    var columns = row.SelectNodes(".//td | .//th");
                    if (columns == null) continue;

                    //Percorre as colunas da tabela
                    foreach (HtmlNode column in columns)
                    {
                        Int32 colSpan = 1;

                        //Substitui as tags br por espaço
                        HtmlNodeCollection breakLines = column.SelectNodes(".//br");
                        if (breakLines != null)
                            foreach (HtmlNode breakLine in breakLines)
                                breakLine.InnerHtml = " ";

                        //Recupera dados da célula
                        String conteudo = FormatarValor(column.InnerText, delimiter);
                        if (column.Attributes["colSpan"] != null)
                            colSpan = Convert.ToInt32(column.Attributes["colspan"].Value);

                        csv.Append(conteudo);
                        csv.Append(delimiter);
                        for (Int32 iSpan = 1; iSpan < colSpan; iSpan++)
                            csv.Append(delimiter);
                    }
                    csv.AppendLine();
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }

        #endregion

        #region [ Objeto para CSV ]

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        public static String GerarCSV<T>(IEnumerable<T> registros, Func<T, List<String>> funcaoRegistros)
        {
            return GerarCSV<T>(registros, funcaoRegistros, ",");
        }

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        /// <typeparam name="T">Tipo do registro</typeparam>
        /// <param name="registros">Registros</param>
        /// <param name="funcaoRegistros">Função que obtém os registros</param>
        /// <param name="encoding">Encoding</param>
        /// <returns></returns>
        public static Byte[] GerarCSV<T>(IEnumerable<T> registros, Func<T, List<String>> funcaoRegistros, Encoding encoding)
        {
            String csv = GerarCSV<T>(registros, funcaoRegistros);
            return encoding.GetBytes(csv);
        }

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        /// <typeparam name="T">Tipo do registro</typeparam>
        /// <param name="delimiter">Delimitador</param>
        /// <param name="funcaoRegistros">Função que obtém os registros</param>
        /// <param name="registros">Registros</param>
        /// <returns></returns>
        public static String GerarCSV<T>(IEnumerable<T> registros, Func<T, List<String>> funcaoRegistros, String delimiter)
        {
            return GerarCSV<T>(registros, null, funcaoRegistros, delimiter);
        }

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        /// <typeparam name="T">Tipo do registro</typeparam>
        /// <param name="delimiter">Delimitador</param>
        /// <param name="funcaoRegistros">Função que obtém os registros</param>
        /// <param name="registros">Registros</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>CSV binário</returns>
        public static Byte[] GerarCSV<T>(IEnumerable<T> registros, Func<T, List<String>> funcaoRegistros, String delimiter, Encoding encoding)
        {
            String csv = GerarCSV<T>(registros, funcaoRegistros, delimiter);
            return encoding.GetBytes(csv);
        }

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        /// <typeparam name="T">Tipo do registro</typeparam>
        /// <param name="colunas">Colunas</param>
        /// <param name="delimiter">Delimitador</param>
        /// <param name="funcaoRegistros">Função que obtém os registros</param>
        /// <param name="registros">Registros</param>
        /// <returns>CSV</returns>
        public static String GerarCSV<T>(IEnumerable<T> registros, List<String> colunas, 
            Func<T, List<String>> funcaoRegistros, String delimiter)
        {
            StringBuilder csv = new StringBuilder();

            if (colunas != null)
            {
                foreach (String col in colunas)
                {
                    csv.Append(FormatarValor(col, delimiter));
                    csv.Append(delimiter);
                }
                csv.AppendLine();
            }

            foreach (T registro in registros)
            {
                List<String> valores = funcaoRegistros(registro);

                foreach (String valor in valores)
                {
                    csv.Append(FormatarValor(valor, delimiter));
                    csv.Append(delimiter);
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }

        /// <summary>
        /// Gera CSV a partir de um objeto e function customizada
        /// </summary>
        /// <typeparam name="T">Tipo do registro</typeparam>
        /// <param name="colunas">Colunas</param>
        /// <param name="delimiter">Delimitador</param>
        /// <param name="funcaoRegistros">Função que obtém os registros</param>
        /// <param name="registros">Registros</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>CSV binário</returns>
        public static Byte[] GerarCSV<T>(IEnumerable<T> registros, List<String> colunas, 
            Func<T, List<String>> funcaoRegistros, String delimiter, Encoding encoding)
        {
            String csv = GerarCSV<T>(registros, colunas, funcaoRegistros, delimiter);
            return encoding.GetBytes(csv);
        }

        #endregion

        /// <summary>
        /// Formata o valor
        /// </summary>
        private static String FormatarValor(String valor, String delimiter)
        {
            if (String.IsNullOrEmpty(valor)) 
                return String.Empty;

            String output = valor;

            //Trata vírgulas e aspas duplas
            if (output.Contains(delimiter) || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            //Remove tabulações, quebras de linha
            output = Regex.Replace(output, @"\t|\n|\r", "");

            //Remove espaços em branco extras
            output = Regex.Replace(output, @"\s+", " ");

            return output.Trim();
        }
    }
}
