using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Negocio;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Home" in code, svc and config file together.
    public class Home : Redecard.PN.Comum.ServicoBase, IHome
    {
        #region WACA1107 - Home - Últimas Vendas.
        /// <summary>
        /// WACA1107 - Home - Últimas Vendas.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarTransacoesCreditoDebitoRetorno> ConsultarTransacoesCreditoDebito(out StatusRetorno statusRetorno, ConsultarTransacoesCreditoDebitoEnvio envio)
        {
            using (Logger Log = Logger.IniciarLog("Home - Últimas Vendas [WACA1107]"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { envio });

                try
                {                                     
                    //Objeto de retorno
                    List<ConsultarTransacoesCreditoDebitoRetorno> result = null;

                    //Gera chave de últimas vendas para os PVs selecionados
                    String chaveCache = String.Format("UV_{0}_{1}",
                        DateTime.Today.ToString("ddMMyy"),
                        String.Join<Int32>(";", envio.Estabelecimentos));
#if !DEBUG                    
                    //Busca objeto no cache
                    result = CacheAdmin.Recuperar<List<ConsultarTransacoesCreditoDebitoRetorno>>(Cache.Home, chaveCache);
#endif

                    //Se não existir, executa consulta ao mainframe
                    if (result == null)
                    {
                        ConsultarTransacoesCreditoDebitoEnvioDTO envioDTO = TradutorConsultarTransacoesCreditoDebito.TraduzirEnvioConsultarTransacoesCreditoDebito(envio);

                        StatusRetornoDTO statusRetornoDTO;

                        Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO });
                        List<ConsultarTransacoesCreditoDebitoRetornoDTO> registros = ConsultarTransacoesCreditoDebitoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                        Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                        statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                        if (statusRetorno.CodigoRetorno != 0)
                        {
                            Log.GravarLog(EventoLog.FimServico, new { registros, statusRetornoDTO });
                            return null;
                        }

                        //Executa consulta no mainframe
                        result = TradutorConsultarTransacoesCreditoDebito.TraduzirRetornoListaConsultarTransacoesCreditoDebito(registros);

#if !DEBUG                        
                        //Inclui objeto no cache
                        CacheAdmin.Adicionar(Cache.Home, chaveCache, result);
#endif
                    }
                    else
                    {
                        //Gera status "fake": se objeto está em cache, a consulta foi executada anteriormente com sucesso
                        statusRetorno = new StatusRetorno(0, String.Empty, FONTE);

                        Log.GravarMensagem("Dados obtidos do cache", new { chaveCache });
                    }
                                        
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (Exception ex)
                {                    
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        #region WACA1108 - Home - Lançamentos futuros.
        /// <summary>
        /// WACA1108 - Home - Lançamentos futuros.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarCreditoDebitoRetorno> ConsultarCreditoDebito(out StatusRetorno statusRetorno, ConsultarCreditoDebitoEnvio envio)
        {
            using (Logger Log = Logger.IniciarLog("Home - Lançamentos Futuros [WACA1108]"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { envio });

                try
                {   
                    //Objeto de retorno                 
                    List<ConsultarCreditoDebitoRetorno> result = null;

                    //Gera chave de lançamentos futuros para os PVs selecionados
                    String chaveCache = String.Format(
                        "LF_{0}_{1}",
                        DateTime.Today.ToString("ddMMyy"),
                        String.Join<Int32>(";", envio.Estabelecimentos));

#if !DEBUG
                    //Busca objeto no cache
                    result = CacheAdmin.Recuperar<List<ConsultarCreditoDebitoRetorno>>(Cache.Home, chaveCache);
#endif

                    //Se não existir, executa consulta ao mainframe
                    if (result == null)
                    {
                        ConsultarCreditoDebitoEnvioDTO envioDTO = TradutorConsultarCreditoDebito.TraduzirEnvioConsultarCreditoDebito(envio);

                        StatusRetornoDTO statusRetornoDTO;
                        Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO });
                        List<ConsultarCreditoDebitoRetornoDTO> registros = ConsultarCreditoDebitoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                        Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                        statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                        if (statusRetorno.CodigoRetorno != 0)
                        {
                            Log.GravarLog(EventoLog.FimServico, new { registros, statusRetornoDTO });
                            return null;
                        }

                        //Executa consulta no mainframe
                        result = TradutorConsultarCreditoDebito.TraduzirRetornoListaConsultarCreditoDebito(registros);

#if !DEBUG
                        //Inclui objeto no cache
                        CacheAdmin.Adicionar(Cache.Home, chaveCache, result);
#endif
                    }
                    else
                    {
                        //Gera status "fake": se objeto está em cache, a consulta foi executada anteriormente com sucesso
                        statusRetorno = new StatusRetorno(0, String.Empty, FONTE);

                        Log.GravarMensagem("Dados obtidos do cache", new { chaveCache });
                    }
                    
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (Exception ex)
                {                    
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion
    }
}
