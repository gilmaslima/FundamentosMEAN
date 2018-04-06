/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Modelos;
using Redecard.PN.RAV.Dados.ServicosRAV;

namespace Redecard.PN.RAV.Dados
{
    public class DadPortalRAV : BancoDeDadosBase
    {
        #region SINGLETON
        private static DadPortalRAV dadosPortalRav = null;
        
        private DadPortalRAV()
        { }

        /// <summary>
        /// Retorna a instância do objeto.
        /// </summary>
        /// <returns></returns>
        public static DadPortalRAV GetInstance()
        {
            if (dadosPortalRav == null)
            { dadosPortalRav = new DadPortalRAV(); }
            return dadosPortalRav;
        }
        #endregion

        #region Transação MA30 - RAV Avulso
        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível. 
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida VerificarRAVDisponivel(Int32 numeroPDV)
        {
            ModRAVAvulsoSaida ravSaida = null;

            try
            {

                string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                short bancoParaCredito = 0;//MA030_BANCO, 
                int agenciaParaCredito = 0;//MA030_AGENCIA, 
                decimal contaParaCredito = 0;//MA030_CONTA,
                decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                List<FILLER> filler = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                string mensagemDeErro = "";//MA030_MSGERRO,
                string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                List<FILLER1> filler1 = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                string totalParcial = "";//MA030_CA_IND_ANTEC, 
                string valorParcialAntecipacao = ""; //MA030_CA_VAL_ANTEC,
                string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                int numPdv = 0;//MA030_NUM_PDV, 
                short funcao = 0;//MA030_FUNCAO, 
                short tipoCredito = 0; //MA030_TIP_CREDITO, 
                short canal = 0;//MA030_CANAL, 
                decimal valorAAntecipar = 0;//MA030_VALOR_A_ANTECIPAR


                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {
                    short ret = cliente.BMA030(out dataDoProcessamento, out horaDoProcessamento, out bancoParaCredito, out agenciaParaCredito,
                                                out contaParaCredito, out valorMinimoParaAntecipacao, out horaInicioAntecipacaoD0, out horaFimAntecipacaoD0,
                                                out horaInicioAntecipacaoDN, out horaFimAntecipacaoDN, out filler, out taxaDeDesconto,
                                                out valorTotalDisponivelParaAntecipacao, out valorBrutoOriginal, out periodoDeSelecaoDe, out periodoDeSelecaoAte,
                                                out mensagemDeErro, out dataFimCarencia, out valorTotalAntecipadoParaPagamentoD0, out valorTotalAntecipadoParaPagamentoD1,
                                                out valorDisponivel, out qtdDeRVExistente, out filler1, out totalParcial, out valorParcialAntecipacao,
                                                out tipoDeSelecaoPorPeriodo, out periodoDeSelecaoDe, out periodoDeSelecaoAte, out DataQueTemRestricao,
                                                numPdv, funcao, tipoCredito, canal, valorAAntecipar);

                    ravSaida.Agencia = agenciaParaCredito;
                    ravSaida.Banco = bancoParaCredito;
                    ravSaida.Conta = long.Parse(contaParaCredito.ToString());
                    ravSaida.DataProcessamento = dataDoProcessamento;
                    ravSaida.Desconto = taxaDeDesconto;
                    ravSaida.FimCarencia = Convert.ToDateTime(dataFimCarencia);
                    ravSaida.HoraFimD0 = horaFimAntecipacaoD0.ToString();
                    ravSaida.HoraFimDn = horaFimAntecipacaoDN.ToString();
                    ravSaida.HoraIniD0 = horaInicioAntecipacaoD0.ToString();
                    ravSaida.HoraIniDn = horaInicioAntecipacaoDN.ToString();
                    ravSaida.HoraProcessamento = horaDoProcessamento;
                    ravSaida.MsgErro = mensagemDeErro;
                    ravSaida.PeriodoAte = Convert.ToDateTime(periodoDeSelecaoAte);
                    ravSaida.PeriodoDe = Convert.ToDateTime(periodoDeSelecaoDe);
                    ravSaida.Retorno = Convert.ToInt32(ret);
                    ravSaida.ValorAntecipadoD0 = valorTotalAntecipadoParaPagamentoD0;
                    ravSaida.ValorAntecipadoD1 = valorTotalAntecipadoParaPagamentoD1;
                    ravSaida.ValorBruto = valorBrutoOriginal;
                    ravSaida.ValorDisponivel = valorTotalDisponivelParaAntecipacao;
                    ravSaida.ValorMinimo = valorMinimoParaAntecipacao;
                    ravSaida.ValorOriginal = valorBrutoOriginal;

                    ravSaida.DadosAntecipado.DataAte = Convert.ToDateTime(fimPeriodoAntecipacao);
                    ravSaida.DadosAntecipado.DataDe = Convert.ToDateTime(inicioPeriodoAntecipacao);
                    ravSaida.DadosAntecipado.Indicador = totalParcial == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total;
                    ravSaida.DadosAntecipado.IndicadorData = tipoDeSelecaoPorPeriodo == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao;
                    ravSaida.DadosAntecipado.IndicadorProduto = DataQueTemRestricao == "R" ? ElndProdutoAntecipa.Rotativo : DataQueTemRestricao == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                    ravSaida.DadosAntecipado.Valor = valorAAntecipar;

                    foreach(FILLER f in filler)
                    {
                        ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito(){ 
                                                            DataCredito =Convert.ToDateTime(f.MA030_DAT_CREDITO),
                                                            TaxaEfetiva = f.MA030_PCT_EFETIVA,
                                                            TaxaPeriodo = f.MA030_PCT_PERIODO,
                                                            ValorLiquido = f.MA030_VALOR_LIQUIDO,
                                                            ValorParcelado = f.MA030_VALOR_PARCELADO,
                                                            ValorRotativo = f.MA030_VALOR_ROTATIVO
                        } );
                    }

                    foreach (FILLER1 f in filler1)
                    {
                      ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno(){
                                                 DataApresentacao = Convert.ToDateTime(f.MA030_RV_DAT_APRS),
                                                 NumeroRAV = f.MA030_RV_NUM_RV,
                                                 QuantidadeOC = f.MA030_RV_QTD_OC,
                                                 ValorBruto = f.MA030_RV_VAL_BRTO,
                                                 ValorLiquido = f.MA030_RV_VAL_LQDO
                      });
                    }
                    
             
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return ravSaida;
        }

