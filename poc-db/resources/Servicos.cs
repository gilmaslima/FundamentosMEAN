using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.GEBancos;
using System.ServiceModel;
using Redecard.PN.Boston.Sharepoint.GEAgencias;
using Redecard.PN.Boston.Sharepoint.GEContaCorr;
using Redecard.PN.Boston.Sharepoint.GERamosAtd;
using Redecard.PN.Boston.Sharepoint.PNTransicoes;
using Redecard.PN.Boston.Sharepoint.WFOrigem;
using Redecard.PN.Boston.Sharepoint.WFAdministracao;
using Redecard.PN.Boston.Sharepoint.WFProposta;
using Redecard.PN.Boston.Sharepoint.GECanais;
using Redecard.PN.Boston.Sharepoint.FEToken;
using System.Globalization;
using Redecard.PN.Boston.Sharepoint.GEPontoVen;
using Redecard.PN.Boston.Sharepoint.WMOcorrencia;
using Redecard.PN.Boston.Sharepoint.GEDomBancario;
using Redecard.PN.Boston.Sharepoint.DRParametrizacao;

namespace Redecard.PN.Boston.Sharepoint.Negocio
{
    /// <summary>
    /// Classe com métodos que chamam web services
    /// </summary>
    public static class Servicos
    {
        /// <summary>
        /// Identificação da proposta (id único para o Portal)
        /// </summary>
        public static String IdProposta { get { return "PORTAL"; } }

        #region [ GE ]

        /// <summary>
        /// Método que busca lista de profissões
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetProfissoes(Char codTipoPessoa, Int32? codCanal, Int32? codGrupoRamo, Int32? codRamoAtivididade, String codEquipamento)
        {
            using (var log = Logger.IniciarLog("Get Profissões"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoPessoa,
                    codCanal,
                    codGrupoRamo,
                    codRamoAtivididade,
                    codEquipamento
                });

                List<ListItem> profissoes = new List<ListItem>();
                profissoes.Add(new ListItem(String.Empty));

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaRamosAtividadesPorCanalTipoPessoaMPOS(codTipoPessoa, codCanal, codGrupoRamo, codRamoAtivididade, codEquipamento);
                    foreach (var item in retorno)
                    {
                        profissoes.Add(new ListItem
                        {
                            Text = item.DescRamoAtividade,
                            Value = String.Format("{0}{1:0000}", item.CodGrupoRamo, item.CodRamoAtivididade)
                        });
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { profissoes });

                return profissoes.OrderBy(p => p.Text);
            }
        }

