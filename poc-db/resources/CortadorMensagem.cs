/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Redecard.PN.OutrosServicos.Agentes
{
    /// <summary>
    /// Classe genérica para ler os REDEFINES dos books
    /// </summary>
    public class CortadorMensagem
    {
        private StringReader reader;

        /// <summary>
        /// Inicialização da mensagem
        /// </summary>
        /// <param name="mensagem"></param>
        public CortadorMensagem(string mensagem)
        {
            if (mensagem == null)
            {
                mensagem = string.Empty;
            }
            reader = new StringReader(mensagem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public string LerString(int tamanho)
        {
            char[] buffer = new char[tamanho];

            int i = reader.Read(buffer, 0, tamanho);
            if (i == -1)
            {
                return string.Empty;
            }

            return new String(buffer).TrimEnd().Replace("\0", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public short LerShort(int tamanho)
        {
            short result;

            string s = LerString(tamanho);

            short.TryParse(s, out result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public Int16 LerInt16(int tamanho)
        {
            Int16 result;

            string s = LerString(tamanho);

            Int16.TryParse(s, out result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public Int32 LerInt32(int tamanho)
        {
            Int32 result;

            string s = LerString(tamanho);

            Int32.TryParse(s, out result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        public Int64 LerInt64(int tamanho)
        {
            Int64 result;

            string s = LerString(tamanho);

            Int64.TryParse(s, out result);

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="quantidadeCasasDecimais"></param>
        /// <returns></returns>
        public decimal LerDecimal(int tamanho, short quantidadeCasasDecimais)
        {
            double result;

            String s = LerString(tamanho + quantidadeCasasDecimais);

            double.TryParse(s, out result);

            return Convert.ToDecimal((result / (Math.Pow(Convert.ToDouble(10), Convert.ToDouble(quantidadeCasasDecimais)))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="formato"></param>
        /// <returns></returns>
        public DateTime LerData(int tamanho, string formato)
        {
            return LerData(tamanho, formato, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="formato"></param>
        /// <param name="normalizarSeparadoresParaBarra"></param>
        /// <returns></returns>
        public DateTime LerData(int tamanho, string formato, Boolean normalizarSeparadoresParaBarra)
        {
            DateTime result;

            string s = LerString(tamanho);

            if (normalizarSeparadoresParaBarra)
            {
                s = s.Replace(".", "/").Replace("-", "/").Replace(" ", "/");
            }
            DateTime.TryParseExact(s, formato, null, System.Globalization.DateTimeStyles.None, out result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="formato"></param>
        /// <returns></returns>
        public DateTime LerDataSemSeparador(int tamanho, string formato)
        {
            DateTime result;

            string s = LerString(tamanho);

            s = String.Concat(s.Substring(0, 2), "/", s.Substring(2, 2), "/", s.Substring(4,4));
            
            DateTime.TryParseExact(s, formato, null, System.Globalization.DateTimeStyles.None, out result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="formato"></param>
        /// <returns></returns>
        public String LerHoraSemSeparador(int tamanho)
        {
            String result;

            string s = LerString(tamanho);

            s = String.Concat(s.Substring(2, 2), ":", s.Substring(4, 2), ":", s.Substring(6, 2));

            result = s;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tamanhoOccurs"></param>
        /// <param name="numeroOccurs"></param>
        /// <returns></returns>
        public string[] LerOccurs(int tamanhoOccurs, int numeroOccurs)
        {
            string[] result = new string[numeroOccurs];

            for (int i = 0; i < numeroOccurs; i++)
            {
                result[i] = LerString(tamanhoOccurs);
            }

            return result;
        }
    }
}
