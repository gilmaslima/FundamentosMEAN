using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Modelo;
using System.Collections;

namespace Rede.PN.Credenciamento.Sharepoint.Servicos
{
    public static class ServicosWF
    {

        #region [ SERASA ]

        /// <summary>
        /// Consulta CNPJ no SERASA
        /// </summary>
        /// <param name="CNPJ">Situação do Canal</param>
        /// <param name="codCanal">Código do Canal</param>
        /// <param name="indSinalizacao">Indicador de Sinalização</param>
        /// <returns>Retorna Objeto da classe Serasa</returns>
        public static Modelo.Serasa ConsultarDadosCNPJ(Modelo.TipoPessoa tipoPessoa, Int64 numeroCnpj, Int64 codigoCanal, Char usuarioInclusao)
        {
            Modelo.Serasa dadosPj = new Modelo.Serasa();
            using (Logger log = Logger.IniciarLog("Carregar Dados do Serasa PJ"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    numeroCnpj,
                    codigoCanal,
                    usuarioInclusao
                });

                using (WFPropostaValidarPassos.WFPropostaValidarPassosClient client = new WFPropostaValidarPassos.WFPropostaValidarPassosClient())
                {
                    dadosPj = client.RecuperarDadosSerasa(tipoPessoa, numeroCnpj, codigoCanal, usuarioInclusao);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    dadosPj
                });
            }