        /// <summary>
        /// Método que busca lista de ramos de atividade
        /// </summary>
        /// <param name="codGrupoRamo"></param>
        /// <param name="codRamoAtividade"></param>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetRamosAtividade(Int32? codGrupoRamo, Int32? codRamoAtividade)
        {
            using (var log = Logger.IniciarLog("Get Ramos Atividade"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codGrupoRamo,
                    codRamoAtividade
                });

                List<ListItem> ramosAtividade = new List<ListItem>();
                ramosAtividade.Add(new ListItem(String.Empty));

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var ramos = contexto.Cliente.ListaDadosCadastraisRamosAtividades(codGrupoRamo, codRamoAtividade);

                    foreach (var ramo in ramos)
                    {
                        ramosAtividade.Add(new ListItem
                        {
                            Text = ramo.DescrRamoAtividade,
                            Value = String.Format("{0:0000}", ramo.CodRamoAtivididade)
                        });
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { ramosAtividade });

                return ramosAtividade.OrderBy(r => r.Text);
            }
        }

        /// <summary>
        /// Método que busca lista de grupos de atuação
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetGruposAtuacao()
        {
            using (var log = Logger.IniciarLog("Get Grupos Atuação"))
            {
                List<ListItem> gruposAtuacao = new List<ListItem>();
                gruposAtuacao.Add(new ListItem(String.Empty));

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var grupos = contexto.Cliente.ListaDadosCadastraisGruposRamosAtividades();

                    foreach (var grupo in grupos)
                    {
                        gruposAtuacao.Add(new ListItem
                        {
                            Text = grupo.DescrRamoAtividade,
                            Value = grupo.CodGrupoRamoAtividade.ToString()
                        });
                    }

                }

                log.GravarLog(EventoLog.RetornoServico, new { gruposAtuacao });

                return gruposAtuacao.OrderBy(g => g.Text);
            }
        }

        /// <summary>
        /// Método que busca o nome do grupo de atuação
        /// </summary>
        /// <returns></returns>
        public static String GetNomeGrupoAtuacao(Int32 codGrupoRamo)
        {
            using (var log = Logger.IniciarLog("Get Nome Grupo Atuação"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codGrupoRamo
                });

                String nomeGrupoAtuacao = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var grupos = contexto.Cliente.ListaDadosCadastraisGruposRamosAtividades();
                    var grupo = grupos.FirstOrDefault(g => g.CodGrupoRamoAtividade == codGrupoRamo);

                    if (grupo != null)
                        nomeGrupoAtuacao = grupo.DescrRamoAtividade;
                }

                log.GravarLog(EventoLog.RetornoServico, new { nomeGrupoAtuacao });

                return nomeGrupoAtuacao;
            }
        }

        /// <summary>
        /// Método que busca o nome do ramo de atividade
        /// </summary>
        /// <returns></returns>
        public static String GetNomeRamoAtuacao(Int32 codGrupoRamo, Int32 codRamoAtividade)
        {
            using (var log = Logger.IniciarLog("Get Nome Ramo Atuação"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codGrupoRamo,
                    codRamoAtividade
                });

                String nomeRamoAtuacao = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var ramos = contexto.Cliente.ListaDadosCadastraisRamosAtividadesMPOS(codGrupoRamo, codRamoAtividade);

                    if (ramos.Length > 0)
                        nomeRamoAtuacao = ramos[0].DescrRamoAtividade;
                }

                log.GravarLog(EventoLog.RetornoServico, new { nomeRamoAtuacao });

                return nomeRamoAtuacao;
            }
        }

        /// <summary>
        /// Método que busca lista de bancos
        /// </summary>
        /// <param name="codTipoOperacao"></param>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetBancos(Char codTipoOperacao)
        {
            using (var log = Logger.IniciarLog("Get Bancos"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoOperacao
                });

                List<ListItem> bancos = new List<ListItem>();
                bancos.Add(new ListItem(String.Empty));

                using (var contexto = new ContextoWCF<ServicoPortalGEBancoClient>())
                {

                    var bancosDados = contexto.Cliente.ListaDadosCadastraisReduzidos(codTipoOperacao);

                    foreach (var banco in bancosDados)
                    {
                        bancos.Add(new ListItem
                        {
                            Text = String.Format("{0} - {1}", banco.CodBancoCompensacao, banco.NomeBanco),
                            Value = banco.CodBancoCompensacao.ToString()
                        });
                    }

                }

                log.GravarLog(EventoLog.RetornoServico, new { bancos });

                return bancos.OrderBy(b => b.Text);
            }
        }

        /// <summary>
        /// Método que busca o nome de uma agência
        /// </summary>
        /// <param name="codBanco"></param>
        /// <param name="codAgencia"></param>
        /// <returns></returns>
        public static String GetNomeAgencia(Int32 codBanco, Int32 codAgencia)
        {
            using (var log = Logger.IniciarLog("Get Nome Agencia"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codBanco,
                    codAgencia
                });

                String nomeAgencia = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGEAgenciasClient>())
                {
                    var agencias = contexto.Cliente.ConsultaDetalheAgencia(codBanco, codAgencia);

                    if (agencias.Length > 0)
                        nomeAgencia = String.Format("- {0}", agencias[0].NomeAgencia);
                }

                log.GravarLog(EventoLog.RetornoServico, new { nomeAgencia });

                return nomeAgencia;
            }
        }

        /// <summary>
        /// Método que válida o número de uma agência
        /// </summary>
        /// <param name="codBanco"></param>
        /// <param name="codAgencia"></param>
        /// <param name="mensagemRetorno"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public static Boolean ValidaAgencia(Int32 codBanco, Int32 codAgencia, out String mensagemRetorno, out Int32 codigoRetorno)
        {
            using (var log = Logger.IniciarLog("Valida Agencia"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codBanco,
                    codAgencia
                });

                Boolean agenciaValida = false;

                using (var contexto = new ContextoWCF<ServicoPortalGEAgenciasClient>())
                {
                    var retorno = contexto.Cliente.ValidaAgencias(codBanco, codAgencia);

                    if (retorno[0].CodErro == 0)
                    {
                        mensagemRetorno = String.Empty;
                        codigoRetorno = 0;
                        agenciaValida = true;
                    }

                    mensagemRetorno = retorno[0].DescricaoErro;
                    codigoRetorno = (Int32)retorno[0].CodErro;
                }

                log.GravarLog(EventoLog.RetornoServico, new { agenciaValida });

                return agenciaValida;
            }
        }

        /// <summary>
        /// Método que válida o número de uma conta corrente
        /// </summary>
        /// <param name="codigoBanco"></param>
        /// <param name="codigoAgencia"></param>
        /// <param name="contaCorrente"></param>
        /// <returns></returns>
        public static Boolean ValidaContaCorrente(Int32 codigoBanco, Int64 codigoAgencia, Int64 contaCorrente)
        {
            using (var log = Logger.IniciarLog("Valida Conta Corrente"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoBanco,
                    codigoAgencia,
                    contaCorrente
                });

                Boolean contaCorrenteValida = false;

                using (var contexto = new ContextoWCF<ServicoPortalGEContaCorrenteClient>())
                {
                    contaCorrenteValida = contexto.Cliente.ChecaDigito(codigoBanco, codigoAgencia, contaCorrente);
                }

                log.GravarLog(EventoLog.RetornoServico, new { contaCorrenteValida });

                return contaCorrenteValida;
            }
        }

        /// <summary>
        /// Recupera se o canal exige participação integral dos sócios
        /// </summary>
        /// <returns></returns>
        public static Char GetExigeParticipacaoIntegral(Char? indSituacaoCanal, Int32? codCanal, String indSinalizacao)
        {
            using (var log = Logger.IniciarLog("Get Exige Participação Integral"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    indSituacaoCanal,
                    codCanal,
                    indSinalizacao
                });

                Char exigeParticipacaoIntegral = default(Char);

                using (var contexto = new ContextoWCF<ServicoPortalGECanaisClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastrais(indSituacaoCanal, codCanal, indSinalizacao);

                    exigeParticipacaoIntegral = (Char)retorno[0].IndExigeParticipacaoIntegral;
                }

                log.GravarLog(EventoLog.RetornoServico, new { exigeParticipacaoIntegral });

                return exigeParticipacaoIntegral;
            }
        }

        /// <summary>
        /// Recupera o endereço de instalação de um PV
        /// </summary>
        /// <returns>Endereço de instalação de um PV</returns>
        public static Endereco GetEnderecoInstalacaoPorPV(Int32 numPdv)
        {
            Endereco endereco = new Endereco();

            using (var log = Logger.IniciarLog("Get Endereco Instalacao Por PV"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

#if DEBUG
                endereco = new Endereco
                {
                    Bairro = "Tamboré",
                    CEP = "03323-020",
                    Cidade = "Barueri",
                    Estado = "SP",
                    Complemento = "Bloco 30",
                    Numero = "835",
                    Logradouro = "Av. Marcos Penteado de Ulhoa Rodrigues",
                    TipoEndereco = 'R'
                };
#else
                using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                {
                    var retorno = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (retorno.Length > 0)
                    {
                        endereco.CEP = String.Format("{0}-{1}", retorno[0].CodCEPTecnologia, retorno[0].CodCompCEPTecnologia);
                        endereco.Logradouro = retorno[0].NomeLogradouroTecnologia;
                        endereco.Numero = retorno[0].NumLogradouroTecnologia;
                        endereco.Complemento = retorno[0].CompEnderecoTecnologia;
                        endereco.Estado = retorno[0].NomeUFTecnologia;
                        endereco.Cidade = retorno[0].NomeCidadeTecnologia;
                        endereco.Bairro = retorno[0].NomeBairroTecnologia;
                    }
                }
#endif

                log.GravarLog(EventoLog.RetornoServico, new { endereco });
            }

            return endereco;
        }

        /// <summary>
        /// Recupera o endereço de comercial de um PV
        /// </summary>
        /// <param name="numPdv"></param>
        /// <returns></returns>
        public static Endereco GetEnderecoComercialPorPV(Int32 numPdv)
        {
            Endereco endereco = new Endereco();

            using (var log = Logger.IniciarLog("Get Endereco Comercial Por PV"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                {
                    var retorno = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (retorno.Length > 0)
                    {
                        endereco.CEP = String.Format("{0}-{1}", retorno[0].CodCEP, retorno[0].CodCompCEP);
                        endereco.Logradouro = retorno[0].NomeLogradouro;
                        endereco.Numero = retorno[0].NumLogradouro;
                        endereco.Complemento = retorno[0].CompEndereco;
                        endereco.Estado = retorno[0].NomeUF;
                        endereco.Cidade = retorno[0].NomeCidade;
                        endereco.Bairro = retorno[0].NomeBairro;
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { endereco });
            }

            return endereco;
        }

        /// <summary>
        /// Recupera o endereço de correspondência de um PV
        /// </summary>
        /// <param name="numPdv"></param>
        /// <returns></returns>
        public static Endereco GetEnderecoCorrespondenciaPorPV(Int32 numPdv)
        {
            Endereco endereco = new Endereco();

            using (var log = Logger.IniciarLog("Get Endereco Correspondência Por PV"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                {
                    var retorno = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (retorno.Length > 0)
                    {
                        endereco.CEP = String.Format("{0}-{1}", retorno[0].CodCEPCorrespondencia, retorno[0].CodCompCEPCorrespondencia);
                        endereco.Logradouro = retorno[0].NomeLogradouroCorrespondencia;
                        endereco.Numero = retorno[0].NumLogradouroCorrespondencia;
                        endereco.Complemento = retorno[0].CompEnderecoCorrespondencia;
                        endereco.Estado = retorno[0].NomeUFCorrespondencia;
                        endereco.Cidade = retorno[0].NomeCidadeCorrespondencia;
                        endereco.Bairro = retorno[0].NomeBairroCorrespondencia;
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { endereco });
            }

            return endereco;
        }

        /// <summary>
        /// Valida se o domicílio bancário está duplicado
        /// </summary>
        public static Boolean ValidaDomicilioBancarioDuplicado(Int64 cpfCnpj, Int32 codigoBanco, Int32 codigoAgencia, Int64 numeroConta, Char codigoTipoPessoa)
        {
            CodigoDescricaoPvsDuplicados retorno;

            using (var log = Logger.IniciarLog("Valida Domicílio Bancário Duplicado"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cpfCnpj,
                    codigoBanco,
                    codigoAgencia,
                    numeroConta,
                    codigoTipoPessoa
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEDomicilioBancarioClient>())
                {
                    retorno = contexto.Cliente.ValidaDomicilioDuplicado(cpfCnpj, codigoBanco, codigoAgencia, numeroConta, codigoTipoPessoa);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            if (retorno.CodErro != null && retorno.CodErro != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Recupera o Canal do ponto de venda
        /// </summary>
        /// <param name="numPdv">Número do Ponto de Venda</param>
        /// <returns></returns>
        public static Int32 GetCanalPontoVenda(Int32 numPdv)
        {
            Int32 canal = 0;

            using (var log = Logger.IniciarLog("Get Canal do PV"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                {
                    var dadosPontoVenda = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (dadosPontoVenda.Length > 0)
                    {
                        canal = (Int32)dadosPontoVenda[0].CodCanal;
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { canal });
            }

            return canal;
        }

        /// <summary>
        /// Recupera a Célula do ponto de venda
        /// </summary>
        /// <param name="numPdv">Número do Ponto de Venda</param>
        /// <returns></returns>
        public static int GetCelulaPontoVenda(int numPdv)
        {
            Int32 celula = 0;

            using (var log = Logger.IniciarLog("Get Célula do PV"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                {
                    var dadosPontoVenda = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (dadosPontoVenda.Length > 0)
                    {
                        celula = (Int32)dadosPontoVenda[0].CodCelula;
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { celula });
            }

            return celula;
        }

        #endregion

        #region [ WF ]

        /// <summary>
        /// Método que busca lista de "Como Conheceu o Produto"
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetComoConheceu(Int32? codOrigemInteresse, Char? situacao)
        {
            using (var log = Logger.IniciarLog("Get Como Conheceu"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codOrigemInteresse,
                    situacao
                });

                List<ListItem> comoConheceuLista = new List<ListItem>();
                comoConheceuLista.Add(new ListItem(String.Empty));

                using (var contexto = new ContextoWCF<ServicoPortalWFOrigemInteresseClient>())
                {

                    var retorno = contexto.Cliente.ListaDadosOrigemInteresse(codOrigemInteresse, situacao);
                    foreach (var item in retorno)
                    {
                        comoConheceuLista.Add(new ListItem
                        {
                            Text = item.DescOrigemInteresse,
                            Value = item.CodOrigemInteresse.ToString()
                        });
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new { comoConheceuLista });

                return comoConheceuLista.OrderBy(c => c.Text);
            }
        }

        /// <summary>
        /// Retorna a taxa de ativação do MPOS
        /// </summary>
        /// <returns></returns>
        public static String GetTaxaAtivacao(Int32 codCanal, Int32? codCelula, Char? codTipoPessoa, String codTipoEquipamento, Int32? codGrupoRamo, Int32? codRamoAtividade, Int32? numSolicitacao, Int32 codParametro)
        {
            using (var log = Logger.IniciarLog("Get Taxa Ativação"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codCanal,
                    codCelula,
                    codTipoPessoa,
                    codTipoEquipamento,
                    codGrupoRamo,
                    codRamoAtividade,
                    numSolicitacao,
                    codParametro
                });

                String taxaAtivacao = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalWFAdministracaoClient>())
                {
                    var retorno = contexto.Cliente.PesquisaParametroCredenciamento(codCanal, codCelula, codTipoPessoa, codTipoEquipamento, codGrupoRamo, codRamoAtividade, numSolicitacao, codParametro);

                    taxaAtivacao = String.Format("{0:C}", Double.Parse(retorno[0].ValorParametroString, NumberStyles.Currency));
                }

                log.GravarLog(EventoLog.RetornoServico, new { taxaAtivacao });

                return taxaAtivacao;
            }
        }

        /// <summary>
        /// Retorna o próximo número de sequência de propostas
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <param name="cnpjCpf"></param>
        /// <returns></returns>
        public static Int32 GetNumeroSequencia(Char tipoPessoa, Int64 cnpjCpf)
        {
            using (var log = Logger.IniciarLog("Get Número Sequência"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    tipoPessoa,
                    cnpjCpf
                });

                Int32 numSequencia = 0;

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.ConsultaProximaSequencia(tipoPessoa, cnpjCpf);

                    numSequencia = (Int32)retorno[0].NumSequencia;
                }

                log.GravarLog(EventoLog.RetornoServico, new { numSequencia });

                return numSequencia;
            }
        }

        /// <summary>
        /// Atualiza a situação da proposta
        /// </summary>
        /// <param name="codTipoPessoa"></param>
        /// <param name="cnpjCpf"></param>
        /// <param name="numSeqProp"></param>
        /// <param name="indSituacaoProposta"></param>
        /// <param name="usuarioUltimaAtualizacao"></param>
        /// <param name="codMotivoRecusa"></param>
        /// <param name="numeroPontoVenda"></param>
        /// <param name="indOrigemAtualizacao"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public static Int32 AtualizaSituacaoProposta(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, Char indSituacaoProposta, String usuarioUltimaAtualizacao, Int32? codMotivoRecusa, Int32 numeroPontoVenda, Int32? indOrigemAtualizacao, out String mensagemRetorno)
        {
            using (var log = Logger.IniciarLog("Atualiza Situação Proposta"))
            {
                Int32 codRetorno = 0;

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSeqProp,
                    indSituacaoProposta,
                    usuarioUltimaAtualizacao,
                    codMotivoRecusa,
                    numeroPontoVenda,
                    indOrigemAtualizacao
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.AtualizaSituacaoProposta(codTipoPessoa, cnpjCpf, numSeqProp, indSituacaoProposta, usuarioUltimaAtualizacao, codMotivoRecusa, numeroPontoVenda, indOrigemAtualizacao);

                    codRetorno = (Int32)retorno[0].CodigoErro;
                    mensagemRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                return codRetorno;
            }
        }

        /// <summary>
        /// Atualiza a taxa de ativação da prosposta
        /// </summary>
        /// <param name="codTipoPessoa"></param>
        /// <param name="cnpjCpf"></param>
        /// <param name="numSeqProp"></param>
        /// <param name="valorAtivacao"></param>
        /// <param name="codFaseFiliacao"></param>
        /// <param name="usuarioUltimaAtualizacao"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public static Int32 AtualizaTaxaAtivacaoPropostaMPOS(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, Decimal? valorAtivacao, Int32? codFaseFiliacao, String usuarioUltimaAtualizacao, out String mensagemRetorno)
        {
            using (var log = Logger.IniciarLog("Atualiza Situação Proposta"))
            {
                Int32 codRetorno = 0;

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSeqProp,
                    valorAtivacao,
                    codFaseFiliacao,
                    usuarioUltimaAtualizacao

                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.AtualizaTaxaAtivacaoPropostaMPOS(codTipoPessoa, cnpjCpf, numSeqProp, valorAtivacao, codFaseFiliacao, usuarioUltimaAtualizacao);

                    codRetorno = retorno.CodigoErro ?? 0;
                    mensagemRetorno = retorno.DescricaoErro;
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                return codRetorno;
            }
        }

        /// <summary>
        /// Serviço para inclusão de proposta por PV
        /// </summary>
        /// <param name="numPDV"></param>
        /// <param name="codTipoEquipamento"></param>
        /// <param name="geraFCT"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public static Int32 IncluirPropostaVendaTecnologiaPorPV(Int32 numPDV, String codTipoEquipamento, String usuarioProposta, Int32 codCanal, Int32 codCelula, Int64 numCPFVendedor, Decimal valorEquipamento, out String mensagemRetorno)
        {
            using (var log = Logger.IniciarLog("Incluir Proposta Venda Tecnologia Por PV"))
            {
                Int32 codRetorno = 0;

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPDV,
                    codTipoEquipamento,
                    usuarioProposta,
                    codCanal,
                    codCelula,
                    numCPFVendedor,
                    valorEquipamento
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.IncluirPropostaVendaTecnologiaPorPV(numPDV, codTipoEquipamento, usuarioProposta, codCanal, codCelula, numCPFVendedor, valorEquipamento);

                    mensagemRetorno = retorno.DescricaoErro;
                    codRetorno = retorno.CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno, mensagemRetorno });

                return codRetorno;
            }
        }

        /// <summary>
        /// Serviço para inclusão de proposta Venda Tecnologia (Mobile 2.0 - Solicitação de Novo Leitor de Cartão)
        /// </summary>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <param name="listaEquipamento">Lista de equipamentos</param>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        public static WFProposta.RetornoErro IncluirPropostaVendaTecnologia(
            Int32 numeroPv, Int32 codigoCanal, Int32 codigoCelula,
            ListaPropostaVendaTecnologia[] listaEquipamento)
        {
            using (Logger log = Logger.IniciarLog("Incluir Proposta Venda Tecnologia"))
            {
                var retorno = default(WFProposta.RetornoErro);

                try
                {
                    log.GravarLog(EventoLog.ChamadaServico,
                        new { numeroPv, codigoCanal, codigoCelula, listaEquipamento });

#if DEBUG
                    retorno = new WFProposta.RetornoErro
                    {
                        CodigoErro = 0,
                        DescricaoErro = "SUCESSO!"
                    };
#else
                    using (var ctx = new ContextoWCF<ServicoPortalWFPropostaClient>())
                        retorno = ctx.Cliente.IncluirPropostaVendaTecnologia(numeroPv,
                            IdProposta, codigoCanal, codigoCelula, listaEquipamento);
#endif

                    log.GravarLog(EventoLog.RetornoServico, retorno);
                }
                catch (FaultException<WFProposta.ModelosErroServicos> ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                
                return retorno;
            }
        }

        #endregion

        #region [ PN ]

        /// <summary>
        /// Método que grava os dados da tela 1 para tela 2
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public static PNTransicoes.RetornoGravarAtualizarPasso1 TransicaoPasso1(DadosCredenciamento dados)
        {
            using (var log = Logger.IniciarLog("Transicao Passo 1"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    dados.TipoPessoa,
                    dados.CPF_CNPJ,
                    dados.CodEquipamento,
                    dados.Canal,
                    dados.CodigoGrupoAtuacao,
                    dados.CodigoRamoAtividade
                });

                PNTransicoes.RetornoGravarAtualizarPasso1 retorno;

                using (var contexto = new ContextoWCF<TransicoesBostonClient>())
                {
                    retorno = contexto.Cliente.GravarAtualizarPasso1(dados.TipoPessoa, dados.CPF_CNPJ.CpfCnpjToLong(), dados.CodEquipamento, dados.Canal, dados.CodigoGrupoAtuacao, dados.CodigoRamoAtividade, dados.CPF_CNPJProprietario);
                }

                log.GravarLog(EventoLog.RetornoServico, retorno);

                return retorno;
            }
        }

        /// <summary>
        /// Método que grava os dados da tela 2 para tela 3
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public static Int32 TransicaoPasso2(DadosCredenciamento dados)
        {
            using (var log = Logger.IniciarLog("Transicao Passo 2"))
            {
                PNTransicoes.Proposta proposta = PreencheProposta(dados);
                List<PNTransicoes.Proprietario> proprietarios = PreencheProprietarios(dados.Proprietarios, dados);
                PNTransicoes.Endereco endComercial = PreencheEndereco(dados.EnderecoComercial, dados);
                PNTransicoes.Endereco endCorrespondencia = PreencheEndereco(dados.EnderecoCorrespondencia, dados);
                PNTransicoes.Endereco endInstalacao = PreencheEndereco(dados.EnderecoInstalacao, dados);
                PNTransicoes.Tecnologia tecnologia = PreencheTecnologia(dados);

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    proposta,
                    proprietarios,
                    endComercial,
                    endCorrespondencia,
                    endInstalacao,
                    tecnologia
                });

                Int32 codRetorno = 0;

                using (var contexto = new ContextoWCF<TransicoesBostonClient>())
                {
                    codRetorno = contexto.Cliente.GravarAtualizarPasso2(proposta, proprietarios, endComercial, endCorrespondencia, endInstalacao, tecnologia);
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                return codRetorno;
            }
        }

        /// <summary>
        /// Método que grava os dados da tela 3 para tela 4
        /// </summary>
        /// <param name="dados"></param>
        public static Int32 TransicaoPasso3(DadosCredenciamento dados)
        {
            using (var log = Logger.IniciarLog("Transicao Passo 3"))
            {
                DomicilioBancario domBancarioCredito = PreencheDomicilioBancario(dados);
                Char codTipoPessoa = dados.TipoPessoa; 
                Int64 cnpjCpf = dados.CPF_CNPJ.CpfCnpjToLong();
                Int32 numSeqProp = dados.NumeroSequencia;

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    domBancarioCredito,
                    codTipoPessoa,
                    cnpjCpf,
                    numSeqProp
                });

                Int32 codRetorno = 0;

                using (var contexto = new ContextoWCF<TransicoesBostonClient>())
                {
                    codRetorno = contexto.Cliente.GravarAtualizarPasso3(codTipoPessoa, cnpjCpf, numSeqProp, domBancarioCredito);
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                return codRetorno;
            }
        }

        /// <summary>
        /// Método que grava os dados da tela 4 para tela 5
        /// </summary>
        /// <param name="DadosCredenciamento"></param>
        /// <returns></returns>
        public static Int32 TransicaoPasso4(DadosCredenciamento dados, out String descRetorno, out Int32 numPdv, ref Int32 numSolicitacao)
        {
            using (var log = Logger.IniciarLog("Transicao Passo 4"))
            {
                Char codTipoPessoa = dados.TipoPessoa;
                Int64 cnpjCpf = dados.CPF_CNPJ.CpfCnpjToLong();
                Int32 numSequencia = dados.NumeroSequencia;
                String usuario = dados.Usuario;
                String tipoEquipamento = dados.CodEquipamento;
                Int32 codBanco = dados.CodigoBanco.ToInt32();
                PNTransicoes.Endereco endereco = PreencheEndereco(dados.EnderecoComercial, dados);

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                    numSolicitacao,
                    usuario,
                    tipoEquipamento,
                    codBanco,
                    endereco
                });

                Int32 codRetorno = 0;

                using (var contexto = new ContextoWCF<TransicoesBostonClient>())
                {
                    codRetorno = contexto.Cliente.GravarAtualizarPasso4(codTipoPessoa, cnpjCpf, numSequencia, ref numSolicitacao, out descRetorno, out numPdv, usuario, tipoEquipamento, codBanco, endereco);
                }

                log.GravarLog(EventoLog.RetornoServico, new { codRetorno, descRetorno, numPdv, numSolicitacao });

                return codRetorno;
            }
        }

        #endregion

        #region [ FE ]

        /// <summary>
        /// Chama o serviço que gera um token para o datacash
        /// </summary>
        /// <param name="numPdv"></param>
        /// <param name="valorTransacao"></param>
        /// <param name="numPedido"></param>
        /// <param name="qtdParcela"></param>
        /// <param name="urlRetorno"></param>
        /// <returns></returns>
        public static String GetToken(String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, Int32[] codServicos)
        {
            using (var log = Logger.IniciarLog("Get Token"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv,
                    valorTransacao,
                    numPedido,
                    qtdParcela,
                    urlRetorno
                });

                String token = String.Empty;

                using (var contexto = new ContextoWCF<TokenClient>())
                {
                    token = contexto.Cliente.GetToken(numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos);
                }

                log.GravarLog(EventoLog.RetornoServico, new { token });

                return token;
            }
        }

        public static String GetTokenAnaliseRisco(String cpfCnpj, String nome, String sobrenome, DateTime dataFundacao, String email, String telefone1, String telefone2, String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, Int32[] codServicos, Endereco enderecoPrincipal, Endereco enderecoEntrega, Endereco enderecoCobranca)
        {
            using (var log = Logger.IniciarLog("Get Token"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cpfCnpj,
                    nome,
                    sobrenome,
                    dataFundacao,
                    email,
                    telefone1,
                    telefone2,
                    numPdv,
                    valorTransacao,
                    numPedido,
                    qtdParcela,
                    urlRetorno,
                    enderecoPrincipal,
                    enderecoEntrega,
                    enderecoCobranca
                });

                String token = String.Empty;

                using (var contexto = new ContextoWCF<TokenClient>())
                {
                    token = contexto.Cliente.GetTokenAnaliseRisco(cpfCnpj, nome, sobrenome, dataFundacao, email, telefone1, telefone2, numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos, PreencheEndereco(enderecoPrincipal), PreencheEndereco(enderecoEntrega), PreencheEndereco(enderecoCobranca));
                }

                log.GravarLog(EventoLog.RetornoServico, new { token });

                return token;
            }
        }

        #endregion

        #region [ WM ]

        /// <summary>
        /// Cancela Ocorrência Credenciamento
        /// </summary>
        /// <param name="usuarioOcorrencia"></param>
        /// <param name="numSolicitacao"></param>
        /// <param name="codCasoOcorrencia"></param>
        /// <param name="motivoCancelamento"></param>
        /// <param name="obsCancelamento"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public static Int32 CancelaOcorrenciaCredenciamento(String usuarioOcorrencia, Int32 numSolicitacao, Int32 codCasoOcorrencia, String motivoCancelamento, String obsCancelamento, out String mensagemRetorno)
        {
            using (var log = Logger.IniciarLog("Cancela Ocorrencia Credenciamento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    usuarioOcorrencia,
                    numSolicitacao,
                    codCasoOcorrencia,
                    motivoCancelamento,
                    obsCancelamento
                });

                CancelaOcorrenciaCredenciamento[] retorno;

                using (var contexto = new ContextoWCF<ServicoPortalWMOcorrenciaClient>())
                {
                    retorno = contexto.Cliente.CancelaOcorrenciaCredenciamento(usuarioOcorrencia, numSolicitacao, codCasoOcorrencia, motivoCancelamento, obsCancelamento);
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno });

                mensagemRetorno = retorno[0].DescricaoErro;
                return retorno[0].CodErro ?? 0;
            }
        }

        #endregion

        #region [ DR ]

        /// <summary>
        /// Serviço DR: Consulta parâmetros Mobile Rede
        /// </summary>
        /// <param name="codigoTerminal">Código do Terminal</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <returns>Parâmetros Mobile</returns>
        public static ParametroMobile ConsultarParametroMobile(Int32 codigoCanal, Int32 codigoCelula, String codigoTerminal)
        {
            using (Logger log = Logger.IniciarLog("Consultar parâmetros Mobile"))
            {
                var mobile = default(ParametroMobile);

                try
                {
                    log.GravarLog(EventoLog.ChamadaServico, new { codigoCanal, codigoCelula, codigoTerminal });

#if DEBUG
                    switch (codigoTerminal)
                    {
                        case "CCM": //Leitor de Tarja Magnética
                            mobile = new ParametroMobile { QtdeMaximaParcelas = 12, ValorTaxaAtivacao = 129m };
                            break;
                        case "CPA": //Leitor de Chip e Tarja (Único)
                            mobile = new ParametroMobile { QtdeMaximaParcelas = 12, ValorTaxaAtivacao = 300m };
                            break;
                        case "CPC": //Leitor de Chip e Tarja (Mensal)
                            mobile = new ParametroMobile { QtdeMaximaParcelas = 1, ValorTaxaAtivacao = 80m };
                            break;
                        default:
                            mobile = null;
                            break;
                    }
#else
                    //Consulta serviço DR
                    using (var ctx = new ContextoWCF<ServicoDRParametrizacaoClient>())
                        mobile = ctx.Cliente.ConsultarParametroMobile(codigoCanal, codigoCelula, codigoTerminal);
#endif

                    log.GravarLog(EventoLog.RetornoServico, mobile);
                }
                catch (FaultException<ServicoDRException> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex.Detail.Mensagem, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }

                return mobile;
            }
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Método que preenche a classe de Domicílio Bancário
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static DomicilioBancario PreencheDomicilioBancario(DadosCredenciamento dados)
        {
            DomicilioBancario domBancario = new DomicilioBancario();

            domBancario.CodTipoPessoa = dados.TipoPessoa;
            domBancario.NumCNPJ = dados.CPF_CNPJ.CpfCnpjToLong();
            domBancario.NumSeqProp = dados.NumeroSequencia;
            domBancario.CodigoBanco = dados.CodigoBanco.ToInt32();
            domBancario.NomeBanco = dados.DescricaoBanco;
            domBancario.CodigoAgencia = dados.CodigoAgencia.ToInt32();
            domBancario.NumContaCorrente = dados.ContaCorrente;
            domBancario.IndConfirmacaoDomicilio = ' ';
            domBancario.IndDomicilioDuplicado = ' ';
            domBancario.IndTipoOperacaoProd = 1;
            domBancario.DataHoraUltimaAtualizacao = DateTime.Now;
            domBancario.UsuarioUltimaAtualizacao = "PORTAL";

            return domBancario;
        }

        /// <summary>
        /// Preenche dados da tecnologia para gravação
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static Tecnologia PreencheTecnologia(DadosCredenciamento dados)
        {
            PNTransicoes.Tecnologia retorno = new Tecnologia();

            retorno.CodTipoPessoa = dados.TipoPessoa;
            retorno.NumCNPJ = dados.CPF_CNPJ.CpfCnpjToLong();
            retorno.NumSeqProp = dados.NumeroSequencia;
            retorno.UsuarioUltimaAtualizacao = dados.Usuario;

            // Endereço de Correspondência
            retorno.CodCepTecnologia = dados.EnderecoCorrespondencia.CEP.Substring(0, 5);
            retorno.CodComplCepTecnologia = dados.EnderecoCorrespondencia.CEP.Substring(6, 3);
            retorno.LogradouroTecnologia = dados.EnderecoCorrespondencia.Logradouro;
            retorno.ComplEnderecoTecnologia = dados.EnderecoCorrespondencia.Complemento;
            retorno.NumEnderecoTecnologia = dados.EnderecoCorrespondencia.Numero;
            retorno.BairroTecnologia = dados.EnderecoCorrespondencia.Bairro;
            retorno.CidadeTecnologia = dados.EnderecoCorrespondencia.Cidade;
            retorno.EstadoTecnologia = dados.EnderecoCorrespondencia.Estado;
            retorno.IndEnderecoIgualComercial = dados.EnderecoComercialIgualCorrespondenciaChecked ? 'S' : 'N';

            // Dados Contato
            retorno.NomeContato = dados.NomeContato;
            retorno.NumDDD = dados.DDDTelefone1;
            retorno.NumTelefone = dados.NumeroTelefone1.ToInt32();
            retorno.NumRamal = dados.RamalTelefone1.ToInt32();

            retorno.CodTipoEquipamento = dados.CodEquipamento;
            retorno.NumPontoVenda = dados.NumPdv;

            retorno.DiaFimFuncionamento = dados.DiaFuncionamentoFinal.ToInt32();
            retorno.DiaInicioFuncionamento = dados.DiaFuncionamentoInicio.ToInt32();
            retorno.HoraFimFuncionamento = dados.HorarioFuncionamentoFinal.ToInt32();
            retorno.HoraInicioFuncionamento = dados.HorarioFuncionamentoInicio.ToInt32();

            retorno.IndHabilitaVendaDigitada = ' ';
            retorno.IndFct = ' ';
            retorno.IndPinPad = 'N';
            retorno.CodPropEquipamento = 1;
            retorno.CodTipoLigacao = 2;
            retorno.CodFilialTecnologia = 0;
            retorno.CodCentroCustoTecnologia = 0;
            retorno.CodRegimeTecnologia = 0;
            retorno.NumeroRenpac = 0;
            retorno.QtdeCheckOut = 0;
            retorno.ValorEquipamento = 0;
            retorno.QtdeTerminalSolicitado = 1;
            retorno.CodigoCenario = null;
            retorno.CodFabricanteHardware = String.Empty;
            retorno.CodFornecedorSoftware = String.Empty;
            retorno.NomeFabricanteHardware = String.Empty;
            retorno.NomeFornecedorSoftware = String.Empty;
            retorno.NumCpfTecnologia = null;
            retorno.CodigoEventoEspecial = null;
            retorno.Observacao = String.Empty;
            retorno.CodigoAcaoComercial = null;
            retorno.TerminalFatrExps = null;

            return retorno;
        }

        /// <summary>
        /// Preenche dados do endereço para gravação
        /// </summary>
        /// <param name="endereco"></param>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static PNTransicoes.Endereco PreencheEndereco(Endereco endereco, DadosCredenciamento dados)
        {
            PNTransicoes.Endereco retorno = new PNTransicoes.Endereco();

            retorno.CodTipoPessoa = dados.TipoPessoa;
            retorno.NumCNPJ = dados.CPF_CNPJ.CpfCnpjToLong();
            retorno.NumSeqProp = dados.NumeroSequencia;
            retorno.UsuarioUltimaAtualizacao = dados.Usuario;
            retorno.DataHoraUltimaAtualizacao = DateTime.Now;

            retorno.IndTipoEndereco = endereco.TipoEndereco;
            retorno.CodigoCep = endereco.CEP.Substring(0, 5);
            retorno.CodComplementoCep = endereco.CEP.Substring(6, 3);
            retorno.Logradouro = endereco.Logradouro;
            retorno.ComplementoEndereco = endereco.Complemento;
            retorno.NumeroEndereco = endereco.Numero;
            retorno.Estado = endereco.Estado;
            retorno.Cidade = endereco.Cidade;
            retorno.Bairro = endereco.Bairro;

            return retorno;
        }

        /// <summary>
        /// Preenche dados do endereço para gravação
        /// </summary>
        /// <param name="endereco"></param>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static FEToken.Endereco PreencheEndereco(Endereco endereco)
        {
            FEToken.Endereco retorno = new FEToken.Endereco();

            retorno.IndTipoEndereco = endereco.TipoEndereco;
            retorno.CodigoCep = endereco.CEP.Substring(0, 5);
            retorno.CodComplementoCep = endereco.CEP.Substring(6, 3);
            retorno.Logradouro = endereco.Logradouro;
            retorno.ComplementoEndereco = endereco.Complemento;
            retorno.NumeroEndereco = endereco.Numero;
            retorno.Estado = endereco.Estado;
            retorno.Cidade = endereco.Cidade;
            retorno.Bairro = endereco.Bairro;

            return retorno;
        }

        /// <summary>
        /// Preenche dados dos proprietários para gravação
        /// </summary>
        /// <param name="proprietarios"></param>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static List<PNTransicoes.Proprietario> PreencheProprietarios(ICollection<Proprietario> proprietarios, DadosCredenciamento dados)
        {
            List<PNTransicoes.Proprietario> retorno = new List<PNTransicoes.Proprietario>();

            foreach (var proprietario in proprietarios)
            {
                retorno.Add(new PNTransicoes.Proprietario
                {
                    CodTipoPessoa = dados.TipoPessoa,
                    NumCNPJ = dados.CPF_CNPJ.CpfCnpjToLong(),
                    NumSeqProp = dados.NumeroSequencia,
                    UsuarioUltimaAtualizacao = dados.Usuario,
                    NumCNPJCPFProprietario = proprietario.CPF_CNPJ.CpfCnpjToLong(),
                    NomeProprietario = proprietario.NomeProprietario,
                    CodTipoPesProprietario = proprietario.TipoPessoa,
                    ParticipacaoAcionaria = proprietario.PartAcionaria,
                    DataNascProprietario = DateTime.Now
                });
            }

            return retorno;
        }

        /// <summary>
        /// Preenche dados da proposta
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static PNTransicoes.Proposta PreencheProposta(DadosCredenciamento dados)
        {
            PNTransicoes.Proposta retorno = new PNTransicoes.Proposta();

            retorno.CodTipoPessoa = dados.TipoPessoa;
            retorno.DataFundacao = dados.DataFundacao;
            retorno.RazaoSocial = dados.RazaoSocial;
            retorno.NumCnpjCpf = dados.CPF_CNPJ.CpfCnpjToLong();
            retorno.IndSeqProp = dados.NumeroSequencia;
            retorno.UsuarioUltimaAtualizacao = dados.Usuario;
            retorno.UsuarioInclusao = dados.Usuario;
            retorno.CodCanal = dados.Canal;
            retorno.CodMotivoRecusa = 0;
            retorno.NumPdv = dados.NumPdv;
            retorno.CodTipoMovimento = dados.CodTipoMovimento == default(Char) ? 'I' : dados.CodTipoMovimento;
            retorno.CodGrupoRamo = dados.CodigoGrupoAtuacao;
            retorno.CodRamoAtividade = dados.CodigoRamoAtividade;
            retorno.IndEnderecoIgualCom = dados.EnderecoComercialIgualCorrespondenciaChecked ? 'S' : 'N';
            retorno.PessoaContato = dados.NomeContato;
            retorno.IndAcessaInternet = 'N';
            retorno.NomeEmail = dados.Email;
            retorno.NomeHomePage = dados.Site;
            retorno.NumDDD1 = dados.DDDTelefone1;
            retorno.NumTelefone1 = dados.NumeroTelefone1.ToInt32();
            retorno.Ramal1 = dados.RamalTelefone1.ToInt32(0);
            retorno.NumDDD2 = dados.DDDTelefone2;
            retorno.NumTelefone2 = dados.NumeroTelefone2.ToInt32(0);
            retorno.Ramal2 = dados.RamalTelefone2.ToInt32(0);
            retorno.NumDDDFax = dados.DDDFax;
            retorno.NumTelefoneFax = dados.NumeroFax.ToInt32(0);
            retorno.IndRegiaoLoja = dados.EstabelecimentoLocalizadoShoppingChecked ? 'S' : 'R';
            retorno.NomePlaqueta1 = String.Empty;
            retorno.NomePlaqueta2 = String.Empty;
            retorno.CodFilial = 0;
            retorno.CodGerencia = 'V';
            retorno.CodCarteira = 0;
            retorno.CodZona = 0;
            retorno.CodNucleo = 0;
            retorno.CodHoraFuncionamentoPV = 0;
            retorno.IndicadorMaquineta = 'N';
            retorno.QuantidadeMaquineta = 0;
            retorno.IndicadorIATA = ' ';
            retorno.CodTipoEstabelecimento = dados.CodTipoEstabelecimento;
            retorno.IndComercializacaoNormal = dados.IndMarketingDireto == 'S' ? 'N' : 'S';
            retorno.IndComercializacaoCatalogo = dados.IndMarketingDireto;
            retorno.IndComercializacaoTelefone = dados.IndMarketingDireto;
            retorno.IndComercializacaoEletronico = dados.IndMarketingDireto;
            retorno.NumeroMatriz = dados.NumPdvMatriz;
            retorno.NomeFatura = dados.NomeFantasia;
            retorno.CodTipoConsignacao = null;
            retorno.NumeroConsignador = 0;
            retorno.NumGrupoComercial = 0;
            retorno.NumGrupoGerencial = 0;
            retorno.CodLocalPagamento = null;
            retorno.NumCentralizadora = 0;
            retorno.IndSolicitacaoTecnologia = 'N';
            retorno.NomeProprietario1 = null;
            retorno.NumCPFProprietario1 = null;
            retorno.TipoPessoaProprietario1 = null;
            retorno.NomeProprietario2 = null;
            retorno.DataNascProprietario2 = null;
            retorno.NumCPFProprietario2 = null;
            retorno.TipoPessoaProprietario2 = null;
            retorno.CodCelula = dados.Celula;
            retorno.CodRoteiro = null;
            retorno.CodAgenciaFiliacao = 0;
            retorno.CodTerceirizacaoVista = 0;
            retorno.DataCadastroProposta = DateTime.Now;
            retorno.NumCPFVendedor = 0;
            retorno.CodFaseFiliacao = 1;
            retorno.CodImpressoraFiscal = 0;
            retorno.IndFinanceira = 'N';
            retorno.CodFinanceira1 = 0;
            retorno.CodFinanceira2 = 0;
            retorno.CodFinanceira3 = 0;
            retorno.SituacaoProposta = 'N';
            retorno.CodPesoTarget = 0;
            retorno.CodPeriodicidadeRAV = ' ';
            retorno.CodPeriodicidadeDia = 0;
            retorno.ValorTaxaAdesao = 0;
            retorno.IndEnvioForcaVenda = "N";
            retorno.NumInvestigacaoPropDigitada = null;
            retorno.IndOrigemProposta = "Portal";
            retorno.CodigoCampanha = null;
            retorno.CodModeloProposta = null;
            retorno.CodigoEVD = null;
            retorno.IndProntaInstalacao = 'N';
            retorno.IndEnvioExtratoEmail = 'S';
            retorno.IndControle = "PI";
            retorno.CodTipoConselhoRegional = null;
            retorno.NumConselhoRegional = null;
            retorno.UFConselhoRegional = null;
            retorno.CodigoCNAE = dados.CNAE;
            retorno.TaxaAtivacao = 0;
            retorno.ComoConheceu = dados.CodigoComoConheceu.ToInt32();

            return retorno;
        }

        #endregion
    }
}
