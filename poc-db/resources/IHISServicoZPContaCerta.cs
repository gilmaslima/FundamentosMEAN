using Redecard.PN.OutrosServicos.Servicos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Interface de Serviço para expor os métodos de consulta mainframe do módulo Conta Certa
    /// </summary>
    [ServiceContract]
    public interface IHISServicoZPContaCerta
    {
        /// <summary>
        /// Consulta detalhe de ofertas.<br/>        
        /// - Book ZPL05000 / Programa ZPC050 / TranID ZPC0 / Método: ConsultarContratoVigencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPL05000 / Programa ZPC050 / TranID ZPC0 / Método: ConsultarContratoVigencia
        /// </remarks>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="dataFimVigencia"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Detalhe da oferta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DetalheOferta ConsultarContratoVigencia(Int32 numeroPV,
                                                Int16 codSitContrato,
                                                DateTime dataFimVigencia,
                                                out Int16 codRetorno);

        /// <summary>
        /// Consulta historico do detalhe de ofertas.<br/>        
        /// - Book ZPL05100 / Programa ZPC051 / TranID ZPC1 / Método: ConsultarContratoHistorico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPL05100 / Programa ZPC051 / TranID ZPC1 / Método: ConsultarContratoHistorico
        /// </remarks>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Lista de histórico do detalhe de oferta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DetalheOferta> ConsultarContratoHistorico(Int32 numeroPV,
                                                Int16 codSitContrato,
                                                out Int16 codRetorno);

        /// <summary>
        /// Verifica se o estabelecimento contém um histórico de ofertas
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <param name="codSitContrato"></param>
        /// <param name="codRetorno"></param>
        /// <returns>Verdadeiro ou falso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ContemHistorico(Int32 numeroPV, Int16 codSitContrato, out Int16 codRetorno);
    }
}
