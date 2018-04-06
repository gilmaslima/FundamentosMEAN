using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using System.IO;
using System.Drawing;

namespace Redecard.PN.Request.Negocio
{
    public class Imagem : RegraDeNegocioBase
    {        
        public static String PastaServidor(String pathServidor)
        {            
            try
            {                    
                if (!Directory.Exists(pathServidor))
                    Directory.CreateDirectory(pathServidor);
                return pathServidor;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }           
        }

        /// <summary>Grava a imagem temporária (fisicamente armazenada no servidor) no banco de dados.</summary>
        /// <param name="arquivo">Nome relativo do arquivo</param>
        /// <param name="pastaServidor">Pasta onde o arquivo está armazenado</param>
        /// <param name="data">Data</param>
        /// <param name="id">Id (deve ser 72, valor definido pela Synergy)</param>
        /// <param name="processo">Processo</param>
        /// <param name="mensagem">Mensagem</param>
        public void GravarImagens(String pastaServidor, String arquivo, String data, Int32 id, String processo, ref String mensagem)
        {
            try
            {
                //Carrega a imagem que será armazenada em banco de dados
                byte[] file = File.ReadAllBytes(Path.Combine(PastaServidor(pastaServidor), arquivo));

                //Instanciação da classe de dados
                var dadosImagem = new Dados.Imagem();

                //Insere a imagem em banco
                dadosImagem.RunInsertImg(file, data, id, processo, ref mensagem);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consolida uma lista de imagens em páginas.
        /// Junta as imagens em um só arquivo TIF) e grava no banco de dados da Central Fax.
        /// </summary>
        /// <param name="pastaServidor">Pasta onde o arquivo está armazenado</param>
        /// <param name="imagens">Nomes relativos das imagens no servidor</param>
        /// <param name="imgTiff">Nome do arquivo Tiff que será gerado</param>
        public void ConsolidarImagensPagina(String[] imagens, String pastaServidor, String imgTiff)
        {
            try
            {
                Negocio.TrataImagem.ConsolidarImagensPaginas(PastaServidor(pastaServidor), imagens, imgTiff);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>Transfere a imagem para o servidor, armazenando-a temporária e fisicamente.</summary>
        /// <param name="imgContent">Stream da imagem</param>
        /// <param name="pastaServidor">Pasta onde o arquivo está armazenado</param>
        /// <param name="arquivo">Nome relativo do arquivo no servidor</param>
        /// <returns>Se false, a imagem não é válida</returns>
        public Boolean EnviarImagem(byte[] imgContent, String pastaServidor, String arquivo)
        {
            try
            {
                if (arquivo.Contains(".pdf") || this.ValidarImagem(imgContent))
                {
                    File.WriteAllBytes(Path.Combine(PastaServidor(pastaServidor), arquivo), imgContent);
                    return true;
                }
                else
                {
                    return false;
                }                
            }                 
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>Altera as características da imagem para armazenamento no banco de dados.</summary>
        /// <param name="pastaServidor">Pasta onde o arquivo está armazenado</param>
        /// <param name="fileName">Nome relativo do arquivo no servidor</param>
        /// <param name="maxHeight">Altura máxima da imagem</param>
        /// <param name="maxWidth">Largura máxima da imagem</param>
        /// <param name="maxDpi">Resolução Dpi máxima da imagem</param>
        /// <returns>Nome do arquivo convertido</returns>
        public String ConverterImagem(String pastaServidor, String fileName, Int32 maxHeight, Int32 maxWidth, Int32 maxDpi)
        {
            try
            {
                return Negocio.TrataImagem.ConverteImagem(fileName, PastaServidor(pastaServidor), maxHeight, maxWidth, maxDpi);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
                
        /// <summary>Verifica se é uma imagem válida.</summary>
        /// <param name="imgcontent">Binário a ser verificado se é uma imagem válida</param>
        /// <returns>Booleano indicando se a imagem é válida</returns>
        private Boolean ValidarImagem(Byte[] imgcontent)
        {
            try
            {
                Image g = null;
                try
                {
                    using (MemoryStream ms = new MemoryStream(imgcontent))
                    {
                        //Tenta carregar a imagem, se for inválida, vai para o "Catch"
                        g = Bitmap.FromStream(ms);                        
                        return true;
                    }                    
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (g != null)
                        g.Dispose();
                }
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        
        /// <summary>Apaga fisicamente o arquivo informado.</summary>
        /// <param name="arquivo">Nome relativo do arquivo</param>        
        public Boolean ApagarArquivo(String pastaServidor, String arquivo)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(PastaServidor(pastaServidor));
                if (di != null)
                {
                    FileSystemInfo[] fia = di.GetFileSystemInfos(arquivo);
                    if (fia != null && fia.Length > 0)
                    {
                        foreach (FileSystemInfo fi in fia)
                            fi.Delete();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
