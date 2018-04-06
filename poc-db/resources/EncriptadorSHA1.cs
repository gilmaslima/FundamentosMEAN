using System;
using System.Security.Cryptography;
using System.Text;

namespace Rede.PN.ApiLogin.Core.Padrao.Criptografia
{
    public class EncriptadorSha1
    {
        /// <summary>
        /// Criptografa uma string como SHA1
        /// </summary>
        /// <param name="valor">Valor a ser criptografado</param>
        /// <returns></returns>
        public static String EncryptString(String valor)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
            byte[] valorCriptografado = provider.ComputeHash(encoding.GetBytes(valor));

            return EncriptadorSha1.ByteArrayToString(valorCriptografado);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns></returns>
        private static String ByteArrayToString(byte[] inputArray)
        {
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < inputArray.Length; i++)
            {
                string caractTemp = inputArray[i].ToString("X2");

                if (caractTemp.Substring(0, 1) == "0")
                    output.Append(caractTemp.Substring(1));
                else
                    output.Append(caractTemp);

            }
            return output.ToString();
        }
    }
}
