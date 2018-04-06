using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Rede.PN.Credenciamento.Sharepoint.Servicos
{
    public static class ServicosGE
    {
        /// <summary>
        /// Consulta Lista de Canais no GE
        /// </summary>
        /// <param name="indSituacaoCanal">Situação do Canal</param>
        /// <param name="codCanal">Código do Canal</param>
        /// <param name="indSinalizacao">Indicador de Sinalização</param>
        /// <returns>Retorna lista da classe GECanais.CanaisListaDadosCadastrais</returns>
        public static List<GECanais.CanaisListaDadosCadastrais> ConsultaCanais(Char? indSituacaoCanal, Int32? codCanal, String indSinalizacao)
        {
            List<GECanais.CanaisListaDadosCadastrais> canais = new List<GECanais.CanaisListaDadosCadastrais>();

            using (var log = Logger.IniciarLog("GECanais - ListaDadosCadastrais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    indSituacaoCanal,
                    codCanal,
                    indSinalizacao
                });

                using (var contexto = new ContextoWCF<GECanais.ServicoPortalGECanaisClient>())
                {

                    canais = contexto.Cliente.ListaDadosCadastrais(indSituacaoCanal, codCanal, indSinalizacao);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    canais
                });
            }

            return canais;
        }

        /// <summary>
        /// Consulta lista de células no GE
        /// </summary>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <param name="codigoAgencia">Código da Agência</param>
        /// <returns>Retorna Lista da classe GECelulas.CelulasoListaDadosCadastraisPorCanal</returns>
        public static List<GECelulas.CelulasoListaDadosCadastraisPorCanal> ConsultaCelulas(Int32 codigoCanal, Int32? codigoCelula, Int32? codigoAgencia)
        {
            List<GECelulas.CelulasoListaDadosCadastraisPorCanal> celulas = new List<GECelulas.CelulasoListaDadosCadastraisPorCanal>();

            using (var log = Logger.IniciarLog("GECelulas - ListaDadosCadastraisPorCanal"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCanal,
                    codigoCelula,
                    codigoAgencia
                });

                using (var contexto = new ContextoWCF<GECelulas.ServicoPortalGECelulasClient>())
                {
                    celulas = contexto.Cliente.ListaDadosCadastraisPorCanal(codigoCanal, codigoCelula, codigoAgencia);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    celulas
                });
            }

            return celulas;
        }

        /// <summary>
        /// Consulta lista de ramos de Atuação
        /// </summary>
        /// <returns>Retorna Lista da classe GERamosAtd.RamosAtividadesListaDadosCadastraisGruposRamosAtividades</returns>
        public static List<GERamosAtd.RamosAtividadesListaDadosCadastraisGruposRamosAtividades> ConsultaRamosAtuacao()
        {
            List<GERamosAtd.RamosAtividadesListaDadosCadastraisGruposRamosAtividades> ramosAtuacao = new List<GERamosAtd.RamosAtividadesListaDadosCadastraisGruposRamosAtividades>();

            using (var log = Logger.IniciarLog("GERamosAtd - ListaDadosCadastraisGruposRamosAtividades"))
            {
                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    ramosAtuacao = contexto.Cliente.ListaDadosCadastraisGruposRamosAtividades();
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    ramosAtuacao
                });
            }

            return ramosAtuacao;
        }

        /// <summary>
        /// Consulta lista de ramos de Atuação
        /// </summary>
        /// <returns>Retorna lista da classe GERamosAtd.RamosAtividadesListaDadosCadastraisRamosAtividades</returns>
        public static List<GERamosAtd.RamosAtividadesListaDadosCadastraisRamosAtividades> ConsultaRamosAtividade(int? codigoRamoAtuacao, int? codigoRamoAtividade)
        {
            List<GERamosAtd.RamosAtividadesListaDadosCadastraisRamosAtividades> ramosAtividade = new List<GERamosAtd.RamosAtividadesListaDadosCadastraisRamosAtividades>();

            using (var log = Logger.IniciarLog("GERamosAtd - ListaDadosCadastraisGruposRamosAtividades"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoRamoAtuacao,
                    codigoRamoAtividade
                });

                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    ramosAtividade = contexto.Cliente.ListaDadosCadastraisRamosAtividades(codigoRamoAtuacao, codigoRamoAtividade);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    ramosAtividade
                });
            }

            return ramosAtividade;
        }

        /// <summary>
        /// Consulta lista de ramos de atividade para pessoa física
        /// </summary>
        /// <param name="codigoTipoPessoa">Código do tipo de pessoa</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoGrupoRamo">Código do Grupo Ramo</param>
        /// <param name="codigoRamoAtividade">Código do Ramo de Atividade</param>
        /// <returns>Retorna Lista de Ramos Atividades</returns>
        public static List<GERamosAtd.ListaRamosAtividadesPorCanalTipoPessoa> ConsultaRamosAtividade(Char codigoTipoPessoa, Int32? codigoCanal, Int32? codigoGrupoRamo, Int32? codigoRamoAtividade)
        {
            List<GERamosAtd.ListaRamosAtividadesPorCanalTipoPessoa> ramosAtividade = new List<GERamosAtd.ListaRamosAtividadesPorCanalTipoPessoa>();

            using (var log = Logger.IniciarLog("GERamosAtd - ListaRamosAtividadesPorCanalTipoPessoa"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoPessoa,
                    codigoCanal,
                    codigoGrupoRamo,
                    codigoRamoAtividade
                });

                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    ramosAtividade = contexto.Cliente.ListaRamosAtividadesPorCanalTipoPessoa(codigoTipoPessoa, codigoCanal, codigoGrupoRamo, codigoRamoAtividade);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    ramosAtividade
                });
            }

            return ramosAtividade;
        }

        /// <summary>
        /// Consulta lista de Equipamentos por Ramo e Atividade GE
        /// </summary>
        /// <param name="codigoCanal">Código do Grupo do Ramo</param>
        /// <param name="codigoCelula">Código Ramo de Atividade</param>
        /// <returns>Retorna lista de objetos da classe GERamosAtd.ListaEquipamentosPorRamoAtividade</returns>
        public static List<GERamosAtd.ListaEquipamentosPorRamoAtividade> ConsultaEquipamentosPorRamoAtividade(Int32 codGrupoRamo, Int32 codRamoAtividade)
        {
            List<GERamosAtd.ListaEquipamentosPorRamoAtividade> lstEquipamentos = new List<GERamosAtd.ListaEquipamentosPorRamoAtividade>();

            using (var log = Logger.IniciarLog("GERamosAtd - ListaEquipamentosPorRamoAtividade"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codGrupoRamo,
                    codRamoAtividade
                });

                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    lstEquipamentos = contexto.Cliente.ListaEquipamentosPorRamoAtividade(codGrupoRamo, codRamoAtividade);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstEquipamentos
                });
            }

            return lstEquipamentos;
        }

        /// <summary>
        /// Consulta Lista de Bancos
        /// </summary>
        /// <param name="codigoTipoOperacao">Código do tipo de Operação (Credito/Debito/Construcard)</param>
        /// <returns>retorna lista de objetos da classe GEBancos.BancosListaDadosCadastraisReduzidos</returns>
        public static List<GEBancos.BancosListaDadosCadastraisReduzidos> ConsultaBancos(Char codigoTipoOperacao)
        {
            List<GEBancos.BancosListaDadosCadastraisReduzidos> bancos = new List<GEBancos.BancosListaDadosCadastraisReduzidos>();

            using (var log = Logger.IniciarLog("GEBancos - ListaDadosCadastraisReduzidos"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoOperacao
                });

                using (var contexto = new ContextoWCF<GEBancos.ServicoPortalGEBancoClient>())
                {
                    bancos = contexto.Cliente.ListaDadosCadastraisReduzidos(codigoTipoOperacao);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    bancos
                });
            }

            return bancos;
        }

        public static List<GEBancos.BancoConveniadoVoucher> ConsultaBancosConveniadosVoucher(Char operacao, Int32? codCca, Int32? codFeature, Int32? codBanco, String usuario)
        {
            List<GEBancos.BancoConveniadoVoucher> bancos = new List<GEBancos.BancoConveniadoVoucher>();

            using (var log = Logger.IniciarLog("GEBancos - ListaDadosCadastraisReduzidos"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    usuario
                });

                using (var contexto = new ContextoWCF<GEBancos.ServicoPortalGEBancoClient>())
                {
                    bancos = contexto.Cliente.ConsultaBancosParceiros(operacao, codCca, codFeature, codBanco, usuario);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    bancos
                });
            }

            return bancos;
        }

        /// <summary>
        /// Consulta Agencia
        /// </summary>
        /// <param name="codigoTipoOperacao">Código do Banco</param>
        /// <param name="codigoTipoOperacao">Código da Agencia</param>
        /// <returns>Retorna objeto da classe GEAgencias.ConsultaDetalheAgencia</returns>
        public static GEAgencias.ConsultaDetalheAgencia ConsultaDetalheAgencia(int codigoBanco, int codigoAgencia, ref string iCodigoErro, ref string mensagemErro)
        {
            List<GEAgencias.ConsultaDetalheAgencia> agencias = new List<GEAgencias.ConsultaDetalheAgencia>();

            using (var log = Logger.IniciarLog("GEAgencias - ConsultaDetalheAgencia"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoBanco,
                    codigoAgencia
                });

                using (var contexto = new ContextoWCF<GEAgencias.ServicoPortalGEAgenciasClient>())
                {
                    agencias = contexto.Cliente.ConsultaDetalheAgencia(codigoBanco, codigoAgencia);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    agencias
                });
            }

            if (agencias.Count > 0)
                return agencias[0];

            return null;
        }

        /// <summary>
        /// Serviço que valida agência bancária
        /// </summary>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="codigoAgencia">Código da agência</param>
        /// <returns>Retorna Boolean sobre validação do código da agência</returns>
        public static Boolean ValidaAgencia(Int32 codigoBanco, Int32 codigoAgencia)
        {
            Boolean agenciaValida = false;

            using (var log = Logger.IniciarLog("Valida Agência Bancária"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoBanco,
                    codigoAgencia
                });

                using (var contexto = new ContextoWCF<GEAgencias.ServicoPortalGEAgenciasClient>())
                {
                    var retorno = contexto.Cliente.ValidaAgencias(codigoBanco, codigoAgencia);

                    if (retorno.Count > 0 && retorno[0] != null)
                        agenciaValida = retorno[0].CodErro == 0;
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    agenciaValida
                });
            }

            return agenciaValida;
        }

        /// <summary>
        /// Checa Dígito para Conta Corrente Válida
        /// </summary>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="codigoAgencia">Código da Agencia</param>
        /// <param name="codigoContaCorrente">Número da Conta Corrente</param>
        /// <param name="codigoTipoPessoa">Tipo pessoa do Credenciamento</param>
        /// <param name="mensagemErro">Mensagem de Erro</param>
        /// <returns>retorna boolean para conta corrente válida</returns>
        public static bool ChecaDigito(int codigoBanco, long codigoAgencia, long codigoContaCorrente, Char codigoTipoPessoa, ref string mensagemErro)
        {
            bool contaValida = false;

            using (var log = Logger.IniciarLog("GEContaCorr - ChecaCriterioDigito"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoBanco,
                    codigoAgencia,
                    codigoContaCorrente,
                    codigoTipoPessoa
                });

                using (var contexto = new ContextoWCF<GEContaCorr.ServicoPortalGEContaCorrenteClient>())
                {
                    contaValida = contexto.Cliente.ChecaCriterioDigito(codigoBanco, codigoAgencia, codigoContaCorrente, codigoTipoPessoa);
                }

                if (!contaValida)
                    mensagemErro = "Conta Corrente inválida";


                log.GravarLog(EventoLog.RetornoServico, new
                {
                    contaValida,
                    mensagemErro
                });
            }
            return contaValida;
        }

        /// <summary>
        /// Consulta tipo de Estabelecimento através do CNPJ e Numero do PV
        /// </summary>
        /// <param name="numeroCnpj">Número do CNPJ</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <returns>retorna Tipo de Estabelecimento através do CNPJ e Numero do PV</returns>
        public static GEPontoVen.PontoVendaConsultaTipoEstabCredenciamento ConsultaTipoEstabelecimento(Int64 numeroCnpj, Int32? numeroPV)
        {
            GEPontoVen.PontoVendaConsultaTipoEstabCredenciamento retorno = new GEPontoVen.PontoVendaConsultaTipoEstabCredenciamento();

            using (var log = Logger.IniciarLog("GEPontoVen - ConsultaTipoEstabCredenciamento"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numeroCnpj,
                    numeroPV
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {
                    List<GEPontoVen.PontoVendaConsultaTipoEstabCredenciamento> listaConsultaTipoEstabelecimento = contexto.Cliente.ConsultaTipoEstabCredenciamento(numeroCnpj, numeroPV);

                    if (listaConsultaTipoEstabelecimento.Count > 0)
                        retorno = listaConsultaTipoEstabelecimento[0];
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Retorna lista de Gerencia dos dados cadastrais
        /// </summary>
        /// <returns>Retorna lista de Gerencia dos dados cadastrais</returns>
        public static List<GEGerencias.GerenciasListaDadosCadastrais> ConsultaListaGerencia()
        {
            List<GEGerencias.GerenciasListaDadosCadastrais> retorno = new List<GEGerencias.GerenciasListaDadosCadastrais>();

            using (var log = Logger.IniciarLog("GEGerencias - ConsultaListaGerencia"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new { });

                using (var contexto = new ContextoWCF<GEGerencias.ServicoPortalGEGerenciaClient>())
                {
                    retorno = contexto.Cliente.ListaDadosCadastrais();
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }


        /// <summary>
        /// Consulta Lista de Carteiras
        /// </summary>
        /// <param name="codigoGerencia">Código da Gerência</param>
        /// <param name="codigoCarteira">Código da Carteira</param>
        /// <param name="nomeCarteira">Nome da Carteira</param>
        /// <returns>Retorna Lista de Carteiras</returns>
        public static List<GECarteiras.CarteirasListaDadosCadastrais> ConsultaListaCarteiras(char codigoGerencia, int? codigoCarteira, string nomeCarteira)
        {

            List<GECarteiras.CarteirasListaDadosCadastrais> retorno = new List<GECarteiras.CarteirasListaDadosCadastrais>();

            using (var log = Logger.IniciarLog("GECarteiras - ConsultaListaCarteiras"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new { });

                using (var contexto = new ContextoWCF<GECarteiras.ServicoPortalGECarteirasClient>())
                {
                    retorno = contexto.Cliente.ListaDadosCadastrais(codigoGerencia, codigoCarteira, nomeCarteira);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna validação do número Grupo Comercial
        /// </summary>
        /// <param name="grupoComercial">Grupo Comercial</param>
        /// <returns>retorna objeto da classe GEGrpComGer.CodErroDescricaoErro</returns>
        public static GEGrpComGer.CodErroDescricaoErro ValidaGrupoComercial(Int32 grupoComercial)
        {

            GEGrpComGer.CodErroDescricaoErro retorno = new GEGrpComGer.CodErroDescricaoErro();

            using (var log = Logger.IniciarLog("GEGrpComGer - CodErroDescricaoErro Grupo Comercial"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    grupoComercial
                });

                using (var contexto = new ContextoWCF<GEGrpComGer.ServicoPortalGEGruposComerciaisGerenciaisClient>())
                {
                    retorno = contexto.Cliente.ValidaGrupoComercial(grupoComercial);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna validação do número Grupo Gerencial
        /// </summary>
        /// <param name="grupoComercial">Grupo Gerencial</param>
        /// <returns>retorna objeto da classe GEGrpComGer.CodErroDescricaoErro</returns>
        public static GEGrpComGer.CodErroDescricaoErro ValidaGrupoGerencial(Int32 grupoGerencial)
        {

            GEGrpComGer.CodErroDescricaoErro retorno = new GEGrpComGer.CodErroDescricaoErro();

            using (var log = Logger.IniciarLog("GEGrpComGer - CodErroDescricaoErro Grupo Gerencial"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    grupoGerencial
                });

                using (var contexto = new ContextoWCF<GEGrpComGer.ServicoPortalGEGruposComerciaisGerenciaisClient>())
                {
                    retorno = contexto.Cliente.ValidaGrupoGerencial(grupoGerencial);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna Grupo gerencial do BNDES
        /// </summary>
        /// <param name="grupoGerencial">Grupo Gerencial</param>
        /// <returns>retorna valor do grupo Gerencial BNDES</returns>
        public static int RetornaGrupoGerencialBNDES()
        {

            int retorno = 0;

            using (var log = Logger.IniciarLog("GEGrpComGer - CodErroDescricaoErro Grupo Gerencial"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                });

                using (var contexto = new ContextoWCF<GEGrpComGer.ServicoPortalGEGruposComerciaisGerenciaisClient>())
                {
                    retorno = contexto.Cliente.ConsultaGrupoGerencialBNDES();
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }

        /// <summary>
        /// Retorna lista de PVs pendentes com base no CPNJ/CPF
        /// </summary>
        /// <param name="listaCpfCnpj">Lista de CPF/CNPJ</param>
        /// <returns>retorna lista de PVs Pendentes da classe GEPendencias.PvsPendentes</returns>
        public static GEPendencias.PvsPendentes[] ListaPvsPendentes(Int64[] listaCpfCnpj)
        {
            GEPendencias.PvsPendentes[] listaPvs;

            using (Logger log = Logger.IniciarLog("Lista PVs Pendentes"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaCpfCnpj
                });

                using (var contexto = new ContextoWCF<GEPendencias.ServicoPortalGEConsultaPendenciasPVPorCpfCnpjClient>())
                {
                    listaPvs = contexto.Cliente.ListaPVsPendentes(listaCpfCnpj);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaPvs
                });
            }

            return listaPvs;
        }

        /// <summary>
        /// Retorna a descrição do ramo atividade
        /// </summary>
        /// <param name="grupoRamo">código do Grupo do Ramo</param>
        /// <param name="ramoAtividade">Código do Ramo de Atividade</param>
        /// <returns>Descrição do Ramo de Atividade</returns>
        public static String GetDescricaoRamoAtividade(Int32 grupoRamo, Int32 ramoAtividade)
        {
            String descricaoRamoAtividade;

            using (var log = Logger.IniciarLog("Recupera descrição do ramo de atividade"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    grupoRamo,
                    ramoAtividade
                });

                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastraisRamosAtividades(grupoRamo, ramoAtividade);

                    descricaoRamoAtividade = retorno[0].DescrRamoAtividade;
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    descricaoRamoAtividade
                });
            }

            return descricaoRamoAtividade;
        }

        /// <summary>
        /// Retorna a descrição do Grupo Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoRamo">código grupo Ramo de Atividade</param>
        /// <returns>Retorna string de descrição do Grupo Ramo Atividade</returns>
        public static String GetDescricaoGrupoRamo(Int32 codGrupoRamo)
        {
            String descricaoGrupoRamo;

            using (var log = Logger.IniciarLog("Recupera descrição do grupo ramo"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codGrupoRamo
                });

                using (var contexto = new ContextoWCF<GERamosAtd.ServicoPortalGERamosAtividadesClient>())
                {
                    var retorno = contexto.Cliente.ListaDadosCadastraisGruposRamosAtividades();

                    var grupoRamo = retorno.FirstOrDefault(g => g.CodGrupoRamoAtividade == codGrupoRamo);

                    if (grupoRamo != null)
                        return grupoRamo.DescrRamoAtividade;

                    descricaoGrupoRamo = codGrupoRamo.ToString();
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    descricaoGrupoRamo
                });
            }

            return descricaoGrupoRamo;
        }

        /// <summary>
        /// Recupera Propostas Canceladas ou Ativas
        /// </summary>
        /// <param name="codigoTipoPessoa">Código do tipo de pessoa</param>
        /// <param name="cpfCnpj">CPF/CNPJ</param>
        /// <returns>Retorna Lista de Propostas Canceladas Ou Ativas</returns>
        public static List<Modelo.PropostaPendente> RecuperarPropostasCanceladasOuAtivas(Char codigoTipoPessoa, Int64 cpfCnpj)
        {
            var retorno = new List<Modelo.PropostaPendente>();

            using (Logger log = Logger.IniciarLog("Carrega dados das propostas"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoTipoPessoa,
                    cpfCnpj
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {
                    var pontosVenda = contexto.Cliente.ListaCadastroReduzidoPorCNPJ(codigoTipoPessoa, cpfCnpj);

                    foreach (var pontoVenda in pontosVenda)
                    {
                        retorno.Add(new Modelo.PropostaPendente
                        {
                            NroEstabelecimento = pontoVenda.NumPdv,
                            TipoPessoa = pontoVenda.CodTipoPessoa,
                            CNPJ = pontoVenda.NumCNPJ,
                            RazaoSocial = pontoVenda.RazaoSocial,
                            Ramo = String.Format(@"{0}{1:0000}", pontoVenda.CodGrupoRamo, pontoVenda.CodRamoAtivididade),
                            TipoEstabelecimento = pontoVenda.CodTipoEstabelecimento,
                            Categoria = pontoVenda.CodCategoria,
                            EnderecoComercial = String.Format("{0}, {1}", pontoVenda.NomeLogradouro, pontoVenda.NumLogradouro),
                            StatusProposta = pontoVenda.CodCategoria == 'X' || pontoVenda.CodCategoria == 'E' ? 2 : 3
                        });
                    }
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });

                return retorno;
            }
        }

        /// <summary>
        /// Lista proprietários por Ponto de Venda
        /// </summary>
        /// <param name="numPdv">Número do PDV - Ponto de Venda </param>
        /// <returns>Retorna lista de Proprietários por Ponto de Venda</returns>
        public static List<GEProprietarios.ProprietarioListaDadosPorPontoVenda> ListaProprietariosPontoVenda(Int32 numPdv)
        {
            var proprietarios = new List<GEProprietarios.ProprietarioListaDadosPorPontoVenda>();

            using (Logger log = Logger.IniciarLog("Lista Proprietários por ponto de venda"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<GEProprietarios.ServicoPortalGEProprietariosClient>())
                {
                    proprietarios = contexto.Cliente.ListaDadosPorPontoVenda(numPdv);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    proprietarios
                });
            }

            return proprietarios;
        }

        /// <summary>
        /// Carrega dados do Recredenciamento
        /// </summary>
        /// <param name="tipoPessoa">Tipo de pessoa</param>
        /// <param name="cpfCnpj">Valor CPF/CNPJ</param>
        /// <returns>retorna objeto com dados do Recredenciamento</returns>
        public static GEPontoVen.ListaCadastroRecredenciamento CarregarDadosRecredenciamento(Char tipoPessoa, Int64 cpfCnpj)
        {
            var dadosRecredenciamento = new GEPontoVen.ListaCadastroRecredenciamento();

            using (Logger log = Logger.IniciarLog("Carrega dados para recredenciamento"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    cpfCnpj,
                    tipoPessoa
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {
                    var dados = contexto.Cliente.ListaCadastroRecredenciamento(cpfCnpj, tipoPessoa);

                    if (dados.Count > 0)
                        dadosRecredenciamento = dados[0];
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    dadosRecredenciamento
                });
            }

            return dadosRecredenciamento;
        }

        /// <summary>
        /// Carrega dados do Domicilio Bancario baseado no Ponto de Venda
        /// </summary>
        /// <param name="numPontoVenda">Número do Ponto de Venda</param>
        /// <param name="codTipoOperacao">Código do tipo de operação</param>
        /// <param name="siglaProduto">Sigla do Produto</param>
        /// <returns>Retorna dados do Domicilio Bancario baseado no Ponto de Venda</returns>
        public static List<GEDomBancario.DomBancariosPorPVOperProd> CarregarDadosDomicilioBancarioPorPontoVenda(Int32 numPontoVenda, Char? codTipoOperacao, String siglaProduto)
        {
            var domiciliosBancarios = new List<GEDomBancario.DomBancariosPorPVOperProd>();

            using (Logger log = Logger.IniciarLog("Carrega dados domicílio bancário crédito do ponto de venda"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPontoVenda,
                    codTipoOperacao,
                    siglaProduto
                });

                using (var contexto = new ContextoWCF<GEDomBancario.ServicoPortalGEDomicilioBancarioClient>())
                {
                    domiciliosBancarios = contexto.Cliente.ConsultaDomBancariosPorPVOperProd(numPontoVenda, codTipoOperacao, siglaProduto);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    domiciliosBancarios
                });
            }

            return domiciliosBancarios;
        }

        /// <summary>
        /// Carrega lista de cadastros por Ponto de Venda
        /// </summary>
        /// <param name="numPdv">Número do PDV - Ponto de Venda </param>
        /// <returns>Retorna lista de cadastros por Ponto de Venda</returns>
        public static GEPontoVen.ListaCadastroPorPontoVenda CarregarDadosPorPontoVenda(Int32 numPdv)
        {
            var pontoDeVenda = new GEPontoVen.ListaCadastroPorPontoVenda();

            using (Logger log = Logger.IniciarLog("Carrega dados por ponto de venda"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    numPdv
                });

                using (var contexto = new ContextoWCF<GEPontoVen.ServicoPortalGEPontoVendaClient>())
                {
                    var pontosDeVenda = contexto.Cliente.ListaCadastroPorPontoVenda(numPdv);

                    if (pontosDeVenda.Count > 0)
                        pontoDeVenda = pontosDeVenda[0];
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    pontoDeVenda
                });
            }

            return pontoDeVenda;
        }

        /// <summary>
        /// Consulta taxa de Filiação
        /// </summary>
        /// <returns>Retorna objeto com valores sobre a Taxa de Filiação</returns>
        public static GETaxaFiliacao.TaxaFiliacaoConsultaValorTaxaFiliacao ConsultaTaxaFiliacao()
        {
            List<GETaxaFiliacao.TaxaFiliacaoConsultaValorTaxaFiliacao> taxaFiliacao;

            using (var log = Logger.IniciarLog("GETaxaFiliacao - ConsultaValorTaxaFiliacao"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                });

                using (var contexto = new ContextoWCF<GETaxaFiliacao.ServicoPortalGETaxaFiliacaoClient>())
                {
                    taxaFiliacao = contexto.Cliente.ConsultaValorTaxaFiliacao();
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    taxaFiliacao
                });
            }

            if (taxaFiliacao.Count > 0)
                return taxaFiliacao[0];

            return null;
        }

        /// <summary>
        /// Carrega Produtos VAN de um grupo Ramo Atuação e um Ramo Atividade
        /// </summary>
        /// <param name="codigoGrupoRamoAtuacao">Código do grupo de ramo Atuação</param>
        /// <param name="codigoRamoAtividade">Código do Ramo de Atividade</param>
        /// <returns>retorna lista de Produtos VAN de um grupo Ramo Atuação e um Ramo Atividade</returns>
        public static List<GEProdutos.ProdutosListaDadosProdutosVanPorRamo> CarregarProdutosVAN(Int32 codigoGrupoRamoAtuacao, Int32 codigoRamoAtividade)
        {
            List<GEProdutos.ProdutosListaDadosProdutosVanPorRamo> listaProdutosVan;

            using (var log = Logger.IniciarLog("GEProdutos - ListaDadosProdutosVanPorRamo"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoGrupoRamoAtuacao,
                    codigoRamoAtividade
                });

                using (var contexto = new ContextoWCF<GEProdutos.ServicoPortalGEProdutosClient>())
                {
                    listaProdutosVan = contexto.Cliente.ListaDadosProdutosVanPorRamo(codigoGrupoRamoAtuacao, codigoRamoAtividade);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaProdutosVan
                });
            }

            return listaProdutosVan;
        }

        /// <summary>
        /// Recupera Lista de serviços Disponíveis para pacote específico
        /// </summary>
        /// <param name="codigoPacote">Código do pacote</param>
        /// <returns>Retorna Lista de serviços Disponíveis para pacote específico</returns>
        public static List<Modelo.Servico> RecuperarServicosDisponiveisParaPacote(Int32 codigoPacote)
        {
            List<GEServicos.ServicosPorPacote> listaServicos = new List<GEServicos.ServicosPorPacote>();

            using (var log = Logger.IniciarLog("GEServicos - ListaServicosPorPacote"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoPacote
                });

                using (var contexto = new ContextoWCF<GEServicos.ServicoPortalGEServicosClient>())
                {
                    listaServicos = contexto.Cliente.ListaServicosPorPacote(codigoPacote);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    listaServicos
                });
            }

            return ConvertToListServicos(listaServicos);
        }

        /// <summary>
        /// Converte uma lista de Serviços GE para o Modelo de classe padrão do sistema
        /// </summary>
        /// <param name="listaServicos">Lista de objetos do tipo GEServicos.ServicosPorPacote</param>
        /// <returns>Retorna lista de Serviços</returns>
        private static List<Modelo.Servico> ConvertToListServicos(List<GEServicos.ServicosPorPacote> listaServicos)
        {
            List<Modelo.Servico> servicos = new List<Modelo.Servico>();

            foreach (GEServicos.ServicosPorPacote servico in listaServicos)
            {
                servicos.Add(new Modelo.Servico()
                {
                    CodigoServico = servico.CodServico,
                    DescricaoServico = servico.DescricaoServico
                });
            }

            return servicos;
        }

        /// <summary>
        /// Função que valida número de telefone
        /// </summary>
        /// <param name="telefone">string telefone</param>
        /// <returns>retorna boolean para telefone válido</returns>
        public static Boolean ValidaTelefone(String telefone)
        {
            if (telefone.Length < 8)
                return false;

            if (telefone.Equals("11111111") || telefone.Equals("111111111") ||
                telefone.Equals("22222222") || telefone.Equals("222222222") ||
                telefone.Equals("33333333") || telefone.Equals("333333333") ||
                telefone.Equals("44444444") || telefone.Equals("444444444") ||
                telefone.Equals("55555555") || telefone.Equals("555555555") ||
                telefone.Equals("66666666") || telefone.Equals("666666666") ||
                telefone.Equals("77777777") || telefone.Equals("777777777") ||
                telefone.Equals("88888888") || telefone.Equals("888888888") ||
                telefone.Equals("99999999") || telefone.Equals("999999999") ||
                telefone.Equals("00000000") || telefone.Equals("000000000") ||
                telefone.Equals("12345678") || telefone.Equals("123456789") ||
                telefone[0].Equals('0') || telefone[0].Equals('1'))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Função que valida se a data é válida
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Retorna boolean para Data válida</returns>
        public static Boolean ValidaData(String data)
        {
            if (String.IsNullOrEmpty(data))
                return false;

            DateTime dataConvertida = data.ToDate("dd/MM/yyyy");
            if (dataConvertida == DateTime.MinValue || dataConvertida > DateTime.Now)
                return false;

            return true;
        }

        /// <summary>
        /// Verifca se o voucher é válido para o grupo e ramos atividade
        /// </summary>
        /// <param name="grupoRamo"></param>
        /// <param name="ramoAtividade"></param>
        /// <param name="codCca"></param>
        /// <returns></returns>
        public static List<GEProdutoParceiro.GrupoRamoAtividadeProduto> ValidaRamoAtividadeParceiro(int grupoRamo, int ramoAtividade, int codCca)
        {
            using (var log = Logger.IniciarLog("Verifica se o voucher é válido para o grupo e ramos atividade"))
            {
                List<GEProdutoParceiro.GrupoRamoAtividadeProduto> retorno = new List<GEProdutoParceiro.GrupoRamoAtividadeProduto>();

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    grupoRamo,
                    ramoAtividade,
                    codCca
                });

                try
                {
                    using (var contexto = new ContextoWCF<GEProdutoParceiro.ServicoPortalGEProdutoParceiroClient>())
                    {
                        retorno = contexto.Cliente.ListarRamosAtividadeParceiro('B', grupoRamo, ramoAtividade, codCca, null);

                        return retorno.Where(p => p.IndcSituacaoRamoAtividadeParceiro).ToList();
                    }
                }
                catch (FaultException ex)
                {
                    log.GravarLog(EventoLog.RetornoServico);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarLog(EventoLog.RetornoServico);
                    throw ex;
                }
                finally
                {
                    log.GravarLog(EventoLog.RetornoServico, new
                    {
                        retorno
                    });
                }

            }

        }
    }
}