        /// <summary>
        /// Método que realiza a consulta de RAV Avulso disponível.
        /// </summary>
        /// <param name="entradaRAV"></param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorAntecipado"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida ConsultarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorAntecipado)
        {
            //RAVDadosRetorno ravDadosRetorno = new RAVDadosRetorno();
            ModRAVAvulsoSaida ravSaida = new ModRAVAvulsoSaida();

            
                string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                short bancoParaCredito = 0;//MA030_BANCO, 
                int agenciaParaCredito = 0;//MA030_AGENCIA, 
                decimal contaParaCredito = 0;//MA030_CONTA,
                decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                List<FILLER> filler = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                string mensagemDeErro = "";//MA030_MSGERRO,
                string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                List<FILLER1> filler1 = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                string totalParcial = "";//MA030_CA_IND_ANTEC, 
                string valorParcialAntecipacao = ""; //MA030_CA_VAL_ANTEC,
                string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                int numPdv =  numeroPDV;//MA030_NUM_PDV, 
                short funcao = 0;//MA030_FUNCAO, 
                short tipCredito = (short)tipoCredito; //MA030_TIP_CREDITO, 
                short canal = 0;//MA030_CANAL, 
                decimal valorAAntecipar = valorAntecipado;//MA030_VALOR_A_ANTECIPAR

            try
            {
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {


                    short ret = cliente.BMA030(out dataDoProcessamento, out horaDoProcessamento, out bancoParaCredito, out agenciaParaCredito,
                                                out contaParaCredito, out valorMinimoParaAntecipacao, out horaInicioAntecipacaoD0, out horaFimAntecipacaoD0,
                                                out horaInicioAntecipacaoDN, out horaFimAntecipacaoDN, out filler, out taxaDeDesconto,
                                                out valorTotalDisponivelParaAntecipacao, out valorBrutoOriginal, out periodoDeSelecaoDe, out periodoDeSelecaoAte,
                                                out mensagemDeErro, out dataFimCarencia, out valorTotalAntecipadoParaPagamentoD0, out valorTotalAntecipadoParaPagamentoD1,
                                                out valorDisponivel, out qtdDeRVExistente, out filler1, out totalParcial, out valorParcialAntecipacao,
                                                out tipoDeSelecaoPorPeriodo, out periodoDeSelecaoDe, out periodoDeSelecaoAte, out DataQueTemRestricao,
                                                numeroPDV, funcao, tipCredito, canal, valorAAntecipar);


                    ravSaida.Agencia = agenciaParaCredito;
                    ravSaida.Banco = bancoParaCredito;
                    ravSaida.Conta = long.Parse(contaParaCredito.ToString());
                    ravSaida.DataProcessamento = dataDoProcessamento;
                    ravSaida.Desconto = taxaDeDesconto;
                    ravSaida.FimCarencia = Convert.ToDateTime(dataFimCarencia);
                    ravSaida.HoraFimD0 = horaFimAntecipacaoD0.ToString();
                    ravSaida.HoraFimDn = horaFimAntecipacaoDN.ToString();
                    ravSaida.HoraIniD0 = horaInicioAntecipacaoD0.ToString();
                    ravSaida.HoraIniDn = horaInicioAntecipacaoDN.ToString();
                    ravSaida.HoraProcessamento = horaDoProcessamento;
                    ravSaida.MsgErro = mensagemDeErro;
                    ravSaida.PeriodoAte = Convert.ToDateTime(periodoDeSelecaoAte);
                    ravSaida.PeriodoDe = Convert.ToDateTime(periodoDeSelecaoDe);
                    ravSaida.Retorno = Convert.ToInt32(ret);
                    ravSaida.ValorAntecipadoD0 = valorTotalAntecipadoParaPagamentoD0;
                    ravSaida.ValorAntecipadoD1 = valorTotalAntecipadoParaPagamentoD1;
                    ravSaida.ValorBruto = valorBrutoOriginal;
                    ravSaida.ValorDisponivel = valorTotalDisponivelParaAntecipacao;
                    ravSaida.ValorMinimo = valorMinimoParaAntecipacao;
                    ravSaida.ValorOriginal = valorBrutoOriginal;

                    ravSaida.DadosAntecipado.DataAte = Convert.ToDateTime(fimPeriodoAntecipacao);
                    ravSaida.DadosAntecipado.DataDe = Convert.ToDateTime(inicioPeriodoAntecipacao);
                    ravSaida.DadosAntecipado.Indicador = totalParcial == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total;
                    ravSaida.DadosAntecipado.IndicadorData = tipoDeSelecaoPorPeriodo == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao;
                    ravSaida.DadosAntecipado.IndicadorProduto = DataQueTemRestricao == "R" ? ElndProdutoAntecipa.Rotativo : DataQueTemRestricao == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                    ravSaida.DadosAntecipado.Valor = valorAAntecipar;

                    foreach (FILLER f in filler)
                    {
                        ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito()
                        {
                            DataCredito = Convert.ToDateTime(f.MA030_DAT_CREDITO),
                            TaxaEfetiva = f.MA030_PCT_EFETIVA,
                            TaxaPeriodo = f.MA030_PCT_PERIODO,
                            ValorLiquido = f.MA030_VALOR_LIQUIDO,
                            ValorParcelado = f.MA030_VALOR_PARCELADO,
                            ValorRotativo = f.MA030_VALOR_ROTATIVO
                        });
                    }

                    foreach (FILLER1 f in filler1)
                    {
                        ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno()
                        {
                            DataApresentacao = Convert.ToDateTime(f.MA030_RV_DAT_APRS),
                            NumeroRAV = f.MA030_RV_NUM_RV,
                            QuantidadeOC = f.MA030_RV_QTD_OC,
                            ValorBruto = f.MA030_RV_VAL_BRTO,
                            ValorLiquido = f.MA030_RV_VAL_LQDO
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return ravSaida;
        }

        /// <summary>
        /// Método que realiza a efetivação de RAV Avulso disponível.
        /// </summary>
        /// <param name="entradaRAV"></param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorAntecipado"></param>
        /// <returns></returns>
        public Int32 EfetuarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorAntecipado)
        {
            //RAVDadosRetorno ravDadosRetorno = new RAVDadosRetorno();
            int codigoRetorno = 0;

            try
            {
                string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                short bancoParaCredito = 0;//MA030_BANCO, 
                int agenciaParaCredito = 0;//MA030_AGENCIA, 
                decimal contaParaCredito = 0;//MA030_CONTA,
                decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                List<FILLER> filler = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                string mensagemDeErro = "";//MA030_MSGERRO,
                string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                List<FILLER1> filler1 = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                string totalParcial = "";//MA030_CA_IND_ANTEC, 
                string valorParcialAntecipacao = ""; //MA030_CA_VAL_ANTEC,
                string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                int numPdv = numeroPDV;//MA030_NUM_PDV, 
                short funcao = 0;//MA030_FUNCAO, 
                short tipCredito = (short)tipoCredito; //MA030_TIP_CREDITO, 
                short canal = 0;//MA030_CANAL, 
                decimal valorAAntecipar = valorAntecipado;//MA030_VALOR_A_ANTECIPAR


                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {
                    short ret = cliente.BMA030(out dataDoProcessamento, out horaDoProcessamento, out bancoParaCredito, out agenciaParaCredito,
                                                  out contaParaCredito, out valorMinimoParaAntecipacao, out horaInicioAntecipacaoD0, out horaFimAntecipacaoD0,
                                                  out horaInicioAntecipacaoDN, out horaFimAntecipacaoDN, out filler, out taxaDeDesconto,
                                                  out valorTotalDisponivelParaAntecipacao, out valorBrutoOriginal, out periodoDeSelecaoDe, out periodoDeSelecaoAte,
                                                  out mensagemDeErro, out dataFimCarencia, out valorTotalAntecipadoParaPagamentoD0, out valorTotalAntecipadoParaPagamentoD1,
                                                  out valorDisponivel, out qtdDeRVExistente, out filler1, out totalParcial, out valorParcialAntecipacao,
                                                  out tipoDeSelecaoPorPeriodo, out periodoDeSelecaoDe, out periodoDeSelecaoAte, out DataQueTemRestricao,
                                                  numPdv, funcao, tipCredito, canal, valorAAntecipar);

                    codigoRetorno = ret;
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return codigoRetorno;
        }


        /// <summary>
        /// Retorna uma entidade RAVAvulsoSaida preenchida com os valores de RAVRetorno e RAVDadosRetorno.
        /// </summary>
        /// <param name="ravRetorno"></param>
        /// <param name="ravDadosRetorno"></param>
        /// <returns></returns>
        //private ModRAVAvulsoSaida PreencherCamposSaidaMA30(RAVRetorno ravRetorno, RAVDadosRetorno ravDadosRetorno)
        //{
        //    ModRAVAvulsoSaida ravSaida = new ModRAVAvulsoSaida();

        //    ravSaida.Agencia = ravRetorno.MA030_AGENCIA;
        //    ravSaida.Banco = ravRetorno.MA030_BANCO;
        //    ravSaida.Conta = (long)ravRetorno.MA030_CONTA;

        //    for (int i = 0; i < ravRetorno.MA030_CREDITO.Count; i++)
        //    {
        //        ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito()
        //        {
        //            DataCredito = Convert.ToDateTime(ravRetorno.MA030_CREDITO[i].MA030_DAT_CREDITO),
        //            TaxaEfetiva = ravRetorno.MA030_CREDITO[i].MA030_PCT_EFETIVA,
        //            TaxaPeriodo = ravRetorno.MA030_CREDITO[i].MA030_PCT_PERIODO,
        //            ValorLiquido = ravRetorno.MA030_CREDITO[i].MA030_VALOR_LIQUIDO,
        //            ValorParcelado = ravRetorno.MA030_CREDITO[i].MA030_VALOR_PARCELADO,
        //            ValorRotativo = ravRetorno.MA030_CREDITO[i].MA030_VALOR_ROTATIVO
        //        });
        //    }

        //    ravSaida.DataProcessamento = ravRetorno.MA030_DAT_PROCESSAMENTO;
        //    ravSaida.Desconto = ravRetorno.MA030_PCT_DESCONTO;
        //    ravSaida.FimCarencia = Convert.ToDateTime(ravRetorno.MA030_DATA_FIM_CARENCIA);
        //    ravSaida.HoraFimD0 = ravRetorno.MA030_HORA_FIM_D0.ToString();
        //    ravSaida.HoraFimDn = ravRetorno.MA030_HORA_FIM_DN.ToString();
        //    ravSaida.HoraIniD0 = ravRetorno.MA030_HORA_INI_D0.ToString();
        //    ravSaida.HoraIniDn = ravRetorno.MA030_HORA_INI_DN.ToString();
        //    ravSaida.HoraProcessamento = ravRetorno.MA030_HOR_PROCESSAMENTO;
        //    ravSaida.MsgErro = ravRetorno.MA030_MSGERRO;
        //    ravSaida.PeriodoAte = Convert.ToDateTime(ravRetorno.MA030_DAT_PERIODO_ATE);
        //    ravSaida.PeriodoDe = Convert.ToDateTime(ravRetorno.MA030_DAT_PERIODO_DE);
        //    ravSaida.Retorno = (Int32)ravRetorno.MA030_COD_RETORNO;

        //    for (int i = 0; i < ravRetorno.MA030_VALORES.Count; i++)
        //    {
        //        ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno()
        //        {
        //            DataApresentacao = Convert.ToDateTime(ravRetorno.MA030_VALORES[i].MA030_RV_DAT_APRS),
        //            NumeroRAV = ravRetorno.MA030_VALORES[i].MA030_RV_NUM_RV,
        //            QuantidadeOC = ravRetorno.MA030_VALORES[i].MA030_RV_QTD_OC,
        //            ValorBruto = ravRetorno.MA030_VALORES[i].MA030_RV_VAL_BRTO,
        //            ValorLiquido = ravRetorno.MA030_VALORES[i].MA030_RV_VAL_LQDO
        //        });
        //    }

        //    ravSaida.ValorAntecipadoD0 = ravRetorno.MA030_VALOR_ANTEC_D0;
        //    ravSaida.ValorAntecipadoD1 = ravRetorno.MA030_VALOR_ANTEC_D1;
        //    ravSaida.ValorBruto = ravRetorno.MA030_VALOR_BRUTO;
        //    ravSaida.ValorDisponivel = ravRetorno.MA030_VALOR_DISP_ANTEC;
        //    ravSaida.ValorMinimo = ravRetorno.MA030_VALOR_MINIMO;
        //    ravSaida.ValorOriginal = ravRetorno.MA030_VALOR_ORIG;

        //    ravSaida.DadosAntecipado = new ModRAVAntecipa()
        //    {
        //        Indicador = ravDadosRetorno.MA030_CA_IND_ANTEC == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total,
        //        IndicadorData = ravDadosRetorno.MA030_CA_IND_DATA_ANTEC == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao,
        //        DataAte = Convert.ToDateTime(ravDadosRetorno.MA030_CA_PER_DATA_ATE),
        //        DataDe = Convert.ToDateTime(ravDadosRetorno.MA030_CA_PER_DATA_DE),
        //        Valor = ravDadosRetorno.MA030_CA_VAL_ANTEC,
        //        IndicadorProduto = ravDadosRetorno.MA030_DAT_RESTRICAO == "R" ? ElndProdutoAntecipa.Rotativo : ravDadosRetorno.MA030_DAT_RESTRICAO == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos
        //    };

        //    return ravSaida;
        //}        
        
        #endregion
        

        #region Transação MA61 - RAV Automatico
        /// <summary>
        /// Método que realiza a consulta do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAutomatico ConsultarRAVAutomatico(Int32 numeroPDV)
        {
            ModRAVAutomatico ravDados = null;

            try
            {

                string codigoFuncao = "21"; //  MA061_COD_FUNCAO, 
                decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                string tipoRvASerAntecipado = "R"; //MA061_TIP_RV, 
                short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                short numeroDParcelasFinal = 3; // MA061_NUM_PRCL_FIM, 
                short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                string codigoPeriodicidade = "D";//MA061_COD_PERIODICIDADE, 
                string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                string diaAntecipacao = "";//MA061_DIA_ANTC, 
                string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                short codigoRetorno = 0;// MA061_COD_RETORNO, 
                string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                string nomeDaCategoria = "";//MA061_DES_CATEG, 
                decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                string dataAlteracao = "";// MA061_DAT_ALTER, 
                string horaAlteracao = "";//MA061_HOR_ALTER, 
                string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                string taxaDeFidelizacao = "";//MA061_TAXA_FDLZ, 
                string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 
                string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {
                    cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                ref valorMinimoAntecipar,ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                ref paraUsoFuturoSaida);
                        
                   
                    if (codigoRetorno != 0)
                    {
                        ravDados.CodigoProduto = codigoProduto;
                        ravDados.CodMotivoExclusao = codigoMotivoExclusao;
                        ravDados.CodSituacao = codigoSituacaoPendencia;
                        ravDados.CodVenda = codigoCanalDeVenda;
                        ravDados.DataContrato = Convert.ToDateTime(dataDeContratacaoProduto);
                        ravDados.DataIniEstoq = Convert.ToDateTime(dataBaseProxAntec);
                        ravDados.DataVigenciaFim = Convert.ToDateTime(dataFimVigencia);
                        ravDados.DataVigenciaIni = Convert.ToDateTime(dataInicioVigencia);
                        ravDados.DescMotivoExclusao = descricaoMotivoExclusao;
                        ravDados.DiaAntecipacao = diaAntecipacao;
                        ravDados.IndAnteEstoq = indicadorAnteciparEstoque == "S" ? ElndAntecEstoq.Sim : ElndAntecEstoq.Nao;
                        ravDados.IndContratoPortal = Convert.ToChar(codigoCanalDeVenda);
                        ravDados.IndPRFComercial = Convert.ToChar(usuarioPertenceAreaComecial);
                        ravDados.NomeContato = nomeDoContatoDoEstabelecimento;
                        ravDados.NumeroPDV = numeroPDV;
                        ravDados.NumeroPDVRef = numeroPvReferencia;
                        ravDados.NumParcelaFim = numeroDParcelasFinal;
                        ravDados.NumParcelaIni = numeroDParcelasFinal;
                        ravDados.Periodicidade = codigoPeriodicidade == "D" ? EPeriodicidade.Diario : "" == "Q" ? EPeriodicidade.Quinzenal : EPeriodicidade.Semanal;
                        ravDados.QtdeDiasCancelamento = qtdDeDiasDeAgendamentoParaExclusao;
                        ravDados.TipoRAV = tipoRvASerAntecipado == "R" ? ElndProdutoAntecipa.Rotativo : "" == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                        ravDados.ValorMinimo = valorMinimoAntecipar;
                        ravDados.Funcao = codigoFuncao == "11" ? ECodFuncao.Simulacao : codigoFuncao == "21" ? ECodFuncao.Consultar : ECodFuncao.Efetivar;

                        ravDados.DadosRetorno.CodCategoria = Convert.ToInt32(codigoCategoriaTaxa);
                        ravDados.DadosRetorno.CodOpidAlteracao = Convert.ToInt32(codigoOperadorDeAlteracao);
                        ravDados.DadosRetorno.CodOpidAutorizacao = Convert.ToInt32(codigoOperacaoAutorizacao);
                        ravDados.DadosRetorno.CodRetorno = codigoRetorno;
                        ravDados.DadosRetorno.CodSituacaoPendente = codigoSituacaoPendente;
                        ravDados.DadosRetorno.CPF_CNPJ = numeroCNPJCPF.ToString();
                        ravDados.DadosRetorno.DataAgendaExclusao = Convert.ToDateTime(dataAgendadaParaExclusao);
                        ravDados.DadosRetorno.DataAlteracao = Convert.ToDateTime(dataAlteracao);
                        ravDados.DadosRetorno.DataAutorizacao = Convert.ToDateTime(dataAutorizacao);
                        ravDados.DadosRetorno.DataBaseAntecipacao = Convert.ToDateTime(dataBaseAntcEstoque);
                        ravDados.DadosRetorno.DataFimFidelizacao = Convert.ToDateTime(dataFimFidelizacao);
                        ravDados.DadosRetorno.DataIniFidelizacao = Convert.ToDateTime(dataInicioFidelizacao);
                        ravDados.DadosRetorno.DataProximaAntecipacao = Convert.ToDateTime(dataDaProximaAntecipacao);
                        ravDados.DadosRetorno.DescCategoria = nomeDaCategoria;
                        ravDados.DadosRetorno.DescSituacaoCategoria = descricaoPendenciaDaTaxa;
                        ravDados.DadosRetorno.Estabelecimento = nomeEstabelecimento;
                        ravDados.DadosRetorno.HoraAlteracao = horaAlteracao;
                        ravDados.DadosRetorno.HoraAutorizacao = horaAutorizacao;
                        ravDados.DadosRetorno.IndBloqueio = Convert.ToChar(indicadorDeBloqueioPorFidelizacao);
                        ravDados.DadosRetorno.MsgRetorno = mensagemDeRetorno;
                        ravDados.DadosRetorno.NumMatrix = numeroDaMatriz;
                        ravDados.DadosRetorno.TaxaCategoria = taxaCategoria;
                        ravDados.DadosRetorno.TaxaFidelizacao = Convert.ToDecimal(taxaDeFidelizacao);

                        switch (diaSemanaAntecipacao)
                        {
                            case "SEG":
                                { ravDados.DiaSemana = EDiaSemana.Segunda; break; }
                            case "TER":
                                { ravDados.DiaSemana = EDiaSemana.Terca; break; }
                            case "QUA":
                                { ravDados.DiaSemana = EDiaSemana.Quarta; break; }
                            case "QUI":
                                { ravDados.DiaSemana = EDiaSemana.Quinta; break; }
                            case "SEX":
                                { ravDados.DiaSemana = EDiaSemana.Sexta; break; }
                            default:
                                { break; }
                        }
                    }
                    else
                    {
                        throw new Exception("Erro " + codigoRetorno + ": " + mensagemDeRetorno);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return ravDados;
        }

        /// <summary>
        /// Método que realiza a efetivação do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean EfetuarRAVAutomatico(Int32 numeroPDV)
        {
            bool status = false;

            try
            {
                string codigoFuncao = "12"; //  MA061_COD_FUNCAO, 
                decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                string tipoRvASerAntecipado = "R"; //MA061_TIP_RV, 
                short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                short numeroDParcelasFinal = 3; // MA061_NUM_PRCL_FIM, 
                short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                string codigoPeriodicidade = "D";//MA061_COD_PERIODICIDADE, 
                string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                string diaAntecipacao = "";//MA061_DIA_ANTC, 
                string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                short codigoRetorno = 0;// MA061_COD_RETORNO, 
                string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                string nomeDaCategoria = "";//MA061_DES_CATEG, 
                decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                string operadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                string dataAlteracao = "";// MA061_DAT_ALTER, 
                string horaAlteracao = "";//MA061_HOR_ALTER, 
                string operacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                string taxaDeFidelizacao = "";//MA061_TAXA_FDLZ, 
                string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 
                string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA

                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {

                    cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                ref dataDaProximaAntecipacao, ref operadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                ref operacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                ref paraUsoFuturoSaida);
                    
                    status = codigoRetorno == 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return status;
        }
        
        /// <summary>
        /// Método que simula a inclusão de um RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAutomatico SimularRAVAutomatico(Int32 numeroPDV)
        {
            ModRAVAutomatico ravDados = new ModRAVAutomatico();

            try
            {
                string codigoFuncao = "11"; //  MA061_COD_FUNCAO, 
                decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                string tipoRvASerAntecipado = "R"; //MA061_TIP_RV, 
                short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                short numeroDParcelasFinal = 3; // MA061_NUM_PRCL_FIM, 
                short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                string codigoPeriodicidade = "D";//MA061_COD_PERIODICIDADE, 
                string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                string diaAntecipacao = "";//MA061_DIA_ANTC, 
                string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                short codigoRetorno = 0;// MA061_COD_RETORNO, 
                string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                string nomeDaCategoria = "";//MA061_DES_CATEG, 
                decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                string dataAlteracao = "";// MA061_DAT_ALTER, 
                string horaAlteracao = "";//MA061_HOR_ALTER, 
                string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                string taxaDeFidelizacao = "";//MA061_TAXA_FDLZ, 
                string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 
                string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA

                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {

                    cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                ref paraUsoFuturoSaida);
                    
       
                    if (codigoRetorno != 0)
                    {
                        ravDados.CodigoProduto = codigoProduto;
                        ravDados.CodMotivoExclusao = codigoMotivoExclusao;
                        ravDados.CodSituacao = codigoSituacaoPendencia;
                        ravDados.CodVenda = codigoCanalDeVenda;
                        ravDados.DataContrato = Convert.ToDateTime(dataDeContratacaoProduto);
                        ravDados.DataIniEstoq = Convert.ToDateTime(dataBaseProxAntec);
                        ravDados.DataVigenciaFim = Convert.ToDateTime(dataFimVigencia);
                        ravDados.DataVigenciaIni = Convert.ToDateTime(dataInicioVigencia);
                        ravDados.DescMotivoExclusao = descricaoMotivoExclusao;
                        ravDados.DiaAntecipacao = diaAntecipacao;
                        ravDados.IndAnteEstoq = indicadorAnteciparEstoque == "S" ? ElndAntecEstoq.Sim : ElndAntecEstoq.Nao;
                        ravDados.IndContratoPortal = Convert.ToChar(codigoCanalDeVenda);
                        ravDados.IndPRFComercial = Convert.ToChar(usuarioPertenceAreaComecial);
                        ravDados.NomeContato = nomeDoContatoDoEstabelecimento;
                        ravDados.NumeroPDV = numeroPDV;
                        ravDados.NumeroPDVRef = numeroPvReferencia;
                        ravDados.NumParcelaFim = numeroDParcelasFinal;
                        ravDados.NumParcelaIni = numeroDParcelasFinal;
                        ravDados.Periodicidade = codigoPeriodicidade == "D" ? EPeriodicidade.Diario : "" == "Q" ? EPeriodicidade.Quinzenal : EPeriodicidade.Semanal;
                        ravDados.QtdeDiasCancelamento = qtdDeDiasDeAgendamentoParaExclusao;
                        ravDados.TipoRAV = tipoRvASerAntecipado == "R" ? ElndProdutoAntecipa.Rotativo : ""=="P"? ElndProdutoAntecipa.Parcelado: ElndProdutoAntecipa.Ambos;
                        ravDados.ValorMinimo = valorMinimoAntecipar;
                        ravDados.Funcao = codigoFuncao == "11" ? ECodFuncao.Simulacao : codigoFuncao == "21" ? ECodFuncao.Consultar : ECodFuncao.Efetivar;
                        
                        ravDados.DadosRetorno.CodCategoria = Convert.ToInt32(codigoCategoriaTaxa);
                        ravDados.DadosRetorno.CodOpidAlteracao = Convert.ToInt32(codigoOperadorDeAlteracao);
                        ravDados.DadosRetorno.CodOpidAutorizacao = Convert.ToInt32(codigoOperacaoAutorizacao);
                        ravDados.DadosRetorno.CodRetorno = codigoRetorno;
                        ravDados.DadosRetorno.CodSituacaoPendente = codigoSituacaoPendente;
                        ravDados.DadosRetorno.CPF_CNPJ = numeroCNPJCPF.ToString();
                        ravDados.DadosRetorno.DataAgendaExclusao = Convert.ToDateTime(dataAgendadaParaExclusao);
                        ravDados.DadosRetorno.DataAlteracao = Convert.ToDateTime(dataAlteracao);
                        ravDados.DadosRetorno.DataAutorizacao = Convert.ToDateTime(dataAutorizacao);
                        ravDados.DadosRetorno.DataBaseAntecipacao = Convert.ToDateTime(dataBaseAntcEstoque);
                        ravDados.DadosRetorno.DataFimFidelizacao = Convert.ToDateTime(dataFimFidelizacao);
                        ravDados.DadosRetorno.DataIniFidelizacao = Convert.ToDateTime(dataInicioFidelizacao);
                        ravDados.DadosRetorno.DataProximaAntecipacao = Convert.ToDateTime(dataDaProximaAntecipacao);
                        ravDados.DadosRetorno.DescCategoria = nomeDaCategoria;
                        ravDados.DadosRetorno.DescSituacaoCategoria = descricaoPendenciaDaTaxa;
                        ravDados.DadosRetorno.Estabelecimento = nomeEstabelecimento;
                        ravDados.DadosRetorno.HoraAlteracao = horaAlteracao;
                        ravDados.DadosRetorno.HoraAutorizacao = horaAutorizacao;
                        ravDados.DadosRetorno.IndBloqueio = Convert.ToChar(indicadorDeBloqueioPorFidelizacao);
                        ravDados.DadosRetorno.MsgRetorno = mensagemDeRetorno;
                        ravDados.DadosRetorno.NumMatrix = numeroDaMatriz;
                        ravDados.DadosRetorno.TaxaCategoria = taxaCategoria;
                        ravDados.DadosRetorno.TaxaFidelizacao = Convert.ToDecimal(taxaDeFidelizacao);
                        
                        switch (diaSemanaAntecipacao)
                        {
                            case "SEG":
                                { ravDados.DiaSemana = EDiaSemana.Segunda; break; }
                            case "TER":
                                { ravDados.DiaSemana = EDiaSemana.Terca; break; }
                            case "QUA":
                                { ravDados.DiaSemana = EDiaSemana.Quarta; break; }
                            case "QUI":
                                { ravDados.DiaSemana = EDiaSemana.Quinta; break; }
                            case "SEX":
                                { ravDados.DiaSemana = EDiaSemana.Sexta; break; }
                            default:
                                { break; }
                        }

                    }
                    else
                    {
                        throw new Exception("Erro " + codigoRetorno + ": " + mensagemDeRetorno);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return ravDados;
        }

        /// <summary>
        /// Retorna uma entidade RAVAutoSaida preenchida com os valores de MA061_SAIDA.
        /// </summary>
        /// <param name="ravRetorno"></param>
        /// <returns></returns>
        //private ModRAVAutomatico PreencherCamposSaidaMA61(MA061CO ravRetorno)
        //{
        //    ModRAVAutomatico ravDadosRetorno = new ModRAVAutomatico();

        //    ravDadosRetorno.NumeroPDV = (long)ravRetorno.MA061_NUM_PDV;
        //    switch (ravRetorno.MA061_TIP_RV)
        //    {
        //        case "R":
        //            { ravDadosRetorno.TipoRAV = ElndProdutoAntecipa.Rotativo; break; }
        //        case "P":
        //            { ravDadosRetorno.TipoRAV = ElndProdutoAntecipa.Parcelado; break; }
        //        case "A":
        //            { ravDadosRetorno.TipoRAV = ElndProdutoAntecipa.Ambos; break; }
        //        default:
        //            { break; }
        //    }
        //    ravDadosRetorno.NumParcelaIni = ravRetorno.MA061_NUM_PRCL_INI;
        //    ravDadosRetorno.NumParcelaFim = ravRetorno.MA061_NUM_PRCL_FIM;
        //    ravDadosRetorno.CodSituacao = ravRetorno.MA061_COD_SIT_PENDENCIA;
        //    switch (ravRetorno.MA061_COD_PERIODICIDADE)
        //    {
        //        case "D":
        //            { ravDadosRetorno.Periodicidade = EPeriodicidade.Diario; break; }
        //        case "S":
        //            { ravDadosRetorno.Periodicidade = EPeriodicidade.Semanal; break; }
        //        case "Q":
        //            { ravDadosRetorno.Periodicidade = EPeriodicidade.Quinzenal; break; }
        //        default:
        //            { break; }
        //    }
        //    switch (ravRetorno.MA061_DIA_SEMANA)
        //    {
        //        case "SEG":
        //            { ravDadosRetorno.DiaSemana = EDiaSemana.Segunda; break; }
        //        case "TER":
        //            { ravDadosRetorno.DiaSemana = EDiaSemana.Terca; break; }
        //        case "QUA":
        //            { ravDadosRetorno.DiaSemana = EDiaSemana.Quarta; break; }
        //        case "QUI":
        //            { ravDadosRetorno.DiaSemana = EDiaSemana.Quinta; break; }
        //        case "SEX":
        //            { ravDadosRetorno.DiaSemana = EDiaSemana.Sexta; break; }
        //        default:
        //            { break; }
        //    }
        //    ravDadosRetorno.DataVigenciaIni = Convert.ToDateTime(ravRetorno.MA061_DAT_VIG_INI);
        //    ravDadosRetorno.DataVigenciaFim = Convert.ToDateTime(ravRetorno.MA061_DAT_VIG_FIM);
        //    ravDadosRetorno.DiaAntecipacao = ravRetorno.MA061_DIA_ANTC;
        //    ravDadosRetorno.ValorMinimo = ravRetorno.MA061_VAL_MIN_ANTC;
        //    switch (ravRetorno.MA061_IND_ANTC_ESTOQ)
        //    {
        //        case "S":
        //            { ravDadosRetorno.IndAnteEstoq = ElndAntecEstoq.Sim; break; }
        //        case "N":
        //            { ravDadosRetorno.IndAnteEstoq = ElndAntecEstoq.Nao; break; }
        //        default:
        //            { break; }
        //    }
        //    ravDadosRetorno.CodVenda = ravRetorno.MA061_COD_OPID_VENDA;
        //    ravDadosRetorno.NomeContato = ravRetorno.MA061_NOM_CONTATO;
        //    ravDadosRetorno.DataContrato = Convert.ToDateTime(ravRetorno.MA061_DAT_CONTRATO);
        //    ravDadosRetorno.CodMotivoExclusao = ravRetorno.MA061_COD_MOT_EXCLUSAO;
        //    ravDadosRetorno.DescMotivoExclusao = ravRetorno.MA061_DES_MOT_EXCLUSAO;
        //    ravDadosRetorno.QtdeDiasCancelamento = ravRetorno.MA061_QTD_DIA_CANC;

        //    #region Dados Retorno
        //    ravDadosRetorno.DadosRetorno.CodRetorno = ravRetorno.MA061_COD_RETORNO;
        //    ravDadosRetorno.DadosRetorno.MsgRetorno = ravRetorno.MA061_MSG_RETORNO;
        //    ravDadosRetorno.DadosRetorno.Estabelecimento = ravRetorno.MA061_NOM_PDV;
        //    ravDadosRetorno.DadosRetorno.CPF_CNPJ = ravRetorno.MA061_NUM_CNPJ.ToString();
        //    ravDadosRetorno.DadosRetorno.CodCategoria = Convert.ToInt32(ravRetorno.MA061_COD_CATEG);
        //    ravDadosRetorno.DadosRetorno.DescCategoria = ravRetorno.MA061_DES_CATEG;
        //    ravDadosRetorno.DadosRetorno.CodSituacaoPendente = ravRetorno.MA061_COD_SIT_PENDENTE;
        //    ravDadosRetorno.DadosRetorno.DescSituacaoCategoria = ravRetorno.MA061_DES_SIT_CATEG;
        //    ravDadosRetorno.DadosRetorno.TaxaCategoria = ravRetorno.MA061_PCT_TAXA_CATEG;
        //    ravDadosRetorno.DadosRetorno.DataBaseAntecipacao = Convert.ToDateTime(ravRetorno.MA061_DAT_BASE_ANTC);
        //    ravDadosRetorno.DadosRetorno.DataProximaAntecipacao = Convert.ToDateTime(ravRetorno.MA061_DAT_PRX_ANTC);
        //    ravDadosRetorno.DadosRetorno.DataAlteracao = Convert.ToDateTime(ravRetorno.MA061_DAT_ALTER);
        //    ravDadosRetorno.DadosRetorno.HoraAlteracao = ravRetorno.MA061_HOR_ALTER;

        //    //Não se aplica?
        //    ravDadosRetorno.DadosRetorno.CodOpidAlteracao = Convert.ToInt32(ravRetorno.MA061_COD_OPID_ALTER);
        //    ravDadosRetorno.DadosRetorno.CodOpidAutorizacao = Convert.ToInt32(ravRetorno.MA061_COD_OPID_AUTOR);
        //    ravDadosRetorno.DadosRetorno.DataAutorizacao = Convert.ToDateTime(ravRetorno.MA061_DAT_AUTOR);
        //    ravDadosRetorno.DadosRetorno.HoraAutorizacao = ravRetorno.MA061_HOR_AUTOR;
        //    //Fim Não se aplica

        //    ravDadosRetorno.DadosRetorno.DataAgendaExclusao = Convert.ToDateTime(ravRetorno.MA061_DAT_AGND_EXC);
        //    ravDadosRetorno.DadosRetorno.NumMatrix = ravRetorno.MA061_NUM_MATRIZ;
        //    ravDadosRetorno.DadosRetorno.IndBloqueio = Convert.ToChar(ravRetorno.MA061_IND_BLQ_FDLZ);
        //    ravDadosRetorno.DadosRetorno.TaxaFidelizacao = ravRetorno.MA061_TAXA_FDLZ;
        //    ravDadosRetorno.DadosRetorno.DataIniFidelizacao = Convert.ToDateTime(ravRetorno.MA061_DAT_INI_FDLZ);
        //    ravDadosRetorno.DadosRetorno.DataFimFidelizacao = Convert.ToDateTime(ravRetorno.MA061_DAT_FIM_FDLZ);
        //    #endregion

        //    return ravDadosRetorno;
        //}
        #endregion


        #region Transação MA135 - RAV Email
        /// <summary>
        /// Retorna os emails cadastrados para o PDV consultado.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVEmailEntradaSaida ConsultarEmails(Int32 numeroPDV)
        {
            ModRAVEmailEntradaSaida saida = null;

            try
            {
                String codigoFuncao = "C";//MA135_COD_FUNCAO, 
                Int32 codigoEstabelecimento = numeroPDV; //MA135_NUM_PDV, 
                String indicadorEnviaEmail = "";//MA135_IND_ENVO_EMAL, 
                String indicadorEnviaFax = "N";//MA135_IND_ENVO_FAX,
                String enderecoEmail = "";//MA135_TXT_INFD_EMAL, 
                String indicadorEnvioFluxoCaixa = "";//MA135_IND_OPC1, 
                String indicadorEnvioValoresPV = "";//MA135_IND_OPC2,
                String indicadorEnvioResumoOperacao = "";//MA135_IND_OPC3, 
                String indicadorSituacaoEmailFax = "A";//MA135_IND_SIT_EMAL_FAX, 
                //String indEnvioFax = "N";
                String opidQueFezAlteracao = "";//MA135_NUM_FNCL_CAD_EMAL, 
                short codigoRetorno = 0;//MA135_COD_RETORNO, 
                String mensagemRetorno = "";//MA135_MSG_RETORNO, 
                String erroSqlCode = "";//MA135_COD_SQLCODE, 
                String existemMaisRegistros = "";//MA135_IND_MAIS_OCORRENCIA,
                String dataUltimaAlteracao = "";//MA135_DAT_ULT_ALTER, 
                String dataInclusaoCadastro = "";//MA135_DAT_INC_CAD
               
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {
                    //Máximo de 3 emails
                    for (int i = 0; i < 3; i++)
                    {
                        short numeroSequencia = (short)(i + 1);//MA135_NUM_SEQ,
                        cliente.BMA135M(ref codigoFuncao, ref codigoEstabelecimento, ref numeroSequencia, ref indicadorEnviaEmail, ref indicadorEnviaFax, ref enderecoEmail,
                                       ref indicadorEnvioFluxoCaixa, ref indicadorEnvioValoresPV, ref indicadorEnvioResumoOperacao, ref indicadorSituacaoEmailFax,
                                       ref opidQueFezAlteracao, ref codigoRetorno, ref mensagemRetorno, ref erroSqlCode, ref existemMaisRegistros, ref dataUltimaAlteracao,
                                       ref dataInclusaoCadastro);


                        if (codigoRetorno != 15)
                        {
                            ModRAVEmail email = new ModRAVEmail();
                            email.Sequencia = numeroSequencia;
                            email.DataUltAlteracao = Convert.ToDateTime(dataUltimaAlteracao);
                            email.DataUltInclusao = Convert.ToDateTime(dataUltimaAlteracao);
                            email.Email = enderecoEmail;
                            saida.ListaEmails.Add(email);
                        }

                        if (existemMaisRegistros == "N")
                        {
                            break;
                        }
                    }

                    return saida;
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }

        /// <summary>
        /// Salva as alterações e inclusões de emails do PDV.
        /// </summary>
        /// <param name="dadosEmail"></param>
        /// <returns></returns>
        public Boolean SalvarEmails(ModRAVEmailEntradaSaida dadosEmail)
        {
            try
            {
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {
                    String codigoFuncao = "";//MA135_COD_FUNCAO, 

                    foreach (ModRAVEmail item in dadosEmail.ListaEmails)
                    {
                        if (item.Status != EStatusEmail.None)
                        {
                            if (item.Status == EStatusEmail.Incluso)
                            {
                                codigoFuncao = "I";
                            }
                            else
                            {
                                codigoFuncao = "A";
                            }

                            Int32 codigoEstabelecimento = Convert.ToInt32(dadosEmail.NumeroPDV); //MA135_NUM_PDV, 
                            short numeroSequenciaEmailFax = (short)item.Sequencia;//MA135_NUM_SEQ, 
                            String indicadorEnviaEmail = Convert.ToString(dadosEmail.IndEnviaEmail);//MA135_IND_ENVO_EMAL, 
                            String indicadorEnviaFax = "N";//MA135_IND_ENVO_FAX,
                            String enderecoEmail = item.Email;//MA135_TXT_INFD_EMAL, 
                            String indicadorEnvioFluxoCaixa = Convert.ToString(dadosEmail.IndEnviaFluxoCaixa);//MA135_IND_OPC1, 
                            String indicadorEnvioValoresPV = Convert.ToString(dadosEmail.IndEnviaValoresPV);//MA135_IND_OPC2,
                            String indicadorEnvioResumoOperacao = Convert.ToString(dadosEmail.IndEnviaResumoOperacao);//MA135_IND_OPC3, 
                            String indicadorSituacaoEmailFax = "A";//MA135_IND_SIT_EMAL_FAX, 
                            //String indEnvioFax = "N";
                            String opidQueFezAlteracao = "";//MA135_NUM_FNCL_CAD_EMAL, 
                            short codigoRetorno = 0;//MA135_COD_RETORNO, 
                            String mensagemRetorno = "";//MA135_MSG_RETORNO, 
                            String erroSqlCode = "";//MA135_COD_SQLCODE, 
                            String existemMaisRegistros = "";//MA135_IND_MAIS_OCORRENCIA,
                            String dataUltimaAlteracao = item.DataUltAlteracao.ToString();//MA135_DAT_ULT_ALTER, 
                            String dataInclusaoCadastro = "";//MA135_DAT_INC_CAD


                            cliente.BMA135M(ref codigoFuncao, ref codigoEstabelecimento, ref numeroSequenciaEmailFax, ref indicadorEnviaEmail, ref indicadorEnviaFax,
                                        ref enderecoEmail, ref indicadorEnvioFluxoCaixa, ref indicadorEnvioValoresPV, ref indicadorEnvioResumoOperacao, 
                                        ref indicadorSituacaoEmailFax, ref opidQueFezAlteracao, ref codigoRetorno,ref mensagemRetorno, ref erroSqlCode, 
                                        ref existemMaisRegistros, ref dataUltimaAlteracao, ref dataInclusaoCadastro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }

            return true;
        }
        #endregion


        #region Transação RW110 - Senha
        /// <summary>
        /// Método para verificar se o usuário tem acesso ao RAV.
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean ValidarSenha(String senha, Int32 numeroPDV)
        {
            try
            {
                String sistemaChamador = "IS";//BRW110E_SISTEMA, 
                Int32 numPDV = numeroPDV; //BRW110E_NUM_PDV, 
                short codigoDoProduto = 1;//BRW110E_COD_PROD,
                short tamanhoSenha = (short)senha.Length;//BRW110E_TAM_SENHA, 
                string senhaCriptografada = senha;//BRW110E_SENHA, 
                short telefoneChamadorDDD = 0;//BRW110E_NUM_DDD, 
                int telefoneChamador = 0;//BRW110E_NUM_FONE, 
                String areaLivre = "";//BRW110E_LIVRE, 
                short codRetorno = 0;//BRW110S_COD_ERRO, 
                String mensagemErro = "";//BRW110S_MSG_ERRO, 
                String dataUltimaSolicitacao = "";//BRW110S_ULT_SOLIC, 
                short qtdDiasParaExpirarSenha = 0;//BRW110S_QTD_DIAS, 
                String filler = "";//BRW110S_FILLER
                
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {

                    cliente.BRW110(ref sistemaChamador, ref numPDV, ref codigoDoProduto, ref tamanhoSenha, ref senhaCriptografada, ref telefoneChamadorDDD, ref telefoneChamador,
                        ref areaLivre, ref codRetorno, ref mensagemErro, ref dataUltimaSolicitacao, ref qtdDiasParaExpirarSenha, ref filler);

                    return codRetorno == 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }

        /// <summary>
        /// Método para verificar se o usuário tem acesso ao RAV.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean VerificarAcesso(Int32 numeroPDV)
        {
            return true;
        }
        #endregion
    }
}