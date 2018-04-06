/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Redecard.PN.Maximo.Modelo;
using Redecard.PN.Maximo.Modelo.OrdemServico;
using Redecard.PN.Maximo.Modelo.Terminal;

namespace Redecard.PN.Maximo.Servicos
{
    /// <summary>
    /// Interface de serviço para integração com o Sistema Maximo.
    /// </summary>
    [ServiceContract]
    public interface IMaximoServicoRest
    {        
        #region [ Ordem Serviço ]

        /// <summary>
        /// Consulta de Ordens de Serviço.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarOS")]
        List<OSRest> ConsultarOS(FiltroOS filtro);

        /// <summary>
        /// Consulta de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarOSAberta")]
        List<OSRest> ConsultarOSAberta(FiltroOS filtro);

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarOSDetalhada")]
        List<OSDetalhadaRest> ConsultarOSDetalhada(FiltroOS filtro);

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarOSAbertaDetalhada")]
        List<OSDetalhadaRest> ConsultarOSAbertaDetalhada(FiltroOS filtro);

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="pontoVenda">PV do Estabelecimento</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarOSAbertaAtendimento/{pontoVenda}")]
        List<OSDetalhadaRest> ConsultarOSAbertaAtendimento(String pontoVenda);

        /// <summary>
        /// Criação de Ordem de Serviço
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="ordemServico">Ordem de serviço que será criada</param>
        /// <param name="dataProgramada">Data programada para atendimento da OS</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
           Method = "POST",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "CriarOS")]
        OSCriacaoRetorno CriarOS(OSCriacao ordemServico);

        #endregion

        #region [ Terminal ]

        /// <summary>
        /// Consulta de Terminais.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
          Method = "POST",
          BodyStyle = WebMessageBodyStyle.Bare,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
         UriTemplate = "ConsultarTerminal")]
        List<TerminalConsultaRest> ConsultarTerminal(FiltroTerminal filtro);

        /// <summary>
        /// Consulta Detalhada de Terminais.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
          Method = "POST",
          BodyStyle = WebMessageBodyStyle.Bare,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
         UriTemplate = "ConsultarTerminalDetalhado")]
        List<TerminalDetalhadoRest> ConsultarTerminalDetalhado(FiltroTerminal filtro);

        /// <summary>
        /// Consulta Detalhada de Terminais Apenas POS e POO.
        /// </summary>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="pontoVenda">PV do Estabelecimento</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
          Method = "GET",
          BodyStyle = WebMessageBodyStyle.Bare,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
         UriTemplate = "ConsultarTerminalAtendimento/{pontoVenda}")]
        List<TerminalDetalhadoRest> ConsultarTerminalAtendimento(String pontoVenda);

        /// <summary>
        /// Consulta de Terminais que necessitam de bobina.
        /// </summary>
        /// <remarks>
        ///     Histórico: 10/05/2017 - Criação do método
        /// </remarks>
        /// <param name="pontovenda">PV do Estabelecimento</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
          Method = "GET",
          BodyStyle = WebMessageBodyStyle.Bare,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
         UriTemplate = "PossuiTerminalComBobina/{pontoVenda}")]
        bool PossuiTerminalComBobina(String pontoVenda);

        #endregion
    }
}