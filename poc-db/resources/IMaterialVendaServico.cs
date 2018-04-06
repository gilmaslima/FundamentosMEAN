#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Contrato do serviço de Material de Venda
    /// </summary>
    [ServiceContract]
    public interface IMaterialVendaServico
    {
        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Remessa> ConsultarUltimasRemessas(Int32 codigoPV);

        /// <summary>
        /// Lista as próximas remessas enviadas para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Remessa> ConsultarProximasRemessas(Int32 codigoPV);

        /// <summary>
        /// Consultar a composição de um Kit para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Material> ConsultarComposicaoKit(Int32 codigoKit);

        /// <summary>
        /// Lista os motivos das remessas de Material de venda
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Motivo> ConsultarMotivos();

        /// <summary>
        /// Método consultar kits de material de vendas para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Kit> ConsultarKitsVendas(Int32 numeroPV);

        /// <summary>
        /// Método consultar kits de material de sinalização para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Kit> ConsultarKitsSinalizacao(Int32 numeroPV);

        /// <summary>
        /// Método consultar kits de material de tecnologia para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Kit> ConsultarKitsTecnologia(Int32 numeroPV);

        /// <summary>
        /// Método consultar kits de material de apoio para o estabelecimento
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Kit> ConsultarKitsApoio(Int32 numeroPV);

        /// <summary>
        /// Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarQuantidadeMaximaKitsSinalizacao(out Int32 codigoRetorno);

        /// <summary>
        /// Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarQuantidadeMaximaKitsApoio(out Int32 codigoRetorno);

        /// <summary>
        /// Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do
        /// estabelecimento especificado.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 IncluirKit(Kit[] kits, Int32 codigoPV, String descricaoPV, String usuario, String solicitante, Boolean enderecoTemporario, Modelo.Endereco endereco = null);
    }
}
