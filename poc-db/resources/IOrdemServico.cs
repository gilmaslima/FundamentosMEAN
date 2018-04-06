/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using Redecard.PN.Maximo.Modelo.OrdemServico;

namespace Redecard.PN.Maximo.Negocio
{
    /// <summary>
    /// Interface para integração com o sistema Máximo para acesso
    /// às informações das Ordens de Serviço.
    /// </summary>
    public interface IOrdemServico
    {
        /// <summary>
        /// Consulta de Ordens de Serviço.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        List<OS> ConsultarOS(
            Autenticacao autenticacao,
            FiltroOS filtro);

        /// <summary>
        /// Consulta de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        List<OS> ConsultarOSAberta(
            Autenticacao autenticacao,
            FiltroOS filtro);

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        List<OSDetalhada> ConsultarOSDetalhada(
            Autenticacao autenticacao,
            FiltroOS filtro);

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        List<OSDetalhada> ConsultarOSAbertaDetalhada(
            Autenticacao autenticacao,
            FiltroOS filtro);

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
        String CriarOS(
            Autenticacao autenticacao,
            OSCriacao ordemServico,
            out DateTime? dataProgramada);
    }
}