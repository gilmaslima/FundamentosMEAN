using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.Eadquirencia.Sharepoint.Helper
{
    public static class TextHelper
    {
        /// <summary>
        /// Retira todo caracter que pode quebrar com uma string javascript.
        /// </summary>
        /// <param name="mensagem">Mensagem que terá os caracteres especiais retirados.</param>
        /// <returns>mensagem sem caracter especial.</returns>
        public static String FormatarMensagemJavaScript(String mensagem)
        {
            return mensagem.Replace("'", "\\'")
                            .Replace(Environment.NewLine, "<br />")
                            .Replace("\r\n", "<br />")
                            .Replace("\n", "<br />");
        }

        /// <summary>
        /// Converte uma String no formato "120000" para "R$ 1.200,99".
        /// </summary>
        /// <param name="valor">String que será convertida.</param>
        /// <returns>valor em string.</returns>
        public static String ConverterValorMonetario(String valor)
        {
            decimal check;
            decimal.TryParse(valor, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, new CultureInfo("pt-BR"), out check);

            check = check / 100;

            return check.ToString("#,##0.00", CultureInfo.CurrentUICulture);
        }
    }
}
