using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Keys
{
    /// <summary>
    /// Classe para gerenciar o acesso as chaves RSA para Tokens
    /// </summary>
    public class TokenKeyManager
    {
        /// <summary>
        /// Realiza a leitura dos Bytes de um arquivo de chave privada configurado
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider ReadPrivateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["PrivateKeyLocation"] != null)
            {
                String caminhoArquivo = ConfigurationManager.AppSettings["PrivateKeyLocation"];

                if (File.Exists(caminhoArquivo))
                {
                    string privateKey = File.ReadAllText(caminhoArquivo);
                    rsa.FromXmlString(privateKey);
                }
                else
                    throw new FileNotFoundException("Private Key não encontrada");
            }
            else
                throw new NullReferenceException("Configurações não encontradas");

            return rsa;
        }

        /// <summary>
        /// Realiza a leitura dos Bytes de um arquivo de chave pública configurado
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider ReadPublicKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["PublicKeyLocation"] != null)
            {
                String caminhoArquivo = ConfigurationManager.AppSettings["PublicKeyLocation"];

                if (File.Exists(caminhoArquivo))
                {
                    string privateKey = File.ReadAllText(caminhoArquivo);
                    rsa.FromXmlString(privateKey);
                }
                else
                    throw new FileNotFoundException("Public Key não encontrada");
            }
            else
                throw new NullReferenceException("Configurações não encontradas");

            return rsa;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] PEM(string type, byte[] data)
        {
            string pem = Encoding.ASCII.GetString(data);

            string header = String.Format("-----BEGIN {0}-----\n", type);
            string footer = String.Format("-----END {0}-----\n", type);
            
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            
            string base64 = pem.Substring(start, (end - start));
            
            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tipoChave">RSA PRIVATE KEY ou PUBLIC KEY</param>
        /// <returns></returns>
        private static X509Certificate2 LoadCertificateFile(string fileName, string tipoChave)
        {
            X509Certificate2 x509 = null;
            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    // maybe it's ASCII PEM base64 encoded ? 
                    data = PEM(tipoChave, data);
                }
                if (data != null)
                    x509 = new X509Certificate2(data);
            }
            return x509;
        } 
    }
}