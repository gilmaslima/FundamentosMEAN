/*
(c) Copyright [2012] Redecard S.A.
Autor : [Guilherme Alves Brito]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/15 - Guilherme Alves Brito - Versão Inicial
- 2012/08/21 - Tiago Barbosa dos Santos - Consultas do Histórico de cancelamento
- 2012/08/22 - Lucas Nicoletto da Cunha - Cancelamento de Venda
- 2012/08/23 - Guilherme Alves Brito - Comprovante Cancelamento Vendas
- 2012/08/23 - Guilherme Alves Brito - Comprovante Anulacao Cancelamento Vendas
- 2012/08/29 - Guilherme Alves Brito / Lucas Nicoletto da Cunha - Anulação Cancelamento
 * 
*/


using System;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Agentes;
using Redecard.PN.Cancelamento.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Negocio
{
    public class NegociosPortalCancelamento : RegraDeNegocioBase
    {

        #region Consultas cancelamento
        public ModConsultaResult ConsultaPorPeriodo(string numPdv, string dataInicial, string dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Consulta por período"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numPdv, dataInicial, dataFinal });
                var retorno = AgentesPortalCancelamento.GetInstance().ConsultaPorPeriodo(numPdv, dataInicial, dataFinal, 0, string.Empty, "N");
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }

        public ModConsultaResult ConsultaPorNumeroCancelamento(int numPdv, Decimal numAvisoCan)
        {
            using (Logger Log = Logger.IniciarLog("Consulta por número cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numPdv, numAvisoCan });

                String numAvisoCanAUX = numAvisoCan.ToString();
                if (numAvisoCan == 0)
                {
                    Logger.GravarErro("Número de Aviso de Cancelamento Obrigatório.");
                    throw new PortalRedecardException(0, "Número de Aviso de Cancelamento Obrigatório.");
                }

                var retorno = AgentesPortalCancelamento.GetInstance().ConsultaPorAvisoCancelamento(numPdv, numAvisoCanAUX, string.Empty, "N");

                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }
        #endregion

        #region Cancelamento de Vendas

        public List<ItemCancelamentoSaida> Cancelamento(List<ItemCancelamentoEntrada> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { input });
                var retorno = AgentesPortalCancelamento.GetInstance().Cancelamento(input);
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }

        #endregion

        #region Comprovante Cancelamento Vendas / Comprovante Anulação Cancelamento
        public List<ModComprovante> ConsultarCancelamentoVendas(int codEstabelecimento, long lonSession, string strServer, short CodCancelamento, string Continua, int NumAvisoCancel)
        {
            using (Logger Log = Logger.IniciarLog("Consultar cancelamento vendas"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento, lonSession, strServer, CodCancelamento, Continua, NumAvisoCancel });
                if (codEstabelecimento < 0)
                {
                    Logger.GravarErro("Parâmetro código do estabelecimento não informado.");
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Parâmetro código do estabelecimento não informado."));
                }

                if (strServer == "0")
                {  // Verificar se a regra é essa mesmo...
                    Logger.GravarErro("Parametros do servidor não informados.");
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Parametros do servidor não informados."));
                }
                Log.GravarLog(EventoLog.FimNegocio);
                return null;
            }
        }

        public List<ModComprovante> ConsultarAnulacaoCancelamentoVendas(int codEstabelecimento)
        {
            using (Logger Log = Logger.IniciarLog("Consultar anulação cancelamento vendas"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { codEstabelecimento });
                var retorno = AgentesPortalCancelamento.GetInstance().ConsultaDia(codEstabelecimento, string.Empty, "S");
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }
        #endregion

        #region Anulação de Cancelamentos

        public List<ModAnularCancelamento> RealizarAnulacaoCancelamento(string usuario, string ipUsuario, List<ModComprovante> registrosDesfazer)
        {
            using (Logger Log = Logger.IniciarLog("Realizar anulação cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { usuario, ipUsuario, registrosDesfazer });
                var retorno = AgentesPortalCancelamento.GetInstance().RealizarAnulacaoCancelamento(usuario, ipUsuario, registrosDesfazer);
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }

        #endregion

        #region Dados Estabelecimento Cancelamento
        /// <summary>
        /// Consultar uma entidade pelo código
        /// </summary>
        /// <param name="codigoEstabelecimento">Código da Entidade</param>
        /// <returns>Objeto ModEntidade preenchido</returns>
        public Modelo.EstabelecimentoCancelamento ConsultarEstabelecimentoCancelamento(Int32 codigoEstabelecimento)
        {
            using (Logger Log = Logger.IniciarLog("Consultar uma entidade pelo código"))
            {
                //try
                //{
                //    // Instancia classe de dados
                //    var dadosEntidade = new Dados.EstabelecimentoCancelamento();
                //    int codigoRetornoGE = 0;
                //    // Retorna objeto ModEntidade preenchido
                //    Modelo.EstabelecimentoCancelamento estabelecimento = dadosEntidade.Consultar(codigoEstabelecimento, out codigoRetornoGE);
                //    estabelecimento.CodErro = codigoRetornoGE;

                //    return estabelecimento;
                //}
                //catch (PortalRedecardException)
                //{
                //    return null;
                //}
                //catch (Exception ex)
                //{
                //    return null;
                //}

                // Instancia classe de dados
                var dadosEntidade = new Dados.EstabelecimentoCancelamento();
                int codigoRetornoGE = 0;
                // Retorna objeto ModEntidade preenchido

                Log.GravarLog(EventoLog.InicioNegocio, new { codigoEstabelecimento });
                Modelo.EstabelecimentoCancelamento estabelecimento = dadosEntidade.Consultar(codigoEstabelecimento, out codigoRetornoGE);
                Log.GravarLog(EventoLog.FimNegocio, new { estabelecimento, codigoRetornoGE });
                estabelecimento.CodErro = codigoRetornoGE;

                return estabelecimento;
            }
        }
        #endregion

        #region Cancelamento Duplicados
        public List<ModConsultaDuplicado> ConsultaDuplicados(List<ItemCancelamentoEntrada> entrada)
        {
            using (Logger Log = Logger.IniciarLog("Consulta duplicados"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { entrada });
                var retorno = AgentesPortalCancelamento.GetInstance().ConsultaDuplicados(entrada);
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }

        public List<ItemCancelamentoSaida> CancelamentoDuplicadas(List<ModConsultaDuplicado> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento duplicadas"))
            {                
                Log.GravarLog(EventoLog.InicioNegocio, new { input });
                var retorno = AgentesPortalCancelamento.GetInstance().CancelamentoDuplicadas(input);
                Log.GravarLog(EventoLog.FimNegocio, new { retorno });
                return retorno;
            }
        }
        #endregion
    }
}
