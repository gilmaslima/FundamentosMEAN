using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Globalization;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Lança uma exceção quando a QueryString esta expirada e não é mais válida.
    /// </summary>
    public class QueryStringExpiradaException : Exception
    {
        public QueryStringExpiradaException() : base() { }
    }

    /// <summary>
    /// Lança uma exceção quando tentar criptografar ou descriptografar uma QueryString inválida.
    /// </summary>
    public class QueryStringInvalidaException : Exception
    {
        public QueryStringInvalidaException() : base() { }
    }

    /// <summary>
    /// Prove uma maneira segura de passar informações via QueryString
    /// </summary>
    public class QueryStringSegura : NameValueCollection
    {
        /// <summary>
        /// Construtor padrão da classe.
        /// </summary>
        public QueryStringSegura() : base() { }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="UserKey">UserKey</param>
        public QueryStringSegura(Object UserKey)
        {
            cryptoUserKey = UserKey.ToString();
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="encryptedString">Conteúdo criptografado da QueryString</param>
        public QueryStringSegura(string encryptedString)
        {
            deserialize(Decrypt(encryptedString));

            // Compare the Expiration Time with the current Time to ensure
            // that the queryString has not expired.
            if (DateTime.Compare(ExpireTime, DateTime.Now) < 0)
            {
                throw new QueryStringExpiradaException();
            }
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="encryptedString">Conteúdo criptografado da QueryString</param>
        /// <param name="UserKey">UserKey</param>
        public QueryStringSegura(string encryptedString, Object UserKey)
        {
            cryptoUserKey = UserKey.ToString();
            deserialize(Decrypt(encryptedString));

            // Compare the Expiration Time with the current Time to ensure
            // that the queryString has not expired.
            if (DateTime.Compare(ExpireTime, DateTime.Now) < 0)
            {
                throw new QueryStringExpiradaException();
            }
        }

        /// <summary>
        /// Retorna a QueryString criptografada.
        /// </summary>
        public string EncryptedString
        {
            get
            {
                return HttpUtility.UrlEncode(Encrypt(serialize()));
            }
        }

        private DateTime _expireTime = DateTime.MaxValue;

        /// <summary>
        /// The timestamp in which the EncryptedString should expire
        /// </summary>
        public DateTime ExpireTime
        {
            get
            {
                return _expireTime;
            }
            set
            {
                _expireTime = value;
            }
        }

        /// <summary>
        /// Retrona a QueryString criptografada.
        /// </summary>
        public override String ToString()
        {
            return EncryptedString;
        }

        /// <summary>
        /// Criptografa QueryString serializada.
        /// </summary>
        private String Encrypt(string serializedQueryString)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(serializedQueryString);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(string.Concat(cryptoKey, cryptoUserKey)));
            des.IV = IV;
            return Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                )
            );
        }

        /// <summary>
        /// Realiza a descriptografia da QueryString serializada.
        /// </summary>
        private String Decrypt(string encryptedQueryString)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(encryptedQueryString);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(string.Concat(cryptoKey, cryptoUserKey)));
                des.IV = IV;
                return Encoding.ASCII.GetString(
                    des.CreateDecryptor().TransformFinalBlock(
                        buffer,
                        0,
                        buffer.Length
                    )
                );
            }
            catch (CryptographicException)
            {
                throw new QueryStringInvalidaException();
            }
            catch (FormatException)
            {
                throw new QueryStringInvalidaException();
            }
        }

        /// <summary>
        /// Deserializes a decrypted query string and stores it
        /// as name/value pairs.
        /// </summary>
        private void deserialize(string decryptedQueryString)
        {
            string[] nameValuePairs = decryptedQueryString.Split('&');
            for (int i = 0; i < nameValuePairs.Length; i++)
            {
                string[] nameValue = nameValuePairs[i].Split('=');
                if (nameValue.Length == 2)
                {
                    base.Add(nameValue[0], nameValue[1]);
                }
            }
            // Ensure that timeStampKey exists and update the expiration time.
            if (base[timeStampKey] != null)
            {
                _expireTime = DateTime.Parse(base[timeStampKey], CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Serializes the underlying NameValueCollection as a QueryString
        /// </summary>
        private string serialize()
        {
            Boolean timeStamp = false;
            StringBuilder sb = new StringBuilder();
            foreach (string key in base.AllKeys)
            {
                sb.Append(key);
                sb.Append('=');
                sb.Append(base[key]);
                sb.Append('&');

                if (key.Equals(timeStampKey)) timeStamp = true;
            }

            if (!timeStamp)
            {
                // Append timestamp
                sb.Append(timeStampKey);
                sb.Append('=');
                sb.Append(_expireTime.ToString(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private const string timeStampKey = "__TimeStamp__";

        // The key used for generating the encrypted string
        /// <summary>
        /// Chave utilizada para gerar a string criptografada
        /// </summary>
        private string cryptoKey = "Redecard.PN";

        /// <summary>
        /// Caso queira utilizar alguma chave "adicional" para criptografia (ex: cod_funcionario,Session.SessionID,etc)
        /// </summary>
        private string cryptoUserKey = string.Empty;

        /// <summary>
        /// Inicializa um vetor para a rotina de criptografia
        /// </summary>
        private readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };
    }
}
