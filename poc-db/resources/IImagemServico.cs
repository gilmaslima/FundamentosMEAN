using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Serviço para manipulação de Imagens de documentos de comprovação de Requests pendentes.
    /// </summary>
    [ServiceContract]
    public interface IImagemServico
    {
        /// <summary>
        /// Consolida uma lista de imagens em páginas.
        /// Junta as imagens em um só arquivo TIF) e grava no banco de dados da Central Fax.
        /// </summary>
        /// <param name="imagens">Nomes relativos das imagens no servidor</param>        
        /// <param name="imgTiff">Nome do arquivo Tiff que será gerado</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsolidarImagensPagina(String[] imagens, String imgTiff);

        /// <summary>Altera as características da imagem para armazenamento no banco de dados.</summary>        
        /// <param name="fileName">Nome relativo do arquivo no servidor</param>        
        /// <returns>Nome do arquivo convertido</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConverterImagem(String fileName);

        /// <summary>Grava a imagem temporária (fisicamente armazenada no servidor) no banco de dados.</summary>        
        /// <param name="arquivo">Nome relativo do arquivo</param>
        /// <param name="data">Data</param>
        /// <param name="id">Id (deve ser 72, valor definido pela Synergy)</param>
        /// <param name="processo">Processo</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarImagens(String arquivo, String data, Int32 id, String processo);

        /// <summary>
        /// Envia uma imagem para o servidor.
        /// </summary>
        /// <param name="imgContent">Conteúdo binário da imagem a ser salva</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <returns>Nome do arquivo salvo no servidor. Se vazio, erro durante envio de imagem ou imagem inválida</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String EnviarImagem(Byte[] imgContent, String nomeArquivo);

        /// <summary>Apaga o arquivo do servidor.</summary>
        /// <param name="nomeArquivo">Nome do arquivo a ser excluído.</param>
        /// <returns>Se TRUE, sucesso na deleção do arquivo.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ApagarArquivo(String nomeArquivo);
    }
}
