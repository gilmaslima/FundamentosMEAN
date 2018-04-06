using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using AutoMapper;
using Redecard.PN.DataCash.Modelos;

namespace Redecard.PN.DataCash.Agentes
{
    /// <summary>
    /// Classe Agente para as funcionalidades do sub-módulo Configurações do DataCash.
    /// </summary>
    public class Configuracao : AgentesBase
    {
        #region [ Gerenciamento IP & URLBack ]

        /// <summary>
        /// Consulta dados de IP e URLBack de um estabelecimento.
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <returns>Dados de IP e URLBack do estabelecimento</returns>
        public GerenciamentoPV ConsultaGerencimentoPV(Int32 pv)
        {
            GerenciamentoPV gerenciamentoPV = null;

            try
            {
                //Instanciação e chamada de serviço externo
                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                    gerenciamentoPV = contexto.Cliente.ConsultaGerencimentoPV(pv);

                return gerenciamentoPV;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Atualiza os dados de configuração de IP e URLBack de um estabelecimento.
        /// </summary>
        /// <param name="mensagemErro">Mensagem de erro e código de retorno</param>
        /// <param name="gerenciamentoPV">Dados de configuração de IP e URLBack do estabelecimento</param>
        /// <returns>Boolean indicando sucesso ou erro da atualização dos dados do PV.</returns>
        public Boolean ManutencaoGerenciamentoPV(out MensagemErro mensagemErro, GerenciamentoPV gerenciamentoPV)
        {
            Boolean retorno = false;

            try
            {
                //Instanciação e chamada de serviço externo
                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                    retorno = contexto.Cliente.ManutencaoGerenciamentoPV(out mensagemErro, gerenciamentoPV);

                return retorno;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        #endregion [ FIM: Gerenciamento IP & URLBack ]

        #region [ Configuração AVS ]

        public List<Modelo.Configuracao.GrupoAVS> GetGruposAvs(Int32 pv)
        {
            //TODO ASH - Agentes: Grupo AVS - método foi removido do serviço externo
            throw new NotImplementedException();
            //var dataCashConsultaClient = new DataCashConsultaService.ConsultaTransactionClient();            
            //List<DataCashConsultaService.GrupoAVS> lstConsulta = dataCashConsultaClient.GetGrupoAvs(pv);
            //return Converter(lstConsulta);            
        }

        public List<Modelo.Configuracao.GrupoAVS> GerenciamentoAVS(Int32 pv, List<Modelo.Configuracao.GrupoAVS> lstGrupos)
        {
            try
            {
                //TODO ASH - Agentes: Grupo AVS - método foi removido do serviço externo
                throw new NotImplementedException();
                //using (DataCashConsultaService.ConsultaTransactionClient consultaTransactionClient = new Service.ConsultaTransactionClient())
                //{
                //    List<DataCashConsultaService.GrupoAVS> lstParams = new List<Service.GrupoAVS>();

                //    lstGrupos.ForEach(item => lstParams.Add(new Service.GrupoAVS() {
                //        Grupo = (DataCashConsultaService.ETipoAVS)item.IDGrupo,
                //        NotMatch = item.NotMatch                    
                //    }));

                //    DataCashConsultaService.ParametroGerencimentoAVS parametros = new Service.ParametroGerencimentoAVS();
                //    parametros.Grupos = lstParams;
                //    parametros.PV = pv;
                //    bool ret = consultaTransactionClient.GerenciamentoAVS(parametros);

                //    return ret ? lstGrupos : null;
                //}
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

        }

        public String ConsultarRegraAVS(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            String retorno = String.Empty;

            using (var log = Logger.IniciarLog("Consulta Regra AVS - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { pv });

                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                {
                    retorno = contexto.Cliente.ConsultaRegraAVS(out mensagemErro, pv);
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Boolean GerenciaRegraAVS(Int32 pv, Char avs, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Gerencia Regra AVS - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { pv, avs });

                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                {
                    retorno = contexto.Cliente.GerenciaRegraAVS(out mensagemErro, pv, avs);
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno, mensagemErro });
            }

            return retorno;
        }

        #endregion

        #region [ Configurações Boleto ]

        public Boleto ConsultaBoleto(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Boleto retorno = new Boleto();

            using (var log = Logger.IniciarLog("Consulta Boleto - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { pv });

                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                {
                    retorno = contexto.Cliente.ConsultarBoleto(out mensagemErro, pv);
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Boolean GerenciaBoleto(Int32 pv, Boleto boleto, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Gerencia Boleto - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { pv, boleto });

                using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                {
                    retorno = contexto.Cliente.GerenciaBoleto(out mensagemErro, pv, boleto);
                }

                log.GravarLog(EventoLog.FimAgente, new { retorno, mensagemErro });
            }

            return retorno;
        }

        #endregion

        #region [ Configurações Bandeiras Adicionais ]

        public RetornoBandeirasAdicionais ConsultaBandeirasAdicionais(Int32 pv, out MensagemErro mensagemErro)
        {
            var retorno = new RetornoBandeirasAdicionais();

            using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
            {
                retorno = contexto.Cliente.ConsultaBandeirasAdicionais(out mensagemErro, pv);
            }

            return retorno;
        }

        public Boolean GravarAtualizarBandeirasAdicionais(Int32 pv, out MensagemErro mensagemErro, String numeroAfiliacaoPdv, String chaveConfiguracaoPdv)
        {
            Boolean retorno = false;

            using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
            {
                retorno = contexto.Cliente.GerenciaBandeirasAdicionais(out mensagemErro, pv, numeroAfiliacaoPdv, chaveConfiguracaoPdv);
            }

            return retorno;
        }

        public RetornoServicoPV ListaServicoPV(Int32 pv)
        {
            RetornoServicoPV retorno;

            using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
            {
                retorno = contexto.Cliente.ListaServicoPV(pv);
            }

            return retorno;
        }

        /// <summary>
        /// Lista as bandeiras disponíveis
        /// </summary>
        /// <returns>Dicionário contendo ID e descrição das bandeiras</returns>
        public Dictionary<Int32, String> ObtemBandeirasFiltro()
        {
            var retorno = new ObtemBandeirasFiltroResponse().ObtemBandeirasFiltroResult;

            using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
            {
                retorno = contexto.Cliente.ObtemBandeirasFiltro();
            }

            return retorno;
        }

        #endregion
    }
}
