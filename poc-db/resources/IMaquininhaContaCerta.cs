using Redecard.PN.OutrosServicos.Modelo;
using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Contrato do serviço de oferta maquininha conta certa
    /// </summary>
    [ServiceContract]
    public interface IMaquininhaContaCerta
    {
        /// <summary>
        /// Consulta o contrato da oferta maquininha conta certa
        /// </summary>
        /// <param name="numEstabelecimento">Código do estabelecimento</param>
        /// <param name="dataFimVigencia">Data fim de vigência</param>
        /// <param name="codigoStatusContrato">Código da situação do contrato</param>
        /// <returns>Sumário do contrato maquininha conta certa</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        MaquininhaContrato ConsultaContrato(Int32 numEstabelecimento, DateTime dataFimVigencia, Int16? codigoStatusContrato);

        /// <summary>
        /// Consulta as faixas de faturamento das metas do contrato maquininha conta certa
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Faixa de faturamento das metas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<MaquininhaMetas> ConsultaMetas(Int32 numPdv);

        /// <summary>
        /// Consulta o histórico de apuração
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Lista com o histórico de apuração</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<MaquininhaHistoricoApuracao> ConsultaHistoricoApuracao(Int32 numPdv);
    }
}
