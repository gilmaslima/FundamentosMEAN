/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint
{
    /// <summary>
    /// Classe contendo métodos auxiliares para formatação de campos
    /// </summary>
    public static class Formatador
    {
        /// <summary>
        /// Formatação de telefone no padrão (XX) XXXXX-XXXX
        /// </summary>
        /// <param name="numero">Número do telefone</param>
        /// <returns>Número formatado</returns>
        public static String Telefone(String numero)
        {
            //limpa a string, mantendo apenas os números
            numero = numero.RemoverLetras();

            //trata as 4 situações, baseando-se na quantidade de caracteres na string:
            // 10 caracteres:   DDD + 8 dígitos
            // 11 caracteres:   DDD + 9 dígitos
            // 9 caracteres:    9 dígitos
            // 8 caracteres:    8 dígitos

            switch (numero.Length)
            {
                case 8:     // sem DDD, 8 dígitos
                    numero = Regex.Replace(numero, @"(\d{4})(\d{4})", "(00) $1-$2");
                    break;

                case 9:     // sem DDD, 9 dígitos
                    numero = Regex.Replace(numero, @"(\d{5})(\d{4})", "(00) $1-$2");
                    break;

                case 10:    // com DDD, 8 dígitos
                    numero = Regex.Replace(numero, @"(\d{2})(\d{4})(\d{4})", "($1) $2-$3");
                    break;

                case 11:    // com DDD, 9 dígitos
                    numero = Regex.Replace(numero, @"(\d{2})(\d{5})(\d{4})", "($1) $2-$3");
                    break;

                default:
                    numero = String.Empty;
                    break;
            }

            return numero;
        }

        /// <summary>
        /// Formatação de telefone no padrão (XX) XXXXX-XXXX.
        /// </summary>
        /// <param name="ddd">DDD</param>
        /// <param name="numero">Número do telefone (8 ou 9 dígitos)</param>
        /// <returns></returns>
        public static String Telefone(Int32? ddd, Int32? numero)
        {
            //Se número é inexistente, retorna string vazia
            if (!numero.HasValue)
                return String.Empty;

            //Força DDD a ter no mínimo 2 dígitos (completa com zeros)
            String valorDdd = (ddd ?? 0).ToString("D2");
            String valorNumero = numero.Value.ToString();

            //Se número possui menos que 8 caracteres, retorna string vazia
            if (valorNumero.Length < 8)
                return String.Empty;

            return Telefone(String.Concat(valorDdd, valorNumero));
        }
    }
}
