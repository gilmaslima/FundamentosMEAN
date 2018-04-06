/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Maximo.Modelo;
using Redecard.PN.Maximo.Negocio;

namespace Redecard.PN.Maximo.Servicos
{
    /// <summary>
    /// Serviço para integração com o Sistema Maximo.
    /// </summary>
    public class MaximoServico : ServicoBase, IMaximoServico
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
        public List<Modelo.OrdemServico.OS> ConsultarOS(            
            Modelo.OrdemServico.FiltroOS filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.OrdemServico.OS>);

            using (Logger Log = Logger.IniciarLog("Consultar OS"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);
                
                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.OrdemServico.Autenticacao>();

                    //Criação de classe de negócio e chamada de método
                    retorno = OrdemServico.Instancia.ConsultarOS(autenticacao, filtro);
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        public List<Modelo.OrdemServico.OS> ConsultarOSAberta(
            Modelo.OrdemServico.FiltroOS filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.OrdemServico.OS>);

            using (Logger Log = Logger.IniciarLog("Consultar OS Aberta"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.OrdemServico.Autenticacao>();
                    
                    //Criação de classe de negócio e chamada de método
                    retorno = OrdemServico.Instancia.ConsultarOSAberta(autenticacao, filtro);
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        public List<Modelo.OrdemServico.OSDetalhada> ConsultarOSDetalhada(            
            Modelo.OrdemServico.FiltroOS filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.OrdemServico.OSDetalhada>);

            using (Logger Log = Logger.IniciarLog("Consultar OS Detalhada"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.OrdemServico.Autenticacao>();
                                        
                    //Criação de classe de negócio e chamada de método
                    retorno = OrdemServico.Instancia.ConsultarOSDetalhada(autenticacao, filtro);
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="filtro">Filtro para consulta de ordens de serviço</param>
        public List<Modelo.OrdemServico.OSDetalhada> ConsultarOSAbertaDetalhada(            
            Modelo.OrdemServico.FiltroOS filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.OrdemServico.OSDetalhada>);

            using (Logger Log = Logger.IniciarLog("Consultar OS Aberta Detalhada"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.OrdemServico.Autenticacao>();
                    
                    //Criação de classe de negócio e chamada de método
                    retorno = OrdemServico.Instancia.ConsultarOSDetalhada(autenticacao, filtro);

                    if(retorno == null)
                        retorno = new List<Modelo.OrdemServico.OSDetalhada>();

                    //Filtra as OS abertas (EmCampo ou Encaminhada)
                    retorno = retorno.Where(os => os.Situacao == Modelo.OrdemServico.TipoOSSituacao.EMCAMPO
                        || os.Situacao == Modelo.OrdemServico.TipoOSSituacao.ENCAMINHADA).ToList();
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

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
        public String CriarOS(            
            Modelo.OrdemServico.OSCriacao ordemServico,
            out DateTime? dataProgramada)
        {
            //Variável de retorno
            var retorno = default(String);

            using (Logger Log = Logger.IniciarLog("Criar OS"))
            {
                Log.GravarLog(EventoLog.InicioServico, ordemServico);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.OrdemServico.Autenticacao>();

                    //Criação de classe de negócio e chamada de método
                    retorno = OrdemServico.Instancia.CriarOS(autenticacao, ordemServico, out dataProgramada);
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

                Log.GravarLog(EventoLog.RetornoServico, new { retorno, dataProgramada });
            }

            return retorno;
        }

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
        public List<Modelo.Terminal.TerminalConsulta> ConsultarTerminal(            
            Modelo.Terminal.FiltroTerminal filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.Terminal.TerminalConsulta>);

            using (Logger Log = Logger.IniciarLog("Consultar Terminal"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.Terminal.Autenticacao>();
                    
                    //Criação de classe de negócio e chamada de método
                    retorno = Terminal.Instancia.ConsultarTerminal(autenticacao, filtro);
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
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
        public List<Modelo.Terminal.TerminalDetalhado> ConsultarTerminalDetalhado(            
            Modelo.Terminal.FiltroTerminal filtro)
        {
            //Variável de retorno
            var retorno = default(List<Modelo.Terminal.TerminalDetalhado>);

            using (Logger Log = Logger.IniciarLog("Consultar Terminal Detalhado"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.Terminal.Autenticacao>();
                    
                    //Criação de classe de negócio e chamada de método
                    retorno = Terminal.Instancia.ConsultarTerminalDetalhado(autenticacao, filtro);

                    //Garante filtro por Status
                    if(filtro.Situacao.HasValue)
                        retorno = retorno.Where(terminal => terminal.Status == filtro.Situacao.Value).ToList();
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta de Terminais que necessitam de bobina.
        /// </summary>
        /// <remarks>
        ///     Histórico: 10/05/2017 - Criação do método
        /// </remarks>
        /// <param name="pontovenda">PV do Estabelecimento</param>
        public bool PossuiTerminalComBobina(String pontoVenda) {
            //Variável de retorno
            var retorno = default(List<Modelo.Terminal.TerminalConsulta>);

            using (Logger Log = Logger.IniciarLog("Consultar Terminal Detalhado"))
            {
                Log.GravarLog(EventoLog.InicioServico, pontoVenda);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.Terminal.Autenticacao>();

                    //Criação de classe de negócio e chamada de método
                    retorno = Terminal.Instancia.ConsultarTerminal(autenticacao, new Modelo.Terminal.FiltroTerminal { 
                        PontoVenda = pontoVenda,
                        Situacao = Modelo.Terminal.TipoTerminalStatus.EMPRODUCAO
                    });

                    //Garante filtro por Status
					//"POO", "POS", "POY" e todos que não constam na lista abaixo, informada pela equipe TGA.
                    //if (retorno != null && !retorno.Any(a => new List<String> { "SNT", "TOL", "TOF", "PDV", "MPO", "CPC", "LCB" }.Contains(a.Terminal.TipoEquipamento)))
                    if (retorno != null && retorno.FirstOrDefault(a => !(new List<string> { "SNT", "TOL", "TOF", "PDV", "MPO", "CPC", "LCB" }).Contains(a.Terminal.TipoEquipamento)) != null)
                        return true;
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

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return false;
        }
        #endregion
    }
}