            return dadosPj;
        }

        #endregion

        #region [ CAMPANHAS ]

        /// <summary>
        /// Lista de campanhas por canal Celula Ramo Cep
        /// </summary>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <param name="codigoGrupoRamo">Código do Grupo Ramo</param>
        /// <param name="codRamoAtividade">Código do Ramo de Atividade</param>
        /// <param name="codigoCep">Código do CEP</param>
        /// <param name="codTipoCampanha">Código do Tipo da Campanha</param>
        /// <param name="codigoCampanha">Código da Campanha</param>
        /// <param name="codTipoEquipamento">Código do Tipo de Equipamento</param>
        /// <returns>retorna lista de campanhas</returns>
        public static List<WFCampanhas.ListaCampanhaPorCanalCelulaRamoCep> ConsultaCampanhaPorCanalCelulaRamoCep(int codigoCanal, int codigoCelula, int codigoGrupoRamo, int codRamoAtividade, string codigoCep, char? codTipoCampanha, string codigoCampanha, string codTipoEquipamento)
        {
            List<WFCampanhas.ListaCampanhaPorCanalCelulaRamoCep> lstCampanhas = new List<WFCampanhas.ListaCampanhaPorCanalCelulaRamoCep>();

            using (var log = Logger.IniciarLog("WFCampanhas - ListaCampanhaPorCanalCelulaRamoCep"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCanal,
                    codigoCelula,
                    codigoGrupoRamo,
                    codRamoAtividade,
                    codigoCep,
                    codTipoCampanha,
                    codigoCampanha,
                    codTipoEquipamento
                });

                using (var contexto = new ContextoWCF<WFCampanhas.ServicoPortalWFCampanhasClient>())
                {
                    lstCampanhas = contexto.Cliente.ListaCampanhaPorCanalCelulaRamoCep(codigoCanal, codigoCelula, codigoGrupoRamo, codRamoAtividade, codigoCep, codTipoCampanha, codigoCampanha, codTipoEquipamento);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstCampanhas
                });
            }

            return lstCampanhas;
        }

        #endregion

        #region [Dados Bancarios]

        /// <summary>
        /// Salvar dados Bancários no WF
        /// </summary>
        /// <param name="proposta">Código da Proposta</param>
        /// <param name="domiciliosBancarios">Domicílio Bancário</param>
        /// <param name="codigoRetorno">Código do Retorno</param>
        /// <param name="mensagemErro">Mensagem de Erro</param>
        public static void SalvarDadosBancarios(Proposta proposta, List<Modelo.DomicilioBancario> domiciliosBancarios, List<Modelo.ProdutoParceiro> parceiros, ref int? codigoRetorno, ref string mensagemErro)
        {
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosBancarios"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta,
                    domiciliosBancarios,
                    parceiros
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    contexto.Cliente.SalvarDadosBancario(proposta, domiciliosBancarios, ConverterProdutosParceiros(parceiros, proposta.CodigoTipoPessoa, proposta.IndicadorSequenciaProposta), ref codigoRetorno, ref mensagemErro);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    codigoRetorno,
                    mensagemErro
                });
            }
        }

        #endregion [Dados Bancarios]

        #region [Proposta]

        /// <summary>
        /// Retorna boolean sobre existir propostas pendentes para o CNPJ e tipo pessoa parametrizados
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="numeroCnpj">Número do CNPJ</param>
        /// <returns>retorna boolean existe Propostas pendentes</returns>
        public static Boolean PropostasPendentes(Char tipoPessoa, Int64 numeroCnpj)
        {
            Boolean existemPropostasPendentes = false;
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosBancarios"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    numeroCnpj
                });

                using (var contexto = new ContextoWCF<WFProposta.ServicoPortalWFPropostaClient>())
                {
                    List<WFProposta.PropostasPendentes> propostasPendentes = contexto.Cliente.ConsultaQtdePropostasPendentes(tipoPessoa, numeroCnpj);

                    existemPropostasPendentes = propostasPendentes.Where(i => i.QtdePropostasPendentes > 0 || i.QtdePVsAtivos > 0).Count() > 0;
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    existemPropostasPendentes
                });

                return existemPropostasPendentes;
            }
        }

        /// <summary>
        /// Recuperar dados da matriz
        /// </summary>
        /// <param name="numeroPontoVenda">Número do Ponto de Venda</param>
        /// <param name="tipoOperacao">Tipo de Operação</param>
        /// <param name="siglaProduto">Sigla do Produto</param>
        /// <returns>Retorna objeto de Dados da Matriz</returns>
        public static Modelo.DadosMatriz RecuperarDadosMatriz(Int32 numeroPontoVenda, Modelo.TipoOperacao tipoOperacao, String siglaProduto)
        {
            Modelo.DadosMatriz objDadosMatriz = new Modelo.DadosMatriz();
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - RecuperarDadosMatriz"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda,
                    tipoOperacao,
                    siglaProduto
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    objDadosMatriz = contexto.Cliente.ConsultaDadosMatriz(numeroPontoVenda, tipoOperacao, siglaProduto);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    objDadosMatriz
                });

                return objDadosMatriz;
            }

        }

        #endregion

        #region [Dados Iniciais]

        /// <summary>
        /// Salvar dados inicias na base de dados WF
        /// </summary>
        /// <param name="proposta">Proposta</param>
        /// <param name="endereco">Endereço</param>
        /// <param name="proprietario">Proprietáiro</param>
        /// <param name="domicilioBancario">Domicílio Bancário</param>
        /// <returns>retorna número da sequência criada ao salvar dados iniciais</returns>
        public static Int32 SalvarDadosIniciais(Modelo.Proposta proposta, Modelo.Endereco endereco, ICollection<Modelo.Proprietario> proprietario, ICollection<Modelo.DomicilioBancario> domicilioBancario)
        {
            Int32 numeroSequencia = 0;

            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - RecuperarDadosMatriz"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta,
                    endereco,
                    proprietario,
                    domicilioBancario
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    numeroSequencia = contexto.Cliente.SalvarDadosInicias(proposta, endereco, (List<Modelo.Proprietario>)proprietario, (List<Modelo.DomicilioBancario>)domicilioBancario);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroSequencia
                });
            }

            return numeroSequencia;
        }
        #endregion

        #region [Dados Operacionais]

        /// <summary>
        /// Salvar Dados Operacionais no BD WF
        /// </summary>
        /// <param name="proposta">Proposta</param>
        /// <param name="tecnologia">Tecnologia</param>
        public static void SalvarDadosOperacionais(Modelo.Proposta proposta, Modelo.Tecnologia tecnologia)
        {
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosOperacionais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    contexto.Cliente.SalvarDadosOperacionaisV02(proposta, new[] { tecnologia }.ToList());
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                });
            }
        }
        #endregion

        #region [ Confirmação ]

        /// <summary>
        /// Efetuar credenciamento
        /// </summary>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="numeroSolicitacao">Número da Solicitação</param>
        /// <param name="proposta">Proposta</param>
        /// <param name="endereco">Endereço</param>
        /// <param name="usuario">Usuário</param>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="tipoEquipamento">Tipo de Equipamento</param>
        public static void EfetuarCredenciamento(out Int32 numeroPontoVenda, out Int32 numeroSolicitacao, Modelo.Proposta proposta, Modelo.Endereco endereco, String usuario, Int32 codigoBanco, String tipoEquipamento)
        {
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - Efetuar Credenciamento"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta,
                    endereco,
                    usuario,
                    codigoBanco,
                    tipoEquipamento
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    contexto.Cliente.EfetuarCredenciamento(out numeroPontoVenda, out numeroSolicitacao, proposta, endereco, usuario, codigoBanco, tipoEquipamento);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroSolicitacao,
                    numeroPontoVenda
                });
            }
        }

        #endregion

        #region [ Conclusão ]

        /// <summary>
        /// Recupera dados do PV e FCT
        /// </summary>
        /// <param name="proposta">Proposta</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="listaFct">Lista FCT</param>
        /// <param name="situacaoProposta">Situação da proposta</param>
        /// <param name="descSituacaoProposta">Descrição da Situação da proposta</param>
        public static void RecuperarDadosPveFct(Modelo.Proposta proposta, out Int32 numeroPontoVenda, out String listaFct, out Char situacaoProposta, out String descSituacaoProposta)
        {
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosOperacionais"))
            {
                Char codTipoPessoa = proposta.CodigoTipoPessoa;
                Int64 numCNPJ = proposta.NumeroCnpjCpf;
                Int32 numSequencia = proposta.IndicadorSequenciaProposta;

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    numeroPontoVenda = contexto.Cliente.RecuperarDadosPveFct(out situacaoProposta, out descSituacaoProposta, out listaFct, codTipoPessoa, numCNPJ, numSequencia);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroPontoVenda,
                    listaFct,
                    situacaoProposta,
                    descSituacaoProposta
                });
            }
        }

        /// <summary>
        /// Consulta Débitos pendentes para um PDV específico
        /// </summary>
        /// <param name="numPdvs">Lista de números PDVs</param>
        /// <returns>retorna objeto com informação sobre débitos pendentes</returns>
        public static Servico.DD.ConsultarDetalhamentoDebitosRetorno ConsultaDebitosPendentes(Int32[] numPdvs)
        {
            Servico.DD.ConsultarDetalhamentoDebitosRetorno retorno = new Servico.DD.ConsultarDetalhamentoDebitosRetorno();
            Servico.DD.StatusRetorno statusRetorno = new Servico.DD.StatusRetorno();
            Servico.DD.ConsultarDetalhamentoDebitosEnvio envio = new Servico.DD.ConsultarDetalhamentoDebitosEnvio
            {
                Estabelecimentos = numPdvs,
                Versao = Servico.DD.VersaoDebitoDesagendamento.ISF,
                DataFinal = DateTime.Now,
                DataInicial = DateTime.Now,
                ChavePesquisa = null,
                CodigoBandeira = 0,
                TipoPesquisa = "P",
            };

            using (Logger log = Logger.IniciarLog("Consulta Débitos Pendentes por PV"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdvs,
                    envio
                });

                using (var contexto = new ContextoWCF<Servico.DD.RelatorioDebitosDesagendamentosClient>())
                {
                    retorno = contexto.Cliente.ConsultarDetalhamentoDebitosPesquisa(out statusRetorno, envio, 1, 500, Guid.NewGuid(), Guid.NewGuid());
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno,
                    statusRetorno
                });
            }

            return retorno;
        }

        #endregion

        #region [ Recuperar Propostas ]

        /// <summary>
        /// Carrega lista de propostas Pendênctes
        /// </summary>
        /// <param name="codigoTipoPessoa">Código tipo de Pessoa</param>
        /// <param name="cpfCnpj">CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <returns>Retorna Lista de Propostas Pendentes</returns>
        public static List<Modelo.PropostaPendente> RecuperarPropostasIncompletas(Char codigoTipoPessoa, Int64 cpfCnpj, Int32? numeroSequencia)
        {
            var retorno = new List<Modelo.PropostaPendente>();

            using (Logger log = Logger.IniciarLog("Carrega dados das propostas"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFProposta.ServicoPortalWFPropostaClient>())
                {
                    var propostas = contexto.Cliente.ConsPropCredenciamentoPendente(codigoTipoPessoa, cpfCnpj, numeroSequencia);

                    foreach (var proposta in propostas)
                    {
                        retorno.Add(new Modelo.PropostaPendente
                        {
                            NroEstabelecimento = proposta.NumPontoVenda,
                            CNPJ = proposta.NumCNPJCPF,
                            TipoPessoa = proposta.CodTipoPessoa,
                            RazaoSocial = proposta.RazaoSocial,
                            Ramo = proposta.CodigoGrupoRamo + String.Format(@"{0:0000}", proposta.CodRamoAtividade),
                            TipoEstabelecimento = proposta.CodTipoEstabelecimento,
                            Categoria = ' ',
                            EnderecoComercial = String.Format("{0}, {1}", proposta.Logradouro, proposta.NumEndereco),
                            StatusProposta = 1,
                            NumSequencia = proposta.NumSequencia
                        });
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
        /// Carrega dados de proposta em andamento
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="cpfCnpj">Número CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <returns>retorna dados de uma proposta em andamento</returns>
        public static WFProposta.PropostaPorCNPJCPF CarregarDadosPropostaEmAndamento(Char tipoPessoa, Int64 cpfCnpj, Int32 numeroSequencia)
        {
            var retorno = new WFProposta.PropostaPorCNPJCPF();
            using (Logger log = Logger.IniciarLog("Carrega dados da proposta selecionada"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFProposta.ServicoPortalWFPropostaClient>())
                {
                    var proposta = contexto.Cliente.ConsultaPropostaPorCNPJCPF(tipoPessoa, cpfCnpj, numeroSequencia);

                    if (proposta.Count > 0)
                    {
                        retorno = proposta[0];
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
        /// Carrega dados Tecnologia
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="cpfCnpj">Número CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <returns>retorna dados de Tecnologia</returns>
        public static WFTecnologia.ConsultaTecnologia CarregarDadosTecnologia(char tipoPessoa, long cpfCnpj, int numeroSequencia)
        {
            var retorno = new WFTecnologia.ConsultaTecnologia();

            using (Logger log = Logger.IniciarLog("Carrega dados da tecnologia"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFTecnologia.ServicoPortalWFTecnologiaClient>())
                {
                    var tecnologias = contexto.Cliente.ConsultaTecnologia(tipoPessoa, cpfCnpj, numeroSequencia);

                    if (tecnologias.Count > 0)
                        retorno = tecnologias[0];
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Carrega dados Endereços
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="cpfCnpj">Número CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <returns>retorna lista de dados de endereços</returns>
        public static List<WFEnderecos.ConsultaEnderecos> CarregarDadosEnderecos(Char tipoPessoa, Int64 cpfCnpj, Int32 numeroSequencia)
        {
            var enderecos = new List<WFEnderecos.ConsultaEnderecos>();

            using (Logger log = Logger.IniciarLog("Carrega dados dos endereços da proposta"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFEnderecos.ServicoPortalWFEnderecosClient>())
                {
                    enderecos = contexto.Cliente.ConsultaEnderecos(tipoPessoa, cpfCnpj, numeroSequencia, null);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    enderecos
                });
            }

            return enderecos;
        }

        /// <summary>
        /// Carrega Dados Domicílio Bancário
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="cpfCnpj">Número CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <param name="indTipoOperacaoProd">Indicador Tipo de Operação do Produto</param>
        /// <returns>retorna dados sobre domicílio bancário</returns>
        public static WFDomBancario.ConsultaDomicilioBancario CarregarDadosDomicilioBancario(Char tipoPessoa, Int64 cpfCnpj, Int32 numeroSequencia, Int32 indTipoOperacaoProd)
        {
            var retorno = new WFDomBancario.ConsultaDomicilioBancario();

            using (Logger log = Logger.IniciarLog("Carrega dados do domicilio bancário"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFDomBancario.ServicoPortalWFDomicilioBancarioClient>())
                {
                    var domicilioBancario = contexto.Cliente.ConsultaDomicilioBancario(tipoPessoa, cpfCnpj, numeroSequencia, indTipoOperacaoProd);

                    if (domicilioBancario.Count > 0)
                        retorno = domicilioBancario[0];
                    else
                        retorno = null;
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Carrega dados dos proprietários
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="cpfCnpj">Número CPF/CNPJ</param>
        /// <param name="numeroSequencia">Número da Sequência</param>
        /// <returns>retorna lista de dados dos proprietários</returns>
        public static List<WFProprietarios.ConsultaProprietarios> CarregarDadosProprietarios(char tipoPessoa, long cpfCnpj, int numeroSequencia)
        {
            var retorno = new List<WFProprietarios.ConsultaProprietarios>();

            using (Logger log = Logger.IniciarLog("Carrega lista de proprietários"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cpfCnpj,
                    numeroSequencia
                });

                using (var contexto = new ContextoWCF<WFProprietarios.ServicoPortalWFProprietariosClient>())
                {
                    retorno = contexto.Cliente.ConsultaProprietarios(tipoPessoa, cpfCnpj, numeroSequencia, null, null, null);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Consulta próxima sequencia da proposta
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="numCnpjCpf">Número do CNPJ/CPF</param>
        /// <returns>retorna valor da próxima sequência da proposta</returns>
        public static Int32 ConsultaProximaSequencia(Char tipoPessoa, Int64 numCnpjCpf)
        {
            Int32 retorno = 0;

            using (Logger log = Logger.IniciarLog("Consulta Próxima Sequência"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    numCnpjCpf
                });

                using (var contexto = new ContextoWCF<WFProposta.ServicoPortalWFPropostaClient>())
                {
                    var sequencias = contexto.Cliente.ConsultaProximaSequencia(tipoPessoa, numCnpjCpf);

                    if (sequencias.Count > 0)
                    {
                        retorno = (Int32)sequencias[0].NumSequencia;
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        #region [Dados Cenario]

        /// <summary>
        /// Recuperar dados do Cenário
        /// </summary>
        /// <param name="codigoCenario">Código do Cenário</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoTipoEquipamento">Código do tipo de Equipamento</param>
        /// <param name="codigoSituacaoCenarioCanal">Código da situação do Cenário do Canal</param>
        /// <param name="codigoCampanha">Código da Campanha</param>
        /// <param name="codigoOrigemChamada">Código da Origem de Chamada</param>
        /// <param name="valorDefaultAluguel">Valor Default do Aluguel</param>
        /// <param name="usuarioInclusao">Usuário Inclusão</param>
        /// <returns>retorna dados do cenário</returns>
        public static List<Modelo.Cenario> RecuperarDadosCenario(Int32 codigoCenario, Int32 codigoCanal, String codigoTipoEquipamento, Char codigoSituacaoCenarioCanal, String codigoCampanha,
                                                 String codigoOrigemChamada, float valorDefaultAluguel, String usuarioInclusao)
        {
            List<Modelo.Cenario> listaCenario;

            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - RecuperarDadosCenario"))
            {

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCenario,
                    codigoCanal,
                    codigoTipoEquipamento,
                    codigoSituacaoCenarioCanal,
                    codigoCampanha,
                    codigoOrigemChamada,
                    valorDefaultAluguel,
                    usuarioInclusao
                });

                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    listaCenario = contexto.Cliente.RecuperarDadosCenario(codigoCenario, codigoCanal, codigoTipoEquipamento, codigoSituacaoCenarioCanal, codigoCampanha, codigoOrigemChamada, valorDefaultAluguel, usuarioInclusao);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaCenario
                });
            }

            return listaCenario;
        }
        #endregion

        #region [Pacotes Servicos]

        /// <summary>
        /// Retorna lista de Serviços do tipo "Pacote"
        /// </summary>
        /// <param name="tipoServico">Tipo de Serviço</param>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="codigoGrupoRamoAtuacao">Código do Grupo de Ramo Atuação</param>
        /// <param name="codigoRamoAtividade">Código do Ramo de Atividade</param>
        /// <param name="indicadorOrigemTelemarketing">Indicador Origem Telemarketing</param>
        /// <param name="codigoCanalOrigem">Código Canal Origem</param>
        /// <param name="tipoEquipamento">Tipo de Equipamento</param>
        /// <returns>retorna lista de serviços do tipo "Pacote"</returns>
        public static List<Modelo.Servico> ConsultaServicosPacotes(TipoServico tipoServico, TipoPessoa tipoPessoa, Int32 codigoGrupoRamoAtuacao, Int32 codigoRamoAtividade, Char indicadorOrigemTelemarketing, Int32 codigoCanalOrigem, String tipoEquipamento)
        {
            List<Modelo.Servico> listaServicosPacotes = new List<Modelo.Servico>();

            using (var log = Logger.IniciarLog("WFServicos - RecuperarDadosServicosPacote"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoServico,
                    tipoPessoa,
                    codigoGrupoRamoAtuacao,
                    codigoRamoAtividade,
                    indicadorOrigemTelemarketing,
                    codigoCanalOrigem,
                    tipoEquipamento
                });

                using (var contexto = new ContextoWCF<WFServicos.ServicoPortalWFServicosClient>())
                {
                    listaServicosPacotes = contexto.Cliente.RecuperarDadosServicosPacote(tipoServico, tipoPessoa, codigoGrupoRamoAtuacao, codigoRamoAtividade, indicadorOrigemTelemarketing, codigoCanalOrigem, tipoEquipamento);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaServicosPacotes
                });
            }

            return listaServicosPacotes;
        }

        /// <summary>
        /// Retorna lista de Serviços
        /// </summary>
        /// <param name="tipoPessoa">Tipo de Pessoa</param>
        /// <param name="NumeroCNPJ">Número do CNPJ</param>
        /// <param name="numeroSequenciaProposta">Número da Sequència Proposta</param>
        /// <returns>retorna Lista de serviços</returns>
        public static List<WFServicos.ConsultaServico> ConsultaServicos(Char tipoPessoa, Int64 NumeroCNPJ, Int32 numeroSequenciaProposta)
        {

            List<WFServicos.ConsultaServico> lstServicosWF = new List<WFServicos.ConsultaServico>();

            using (var log = Logger.IniciarLog("WFServicos - RecuperarDadosServicos"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    NumeroCNPJ,
                    numeroSequenciaProposta
                });

                using (var contexto = new ContextoWCF<WFServicos.ServicoPortalWFServicosClient>())
                {
                    lstServicosWF = contexto.Cliente.ConsultaServicos(tipoPessoa, NumeroCNPJ, numeroSequenciaProposta, null);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstServicosWF
                });
            }

            return lstServicosWF;
        }
        #endregion

        #region [ Condicao Comercial ]

        /// <summary>
        /// Consulta a parametrização das ofertas.
        /// </summary>
        /// <param name="codigoParametroOferta">Código do parâmetro de oferta</param>
        /// <param name="codigoOfertaPadrao">Código da oferta padrão</param>
        /// <param name="indicadorTipoPessoa">Tipo de pessoa</param>
        /// <param name="numeroCnpjCpf">Número do CNPJ ou CPF</param>
        /// <param name="indicadorSequenciaProposta">Indicador de Sequência da Proposta</param>
        /// <param name="codigoTipoEstabelecimento">Código do Tipo de Estabelecimento (Filial = 1)</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="codigoGrupoRamo">Código do Grupo Ramo</param>
        /// <param name="codigoRamoAtividade">Código do Ramo de Atividade</param>
        /// <param name="codigoSiglaUF">Sigla UF</param>
        /// <param name="codigoSituacaoRegistro">Situação do registro</param>
        /// <param name="usuarioAtualizacao">Usuário responsável pela última atualização</param>
        /// <param name="dataAtualizacaoInicio">Data/hora inicial da atualização </param>
        /// <param name="dataAtualizacaoFim">Data/hora final da atualização</param>
        /// <param name="registroInicial">Registro inicial</param>
        /// <param name="registroFinal">Registro final</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros</param>
        /// <returns>Parametrização das ofertas</returns>
        public static List<WFParamOfertas.ParametroOferta> ConsultaParametrizacaoOfertas(Int32? codigoParametroOferta,
                                                                                         Int32? codigoOfertaPadrao,
                                                                                         Char? indicadorTipoPessoa,
                                                                                         Int64? numeroCnpjCpf,
                                                                                         Int32? indicadorSequenciaProposta,
                                                                                         Int32? codigoTipoEstabelecimento,
                                                                                         Int16? codigoCanal,
                                                                                         Int32? codigoCelula,
                                                                                         Int32? codigoGrupoRamo,
                                                                                         Int32? codigoRamoAtividade,
                                                                                         String codigoSiglaUF,
                                                                                         Char? codigoSituacaoRegistro,
                                                                                         String descricaoOferta,
                                                                                         String usuarioAtualizacao,
                                                                                         DateTime? dataAtualizacaoInicio,
                                                                                         DateTime? dataAtualizacaoFim,
                                                                                         Int32? registroInicial,
                                                                                         Int32? registroFinal,
                                                                                         out Int32 quantidadeTotalRegistros)
        {
            List<WFParamOfertas.ParametroOferta> listaParametrizacaoOfertas;

            using (var log = Logger.IniciarLog("ServicosWF - ConsultaCondicoesComerciais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoParametroOferta,
                    codigoOfertaPadrao,
                    indicadorTipoPessoa,
                    numeroCnpjCpf,
                    indicadorSequenciaProposta,
                    codigoTipoEstabelecimento,
                    codigoCanal,
                    codigoCelula,
                    codigoGrupoRamo,
                    codigoRamoAtividade,
                    codigoSiglaUF,
                    codigoSituacaoRegistro,
                    usuarioAtualizacao,
                    dataAtualizacaoInicio,
                    dataAtualizacaoFim,
                    registroInicial,
                    registroFinal
                });

                using (var contexto = new ContextoWCF<WFParamOfertas.ServicoPortalWFParametrizacaoOfertasClient>())
                {
                    listaParametrizacaoOfertas = contexto.Cliente.ConsultarParametrosOfertas(codigoParametroOferta,
                                                                                             codigoOfertaPadrao,
                                                                                             indicadorTipoPessoa,
                                                                                             numeroCnpjCpf,
                                                                                             indicadorSequenciaProposta,
                                                                                             codigoTipoEstabelecimento,
                                                                                             codigoCanal,
                                                                                             codigoCelula,
                                                                                             codigoGrupoRamo,
                                                                                             codigoRamoAtividade,
                                                                                             codigoSiglaUF,
                                                                                             codigoSituacaoRegistro,
                                                                                             descricaoOferta,
                                                                                             usuarioAtualizacao,
                                                                                             dataAtualizacaoInicio,
                                                                                             dataAtualizacaoFim,
                                                                                             registroInicial,
                                                                                             registroFinal,
                                                                                             out quantidadeTotalRegistros);
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    listaParametrizacaoOfertas
                });
            }

            return listaParametrizacaoOfertas;
        }

        /// <summary>
        /// Consulta de Condições Comerciais (Ofertas) do HIS
        /// </summary>
        /// <param name="codigoOferta"></param>
        /// <returns></returns>
        public static List<WFOfertas.OfertaPadrao> ConsultaCondicoesComerciais(Int32? codigoOferta)
        {
            List<WFOfertas.OfertaPadrao> listaCondicoesComerciais;

            using (var log = Logger.IniciarLog("ServicosWF - ConsultaDetalhesCondicoesComerciais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoOferta
                });

                using (var contexto = new ContextoWCF<WFOfertas.ServicoPortalWFOfertasClient>())
                {
                    listaCondicoesComerciais = contexto.Cliente.ConsultarOfertasPadrao(codigoOferta);
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    listaCondicoesComerciais
                });
            }

            return listaCondicoesComerciais;
        }

        /// <summary>
        /// Recupera detalhes de uma Condição Comercial (Oferta) do HIS
        /// </summary>
        /// <param name="codigoOferta"></param>
        /// <returns></returns>
        public static WFOfertas.OfertaPadrao RecuperarOfertaPadrao(Int32 codigoOferta)
        {
            WFOfertas.OfertaPadrao condicaoComercial;

            using (var log = Logger.IniciarLog("ServicosWF - RecuperarOfertaPadrao"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoOferta
                });

                using (var contexto = new ContextoWCF<WFOfertas.ServicoPortalWFOfertasClient>())
                {
                    condicaoComercial = contexto.Cliente.RecuperarOfertaPadrao(codigoOferta);
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    condicaoComercial
                });
            }

            return condicaoComercial;
        }

        /// <summary>
        /// Consulta dados Bancários - Produtos
        /// </summary>
        /// <param name="tipoProduto">Tipo de Produto</param>
        /// <param name="codigoGrupoRamo">Código do Grupo Ramo</param>
        /// <param name="codigoRamoAtividade">Código do Ramo de Atividade</param>
        /// <param name="codigoCanalOrigem">Código do canal de Origem</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>retorna lista de produtos</returns>
        public static List<Modelo.Produto> ConsultaDadosBancarios(Modelo.TipoProduto tipoProduto, Int32 codigoGrupoRamo, String codigoRamoAtividade, Int32 codigoCanalOrigem, Int32 numeroPontoVenda)
        {
            List<Modelo.Produto> listaProduto;

            using (var log = Logger.IniciarLog("WFServicos - ConsutaDadosBancarios"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoProduto,
                    codigoGrupoRamo,
                    codigoRamoAtividade,
                    codigoCanalOrigem,
                    numeroPontoVenda
                });

                using (var contexto = new ContextoWCF<WFProdutos.ServicoPortalWFProdutosClient>())
                {
                    listaProduto = contexto.Cliente.RecuperarDadosProduto(tipoProduto, codigoGrupoRamo, codigoRamoAtividade, codigoCanalOrigem, numeroPontoVenda);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaProduto
                });
            }

            return listaProduto;
        }

        /// <summary>
        /// Salvar dados Comerciais no BD WF
        /// </summary>
        /// <param name="credenciamento">Credenciamento</param>
        public static void SalvarDadosComerciais(Modelo.Credenciamento credenciamento)
        {
            Char indicadorMaquineta = ' ';
            Char indicadorIATA = ' ';
            Char indicadorMarketingDireto = ' ';

            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosComerciaisV02"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    credenciamento
                });
                
                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    SharePointUlsLog.LogMensagem("Chamada ao serviço de salvar dados comerciais");
                    indicadorMaquineta = contexto.Cliente.SalvarDadosComerciaisV02(out indicadorIATA,
                                                                                   out indicadorMarketingDireto,
                                                                                   credenciamento.Proposta,
                                                                                   new[] { credenciamento.Tecnologia }.ToList(),
                                                                                   credenciamento.Produtos.ToList(),
                                                                                   credenciamento.Servicos.ToList(),
                                                                                   credenciamento.DadosCampanhas.ToList(),
                                                                                   ConverterOfertasPrecoUnico(credenciamento.OfertasPrecoUnico, credenciamento.Proposta.ValorTaxaAdesao.GetValueOrDefault()),
                                                                                   ConverterProdutosParceiros(credenciamento.ProdutoParceiro, credenciamento.Proposta.CodigoTipoPessoa, credenciamento.Proposta.IndicadorSequenciaProposta));
                    SharePointUlsLog.LogMensagem("Retorno do serviço de salvar dados comerciais");
                }

                credenciamento.Proposta.IndicadorMaquineta = indicadorMaquineta;
                credenciamento.Proposta.IndicadorIATA = indicadorIATA;
                credenciamento.Proposta.IndicadorComercializacaoCatalogo = indicadorMarketingDireto;
                credenciamento.Proposta.IndicadorComercializacaoEletronico = indicadorMarketingDireto;
                credenciamento.Proposta.IndicadorComercializacaoNormal = indicadorMarketingDireto;
                credenciamento.Proposta.IndicadorComercializacaoTelefone = indicadorMarketingDireto;
                

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    credenciamento
                });
            }
        }

        #endregion

        #region [ Dados Campanha ]

        /// <summary>
        /// Retorna dados da Campanha selecionada
        /// </summary>
        /// <param name="codigoCampanha">Código da Campanha</param>
        /// <param name="tipoEquipamento">Tipo de Equipamento</param>
        /// <param name="usuarioInclusao">Usuário Inclusão</param>
        /// <returns>retorna dados da campanha selecionada</returns>
        public static Modelo.DadosCampanha ConsultaDadosCampanha(String codigoCampanha, String tipoEquipamento, String usuarioInclusao)
        {
            Modelo.DadosCampanha listaDadosCampanha = null;

            using (var log = Logger.IniciarLog("WFCampanhas - RecuperarDadosCampanha"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCampanha,
                    tipoEquipamento,
                    usuarioInclusao
                });

                using (var contexto = new ContextoWCF<WFCampanhas.ServicoPortalWFCampanhasClient>())
                {
                    listaDadosCampanha = contexto.Cliente.RecuperarDadosCampanha(codigoCampanha, tipoEquipamento, usuarioInclusao);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaDadosCampanha
                });
            }

            return listaDadosCampanha;
        }

        #endregion

        /// <summary>
        /// Salvar dados Contato no BD WF
        /// </summary>
        /// <param name="proposta">Proposta</param>
        /// <param name="enderecos">Endereços</param>
        /// <param name="tecnologia">Tecnologia</param>
        /// <param name="codigoFilial">Código Filial</param>
        /// <param name="codigoZonaVenda">Código da Zona de Venda</param>
        public static void SalvarDadosContato(Modelo.Proposta proposta, List<Modelo.Endereco> enderecos, Modelo.Tecnologia tecnologia, ref Int32? codigoFilial, ref Int32? codigoZonaVenda)
        {
            using (var log = Logger.IniciarLog("WFPropostaValidarPassos - SalvarDadosContato"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta,
                    enderecos,
                    tecnologia
                });


                using (var contexto = new ContextoWCF<WFPropostaValidarPassos.WFPropostaValidarPassosClient>())
                {
                    contexto.Cliente.SalvarDadosContato(proposta, enderecos, tecnologia, ref codigoFilial, ref codigoZonaVenda);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoFilial,
                    codigoZonaVenda
                });
            }
        }

        /// <summary>
        /// Recupera Quantidade de PVs com base no CNPJ/CPF selecionado
        /// </summary>
        /// <param name="codigoTipoPessoa">Código do tipo de pessoa</param>
        /// <param name="numeroCpfCnpj">Número do CPF/CNPJ</param>
        /// <returns>retorna boolean "existem PVs Ativos ou Cancelados"</returns>
        public static Boolean RecuperaQuantidadePvs(Char codigoTipoPessoa, Int64 numeroCpfCnpj)
        {
            Boolean existemPvsAtivosOuCancelados = false;

            using (Logger log = Logger.IniciarLog("Lista Cadastro Reduzido por CNPJ"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoPessoa,
                    numeroCpfCnpj
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {

                    var pontosVenda = contexto.Cliente.ListaCadastroReduzidoPorCNPJ(codigoTipoPessoa, numeroCpfCnpj);

                    existemPvsAtivosOuCancelados = pontosVenda.Count > 0;
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    existemPvsAtivosOuCancelados
                });
            }

            return existemPvsAtivosOuCancelados;
        }

        /// <summary>
        /// Converte uma Tecnologia por uma lista de Tecnologias
        /// </summary>
        /// <param name="tecnologia">Objeto de Modelo.Tecnologia</param>
        /// <returns>Lista de Tecnologias com o objeto Tecnologia</returns>
        private static List<Modelo.Tecnologia> ConverterTecnologias(Modelo.Tecnologia tecnologia)
        {
            List<Modelo.Tecnologia> tecnologias = new List<Modelo.Tecnologia>();
            if (tecnologia != null) tecnologias.Add(tecnologia);
            return tecnologias;
        }

        /// <summary>
        /// Converte uma Campanha por uma lista de Campanhas
        /// </summary>
        /// <param name="campanha">Objeto de Modelo.DadosCampanha</param>
        /// <returns>Lista de Campanhas com o objeto Campanha</returns>
        private static List<Modelo.DadosCampanha> ConverterCampanhas(Modelo.DadosCampanha campanha)
        {
            List<Modelo.DadosCampanha> campanhas = new List<Modelo.DadosCampanha>();
            if (campanha != null) campanhas.Add(campanha);
            return campanhas;
        }

        /// <summary>
        /// Converte o objeto ProdutoParceiro do PN para WF
        /// </summary>
        /// <param name="parceiros"></param>
        /// <returns></returns>
        private static List<WFPropostaValidarPassos.ProdutoParceiro> ConverterProdutosParceiros(List<ProdutoParceiro> parceiros, Char codigoTipoPessoa, Int32? indicadorSequencia)
        {
            List<WFPropostaValidarPassos.ProdutoParceiro> produtos = new List<WFPropostaValidarPassos.ProdutoParceiro>();

            using (Logger log = Logger.IniciarLog("Resultado do método ConverterProdutosParceiros"))
            {
                if (parceiros != null && parceiros.Count > 0)
                {
                    log.GravarLog(EventoLog.InicioServico, new
                    {
                        parceiros
                    });
                    produtos = parceiros.ConvertAll(p => new WFPropostaValidarPassos.ProdutoParceiro
                    {
                        AreaAtendimento = p.AreaAtendimento,
                        AreaLoja = p.AreaLoja,
                        CodCca = p.Produto.CodigoCCA,
                        CodFeature = p.Produto.CodigoFeature,
                        CodigoAgencia = p.Agencia,
                        CodigoBanco = p.Banco,
                        CodigoTipoNegocio = p.Produto.CodigoTipoNegocio.ToString(),
                        CodigoTipoPessoa = codigoTipoPessoa.ToString(),
                        Indc24h = null,
                        IndcComercial = null,
                        IndcDomingo = null,
                        IndcNoturno = null,
                        IndcSabado = null,
                        IndcSegSex = null,
                        IndicadorSequencia = indicadorSequencia,
                        NumAssentos = p.NumAssentos,
                        NumeroCnpjCpf = p.NumeroCnpj,
                        NumeroConta = p.Conta,
                        NumMaximoRefeicoes = p.NumMaximoRefeicoes,
                        NumMesas = p.NumMesas,
                        QtdCheckOutLoja = p.QtdCheckOutLoja,
                        UsuarioAlteracao = p.UsuarioAlteracao
                    });
                    log.GravarLog(EventoLog.FimServico, new
                    {
                        produtos
                    });
                }
            }

            return produtos;
        }

        /// <summary>
        /// Converte uma lista de tipo Modelo.OfertaPrecoUnico para uma lista de tipo WFPropostaValidarPassos.ConsultaOfertaPrecoUnico
        /// </summary>
        /// <param name="ofertasPrecoUnico">Lista de Ofertas de tipo Modelo.OfertaPrecoUnico</param>
        /// <returns></returns>
        private static List<WFPropostaValidarPassos.ConsultaOfertaPrecoUnico> ConverterOfertasPrecoUnico(ICollection<Modelo.OfertaPrecoUnico> ofertasPrecoUnico, double valorTaxaAdesao)
        {
            return ofertasPrecoUnico.ToList().ConvertAll(o => new WFPropostaValidarPassos.ConsultaOfertaPrecoUnico
            {
                CodigosOfertaPrecoUnico = o.CodigosOfertaPrecoUnico,
                CodigoTipoPessoa = o.CodigoTipoPessoa,
                CodigoUsuario = o.CodigoUsuario,
                DataHoraUltimaAtualizacao = o.DataHoraUltimaAtualizacao,
                NumeroCNPJ = o.NumeroCNPJ,
                NumeroSequenciaProposta = o.NumeroSequenciaProposta,
                IsentaTaxaAdesao = valorTaxaAdesao > 0 ? 'N' : 'S'
            });
        }

        /// <summary>
        /// Tem por objetivo realizar a consulta das ofertas de uma proposta.
        /// </summary>
        /// <param name="codigoTipoOperacao">Tipo da Operacao “C”–Consulta com a chave completa  “L”–Lista as ofertas a partir da chave da proposta</param>
        /// <param name="codigoTipoPessoa">Tipo de Pessoa</param>
        /// <param name="numeroCNPJ">Número do CNPJ/CPF do estabelecimento</param>
        /// <param name="numeroSequenciaProposta">Número da Sequencia da Proposta</param>
        /// <param name="codigoOferta">Código da Oferta</param>
        /// <param name="usuario">Usuário que efetuou a operação</param>     
        public static List<WFOfertas.ConsultaOfertaPrecoUnico> ConsultaOfertaPrecoUnico(Char codigoTipoOperacao, Char codigoTipoPessoa, Int64 numeroCNPJ, Int32 numeroSequenciaProposta, Int32? codigoOferta)
        {
            using (Logger log = Logger.IniciarLog("Consulta as ofertas de uma proposta"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoOperacao,
                    codigoTipoPessoa,
                    numeroCNPJ,
                    numeroSequenciaProposta,
                    codigoOferta
                });

                using (var contexto = new ContextoWCF<WFOfertas.ServicoPortalWFOfertasClient>())
                {
                    return contexto.Cliente.ConsultaOfertaPrecoUnico(codigoTipoOperacao, codigoTipoPessoa, numeroCNPJ, numeroSequenciaProposta, codigoOferta);
                }
            }
        }
    }
}
