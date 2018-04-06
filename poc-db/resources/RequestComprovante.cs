using Redecard.PN.Request.SharePoint.Model;
using System;

namespace Redecard.PN.Request.SharePoint.Business
{
    /// <summary>
    /// Modelo de request para a busca de comprovantes
    /// </summary>
    public abstract class RequestComprovante
    {
        /// <summary>
        /// Dados da requisição
        /// </summary>
        public ComprovanteServiceRequest RequestData { get; set; }

        public RequestComprovante() { }
        public RequestComprovante(ComprovanteServiceRequest requestData)
        {
            this.RequestData = requestData;
        }

        /// <summary>
        /// Método de consulta dos dados
        /// </summary>
        /// <returns></returns>
        public ComprovanteServiceResponse ConsultarServico()
        {
            if (this.RequestData == null)
                throw new Exception("Dados de busca não carregados");

            return this.ConsultarServico(RequestData);
        }

        /// <summary>
        /// Método customizado para consulta à camada de serviços para consulta dos comprovantes
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <returns>Response customizado com a relação dos comprovantes ou conteúdo de exceção</returns>
        public abstract ComprovanteServiceResponse ConsultarServico(ComprovanteServiceRequest request);
    }
}
