/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.HomePage.SharePoint
{
    public static class EncryptFactoryRedeRecompensa
    {
        #region [Atributos]
        private const String cryptoKey = "lTJUMr3/xfc0UKXCI5Zj61ZDMA4LTLzk65WffB/og2U=";
        private const String cryptoIV = "8mszk2Jza3LGb5lIZYUtqw==";        
        #endregion

        #region [Métodos]
        /// <summary>
        /// Realiza a criptografia de uma string utilizando o algoritimo Rijndael e as chaves informadas.
        /// </summary>
        /// <param name="plainText">String que será criptografada</param>
        /// <returns>Chave criptografada.</returns>
        public static String EncryptString(String plainText)
        {
            using (Logger log = Logger.IniciarLog("Criptografia - Link Rede Recompensa"))
            {
                // Check arguments.
                if (String.IsNullOrWhiteSpace(plainText))
                    throw new ArgumentNullException("plainText");

                // Declare the stream used to encrypt to an in memory
                // array of bytes.
                MemoryStream msEncrypt = null;

                // Declare the RijndaelManaged object
                // used to encrypt the data.
                RijndaelManaged cryptographyAlg = null;

                try
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    cryptographyAlg = new RijndaelManaged();
                    cryptographyAlg.Key = Convert.FromBase64String(cryptoKey);
                    cryptographyAlg.IV = Convert.FromBase64String(cryptoIV);

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = cryptographyAlg.CreateEncryptor(cryptographyAlg.Key, cryptographyAlg.IV);

                    // Create the streams used for encryption.
                    msEncrypt = new MemoryStream();
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }

                }
                catch (CryptographicException cryEx)
                {
                    log.GravarErro(cryEx);
                    throw new Exception("Erro ao gerar chave criptografada para o link Rede Recompensa:", cryEx);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new Exception("Erro ao gerar chave criptografada para o link Rede Recompensa:", ex);
                }
                finally
                {
                    // Clear the RijndaelManaged object.
                    if (cryptographyAlg != null)
                        cryptographyAlg.Clear();
                }

                // Return the encrypted bytes from the memory stream.
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
        #endregion
    }
}
