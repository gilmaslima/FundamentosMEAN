#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [16/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using AutoMapper;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    public class RegimeFranquiaServico : ServicoBase, IRegimeFranquiaServico
    {
        /// <summary>
        /// Consultar Restrição de Serviço para a Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Dados da Restrição de Regime</returns>
        public RestricaoRegime ConsultarRestricaoRegime(Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            Modelo.RestricaoRegime restricaoModelo = new Modelo.RestricaoRegime();
            Servicos.RestricaoRegime restricaoServico = null;
            using (Logger Log = Logger.IniciarLog("Consultar Restrição de Serviço para a Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var regimeNegocios = new Negocio.Regime();
                    Mapper.CreateMap<Modelo.RestricaoRegime, Servicos.RestricaoRegime>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    restricaoModelo = regimeNegocios.ConsultarRestricaoRegime(codigoEntidade, codigoGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { restricaoModelo });

                    restricaoServico = Mapper.Map<Modelo.RestricaoRegime, Servicos.RestricaoRegime>(restricaoModelo);
                    Log.GravarLog(EventoLog.FimServico, new { restricaoServico });

                    return restricaoServico;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Busca o conteúdo do contrato de acordo com o código da Versão de Regime atual
        /// </summary>
        /// <param name="codigoVersao">Código da versão do regime atual</param>
        /// <returns></returns>
        public RestricaoRegime ConsultarContratoRestricao(Int32 codigoVersao)
        {
            Modelo.RestricaoRegime restricaoModelo = new Modelo.RestricaoRegime();
            Servicos.RestricaoRegime restricaoServico = null;
            using (Logger Log = Logger.IniciarLog("Busca o conteúdo do contrato de acordo com o código da Versão de Regime atual"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var regimeNegocios = new Negocio.Regime();
                    Mapper.CreateMap<Modelo.RestricaoRegime, Servicos.RestricaoRegime>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoVersao });

                    restricaoModelo = regimeNegocios.ConsultarContratoRestricao(codigoVersao);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { restricaoModelo });


                    restricaoServico = Mapper.Map<Modelo.RestricaoRegime, Servicos.RestricaoRegime>(restricaoModelo);
                    Log.GravarLog(EventoLog.FimServico, new { restricaoServico });

                    return restricaoServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Consulta Código de Regime atual da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de erro retornado pela proc</param>
        /// <returns>Código do Regime</returns>
        public Int32 ConsultarCodigoRegime(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            Int32 codigoRegime = 0;
            using (Logger Log = Logger.IniciarLog("Consulta Código de Regime atual da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var regimeNegocio = new Negocio.Regime();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });

                    codigoRegime = regimeNegocio.ConsultarCodigoRegime(codigoEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoEntidade, codigoRegime });

                    Log.GravarLog(EventoLog.FimServico, new { codigoRegime });
                    return codigoRegime;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Atualiza o Regime de franquia da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRegime">Código do regime</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoUsuario">Usuário responsável pela atualização</param>
        /// <returns>Código de erro retornado da procedure spge0362</returns>
        public Int32 AtualizarFranquia(Int32 codigoEntidade, Int32 codigoRegime, Int32 codigoCelula, Int32 codigoCanal,
            String codigoUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza o Regime de franquia da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var regimeNegocio = new Negocio.Regime();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoRegime, codigoCelula, codigoCanal, codigoUsuario });
                    var result = regimeNegocio.AtualizarFranquia(codigoEntidade, codigoRegime, codigoCelula, codigoCanal, codigoUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {

                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inclui/Atualiza a confirmação do contrato Franquia
        /// </summary>
        /// <param name="codigoVersao">Código de versão</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário responsável pela inclusão/atualização</param>
        /// <returns>0</returns>
        public Int16 IncluirAceite(Int32 codigoVersao, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Inclui/Atualiza a confirmação do contrato Franquia"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var regimeNegocio = new Negocio.Regime();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoVersao, codigoGrupoEntidade, codigoEntidade, codigoUsuario });
                    var result = regimeNegocio.IncluirAceite(codigoVersao, codigoGrupoEntidade, codigoEntidade, codigoUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        /// <summary>
        /// Inclui/Atualiza a confirmação do usuário
        /// </summary>
        /// <param name="codigoRestricao">Código de restrição</param>
        /// <param name="codigoVersao">Código de versão</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário responsável pela inclusão/atualização</param>
        /// <returns>0</returns>
        public Int16 IncluirAceiteUsuario(Int32 codigoRestricao, Int32 codigoVersao, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Inclui/Atualiza a confirmação do usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var regimeNegocio = new Negocio.Regime();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoRestricao, codigoVersao, codigoGrupoEntidade, codigoEntidade, codigoUsuario });
                    var result = regimeNegocio.IncluirAceite(codigoRestricao, codigoVersao, codigoGrupoEntidade, codigoEntidade, codigoUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
    }
}
