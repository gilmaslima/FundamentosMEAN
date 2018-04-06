using System;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.WAExtratoResumoVendas;
using Redecard.PN.Extrato.Modelo.ResumoVendas;
using System.Text.RegularExpressions;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public class ResumoVendasTR : ITradutor
    {
        #region [ Resumo Vendas - Débito CVs Aceitos - WACA668 / WA668 / IS10 ]

        public static List<DebitoCVsAceitos> ConsultarDebitoCVsAceitosSaida(WACA668_TB1_LINHA_RV[] tb, Int32 quantidadeRegistros)
        {
            List<DebitoCVsAceitos> retorno = new List<DebitoCVsAceitos>();

            Func<Int32, DateTime> ToDateTime = (horasCV) =>
            {
                Regex regex = new Regex(@"(\d{2})(\d{2})(\d{2})");
                String str = horasCV.ToString().PadLeft(6, '0');
                return regex.Replace(str, "$1:$2:$3").ToDate("HH:mm:ss", DateTime.MinValue);
            };
       
            foreach (WACA668_TB1_LINHA_RV registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new DebitoCVsAceitos()
                    {
                        DataCV = registro.WACA668_S_DT_CV.TrimNullChar().ToDate("yyyyMMdd", DateTime.MinValue),
                        DataVencimento = registro.WACA668_S_DT_VENCTO.TrimNullChar().ToDate("yyyyMMdd", DateTime.MinValue),
                        Descricao = registro.WACA668_S_DESCR.TrimNullChar(),
                        HorasCV = ToDateTime(registro.WACA668_S_HS_CV.TrimNullChar().ToInt32(0)),
                        NumeroCartao = registro.WACA668_S_NU_CART.TrimNullChar(),
                        NumeroCV = new CortadorMensagem(registro.WACA668_S_NU_TRANS.TrimNullChar()).LerDecimal(12, 0),
                        Plano = registro.WACA668_S_PLANO.TrimNullChar().ToInt16(0),
                        TID = registro.WACA668_S_TID.TrimNullChar(),
                        ValorCompra = new CortadorMensagem(registro.WACA668_S_VAL_CMPR.TrimNullChar()).LerDecimal(13, 2),
                        ValorCV = new CortadorMensagem(registro.WACA668_S_VL_CV.TrimNullChar()).LerDecimal(11, 2),
                        ValorLiquido = new CortadorMensagem(registro.WACA668_S_VL_LIQ.TrimNullChar()).LerDecimal(9, 2),
                        ValorSaque = new CortadorMensagem(registro.WACA668_S_VL_SAQUE.TrimNullChar()).LerDecimal(9, 2),
                        VendaCancelada = registro.WACA668_S_VDA_CANC.TrimNullChar(),
                        IndicadorTokenizacao = registro.WACA668_S_ID_TOK
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Resumo Vendas - Crédito Ajustes - WACA704 / WA704 / IS14 ]

        public static List<CreditoAjustes> ConsultarCreditoAjustesSaida(String areaFixa, Int32 quantidadeRegistros)
        {
            List<CreditoAjustes> retorno = new List<CreditoAjustes>();

            CortadorMensagem cortador = new CortadorMensagem(areaFixa);

            String[] valores = cortador.LerOccurs(136, 200);

            foreach (String valor in valores)
            {
                CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                if(retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new CreditoAjustes()
                    {
                        CodigoAjuste = cortadorRegistro.LerInt32(3),
                        DescricaoAjuste = cortadorRegistro.LerString(31),
                        ReferenciaAjuste = cortadorRegistro.LerString(17),
                        PVAjuste = cortadorRegistro.LerInt32(9),
                        ValorDebito = cortadorRegistro.LerDecimal(15, 2),
                        ValorAjuste = cortadorRegistro.LerDecimal(15, 2),
                        DataVencimento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true),
                        CodigoBandeira = cortadorRegistro.LerString(7),
                        IndicadorDebitoDesagendamento = cortadorRegistro.LerString(3),
                        ProcessoRetencao = cortadorRegistro.LerString(15),
                        DataReferencia = cortadorRegistro.LerString(20)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Resumo Vendas - Crédito CVs Aceitos - WACA706 / WA706 / IS16 ]

        public static List<CreditoCVsAceitos> ConsultarCreditoCVsAceitosSaida(WACA706_TB1_LINHA_RV[] tb, Int32 quantidadeRegistros)
        {
            List<CreditoCVsAceitos> retorno = new List<CreditoCVsAceitos>();

            foreach (WACA706_TB1_LINHA_RV registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new CreditoCVsAceitos()
                    {
                        CancelamentoVenda = registro.WACA706_CANC_VND.TrimNullChar(),
                        Cartao = registro.WACA706_CARTAO_B.TrimNullChar(),
                        DataCV = registro.WACA706_DT_CV.TrimNullChar().ToDate("dd/MM/yyyy", DateTime.MinValue),
                        Hora = registro.WACA706_HORA.TrimNullChar().ToDate("HH:mm:ss", DateTime.MinValue),
                        NSU = registro.WACA706_NSU.TrimNullChar(),
                        NumCV = registro.WACA706_SEQ.TrimNullChar().ToInt32(0),
                        NumeroPV = registro.WACA706_NU_PVA.TrimNullChar().ToInt32(0),
                        Pais = registro.WACA706_PAIS.TrimNullChar(),
                        QuantidadeParcelas = registro.WACA706_QT_PAR.TrimNullChar().ToInt16(0),
                        TID = registro.WACA706_TID.TrimNullChar(),
                        TipoCaptura = registro.WACA706_CAPTURA.TrimNullChar(),
                        Valor = new CortadorMensagem(registro.WACA706_VALOR.TrimNullChar()).LerDecimal(15, 2),
                        IndicadorTokenizacao = registro.WACA706_ID_TOK
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Resumo Vendas - Débito CDC Ajustes - WACA748 / WA748 / ISD4 ]

        public static List<DebitoCDCAjuste> ConsultarDebitoCDCAjusteSaida(WACA748_TAB_RET[] tb, Int32 quantidadeRegistros)
        {
            List<DebitoCDCAjuste> retorno = new List<DebitoCDCAjuste>();

            foreach (WACA748_TAB_RET registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new DebitoCDCAjuste()
                    {
                        CodigoAjuste = registro.WACA748_CODAJS.TrimNullChar().ToInt16(0),
                        DataReferencia = registro.WACA748_DAT_REF.TrimNullChar().ToDate("dd/MM/yyyy", DateTime.MinValue),
                        IndicadorDebitoDesagendamento = registro.WACA748_DEB_DES.TrimNullChar(),
                        DescricaoAjuste = registro.WACA748_DSCAJS.TrimNullChar(),
                        PVAjuste = registro.WACA748_PV_AJS.TrimNullChar().ToInt32(0),
                        ReferenciaAjuste = registro.WACA748_REF_AJS.TrimNullChar(),
                        DataReferencia2 = registro.WACA748_DAT_REF2.TrimNullChar(),
                        ValorAjuste = new CortadorMensagem(registro.WACA748_VAL_AJS.TrimNullChar()).LerDecimal(15, 2),
                        ValorDebito = new CortadorMensagem(registro.WACA748_VAL_DEB.TrimNullChar()).LerDecimal(15, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Resumo Vendas - Construcard CVs Aceitos - WACA797 / WA797 / IS35 ]

        public static List<ConstrucardCVsAceitos> ConsultarConstrucardCVsAceitos(WACA797_TB1_LINHA_RV[] tb, Int32 quantidadeRegistros)
        {
            List<ConstrucardCVsAceitos> retorno = new List<ConstrucardCVsAceitos>();

            foreach (WACA797_TB1_LINHA_RV registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new ConstrucardCVsAceitos()
                    {
                        NumeroCartao = registro.WACA797_NU_CART.TrimNullChar(),
                        DataCV = registro.WACA797_DT_CV.TrimNullChar().ToDate("yyyyMMdd", DateTime.MinValue),
                        Descricao = registro.WACA797_DESCR.TrimNullChar(),
                        HorasCV = registro.WACA797_HS_CV.TrimNullChar().TrimStart('0').PadLeft(6, '0').ToDate("HHmmss", DateTime.MinValue),
                        Plano = registro.WACA797_PLANO.TrimNullChar().ToInt16(0),
                        SubTipoTransacao = registro.WACA797_SUB_TP_TRANS.TrimNullChar().ToInt16(0),
                        TID = registro.WACA797_TID.TrimNullChar(),
                        TipoTransacao = registro.WACA797_TP_TRANS.TrimNullChar().ToInt16(0),
                        ValorCompra = new CortadorMensagem(registro.WACA797_VAL_CMPR.TrimNullChar()).LerDecimal(13, 2),
                        ValorCV = new CortadorMensagem(registro.WACA797_VL_CV.TrimNullChar()).LerDecimal(15, 2),
                        ValorSaque = new CortadorMensagem(registro.WACA797_VAL_SQUE.TrimNullChar()).LerDecimal(13, 2),
                        NumeroCV = new CortadorMensagem(registro.WACA797_NU_TRANS.TrimNullChar()).LerDecimal(12, 0)
                    });
                }
            }

            return retorno;
        }
        
        #endregion

        #region [ Resumo Vendas - Recarga de Celular - Ajustes - BKWA2450 / WA245 / ISIE ]

        public static List<RecargaCelularAjuste> ConsultarRecargaCelularAjustes(
            WA245S_DETALHE[] tb, Int32 quantidadeRegistros, Int32 tipoPesquisa)
        {
            var retorno = new List<RecargaCelularAjuste>();

            foreach (WA245S_DETALHE registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    var ajuste = new RecargaCelularAjuste();
                    if (tipoPesquisa == 1)
                        ajuste = new RecargaCelularAjusteCredito();
                    else if (tipoPesquisa == 2)
                        ajuste = new RecargaCelularAjusteDebito();

                    ajuste.CodigoAjustes = registro.WA245S_COD_AJS;
                    ajuste.DataRecebimento = registro.WA245S_DAT_REC.ToDateTimeNull("dd.MM.yyyy");
                    ajuste.DataReferencia = registro.WA245S_DAT_REF.ToDateTimeNull("MM.yyyy");
                    ajuste.DescricaoAjuste = registro.WA245S_DES_AJS;
                    ajuste.DescricaoOrigemAjuste = registro.WA245S_DES_ORIG;
                    ajuste.NumeroPvAjuste = registro.WA245S_NUM_PV_AJ;
                    ajuste.ValorAjuste = registro.WA245S_VAL_AJU;
                    ajuste.ValorVenda = registro.WA245S_VAL_VDA;

                    retorno.Add(ajuste);
                }
            }
            return retorno;
        }

        #endregion

        #region [ Resumo Vendas - Recarga de Celular - Comprovantes - BKWA2460 / WA246 / ISIF ]

        public static List<RecargaCelularComprovante> ConsultarRecargaCelularComprovantes(
            WA246S_DETALHE[] tb, Int32 quantidadeRegistros)
        {
            var retorno = new List<RecargaCelularComprovante>();

            foreach (WA246S_DETALHE registro in tb)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    retorno.Add(new RecargaCelularComprovante
                    {
                        DataHoraTransacao = String.Join(String.Empty, 
                            String.Concat(registro.WA246S_DAT_TRAN, registro.WA246S_HOR_TRAN)
                            .Where(c => Char.IsNumber(c))).ToDateTimeNull("ddMMyyyyHHmmss"),
                        NumeroCelular = registro.WA246S_NUM_CEL,
                        NomeOperadora = registro.WA246S_DSC_OPER,
                        NumeroTransacao = registro.WA246S_NUM_TRAN,
                        StatusTransacao = registro.WA246S_STA_TRAN,
                        ValorComissao = registro.WA246S_VAL_CMSS,
                        ValorTransacao = registro.WA246S_VAL_TRAN
                    });
                }
            }
            return retorno;
        }

        #endregion
    }
}