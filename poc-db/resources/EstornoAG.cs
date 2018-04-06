/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;
using System.Reflection;
using Redecard.PN.Extrato.Agentes.WAExtratoEstornos;
using Redecard.PN.Extrato.Modelo.Estornos;
using System.ServiceModel;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.EstornosTR;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// 
    /// </summary>
    public class EstornoAG : AgentesBase
    {
        /// <summary>
        /// Instacia atual da classe. (Singleton)
        /// </summary>
        private static EstornoAG instancia;

        /// <summary>
        /// Retorna a instacia atual da classe ou cria uma nova. (Singleton)
        /// </summary>
        public static EstornoAG Instancia { get { return instancia ?? (instancia = new EstornoAG()); } }

        #region    [ Relatório de Estornos ]

        /// <summary>
        /// Consultar
        /// </summary>
        /// <param name="tipoVendaEntrada"></param>
        /// <param name="pvs"></param>
        /// <param name="dataInicialEntrada"></param>
        /// <param name="dataFinalEntrada"></param>
        /// <param name="tipoModalidadeEntrada"></param>
        /// <param name="rechamada"></param>
        /// <param name="indicadorRechamada"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Estorno> Consultar(
            Int16 tipoVendaEntrada,
            List<Int32> pvs,
            DateTime dataInicialEntrada,
            DateTime dataFinalEntrada,
            Int16 tipoModalidadeEntrada,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Estornos - BKWA2940"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC294";
                    // Crédito - 1
                    // Débito  - 2
                    Int16 tipoVenda = tipoVendaEntrada;
                    Int32 dataInicial = dataInicialEntrada.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFinal = dataFinalEntrada.ToString("yyyyMMdd").ToInt32();
                    // AMBOS       - 0 - TODOS
                    // QDO CREDITO - 1 - ROTATIVO
                    // QDO CREDITO - 2 - PARCELADO COM JUROS
                    // QDO CREDITO - 3 - PARCELADO SEM JUROS
                    // QDO DEBITO  - 1 - DEBITO A VISTA
                    // QDO DEBITO  - 2 - DEBITO PRE-DATADO
                    Int16 tipoModalidade = tipoModalidadeEntrada;
                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();

                    String rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Int32 rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    String rechamadaDatTran = rechamada.GetValueOrDefault<String>("DatTran");
                    Int32 rechamadaHorTran = rechamada.GetValueOrDefault<Int32>("HorTran");
                    Decimal rechamadaNsu = rechamada.GetValueOrDefault<Decimal>("Nsu");
                    Int16 rechamadaNumBlc = rechamada.GetValueOrDefault<Int16>("NumBlc");

                    String reservaDados = default(String);

                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String programaRetorno = default(String);
                    Int16 sequenciaRetorno = default(Int16);
                    String descricaoRetorno = default(String);
                    String reservaDadosRetorno = default(String);

                    String areaFixa = TR.ConsultarEntrada(pvs ?? new List<Int32>());

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            tipoVenda,
                            dataInicial,
                            dataFinal,
                            rechamada,
                            rechamadaIndicador,
                            rechamadaNumPV,
                            rechamadaDatTran,
                            rechamadaHorTran,
                            rechamadaNsu,
                            rechamadaNumBlc,
                            areaFixa
                        });

                        client.BKWA2940(ref nomePrograma,
                            ref tipoVenda,
                            ref dataInicial,
                            ref dataFinal,
                            ref tipoModalidade,
                            ref rechamadaIndicador,
                            ref rechamadaNumPV,
                            ref rechamadaDatTran,
                            ref rechamadaHorTran,
                            ref rechamadaNsu,
                            ref rechamadaNumBlc,
                            ref reservaDados,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref programaRetorno,
                            ref sequenciaRetorno,
                            ref descricaoRetorno,
                            ref reservaDadosRetorno,
                            ref areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            nomePrograma,
                            tipoVenda,
                            dataInicial,
                            dataFinal,
                            tipoModalidade,
                            rechamadaIndicador,
                            rechamadaNumPV,
                            rechamadaDatTran,
                            rechamadaHorTran,
                            rechamadaNsu,
                            rechamadaNumBlc,
                            reservaDados,
                            codigoRetorno,
                            codigoErro,
                            programaRetorno,
                            sequenciaRetorno,
                            descricaoRetorno,
                            reservaDadosRetorno,
                            areaFixa
                        });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["NumPV"] = rechamadaNumPV;
                    rechamada["DatTran"] = rechamadaDatTran;
                    rechamada["HorTran"] = rechamadaHorTran;
                    rechamada["Nsu"] = rechamadaNsu;
                    rechamada["NumBlc"] = rechamadaNumBlc;
                    rechamada["Indicador"] = rechamadaIndicador;

                    status = new StatusRetornoDTO(codigoRetorno, descricaoRetorno, fonteMetodo);
                    indicadorRechamada = "S".Equals((rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0 && status.CodigoRetorno != 60)
                        return null;

                    return TR.ConsultarSaida(areaFixa);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

            }
        }

        /// <summary>
        /// ConsultarTotalizadores
        /// </summary>
        /// <param name="tipoVendaEntrada"></param>
        /// <param name="pvsEntrada"></param>
        /// <param name="dataInicialEntrada"></param>
        /// <param name="dataFinalEntrada"></param>
        /// <param name="tipoModalidadeEntrada">Tipo da Modalidade da pesquisa</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public EstornoTotalizador ConsultarTotalizadores(
            Int16 tipoVendaEntrada,
            List<Int32> pvsEntrada,
            DateTime dataInicialEntrada,
            DateTime dataFinalEntrada,
            Int16 tipoModalidadeEntrada,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Estorno - Totalizadores - BKWA2930"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC293";
                    // Crédito - 1
                    // Débito  - 2
                    Int16 tipoVenda = tipoVendaEntrada;
                    Int32 dataInicial = dataInicialEntrada.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFinal = dataFinalEntrada.ToString("yyyyMMdd").ToInt32();
                    // AMBOS       - 0 - TODOS
                    // QDO CREDITO - 1 - ROTATIVO
                    // QDO CREDITO - 2 - PARCELADO COM JUROS
                    // QDO CREDITO - 3 - PARCELADO SEM JUROS
                    // QDO DEBITO  - 1 - DEBITO A VISTA
                    // QDO DEBITO  - 2 - DEBITO PRE-DATADO
                    Int16 tipoModalidade = tipoModalidadeEntrada;
                    Int32 quantidadePvs = pvsEntrada.Count;
                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String programaRetorno = default(String);
                    Int16 sequenciaRetorno = default(Int16);
                    String descricaoRetorno = default(String);
                    Int16 quantidadeRegistro = default(Int16);
                    WAExtratoEstornos.B293E_TAB_PDV[] pvs = new B293E_TAB_PDV[3000];
                    WAExtratoEstornos.B293S_BANDEIRAS[] bandeiras = new B293S_BANDEIRAS[99];
                    Decimal valorTotalEstornos = default(Decimal);
                    String reservaDados = String.Empty;

                    for (int i = 0; i < pvsEntrada.Count; i++)
                        pvs[i] = new B293E_TAB_PDV() { B293E_NUM_PV = pvsEntrada[i] };

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            nomePrograma,
                            tipoVendaEntrada,
                            pvsEntrada,
                            dataInicialEntrada,
                            dataFinalEntrada,
                            tipoModalidadeEntrada
                        });

                        client.BKWA2930(
                            ref nomePrograma,
                            ref tipoVenda,
                            ref dataInicial,
                            ref dataFinal,
                            ref tipoModalidade,
                            ref quantidadePvs,
                            ref pvs,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref programaRetorno,
                            ref sequenciaRetorno,
                            ref descricaoRetorno,
                            ref valorTotalEstornos,
                            ref quantidadeRegistro,
                            ref bandeiras,
                            ref reservaDados);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            nomePrograma,
                            tipoVenda,
                            dataInicial,
                            dataFinal,
                            tipoModalidade,
                            quantidadePvs,
                            pvs,
                            codigoRetorno,
                            codigoErro,
                            programaRetorno,
                            sequenciaRetorno,
                            descricaoRetorno,
                            valorTotalEstornos,
                            quantidadeRegistro,
                            bandeiras,
                            reservaDados
                        });
                    }

                    status = new StatusRetornoDTO(codigoRetorno, descricaoRetorno, fonteMetodo);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0 && status.CodigoRetorno != 60)
                        return null;

                    return new EstornoTotalizador()
                    {
                        CodigoTipoVenda = tipoVendaEntrada,
                        QuantidadeRegistros = quantidadeRegistro,
                        ValorTotalEstornos = valorTotalEstornos,
                        Bandeiras = PrepararRetornoBandeiras(bandeiras, quantidadeRegistro)
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ Relatório de Estornos ]

        #region    [ RETORNOS HIS ]

        /// <summary>
        /// Lê o <code>array</code> retornado do mainframe
        /// </summary>
        /// <param name="itens">Itens retornado do mainframe</param>
        /// <param name="quantidade">Quantidade de itens retornados</param>
        /// <returns><code>List</code> de OfertaPadrao</returns>
        private List<EstornoTotalizadorBandeira> PrepararRetornoBandeiras(WAExtratoEstornos.B293S_BANDEIRAS[] itens, Int32 quantidade)
        {
            return itens.Take(quantidade).Select(x => new EstornoTotalizadorBandeira
            {
                DescricaoBandeira = x.B293S_DES_BAND,
                ValorBandeira = x.B293S_TOT_BAND
            }).ToList();
        }
        #endregion [ RETORNOS HIS ]

    }
}

