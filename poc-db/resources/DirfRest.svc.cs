using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;
using Redecard.PN.Extrato.Negocio;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Servicos.Negocio;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Dirf" in code, svc and config file together.
    public class DirfRest : Redecard.PN.Comum.ServicoBase, IDirfRest
    {
        #region WACA075 - Dirf.
        /// <summary>
        /// WACA075 - Dirf.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarExtratoDirfRetornoRest ConsultarExtratoDirf(ConsultarExtratoDirfEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Dirf [WACA075]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarExtratoDirfEnvioDTO envioDTO = TradutorConsultarExtratoDirf.TraduzirEnvioConsultarExtratoDirf(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO });
                    ConsultarExtratoDirfRetornoDTO dto = ConsultarExtratoDirfBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { dto, statusRetornoDTO });

                    StatusRetorno statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dto, statusRetorno });
                        return null;
                    }

                    ConsultarExtratoDirfRetorno result = TradutorConsultarExtratoDirf.TraduzirRetornoConsultarExtratoDirf(dto);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return new ConsultarExtratoDirfRetornoRest
                    {
                        Item = result,
                        StatusRetorno = (Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response.StatusRetorno)statusRetorno.CodigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
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

        #region WACA079 - Dirf.
        /// <summary>
        /// WACA079 - Dirf.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <returns></returns>
        public ConsultarAnosBaseDirfRetornoRest ConsultarAnosBaseDirf()
        {

            using (Logger Log = Logger.IniciarLog("Dirf [WACA079]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente);

                    ConsultarAnosBaseDirfRetornoDTO dto = ConsultarAnosBaseDirfBLL.Instance.Pesquisar(out statusRetornoDTO);

                    Log.GravarLog(EventoLog.RetornoAgente, new { dto, statusRetornoDTO });

                    StatusRetorno statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dto, statusRetorno });
                        return null;
                    }

                    ConsultarAnosBaseDirfRetorno resultAnos = TradutorConsultarAnosBaseDirf.TraduzirRetornoConsultarAnosBaseDirf(dto);

                    ConsultarAnosBaseDirfRetornoRest result = new ConsultarAnosBaseDirfRetornoRest
                    {
                        AnosBase = resultAnos != null ? resultAnos.AnosBase : null,
                        StatusRetorno = (Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response.StatusRetorno)statusRetorno.CodigoRetorno
                    };

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
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

        #region Resumo
        public ResumoRetornoListRest Resumo(ConsultarExtratoDirfResumoEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Dirf Resumo"))
            {

                try
                {
                    #region Anos
                    Log.GravarLog(EventoLog.InicioServico);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente);

                    ConsultarAnosBaseDirfRetornoDTO dtoAnos = ConsultarAnosBaseDirfBLL.Instance.Pesquisar(out statusRetornoDTO);

                    Log.GravarLog(EventoLog.RetornoAgente, new { dtoAnos, statusRetornoDTO });

                    StatusRetorno statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dtoAnos, statusRetorno });
                        return null;
                    }

                    ConsultarAnosBaseDirfRetorno resultAnos = TradutorConsultarAnosBaseDirf.TraduzirRetornoConsultarAnosBaseDirf(dtoAnos);


                    resultAnos.AnosBase = resultAnos.AnosBase != null
                                        ? resultAnos.AnosBase.OrderByDescending(o => o).ToList()
                                        : new List<Int16>();

                    resultAnos.AnosBase = resultAnos.AnosBase.Count > 3
                                        ? resultAnos.AnosBase.Take(3).ToList()
                                        : resultAnos.AnosBase;

                    #endregion

                    #region Extrato/Ano
                    IDictionary<Int16, ConsultarExtratoDirfRetorno> dictionaryResult = new Dictionary<Int16, ConsultarExtratoDirfRetorno>();

                    foreach (Int16 ano in resultAnos.AnosBase)
                    {
                        ConsultarExtratoDirfEnvioDTO envioDTO = TradutorConsultarExtratoDirf.TraduzirEnvioConsultarExtratoDirf(new ConsultarExtratoDirfEnvio
                        {
                            AnoBaseDirf = ano,
                            CnpjEstabelecimento = envio.CnpjEstabelecimento,
                            NumeroEstabelecimento = envio.NumeroEstabelecimento
                        });

                        Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO });

                        ConsultarExtratoDirfRetornoDTO dtoExtrato = ConsultarExtratoDirfBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);

                        Log.GravarLog(EventoLog.RetornoAgente, new { dtoExtrato, statusRetornoDTO });

                        statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                        if (statusRetorno.CodigoRetorno != 0)
                        {
                            Log.GravarLog(EventoLog.FimServico, new { dtoExtrato, statusRetorno });
                            return null;
                        }

                        dictionaryResult.Add(ano, TradutorConsultarExtratoDirf.TraduzirRetornoConsultarExtratoDirf(dtoExtrato));
                    }
                    #endregion

                    #region Retorno
                    ResumoRetornoListRest retorno = new ResumoRetornoListRest
                    {
                        Itens = resultAnos.AnosBase.Select(s => new ResumoRetorno
                        {
                            Ano = s,
                            Valor = OperacaoResumoDirf.ObterValorTotalConsolidadoAnoDirf(dictionaryResult[s])
                        }).ToArray(),
                        StatusRetorno = (Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response.StatusRetorno)statusRetorno.CodigoRetorno
                    };

                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    #endregion

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
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
