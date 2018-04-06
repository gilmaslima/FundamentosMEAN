/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Maximo.Modelo;

namespace Redecard.PN.Maximo.Servicos
{
    /// <summary>
    /// Interface de serviço para integração com o Sistema Maximo.
    /// </summary>
    [ServiceContract]
    public interface IMaximoServico
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
        List<Modelo.OrdemServico.OS> ConsultarOS(
            Modelo.OrdemServico.FiltroOS filtro);

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
        List<Modelo.OrdemServico.OS> ConsultarOSAberta(
            Modelo.OrdemServico.FiltroOS filtro);

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
        List<Modelo.OrdemServico.OSDetalhada> ConsultarOSDetalhada(
            Modelo.OrdemServico.FiltroOS filtro);

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
        List<Modelo.OrdemServico.OSDetalhada> ConsultarOSAbertaDetalhada(
            Modelo.OrdemServico.FiltroOS filtro);

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
        String CriarOS(
            Modelo.OrdemServico.OSCriacao ordemServico,
            out DateTime? dataProgramada);

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
        List<Modelo.Terminal.TerminalConsulta> ConsultarTerminal(
            Modelo.Terminal.FiltroTerminal filtro);

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
        List<Modelo.Terminal.TerminalDetalhado> ConsultarTerminalDetalhado(
            Modelo.Terminal.FiltroTerminal filtro);
        
        /// <summary>
        /// Consulta de Terminais que necessitam de bobina.
        /// </summary>
        /// <remarks>
        ///     Histórico: 10/05/2017 - Criação do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        bool PossuiTerminalComBobina(String pontoVenda);

        #endregion
    }
}