using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Interface do Serviço de Solicitações
    /// </summary>
    [ServiceContract]
    public interface ISolicitacaoServico
    {
        /// <summary>
        /// Inclui uma nova solicitação para o numero do PV
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 IncluirSolicitacao(String ocorrencia, Int32 numeroPV, String motivo, String descricaoCaso, List<KeyValuePair<String, String>> vencimentos, List<KeyValuePair<String, String>> formaEnvio);

        /// <summary>
        /// Consultar modo de envio de resposta para o ocorrência
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarModoEnvio(String ocorrencia);

        /// <summary>
        /// Consulta lista de ocorrências para solicitação
        /// </summary>
        /// <returns>Lista de ocorrências de solicitação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Ocorrencia> ConsultarOcorrencias();

        /// <summary>
        /// Consulta as solicitações abertas de acordo com os parâmetros
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="periodoInicio">Data inicial do período de busca</param>
        /// <param name="periodoFim">Data final do período de busca</param>
        /// <param name="tipoOcorrencia">Tipo de Ocorrência de solicitação</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações abertas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Solicitacao> Consultar(Int32 numeroSolicitacao, DateTime periodoInicio, DateTime periodoFim,
                        String tipoOcorrencia, Int32 codigoEntidade);

        /// <summary>
        /// Consulta lista de motivos para o ocorrência
        /// </summary>
        /// <returns>Lista de motivos de solicitação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Motivo> ConsultarMotivos(String ocorrencia);

        /// <summary>
        /// Consulta lista de propriedades para o ocorrência
        /// </summary>
        /// <returns>Lista de propriedades da ocorrência</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Propriedade> ConsultarPropriedades(String ocorrencia);

        /// <summary>
        /// Consulta a descrição de caso da Solicitação
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso</param>
        /// <returns>Descrição de Caso da Solicitação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarDescricaoCaso(Int32 numeroSolicitacao, Int32 codigoCaso);

        /// <summary>
        /// Consulta as ocorrências da Solicitação
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Lista de ocorrências da Solicitação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Solicitacao> ConsultarOcorrenciasSolicitacao(Int32 numeroSolicitacao, Int32 codigoCaso);

        /// <summary>
        /// Consulta os pré-requisitos de uma Solicitação cadastrada
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Lista de pré-requisitos da Solicitação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<PreRequisitoSolicitacao> ConsultarPreRequisitosSolicitacao(Int32 numeroSolicitacao, Int32 codigoCaso);

        /// <summary>
        /// Consultar o status da carta
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Modelo de carta com informações de status</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        CartaSolicitacao ConsultarStatusCarta(Int32 numeroSolicitacao, Int32 codigoCaso);

        /// <summary>
        /// Consulta os destinários e formas de resposta da carta de solicitação
        /// </summary>
        /// <param name="numeroSolicitacao"></param>
        /// <returns>Lista de formas de envio</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CartaSolicitacao> ConsultarFormaRespostaCarta(Int32 numeroSolicitacao);
    }
}
