using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Boston.Modelo;
using Redecard.PN.Boston.Agentes.WFProposta;
using Redecard.PN.Boston.Agentes.WFEnderecos;
using Redecard.PN.Boston.Agentes.WFProprietarios;
using Redecard.PN.Boston.Agentes.WFTecnologia;
using Redecard.PN.Boston.Agentes.WFDomicilioBancario;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Boston.Agentes.WFProdutos;
using Redecard.PN.Boston.Agentes.WFServicos;
using Redecard.PN.Boston.Agentes.WMOcorrencia;
using Redecard.PN.Boston.Agentes.WFAdministracao;
using Redecard.PN.Boston.Agentes.WFScoreRisco;
using Redecard.PN.Boston.Agentes.WFSerasa;
using Redecard.PN.Boston.Agentes.GEPontoVenda;
using Redecard.PN.Boston.Agentes.GERamosAtd;
using Redecard.PN.Boston.Agentes.TGTipoEquip;
using Redecard.PN.Boston.Agentes.GEFiliaisZonas;
using Redecard.PN.Boston.Agentes.GEProdutos;
using Redecard.PN.Boston.Agentes.GERegimes;
using Redecard.PN.Boston.Agentes.GECanais;
using Redecard.PN.Boston.Agentes.GECelulas;

namespace Redecard.PN.Boston.Agentes
{
    public class TransicoesAG : AgentesBase
    {
        #region [ WF ]

        #region [ Proposta ]

        public void GravarAtualizarProposta(Proposta proposta, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Proposta"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    proposta
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    WFProposta.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoPropostaMPOS(proposta.CodTipoPessoa,
                                        proposta.NumCnpjCpf,
                                        proposta.IndSeqProp,
                                        proposta.UsuarioUltimaAtualizacao,
                                        proposta.UsuarioInclusao,
                                        proposta.CodCanal,
                                        proposta.CodMotivoRecusa,
                                        proposta.NumPdv,
                                        proposta.CodTipoMovimento,
                                        proposta.CodGrupoRamo,
                                        proposta.CodRamoAtividade,
                                        proposta.DataFundacao,
                                        proposta.RazaoSocial,
                                        proposta.IndEnderecoIgualCom,
                                        proposta.PessoaContato,
                                        proposta.IndAcessaInternet,
                                        proposta.NomeEmail,
                                        proposta.NomeHomePage,
                                        proposta.NumDDD1,
                                        proposta.NumTelefone1,
                                        proposta.Ramal1,
                                        proposta.NumDDDFax,
                                        proposta.NumTelefoneFax,
                                        proposta.NumDDD2,
                                        proposta.NumTelefone2,
                                        proposta.Ramal2,
                                        proposta.IndRegiaoLoja,
                                        proposta.NomePlaqueta1,
                                        proposta.NomePlaqueta2,
                                        proposta.CodFilial,
                                        proposta.CodGerencia,
                                        proposta.CodCarteira,
                                        proposta.CodZona,
                                        proposta.CodNucleo,
                                        proposta.CodHoraFuncionamentoPV,
                                        proposta.IndicadorMaquineta,
                                        proposta.QuantidadeMaquineta,
                                        proposta.IndicadorIATA,
                                        proposta.CodTipoEstabelecimento,
                                        proposta.IndComercializacaoNormal,
                                        proposta.IndComercializacaoCatalogo,
                                        proposta.IndComercializacaoTelefone,
                                        proposta.IndComercializacaoEletronico,
                                        proposta.NumeroMatriz,
                                        proposta.NomeFatura,
                                        proposta.CodTipoConsignacao,
                                        proposta.NumeroConsignador,
                                        proposta.NumGrupoComercial,
                                        proposta.NumGrupoGerencial,
                                        proposta.CodLocalPagamento,
                                        proposta.NumCentralizadora,
                                        proposta.IndSolicitacaoTecnologia,
                                        proposta.NomeProprietario1,
                                        proposta.NumCPFProprietario1,
                                        proposta.TipoPessoaProprietario1,
                                        proposta.NomeProprietario2,
                                        proposta.DataNascProprietario2,
                                        proposta.NumCPFProprietario2,
                                        proposta.TipoPessoaProprietario2,
                                        proposta.CodCelula,
                                        proposta.CodRoteiro,
                                        proposta.CodAgenciaFiliacao,
                                        proposta.CodTerceirizacaoVista,
                                        proposta.DataCadastroProposta,
                                        proposta.NumCPFVendedor,
                                        proposta.CodFaseFiliacao,
                                        proposta.CodImpressoraFiscal,
                                        proposta.IndFinanceira,
                                        proposta.CodFinanceira1,
                                        proposta.CodFinanceira2,
                                        proposta.CodFinanceira3,
                                        proposta.SituacaoProposta,
                                        proposta.CodPesoTarget,
                                        proposta.CodPeriodicidadeRAV,
                                        proposta.CodPeriodicidadeDia,
                                        proposta.ValorTaxaAdesao,
                                        proposta.IndEnvioForcaVenda,
                                        proposta.NumInvestigacaoPropDigitada,
                                        proposta.IndOrigemProposta,
                                        proposta.CodigoCampanha,
                                        proposta.CodModeloProposta,
                                        proposta.CodigoEVD,
                                        proposta.IndProntaInstalacao,
                                        proposta.IndEnvioExtratoEmail,
                                        proposta.IndControle,
                                        proposta.CodTipoConselhoRegional,
                                        proposta.NumConselhoRegional,
                                        proposta.UFConselhoRegional,
                                        proposta.CodigoCNAE,
                                        proposta.TaxaAtivacao,
                                        proposta.ComoConheceu
                        );

                    codRetorno = retorno[0].CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public void AtualizaSituacaoProposta(Char codTipoPessoa, Int64 numCNPJ, Int32 numSeqProp, Char indSituacaoProposta, String usuarioUltimaAtualizacao, Int32? codMotivoRecusa, Int32 numeroPontoVenda, Int32? indOrigemAtualizacao, out Int32 codRetorno, out String descricaoRetorno)
        {
            using (var log = Logger.IniciarLog("Atualiza Situação Proposta"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    indSituacaoProposta,
                    usuarioUltimaAtualizacao,
                    codMotivoRecusa,
                    numeroPontoVenda,
                    indOrigemAtualizacao
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.AtualizaSituacaoProposta(codTipoPessoa,
                            numCNPJ,
                            numSeqProp,
                            indSituacaoProposta,
                            usuarioUltimaAtualizacao,
                            codMotivoRecusa,
                            numeroPontoVenda,
                            indOrigemAtualizacao);

                    codRetorno = retorno[0].CodigoErro ?? 0;
                    descricaoRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno, descricaoRetorno });
            }
        }

