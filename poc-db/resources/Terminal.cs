/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.Maximo.Agentes.TGATerminal;
using Redecard.PN.Maximo.Modelo.Terminal;

namespace Redecard.PN.Maximo.Agentes
{
    /// <summary>
    /// Classe de acesso externo para integração com o sistema Máximo para acesso
    /// às informações dos Terminais.
    /// </summary>
    public class Terminal : AgentesBaseMaximo<ITerminal, Terminal>, ITerminal
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
        public List<TerminalConsulta> ConsultarTerminal(
            Autenticacao autenticacao,
            FiltroTerminal filtro)
        {
            try
            {
                //Mapeamento para modelos do Serviço Máximo
                var autenticacaoTGA = Mapper.Map<t_autenticacao>(autenticacao);
                var filtroTGA = Mapper.Map<t_filtro_terminal>(filtro);
                var retornoTGA = default(t_terminal_consulta[]);

                //Chamada do serviço externo (Máximo)
                using (var contexto = new ContextoWCF<TGATerminalClient>())
                    retornoTGA = contexto.Cliente.ConsultarTerminal(autenticacaoTGA, filtroTGA);

                //Tratamento e mapeamento do retorno dos dados para modelos da aplicação
                var retorno = Mapper.Map<List<TerminalConsulta>>(retornoTGA);

                //Retorno dos dados
                return retorno;
            }
            catch (FaultException ex)
            {
                throw new PortalRedecardException(ObterCodigo(ex.Code.Name), "TGATerminal.ConsultarTerminal", ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }  
        }

        /// <summary>
        /// Consulta Detalhada de Terminais.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de terminais</param>
        public List<TerminalDetalhado> ConsultarTerminalDetalhado(
            Autenticacao autenticacao,
            FiltroTerminal filtro)
        {
            try
            {
                //Mapeamento para modelos do Serviço Máximo
                var autenticacaoTGA = Mapper.Map<t_autenticacao>(autenticacao);
                var filtroTGA = Mapper.Map<t_filtro_terminal>(filtro);
                var retornoTGA = default(t_terminal_detalhado[]);

                //Chamada do serviço externo (Máximo)
                using (var contexto = new ContextoWCF<TGATerminalClient>())
                    retornoTGA = contexto.Cliente.ConsultarTerminalDetalhado(autenticacaoTGA, filtroTGA);

                //Tratamento e mapeamento do retorno dos dados para modelos da aplicação
                var retorno = Mapper.Map<List<TerminalDetalhado>>(retornoTGA);

                //Retorno dos dados
                return retorno;
            }
            catch (FaultException ex)
            {
                throw new PortalRedecardException(ObterCodigo(ex.Code.Name), "TGATerminal.ConsultarTerminalDetalhado", ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}