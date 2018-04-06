using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Redecard.PN.RAV.Core.Seguranca.Portal
{
    /// <summary>Criptografia com Rijndael / AES</summary>
    public class Criptografia
    {
        /// <summary>     
        /// Vetor de bytes utilizados para a criptografia (Chave Externa)     
        /// </summary>     
        private static byte[] bIV = 
        //{ 0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18,
        //0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };
        { 0x10, 0x10, 0x1F, 0xDE, 0xAE, 0x30, 0x2F, 0x68,
        0x66, 0x5E, 0xAA, 0x5B, 0x02, 0x4F, 0xD6, 0x61 };

        /// <summary>     
        /// Representação de valor em base 64 (Chave Interna)    
        /// O Valor representa a transformação para base64 de     
        /// um conjunto de 32 caracteres (8 * 32 = 256bits)    
        /// A chave é: "Criptografia Projeto Redecard.PN"
        /// </summary>     
        private const string cryptoKey = "Q3JpcHRvZ3JhZmlhIFByb2pldG8gUmVkZWNhcmQuUE4=";

        /// <summary>     
        /// Metodo de criptografia de valor     
        /// </summary>     
        /// <param name="text">valor a ser criptografado</param>     
        /// <returns>valor criptografado</returns>
        public static String Encrypt(string text)
        {
            return Encrypt(text, bIV);
        }

        private static String Encrypt(string text, byte[] iv)
        {
            try
            {
                // Se a string não está vazia, executa a criptografia
                if (!string.IsNullOrEmpty(text))
                {
                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String(cryptoKey);
                    byte[] bText = new UTF8Encoding().GetBytes(text);

                    // Instancia a classe de criptografia Rijndael
                    Rijndael rijndael = new RijndaelManaged();

                    // Define o tamanho da chave "256 = 8 * 32"                
                    // Lembre-se: chaves possíves:                
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                    rijndael.KeySize = 256;

                    String retorno = String.Empty;

                    // Cria o espaço de memória para guardar o valor criptografado:                
                    // Instancia o encriptador                 
                    using (MemoryStream mStream = new MemoryStream())
                    using (CryptoStream encryptor = new CryptoStream(mStream, rijndael.CreateEncryptor(bKey, iv), CryptoStreamMode.Write))
                    {

                        // Faz a escrita dos dados criptografados no espaço de memória
                        encryptor.Write(bText, 0, bText.Length);
                        // Despeja toda a memória.                
                        encryptor.FlushFinalBlock();
                        // Pega o vetor de bytes da memória e gera a string criptografada

                        retorno = Convert.ToBase64String(mStream.ToArray());
                    }
                    return retorno;
                }
                else
                {
                    // Se a string for vazia retorna nulo                
                    return null;
                }
            }
            catch (InsufficientMemoryException ex)
            {
                // Se algum erro ocorrer, dispara a exceção            
                throw new ApplicationException("Erro ao criptografar", ex);
            }
            catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção            
                throw new ApplicationException("Erro ao criptografar", ex);
            }
        }

        /// <summary>     
        /// Pega um valor previamente criptografado e retorna o valor inicial 
        /// </summary>     
        /// <param name="text">texto criptografado</param>     
        /// <returns>valor descriptografado</returns>     
        public static String Decrypt(string text)
        {
            return Decrypt(text, bIV);
        }

        private static String Decrypt(string text, byte[] iv)
        {
            try
            {
                // Se a string não está vazia, executa a criptografia           
                if (!string.IsNullOrEmpty(text))
                {
                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String(cryptoKey);
                    byte[] bText = Convert.FromBase64String(text);

                    // Instancia a classe de criptografia Rijndael                
                    Rijndael rijndael = new RijndaelManaged();

                    // Define o tamanho da chave "256 = 8 * 32"                
                    // Lembre-se: chaves possíves:                
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                    rijndael.KeySize = 256;

                    // Cria o espaço de memória para guardar o valor DEScriptografado:               
                    MemoryStream mStream = new MemoryStream();

                    // Instancia o Decriptador                 
                    CryptoStream decryptor = new CryptoStream(
                        mStream,
                        rijndael.CreateDecryptor(bKey, iv),
                        CryptoStreamMode.Write);

                    // Faz a escrita dos dados criptografados no espaço de memória   
                    decryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                
                    decryptor.FlushFinalBlock();
                    // Instancia a classe de codificação para que a string venha de forma correta         
                    UTF8Encoding utf8 = new UTF8Encoding();
                    // Com o vetor de bytes da memória, gera a string descritografada em UTF8
                    return utf8.GetString(mStream.ToArray());
                }
                else
                {
                    // Se a string for vazia retorna nulo                
                    return null;
                }
            }
            catch (InsufficientMemoryException ex)
            {
                // Se algum erro ocorrer, dispara a exceção            
                throw new ApplicationException("Erro ao criptografar", ex);
            }
            catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção            
                throw new ApplicationException("Erro ao descriptografar", ex);
            }
        }


        /// <summary>
        /// Método padrão para criptografia de PVs.
        /// Em caso de erro, retorna null.        
        /// </summary>
        /// <param name="pvCriptografar">PV a ser criptografado</param>
        /// <param name="codigoEntidade">Código da entidade do usuário logado, para validação da chave</param>
        /// <returns>PV criptografado (null em caso de erro)</returns>
        public static String CriptografarPV(Int32 pvCriptografar, Int32 codigoEntidade)
        {
            try
            {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(codigoEntidade.ToString(), bIV);
                return Criptografia.Encrypt(pvCriptografar.ToString(), pdb.GetBytes(16));
            }
            catch (InsufficientMemoryException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Método padrão para descriptografia de PVs.
        /// Em caso de erro, retorna null.
        /// </summary>
        /// <param name="pvCriptografado">PV criptografado</param>
        /// <param name="codigoEntidade">Código da entidade do usuário logado, para validação da chave</param>
        /// <returns>PV criptografado (null em caso de erro)</returns>
        public static Int32 DescriptografarPV(String pvCriptografado, Int32 codigoEntidade)
        {
            try
            {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(codigoEntidade.ToString(), bIV);
                return Criptografia.Decrypt(pvCriptografado, pdb.GetBytes(16)).ToInt32(0);
            }
            catch (InsufficientMemoryException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
