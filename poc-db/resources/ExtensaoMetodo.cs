#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [02/05/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Xml;
using System.Text;
using System.ComponentModel;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe genérica contendo extensões de método.
    /// </summary>
    public static class ExtensaoMetodo
    {
        #region [ Métodos Públicos ]
        /// <summary>Cria uma cópia e retorna</summary>
        /// <typeparam name="T">Tipo do objeto que será copiado</typeparam>
        /// <param name="objetoFonte">Objeto que será copiado</param>
        /// <returns>Cópia do objeto fonte</returns>
        public static T ObterCopia<T>(this T objetoFonte)
        {
            //Cria o objeto de destino
            T objetoDestino = (T)Activator.CreateInstance(objetoFonte.GetType());

            //Efetua a cópia dos dados
            Util.CopiarObjeto(objetoFonte, objetoDestino);

            //Retorna o objeto
            return objetoDestino;
        }

        /// <summary>
        /// Remove os caracteres à direita da String que excedem 
        /// o tamanho especificado, da esquerda para a direita.
        /// </summary>
        /// <param name="texto_">String</param>
        /// <param name="tamanho_">Tamanho máximo da String</param>
        /// <returns>String</returns>
        public static string Left(this string texto_, int tamanho_)
        {
            return texto_.Substring(0, tamanho_ > texto_.Length ? texto_.Length : tamanho_);
        }

        /// <summary>
        /// Remove os caracteres à esquerda da String que excedem 
        /// o tamanho especificado, da direita para a esquerda.
        /// </summary>
        /// <param name="texto_">String</param>
        /// <param name="tamanho_">Tamanho máximo da String</param>
        /// <returns>String</returns>
        [Obsolete("Método está incorreto! Utilizar ")]
        public static string Right(this string texto_, int tamanho_)
        {
            return texto_.Substring(tamanho_ > texto_.Length ? texto_.Length : texto_.Length - tamanho_);
        }

        /// <summary>
        /// Remove os caracteres à esquerda da String que excedem 
        /// o tamanho especificado, da direita para a esquerda.
        /// Equivalente à função Right.
        /// </summary>
        /// <param name="texto_">String</param>
        /// <param name="tamanho_">Tamanho máximo da String</param>
        /// <returns>String</returns>
        public static String TruncateLeft(this String texto, int tamanho)
        {
            return texto.Substring(tamanho > texto.Length ? 0 : texto.Length - tamanho);
        }

        /// <summary>
        /// Remove os caracteres à direita da String que excedem 
        /// o tamanho especificado, da esquerda para a direita.
        /// Equivalente à função Left
        /// </summary>
        /// <param name="texto">String</param>
        /// <param name="tamanho">Tamanho máximo da String</param>
        /// <returns>String</returns>
        public static String TruncateRight(this String texto, Int32 tamanho)
        {
            return texto.Substring(0, tamanho > texto.Length ? texto.Length : tamanho);
        }


        /// <summary>Replica a string</summary>
        /// <param name="texto_">Texto</param>
        /// <param name="tamanho_">Quantidade de vezes</param>
        /// <returns>String</returns>
        public static string Replicate(this string texto_, int qtd_)
        {
            StringBuilder retorno = new StringBuilder();
            for (int i = 0; i < qtd_; i++)
                retorno.Append(texto_);
            return retorno.ToString();
        }

        /// <summary>
        /// Obtém o valor da tag de descrição do enumerador.
        /// </summary>
        /// <param name="value">Valor do Enumerador</param>
        /// <returns>Descrição do enumerador</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Obtém o valor da tag de descrição do enumerador.
        /// </summary>
        /// <param name="value">Valor do Enumerador</param>
        /// <param name="funcaoValor">Função que retornará o valor do Atributo</param>
        /// <typeparam name="TAttribute">Tipo do Atributo</typeparam>
        /// <typeparam name="TAttributeValue">Tipo do Valor do Atributo</typeparam>
        /// <returns>Descrição do enumerador</returns>        
        public static TAttributeValue GetDescription<TAttribute, TAttributeValue>(this Enum value, Func<TAttribute, TAttributeValue> funcaoValor) where TAttribute : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            TAttribute[] attributes = (TAttribute[])fi.GetCustomAttributes(typeof(TAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return funcaoValor(attributes[0]);
            else
                return default(TAttributeValue);
        }

        /// <summary>
        /// Obtém o enumerador pela descrição
        /// </summary>
        /// <param name="value">Descrição do enumerador</param>
        /// <typeparam name="T">Tipo do enumerador</typeparam>
        /// <returns>Enumerador tipado</returns>
        public static T GetEnumByDescription<T>(this String value)
        {            
            foreach(Enum enumerador in Enum.GetValues(typeof(T)))
            {
                if (enumerador.GetDescription() == value)
                    return (T) Enum.Parse(typeof(T), enumerador.ToString());
            }
            return default(T);
        }

        /// <summary>Converte string para número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static double ToDouble(this string numero_)
        {
            if (numero_ == null || conteudoVazio(numero_.ToString()))
                return 0;
            else
            {
                double retorno;

                //Verifica se o número é válido
                if (double.TryParse(numero_.ToString(), out retorno))
                    return (double)retorno;
                else
                    return 0;
            }
        }

        /// <summary>Converte string para número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static Int16? ToInt16Null(this string numero_, Int16? numeroPadrao_)
        {
            if (numero_ == null || conteudoVazio(numero_.ToString()))
                return numeroPadrao_;
            else
            {
                Int16 retorno;

                //Verifica se o número é válido
                if (Int16.TryParse(numero_.ToString(), out retorno))
                    return (Int16?)retorno;
                else
                    return numeroPadrao_;
            }
        }

        /// <summary>Converte para número que pode ser null</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna número ou null</returns>
        public static Int16? ToInt16Null(this string numero_)
        {
            return ToInt16Null(numero_, null);
        }

        /// <summary>Retorna número ou número padrão caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao">Número padrão de retorno caso o conteúdo não seja número</param>
        /// <returns>Retorna Número ou número padrão caso o conteúdo não seja número</returns>
        public static Int16 ToInt16(this string numero_, Int16 numeroPadrao)
        {
            return Convert.ToInt16(ToInt16Null(numero_, numeroPadrao));
        }

        /// <summary>Retorna número ou 0 caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna Número ou 0 caso o conteúdo não seja número</returns>
        public static Int16 ToInt16(this string numero_)
        {
            return Convert.ToInt16(ToInt16Null(numero_, 0));
        }

        /// <summary>Converte string para número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static int? ToInt32Null(this string numero_, int? numeroPadrao_)
        {
            if (numero_ == null || conteudoVazio(numero_.ToString()))   
                return numeroPadrao_;
            else
            {
                int retorno;

                //Verifica se o número é válido
                if (int.TryParse(numero_.ToString(), out retorno))
                    return (int?)retorno;
                else
                    return numeroPadrao_;
            }
        }

        /// <summary>Converte para número que pode ser null</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna número ou null</returns>
        public static int? ToInt32Null(this string numero_)
        {
            return ToInt32Null(numero_, null);
        }

        /// <summary>Retorna número ou número padrão caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao">Número padrão de retorno caso o conteúdo não seja número</param>
        /// <returns>Retorna Número ou número padrão caso o conteúdo não seja número</returns>
        public static int ToInt32(this string numero_, int numeroPadrao)
        {
            return Convert.ToInt32(ToInt32Null(numero_, numeroPadrao));
        }

        /// <summary>Retorna número ou 0 caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna Número ou 0 caso o conteúdo não seja número</returns>
        public static int ToInt32(this string numero_)
        {
            return Convert.ToInt32(ToInt32Null(numero_, 0));
        }

        /// <summary>Converte string para número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static Int64? ToInt64Null(this String numero_, Int64? numeroPadrao_)
        {
            if (numero_ == null || conteudoVazio(numero_.ToString()))
                return numeroPadrao_;
            else
            {
                Int64 retorno;

                //Verifica se o número é válido
                if (Int64.TryParse(numero_.ToString(), out retorno))
                    return (Int64?)retorno;
                else
                    return numeroPadrao_;
            }
        }

        /// <summary>Converte para número que pode ser null</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna número ou null</returns>
        public static Int64? ToInt64Null(this String numero_)
        {
            return ToInt64Null(numero_, null);
        }

        /// <summary>Retorna número ou número padrão caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <param name="numeroPadrao">Número padrão de retorno caso o conteúdo não seja número</param>
        /// <returns>Retorna Número ou número padrão caso o conteúdo não seja número</returns>
        public static Int64 ToInt64(this String numero_, Int64 numeroPadrao)
        {
            return Convert.ToInt64(ToInt64Null(numero_, numeroPadrao));
        }

        /// <summary>Retorna número ou 0 caso o conteúdo não seja número</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna Número ou 0 caso o conteúdo não seja número</returns>
        public static Int64 ToInt64(this string numero_)
        {
            return Convert.ToInt64(ToInt64Null(numero_, 0));
        }
    
        /// <summary>Caso seja um valor vazio, retorna null</summary>
        /// <param name="valor">valor</param>
        /// <returns>Retorno</returns>
        public static string EmptyToNull(this string valor)
        {
            if (valor == null || conteudoVazio(valor))
                return null;
            else
                return valor;
        }

        /// <summary>Converte a string (se válida) para Datetime</summary>
        /// <param name="valor">Data</param>
        /// <returns>Datetime</returns>
        public static DateTime? ToDateTimeNull(this string valor)
        {
            if (valor == null || conteudoVazio(valor))
                return null;
            else
            {
                DateTime dtRetorno;
                if (DateTime.TryParse(valor, out dtRetorno))
                    return dtRetorno;
                else
                    return null;
            }
        }

        /// <summary>Converte a string (se válida) para Datetime</summary>
        /// <param name="valor">Data</param>
        /// <returns>Datetime</returns>
        public static DateTime ToDate(this string valor)
        {
            if (valor == null || conteudoVazio(valor))
                return DateTime.MinValue;
            else
            {
                DateTime dtRetorno;
                if (DateTime.TryParse(valor, out dtRetorno))
                    return dtRetorno;
                else
                    return DateTime.MinValue;
            }
        }

        /// <summary>Converte a string (se válida) para Datetime</summary>
        /// <param name="valor">Data</param>
        /// <returns>Datetime</returns>
        public static DateTime ToDate(this string valor, string format)
        {
            if (valor == null || conteudoVazio(valor))
                return DateTime.MinValue;
            else
            {
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                DateTime dtRetorno;
                if (DateTime.TryParseExact(valor, format, provider, System.Globalization.DateTimeStyles.None, out dtRetorno))
                    return dtRetorno;
                else
                    return DateTime.MinValue;                
            }
        }

        /// <summary>Converte a string para DateTime</summary>
        /// <param name="valor">Data</param>
        /// <returns>Datetime</returns>
        public static DateTime? ToDateTimeNull(this string valor, string format)
        {
            if (valor == null || conteudoVazio(valor))
                return null;
            else
            {
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                DateTime dtRetorno;
                if (DateTime.TryParseExact(valor, format, provider, System.Globalization.DateTimeStyles.None, out dtRetorno))
                    return dtRetorno;
                else
                    return null;
            }
        }

        /// <summary>Converte a string para DateTime</summary>
        /// <param name="valor">Data</param>
        /// <returns>Datetime</returns>
        public static DateTime ToDate(this string valor, string format, DateTime valorPadrao)
        {
            if (valor == null || conteudoVazio(valor))
                return valorPadrao;
            else
            {
                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                DateTime dtRetorno;
                if (DateTime.TryParseExact(valor, format, provider, System.Globalization.DateTimeStyles.None, out dtRetorno))
                    return dtRetorno;
                else
                    return valorPadrao;
            }
        }

        /// <summary>Converte a string (se válida) para Decimal</summary>
        /// <param name="valor">Data</param>
        /// <returns>Decimal</returns>
        public static Decimal ToDecimal(this string valor)
        {
            return Convert.ToDecimal(ToDecimalNull(valor, 0));
        }

        /// <summary>Converte a string (se válida) para Decimal</summary>
        /// <param name="valor">Data</param>
        /// <returns>Decimal</returns>
        public static Decimal? ToDecimalNull(this string valor)
        {
            return ToDecimalNull(valor, null);
        }

        /// <summary>Converte a string (se válida) para Decimal</summary>
        /// <param name="valor">Data</param>
        /// <returns>Decimal</returns>
        public static Decimal? ToDecimalNull(this string valor, decimal? valorPadrao)
        {
            if (valor == null || conteudoVazio(valor))
                return valorPadrao;
            else
            {
                Decimal dtRetorno;
                if (Decimal.TryParse(valor, out dtRetorno))
                    return dtRetorno;
                else
                    return valorPadrao;
            }
        }

        /// <summary>Converte a string (se válida) para Bool</summary>
        /// <param name="valor">valor bool</param>
        /// <returns>Boleano</returns>
        public static bool? ToBoolNull(this string valor)
        {
            if (valor == null || conteudoVazio(valor))
                return null;
            else
            {
                bool retorno;
                if (bool.TryParse(valor, out retorno))
                    return retorno;
                else
                    return null;
            }
        }

        /// <summary>Converte a string (se válida) para Guid</summary>
        /// <param name="valor">valor guid</param>
        /// <returns>Guid</returns>
        public static Guid ToGuid(this string valor)
        {
            return ToGuidNull(valor, Guid.Empty).Value;
        }

        /// <summary>Converte a string (se válida) para Guid, c.c. null</summary>
        /// <param name="valor">valor guid</param>
        /// <returns>Guid</returns>
        public static Guid? ToGuidNull(this string valor)
        {
            return ToGuidNull(valor, null);
        }

        /// <summary>Converte a string (se válida) para Guid, c.c. null</summary>
        /// <param name="valor">valor guid</param>
        /// <param name="valorPadrao">valor padrão caso string não seja um guid válido</param>
        /// <returns>Guid nullable</returns>
        public static Guid? ToGuidNull(this string valor, Guid? valorPadrao)
        {
            if (valor == null || conteudoVazio(valor))
                return valorPadrao;
            else
            {
                try { return new Guid(valor); }
                catch { return valorPadrao; }                
            }
        }

        /// <summary>Verifica se o int32 é nulo, se for retorna string vazia</summary>
        /// <param name="numero_">Número</param>
        /// <returns>Retorna número ou string vazia</returns>
        public static string ToEmptyString(this int? numero_)
        {
            return (numero_ == null ? string.Empty : numero_.ToString());
        }

        /// <summary>Verifica se a string é composta só por números</summary>
        /// <param name="valor_">valor</param>
        /// <returns>Se é número</returns>
        public static bool IsNumber(this string valor_)
        {
            Regex re = new Regex(@"^\d+$");
            return re.IsMatch(valor_);
        }

        /// <summary>Formata a data a ser mostrada na tela</summary>
        /// <param name="data_">Data</param>
        /// <returns></returns>
        public static string FormatToDayMonthYear(this DateTime data_)
        {
            //Verifica se possui algum valor
            if (data_ != null)
                return data_.ToString("dd/MM/yyyy");
            else
                return "";
        }

        /// <summary>Formata a data a ser gravada na BD</summary>
        /// <param name="data_">Data</param>
        /// <returns></returns>
        public static DateTime FormatToYearMonthDay(this DateTime data_)
        {
            //Verifica se possui algum valor
            if (data_ != null)
                return Convert.ToDateTime(data_.ToString("yyyy/MM/dd"));
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// Recupera o texto de tempo passado.
        /// </summary>
        /// <param name="source">Data de fim da contagem</param>
        /// <param name="dataInicio">Data de início da contagem</param>
        /// <param name="round">Define se o valor apresentado deve ser aredondado para números inteiros.</param>
        /// <returns>Texto do tempo passado</returns>
        public static string GetTextFrom(this DateTime source, DateTime dataInicio, bool round)
        {
            return GetTimeSpanText(source.Subtract(dataInicio), round);
        }

        /// <summary>
        /// Recupera o texto de tempo passado.
        /// </summary>
        /// <param name="source">Data de início da contagem</param>
        /// <param name="dataFim">Data de fim da contagem</param>
        /// <param name="round">Define se o valor apresentado deve ser aredondado para números inteiros.</param>
        /// <returns>Texto do tempo passado</returns>
        public static string GetTextTo(this DateTime source, DateTime dataFim, bool round)
        {
            return GetTimeSpanText(dataFim.Subtract(source), round);
        }

        /// <summary>Implementa o String.Format</summary>
        /// <param name="texto_">Texto</param>
        /// <param name="args_">Parâmetros</param>
        /// <returns>String formatada</returns>
        public static string Format(this string texto_, params object[] args_)
        {
            return String.Format(texto_, args_);
        }

        /// <summary>
        /// Ordenação da coleção pelo nome da propriedade.
        /// </summary>
        /// <typeparam name="T">Tipo de dado da coleção</typeparam>
        /// <param name="items">Coleção de dados</param>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <returns>Coleção ordenada</returns>
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> items, string propertyName)
        {
            return OrderByPropertyName<T>(items, propertyName, true);
        }

        /// <summary>
        /// Ordenação descrescente da coleção pelo nome da propriedade.
        /// </summary>
        /// <typeparam name="T">Tipo de dado da coleção</typeparam>
        /// <param name="items">Coleção de dados</param>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <returns>Coleção ordenada</returns>
        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> items, string propertyName)
        {
            return OrderByPropertyName<T>(items, propertyName, false);
        }

        private static IEnumerable<T> OrderByPropertyName<T>(this IEnumerable<T> source, string propertyName, bool ascending)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            var Object = Expression.Parameter(typeof(T), "Object");
            var EnumeratedObject = Expression.Parameter(typeof(IEnumerable<T>), "EnumeratedObject");
            var Property = Expression.Property(Object, propertyName);
            var Lamda = Expression.Lambda(Property, Object);
            var Method = Expression.Call(typeof(Enumerable), ascending ? "OrderBy" : "OrderByDescending", new[] { typeof(T), Lamda.Body.Type }, EnumeratedObject, Lamda);
            var SortedLamda = Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(Method, EnumeratedObject).Compile();
            return SortedLamda(source);
        }

        /// <summary>
        /// Adiciona elemento ao XmlDocument
        /// </summary>
        /// <param name="doc">Documento Xml</param>
        /// <param name="elemento">Nome do elemento a ser adicionado</param>
        /// <param name="valor">Valor do elemento a ser adicionado</param>
        public static void AdicionarElemento(this XmlDocument doc, string elemento, string valor)
        {
            //Adiciona o iten dentro do root
            XmlNode nodeItem = doc.CreateElement(elemento);
            nodeItem.AppendChild(doc.CreateTextNode(valor));
            doc.ChildNodes[0].AppendChild(nodeItem);
        }

        /// <summary>
        /// Obtém o valor tipado de um dicionário.
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave do objeto no dicionário</param>
        /// <returns>Objeto tipado ou valor Default</returns>
        public static T GetValueOrDefault<T>(this Dictionary<String, Object> dictionary, String key)
        {
            Object result;            
            return dictionary != null ? (dictionary.TryGetValue(key, out result) ? (T)result : default(T)) : default(T);
        }

        /// <summary>
        /// Obtém o valor tipado de um dicionário.
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave do objeto no dicionário</param>
        /// <param name="defaultValue">Valor padrão</param>
        /// <returns>Objeto tipado ou valor Default</returns>
        public static T GetValueOrDefault<T>(this Dictionary<String, Object> dictionary, String key, T defaultValue)
        {
            Object result;
            return dictionary != null ? (dictionary.TryGetValue(key, out result) ? (T)result : defaultValue) : defaultValue;
        }

        /// <summary>
        /// Obtém o valor tipado de um dicionário.
        /// </summary>
        /// <typeparam name="TKey">Tipo de dado da chave do dicionário</typeparam>
        /// <typeparam name="TValue">Tipo de dado dos valores do dicionário</typeparam>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave do objeto no dicionário</param>
        /// <param name="defaultValue">Valor padrão</param>
        /// <returns>Objeto tipado ou valor Default</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue result;
            return dictionary != null ? (dictionary.TryGetValue(key, out result) ? (TValue)result : defaultValue) : defaultValue;
        }

        /// <summary>
        /// Obtém o valor tipado de um dicionário.
        /// </summary>
        /// <typeparam name="TKey">Tipo de dado da chave do dicionário</typeparam>
        /// <typeparam name="TValue">Tipo de dado dos valores do dicionário</typeparam>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave do objeto no dicionário</param>
        /// <returns>Objeto tipado ou valor Default</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;
            return dictionary != null ? (dictionary.TryGetValue(key, out result) ? (TValue)result : default(TValue)) : default(TValue);
        }

        /// <summary>
        /// Adicionar um valor no dicionário
        /// </summary>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave</param>
        /// <param name="value">Valor</param>
        /// <returns>Dicionário</returns>
        public static Dictionary<T1,T2> AddValue<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            dictionary.Add(key, value);
            return dictionary;
        }

        /// <summary>
        /// Obtém o valor tipado de um dicionário.
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="dictionary">Dicionário</param>
        /// <param name="key">Chave do objeto no dicionário</param>
        /// <returns>Objeto tipado ou valor Padrão</returns>
        public static T GetValue<T>(this Dictionary<String, Object> dictionary, String key, T valorPadrao)
        {
            Object result;
            return dictionary != null ? (dictionary.TryGetValue(key, out result) ? (T)result : valorPadrao) : valorPadrao;
        }

        /// <summary>
        /// Converte uma string de CPF ou CNPJ para um Int64
        /// </summary>
        /// <param name="cpf_cnpj"></param>
        /// <returns></returns>
        public static Int64 CpfCnpjToLong(this String cpf_cnpj)
        {
            Int64 retorno = 0;

            Boolean conversaoValida = Int64.TryParse(cpf_cnpj.Replace("-", "").Replace(".", "").Replace("/", ""), out retorno);

            if (!conversaoValida)
                throw new InvalidOperationException(String.Format("Erro ao converter caracteres da string: {0}.", cpf_cnpj));

            return retorno;
        }

        /// <summary>
        /// Formata um Int64 como CNPJ
        /// </summary>
        /// <param name="cpf_cnpj"></param>
        /// <returns></returns>
        public static String FormatToCnpj(this Int64 cnpj)
        {
            String retorno = String.Empty;
            Int32 lenght = cnpj.ToString().Length;

            if (lenght <= 14)
                retorno = String.Format(@"{0:00\.000\.000\/0000\-00}", cnpj);
            else
                throw new InvalidOperationException("Tamanho do número informado não pode ser formatado para CPNJ.");

            return retorno;
        }

        /// <summary>
        /// Formata um Int64 como CPF
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static String FormatToCpf(this Int64 cpf)
        {
            String retorno = String.Empty;
            Int32 lenght = cpf.ToString().Length;

            if (lenght <= 11)
                retorno = String.Format(@"{0:000\.000\.000\-00}", cpf);
            else
                throw new InvalidOperationException("Tamanho do número informado não pode ser formatado para CPF.");

            return retorno;
        }

        /// <summary>
        /// Valida cpf ou cnpj
        /// </summary>
        /// <param name="cpfcnpj"></param>
        /// <param name="vazio"></param>
        /// <returns></returns>
        public static Boolean IsValidCPFCNPJ(this String cpfcnpj)
        {
            if (string.IsNullOrEmpty(cpfcnpj))
                return true;
            else
            {
                int[] d = new int[14];
                int[] v = new int[2];
                int j, i, soma;
                string Sequencia, SoNumero;

                SoNumero = Regex.Replace(cpfcnpj, "[^0-9]", string.Empty);

                //verificando se todos os numeros são iguais
                if (new string(SoNumero[0], SoNumero.Length) == SoNumero) return false;

                // se a quantidade de dígitos numérios for igual a 11
                // iremos verificar como CPF
                if (SoNumero.Length == 11)
                {
                    for (i = 0; i <= 10; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 8 + i; j++) soma += d[j] * (10 + i - j);

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[9] & v[1] == d[10]);
                }
                // se a quantidade de dígitos numérios for igual a 14
                // iremos verificar como CNPJ
                else if (SoNumero.Length == 14)
                {
                    Sequencia = "6543298765432";
                    for (i = 0; i <= 13; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 11 + i; j++)
                            soma += d[j] * Convert.ToInt32(Sequencia.Substring(j + 1 - i, 1));

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[12] & v[1] == d[13]);
                }
                // CPF ou CNPJ inválido se
                // a quantidade de dígitos numérios for diferente de 11 e 14
                else
                    return false;
            }
        }

        /// <summary>
        /// Valida cpf
        /// </summary>
        /// <param name="cpfcnpj"></param>
        /// <param name="vazio"></param>
        /// <returns></returns>
        public static Boolean IsValidCPF(this String cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return true;
            else
            {
                int[] d = new int[14];
                int[] v = new int[2];
                int j, i, soma;
                string SoNumero;

                SoNumero = Regex.Replace(cpf, "[^0-9]", string.Empty);

                //verificando se todos os numeros são iguais
                if (new string(SoNumero[0], SoNumero.Length) == SoNumero) return false;

                // se a quantidade de dígitos numérios for igual a 11
                // iremos verificar como CPF
                if (SoNumero.Length == 11)
                {
                    for (i = 0; i <= 10; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 8 + i; j++) soma += d[j] * (10 + i - j);

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[9] & v[1] == d[10]);
                }
                // CPF ou CNPJ inválido se
                // a quantidade de dígitos numérios for diferente de 11 e 14
                else
                    return false;
            }
        }

        /// <summary>
        /// Valida cnpj
        /// </summary>
        /// <param name="cpfcnpj"></param>
        /// <param name="vazio"></param>
        /// <returns></returns>
        public static Boolean IsValidCNPJ(this String cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return true;
            else
            {
                int[] d = new int[14];
                int[] v = new int[2];
                int j, i, soma;
                string Sequencia, SoNumero;

                SoNumero = Regex.Replace(cnpj, "[^0-9]", string.Empty);

                //verificando se todos os numeros são iguais
                if (new string(SoNumero[0], SoNumero.Length) == SoNumero) return false;

                // se a quantidade de dígitos numérios for igual a 14
                // iremos verificar como CNPJ
                if (SoNumero.Length == 14)
                {
                    Sequencia = "6543298765432";
                    for (i = 0; i <= 13; i++) d[i] = Convert.ToInt32(SoNumero.Substring(i, 1));
                    for (i = 0; i <= 1; i++)
                    {
                        soma = 0;
                        for (j = 0; j <= 11 + i; j++)
                            soma += d[j] * Convert.ToInt32(Sequencia.Substring(j + 1 - i, 1));

                        v[i] = (soma * 10) % 11;
                        if (v[i] == 10) v[i] = 0;
                    }
                    return (v[0] == d[12] & v[1] == d[13]);
                }
                // CPF ou CNPJ inválido se
                // a quantidade de dígitos numérios for diferente de 11 e 14
                else
                    return false;
            }
        }

        /// <summary>
        /// Apaga espaços em branco a mais na string
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static String DeleteExtraWhitespaces(this String texto)
        {
            Regex regex = new Regex(@"\s{2,}");

            return regex.Replace(texto, " ");
        }

        /// <summary>
        /// Função para remover os acentos da string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String RemoverAcentos(this String input)
        {
            if (String.IsNullOrEmpty(input))
                return "";
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(input);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }

        /// <summary>
        /// Remove Caracteres Especiais da String
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static String RemoverCaracteresEspeciais(this String texto)
        {
            return Regex.Replace(texto, @"[^0-9a-zA-Z\s]+?", "");
        }

        /// <summary>
        /// Busca o valor da enumeração pela descrição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnumValueFromDescription<T>(this String description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Descrição não encontrada");
        }

        /// <summary>
        /// Join, com separador para o último elemento
        /// </summary>
        public static String Join(this IEnumerable<String> items, String separator, String lastSeparator)
        {
            var sep = String.Empty;
            return items.Aggregate(String.Empty, (current, item) =>
            {
                var result = String.Concat(current,
                  current == String.Empty || !items.Last().Equals(item) ? sep : lastSeparator,
                  item.ToString());
                sep = separator;
                return result;
            });
        }

        /// <summary>
        /// "Achata" a estrutura de lista hierarquica (pai, filhos, netos, etc....) 
        /// para uma lista de apenas 1 nível.
        /// </summary>
        public static IEnumerable<T> Flatten<T, R>(this IEnumerable<T> source, Func<T, R> recursion) 
            where R : IEnumerable<T>
        {
            return source.SelectMany(c => recursion(c).Flatten(recursion)).Concat(source);
        }

        /// <summary>
        /// Divide uma string em blocos de tamanho pré-determinado
        /// </summary>
        /// <param name="value">String a ser dividida</param>
        /// <param name="blockSize">Tamanho dos blocos de string</param>
        /// <returns>Coleção de blocos de string de tamanho "blockSize" pré-determinado</returns>
        public static IEnumerable<String> SplitByLength(this String value, Int32 blockSize)
        {
            for (Int32 index = 0; index < value.Length; index += blockSize)
                yield return value.Substring(index, Math.Min(blockSize, value.Length - index));
        }

        #endregion [ FIM - Métodos Públicos ]

        #region [ PRIVADO ]

        /// <summary>Verifica se é um conteúdo para considerar como vazio</summary>
        /// <param name="texto_">Texto</param>
        /// <returns>Se é para considerar como texto vazio</returns>
        private static bool conteudoVazio(string texto_)
        {
            return (texto_ == "" || texto_ == "-- Selecione --" || texto_ == "-- Todas --" || texto_ == "-- Todos --");
        }

        /// <summary>
        /// Recupera o texto de um timespan
        /// </summary>
        /// <param name="round">Define se o valor apresentado deve ser aredondado para números inteiros.</param>
        /// <param name="span">TimeSpan entre dois DateTimes</param>
        /// <returns></returns>
        private static string GetTimeSpanText(TimeSpan span, bool round)
        {
            // Recuperando o tempo em horas
            decimal tempo = (span.Seconds / 60) + span.Minutes;
            tempo = (tempo / 60) + span.Hours;

            // Recuperando o tempo em dias
            bool isDia = (span.Days >= 1);
            if (isDia)
                tempo = (tempo / 24) + span.Days;

            // Criando a mascara de formatação
            string mascara = round ? "{0:#,##0} {1}" : "{0:#,##0.00} {1}";

            // Retornando o valor
            return string.Format(mascara,
                tempo,
                isDia ? (tempo > 1) ? " dias" : " dia" : (tempo > 1) ? " horas" : " hora");
        }

        /*
         *  //Implementação para converção genérica.
            //Conversão genérica de tipos, implementado para teste somente int
            public static T ConvertToType<T>(this string valor_)
            {
                if (typeof(T) == typeof(int))
                {
                    int retInt;
                    if (int.TryParse(valor_, out retInt))
                        return (T)(object)retInt;
                    else
                        return (T)(object)0;
                }
                else
                    return (T)(object)valor_;            
            }
         */

        #endregion
    }
}
