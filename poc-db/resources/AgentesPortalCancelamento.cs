/*
(c) Copyright [2012] Redecard S.A.
Autor : [Guilherme Alves Brito]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/08/15 - Guilherme Alves Brito - Versão Inicial
- 2012/08/21 - Tiago Barbosa dos Santos - Consulta de cancelamento
- 2012/08/29 - Guilherme Alves Brito / Lucas Nicoletto da Cunha - Anulação Cancelamento
- 2012/09/12 - Lucas Nicoletto da Cunha - Cancelamento 
*/

using System;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Agentes.ModuloCancelamento;
using Redecard.PN.Cancelamento.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Agentes
{
    public class AgentesPortalCancelamento
    {
        #region SINGLETON
        private static AgentesPortalCancelamento agentesCancelamento = null;

        public const int CODIGO_ERRO = 400;
        public const string FONTE = "Redecard.PN.Agentes";

        public AgentesPortalCancelamento()
        { }

        /// <summary>
        /// Retorna a instância do objeto.
        /// </summary>
        /// <returns></returns>
        public static AgentesPortalCancelamento GetInstance()
        {
            if (agentesCancelamento == null)
            { agentesCancelamento = new AgentesPortalCancelamento(); }
            return agentesCancelamento;
        }
        #endregion

        #region MEC128CO Consulta de Cancelamento

        /// <summary>
        /// Consulta por Periodo
        /// </summary>
        /// <param name="numEst">Número do estabelecimento</param>
        /// <param name="inicial">Data inicial</param>
        /// <param name="final">data Final </param>
        /// <returns>Historico</returns>
        public ModConsultaResult ConsultaPorPeriodo(string numPDV, string inicial, string final, Decimal avsIdeRechama, String numideRechama, String indRechama)
        {
            using (Logger Log = Logger.IniciarLog("Consulta por período"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numPDV, inicial, final });

                ModConsultaResult result = new ModConsultaResult();

                string NumEst = numPDV.PadLeft(9, '0');

                //Variaveis de entrada
                string NomePrograma = "MEC128  ";
                string CodigoRetorno = "0";
                string CodigoErro = "0";
                string DescricaoErro = "0";
                int QtdOcorrencia = 0;
                MEC128_DETALHE[] det = new MEC128_DETALHE[] { };
                string MEC127_SAI_FILLER = "";
                string MEC128_IND_CONT = "";
                Decimal MEC128_NU_AVS_CONT = avsIdeRechama;
                string MEC128_NU_IDE_CHE_CONT = numideRechama;
                string MEC128_ENTRA_FILLER = indRechama;
                string input = "";
                string FILLER = "";

                string DataInicial = inicial.Split('/')[2] + inicial.Split('/')[1] + inicial.Split('/')[0];
                string DataFinal = final.Split('/')[2] + final.Split('/')[1] + final.Split('/')[0]; ;

                //Chamada do Mainframe
                using (CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { NomePrograma, NumEst, DataInicial, DataFinal, MEC128_IND_CONT, MEC128_NU_AVS_CONT, MEC128_NU_IDE_CHE_CONT, FILLER });
                        client.MEC128CO(NomePrograma, NumEst, DataInicial, DataFinal, ref MEC128_IND_CONT, ref MEC128_NU_AVS_CONT, ref MEC128_NU_IDE_CHE_CONT,
                                         out NomePrograma, out CodigoRetorno, out CodigoErro, out DescricaoErro, out QtdOcorrencia, out det, out MEC127_SAI_FILLER, FILLER);
                        Log.GravarLog(EventoLog.RetornoHIS, new { MEC128_IND_CONT, MEC128_NU_AVS_CONT, MEC128_NU_IDE_CHE_CONT, NomePrograma, CodigoRetorno, CodigoErro, DescricaoErro, QtdOcorrencia, det, MEC127_SAI_FILLER });
                    }
                    catch (PortalRedecardException rex)
                    {
                        Log.GravarErro(rex);
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, rex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw ex;
                    }
                    finally
                    {
                        client.Close();
                    }
                }

                int CodErro = 0;
                if (!int.TryParse(CodigoRetorno, out CodErro))
                {
                    Logger.GravarErro("Parse error");
                    throw new PortalRedecardException(300, "Parse Error");
                }

                ///Lançamento de Exceções
                if (CodErro != 0)
                {

                    result.CodErro = CodErro;
                    result.DescErro = DescricaoErro;
                    Log.GravarLog(EventoLog.RetornoAgente, new { result });
                    return result;
                }

                //Convertendo os Detalhes em Classe
                List<ModCancelamentoConsulta> lstModConsulta = new List<ModCancelamentoConsulta>();

                int index = 0;
                if (QtdOcorrencia > 0)
                {

                    foreach (MEC128_DETALHE d in det)
                    {
                        ModCancelamentoConsulta m = new ModCancelamentoConsulta();
                        m.CodigoCancelamento = Convert.ToInt16(d.MEC128_CD_CANC);
                        m.DataCancelamento = d.MEC128_DT_CANC;
                        m.NumCartao = d.MEC128_NU_CARTAO;
                        m.NumeroAvisoCancel = d.MEC128_NU_AVS_CNO;
                        m.NumeroNSU = d.MEC128_NU_NSU;
                        m.TipoCancelamento = d.MEC128_TP_CANC;
                        m.NumeroPV = d.MEC128_NU_PV;

                        m.TipoTransacao = d.MEC128_TP_TRANS;
                        if (m.TipoCancelamento == "T")
                            m.DescTipoCancelamento = "Total";
                        else if (m.TipoCancelamento == "P")
                            m.DescTipoCancelamento = "Parcial";

                        if (m.TipoTransacao == 1)
                            m.DescTipoTransacao = "Rotativo";
                        else if (m.TipoTransacao == 2)
                            m.DescTipoTransacao = "Parcelado sem juros";
                        else if (m.TipoTransacao == 3)
                            m.DescTipoTransacao = "Parcelado com juros";

                        m.ValorCancelado = d.MEC128_VL_CANC;
                        m.ValorTransacao = d.MEC128_VL_TRANS;
                        m.CodigoCancelamento = d.MEC128_CD_CANC;

                        if (m.CodigoCancelamento == 0)
                            m.DescCodigoCancelamento = "Cancelamento ME";
                        else if (m.CodigoCancelamento == 1)
                            m.DescCodigoCancelamento = "Cancel. Chargeback (XA)";
                        else if (m.CodigoCancelamento == 2)
                            m.DescCodigoCancelamento = "Cancel. Eletrônico (OL)";
                        else if (m.CodigoCancelamento == 3)
                            m.DescCodigoCancelamento = "Cancel. Internet (IS)";

                        lstModConsulta.Add(m);

                        index++;
                        if (index == QtdOcorrencia) break;
                    }
                }

                result.ListaRetorno = lstModConsulta;

                if (MEC128_IND_CONT.CompareTo("S") == 0)
                {
                    ModConsultaResult res = ConsultaPorPeriodo(numPDV, inicial, final, MEC128_NU_AVS_CONT, MEC128_NU_IDE_CHE_CONT, MEC128_IND_CONT);
                    result.ListaRetorno.AddRange(res.ListaRetorno);
                }

                Log.GravarLog(EventoLog.RetornoAgente, new { result });

                return result;
            }
        }

        /// <summary>
        /// Consulta por numero de Aviso de Cancelamento MEC127CO
        /// </summary>
        /// <param name="numPV">Número do ponto de Venda</param>
        /// <param name="numAvsCan">Número do Aviso de Cancelkamento</param>
        /// <returns>Historico</returns>
        public ModConsultaResult ConsultaPorAvisoCancelamento(int numPV, string numAvsCan, String numideRechama, String indRechama)
        {
            using (Logger Log = Logger.IniciarLog("Consulta por número de aviso de cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numPV, numAvsCan });

                String NumEst = numPV.ToString().PadLeft(9, '0');

                ModConsultaResult result = new ModConsultaResult();

                String NomePrograma = "MEC127  ";
                String CodigoRetorno = "";
                int CodigoErro = 0;
                string codErr = "";
                String DescricaoErro = "";
                int QtdOcorrencia = 0;
                MEC127_DETALHE[] det = new MEC127_DETALHE[] { new MEC127_DETALHE() };
                string MEC127_SAI_FILLER = "";
                string MEC127_IND_CONT = indRechama;
                string MEC127_NU_IDE_CHE_CONT = numideRechama;
                string MEC128_ENTRA_FILLER = "";
                string FILLER = "";

                using (CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        Decimal decNumAvsCan = decimal.Parse(!string.IsNullOrEmpty(numAvsCan) ? numAvsCan : "0");
                        Log.GravarLog(EventoLog.ChamadaHIS, new { NomePrograma, NumEst, decNumAvsCan, MEC127_IND_CONT, MEC127_NU_IDE_CHE_CONT, FILLER });
                        client.MEC127CO(out CodigoRetorno, out codErr, out DescricaoErro, out QtdOcorrencia, out det, out MEC127_SAI_FILLER,
                            NomePrograma, NumEst, decNumAvsCan, MEC127_IND_CONT, MEC127_NU_IDE_CHE_CONT, FILLER);
                        Log.GravarLog(EventoLog.RetornoHIS, new { CodigoRetorno, codErr, DescricaoErro, QtdOcorrencia, det, MEC127_SAI_FILLER });
                    }
                    catch (PortalRedecardException rex)
                    {
                        Log.GravarErro(rex);
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, rex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw ex;
                    }
                    finally
                    {
                        client.Close();
                    }
                }

                if (!int.TryParse(CodigoRetorno, out CodigoErro))
                {
                    Logger.GravarErro("Parse error");
                    throw new PortalRedecardException(300, "Parse Error");
                }

                ///Lançamento de Exceções
                if (CodigoErro != 0)
                {

                    result.CodErro = CodigoErro;
                    result.DescErro = DescricaoErro;
                    Log.GravarLog(EventoLog.RetornoAgente, new { result });
                    return result;
                }

                List<ModCancelamentoConsulta> lstModConsulta = new List<ModCancelamentoConsulta>();

                int index = 0;
                if (QtdOcorrencia > 0)
                {
                    foreach (MEC127_DETALHE d in det)
                    {
                        ModCancelamentoConsulta m = new ModCancelamentoConsulta();
                        m.CodigoCancelamento = d.MEC127_CD_CANC;
                        m.DataCancelamento = d.MEC127_DT_CANC;
                        m.NumCartao = d.MEC127_NU_CARTAO;
                        m.NumeroPV = d.MEC127_NU_PV;
                        m.NumeroNSU = d.MEC127_NU_NSU;
                        m.TipoCancelamento = d.MEC127_TP_CANC;
                        m.NumeroAvisoCancel = d.MEC127_NU_IDE_CARTA;

                        if (m.TipoCancelamento == "T")
                            m.DescTipoCancelamento = "Total";
                        else if (m.TipoCancelamento == "P")
                            m.DescTipoCancelamento = "Parcial";


                        if (m.CodigoCancelamento == 0)
                            m.DescCodigoCancelamento = "Cancelamento ME";
                        else if (m.CodigoCancelamento == 1)
                            m.DescCodigoCancelamento = "Cancel. Chargeback (XA)";
                        else if (m.CodigoCancelamento == 2)
                            m.DescCodigoCancelamento = "Cancel. Eletrônico (OL)";
                        else if (m.CodigoCancelamento == 3)
                            m.DescCodigoCancelamento = "Cancel. Internet (IS)";

                        m.TipoTransacao = d.MEC127_TP_TRANS;

                        if (m.TipoTransacao == 1)
                            m.DescTipoTransacao = "Rotativo";
                        else if (m.TipoTransacao == 2)
                            m.DescTipoTransacao = "Parcelado sem juros";
                        else if (m.TipoTransacao == 3)
                            m.DescTipoTransacao = "Parcelado com juros";

                    m.ValorCancelado = d.MEC127_VL_CANC;
                    m.ValorTransacao = d.MEC127_VL_TRANS;
                    lstModConsulta.Add(m);

                        index++;
                        if (index == QtdOcorrencia) break;
                    }
                }

                result.ListaRetorno = lstModConsulta;

                if (MEC127_IND_CONT.CompareTo("S") == 0)
                {
                    ModConsultaResult res = ConsultaPorAvisoCancelamento(numPV, numAvsCan, MEC127_NU_IDE_CHE_CONT, MEC127_IND_CONT);
                    result.ListaRetorno.AddRange(res.ListaRetorno);
                }

                Log.GravarLog(EventoLog.RetornoAgente, new { result });
                return result;
            }
        }
        #endregion

        #region MEC121CO - Cancelamento de Vendas

        public List<ItemCancelamentoSaida> Cancelamento(List<ItemCancelamentoEntrada> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { input });

                List<ItemCancelamentoSaida> output = new List<ItemCancelamentoSaida>();
                string programname = "MEC121";
                string servicename = "";
                MEC121_DETALHE[] detailsOut = new MEC121_DETALHE[] { };
                string FILLER = "";

                MEC121_DADOS_INPUT[] detailsIn;

                if (input.Count > 50)
                {
                    detailsIn = new MEC121_DADOS_INPUT[input.Count];
                }
                else
                {
                    detailsIn = new MEC121_DADOS_INPUT[50];
                }

                for (int i = 0; i < input.Count; i++)
                {

                    detailsIn[i].MEC121_NU_ESTAB = input[i].NumEstabelecimento;

                    if (input[i].NSU.Length > 12)
                    {
                        detailsIn[i].MEC121_NU_NSU = string.Empty;
                        detailsIn[i].MEC121_NU_CARTAO = input[i].NSU.PadLeft(19, '0').Substring(0, 19);
                    }
                    else
                    {
                        detailsIn[i].MEC121_NU_CARTAO = string.Empty;
                        detailsIn[i].MEC121_NU_NSU = input[i].NSU.PadLeft(12, '0').Substring(0, 12);
                    }

                    detailsIn[i].MEC121_VL_TRANS = input[i].VlTransStr.PadLeft(17, '0');
                    detailsIn[i].MEC121_VL_CANC = input[i].VlCancStr.PadLeft(17, '0');
                    detailsIn[i].MEC121_DT_TRANS = input[i].DtTransfInt;

                    detailsIn[i].MEC121_TP_VENDA = input[i].TpVenda;
                    detailsIn[i].MEC121_NUM_PDV_CAN = input[i].NumPDVCanc;
                    detailsIn[i].MEC121_COD_CNL_CAN = 3;
                    detailsIn[i].MEC121_COD_USR_CAN = input[i].CodUserCanc.Length <= 20 ? input[i].CodUserCanc : input[i].CodUserCanc.Substring(0, 19);
                    detailsIn[i].MEC121_COD_IP_CAN = input[i].IPCanc;
                }


                using (ModuloCancelamento.CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programname, detailsIn, servicename });
                        client.MEC121CO(programname, detailsIn, ref servicename, out detailsOut, out FILLER);
                        Log.GravarLog(EventoLog.RetornoHIS, new { servicename, detailsOut, FILLER });
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                    }

                    for (int i = 0; i < detailsOut.Length; i++)
                    {
                        if (int.Parse(detailsOut[i].MEC121_RETCODE) == 0 && long.Parse(detailsOut[i].MEC121_NU_AVS_CNO) == 0)
                        { }
                        else
                        {
                            ItemCancelamentoSaida item = new ItemCancelamentoSaida();
                            item.CodRetorno = detailsOut[i].MEC121_RETCODE;
                            item.CodErro = detailsOut[i].MEC121_COD_ERRO;
                            item.MsgErro = detailsOut[i].MEC121_DCMENSG;
                            if (detailsOut[i].MEC121_RETCODE.CompareTo("0") != 0)
                            {
                                item.NumAvisoCanc = detailsOut[i].MEC121_NU_AVS_CNO;
                                item.VlSaldoAtual = detailsOut[i].MEC121_VL_SDO_ATU;
                            }

                            output.Add(item);
                        }
                    }
                }

                Log.GravarLog(EventoLog.RetornoAgente, new { output });
                return output;
            }
        }

        #endregion

        #region Cancelamento de Vendas Duplicadas
        /// <summary>
        /// Método para cancelar uma venda duplicada.
        /// </summary>
        /// <param name="input">Lista de vendas Duplicadas.</param>
        /// <returns>Lista com o resultado do cancelamento.</returns>
        public List<ItemCancelamentoSaida> CancelamentoDuplicadas(List<ModConsultaDuplicado> input)
        {
            using (Logger Log = Logger.IniciarLog("Cancelamento de uma venda duplicada"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { input });

                List<ItemCancelamentoSaida> output = new List<ItemCancelamentoSaida>();
                string programname = "MEC122";
                string servicename = "";
                MEC122_DETALHE[] detailsOut = new MEC122_DETALHE[] { };
                string FILLER = "";

                MEC122_DADOS_INPUT[] detailsIn;

                if (input.Count > 10)
                {
                    detailsIn = new MEC122_DADOS_INPUT[input.Count];
                }
                else
                {
                    detailsIn = new MEC122_DADOS_INPUT[10];
                }

                for (int i = 0; i < input.Count; i++)
                {

                    detailsIn[i].MEC122_COD_CNL_CAN = 3;
                    detailsIn[i].MEC122_COD_IP_CAN = input[i].CodIPCan;
                    detailsIn[i].MEC122_COD_USR_CAN = input[i].CodUsrCan;
                    detailsIn[i].MEC122_IND_INFO_DGTD = "0";
                    detailsIn[i].MEC122_NU_MES = input[i].Nu_mes.ToInt32();
                    detailsIn[i].MEC122_NU_TRX_TCC = input[i].Nu_trx_tcc;
                    detailsIn[i].MEC122_NUM_PDV_CAN = input[i].NumPdvCan;
                    detailsIn[i].MEC122_TP_PROD = input[i].Tp_prod.ToInt32();
                    detailsIn[i].MEC122_VL_CANC = input[i].VlCan;
                }
                using (ModuloCancelamento.CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programname, detailsIn });
                        string ret = client.MEC122CO(out detailsOut, out FILLER, programname, detailsIn);
                        Log.GravarLog(EventoLog.RetornoHIS, new { detailsOut, FILLER });

                        for (int i = 0; i < detailsOut.Length; i++)
                        {
                            if (int.Parse(detailsOut[i].MEC122_RETCODE) == 0 && long.Parse(detailsOut[i].MEC122_NU_AVS_CNO) == 0)
                            { }
                            else
                            {
                                ItemCancelamentoSaida item = new ItemCancelamentoSaida();
                                item.CodRetorno = detailsOut[i].MEC122_RETCODE;
                                item.CodErro = detailsOut[i].MEC122_COD_ERRO;
                                item.MsgErro = detailsOut[i].MEC122_DCMENSG;
                                if (detailsOut[i].MEC122_RETCODE.CompareTo("0") != 0)
                                {
                                    item.NumAvisoCanc = detailsOut[i].MEC122_NU_AVS_CNO;
                                    item.VlSaldoAtual = detailsOut[i].MEC122_VL_SDO_ATU;
                                }

                                output.Add(item);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                    }

                    Log.GravarLog(EventoLog.RetornoAgente, new { output });
                    return output;
                }
            }
        }

        /// <summary>
        /// Método para consultar as vendas duplicadas de uma entrada.
        /// </summary>
        /// <param name="entrada">Lista de Itens duplicados</param>
        /// <returns>Lista com as informações dos itens duplicados.</returns>
        public List<ModConsultaDuplicado> ConsultaDuplicados(List<ItemCancelamentoEntrada> entrada)
        {
            using (Logger Log = Logger.IniciarLog("Consultas vendas duplicadas de uma entrada"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { entrada });

                try
                {
                    List<ModConsultaDuplicado> result = new List<ModConsultaDuplicado>();
                    string codRetorno, codErro, msgErro, filler;
                    MEC124_DETALHE[] output = new MEC124_DETALHE[10];

                    string nsu, cartao;

                    foreach (ItemCancelamentoEntrada item in entrada)
                    {
                        using (ModuloCancelamento.CancelamentoClient client = new CancelamentoClient())
                        {
                            if (item.NSU.Length > 12)
                            {
                                nsu = string.Empty;
                                cartao = item.NSU.PadLeft(19, '0').Substring(0, 19);
                            }
                            else
                            {
                                cartao = string.Empty;
                                nsu = item.NSU.PadLeft(12, '0').Substring(0, 12);
                            }

                            Log.GravarLog(EventoLog.ChamadaHIS, new { noprogc = "MEC124", numEstabelecimento = item.NumEstabelecimento.ToString().PadLeft(9, '0'), cartao, nsu, dtTransfInt = item.DtTransfInt.ToString().PadLeft(8, '0'), vlTransStr = item.VlTransStr.PadLeft(17, '0'), item.TpVenda, filler = string.Empty });
                            string ret = client.MEC124CO(out codRetorno, out codErro, out msgErro, out output, out filler, "MEC124",
                                                         item.NumEstabelecimento.ToString().PadLeft(9, '0'), cartao, nsu, item.DtTransfInt.ToString().PadLeft(8, '0'),
                                                         item.VlTransStr.PadLeft(17, '0'), item.TpVenda, string.Empty);
                            Log.GravarLog(EventoLog.RetornoHIS, new { ret, codRetorno, codErro, msgErro, output, filler });

                            if (int.Parse(codRetorno) == 0)
                            {
                                MEC124_DETALHE detalhe;

                                for (int i = 0; i < output.Length; i++)
                                {
                                    detalhe = output[i];
                                    if (!string.IsNullOrEmpty(detalhe.MEC124_NU_TRX_TCC))
                                    {

                                        result.Add(new ModConsultaDuplicado()
                                        {
                                            Cd_aut_bnd = detalhe.MEC124_CD_AUT_BND,
                                            Cd_aut_inf = detalhe.MEC124_CD_AUT_INF,
                                            Desc_tp_tec = detalhe.MEC124_DESC_TP_TEC,
                                            Ind_can = detalhe.MEC124_IND_CAN,
                                            Nu_mes = detalhe.MEC124_NU_MES,
                                            Nu_trx_tcc = detalhe.MEC124_NU_TRX_TCC,
                                            Qtd_pca = detalhe.MEC124_QTD_PCA,
                                            Tp_prod = detalhe.MEC124_TP_PROD,
                                            Tp_trans = detalhe.MEC124_TP_TRANS,
                                            CodIPCan = item.IPCanc,
                                            CodUsrCan = item.CodUserCanc,
                                            NumPdvCan = item.NumEstabelecimento,
                                            NumNSUCartao = item.NSU.Length >= 16 ? item.NSU.Substring(0, 6) + "******" + item.NSU.Substring(12, 4) : item.NSU,
                                            VlCan = item.VlCanc
                                        });
                                    }
                                }
                            }
                            else
                            {
                                throw new PortalRedecardException(int.Parse(codRetorno), "AgentesPortalCancelamento.cs");
                            }
                        }
                    }

                    Log.GravarLog(EventoLog.RetornoAgente, new { result });
                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(500, "AgentesPortalCancelamento.cs", ex);
                }
            }
        }
        #endregion


        #region MEC125CO Comprovante de Cancelamento / Comprovante de Anulação Cancelamento

        /// <summary>
        /// Consulta as consultas realizados no dia possíveis para anulação
        /// </summary>
        /// <param name="codEstabelecimento">Estabelecimento a ser consultado</param>
        /// <returns></returns>
        public List<ModComprovante> ConsultaDia(int codEstabelecimento, string numideRechama, string indRechama)
        {
#if !DEBUG
            using (Logger Log = Logger.IniciarLog("Consultas realizadas nos dias possíveis para anulação"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento });

                String NomePrograma = "MEC125";          //MEC125_NOPROGC
                string CodigoEstabelecimento = codEstabelecimento.ToString().PadLeft(9, '0');   //MEC125_NU_ESTAB
                string CodigoCancelamento = "3";      //MEC125_ID_CD_CANC
                String CodigoRetorno = "";           //MEC125_RETCODE
                String CodigoErro = "";              //MEC125_COD_ERRO
                String ErroMensagem = "";              //MEC125_DCMENSG
                String NumeroOcorrencia = "0";        //MEC125_OCOR
                Int32 _NumeroOcorrencia = 0;

                //MEC125_DETALHE
                MEC125_DETALHE[] det = new MEC125_DETALHE[] { };
                string MEC125_IND_CONT = indRechama;
                String MEC125_NU_AVS_CONT = numideRechama;
                List<ModComprovante> lstModComprovanteAnulacaoCancalemento = new List<ModComprovante>();
                Boolean rechamada = false;

                using (CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        client.Open();

                        Log.GravarLog(EventoLog.ChamadaHIS, new { NomePrograma, CodigoEstabelecimento, CodigoCancelamento, MEC125_IND_CONT, MEC125_NU_AVS_CONT });

                        System.Diagnostics.Trace.WriteLine("INICIO DO");

                        do
                        {
                            client.MEC125CO(out CodigoRetorno, out CodigoErro, out ErroMensagem, out NumeroOcorrencia, out det,
                                NomePrograma, CodigoEstabelecimento, CodigoCancelamento, MEC125_IND_CONT, MEC125_NU_AVS_CONT);

                            System.Diagnostics.Trace.WriteLine("COD AVSO CANC: " + MEC125_NU_AVS_CONT);
                            System.Diagnostics.Trace.WriteLine("NUMERO OCORRE: " + NumeroOcorrencia);

                            _NumeroOcorrencia = Int32.Parse(NumeroOcorrencia);

                            for (int i = 0; i < _NumeroOcorrencia; i++)
                            {
                                if (rechamada && i == 0)
                                    i++;
                                MEC125_DETALHE d = det[i];
                                if (int.Parse(d.MEC125_CD_CANC) != 0)
                                {
                                    ModComprovante m = new ModComprovante();
                                    m.NumIdentifCarta = d.MEC125_NU_IDE_CARTA;
                                    m.NumMicrofilme = d.MEC125_NU_MCF;
                                    m.Transacao = d.MEC125_NU_TRX_TCC;
                                    m.NumCartao = d.MEC125_NU_CARTAO;
                                    m.NumCentralizadora = d.MEC125_NU_CENTR;
                                    m.TipoTransacao = d.MEC125_TP_TRANS;
                                    m.NumPV = d.MEC125_NU_PV;
                                    m.NumEstabelecimento = codEstabelecimento;
                                    m.NumResumoVenda = d.MEC125_NU_RV;
                                    m.NumNSU = d.MEC125_NU_NSU;
                                    m.DataTransacao = d.MEC125_DT_TRANS.Insert(2, "/").Insert(5, "/").ToDate();
                                    m.DataCarta = d.MEC125_DT_CARTA.Insert(2, "/").Insert(5, "/").ToDate();
                                    m.DataInclusao = d.MEC125_DT_INCL.Insert(2, "/").Insert(5, "/").ToDate();
                                    m.NumAvisoCancel = d.MEC125_NU_AVS_CNO;
                                    m.ValorTransacao = d.MEC125_VL_TRANS.Insert(d.MEC125_VL_TRANS.Length - 2, ",").ToDecimal();
                                    m.ValorCancelamento = d.MEC125_VL_CANC.Insert(d.MEC125_VL_CANC.Length - 2, ",").ToDecimal();
                                    m.ValorLiquido = d.MEC125_VLR_LIQ.Insert(d.MEC125_VLR_LIQ.Length - 2, ",").ToDecimal();
                                    m.TipoCancelamento = d.MEC125_CANCEL;
                                    m.QtdParcelas = int.Parse(d.MEC125_QTD_PCA);
                                    m.DescEstabelecimento = d.MEC125_DE_REF_ESTAB;
                                    m.CodigoCancelamento = d.MEC125_CD_CANC;
                                    lstModComprovanteAnulacaoCancalemento.Add(m);
                                }
                                MEC125_NU_AVS_CONT = d.MEC125_NU_AVS_CNO;
                            }

                            if (_NumeroOcorrencia > 29)
                                rechamada = true;
                            else
                            {
                                rechamada = false;
                                break;
                            }
                            System.Diagnostics.Trace.WriteLine("NUMERO OC RECH: " + NumeroOcorrencia);
                            System.Diagnostics.Trace.WriteLine("INDICA OC RECH: " + rechamada.ToString());
                        }
                        while (rechamada);

                        System.Diagnostics.Trace.WriteLine("FIM DO");

                        Log.GravarLog(EventoLog.RetornoHIS, new { CodigoRetorno, CodigoErro, ErroMensagem, NumeroOcorrencia, det });
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw new PortalRedecardException(500, FONTE, ex);
                    }
                    finally
                    {
                        client.Close();
                    }
                }

                if (CodigoRetorno != "")
                {
                    if (CodigoErro == "10") throw new PortalRedecardException(Int32.Parse(CodigoErro), FONTE, "PROGRAMA CHAMADOR NAO INFORMADO", new Exception());
                    if (CodigoErro == "20") throw new PortalRedecardException(Int32.Parse(CodigoErro), FONTE, "NUMERO ESTABELECIMENTO NAO INFORMADO OU INVALIDO", new Exception());
                    if (CodigoErro == "21") throw new PortalRedecardException(Int32.Parse(CodigoErro), FONTE, "IDENTIFICACAO DO CODIGO INVALIDO", new Exception());
                    if (CodigoErro == "30") throw new PortalRedecardException(Int32.Parse(CodigoErro), FONTE, "ARGUMENTO DE PESQUISA NAO ENCONTRADO", new Exception());
                    if (CodigoErro == "99") throw new PortalRedecardException(Int32.Parse(CodigoErro), FONTE, "ERRO NO ACESSO AO DB2 (VIDE SQLCODE) " + ErroMensagem, new Exception());
                }

                Log.GravarLog(EventoLog.RetornoAgente, new { lstModComprovanteAnulacaoCancalemento });

                //if (MEC125_IND_CONT.CompareTo("S") == 0)
                //{
                //    List<ModComprovante> res = ConsultaDia(codEstabelecimento, MEC125_NU_AVS_CONT, MEC125_IND_CONT);
                //    lstModComprovanteAnulacaoCancalemento.AddRange(res);
                //}

                return lstModComprovanteAnulacaoCancalemento;
            }
