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
using Redecard.PN.Maximo.Modelo.OrdemServico;
using Redecard.PN.Maximo.Modelo.Terminal;
using HelperAutenticacao = Redecard.PN.Maximo.Servicos.Helper.Autenticacao;
using OrdemServicoAutenticacao = Redecard.PN.Maximo.Modelo.OrdemServico.Autenticacao;
using TerminalAutenticacao = Redecard.PN.Maximo.Modelo.Terminal.Autenticacao;
using TerminalNegocio = Redecard.PN.Maximo.Negocio.Terminal;
using System.ServiceModel.Web;
using System.Net;
namespace Redecard.PN.Maximo.Servicos
{
    /// <summary>
    /// Serviço para integração com o Sistema Maximo.
    /// </summary>
    public class MaximoServicoRest : ServicoBase, IMaximoServicoRest
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
        public List<OSRest> ConsultarOS(FiltroOS filtro)
        {
            //Variável de retorno
            List<OSRest> retorno = default(List<OSRest>);
            List<OS> oss;

            using (Logger Log = Logger.IniciarLog("Consultar OS"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    oss = OrdemServico.Instancia.ConsultarOS(autenticacao, filtro);
                    retorno = oss != null && oss.Any()
                            ? oss.Select(s => new OSRest
                                {
                                    ClassificacaoCodigo = s.Classificacao,
                                    DataAtendimento = s.DataAtendimento,
                                    DataProgramada = s.DataProgramada,
                                    DataSituacao = s.DataSituacao,
                                    Fct = s.Fct,
                                    MotivoCancelamento = s.MotivoCancelamento,
                                    Numero = s.Numero,
                                    PrioridadeCodigo = s.Prioridade,
                                    SistemaCodigo = s.Sistema,
                                    SituacaoCodigo = s.Situacao
                                }).ToList()
                            : new List<OSRest>();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public List<OSRest> ConsultarOSAberta(FiltroOS filtro)
        {
            //Variável de retorno
            List<OSRest> retorno = default(List<OSRest>);
            List<OS> oss;

            using (Logger Log = Logger.IniciarLog("Consultar OS Aberta"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    oss = OrdemServico.Instancia.ConsultarOSAberta(autenticacao, filtro);
                    retorno = oss != null && oss.Any()
                            ? oss.Select(s => new OSRest
                                {
                                    ClassificacaoCodigo = s.Classificacao,
                                    DataAtendimento = s.DataAtendimento,
                                    DataProgramada = s.DataProgramada,
                                    DataSituacao = s.DataSituacao,
                                    Fct = s.Fct,
                                    MotivoCancelamento = s.MotivoCancelamento,
                                    Numero = s.Numero,
                                    PrioridadeCodigo = s.Prioridade,
                                    SistemaCodigo = s.Sistema,
                                    SituacaoCodigo = s.Situacao
                                }).ToList()
                           : new List<OSRest>();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public List<OSDetalhadaRest> ConsultarOSDetalhada(FiltroOS filtro)
        {
            //Variável de retorno
            List<OSDetalhadaRest> retorno = default(List<OSDetalhadaRest>);
            List<OSDetalhada> oss;

            using (Logger Log = Logger.IniciarLog("Consultar OS Detalhada"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    oss = OrdemServico.Instancia.ConsultarOSDetalhada(autenticacao, filtro);
                    retorno = oss != null && oss.Any()
                            ? oss.Select(s => new OSDetalhadaRest
                                {
                                    AcaoComercial = s.AcaoComercial,
                                    Aluguel = s.Aluguel == null ? null : new AluguelRest
                                    {
                                        DataInicioCobranca = s.Aluguel.DataInicioCobranca,
                                        Escalonamento = s.Aluguel.Escalonamento == null
                                                        ? null
                                                        : s.Aluguel.Escalonamento.Select(esc => new MesValorRest
                                                        {
                                                            MesCodigo = esc.Mes,
                                                            Valor = esc.Valor
                                                        }).ToList(),
                                        Isento = s.Aluguel.Isento,
                                        Sazonalidade = s.Aluguel.Sazonalidade == null
                                                        ? null
                                                        : s.Aluguel.Sazonalidade.Select(saz => new MesValorRest
                                                        {
                                                            MesCodigo = saz.Mes,
                                                            Valor = saz.Valor
                                                        }).ToList(),
                                        ValorUnitario = s.Aluguel.ValorUnitario
                                    },
                                    Canal = s.Canal,
                                    Celula = s.Celula,
                                    Cenario = s.Cenario,
                                    ClassificacaoCodigo = s.Classificacao,
                                    Cnpj = s.Cnpj,
                                    Contato = s.Contato,
                                    ContatoAlternativo = s.ContatoAlternativo,
                                    DataAtendimento = s.DataAtendimento,
                                    DataProgramada = s.DataProgramada,
                                    DataSituacao = s.DataSituacao,
                                    EnderecoAtendimento = s.EnderecoAtendimento == null ? null : new EnderecoRest
                                    {
                                        Bairro = s.EnderecoAtendimento.Bairro,
                                        Cep = s.EnderecoAtendimento.Cep,
                                        Cidade = s.EnderecoAtendimento.Cidade,
                                        Complemento = s.EnderecoAtendimento.Complemento,
                                        EstadoCodigo = s.EnderecoAtendimento.Estado,
                                        Logradouro = s.EnderecoAtendimento.Logradouro,
                                        Numero = s.EnderecoAtendimento.Logradouro,
                                        PontoReferencia = s.EnderecoAtendimento.PontoReferencia
                                    },
                                    Evento = s.Evento,
                                    Fct = s.Fct,
                                    HorarioAtendimento = s.HorarioAtendimento == null
                                                        ? null
                                                        : s.HorarioAtendimento.Select(hor => new HorarioRest
                                                        {
                                                            DiaCodigo = hor.Dia,
                                                            Inicio = hor.Inicio,
                                                            Termino = hor.Termino
                                                        }).ToList(),
                                    MotivoCancelamento = s.MotivoCancelamento,
                                    Numero = s.Numero,
                                    Observacao = s.Observacao,
                                    Origem = s.Origem,
                                    PontoVenda = s.PontoVenda,
                                    PrioridadeCodigo = s.Prioridade,
                                    ProvedorServico = s.ProvedorServico,
                                    Rede = s.Rede,
                                    SistemaCodigo = s.Sistema,
                                    SituacaoCodigo = s.Situacao,
                                    Terminal = s.Terminal == null
                                                ? null
                                                : s.Terminal.Select(ter => new OSTerminalRest
                                                {
                                                    Acao = ter.Acao,
                                                    FamiliaEquipamento = ter.FamiliaEquipamento,
                                                    Integrador = ter.Integrador,
                                                    Lacrado = ter.Lacrado,
                                                    NumeroLogico = ter.NumeroLogico,
                                                    QuantidadeCheckout = ter.QuantidadeCheckout,
                                                    Renpac = ter.Renpac,
                                                    SoftwareHouse = ter.SoftwareHouse,
                                                    TipoEquipamento = ter.TipoEquipamento,
                                                    VendaDigitada = ter.VendaDigitada

                                                }).ToList()
                                }).ToList()
                            : new List<OSDetalhadaRest>();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public List<OSDetalhadaRest> ConsultarOSAbertaDetalhada(FiltroOS filtro)
        {
            //Variável de retorno
            List<OSDetalhadaRest> retorno = default(List<OSDetalhadaRest>);
            List<OSDetalhada> oss;

            using (Logger Log = Logger.IniciarLog("Consultar OS Aberta Detalhada"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    oss = OrdemServico.Instancia.ConsultarOSDetalhada(autenticacao, filtro);
                    retorno = oss != null && oss.Any()
                            ? oss.Select(s => new OSDetalhadaRest
                                {
                                    AcaoComercial = s.AcaoComercial,
                                    Aluguel = s.Aluguel == null ? null : new AluguelRest
                                    {
                                        DataInicioCobranca = s.Aluguel.DataInicioCobranca,
                                        Escalonamento = s.Aluguel.Escalonamento == null
                                                        ? null
                                                        : s.Aluguel.Escalonamento.Select(esc => new MesValorRest
                                                        {
                                                            MesCodigo = esc.Mes,
                                                            Valor = esc.Valor
                                                        }).ToList(),
                                        Isento = s.Aluguel.Isento,
                                        Sazonalidade = s.Aluguel.Sazonalidade == null
                                                        ? null
                                                        : s.Aluguel.Sazonalidade.Select(saz => new MesValorRest
                                                        {
                                                            MesCodigo = saz.Mes,
                                                            Valor = saz.Valor
                                                        }).ToList(),
                                        ValorUnitario = s.Aluguel.ValorUnitario
                                    },
                                    Canal = s.Canal,
                                    Celula = s.Celula,
                                    Cenario = s.Cenario,
                                    ClassificacaoCodigo = s.Classificacao,
                                    Cnpj = s.Cnpj,
                                    Contato = s.Contato,
                                    ContatoAlternativo = s.ContatoAlternativo,
                                    DataAtendimento = s.DataAtendimento,
                                    DataProgramada = s.DataProgramada,
                                    DataSituacao = s.DataSituacao,
                                    EnderecoAtendimento = s.EnderecoAtendimento == null ? null : new EnderecoRest
                                    {
                                        Bairro = s.EnderecoAtendimento.Bairro,
                                        Cep = s.EnderecoAtendimento.Cep,
                                        Cidade = s.EnderecoAtendimento.Cidade,
                                        Complemento = s.EnderecoAtendimento.Complemento,
                                        EstadoCodigo = s.EnderecoAtendimento.Estado,
                                        Logradouro = s.EnderecoAtendimento.Logradouro,
                                        Numero = s.EnderecoAtendimento.Logradouro,
                                        PontoReferencia = s.EnderecoAtendimento.PontoReferencia
                                    },
                                    Evento = s.Evento,
                                    Fct = s.Fct,
                                    HorarioAtendimento = s.HorarioAtendimento == null
                                                        ? null
                                                        : s.HorarioAtendimento.Select(hor => new HorarioRest
                                                        {
                                                            DiaCodigo = hor.Dia,
                                                            Inicio = hor.Inicio,
                                                            Termino = hor.Termino
                                                        }).ToList(),
                                    MotivoCancelamento = s.MotivoCancelamento,
                                    Numero = s.Numero,
                                    Observacao = s.Observacao,
                                    Origem = s.Origem,
                                    PontoVenda = s.PontoVenda,
                                    PrioridadeCodigo = s.Prioridade,
                                    ProvedorServico = s.ProvedorServico,
                                    Rede = s.Rede,
                                    SistemaCodigo = s.Sistema,
                                    SituacaoCodigo = s.Situacao,
                                    Terminal = s.Terminal == null
                                                ? null
                                                : s.Terminal.Select(ter => new OSTerminalRest
                                                {
                                                    Acao = ter.Acao,
                                                    FamiliaEquipamento = ter.FamiliaEquipamento,
                                                    Integrador = ter.Integrador,
                                                    Lacrado = ter.Lacrado,
                                                    NumeroLogico = ter.NumeroLogico,
                                                    QuantidadeCheckout = ter.QuantidadeCheckout,
                                                    Renpac = ter.Renpac,
                                                    SoftwareHouse = ter.SoftwareHouse,
                                                    TipoEquipamento = ter.TipoEquipamento,
                                                    VendaDigitada = ter.VendaDigitada

                                                }).ToList()
                                }).ToList()
                            : new List<OSDetalhadaRest>();

                    //Filtra as OS abertas (EmCampo ou Encaminhada)
                    retorno = retorno.Where(os => os.SituacaoCodigo == TipoOSSituacao.EMCAMPO ||
                                                  os.SituacaoCodigo == TipoOSSituacao.ENCAMINHADA).ToList();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Detalhada de Ordens de Serviço Abertas (EM CAMPO ou ENCAMINHADA).
        /// </summary>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="pontoVenda">PV do Estabelecimento</param>
        public List<OSDetalhadaRest> ConsultarOSAbertaAtendimento(String pontoVenda)
        {
            //Variável de retorno
            List<OSDetalhadaRest> retorno = default(List<OSDetalhadaRest>);

            using (Logger Log = Logger.IniciarLog("Consultar OS Aberta Detalhada"))
            {
                Log.GravarLog(EventoLog.InicioServico, pontoVenda);

                try
                {
                    //filtro
                    DateTime dtInicioTemp = DateTime.Now.AddDays(-90);
                    DateTime dataInicio = new DateTime(dtInicioTemp.Year, dtInicioTemp.Month, dtInicioTemp.Day, 0, 0, 1);

                    DateTime dtFinalTemp = DateTime.Now;
                    DateTime dataFinal = new DateTime(dtFinalTemp.Year, dtFinalTemp.Month, dtFinalTemp.Day, 23, 59, 59);

                    FiltroOS filtro = new FiltroOS
                    {
                        DataAbertura = new Periodo
                        {
                            Inicio = dataInicio,
                            Termino = dataFinal
                        },
                        PontoVenda = pontoVenda
                    };

                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    List<OSDetalhada> retornoServico = OrdemServico.Instancia.ConsultarOSAbertaDetalhada(autenticacao, filtro);

                    if (retornoServico != null && retornoServico.Any())
                    {

                        retorno = retornoServico.Select(s => new OSDetalhadaRest
                        {
                            AcaoComercial = s.AcaoComercial,
                            Aluguel = s.Aluguel == null ? null : new AluguelRest
                            {
                                DataInicioCobranca = s.Aluguel.DataInicioCobranca,
                                Escalonamento = s.Aluguel.Escalonamento == null
                                                ? null
                                                : s.Aluguel.Escalonamento.Select(esc => new MesValorRest
                                                {
                                                    MesCodigo = esc.Mes,
                                                    Valor = esc.Valor
                                                }).ToList(),
                                Isento = s.Aluguel.Isento,
                                Sazonalidade = s.Aluguel.Sazonalidade == null
                                               ? null
                                               : s.Aluguel.Sazonalidade.Select(saz => new MesValorRest
                                               {
                                                   MesCodigo = saz.Mes,
                                                   Valor = saz.Valor
                                               }).ToList(),
                                ValorUnitario = s.Aluguel.ValorUnitario
                            },
                            Canal = s.Canal,
                            Celula = s.Celula,
                            Cenario = s.Cenario,
                            ClassificacaoCodigo = s.Classificacao,
                            Cnpj = s.Cnpj,
                            Contato = s.Contato,
                            ContatoAlternativo = s.ContatoAlternativo,
                            DataAtendimento = s.DataAtendimento,
                            DataProgramada = s.DataProgramada,
                            DataSituacao = s.DataSituacao,
                            EnderecoAtendimento = s.EnderecoAtendimento == null ? null : new EnderecoRest
                            {
                                Bairro = s.EnderecoAtendimento.Bairro,
                                Cep = s.EnderecoAtendimento.Cep,
                                Cidade = s.EnderecoAtendimento.Cidade,
                                Complemento = s.EnderecoAtendimento.Complemento,
                                EstadoCodigo = s.EnderecoAtendimento.Estado,
                                Logradouro = s.EnderecoAtendimento.Logradouro,
                                Numero = s.EnderecoAtendimento.Logradouro,
                                PontoReferencia = s.EnderecoAtendimento.PontoReferencia
                            },
                            Evento = s.Evento,
                            Fct = s.Fct,
                            HorarioAtendimento = s.HorarioAtendimento == null
                                               ? null
                                               : s.HorarioAtendimento.Select(hor => new HorarioRest
                                               {
                                                   DiaCodigo = hor.Dia,
                                                   Inicio = hor.Inicio,
                                                   Termino = hor.Termino
                                               }).ToList(),
                            MotivoCancelamento = s.MotivoCancelamento,
                            Numero = s.Numero,
                            Observacao = s.Observacao,
                            Origem = s.Origem,
                            PontoVenda = s.PontoVenda,
                            PrioridadeCodigo = s.Prioridade,
                            ProvedorServico = s.ProvedorServico,
                            Rede = s.Rede,
                            SistemaCodigo = s.Sistema,
                            SituacaoCodigo = s.Situacao,
                            Terminal = s.Terminal == null
                                     ? null
                                     : s.Terminal.Select(ter => new OSTerminalRest
                                     {
                                         Acao = ter.Acao,
                                         FamiliaEquipamento = ter.FamiliaEquipamento,
                                         Integrador = ter.Integrador,
                                         Lacrado = ter.Lacrado,
                                         NumeroLogico = ter.NumeroLogico,
                                         QuantidadeCheckout = ter.QuantidadeCheckout,
                                         Renpac = ter.Renpac,
                                         SoftwareHouse = ter.SoftwareHouse,
                                         TipoEquipamento = ter.TipoEquipamento,
                                         VendaDigitada = ter.VendaDigitada

                                     }).ToList()
                        }).ToList();
                    }
                    else if (retorno == null)
                    {
                        retorno = new List<OSDetalhadaRest>();
                    }

                    //Filtra as OS abertas (EmCampo ou Encaminhada)
                    retorno = retorno.Where(os => os.SituacaoCodigo == TipoOSSituacao.EMCAMPO ||
                                                  os.SituacaoCodigo == TipoOSSituacao.ENCAMINHADA).ToList();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public OSCriacaoRetorno CriarOS(OSCriacao ordemServico)
        {
            //Variável de retorno
            OSCriacaoRetorno retorno = new OSCriacaoRetorno();
            DateTime? dataProgramada;

            using (Logger Log = Logger.IniciarLog("Criar OS"))
            {
                Log.GravarLog(EventoLog.InicioServico, ordemServico);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    OrdemServicoAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<OrdemServicoAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    retorno.NumeroOs = OrdemServico.Instancia.CriarOS(autenticacao, ordemServico, out dataProgramada);
                    retorno.DataProgramada = dataProgramada;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }

                Log.GravarLog(EventoLog.RetornoServico, new { retorno });
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
        public List<TerminalConsultaRest> ConsultarTerminal(FiltroTerminal filtro)
        {
            //Variável de retorno
            List<TerminalConsultaRest> retorno = default(List<TerminalConsultaRest>);
            List<TerminalConsulta> terminais;

            using (Logger Log = Logger.IniciarLog("Consultar Terminal"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    TerminalAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<TerminalAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    terminais = TerminalNegocio.Instancia.ConsultarTerminal(autenticacao, filtro);
                    retorno = terminais != null && terminais.Any()
                            ? terminais.Select(s => new TerminalConsultaRest
                                {
                                    OrdemServico = s.OrdemServico,
                                    PontoVenda = s.PontoVenda,
                                    Terminal = new TerminalRest
                                    {
                                        FamiliaEquipamento = s.Terminal.FamiliaEquipamento,
                                        NumeroLogico = s.Terminal.NumeroLogico,
                                        NumeroSerie = s.Terminal.NumeroSerie,
                                        StatusCodigo = s.Terminal.Status,
                                        Tecnologia = s.Terminal.Tecnologia,
                                        TipoEquipamento = s.Terminal.TipoEquipamento,
                                    }
                                }).ToList()
                            : new List<TerminalConsultaRest>();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public List<TerminalDetalhadoRest> ConsultarTerminalDetalhado(FiltroTerminal filtro)
        {
            //Variável de retorno
            List<TerminalDetalhadoRest> retorno = default(List<TerminalDetalhadoRest>);
            List<TerminalDetalhado> terminais;

            using (Logger Log = Logger.IniciarLog("Consultar Terminal Detalhado"))
            {
                Log.GravarLog(EventoLog.InicioServico, filtro);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    TerminalAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<TerminalAutenticacao>();

                    //Criação de classe de negócio e chamada de método
                    terminais = TerminalNegocio.Instancia.ConsultarTerminalDetalhado(autenticacao, filtro);
                    retorno = terminais != null && terminais.Any()
                            ? terminais.Select(s => new TerminalDetalhadoRest
                                {
                                    AlteradoPor = s.AlteradoPor,
                                    Bloqueado = s.Bloqueado,
                                    Caracteristica = s.Caracteristica,
                                    CargaPendente = s.CargaPendente,
                                    Chip = s.Chip,
                                    CodigoRede = s.CodigoRede,
                                    DataAtualizacao = s.DataAtualizacao,
                                    DataAtualizacaoTg = s.DataAtualizacaoTg,
                                    DataCompra = s.DataCompra,
                                    DataInstalacao = s.DataInstalacao,
                                    DataRecebimento = s.DataRecebimento,
                                    Fabricante = s.Fabricante,
                                    FamiliaEquipamento = s.FamiliaEquipamento,
                                    Flex = s.Flex,
                                    Inativo = s.Inativo,
                                    InicializacaoPendente = s.InicializacaoPendente,
                                    Integrador = s.Integrador,
                                    NumeroAtivoSap = s.NumeroAtivoSap,
                                    NumeroLogico = s.NumeroLogico,
                                    NumeroSerie = s.NumeroSerie,
                                    PlacaTrocada = s.PlacaTrocada,
                                    Posicao = s.Posicao,
                                    ProprietarioCodigo = s.Proprietario,
                                    Reintegrado = s.Reintegrado,
                                    Sazonal = s.Sazonal,
                                    StatusCodigo = s.Status,
                                    Tecnologia = s.Tecnologia,
                                    TipoConexaoCodigo = s.TipoConexao,
                                    TipoEquipamento = s.TipoEquipamento,
                                    VendaDigitada = s.VendaDigitada,
                                    VersaoKernel = s.VersaoKernel,
                                    VersaoSoftware = s.VersaoSoftware
                                }).ToList()
                            : new List<TerminalDetalhadoRest>();

                    //Garante filtro por Status
                    if (filtro.Situacao.HasValue)
                        retorno = retorno.Where(terminal => terminal.StatusCodigo == filtro.Situacao.Value).ToList();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Detalhada de Terminais Apenas POS e POO.
        /// </summary>
        /// <param name="autenticacao">Dados para autenticação no Sistema Máximo</param>
        /// <param name="pontoVenda">Dados para autenticação no Sistema Máximo</param>
        public List<TerminalDetalhadoRest> ConsultarTerminalAtendimento(String pontoVenda)
        {
            //Variável de retorno
            List<TerminalDetalhadoRest> retorno = default(List<TerminalDetalhadoRest>);

            using (Logger Log = Logger.IniciarLog("Consultar Terminal Detalhado"))
            {
                Log.GravarLog(EventoLog.InicioServico, pontoVenda);

                try
                {
                    FiltroTerminal filtro = new FiltroTerminal
                    {
                        PontoVenda = pontoVenda,
                        Situacao = TipoTerminalStatus.EMPRODUCAO
                    };

                    List<TerminalDetalhado> retornoTemp = new List<TerminalDetalhado>();

                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    TerminalAutenticacao autenticacao = HelperAutenticacao.ObterAutenticacao<TerminalAutenticacao>();

                    List<TerminalDetalhado> retornoPoo = default(List<TerminalDetalhado>);
                    List<TerminalDetalhado> retornoPos = default(List<TerminalDetalhado>);

                    //Criação de classe de negócio e chamada de método - POO
                    try
                    {
                        filtro.TipoEquipamento = "POO";
                        retornoPoo = TerminalNegocio.Instancia.ConsultarTerminalDetalhado(autenticacao, filtro);
                    }
                    catch (PortalRedecardException ex)
                    {
                        if (ex.Codigo == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (CODIGO_ERRO == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //Criação de classe de negócio e chamada de método - POS
                    try
                    {
                        filtro.TipoEquipamento = "POS";
                        retornoPos = TerminalNegocio.Instancia.ConsultarTerminalDetalhado(autenticacao, filtro);
                    }
                    catch (PortalRedecardException ex)
                    {
                        if (ex.Codigo == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (CODIGO_ERRO == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }
                    }


                    if (retornoPoo != null && retornoPoo.Any())
                    {
                        retornoTemp.AddRange(retornoPoo);
                    }

                    if (retornoPos != null && retornoPos.Any())
                    {
                        retornoTemp.AddRange(retornoPos);
                    }

                    //Garante filtro por Status
                    if (filtro.Situacao.HasValue)
                        retorno = retornoTemp.Where(terminal => terminal.Status == filtro.Situacao.Value).Select(s => new TerminalDetalhadoRest
                        {
                            AlteradoPor = s.AlteradoPor,
                            Bloqueado = s.Bloqueado,
                            Caracteristica = s.Caracteristica,
                            CargaPendente = s.CargaPendente,
                            Chip = s.Chip,
                            CodigoRede = s.CodigoRede,
                            DataAtualizacao = s.DataAtualizacao,
                            DataAtualizacaoTg = s.DataAtualizacaoTg,
                            DataCompra = s.DataCompra,
                            DataInstalacao = s.DataInstalacao,
                            DataRecebimento = s.DataRecebimento,
                            Fabricante = s.Fabricante,
                            FamiliaEquipamento = s.FamiliaEquipamento,
                            Flex = s.Flex,
                            Inativo = s.Inativo,
                            InicializacaoPendente = s.InicializacaoPendente,
                            Integrador = s.Integrador,
                            NumeroAtivoSap = s.NumeroAtivoSap,
                            NumeroLogico = s.NumeroLogico,
                            NumeroSerie = s.NumeroSerie,
                            PlacaTrocada = s.PlacaTrocada,
                            Posicao = s.Posicao,
                            ProprietarioCodigo = s.Proprietario,
                            Reintegrado = s.Reintegrado,
                            Sazonal = s.Sazonal,
                            StatusCodigo = s.Status,
                            Tecnologia = s.Tecnologia,
                            TipoConexaoCodigo = s.TipoConexao,
                            TipoEquipamento = s.TipoEquipamento,
                            VendaDigitada = s.VendaDigitada,
                            VersaoKernel = s.VersaoKernel,
                            VersaoSoftware = s.VersaoSoftware
                        }).ToList();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
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
        public bool PossuiTerminalComBobina(String pontoVenda)
        {
            //Variável de retorno
            List<TerminalConsulta> retorno = default(List<Modelo.Terminal.TerminalConsulta>);

            using (Logger Log = Logger.IniciarLog("Consultar Terminal Detalhado"))
            {
                Log.GravarLog(EventoLog.InicioServico, pontoVenda);

                try
                {
                    //Obtenção das credenciais do Portal PN para acesso ao Máximo
                    var autenticacao = Helper.Autenticacao.ObterAutenticacao<Modelo.Terminal.Autenticacao>();

                    //Criação de classe de negócio e chamada de método
                    try
                    {
                        retorno = TerminalNegocio.Instancia.ConsultarTerminal(autenticacao, new Modelo.Terminal.FiltroTerminal
                        {
                            PontoVenda = pontoVenda,
                            Situacao = Modelo.Terminal.TipoTerminalStatus.EMPRODUCAO
                        });
                    }
                    catch (PortalRedecardException ex)
                    {
                        if (ex.Codigo == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (CODIGO_ERRO == 691001)
                        {
                            Log.GravarErro(ex);
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //Garante filtro por Status
                    if (retorno != null && retorno.Any(a => new List<String> { "POO", "POS", "POY" }.Contains(a.Terminal.TipoEquipamento)))
                    {
                        return true;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new WebFaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE, base.RecuperarExcecao(ex)), HttpStatusCode.Forbidden);
                }

                Log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return false;
        }

        #endregion
    }
}