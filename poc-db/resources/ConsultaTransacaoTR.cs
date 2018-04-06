using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.MEExtratoConsultaTransacao;
using Redecard.PN.Extrato.Modelo.ConsultaTransacao;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public class ConsultaTransacaoTR : ITradutor
    {
        #region [ Consulta Transação - Débito - MEC084CO / MEC084 / IS89 ]

        public static List<Debito> ConsultarDebitoSaida(String dados, Int32 quantidadeRegistros, String indicadoresTokenizacao)  
        {
            List<Debito> retorno = new List<Debito>();

            CortadorMensagem cortador = new CortadorMensagem(dados);
            CortadorMensagem cortadorDetalhe = new CortadorMensagem(indicadoresTokenizacao);

            String[] valores = cortador.LerOccurs(474, 66);
            String[] indicadores = cortadorDetalhe.LerOccurs(1, 66);

            foreach(String valor in valores)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    Debito registro = new Debito()
                    {
                        ResumoVenda = cortadorRegistro.LerInt32(9),
                        IdDatacash = cortadorRegistro.LerString(20),
                        NumeroCartao = cortadorRegistro.LerString(19),
                        NSU = cortadorRegistro.LerDecimal(13, 0),
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        DataTransacao = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        DataResumo = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        NumeroParcelas = cortadorRegistro.LerInt16(3),
                        ValorTransacao = cortadorRegistro.LerDecimal(11, 2),
                        NumeroAutorizacaoBanco = cortadorRegistro.LerString(9),
                        CodigoProdutoVenda = cortadorRegistro.LerString(3),
                        IdCancelamento = cortadorRegistro.LerInt16(1),
                        TimeStampTransacao = cortadorRegistro.LerString(26),
                        DescricaoBandeira = cortadorRegistro.LerString(7),
                        QuantidadeCancelamento = cortadorRegistro.LerInt16(2),
                        IndicadorTokenizacao = indicadores[retorno.Count].ToString()
                    };

                    if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.DebitoTipoCancelamento), (Int32)registro.IdCancelamento))
                        registro.TipoCancelamento = (Modelo.ConsultaTransacao.DebitoTipoCancelamento)registro.IdCancelamento;
                    else registro.TipoCancelamento = Modelo.ConsultaTransacao.DebitoTipoCancelamento.NaoIdentificado;

                    switch ((registro.CodigoProdutoVenda ?? "").Trim().ToUpper())
                    {
                        case "MLO": registro.DescricaoProdutoVenda = "MAESTRO LOCAL"; break;
                        case "PMA": registro.DescricaoProdutoVenda = "PARCELE MAIS"; break;
                        case "CDC": registro.DescricaoProdutoVenda = "CDC"; break;
                        case "TRI": registro.DescricaoProdutoVenda = "TRISHOP"; break;
                        case "MRE": registro.DescricaoProdutoVenda = "MAESTRO RECEPTIVE"; break;
                        case "C&S": registro.DescricaoProdutoVenda = "COMPRE E SAQUE"; break;
                        case "CST": registro.DescricaoProdutoVenda = "CONSTRUCARD"; break;
                        case "DIS": registro.DescricaoProdutoVenda = "DISTRIBUTION"; break;
                        case "AVI": registro.DescricaoProdutoVenda = "A VISTA"; break;
                        case "PRE": registro.DescricaoProdutoVenda = "PRE-DATADO"; break;
                    }


                    String[] cancelamentos = cortadorRegistro.LerOccurs(27, 12);
                    foreach (String areaCancelamento in cancelamentos)
                    {
                        if (registro.Cancelamentos.Count < registro.QuantidadeCancelamento)
                        {
                            CortadorMensagem cortadorCancelamento = new CortadorMensagem(areaCancelamento);
                            CancelamentoDebito cancelamento = new CancelamentoDebito();

                            cancelamento.NumeroAvisoProcesso = cortadorCancelamento.LerDecimal(17, 0);
                            cancelamento.DataCancelamento = cortadorCancelamento.LerData(8, "yyyyMMdd", true);
                            cancelamento.CodigoCanal = cortadorCancelamento.LerString(1);
                            cancelamento.IdOrigemDesagendamento = cortadorCancelamento.LerString(1);

                            if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.DebitoCanal), cancelamento.CodigoCanal.ToInt32(-1)))
                                cancelamento.Canal = (Modelo.ConsultaTransacao.DebitoCanal)cancelamento.CodigoCanal.ToInt32(-1);
                            else cancelamento.Canal = Modelo.ConsultaTransacao.DebitoCanal.NaoIdentificado;

                            if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.DebitoOrigemDesagendamento), cancelamento.IdOrigemDesagendamento.ToInt32(-1)))
                                cancelamento.OrigemDesagendamento = (Modelo.ConsultaTransacao.DebitoOrigemDesagendamento)cancelamento.IdOrigemDesagendamento.ToInt32(-1);
                            else cancelamento.OrigemDesagendamento = Modelo.ConsultaTransacao.DebitoOrigemDesagendamento.NaoIdentificado;

                            registro.Cancelamentos.Add(cancelamento);
                        }
                    }

                    retorno.Add(registro);
                }
            }

            return retorno;
        }

        #endregion

        #region [ Consulta Transação - Crédito - MEC119CO / MEC119 / IS69 ]

        public static List<Credito> ConsultarCreditoSaida(String dados, Int32 quantidadeRegistros, String indicadoresTokenizacao)
        {
            CortadorMensagem cortador = new CortadorMensagem(dados);
            CortadorMensagem cortadorDetalhe = new CortadorMensagem(indicadoresTokenizacao);


            List<Credito> retorno = new List<Credito>();

            String[] valores = cortador.LerOccurs(769, 40);
            String[] indicadores = cortadorDetalhe.LerOccurs(1, 40);

            foreach (String valor in valores)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    Credito registro = new Credito()
                    {
                        NumeroIdDatacash = cortadorRegistro.LerString(20),
                        NumeroCartao = cortadorRegistro.LerString(19),
                        DataTransacao = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        ValorTransacao = cortadorRegistro.LerDecimal(11, 2),
                        AutorizacaoVenda = cortadorRegistro.LerString(6),
                        NumeroResumoVendas = cortadorRegistro.LerInt32(9),
                        QuantidadeParcelas = cortadorRegistro.LerInt16(3),
                        CodigoProdutoVenda = cortadorRegistro.LerString(3),
                        IdCancelamento = cortadorRegistro.LerInt16(1),
                        DataResumo = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        TimeStampTransacao = cortadorRegistro.LerString(26),
                        DescricaoBandeira = cortadorRegistro.LerString(7),
                        QuantidadeCancelamentos = cortadorRegistro.LerInt16(2),
                        IndicadorTokenizacao = indicadores[retorno.Count].ToString()
                    };

                    if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.CreditoTipoCancelamento), registro.IdCancelamento))
                        registro.TipoCancelamento = (Modelo.ConsultaTransacao.CreditoTipoCancelamento)registro.IdCancelamento;
                    else registro.TipoCancelamento = Modelo.ConsultaTransacao.CreditoTipoCancelamento.NaoIdentificado;

                    switch ((registro.CodigoProdutoVenda ?? "").Trim().ToUpper())
                    {
                        case "ROT": registro.DescricaoProdutoVenda = "ROTATIVO"; break;
                        case "PCJ": registro.DescricaoProdutoVenda = "PARCELADO COM JUROS"; break;
                        case "PSJ": registro.DescricaoProdutoVenda = "PARCELADO SEM JUROS"; break;
                    }

                    String[] cancelamentos = cortadorRegistro.LerOccurs(28 ,23);

                    foreach(String areaCancelamento in cancelamentos)
                    {
                        if(registro.Cancelamentos.Count < registro.QuantidadeCancelamentos)
                        {
                            CortadorMensagem cortadorCancelamento = new CortadorMensagem(areaCancelamento);
                            CancelamentoCredito cancelamento = new CancelamentoCredito();
                            cancelamento.DataCancelamento = cortadorCancelamento.LerData(8, "yyyyMMdd", true);
                            cancelamento.NumeroAvisoProcesso = cortadorCancelamento.LerDecimal(17, 0);
                            cancelamento.CodigoMotivoCancelamento = cortadorCancelamento.LerInt16(3);

                            if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.CreditoMotivoCancelamento), (Int32)cancelamento.CodigoMotivoCancelamento))
                                cancelamento.MotivoCancelamento = (Modelo.ConsultaTransacao.CreditoMotivoCancelamento)cancelamento.CodigoMotivoCancelamento;
                            else cancelamento.MotivoCancelamento = Modelo.ConsultaTransacao.CreditoMotivoCancelamento.NaoIdentificado;

                            registro.Cancelamentos.Add(cancelamento);
                        }
                    }

                    retorno.Add(registro);
                }
            }

            return retorno;
        }

        #endregion

        #region [ Consulta Transação - Débito TID - MEC324CO / MEC324 / ]

        public static List<CancelamentoDebitoTID> ConsultarDebitoTIDSaida(String area, Int32 quantidadeRegistros)
        {
            List<CancelamentoDebitoTID> retorno = new List<CancelamentoDebitoTID>();

            CortadorMensagem cortador = new CortadorMensagem(area);

            String[] valores = cortador.LerOccurs(27, 12);

            foreach (String valor in valores)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    CancelamentoDebitoTID cancelamento = new CancelamentoDebitoTID()
                    {
                        NumeroAvisoProcesso = cortadorRegistro.LerDecimal(17, 0),
                        DataCancelamento = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        CodigoCanal = cortadorRegistro.LerString(1),
                        IdOrigemDesagendamento = cortadorRegistro.LerString(1)
                    };

                    if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.DebitoTidCanal), cancelamento.CodigoCanal))
                        cancelamento.Canal = (Modelo.ConsultaTransacao.DebitoTidCanal)cancelamento.CodigoCanal.ToInt32(-1);
                    else cancelamento.Canal = Modelo.ConsultaTransacao.DebitoTidCanal.NaoIdentificado;

                    if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.DebitoTidOrigemDesagendamento), cancelamento.IdOrigemDesagendamento.ToInt32(-1)))
                        cancelamento.OrigemDesagendamento = (Modelo.ConsultaTransacao.DebitoTidOrigemDesagendamento)cancelamento.IdOrigemDesagendamento.ToInt32(-1);
                    else cancelamento.OrigemDesagendamento = Modelo.ConsultaTransacao.DebitoTidOrigemDesagendamento.NaoIdentificado;

                    retorno.Add(cancelamento);
                }
            }

            return retorno;            
        }

        #endregion

        #region [ Consulta Transação - Crédito TID - MEC323CO / MEC323 / ]

        public static List<CancelamentoCreditoTID> ConsultarCreditoTIDSaida(String area, Int32 quantidadeRegistros)
        {
            List<CancelamentoCreditoTID> retorno = new List<CancelamentoCreditoTID>();

            CortadorMensagem cortador = new CortadorMensagem(area);

            String[] valores = cortador.LerOccurs(27, 24);

            foreach (String valor in valores)
            {
                if (retorno.Count < quantidadeRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    CancelamentoCreditoTID cancelamento = new CancelamentoCreditoTID
                    {
                        DataCancelamento = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        NumeroAvisoProcesso = cortadorRegistro.LerDecimal(17, 0),
                        CodigoMotivoCancelamento = cortadorRegistro.LerInt16(2)
                    };

                    if (Enum.IsDefined(typeof(Modelo.ConsultaTransacao.CreditoTidMotivoCancelamento), (Int32)cancelamento.CodigoMotivoCancelamento))
                        cancelamento.MotivoCancelamento = (Modelo.ConsultaTransacao.CreditoTidMotivoCancelamento)cancelamento.CodigoMotivoCancelamento;
                    else cancelamento.MotivoCancelamento = Modelo.ConsultaTransacao.CreditoTidMotivoCancelamento.NaoIdentificado;

                    retorno.Add(cancelamento);
                }
            }

            return retorno;         
        }

        #endregion
    }
}