#else
            List<ModComprovante> lstModComprovanteAnulacaoCancalemento = new List<ModComprovante>();
            Random random = new Random(9999999);
            for (int i = 0; i < 50; i++)
            {
                ModComprovante m = new ModComprovante();
                m.NumIdentifCarta = random.Next().ToString();
                m.NumMicrofilme = random.Next().ToString();
                m.Transacao = random.Next().ToString();
                m.NumCartao = random.Next().ToString();
                m.NumCentralizadora = random.Next().ToString();
                m.TipoTransacao = random.Next().ToString();
                m.NumPV = random.Next().ToString();
                m.NumEstabelecimento = codEstabelecimento;
                m.NumResumoVenda = random.Next().ToString();
                m.NumNSU = random.Next().ToString();
                m.DataTransacao = DateTime.Now;
                m.DataCarta = DateTime.Now;
                m.DataInclusao = DateTime.Now;
                m.NumAvisoCancel = random.Next().ToString();
                m.ValorTransacao = random.Next();
                m.ValorCancelamento = random.Next();
                m.ValorLiquido = random.Next();
                m.TipoCancelamento = random.Next().ToString();
                m.QtdParcelas = 1;
                m.DescEstabelecimento = random.Next().ToString();
                m.CodigoCancelamento = random.Next().ToString();
                lstModComprovanteAnulacaoCancalemento.Add(m);
            }
            return lstModComprovanteAnulacaoCancalemento;
