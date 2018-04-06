using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Redecard.PN.Comum.GZip
{
    /// <summary>
    /// Classe auxiliar de Compactação/Descompactação de dados.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Descomprime/descompacta um array de bytes.
        /// </summary>
        /// <param name="conteudoCompactado">Conteúdo compactado</param>
        /// <returns>Conteúdo descompactado</returns>
        public static Byte[] DescompactarConteudo(Byte[] conteudoCompactado)
        {
            MemoryStream strRet = new MemoryStream();
            
            using (MemoryStream contCompac = new MemoryStream(conteudoCompactado))
                using (GZipStream gzstr = new GZipStream(contCompac, CompressionMode.Decompress))
                {
                    gzstr.CopyTo(strRet);
                }
            
            return strRet.ToArray();
        }

        /// <summary>
        /// Comprime/compacta um array de bytes.
        /// </summary>
        /// <param name="conteudoACompactar">Conteúdo que será compactado</param>
        /// <returns>Conteúdo compactado</returns>
        public static Byte[] CompactarConteudo(Byte[] conteudoACompactar)
        {
            MemoryStream strRet = new MemoryStream();
            
            using (MemoryStream contACompac = new MemoryStream(conteudoACompactar))
                using (GZipStream compStream = new GZipStream(strRet, CompressionMode.Compress))
                {
                    contACompac.CopyTo(compStream);
                }
            
            return strRet.ToArray();
        }

        /// <summary>Reflection da versão 4.0: Reads all the bytes from the current stream and writes them to the destination stream.</summary>                
        /// <param name="destination">The stream that will contain the contents of the current stream.</param>        
        internal static void CopyTo(this Stream source, Stream destination)
        {
            byte[] array = new byte[4096];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
                destination.Write(array, 0, count);
        }
    }
}
