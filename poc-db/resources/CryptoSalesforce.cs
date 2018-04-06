/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Security.Cryptography;
using System.Text;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core
{
    /// <summary>
    /// Classe de criptografia para integração com Salesforce.
    /// Métodos copiados da DLL classlibHash da Salesforce.
    /// </summary>
    public sealed class CryptoSalesforce
    {
        /// <summary>
        /// create salt
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>salt</returns>
        private static Byte[] CreateSalt(Int32 size)
        {
            var rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
            Byte[] array = new Byte[size];
            rNGCryptoServiceProvider.GetBytes(array);
            return array;
        }

        /// <summary>
        /// Generate salted hash
        /// </summary>
        /// <param name="plainText">plainText</param>
        /// <param name="salt">salt</param>
        /// <returns>salted hash</returns>
        private static Byte[] GenerateSaltedHash(Byte[] plainText, Byte[] salt)
        {
            HashAlgorithm hashAlgorithm = new SHA256Managed();

            Byte[] array = new Byte[plainText.Length + salt.Length];

            for (Int32 index = 0; index < plainText.Length; index++)
            {
                array[index] = plainText[index];
            }

            for (Int32 index = 0; index < salt.Length; index++)
            {
                array[plainText.Length + index] = salt[index];
            }

            return hashAlgorithm.ComputeHash(array);
        }

        /// <summary>
        /// Geração de hash para PV
        /// </summary>
        /// <param name="numeroPV">número do PV</param>
        /// <returns>hash do PV</returns>
        public static String HashPV(Int32 numeroPV)
        {
            Byte[] pvHash = GenerateSaltedHash(Encoding.UTF8.GetBytes(numeroPV.ToString()), Encoding.UTF8.GetBytes("92"));
            return Convert.ToBase64String(pvHash);
        }
    }
}