#endif
        }



        #endregion

        #region MEC126 Anular Cancelamento
        public List<ModAnularCancelamento> RealizarAnulacaoCancelamento(string usuario, string ipUsuario, List<ModComprovante> registrosDesfazer)
        {
            using (Logger Log = Logger.IniciarLog("Redalizar anulação cancelamento"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { usuario, ipUsuario, registrosDesfazer });

                List<ModAnularCancelamento> ObjDesfazerCancelamentoSaida = new List<ModAnularCancelamento>();

                // Int32 NumEst = numPDV;
                //Variaveis de entrada
                String NomePrograma = "MEC126";
                short CodigoRetorno = 0;
                short CodigoErro = 0;
                String DescricaoErro = "";
                MEC126_DETALHE[] MEC126_SAIDA = new MEC126_DETALHE[] { };
                MEC126_DADOS_INPUT[] MEC126_ENTRADA;

                if (registrosDesfazer.Count > 10)
                {
                    MEC126_ENTRADA = new MEC126_DADOS_INPUT[registrosDesfazer.Count];
                }
                else
                {
                    MEC126_ENTRADA = new MEC126_DADOS_INPUT[10];
                }

                string FILLER = "";

                //Chamada do Mainframe

                for (int i = 0; i < registrosDesfazer.Count; i++)  // recebe por entrada
                {
                    MEC126_ENTRADA[i].MEC126_NU_ESTAB = registrosDesfazer[i].NumEstabelecimento; // = registrosCancelar[i].NumPDV;
                    MEC126_ENTRADA[i].MEC126_NU_AVS_CNO = registrosDesfazer[i].NumAvisoCancel;
                    MEC126_ENTRADA[i].MEC126_NUM_PDV_DFZM = int.Parse(registrosDesfazer[i].NumPV);
                    MEC126_ENTRADA[i].MEC126_COD_CNL_DFZM = int.Parse(registrosDesfazer[i].CodigoCancelamento);
                    MEC126_ENTRADA[i].MEC126_COD_USR_DFZM = usuario;
                    MEC126_ENTRADA[i].MEC126_COD_IP_DFZM = ipUsuario;

                }

#if !DEBUG

                using (CancelamentoClient client = new CancelamentoClient())
                {
                    try
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { NomePrograma, MEC126_ENTRADA, FILLER });
                        client.MEC126CO(out MEC126_SAIDA, NomePrograma, MEC126_ENTRADA, ref FILLER);
                        Log.GravarLog(EventoLog.RetornoHIS, new { MEC126_SAIDA, FILLER});
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        throw new PortalRedecardException(500, FONTE, ex);
                    }
                    finally
                    {
                        client.Close();
                    }
                }
