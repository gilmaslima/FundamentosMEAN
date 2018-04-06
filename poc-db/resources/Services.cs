using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rede.PN.Cancelamento.Sharepoint
{
    /// <summary>
    /// Classe estática com chamadas para os serviços WCFs
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// Chama o serviço que busca lista de Cancelamento de acordo com parâmetros enviados
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="indicadorPesquisa">Indicador do tipo de pesquisa que deve ser feito</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroNsu">Número sequêncial único</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        public static List<SolicitacaoCancelamento> BucarCancelamentos(String codigoUsuario, Int32 numeroEstabelecimento, String indicadorPesquisa, DateTime dataInicial, DateTime dataFinal, Int64 numeroAvisoCancelamento, Int64 numeroNsu, TipoVenda tipoVenda)
        {
            List<SolicitacaoCancelamento> retorno = new List<SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Chamada ao serviço BuscarCancelamentos"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoUsuario,
                    numeroEstabelecimento,
                    indicadorPesquisa,
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroNsu,
                    tipoVenda
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    retorno = contexto.Cliente.BuscarCancelamentos(codigoUsuario, numeroEstabelecimento,
                        indicadorPesquisa, dataInicial, dataFinal, numeroAvisoCancelamento, numeroNsu, tipoVenda);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chama o serviço que faz a inclusão de cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="cancelamentos">Cancelamentos</param>
        /// <returns>Retorna os cancelamentos que foram incluidos</returns>
        public static List<SolicitacaoCancelamento> IncluirCancelamentos(String codigoUsuario, List<SolicitacaoCancelamento> cancelamentos)
        {
            List<SolicitacaoCancelamento> retorno = new List<SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Chamada ao serviço IncluirCancelamento"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoUsuario,
                    cancelamentos
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    retorno = contexto.Cliente.IncluirCancelamentos(codigoUsuario, cancelamentos);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chama o serviço que faz a inclusão de cancelamentos no PN
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="address">Ip da máquina</param>
        /// <param name="cancelamentos">Cancelamentos</param>
        public static void IncluirCancelamentosPn(String codigoUsuario, String address, List<SolicitacaoCancelamento> cancelamentos)
        {
            using (var log = Logger.IniciarLog("Chamada ao serviço IncluirCancelamento no PN"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoUsuario,
                    address,
                    cancelamentos
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    contexto.Cliente.IncluirCancelamentosPn(cancelamentos, address, codigoUsuario);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    
                });
            }
        }

        /// <summary>
        /// Chama do serviço que faz validação de uma lista de solicitações de cancelamento
        /// </summary>
        /// <param name="tipoOperacao">Tipo de operação - 'B' = validações</param>
        /// <param name="cancelamentos">Cancelamentos que serão validados</param>
        /// <returns>Retorna uma lista de validações de cancelamento</returns>
        public static List<Validacao> ValidarCancelamentos(Char tipoOperacao, List<SolicitacaoCancelamento> cancelamentos, String dados)
        {
            List<Validacao> retorno = new List<Validacao>();

            using (var log = Logger.IniciarLog("Chamada ao serviço ValidarParametrosBloqueio"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoOperacao,
                    cancelamentos
                });

                Boolean ehEntidadeValida = !String.IsNullOrEmpty(dados);
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Parâmetros de validação Inválidos");
                    Logger.GravarErro("Parâmetros de validação de cancelamento inconsistentes", ex, cancelamentos);
                    throw ex;
                }

                QueryStringSegura queryStringSegura = new QueryStringSegura(dados);
                Int32 codigoEntidade = 0;
                if (queryStringSegura.AllKeys.Contains("CodigoEntidade"))
                {
                    codigoEntidade = Convert.ToInt32(queryStringSegura["CodigoEntidade"]);
                }

                foreach (var cancelamento in cancelamentos)
                {
                    ehEntidadeValida = VerificarEstabelecimentoEmCentralizadora(cancelamento.NumeroEstabelecimentoVenda, codigoEntidade);
                    if (!ehEntidadeValida)
                    {
                        break;
                    }
                }
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Dados Inválidos");
                    Logger.GravarErro("Dados de cancelamento inconsistentes", ex, cancelamentos);
                    throw ex;
                }
                else
                {

                    using (var contexto = new ContextoWCF<CancelamentoClient>())
                    {
                        retorno = contexto.Cliente.ValidarParametrosBloqueio(tipoOperacao, cancelamentos);
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chamada ao serviço de anular cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do Usuário</param>
        /// <param name="cancelamentos">Lista de cancelamentos</param>
        /// <returns>Retorna uma lista de resultados dos cancelamentos que foram enviados para anulação</returns>
        public static List<CancelamentoServico.Validacao> AnularCancelamentos(String codigoUsuario, Int16 codigoCanal, List<CancelamentoServico.SolicitacaoCancelamento> cancelamentos)
        {
            List<CancelamentoServico.Validacao> retorno = new List<Validacao>();

            using (var log = Logger.IniciarLog("Chamada ao serviço Anular Cancelamentos"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoUsuario,
                    codigoCanal,
                    cancelamentos
                });

                using (var contexto = new ContextoWCF<CancelamentoServico.CancelamentoClient>())
                {
                    retorno = contexto.Cliente.AnularCancelamento(codigoUsuario, codigoCanal, cancelamentos);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chama o serviço que busca dados da transação para o cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento para busca</param>
        /// <returns>Retorna o cancelamento buscado com mais informações</returns>
        public static SolicitacaoCancelamento BuscarTransacaoParaCancelamento(SolicitacaoCancelamento cancelamento, String dados)
        {

            SolicitacaoCancelamento retorno = new SolicitacaoCancelamento();

            using (var log = Logger.IniciarLog("Chamada ao serviço BuscarTransacaoParaCancelamento"))
            {

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cancelamento
                });

                Boolean ehEntidadeValida = !String.IsNullOrEmpty(dados);
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Parâmetros Inválidos");
                    Logger.GravarErro("Parâmetros de cancelamento inconsistentes", ex, cancelamento);
                    throw ex;
                }

                QueryStringSegura queryStringSegura = new QueryStringSegura(dados);
                Int32 codigoEntidade = 0;
                if (queryStringSegura.AllKeys.Contains("CodigoEntidade"))
                {
                    codigoEntidade = Convert.ToInt32(queryStringSegura["CodigoEntidade"]);
                }

                ehEntidadeValida = VerificarEstabelecimentoEmCentralizadora(cancelamento.NumeroEstabelecimentoVenda, codigoEntidade);
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Dados Inválidos");
                    Logger.GravarErro("Dados de cancelamento inconsistentes", ex, cancelamento);
                    throw ex;
                }
                else
                {
                    using (var contexto = new ContextoWCF<CancelamentoClient>())
                    {
                        retorno = contexto.Cliente.BuscarTransacaoParaCancelamento(cancelamento);
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chama o serviço que busca os dados de transações duplicadas para o cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento para busca</param>
        /// <returns>Retorna uma lista de cancelamentos com mesmo ponto de venda, data de transação, tipo de transação e nsu/cartão</returns>
        public static List<SolicitacaoCancelamento> BuscarTransacaoDuplicadaParaCancelamento(SolicitacaoCancelamento cancelamento, String dados)
        {
            List<SolicitacaoCancelamento> retorno = new List<SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Chamada ao serviço BuscarTransacaoDuplicadaParaCancelamento"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cancelamento
                });

                Boolean ehEntidadeValida = !String.IsNullOrEmpty(dados);
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Parâmetros Inválidos");
                    Logger.GravarErro("Parâmetros de cancelamento inconsistentes", ex, cancelamento);
                    throw ex;
                }

                QueryStringSegura queryStringSegura = new QueryStringSegura(dados);
                Int32 codigoEntidade = 0;
                if (queryStringSegura.AllKeys.Contains("CodigoEntidade"))
                {
                    codigoEntidade = Convert.ToInt32(queryStringSegura["CodigoEntidade"]);
                }

                ehEntidadeValida = VerificarEstabelecimentoEmCentralizadora(cancelamento.NumeroEstabelecimentoVenda, codigoEntidade);
                if (!ehEntidadeValida)
                {
                    PortalRedecardException ex = new PortalRedecardException(300, "Dados Inválidos");
                    Logger.GravarErro("Dados de cancelamento inconsistentes", ex, cancelamento);
                    throw ex;
                }
                else
	            {
                    using (var contexto = new ContextoWCF<CancelamentoClient>())
                    {
                        retorno = contexto.Cliente.BuscarTransacaoDuplicadaParaCancelamento(cancelamento);
                    }
	            }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Busca endereço do pv
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="numeroPv">Número do ponto de venda</param>
        /// <param name="tipoEndereco">Tipo de endereço</param>
        /// <returns>Retorna uma lista de endereços</returns>
        public static List<Endereco> BuscaEnderecoPV(out Int32 codigoRetorno, Int32 numeroPv, String tipoEndereco)
        {
            List<Endereco> retorno = new List<Endereco>();

            using (var log = Logger.IniciarLog("Chamada ao serviço de Busca de Endereço do PV"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPv,
                    tipoEndereco
                });

                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    retorno = contexto.Cliente.ConsultarEndereco(out codigoRetorno, numeroPv, tipoEndereco); ;
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    codigoRetorno,
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Formata o endereço do estabelecimento
        /// </summary>
        /// <param name="numeroPv">Número do ponto de venda</param>
        /// <param name="tipoEndereco">Tipo de endereço</param>
        /// <returns>Retorna um endereço formatado para string</returns>
        public static String FormataEnderecoPV(Int32 numeroPv, String tipoEndereco)
        {
            String retorno = String.Empty;
            Int32 codigoRetorno = 0;
            var enderecos = BuscaEnderecoPV(out codigoRetorno, numeroPv, tipoEndereco);

            if (codigoRetorno == 0)
            {
                var endereco = enderecos.FirstOrDefault();

                if (endereco != null)
                {
                    retorno = String.Format(@"{0}, {1}, {2} - {3} - {4}", endereco.EnderecoEstabelecimento.Trim(), endereco.Numero.Trim(), endereco.Complemento.Trim(), endereco.Cidade.Trim(), endereco.UF.Trim());
                }
            }

            return retorno;
        }

        /// <summary>
        /// Serviço de busca de cancelamentos desfeitos por período
        /// </summary>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna lista de cancelamentos desfeitos em certo período</returns>
        public static List<SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPeriodo(DateTime dataInicial, DateTime dataFinal, Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<SolicitacaoCancelamento> cancelamentos = new List<SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Chamada ao serviço BuscaCancelamentosDesfeitosPorPeriodo"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    cancelamentos = contexto.Cliente.BuscaCancelamentosDesfeitosPorPeriodo(dataInicial, dataFinal, numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Serviço de busca de cancelamentos desfeitos por ponto venda e número aviso
        /// </summary>
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns></returns>
        public static List<SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<SolicitacaoCancelamento> cancelamentos = new List<SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Chamada ao serviço BuscaCancelamentosDesfeitosPorPeriodo"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    cancelamentos = contexto.Cliente.BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Serviço que retorna se o pv é uma filial
        /// </summary>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna true se o ponto de venda é uma filial, senão retorna false</returns>
        public static Boolean IsFilial(Int32 numeroPontoVenda)
        {

            var isFilial = false;

#if !DEBUG
            using (var log = Logger.IniciarLog("Verifica se o pv é filial"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {
                    var dadosPontoVenda = contexto.Cliente.ListaCadastroPorPontoVenda(numeroPontoVenda);

                    if (dadosPontoVenda.Count > 0 && dadosPontoVenda[0].CodTipoEstabelecimento != null && dadosPontoVenda[0].CodTipoEstabelecimento == 1)
                        isFilial = true;
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    isFilial
                });
            }
#endif

            return isFilial;
        }

        /// <summary>
        /// Serviço que retorna se o pv é centralizado
        /// </summary>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna true se o ponto de venda é uma centralizadora, senão retorna false</returns>
        public static Boolean IsCentralizado(Int32 numeroPontoVenda)
        {
            var isCentralizado = false;

            using (var log = Logger.IniciarLog("Verifica se o pv é centralizado"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
#if DEBUG
                    isCentralizado = true;
#else
                    Int32 codigoRetorno = 0;
                    var filiais = contexto.Cliente.ConsultarFiliais(out codigoRetorno, numeroPontoVenda, 2);

                    if (codigoRetorno == 0 && filiais.Count > 0)
                        isCentralizado = true;
#endif
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    isCentralizado
                });
            }

            return isCentralizado;
        }

        /// <summary>
        /// Serviço que verifica se o pv está bloqueado por fraude
        /// </summary>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna true se o ponto de venda está bloqueado por fraude, senão retorna false</returns>
        public static Boolean VerificaPVBloqueadoPorFraude(Int32 numeroPontoVenda, Int32 codigoRamoAtividade)
        {
            var pvEstaBloqueadoPorFraude = false;

#if !DEBUG

            using (var log = Logger.IniciarLog("Verifica se o pv está bloqueado por fraude"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda,
                    codigoRamoAtividade
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    var cancelamentos = new List<SolicitacaoCancelamento>();
                    cancelamentos.Add(new SolicitacaoCancelamento { NumeroEstabelecimentoVenda = numeroPontoVenda, CodigoRamoAtividade = codigoRamoAtividade });
                    
                    var retorno = contexto.Cliente.ValidarParametrosBloqueio('V', cancelamentos);

                    var validacao = retorno.FirstOrDefault();
                    if (validacao != null && validacao.CodigoRetorno != 0)
                        pvEstaBloqueadoPorFraude = true;
                    
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    pvEstaBloqueadoPorFraude
                });
            }
#endif

            return pvEstaBloqueadoPorFraude;
        }

        /// <summary>
        /// Verifica se um ponto de venda é filial de uma dada matriz
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="numeroMatriz">Número da matriz</param>
        /// <returns>Retorna verdadeiro quando o número do estabelecimento é filial da matriz informada</returns>
        public static Boolean VerificarEstabelecimentoEmMatriz(Int32 numeroEstabelecimento, Int32 numeroMatriz)
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Chamada ao serviço VerificarEstabelecimentoEmMatriz"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroEstabelecimento, 
                    numeroMatriz
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
#if DEBUG
                    retorno = true;
#else
                    retorno = contexto.Cliente.VerificarEstabelecimentoEmMatriz(numeroEstabelecimento, numeroMatriz);
#endif
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se um estabelecimento
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="numeroCentralizadora">Número do ponto de venda centralizador</param>
        /// <returns></returns>
        public static Boolean VerificarEstabelecimentoEmCentralizadora(Int32 numeroEstabelecimento, Int32 numeroCentralizadora)
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Chamada ao serviço VerificarEsbelecimentoEmCentralizadora"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroEstabelecimento,
                    numeroCentralizadora
                });

                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
#if DEBUG
                    retorno = true;
#else
                    Int32 codigoRetorno = 0;
                    var filiais = contexto.Cliente.ConsultarFiliais(out codigoRetorno, numeroCentralizadora, 2).Select(f => f.PontoVenda);

                    if (codigoRetorno == 0 && filiais.Contains(numeroEstabelecimento) || numeroCentralizadora == numeroEstabelecimento)
                        retorno = true;
#endif
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chamada ao serviço de anular cancelamentos do PN
        /// </summary>
        /// <param name="listaNumerosAvisos">Lista de número de avisos</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="address">Ip</param>
        public static void AnularCancelamentosPn(List<Int64> listaNumerosAvisos, Int32 numeroPontoVenda, String codigoUsuario, String address)
        {
            using (var log = Logger.IniciarLog("Chamada ao serviço AnularCancelamentos do PN"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaNumerosAvisos, numeroPontoVenda, codigoUsuario, address
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    contexto.Cliente.AnularCancelamentosPn(listaNumerosAvisos, numeroPontoVenda, codigoUsuario, address);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {

                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroPontoVenda"></param>
        /// <returns></returns>
        public static List<CancelamentoPn> ConsultarCancelamentosPn(Int32 numeroPontoVenda)
        {
            List<CancelamentoPn> cancelamentos = new List<CancelamentoPn>();

            using (var log = Logger.IniciarLog("Chamada ao serviço ConsultarCancelamentosPn"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<CancelamentoClient>())
                {
                    cancelamentos = contexto.Cliente.ConsultarCancelamentosPn(null, numeroPontoVenda, DateTime.Now, DateTime.Now, null);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }
    }
}
