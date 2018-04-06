using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    [DataContract]
    public class TecnologiaResponse
    {
        /// <summary>
        /// Código da Tecnlogia do Estabelecimento
        /// </summary>
        [DataMember]
        public Int32 CodigoTecnologia { get; set; }

        /// <summary>
        /// Status de Retorno da Consulta
        /// </summary>
        [DataMember]
        public StatusRetorno StatusRetorno { get; set; }
    }
}