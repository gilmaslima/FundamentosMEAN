/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/21 - Tiago Barbosa dos Santos - Versão Inicial
- 2012/08/29 - Guilherme Alves Brito / Lucas Nicoletto da Cunha - Anulação Cancelamento
*/
using System;
using System.Collections.Generic;

using Redecard.PN.Cancelamento.Modelo;
using Redecard.PN.Cancelamento.Negocio;
using Redecard.PN.Cancelamento.Servicos.Interfaces;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CancelamentoService" in code, svc and config file together.
    public class ServicoCancelamento : IServicoPortalCancelamento
    {

        public ModConsultaResult ConsultaPorPeriodo(string numPdv, string DataInicial, string DataFinal)
        {            
            using (Logger Log = Logger.IniciarLog("Consulta por período"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numPdv, DataInicial, DataFinal });
                var retorno = new NegociosPortalCancelamento().ConsultaPorPeriodo(numPdv, DataInicial, DataFinal);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public ModConsultaResult ConsultaPorAvisoCancelamento(int numPdv, decimal NumAvisoCancelamento)
        {
            using (Logger Log = Logger.IniciarLog("Consulta por aviso cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numPdv, NumAvisoCancelamento });
                var retorno = new NegociosPortalCancelamento().ConsultaPorNumeroCancelamento(numPdv, NumAvisoCancelamento);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public List<ModComprovante> ConsultaAnulacao(int codEstabelecimento)
        {
            using (Logger Log = Logger.IniciarLog("Consulta anulação"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento });
                var retorno = new NegociosPortalCancelamento().ConsultarAnulacaoCancelamentoVendas(codEstabelecimento);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public List<ModComprovante> ComprovanteCancelamento(int codEstabelecimento, long lonSession, string strServer, short CodCancelamento, string Continua, int NumAvisoCancel)
        {
            using (Logger Log = Logger.IniciarLog("Comprovante cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento, lonSession, strServer, CodCancelamento, Continua, NumAvisoCancel });
                var retorno = new NegociosPortalCancelamento().ConsultarCancelamentoVendas(codEstabelecimento, lonSession, strServer, CodCancelamento, Continua, NumAvisoCancel);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public List<ModAnularCancelamento> RealizarAnulacaoCancelamento(string usuario, string ipUsuario, List<ModComprovante> registrosDesfazer)
        {
            using (Logger Log = Logger.IniciarLog("Realizar anulação cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { usuario, ipUsuario, registrosDesfazer });
                var retorno = new NegociosPortalCancelamento().RealizarAnulacaoCancelamento(usuario, ipUsuario, registrosDesfazer);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public List<ItemCancelamentoSaida> Cancelamento(List<ItemCancelamentoEntrada> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { input });
                var retorno = new NegociosPortalCancelamento().Cancelamento(input);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public EstabelecimentoCancelamento RetornaDadosEstabelecimentoCancelamento(int codEstabelecimento)
        {
            using (Logger Log = Logger.IniciarLog("Retorna dados estabelecimento cancelamento"))
            {
                if (codEstabelecimento > 0)
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codEstabelecimento });
                    var retorno = new NegociosPortalCancelamento().ConsultarEstabelecimentoCancelamento(codEstabelecimento);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                else
                {
                    Log.GravarMensagem("Código de estabelecimento 0");
                    return null;
                }
            }
        }

        public List<ModConsultaDuplicado> ConsultaDuplicados(List<ItemCancelamentoEntrada> entrada)
        {
            using (Logger Log = Logger.IniciarLog("Consulta duplicados"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { entrada });
                var retorno = new NegociosPortalCancelamento().ConsultaDuplicados(entrada);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        public List<ItemCancelamentoSaida> CancelamentoDuplicadas(List<ModConsultaDuplicado> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento duplicadas"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { input });
                var retorno = new NegociosPortalCancelamento().CancelamentoDuplicadas(input);
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }
    }
}
