using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using Redecard.PN.Comum;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.Util
{
    public class ValidarCelular
    {
        private static Regex RegexCelular 
        { 
            get { return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4,5})\-(?<Parte2>[0-9]{4})$"); } 
        }

        

        /// <summary>
        /// DDD do celular
        /// </summary>
        public static Int32? GetDDD(String numero)
        {

            Match match = RegexCelular.Match(numero);
                if (match.Success)
                    return match.Groups["DDD"].Value.ToInt32Null();
                else
                    return null;
            
        }

        /// <summary>
        /// Número do celular (sem DDD)
        /// </summary>
        public static Int32? GetNumero(String numero)
        {

            Match match = RegexCelular.Match(numero);
                if (match.Success)
                    return String.Concat(match.Groups["Parte1"], match.Groups["Parte2"]).ToInt32Null();
                else
                    return null;
            
        }

       

        /// <summary>
        /// Aplica máscara de celular (99) 99999-9999
        /// </summary>
        public static String AplicarMascara(Int32? ddd, Int32? numero)
        {
            if (ddd.HasValue && numero.HasValue)
            {
                String numeroCelular = numero.Value.ToString();

                //Normaliza para quantidade mínima de 8 números, com zeros à esquerda
                if (numeroCelular.Length < 8)
                    numeroCelular = numeroCelular.PadLeft(8, '0');

                //Obtém a primeira parte do número (pré-hífen)
                String parte1 = numeroCelular.Substring(0, numeroCelular.Length > 8 ? 5 : 4);

                //Obtém a segunda parte do número (pós-hífen)
                String parte2 = numeroCelular.Substring(numeroCelular.Length - 4, 4);

                return String.Format("({0}) {1}-{2}", ddd.Value, parte1, parte2);
            }
            else
                return String.Empty;
        }
    }
}