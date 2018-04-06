using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// BaseResponse
    /// </summary>
    [DataContract]
    public class BaseResponse
    {
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public BaseResponse()
        {
            this.StatusRetorno = new StatusRetorno();
        }

        /// <summary>
        /// Construtor iniciando o código de retorno
        /// </summary>
        /// <param name="codigoRetorno"></param>
        public BaseResponse(Int32 codigoRetorno)
        {
            this.StatusRetorno = new StatusRetorno()
            {
                CodigoRetorno = codigoRetorno
            };
        }

        /// <summary>
        /// Construtor iniciando o objeto de StatusRetorno
        /// </summary>
        /// <param name="status"></param>
        public BaseResponse(StatusRetorno status)
        {
            this.StatusRetorno = status;
        }

        /// <summary>
        /// Status de Retorno da Consulta
        /// </summary>
        [DataMember]
        public StatusRetorno StatusRetorno { get; set; }
    }
}