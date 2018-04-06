using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Rede.PN.Credenciamento.Sharepoint.Servicos
{
    public static class ServicosTG
    {

        #region [ AÇÃO COMERCIAL ]

        /// <summary>
        /// Consulta lista de Ação Comercial TG
        /// </summary>
        /// <param name="codigoCanal">Código da ação comercial</param>
        /// <param name="codigoCelula">Situação (status) da ação comercial ('A' = Ativo)</param>
        /// <returns>Retorna Lista do Tipo TGAcaoCom.ListaDadosCadastrais</returns>
        public static List<TGAcaoCom.ListaDadosCadastrais> ConsultaAcaoComercial(Int32? codAcao, Char statusAcao)
        {
            List<TGAcaoCom.ListaDadosCadastrais> lstAcaoComercial = new List<TGAcaoCom.ListaDadosCadastrais>();

            using (var log = Logger.IniciarLog("TGAcaoCom - ListaDadosCadastrais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codAcao,
                    statusAcao
                });

                using (var contexto = new ContextoWCF<TGAcaoCom.ServicoPortalTGAcaoComercialClient>())
                {
                    lstAcaoComercial = contexto.Cliente.ListaDadosCadastrais(codAcao, statusAcao);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstAcaoComercial
                });
            }

            return lstAcaoComercial;
        }

        #endregion

        #region [ CENARIO ]

        /// <summary>
        /// Consulta Cenários existentes para os parâmetros setados
        /// </summary>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoTipoEquipamento">Código do tipo de Equipamento</param>
        /// <param name="codigoSituacaoCenarioCanal">Código da situação do Cenário do Canal</param>
        /// <param name="codigoCampanha">Código da Campanha</param>
        /// <param name="codigoOrigemChamada">Código da Origem de Chamada</param>
        /// <returns>Retorna Lista de TGCenarios.Cenarios</returns>
        public static List<TGCenarios.Cenarios> ConsultaCenario(Int32  codigoCanal, String codigoTipoEquipamento, Char codigoSituacaoCenarioCanal, String codigoCampanha, String codigoOrigemChamada)
        {
            List<TGCenarios.Cenarios> lstAcaoComercial = new List<TGCenarios.Cenarios>();

            using (var log = Logger.IniciarLog("TGCenarios - ListaDadosCadastrais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCanal,
                    codigoTipoEquipamento,
                    codigoSituacaoCenarioCanal,
                    codigoCampanha,
                    codigoOrigemChamada
                });

                using (var contexto = new ContextoWCF<TGCenarios.ServicoPortalTGCenariosClient>())
                {
                    lstAcaoComercial = contexto.Cliente.ListaDadosCadastrais(null, codigoCanal, codigoTipoEquipamento, codigoSituacaoCenarioCanal, codigoCampanha, codigoOrigemChamada);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstAcaoComercial
                });
            }

            return lstAcaoComercial;
        }
        #endregion

        #region [ EVENTOS ESPECIAIS ]

        /// <summary>
        /// Consulta Eventos especiais com base nos parâmetros
        /// </summary>
        /// <param name="codEvEspecial">Codigo do Evento especial</param>
        /// <param name="statusEvento">status do Evento</param>
        /// <param name="codIndicadorAcaoFCT">Codigo do Indicador Ação FCT</param>
        /// <returns>Retorna Lista di tipo TGEventosEsp.EventosEspeciais</returns>
        public static List<TGEventosEsp.EventosEspeciais> ConsultaEventosEspeciais(string codEvEspecial, Char? statusEvento, Char? codIndicadorAcaoFCT)
        {
            List<TGEventosEsp.EventosEspeciais> lstEventosEspeciais = new List<TGEventosEsp.EventosEspeciais>();

            using (var log = Logger.IniciarLog("TGEventosEspeciais - EventosEspeciais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codEvEspecial,
                    statusEvento,
                    codIndicadorAcaoFCT
                });

                using (var contexto = new ContextoWCF<TGEventosEsp.ServicoPortalTGEventosEspeciaisClient>())
                {
                    lstEventosEspeciais = contexto.Cliente.ListaDadosCadastrais(codEvEspecial, statusEvento, codIndicadorAcaoFCT);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    lstEventosEspeciais
                });
            }

            return lstEventosEspeciais;
        }

        #endregion

        /// <summary>
        /// Busca informações detalhadas do cenário escolhido
        /// </summary>
        /// <param name="codCenario">código do cenário</param>
        /// <param name="codCanal">código do canal</param>
        /// <param name="codTipoEquipamento">código do tipo de equipamento</param>
        /// <param name="codSituacaoCenarioCanal">código da Situação do Cenário por Canal</param>
        /// <param name="codCampanha">código da campanha</param>
        /// <param name="codOrigemChamada">código da Origem da Chamada</param>
        /// <returns>retorna dados do cenário</returns>
        public static TGCenarios.Cenarios BuscarDadosCenario(Int32 codCenario, Int32 codCanal, String codTipoEquipamento, Char codSituacaoCenarioCanal, String codCampanha, String codOrigemChamada)
        {
            TGCenarios.Cenarios retorno = new TGCenarios.Cenarios();

            using (var log = Logger.IniciarLog("Busca dados do cenário"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codCenario,
                    codCanal,
                    codTipoEquipamento,
                    codSituacaoCenarioCanal,
                    codCampanha,
                    codOrigemChamada
                });

                using (var contexto = new ContextoWCF<TGCenarios.ServicoPortalTGCenariosClient>())
                {
                    var cenarios = contexto.Cliente.ListaDadosCadastrais(codCenario, codCanal, codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada);

                    if (cenarios.Count == 1)
                            retorno = cenarios[0];
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        #region [Valor Mensal Aluguel]

        /// <summary>
        /// Consulta Valor mensal do aluguel 
        /// </summary>
        /// <param name="codTipoEquipamento">Código do Tipo de Equipamento</param>
        /// <param name="situacao">Situação</param>
        /// <param name="indicadorEquipamentoCompartilhado">Indicador de Equipamento Compartilhado</param>
        /// <returns>Retorna Valor Mensal do Aluguel</returns>
        public static Double? ConsultaValorMensalAluguel(String codTipoEquipamento, Char situacao, Char indicadorEquipamentoCompartilhado)
        {
            List<TGTipoEquip.TipoEquipamento> listaTipoEquipamento;
            using (var log = Logger.IniciarLog("TGTipoEquip - ListaDadosCadastrais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codTipoEquipamento,
                    situacao,
                    indicadorEquipamentoCompartilhado
                });

                using (var contexto = new ContextoWCF<TGTipoEquip.ServicoPortalTGTipoEquipamentoClient>())
                {
                    listaTipoEquipamento = contexto.Cliente.ListaDadosCadastrais(codTipoEquipamento, situacao, indicadorEquipamentoCompartilhado);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    listaTipoEquipamento
                });
            }

            if(listaTipoEquipamento.Count >0)
                return listaTipoEquipamento[0].ValorDefaultAluguel;
                
            return 0;
        }

        #endregion


        #region [TEF]

        /// <summary>
        /// Consulta Softwares do TEF
        /// </summary>
        /// <param name="codigoFornecedorSotware">Código do fornecedor de Software</param>
        /// <param name="situacao">Situação</param>
        /// <returns>Retorna lista de TGForSoftware.FornecedorSoftware</returns>
        public static List<TGForSoftware.FornecedorSoftware> ConsultaSoftwaresTEF(String codigoFornecedorSotware, Char situacao)
        {
            List<Rede.PN.Credenciamento.Sharepoint.TGForSoftware.FornecedorSoftware> fornecedorSoftware;

            using (var log = Logger.IniciarLog("TGForSoftware - ListaDadosCadastrais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoFornecedorSotware,
                    situacao
                });

                using (var contexto = new ContextoWCF<TGForSoftware.ServicoPortalTGFornecedorSoftwareClient>())
                {
                    fornecedorSoftware = contexto.Cliente.ListaDadosCadastrais(codigoFornecedorSotware, situacao);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    fornecedorSoftware
                });
            }

            return fornecedorSoftware;
        }

        #endregion

        #region [ Marca PDV ]

        /// <summary>
        /// Lista fabricante Hardware
        /// </summary>
        /// <param name="codigoFabricanteHardware">Código do Fabricante de Hardware</param>
        /// <param name="situacao">Situação</param>
        /// <returns>Retorna Vetor do tipo TGFabHardware.FabricanteHardware[]</returns>
        public static TGFabHardware.FabricanteHardware[] ConsultaFabricanteHardware(String codigoFabricanteHardware, Char situacao)
        {
            TGFabHardware.FabricanteHardware[] fabricanteHardware;

            using (var log = Logger.IniciarLog("TGFabHardware - ServicoPortalTGFabricanteHardwareClient"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoFabricanteHardware,
                    situacao
                });

                using (var contexto = new ContextoWCF<TGFabHardware.ServicoPortalTGFabricanteHardwareClient>())
                {
                    fabricanteHardware = contexto.Cliente.ListaDadosCadastrais(codigoFabricanteHardware, situacao);
                }

                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    fabricanteHardware
                });
            }

            return fabricanteHardware;
        }
        #endregion

    }
}
