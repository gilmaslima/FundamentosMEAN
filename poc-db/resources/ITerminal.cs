/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Collections.Generic;
using Redecard.PN.Maximo.Modelo.Terminal;

namespace Redecard.PN.Maximo.Agentes
{
    public interface ITerminal
    {
        /// <summary>
        /// Consulta de Terminais.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        List<TerminalConsulta> ConsultarTerminal(
            Autenticacao autenticacao,
            FiltroTerminal filtro);

        /// <summary>
        /// Consulta Detalhada de Terminais.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        List<TerminalDetalhado> ConsultarTerminalDetalhado(
            Autenticacao autenticacao,
            FiltroTerminal filtro);
    }
}
