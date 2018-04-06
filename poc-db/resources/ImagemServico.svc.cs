#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Alexandre Shiroma]
Empresa     : [Iteris]
Histórico   :
- [30/07/2012] – [Alexandre Shiroma] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using System.Configuration;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Serviço para manipulação de Imagens de documentos de comprovação de Requests pendentes.
    /// </summary>
    /// <example>
    /// <code lang="cs">
    /// try 
    /// {
    ///     using (ImagemServicoClient clientImagem = new ImagemServicoClient())
    ///     {
    ///         String nomeArquivo = clientImagem.EnviarImagem(fileUpload.FileBytes, "arquivo.jpg");
    ///
    ///         /* lógica */
    ///     }
    /// }
    /// catch(FaultException&lt;ImagensServico.GeneralFault&gt; ex)
    /// {
    ///     /* tratamento de erro do serviço de imagem */ 
    /// }
    /// catch(Exception ex)
    /// {
    ///     /* tratamento genérico de erros não tratados previamente */
    /// }
    /// </code>
    /// </example>
    public class ImagemServico : ServicoBase, IImagemServico
    {
        private String _pastaImagensServidor;
        /// <summary>Pasta no servidor onde serão armazenadas as imagens</summary>
        private String PastaImagensServidor
        {
            get
            {
                if (_pastaImagensServidor == null)
                {
                    _pastaImagensServidor = ConfigurationManager.AppSettings["UploadDocumentos"];
                    if (!System.IO.Directory.Exists(_pastaImagensServidor))
                        throw new System.IO.DirectoryNotFoundException();
                }

                return _pastaImagensServidor;
            }
        }
        
        private Int32 _maxHeight;
        /// <summary>Altura máxima da imagem</summary>
        private Int32 MaxHeight
        {
            get
            {
                if (_maxHeight == default(Int32))
                    _maxHeight = ConfigurationManager.AppSettings["MaxHeight"].ToInt32(9999);
                return _maxHeight;
            }            
        }

        private Int32 _maxWidth;
        /// <summary>Largura máxima da imagem</summary>
        private Int32 MaxWidth
        {
            get
            {
                if (_maxWidth == default(Int32))
                    _maxWidth = ConfigurationManager.AppSettings["MaxWidth"].ToInt32(9999);
                return _maxWidth;
            }
        }

        private Int32 _maxDpi;
        /// <summary>Resolução Dpi máxima da imagem</summary>
        private Int32 MaxDpi
        {
            get
            {
                if (_maxDpi == default(Int32))
                    _maxDpi = ConfigurationManager.AppSettings["MaxDpi"].ToInt32(9999);
                return _maxDpi;
            }
        }
        
        /// <summary>Grava a imagem temporária (fisicamente armazenada no servidor) no banco de dados.</summary>
        /// <remarks>
        /// As imagens e as referências a elas são armazenadas nos bancos de dados <b>GSEDM</b> e <b>FileStore</b>.
        /// O método utiliza a procedure <b>spInsertTiff</b> do banco <b>GSEDM</b> para inclusão das referências das imagens.
        /// </remarks>
        /// <param name="arquivo">Nome relativo do arquivo</param>
        /// <param name="data">Data</param>
        /// <param name="id">Id (deve ser 72, valor definido pela Synergy)</param>
        /// <param name="processo">Processo</param>
        public void GravarImagens(String arquivo, String data, Int32 id, String processo)
        {
            using (Logger Log = Logger.IniciarLog("Gravação de Comprovante em banco de dados GSEDM"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { arquivo, data, id, processo });

                try
                {
                    //Instanciação da classe de negócio
                    var negocioImagem = new Negocio.Imagem();

                    String mensagem = String.Empty;

                    //Grava imagem no banco de dados
                    negocioImagem.GravarImagens(PastaImagensServidor, arquivo, data, id, processo, ref mensagem);

                    Log.GravarLog(EventoLog.FimServico);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consolida uma lista de imagens em páginas.
        /// Junta as imagens em um só arquivo TIF) e grava no banco de dados da Central Fax.
        /// </summary>
        /// <param name="imagens">Nomes relativos das imagens no servidor</param>        
        /// <param name="imgTiff">Nome do arquivo Tiff que será gerado</param>
        public void ConsolidarImagensPagina(String[] imagens, String imgTiff)
        {
            using (Logger Log = Logger.IniciarLog("Consolidação de Comprovantes enviados"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { imagens, imgTiff });

                try
                {
                    //Instanciação da classe de negócio de tratamento/manipulação de imagens
                    var negocioImagem = new Negocio.Imagem();

                    //Chama serviço
                    negocioImagem.ConsolidarImagensPagina(imagens, PastaImagensServidor, imgTiff);

                    Log.GravarLog(EventoLog.FimServico);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }
       
        /// <summary>
        /// Envia uma imagem para o servidor.
        /// </summary>
        /// <param name="imgContent">Conteúdo binário da imagem a ser salva</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <returns>Nome do arquivo salvo no servidor. Se vazio, erro durante envio de imagem ou imagem inválida</returns>
        public String EnviarImagem(Byte[] imgContent, String nomeArquivo)
        {
            using (Logger Log = Logger.IniciarLog("Upload temporário de comprovante"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { nomeArquivo, tamanhoEmBytes = imgContent.Length });

                //Instanciação da classe de negócio de tratamento/manipulação de imagens
                var negocio = new Negocio.Imagem();

                try
                {                   
                    //Valida e salva a imagem
                    Boolean envioOK = negocio.EnviarImagem(imgContent, PastaImagensServidor, nomeArquivo);

                    //Se não foi possível salvar a imagem, ou a imagem é inválida, retorna vazio
                    if (!envioOK)
                        return String.Empty;

                    //Padroniza tamanho e qualidade da imagem
                    if (!nomeArquivo.Contains(".pdf"))
                    {
                        nomeArquivo = negocio.ConverterImagem(PastaImagensServidor, nomeArquivo, MaxHeight, MaxWidth, MaxDpi);
                    }

                    Log.GravarLog(EventoLog.FimServico, new { nomeArquivo });

                    //Retorna o nome do arquivo salvo no servidor
                    return nomeArquivo;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    //Se ocorreu alguma exceção, apaga o arquivo do servidor
                    negocio.ApagarArquivo(PastaImagensServidor, nomeArquivo);
                    
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>Altera as características da imagem para armazenamento no banco de dados.</summary>        
        /// <param name="fileName">Nome relativo do arquivo no servidor</param>        
        /// <returns>Nome do arquivo convertido</returns>
        public String ConverterImagem(String fileName)
        {
            using (Logger Log = Logger.IniciarLog("Conversão de Imagem"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { fileName });

                try
                {
                    //Instanciação da classe de negócio de tratamento/manipulação de imagens
                    var negocioImagem = new Negocio.Imagem();

                    //Chama serviço
                    String fileNameRetorno = negocioImagem.ConverterImagem(PastaImagensServidor, fileName, MaxHeight, MaxWidth, MaxDpi);

                    Log.GravarLog(EventoLog.FimServico, new { fileNameRetorno });

                    return fileNameRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>Apaga o arquivo do servidor.</summary>
        /// <param name="nomeArquivo">Nome do arquivo a ser excluído.</param>
        /// <returns>Se TRUE, sucesso na deleção do arquivo.</returns>        
        public Boolean ApagarArquivo(String nomeArquivo)
        {
            using (Logger Log = Logger.IniciarLog("Apaga arquivo temporário do servidor"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { nomeArquivo });

                try
                {
                    //Instanciação da classe de negócio de tratamento/manipulação de imagens
                    var negocio = new Negocio.Imagem();

                    //Chama serviço
                    Boolean retorno = negocio.ApagarArquivo(PastaImagensServidor, nomeArquivo);

                    Log.GravarLog(EventoLog.FimServico, new { retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
    }    
}