#else
                MEC126_SAIDA = new MEC126_DETALHE[MEC126_ENTRADA.Length];
                int contador = 0;
                foreach (ModComprovante input in registrosDesfazer)
                {
                    MEC126_SAIDA[contador] = new MEC126_DETALHE()
                    {
                        MEC126_RETCODE = "20",
                        MEC126_NU_NSU = input.NumNSU,
                        MEC126_NU_CARTAO = input.NumCartao,
                        MEC126_VL_CANC = input.ValorCancelamento.ToString(),
                        MEC126_VL_TRANS = input.ValorTransacao.ToString(),
                        MEC126_DCMENSG = "",
                        MEC126_QTD_PCA = input.QtdParcelas.ToString()
                    };
                    contador++;
                }
#endif
                ///Lançamento de Exceções
                if (CodigoRetorno != 0)
                {
                    if (CodigoErro == 10) throw new PortalRedecardException(CodigoErro, FONTE, "PROGRAMA CHAMADOR NAO INFORMADO", new Exception());
                    if (CodigoErro == 20) throw new PortalRedecardException(CodigoErro, FONTE, "NUMERO ESTABELECIMENTO NAO INFORMADO OU INVALIDO", new Exception());
                    if (CodigoErro == 21) throw new PortalRedecardException(CodigoErro, FONTE, "DATA INICIO PERIODO NAO INFORMADO OU INVALIDO", new Exception());
                    if (CodigoErro == 22) throw new PortalRedecardException(CodigoErro, FONTE, "DATA FINAL PERIODO NAO INFORMADO OU INVALIDO", new Exception());
                    if (CodigoErro == 23) throw new PortalRedecardException(CodigoErro, FONTE, "IDENTIFICACAO DO CODIGO INVALIDO", new Exception());
                    if (CodigoErro == 24) throw new PortalRedecardException(CodigoErro, FONTE, "CHAVE PESQUISA INVALIDO", new Exception());
                    if (CodigoErro == 30) throw new PortalRedecardException(CodigoErro, FONTE, "ARGUMENTO DE PESQUISA NAO ENCONTRADO", new Exception());
                    if (CodigoErro == 99) throw new PortalRedecardException(CodigoErro, FONTE, "ERRO NO ACESSO AO DB2 (VIDE SQLCODE) " + DescricaoErro, new Exception());
                }


                foreach (MEC126_DETALHE item in MEC126_SAIDA)
                {

                    ModAnularCancelamento obj = new ModAnularCancelamento();
                    obj.CodRetorno = item.MEC126_RETCODE;
                    obj.DesfazimentosFalhos = item.MEC126_COD_ERRO;
                    obj.numCartao = item.MEC126_NU_CARTAO;
                    obj.numNsu = item.MEC126_NU_NSU;
                    try
                    {
                        obj.dataTrans = item.MEC126_DT_TRANS.Insert(2, "/").Insert(5, "/").ToDate();
                        obj.valorCancel = item.MEC126_VL_CANC.Insert(item.MEC126_VL_CANC.Length - 2, ",").ToDecimal();
                        obj.valorTrans = item.MEC126_VL_TRANS.Insert(item.MEC126_VL_TRANS.Length - 2, ",").ToDecimal();
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }

                    ObjDesfazerCancelamentoSaida.Add(obj);
                }

                Log.GravarLog(EventoLog.RetornoAgente, new { ObjDesfazerCancelamentoSaida });
                return ObjDesfazerCancelamentoSaida;
            }
        }

        #endregion
    }
}
