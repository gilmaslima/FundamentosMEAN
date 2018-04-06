/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Enums;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Implementação de handler para geração de URLs de redirecionamento
    /// </summary>
    public class QueryString : HandlerBase
    {
        /// <summary>
        /// Obtém as urls de redirecionamento de acordo com os identificadores desejados.
        /// </summary>
        /// <param name="idsRedirecionamento">Ids de redirecionamento</param>
        /// <returns>Dicionário contendo as urls de redirecionamento</returns>
        [HttpGet]
        public HandlerResponse GetQueryString()
        {
            //objeto de retorno
            HandlerResponse response = new HandlerResponse();
            Dictionary<String, String> redirecionamentos = new Dictionary<String, String>();

            //recupera parâmetro de entrada
            String parametro = Request["ids"];

            //valida parâmetro de entrada
            if (String.IsNullOrEmpty(parametro))
                return response;

            String[] idsRedirecionamentos = new JavaScriptSerializer()
                .Deserialize<String[]>(parametro);

            //percorre cada identificador de redirecionamento
            foreach (String id in idsRedirecionamentos)
            {
                QueryStrings tipoRedirecionamento = id.ParseEnumFromDescription<QueryStrings>(QueryStrings.Nenhum);
                redirecionamentos[id] = ObterUrlRedirecionamento(tipoRedirecionamento);
            }

            return new HandlerResponse(redirecionamentos);
        }

        /// <summary>
        /// Método para geração de URLs
        /// </summary>
        /// <param name="tipoRedirecionamento">Identificador do tipo de redirecionamento que será utilizado</param>
        /// <returns>Url de redirecionamento</returns>
        private String ObterUrlRedirecionamento(QueryStrings tipoRedirecionamento)
        {
            String url = default(String);

            switch (tipoRedirecionamento)
            {
                case QueryStrings.ExtratoLancamentosFuturosCredito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.LancamentosFuturos,
                            ExtratoTipoVenda.Credito,
                            DateTime.Now,
                            DateTime.Now.AddDays(dias), null, true);
                        break;
                    }

                case QueryStrings.ExtratoLancamentosFuturosDebito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.LancamentosFuturos,
                            ExtratoTipoVenda.Debito,
                            DateTime.Now,
                            DateTime.Now.AddDays(dias), null, true);
                        break;
                    }

                case QueryStrings.ExtratoVendasCredito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.Vendas,
                            ExtratoTipoVenda.Credito,
                            DateTime.Now.AddDays(-dias),
                            DateTime.Now, null, true);
                        break;
                    }

                case QueryStrings.ExtratoVendasDebito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.Vendas,
                            ExtratoTipoVenda.Debito,
                            DateTime.Now.AddDays(-dias),
                            DateTime.Now, null, true);
                        break;
                    }

                case QueryStrings.ExtratoValoresPagosCredito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.ValoresPagos,
                            ExtratoTipoVenda.Credito,
                            DateTime.Now.AddDays(-dias),
                            DateTime.Now, null, true);
                        break;
                    }

                case QueryStrings.ExtratoValoresPagosDebito:
                    {
                        Int32 dias = Request["dias"].ToInt32(0);
                        url = ObterQueryStringLinkRelatorio(
                            ExtratoTipoRelatorio.ValoresPagos,
                            ExtratoTipoVenda.Debito,
                            DateTime.Now.AddDays(-dias),
                            DateTime.Now, 2, true);
                        break;
                    }

                case QueryStrings.Dirf:
                    {
                        Int32 ano = Request["ano"].ToInt32(0);
                        url = ObterQueryStringDirf(ano);
                        break;
                    }

                case QueryStrings.ConsultaVendas:
                    {
                        Int32 tipoBusca = Request["tipoBusca"].ToInt32(0);
                        Int32 tipoVenda = Request["tipoVenda"].ToInt32(0);
                        String numero = Request["numero"];
                        Int32 pvSelecionado = Sessao.CodigoEntidade;

                        DateTime? dataInicial = null;
                        if (!string.IsNullOrEmpty(Request["de"]))
                            dataInicial = Request["de"].ToDateTimeNull();

                        DateTime? dataFinal = null;
                        if (!string.IsNullOrEmpty(Request["ate"]))
                            dataFinal = Request["ate"].ToDateTimeNull();

                        url = ObterQueryStringConsultaVendas(
                            tipoBusca, 
                            tipoVenda, 
                            numero, 
                            pvSelecionado, 
                            dataInicial, 
                            dataFinal);
                        break;
                    }

                default:
                    url = String.Empty;
                    break;
            }

            return url;
        }

        /// <summary>
        /// Obtém a URL para consulta de vendas
        /// </summary>
        /// <param name="tipoBusca"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="numero"></param>
        /// <param name="pvSelecionado"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        private String ObterQueryStringConsultaVendas(
            Int32 tipoBusca,
            Int32 tipoVenda,
            String numero,
            Int32 pvSelecionado,
            DateTime? dataInicial,
            DateTime? dataFinal)
        {
            String url = String.Empty;

            try
            {
                var qs = new QueryStringSegura();
                qs["tipoBusca"] = tipoBusca.ToString();
                qs["tipoVenda"] = tipoVenda.ToString();
                qs["numero"] = numero;
                qs["numeroEstabelecimento"] = pvSelecionado.ToString();
                if(dataInicial.HasValue)
                    qs["dataInicial"] = dataInicial.Value.ToString("yyyy-MM-dd");
                if (dataFinal.HasValue)
                    qs["dataFinal"] = dataFinal.Value.ToString("yyyy-MM-dd");                        
                return String.Format("dados={0}", qs.ToString());
            }
            catch (ApplicationException ex)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterQueryStringLinkRelatorio");
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterQueryStringLinkRelatorio");
                SharePointUlsLog.LogErro(ex);
            }

            return url;
        }

        /// <summary>
        /// Método para geração de URL para as páginas do Extrato
        /// </summary>
        /// <param name="tipoRelatorio">Tipo do Relatório</param>
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pesquisar">Pesquisar</param>
        /// <returns>Url do relatório do extrato</returns>
        private String ObterQueryStringLinkRelatorio(
            ExtratoTipoRelatorio tipoRelatorio,
            ExtratoTipoVenda tipoVenda,
            DateTime dataInicial,
            DateTime dataFinal,
            Int32? modalidade,
            Boolean pesquisar)
        {
            String url = String.Empty;

            try
            {
                var qs = new QueryStringSegura();
                qs["DataInicial"] = dataInicial.ToString("dd/MM/yyyy");
                qs["DataFinal"] = dataFinal.ToString("dd/MM/yyyy");
                qs["TipoRelatorio"] = ((Int32)tipoRelatorio).ToString();
                qs["TipoVenda"] = ((Int32)tipoVenda).ToString();
                qs["Pesquisar"] = pesquisar ? Boolean.TrueString : Boolean.FalseString;

                if (modalidade.HasValue)
                    qs["Modalidade"] = modalidade.ToString();

                return String.Format("dados={0}", qs.ToString());
            }
            catch (ApplicationException ex)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterQueryStringLinkRelatorio");
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterQueryStringLinkRelatorio");
                SharePointUlsLog.LogErro(ex);
            }

            return url;
        }

        /// <summary>
        /// Método para geração de URL de DIRF
        /// </summary>
        /// <param name="anoBase">Ano basUrl da Dirf para determinado ano</returns>
        private String ObterQueryStringDirf(Int32 anoBase)
        {
            var qs = new QueryStringSegura();
            qs["anoBase"] = anoBase.ToString();
            return String.Format("dados={0}", qs.ToString());
        }
    }
}