        public Int32 AtualizaTaxaAtivacaoPropostaMPOS(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, Decimal? valorAtivacao, Int32? codFaseFiliacao, String usuarioUltimaAtualizacao)
        {
            using (var log = Logger.IniciarLog("Atualiza Situação Proposta"))
            {
                Int32 codRetorno = 0;

                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
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
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });

                return codRetorno;
            }
        }

        public void AtualizaOcorrenciaProposta(Char codTipoPessoa, Int64 numCNPJ, Int32 numSeqProp, Int32 numOcorrencia, DateTime dataAberturaOcorrencia, String usuarioUltimaAtualizacao, out Int32 codRetorno, out String descricaoRetorno)
        {
            using (var log = Logger.IniciarLog("Atualiza Ocorrencia Proposta"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    numOcorrencia,
                    dataAberturaOcorrencia,
                    usuarioUltimaAtualizacao
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.AtualizaOcorrenciaProposta(codTipoPessoa, numCNPJ, numSeqProp, numOcorrencia, dataAberturaOcorrencia, usuarioUltimaAtualizacao);

                    codRetorno = retorno[0].CodigoErro ?? 0;
                    descricaoRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno, descricaoRetorno });
            }
        }

        public void ConsultaQuantidadePropostasPendentesEPVsAtivosPorEquipamento(Char codTipoPessoa, Int64 numCNPJ, String codEquipamento, out Int32 qtdePropostasPendentes, out Int32 qtdePVsAtivos)
        {
            using (var log = Logger.IniciarLog("Consulta Quantidade de Propostas Pendentes"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    numCNPJ,
                    codEquipamento,
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var retorno = contexto.Cliente.ConsultaQtdePropostasPendentesEquipamento(codTipoPessoa, numCNPJ, codEquipamento);

                    qtdePropostasPendentes = (Int32)retorno.QtdePropostasPendentes;
                    qtdePVsAtivos = (Int32)retorno.QtdePVsAtivos;
                }

                log.GravarLog(EventoLog.FimAgente, new { qtdePropostasPendentes, qtdePVsAtivos });
            }
        }

        public Int32 GetNumSequencia(Char codTipoPessoa, Int64 cnpjCpf, String codEquipamento)
        {
            using (var log = Logger.IniciarLog("Recupera número de sequência"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    codEquipamento,
                });

                Int32 numSequencia = 0;

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    numSequencia = (Int32)contexto.Cliente.ConsSequenciaPropCredenciamentoPendenteEquipamento(codTipoPessoa, cnpjCpf, codEquipamento);
                }

                log.GravarLog(EventoLog.FimAgente, new { numSequencia });

                return numSequencia;
            }
        }

        public PropostaPendente GetPropostaPendente(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia)
        {
            using (var log = Logger.IniciarLog("Get Proposta Pendente"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                });

                var retorno = new PropostaPendente();

                using (var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var propostaPendente = contexto.Cliente.ConsultaPropostaPorCNPJCPF(codTipoPessoa, cnpjCpf, numSequencia);

                    if (propostaPendente.Length > 0)
                    {
                        retorno.CodTipoEstabelecimento = (Int32)propostaPendente[0].CodTipoEstabelecimento;
                        retorno.CodTipoMovimento = (Char)propostaPendente[0].CodTipoMovimento;
                        retorno.DataInclusao = (DateTime)propostaPendente[0].DataInclusao;
                        retorno.IndEnderecoIgualCom = (Char)propostaPendente[0].IndEnderecoIgualCom;
                        retorno.NomeEmail = propostaPendente[0].NomeEmail.Trim();
                        retorno.NomeFatura = propostaPendente[0].NomeFatura.Trim();
                        retorno.NomeHomePage = propostaPendente[0].NomeHomePage.Trim();
                        retorno.NumDDD1 = propostaPendente[0].NumDDD1.Trim();
                        retorno.NumDDD2 = propostaPendente[0].NumDDD2.Trim();
                        retorno.NumDDDFax = propostaPendente[0].NumDDDFax.Trim();
                        retorno.NumOcorrencia = propostaPendente[0].NumOcorrencia;
                        retorno.NumPdv = (Int32)propostaPendente[0].NumPdv;
                        retorno.NumTelefone1 = (Int32)propostaPendente[0].NumTelefone1;
                        retorno.NumTelefone2 = propostaPendente[0].NumTelefone2;
                        retorno.NumTelefoneFax = propostaPendente[0].NumTelefoneFax;
                        retorno.NumeroMatriz = (Int32)propostaPendente[0].NumeroMatriz;
                        retorno.PessoaContato = propostaPendente[0].PessoaContato;
                        retorno.Ramal1 = (Int32)propostaPendente[0].Ramal1;
                        retorno.Ramal2 = propostaPendente[0].Ramal2;
                        retorno.IndRegiaoLoja = (Char)propostaPendente[0].IndRegiaoLoja;

                        retorno.Canal = (Int32)propostaPendente[0].CodCanal;
                        retorno.Celula = (Int32)propostaPendente[0].CodCelula;
                        retorno.GrupoRamo = (Int32)propostaPendente[0].CodGrupoRamo;
                        retorno.RamoAtividade = (Int32)propostaPendente[0].CodRamoAtividade;
                        retorno.DataFundacao = (DateTime)propostaPendente[0].DataFundacao;
                        retorno.RazaoSocial = propostaPendente[0].RazaoSocial;
                        retorno.CodFilial = (Int32)propostaPendente[0].CodFilial;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        public void ConsultaPropostaCredenciamento(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, out Int32 numPdv, out Char indSituacaoProposta)
        {
            using (var log = Logger.IniciarLog("Consulta Situação Proposta"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa, cnpjCpf, numSequencia
                });

                using(var contexto = new ContextoWCF<ServicoPortalWFPropostaClient>())
                {
                    var proposta = contexto.Cliente.ConsPropCredenciamentoPendente(codTipoPessoa, cnpjCpf, numSequencia);

                    numPdv = (Int32)proposta[0].NumPontoVenda;
                    indSituacaoProposta = (Char)proposta[0].IndSituacaoProposta;  
                }

                log.GravarLog(EventoLog.FimAgente, new { numPdv, indSituacaoProposta });
            }
        }

        #endregion

        #region [ Endereco ]

        public void GravarAtualizarEndereco(Endereco endereco, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Endereço"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    endereco
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFEnderecosClient>())
                {
                    WFEnderecos.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoEnderecos(
                                            endereco.CodTipoPessoa,
                                            endereco.NumCNPJ,
                                            endereco.NumSeqProp,
                                            endereco.IndTipoEndereco,
                                            endereco.Logradouro,
                                            endereco.ComplementoEndereco,
                                            endereco.NumeroEndereco,
                                            endereco.Bairro,
                                            endereco.Cidade,
                                            endereco.Estado,
                                            endereco.CodigoCep,
                                            endereco.CodComplementoCep,
                                            endereco.DataHoraUltimaAtualizacao,
                                            endereco.UsuarioUltimaAtualizacao);

                    codRetorno = retorno[0].CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public List<Endereco> GetEnderecos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Char? indTipoEndereco)
        {
            using (var log = Logger.IniciarLog("Get Enderecos"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                    indTipoEndereco
                });

                var retorno = new List<Endereco>();

                using (var contexto = new ContextoWCF<ServicoPortalWFEnderecosClient>())
                {
                    var enderecos = from e in contexto.Cliente.ConsultaEnderecos(codTipoPessoa, cnpjCpf, numSequencia, indTipoEndereco)
                                    where e.IndTipoEndereco == '1' || e.IndTipoEndereco == '2'
                                    select e;

                    foreach (var endereco in enderecos)
                    {
                        retorno.Add(new Endereco
                        {
                            IndTipoEndereco = (Char)endereco.IndTipoEndereco,
                            Logradouro = endereco.Logradouro.Trim(),
                            ComplementoEndereco = endereco.ComplementoEndereco.Trim(),
                            NumeroEndereco = endereco.NumeroEndereco.Trim(),
                            Bairro = endereco.Bairro.Trim(),
                            Cidade = endereco.Cidade.Trim(),
                            Estado = endereco.Estado.Trim(),
                            CodigoCep = endereco.CodigoCep.Trim(),
                            CodComplementoCep = endereco.CodComplementoCep.Trim()
                        });
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        #endregion

        #region [ Produtos e Patamares ]

        public void GravarAtualizarProdutos(List<Produto> produtos, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Produtos"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    produtos
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProdutosClient>())
                {
                    codRetorno = 0;
                    Int32 count = 0;

                    while (codRetorno == 0 && produtos.Count > count)
                    {
                        WFProdutos.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoProduto(
                            produtos[count].CodTipoPessoa,
                            produtos[count].NumCNPJ,
                            produtos[count].NumSeqProp,
                            produtos[count].CodCca,
                            produtos[count].IndTipoOperacaoProd,
                            produtos[count].CodFeature,
                            produtos[count].TipoRegimeNegociado,
                            produtos[count].CodRegimePadrao,
                            produtos[count].PrazoPadrao,
                            produtos[count].TaxaPadrao,
                            produtos[count].CodRegimeMinimo,
                            produtos[count].PrazoMinimo,
                            produtos[count].TaxaMinimo,
                            produtos[count].IndAceitaFeature,
                            produtos[count].Usuario,
                            produtos[count].ValorLimiteParcela,
                            produtos[count].IndFormaPagamento
                            );

                        codRetorno = retorno[0].CodigoErro ?? 0;
                        count++;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public void GravarAtualizarPatamares(List<Patamar> patamares, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Patamares"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    patamares
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProdutosClient>())
                {
                    codRetorno = 0;
                    Int32 count = 0;

                    while (codRetorno == 0 && patamares.Count > count)
                    {
                        WFProdutos.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoPatamares(
                            patamares[count].codTipoPessoa,
                            patamares[count].numCNPJ,
                            patamares[count].numSeqProp,
                            patamares[count].codCca,
                            patamares[count].indTipoOperacaoProd,
                            patamares[count].codFeature,
                            patamares[count].sequenciaPatamar,
                            patamares[count].patamarInicial,
                            patamares[count].patamarFinal,
                            patamares[count].taxaPatamar,
                            patamares[count].codRegimePatamar,
                            patamares[count].usuario
                            );

                        codRetorno = retorno[0].CodigoErro ?? 0;
                        count++;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public void GravarAtualizarProdutosVan(List<ProdutoVan> produtosVan, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Produtos Van"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    produtosVan
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProdutosClient>())
                {
                    codRetorno = 0;
                    Int32 count = 0;

                    while (codRetorno == 0 && produtosVan.Count > count)
                    {
                        WFProdutos.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoProdutoVan(
                            produtosVan[count].CodTipoPessoa,
                            produtosVan[count].NumCNPJ,
                            produtosVan[count].NumSeqProp,
                            produtosVan[count].CodCca,
                            produtosVan[count].IndTipoOperacaoProd,
                            produtosVan[count].Usuario);

                        codRetorno = retorno[0].CodigoErro ?? 0;
                        count++;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public void ExcluiTodosProdutos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Exclui Todos os Produtos"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProdutosClient>())
                {
                    var retorno = contexto.Cliente.ExclusaoTodosProduto(codTipoPessoa, cnpjCpf, numSequencia);

                    codRetorno = retorno.CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        #endregion

        #region [ Tecnologia ]

        public void GravarAtualizarTecnologia(Tecnologia tecnologia, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Tecnologia"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    tecnologia
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFTecnologiaClient>())
                {
                    WFTecnologia.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoTecnologia(
                        tecnologia.CodTipoPessoa,
                        tecnologia.NumCNPJ,
                        tecnologia.NumSeqProp,
                        tecnologia.CodTipoEquipamento,
                        tecnologia.QtdeTerminalSolicitado,
                        tecnologia.CodPropEquipamento,
                        tecnologia.CodTipoLigacao,
                        tecnologia.IndHabilitaVendaDigitada,
                        tecnologia.IndEnderecoIgualComercial,
                        tecnologia.LogradouroTecnologia,
                        tecnologia.ComplEnderecoTecnologia,
                        tecnologia.NumEnderecoTecnologia,
                        tecnologia.BairroTecnologia,
                        tecnologia.CidadeTecnologia,
                        tecnologia.EstadoTecnologia,
                        tecnologia.CodCepTecnologia,
                        tecnologia.CodComplCepTecnologia,
                        tecnologia.NomeContato,
                        tecnologia.NumDDD,
                        tecnologia.NumTelefone,
                        tecnologia.NumRamal,
                        tecnologia.CodFabricanteHardware,
                        tecnologia.NomeFabricanteHardware,
                        tecnologia.CodFornecedorSoftware,
                        tecnologia.NomeFornecedorSoftware,
                        tecnologia.NumeroRenpac,
                        tecnologia.DiaInicioFuncionamento,
                        tecnologia.DiaFimFuncionamento,
                        tecnologia.HoraInicioFuncionamento,
                        tecnologia.HoraFimFuncionamento,
                        tecnologia.CodRegimeTecnologia,
                        tecnologia.CodCentroCustoTecnologia,
                        tecnologia.ValorEquipamento,
                        tecnologia.CodFilialTecnologia,
                        tecnologia.Observacao,
                        tecnologia.QtdeCheckOut,
                        tecnologia.NumPontoVenda,
                        tecnologia.IndPinPad,
                        tecnologia.IndFct,
                        tecnologia.UsuarioUltimaAtualizacao,
                        tecnologia.CodigoCenario,
                        tecnologia.CodigoEventoEspecial,
                        tecnologia.NumCpfTecnologia,
                        tecnologia.CodigoAcaoComercial,
                        tecnologia.TerminalFatrExps
                        );

                    codRetorno = retorno[0].CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public Tecnologia GetDadosTecnologia(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia)
        {
            using (var log = Logger.IniciarLog("Get Dados Tecnologia"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia
                });

                var retorno = new Tecnologia();

                using (var contexto = new ContextoWCF<ServicoPortalWFTecnologiaClient>())
                {
                    var tecnologia = contexto.Cliente.ConsultaTecnologia(codTipoPessoa, cnpjCpf, numSequencia);

                    if (tecnologia.Length > 0)
                    {
                        retorno.DiaFimFuncionamento = (Int32)tecnologia[0].DiaFimFuncionamento;
                        retorno.DiaInicioFuncionamento = (Int32)tecnologia[0].DiaInicioFuncionamento;
                        retorno.HoraFimFuncionamento = (Int32)tecnologia[0].HoraFimFuncionamento;
                        retorno.HoraInicioFuncionamento = (Int32)tecnologia[0].HoraInicioFuncionamento;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        #endregion

        #region [ Domicilio Bancario ]

        public void GravarAtualizarDomicilioBancario(DomicilioBancario domicilioBancario, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Domicílio Bancário"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    domicilioBancario
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFDomicilioBancarioClient>())
                {
                    DomicilioBancarioInclusaoAlteracaoDomicilioBancario[] retorno = contexto.Cliente.InclusaoAlteracaoDomicilioBancario(
                        domicilioBancario.CodTipoPessoa,
                        domicilioBancario.NumCNPJ,
                        domicilioBancario.NumSeqProp,
                        domicilioBancario.IndTipoOperacaoProd,
                        domicilioBancario.IndDomicilioDuplicado,
                        domicilioBancario.CodigoBanco,
                        domicilioBancario.NomeBanco,
                        domicilioBancario.CodigoAgencia,
                        domicilioBancario.NumContaCorrente,
                        domicilioBancario.DataHoraUltimaAtualizacao,
                        domicilioBancario.UsuarioUltimaAtualizacao,
                        domicilioBancario.IndConfirmacaoDomicilio
                        );

                    codRetorno = retorno[0].CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public DomicilioBancario GetDomicilioBancario(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Int32 indTipoOperacaoProd)
        {
            using (var log = Logger.IniciarLog("Get Domicílio Bancário"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                    indTipoOperacaoProd
                });

                var retorno = new DomicilioBancario();

                using (var contexto = new ContextoWCF<ServicoPortalWFDomicilioBancarioClient>())
                {
                    var domicilioBancario = contexto.Cliente.ConsultaDomicilioBancario(codTipoPessoa, cnpjCpf, numSequencia, indTipoOperacaoProd);

                    if (domicilioBancario.Length > 0)
                    {
                        retorno.IndTipoOperacaoProd = (Int32)domicilioBancario[0].IndTipoOperacaoProd;
                        retorno.CodigoBanco = (Int32)domicilioBancario[0].CodBancoCompensacao;
                        retorno.NomeBanco = domicilioBancario[0].NomeBanco;
                        retorno.CodigoAgencia = (Int32)domicilioBancario[0].CodigoAgencia;
                        retorno.NumContaCorrente = domicilioBancario[0].NumContaCorrente;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        #endregion

        #region [ Servicos ]

        public void GravarAtualizarServicos(List<Servico> servicos, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Serviços"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    servicos
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFServicosClient>())
                {
                    codRetorno = 0;
                    Int32 count = 0;

                    while (codRetorno == 0 && servicos.Count > count)
                    {
                        WFServicos.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoServicos(
                                    servicos[count].CodTipoPessoa,
                                    servicos[count].NumCNPJ,
                                    servicos[count].NumSeqProp,
                                    servicos[count].CodServico,
                                    servicos[count].CodRegimeServico,
                                    servicos[count].IndAceitaServico,
                                    servicos[count].QtdeMinimaConsulta,
                                    servicos[count].ValorFranquia,
                                    servicos[count].IndHabilitaCargaPre,
                                    servicos[count].UsuarioUltimaAtualizacao
                                    );

                        codRetorno = retorno[0].CodigoErro ?? 0;
                        count++;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        #endregion

        #region [ Administração ]

        public void ConsultaScript(Int32? codigoScript, Char? indEmissaoCarta, out String descricaoScript)
        {
            using (var log = Logger.IniciarLog("Consulta Script"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codigoScript,
                    indEmissaoCarta
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFAdministracaoClient>())
                {
                    var retorno = contexto.Cliente.ConsultaScripts(codigoScript, indEmissaoCarta);

                    descricaoScript = retorno[0].DescricaoScript;
                }

                log.GravarLog(EventoLog.FimAgente, new { descricaoScript });
            }
        }

        #endregion

        #region [ Score Risco ]

        public void CalculoScoreRisco(Int64 numCNPJ, Char codTipoPessoa, Int32 numOcorrencia, DateTime dataFundacao, String usuario, Int32 codGrupoRamo, Int32 codRamoAtivididade, String codCEP, Int32 codTipoEstabelecimento, Int32 codCanal, String codTipoEquipamento, Int32 codBanco, Int32 codServico, out DateTime dataSituacaoScore)
        {
            using (var log = Logger.IniciarLog("Calculo Score Risco"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numCNPJ,
                    codTipoPessoa,
                    numOcorrencia,
                    dataFundacao,
                    usuario,
                    codGrupoRamo,
                    codRamoAtivididade,
                    codCEP,
                    codTipoEstabelecimento,
                    codCanal,
                    codTipoEquipamento,
                    codBanco,
                    codServico
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFScoreRiscoClient>())
                {
                    var retorno = contexto.Cliente.CalculoScoreRisco(numCNPJ, codTipoPessoa, numOcorrencia, dataFundacao, usuario, codGrupoRamo, codRamoAtivididade, codCEP, codTipoEstabelecimento, codCanal, codTipoEquipamento, codBanco, codServico);

                    dataSituacaoScore = (DateTime)retorno[0].DataSituacaoScore;
                }

                log.GravarLog(EventoLog.FimAgente, new { dataSituacaoScore });
            }
        }

        public void AtualizaOcorrenciaScoreRisco(Int64 numCNPJ, Char codTipoPessoa, Int32 numOcorrencia, DateTime dataSituacaoScore, out Int32 codRetorno, out String descricaoRetorno)
        {
            using (var log = Logger.IniciarLog("Atualiza Ocrrencia Score Risco"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numCNPJ,
                    codTipoPessoa,
                    numOcorrencia,
                    dataSituacaoScore
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFScoreRiscoClient>())
                {
                    var retorno = contexto.Cliente.AtualizaOcorrenciaScoreRisco(numCNPJ, codTipoPessoa, numOcorrencia, dataSituacaoScore);

                    codRetorno = retorno[0].CodigoErro ?? 0;
                    descricaoRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno, descricaoRetorno });
            }
        }

        #endregion

        #region [ Proprietarios ]

        public void GravarAtualizarProprietarios(List<Proprietario> proprietarios, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Proprietarios"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    proprietarios
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProprietariosClient>())
                {
                    codRetorno = 0;
                    Int32 count = 0;

                    while (codRetorno == 0 && proprietarios.Count > count)
                    {
                        WFProprietarios.RetornoErro[] retorno = contexto.Cliente.InclusaoAlteracaoProprietarios(
                            proprietarios[count].CodTipoPessoa,
                            proprietarios[count].NumCNPJ,
                            proprietarios[count].NumSeqProp,
                            proprietarios[count].CodTipoPesProprietario,
                            proprietarios[count].NumCNPJCPFProprietario,
                            proprietarios[count].NomeProprietario,
                            proprietarios[count].DataNascProprietario,
                            proprietarios[count].ParticipacaoAcionaria,
                            proprietarios[count].UsuarioUltimaAtualizacao);

                        codRetorno = retorno[0].CodigoErro ?? 0;
                        count++;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        public List<Proprietario> GetProprietarios(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Char? codTipoPesProprietario, Int64? numCNPJCPFProprietario, Char? indBuscaProprietario)
        {
            using (var log = Logger.IniciarLog("Get Proprietarios"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                    codTipoPesProprietario,
                    numCNPJCPFProprietario,
                    indBuscaProprietario
                });

                List<Proprietario> retorno = new List<Proprietario>();

                using (var contexto = new ContextoWCF<ServicoPortalWFProprietariosClient>())
                {
                    var proprietarios = contexto.Cliente.ConsultaProprietarios(codTipoPessoa, cnpjCpf, numSequencia, codTipoPesProprietario, numCNPJCPFProprietario, indBuscaProprietario);

                    foreach (var p in proprietarios)
                    {
                        retorno.Add(new Proprietario
                        {
                            NumCNPJ = cnpjCpf,
                            CodTipoPessoa = codTipoPessoa,
                            NumSeqProp = numSequencia,
                            NumCNPJCPFProprietario = (Int64)p.NumCNPJCPFProprietario,
                            CodTipoPesProprietario = (Char)p.CodTipoPesProprietario,
                            NomeProprietario = p.NomeProprietario
                        });
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        public void ExcluiTodosProprietarios(List<Proprietario> proprietarios, out Int32 codRetorno)
        {
            codRetorno = 0;
            Int32 contador = 0;

            while (codRetorno == 0 && contador < proprietarios.Count)
            {
                ExcluiProprietario(proprietarios[contador], out codRetorno);
                contador++;
            }
        }

        public void ExcluiProprietario(Proprietario proprietario, out Int32 codRetorno)
        {
            using (var log = Logger.IniciarLog("Exclui Proprietarios"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    proprietario
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFProprietariosClient>())
                {
                    Char codTipoPessoa = proprietario.CodTipoPessoa;
                    Int64 numCNPJ = proprietario.NumCNPJ;
                    Int32 numSeqProp = proprietario.NumSeqProp;
                    Char codTipoPesProprietario = proprietario.CodTipoPesProprietario;
                    Int64 numCNPJCPFProprietario = proprietario.NumCNPJCPFProprietario;
                    String nomeProprietario = proprietario.NomeProprietario;
                    DateTime? dataNascProprietario = null;
                    Double? participacaoAcionaria = null;
                    String usuarioUltimaAtualizacao = String.Empty;

                    var retorno = contexto.Cliente.ExclusaoProprietarios(codTipoPessoa, numCNPJ, numSeqProp, codTipoPesProprietario, numCNPJCPFProprietario, nomeProprietario, dataNascProprietario, participacaoAcionaria, usuarioUltimaAtualizacao);

                    codRetorno = retorno[0].CodigoErro ?? 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno });
            }
        }

        #endregion

        #endregion

        #region [ WM ]

        #region [ Ocorrencia ]

        public void CancelaOcorrenciaCredenciamento(String usuarioOcorrencia, Int32 numSolicitacao, Int32 codCasoOcorrencia, String motivoCancelamento, String obsCancelamento, out Int32 codRetorno, out String descricaoRetorno)
        {
            using (var log = Logger.IniciarLog("Cancela Ocorrencia Credenciamento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    usuarioOcorrencia,
                    numSolicitacao,
                    codCasoOcorrencia,
                    motivoCancelamento,
                    obsCancelamento
                });

                using (var contexto = new ContextoWCF<ServicoPortalWMOcorrenciaClient>())
                {
                    var retorno = contexto.Cliente.CancelaOcorrenciaCredenciamento(usuarioOcorrencia, numSolicitacao, codCasoOcorrencia, motivoCancelamento, obsCancelamento);

                    codRetorno = retorno[0].CodErro ?? 0;
                    descricaoRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno, descricaoRetorno });
            }
        }

        public void AberturaOcorrenciaCredenciamento(
                String usuarioOcorrencia,
                Int32 numPontoVenda,
                String codTipoPessoa,
                String codTipoAmbiente,
                String numCNPJCPFCliente,
                String descricaoOcorrencia,
                out Int32 codCasoOcorrencia,
                out DateTime dataRequisicaoOcorrencia,
                out Int32 numRequisicaoOcorrencia,
                out Int32 numSolicitacao)
        {
            using (var log = Logger.IniciarLog("Abertura Ocorrencia Credenciamento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    usuarioOcorrencia,
                    numPontoVenda,
                    codTipoPessoa,
                    codTipoAmbiente,
                    numCNPJCPFCliente,
                    descricaoOcorrencia
                });

                using (var contexto = new ContextoWCF<ServicoPortalWMOcorrenciaClient>())
                {
                    var retorno = contexto.Cliente.AberturaOcorrenciaCredenciamento(
                        usuarioOcorrencia,
                        numPontoVenda,
                        codTipoPessoa,
                        codTipoAmbiente,
                        numCNPJCPFCliente,
                        descricaoOcorrencia);

                    codCasoOcorrencia = (Int32)retorno[0].CodCasoOcorrencia;
                    dataRequisicaoOcorrencia = (DateTime)retorno[0].DataRequisicaoOcorrencia;
                    numRequisicaoOcorrencia = (Int32)retorno[0].NumRequisicaoOcorrencia;
                    numSolicitacao = (Int32)retorno[0].NumSolicitacao;
                }

                log.GravarLog(EventoLog.FimAgente, new { codCasoOcorrencia, dataRequisicaoOcorrencia, numRequisicaoOcorrencia, numSolicitacao });
            }
        }

        public void GravaOcorrenciaCredenciamento(Int32 numRequisicaoOcorrencia, DateTime dataRequisicaoOcorrencia,
                    String usuarioOcorrencia, Int32 numSolicitacao, Int32 codCasoOcorrencia, String codTipoOcorrencia,
                    Int64 numCNPJCPF, Int32 numSeqProposta, String razaoSocial, Char codTipoPessoa, DateTime dataFundacao,
                    Int32 codGrupoRamo, Int32 codRamoAtividade, String descRamoAtividade, String nomeFatura, String logradouro,
                    String complementoEndereco, String numeroEndereco, String bairro, String cidade, String estado,
                    String codigoCep, String codComplementoCep, String pessoaContato, String numDDD1, Int32 numTelefone1,
                    Int32 ramal1, String numDDDFax, Int32 numTelefoneFax, Int32 codFilial, Int32 numeroPontoVenda,
                    Char codCategoriaPontoVenda, Char indPropostaEmissor, Int32 codCanal, String descCanal, Int32 codCelula,
                    String descCelula, Int32 codPesoTarget, Char indProntaInstalacao, String textoScript, Char indMatrizRisco,
                    out Int32 codRetorno, out String descricaoRetorno)
        {
            using (var log = Logger.IniciarLog("Grava Ocorrencia Credenciamento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numRequisicaoOcorrencia, dataRequisicaoOcorrencia,
                    usuarioOcorrencia, numSolicitacao, codCasoOcorrencia, codTipoOcorrencia,
                    numCNPJCPF, numSeqProposta, razaoSocial, codTipoPessoa, dataFundacao,
                    codGrupoRamo, codRamoAtividade, descRamoAtividade, nomeFatura, logradouro,
                    complementoEndereco, numeroEndereco, bairro, cidade, estado,
                    codigoCep, codComplementoCep, pessoaContato, numDDD1, numTelefone1,
                    ramal1, numDDDFax, numTelefoneFax, codFilial, numeroPontoVenda,
                    codCategoriaPontoVenda, indPropostaEmissor, codCanal, descCanal, codCelula,
                    descCelula, codPesoTarget, indProntaInstalacao, textoScript, indMatrizRisco
                });

                using (var contexto = new ContextoWCF<ServicoPortalWMOcorrenciaClient>())
                {
                    var retorno = contexto.Cliente.GravaOcorrenciaCredenciamento(
                        numRequisicaoOcorrencia,
                        dataRequisicaoOcorrencia,
                        usuarioOcorrencia,
                        numSolicitacao,
                        codCasoOcorrencia,
                        codTipoOcorrencia,
                        numCNPJCPF,
                        numSeqProposta,
                        razaoSocial,
                        codTipoPessoa,
                        dataFundacao,
                        codGrupoRamo,
                        codRamoAtividade,
                        descRamoAtividade,
                        nomeFatura,
                        logradouro,
                        complementoEndereco,
                        numeroEndereco,
                        bairro,
                        cidade,
                        estado,
                        codigoCep,
                        codComplementoCep,
                        pessoaContato,
                        numDDD1,
                        numTelefone1,
                        ramal1,
                        numDDDFax,
                        numTelefoneFax,
                        codFilial,
                        numeroPontoVenda,
                        codCategoriaPontoVenda,
                        indPropostaEmissor,
                        codCanal,
                        descCanal,
                        codCelula,
                        descCelula,
                        codPesoTarget,
                        indProntaInstalacao,
                        textoScript,
                        indMatrizRisco);

                    codRetorno = retorno[0].CodErro ?? 0;
                    descricaoRetorno = retorno[0].DescricaoErro;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRetorno, descricaoRetorno });
            }
        }

        #endregion

        #endregion

        #region [ GE ]

        #region [ Ponto de Venda ]

        public void ConsultaTipoEstabelecimento(Char tipoPessoa, Int64 cpfCnpj, Int32? numPdv, out Int32 codTipoEstabelecimento, out Int32 numPdvMatriz)
        {
            using (var log = Logger.IniciarLog("Consulta Tipo Estabelecimento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    tipoPessoa, cpfCnpj, numPdv
                });

                if (tipoPessoa == 'J')
                {
                    using (var contexto = new ContextoWCF<ServicoPortalGEPontoVendaClient>())
                    {
                        var retorno = contexto.Cliente.ConsultaTipoEstabCredenciamento(cpfCnpj, numPdv);

                        if (retorno.Length > 0)
                        {
                            codTipoEstabelecimento = (Int32)retorno[0].CodTipoEstabelecimento;
                            numPdvMatriz = (Int32)retorno[0].NumPdvMatriz;
                        }
                        else
                        {
                            codTipoEstabelecimento = 2;
                            numPdvMatriz = 0;
                        }
                    }
                }
                else
                {
                    codTipoEstabelecimento = 0;
                    numPdvMatriz = 0;
                }

                log.GravarLog(EventoLog.FimAgente, new { codTipoEstabelecimento, numPdvMatriz });
            }
        }

        #endregion

        #region [ Ramos Atividade ]

        public Boolean ValidaCanalTipoPessoa(Int32 codGrupoRamo, Int32 codRamoAtividade, Int32 codCanal, Char tipoPessoa)
        {
            using (var log = Logger.IniciarLog("Valida Canal Tipo Pessoa"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codGrupoRamo, codRamoAtividade, codCanal, tipoPessoa
                });

                Boolean valido = false;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ValidaCanalTipoPessoa(codGrupoRamo, codRamoAtividade, codCanal, tipoPessoa);

                    if (retorno.Length > 0 && retorno[0].IndPermissao != null)
                        valido = retorno[0].IndPermissao == 'N' ? false : true;
                }

                log.GravarLog(EventoLog.FimAgente, new { valido });

                return valido;
            }
        }

        public Boolean ListaRamosAtividadesPorCnaeEquipamento(String codCNAE, String codEquipamento, ref Int32 codGrupoRamo, ref Int32 codRamoAtividade)
        {
            using (var log = Logger.IniciarLog("Lista Ramos Atividades Por CNAE Equipamento"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codCNAE, codEquipamento
                });

                Boolean sucessoDeParaCNAE = false;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaRamosAtividadesPorCnaeEquipamento(codCNAE, codEquipamento);

                    if (retorno.Length > 0)
                    {
                        codGrupoRamo = (Int32)retorno[0].CodGrupoRamo;
                        codRamoAtividade = (Int32)retorno[0].CodRamoAtivididade;
                        sucessoDeParaCNAE = true;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { sucessoDeParaCNAE, codGrupoRamo, codRamoAtividade });

                return sucessoDeParaCNAE;
            }
        }

        public Boolean ValidaRamoTarget(Int32 codGrupoRamo, Int32 codRamoAtividade, ref Char indMarketingDireto)
        {
            using (var log = Logger.IniciarLog("Valida Ramo Target"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codGrupoRamo, codRamoAtividade
                });

                Boolean ramoTargetValido = false;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosComplementaresRamosAtividades(codGrupoRamo, codRamoAtividade);

                    if (retorno.Length > 0)
                    {
                        indMarketingDireto = (Char)retorno[0].IndMarketingDireto;
                        ramoTargetValido = retorno[0].IndRamoTarget == 'N' ? false : true;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { ramoTargetValido, indMarketingDireto });
                
                return ramoTargetValido;
            }
        }

        public String GetDescRamoAtividade(Int32 codGrupoRamo, Int32 codRamoAtividade)
        {
            using (var log = Logger.IniciarLog("Get Descrição Ramo de Atividade"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codGrupoRamo,
                    codRamoAtividade
                });

                String descRamoAtividade = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastraisRamosAtividades(codGrupoRamo, codRamoAtividade);

                    descRamoAtividade = retorno[0].DescrRamoAtividade;
                }

                log.GravarLog(EventoLog.FimAgente, new { descRamoAtividade });

                return descRamoAtividade;
            }
        }

        public Char GetVendaDigitada(Int32 codGrupoRamo, Int32 codRamoAtividade, Char codTipoPessoa)
        {
            using (var log = Logger.IniciarLog("Get Venda Digitada"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codGrupoRamo,
                    codRamoAtividade
                });

                Char indVendaDigitada = default(Char);

                using (var contexto = new ContextoWCF<ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ConsultaVendaDigitadaPorRamoAtividade(codGrupoRamo, codRamoAtividade, codTipoPessoa);

                    indVendaDigitada = (Char)retorno[0].IndVendaDigitada;
                }

                log.GravarLog(EventoLog.FimAgente, new { indVendaDigitada });

                return indVendaDigitada;
            }
        }

        #endregion

        #region [ Canal ]

        public String GetDescCanal(Char? indSituacaoCanal, Int32 codCanal, String indSinalizacao)
        {
            using (var log = Logger.IniciarLog("Get Descricação Canal"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    indSituacaoCanal, codCanal, indSinalizacao
                });

                String descricaoCanal = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGECanaisClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastrais(indSituacaoCanal, codCanal, indSinalizacao);

                    descricaoCanal = retorno[0].NomeCanal;
                }

                log.GravarLog(EventoLog.FimAgente, new { descricaoCanal });

                return descricaoCanal;
            }
        }

        #endregion

        #region [ Celula ]

        public String GetDescCelula(Int32 codCanal, Int32 codCelula, Int32? codAgencia)
        {
            using (var log = Logger.IniciarLog("Get Descricação Celula"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codCanal, codCelula, codAgencia
                });

                String descricaoCelula = String.Empty;

                using (var contexto = new ContextoWCF<ServicoPortalGECelulasClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastraisPorCanal(codCanal, codCelula, codAgencia);

                    descricaoCelula = retorno[0].NomeCelula;
                }

                log.GravarLog(EventoLog.FimAgente, new { descricaoCelula });

                return descricaoCelula;
            }
        }

        #endregion

        #region [ Filiais Zonas ]

        public void GetDadosFiliaisZonas(Char tipoOperacao, String codCEP, Char codTipoCep, ref Int32? codFilial, ref Int32? codZonaVenda)
        {
            using (var log = Logger.IniciarLog("Get Dados Filiais Zonas"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    tipoOperacao, codCEP, codTipoCep
                });

                using (var contexto = new ContextoWCF<ServicoPortalGEFiliaisZonasClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastrais(tipoOperacao, codCEP, codTipoCep, codFilial);

                    if (retorno.Length > 0)
                    {
                        codFilial = retorno[0].CodFilial;
                        codZonaVenda = retorno[0].CodZonaVenda;
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { codFilial, codZonaVenda });
            }
        }

        #endregion

        #region [ Produtos ]

        public List<Produto> GetProdutos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, String usuario, Char? indicadorCredito, Char? indicadorDebito, Char? indicadorVoucher, Char? indicadorPrivate, Int32 codGrupoRamo, String codRamoAtivididade, Int32 codCanalOrigem)
        {
            using (var log = Logger.IniciarLog("Get Produtos"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoPessoa, cnpjCpf, numSequencia, usuario, indicadorCredito, indicadorDebito, indicadorVoucher, indicadorPrivate, codGrupoRamo, codRamoAtivididade, codCanalOrigem
                });

                List<Produto> retorno = new List<Produto>();

                using (var contexto = new ContextoWCF<ServicoPortalGEProdutosClient>())
                {
                    var produtos = contexto.Cliente.ListaDadosProdutosPorRamoCanal(indicadorCredito, indicadorDebito, indicadorVoucher, indicadorPrivate, codGrupoRamo, codRamoAtivididade, codCanalOrigem);

                    foreach (var produto in produtos)
                    {
                        retorno.Add(new Produto
                        {
                            CodCca = (Int32)produto.CodCCA,
                            CodFeature = (Int32)produto.CodFeature,
                            CodRegimeMinimo = 0,
                            CodRegimePadrao = GetCodigoRegime((Int32)produto.ValorPrazoDefault, (Double)produto.ValorTaxaDefault),
                            CodTipoPessoa = codTipoPessoa,
                            IndAceitaFeature = produto.CodTipoNegocio == 'D' ? 'S' : ' ',
                            IndFormaPagamento = (Char)produto.IndFormaPagamento,
                            IndTipoOperacaoProd = (Char)produto.CodTipoNegocio,
                            NumCNPJ = cnpjCpf,
                            NumSeqProp = numSequencia,
                            PrazoMinimo = 0,
                            PrazoPadrao = (Int32)produto.ValorPrazoDefault,
                            TaxaMinimo = 0,
                            TaxaPadrao = (Double)produto.ValorTaxaDefault,
                            TipoRegimeNegociado = 'P',
                            Usuario = usuario,
                            ValorLimiteParcela = (Int32)produto.CodFeature == 3 ? (Double)produto.QtdeDefaultParcela : 0
                        });
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno });

                return retorno;
            }
        }

        #endregion

        #region [ Regime ]

        private Int32 GetCodigoRegime(Int32 valorPrazo, Double valorTaxa)
        {
            using (var log = Logger.IniciarLog("Get Código Regime"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    valorPrazo, valorTaxa
                });

                Int32 codRegime = 0;

                using (var contexto = new ContextoWCF<ServicoPortalGERegimesClient>())
                {
                    var regimes = contexto.Cliente.ConsultaCodigoRegime(valorPrazo, valorTaxa, 0);

                    codRegime = (Int32)regimes[0].CodRegime;
                }

                log.GravarLog(EventoLog.FimAgente, new { codRegime });

                return codRegime;
            }
        }

        #endregion

        #endregion

        #region [ Serasa ]

        public DadosSerasa ConsultaSerasaPJ(String cnpj)
        {
            using (var log = Logger.IniciarLog("Consulta Serasa PJ"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    cnpj
                });

                var dados = new DadosSerasa();

                using (var contexto = new ContextoWCF<SerasaServicoClient>())
                {
                    var retorno = contexto.Cliente.ConsultaSerasaPJ(cnpj);
                    dados.CodigoRetorno = retorno.CodRetorno.ToInt32();

                    if (dados.CodigoRetorno == 0)
                    {
                        dados.RazaoSocial = retorno.ComplGrafia;
                        dados.DataFundacao = retorno.DataFundacao.ToDate("yyyyMMdd");
                        dados.CNAE = retorno.CNAEs[0].CodCNAE;

                        if (retorno.Socios.Length > 0)
                        {
                            dados.Proprietarios = new List<ProprietarioSerasa>();
                            foreach (var socio in retorno.Socios)
                            {
                                dados.Proprietarios.Add(new ProprietarioSerasa()
                                {
                                    CPFCNPJ = String.Compare(socio.TipoPessoa, "F") == 0 ? socio.CPF_CNPJ.ToInt64().FormatToCpf() : socio.CPF_CNPJ.ToInt64().FormatToCnpj(),
                                    Nome = socio.Nome,
                                    Participacao = socio.Participacao,
                                    TipoPessoa = socio.TipoPessoa,
                                });
                            }
                        }
                    }
                }

                log.GravarLog(EventoLog.FimAgente, new { dados });

                return dados;
            }
        }

        #endregion

        #region [ TG ]

        #region [ Tipo Equipamento ]

        public Double GetValorAluguel(String codTipoEquipamento, Char? situacao, Char? indEquipamentoCompartilhado)
        {
            using (var log = Logger.IniciarLog("Get Valor Aluguel"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codTipoEquipamento, situacao, indEquipamentoCompartilhado
                });

                Double valorAluguel = 0;

                using (var contexto = new ContextoWCF<ServicoPortalTGTipoEquipamentoClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastrais(codTipoEquipamento, situacao, indEquipamentoCompartilhado);

                    valorAluguel = (Double)retorno[0].ValorDefaultAluguel;
                }

                log.GravarLog(EventoLog.FimAgente, new { valorAluguel });

                return valorAluguel;
            }
        }

        #endregion

        #endregion
    }
}
