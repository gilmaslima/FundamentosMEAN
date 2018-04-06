using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.FMS.Sharepoint.Helpers;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.FMS.Sharepoint
{
    public static class ExportadorHelper
    {
        /// <summary>
        /// Download do excel.
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="response"></param>
        /// <param name="colunas"></param>
        public static void GerarExcel(IList lista, HttpResponse response, params string[] colunas)
        {
            GridView gr = new GridView();

            gr.DataSource = lista;

            response.Buffer = false;
            response.BufferOutput = false;
            response.Clear();
            response.AddHeader("Content-Disposition", "attachment;filename=fms.xls");
            response.Charset = "ISO-8859-1";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.GetEncoding("Windows-1252");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    int i = 0;

                    gr.DataBind();

                    foreach (string header in colunas)
                    {
                        gr.HeaderRow.Cells[i].Text = header;
                        i++;
                    }

                    gr.RenderControl(hw);

                    response.Write(sw.ToString());
                    response.Flush();
                    response.End();
                }
            }
        }

        /// <summary>
        /// Download do excel.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="response"></param>
        public static void GerarExcelGridView(GridView gr, HttpResponse response)
        {
            response.Buffer = false;
            response.BufferOutput = false;
            response.Clear();
            response.AddHeader("Content-Disposition", "attachment;filename=fms.xls");
            response.Charset = "ISO-8859-1";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.GetEncoding("Windows-1252");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gr.Controls[0].RenderControl(htw);
                }
                response.Write(sw.ToString());
                response.Flush();
                response.End();
            }
        }

        /// <summary>
        ///  Download do excel.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="registros"></param>
        /// <param name="response"></param>
        public static void GerarExcel(GridView gr, IList registros, HttpResponse response)
        {
            GridView gridView = new GridView();

            foreach (DataControlField data in gr.Columns)
            {
                gridView.Columns.Add(data);
            }

            gridView.DataSource = registros;
            gridView.DataBind();

            response.Buffer = false;
            response.BufferOutput = false;
            response.Clear();
            response.AddHeader("Content-Disposition", "attachment;filename=fms.xls");
            response.Charset = "ISO-8859-1";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.GetEncoding("Windows-1252");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gridView.Controls[0].RenderControl(htw);
                }
                response.Write(sw.ToString());
                response.Flush();
                response.End();
            }
        }

        private static string ObterResultadoAutorizacao(ResultadoAutorizacao res)
        {
            switch (res)
            {
                case ResultadoAutorizacao.Autorizado:
                    return "Sim";
                case ResultadoAutorizacao.NaoAplicavel:
                    return "Não Aplicável";
                case ResultadoAutorizacao.NaoAutorizado:
                    return "Não";
                default:
                    return String.Empty;
            }
        }

        private static string ObterSituacaoBloqueio(SituacaoBloqueio res)
        {
            switch (res)
            {
                case SituacaoBloqueio.BloqueadoMesmoUsuario:
                    return "Bloqueio mesmo usuário";
                case SituacaoBloqueio.BloqueadoOutroUsuario:
                    return "Bloqueado outro usuário";
                case SituacaoBloqueio.SemBloqueio:
                    return "Sem Bloqueio";
                default:
                    return String.Empty;
            }
        }

        private static string ObterSituacaoTransacao(SituacaoFraude res)
        {
            switch (res)
            {
                case SituacaoFraude.EmAnalise:
                    return "Em análise";
                case SituacaoFraude.Fraude:
                    return "Fraude";
                case SituacaoFraude.NaoAplicavel:
                    return "Não aplicável";
                default:
                    return String.Empty;
            }
        }

        private static string ObterTipoCartao(TipoCartao res)
        {
            switch (res)
            {
                case TipoCartao.Credito:
                    return "C";
                case TipoCartao.Debito:
                    return "D";
                default:
                    return String.Empty;
            }
        }

        private static string ObterIndicadorTipoCartao(IndicadorTipoCartao res)
        {
            switch (res)
            {
                case IndicadorTipoCartao.Credito:
                    return "C";
                case IndicadorTipoCartao.Debito:
                    return "D";
                case IndicadorTipoCartao.Ambos:
                    return "C/D";
                default:
                    return String.Empty;
            }
        }

        private static string ObterSituacaoCartao(SituacaoCartao res)
        {
            switch (res)
            {
                case SituacaoCartao.Analisado:
                    return "Analisado";
                case SituacaoCartao.NaoAnalisado:
                    return "Não Analisado";
                default:
                    return String.Empty;
            }
        }

        private static string ObterTipoAlarme(TipoAlarme res)
        {
            switch (res)
            {
                case TipoAlarme.NaoAplicavel:
                    return "Não aplicável";
                case TipoAlarme.POC:
                    return "POC";
                case TipoAlarme.Score:
                    return "Score";
                case TipoAlarme.UTL:
                    return "UTL";
                default:
                    return String.Empty;
            }
        }

        #region Exportação.
        public static List<Modelo.AnalisaTransacoesSuspeitasRelatorio> ObterAnalisaTransacoesSuspeitasRelatorio(TransacaoEmissor[] lista)
        {
            List<Modelo.AnalisaTransacoesSuspeitasRelatorio> result = new List<Modelo.AnalisaTransacoesSuspeitasRelatorio>();

            foreach (TransacaoEmissor de in lista)
            {
                Modelo.AnalisaTransacoesSuspeitasRelatorio para = new Modelo.AnalisaTransacoesSuspeitasRelatorio();
                para.TipoAlarme = ObterTipoAlarme(de.TipoAlarme);
                para.Cartao = UtilitariosHelper.ObterCartaoMascarado(de.NumeroCartao);
                para.Valor = de.Valor.ToString("C");
                para.DataHoraTransacao = de.DataTransacao.ToString("dd/MM/yyyy HH:mm:ss");
                para.Score = de.Score.ToString();
                para.MCC = de.CodigoMCC + " - " + de.DescricaoMCC;
                para.TipoCartao = ObterTipoCartao(de.TipoCartao);
                para.Uf = de.UnidadeFederacao;
                para.StatusTransacao = ObterSituacaoTransacao(de.SituacaoTransacao);
                para.Bandeira = de.Bandeira;

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.AnalisaTransacoesSuspeitasPorCartaoRelatorio> ObterAnalisaTransacoesSuspeitasPorCartaoRelatorio(List<Modelo.TransacaoSuspeita> lista)
        {
            List<Modelo.AnalisaTransacoesSuspeitasPorCartaoRelatorio> result = new List<Modelo.AnalisaTransacoesSuspeitasPorCartaoRelatorio>();

            foreach (Modelo.TransacaoSuspeita de in lista)
            {
                Modelo.AnalisaTransacoesSuspeitasPorCartaoRelatorio para = new Modelo.AnalisaTransacoesSuspeitasPorCartaoRelatorio();
                para.TipoAlarme = ObterTipoAlarme(de.TipoAlarme);
                para.TipoResposta = de.TipoResposta.NomeResposta;
                para.DataHoraTransacao = de.DataTransacao.ToString("dd/MM/yyyy HH:mm:ss");
                para.Valor = de.Valor.ToString("C");
                para.Score = de.Score.ToString();
                para.Aut = ObterResultadoAutorizacao(de.ResultadoAutorizacao);
                para.Estabelecimento = de.CodigoEstabelecimento + " - " + de.NomeEstabelecimento;
                para.Mcc = de.CodigoMCC + " - " + de.DescricaoMCC;
                para.Uf = de.UnidadeFederacao;
                para.TipoCartao = ObterTipoCartao(de.TipoCartao);
                para.Em = de.EntryMode;
                para.Usuario = de.UsuarioAnalise;
                para.DataHoraAnalise = de.DataAnalise.ToString("dd/MM/yyyy HH:mm:ss");

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio> ObterTransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio(AgrupamentoTransacaoEstabelecimento[] lista)
        {
            List<Modelo.TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio> result = new List<Modelo.TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio>();

            foreach (AgrupamentoTransacaoEstabelecimento de in lista)
            {
                Modelo.TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio para = new Modelo.TransacoesSuspeitasAgrupadasPorEstabelecimentoRelatorio();
                para.NumeroEstabelecimento = de.NumeroEstabelecimento.ToString();
                para.NomeFantasiaEstabelecimento = de.NomeFantasiaEstabelecimento;
                para.ValorTotalTransacoes = de.ValorTotalTransacoes.ToString("C");
                para.QuantidadeTotalTransacoes = de.QuantidadeTotalTransacoes.ToString();
                para.ValorTransacoesSuspeitasAprovadas = de.ValorTransacoesSuspeitasAprovadas.ToString("C");
                para.QuantidadeTransacoesSuspeitasAprovadas = de.QuantidadeTransacoesSuspeitasAprovadas.ToString();
                para.ValorTransacoesSuspeitasNegadas = de.ValorTransacoesSuspeitasNegadas.ToString("C");
                para.QuantidadeTransacoesSuspeitasNegadas = de.QuantidadeTransacoesSuspeitasNegadas.ToString();
                para.TipoCartao = ObterIndicadorTipoCartao(de.TipoCartao);

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.TransacoesSuspeitasAgrupadasPorCartaoRelatorio> ObterTransacoesSuspeitasAgrupadasPorCartaoRelatorio(AgrupamentoTransacoesCartao[] lista)
        {
            List<Modelo.TransacoesSuspeitasAgrupadasPorCartaoRelatorio> result = new List<Modelo.TransacoesSuspeitasAgrupadasPorCartaoRelatorio>();

            foreach (AgrupamentoTransacoesCartao de in lista)
            {
                Modelo.TransacoesSuspeitasAgrupadasPorCartaoRelatorio para = new Modelo.TransacoesSuspeitasAgrupadasPorCartaoRelatorio();
                para.NumeroCartao = UtilitariosHelper.ObterCartaoMascarado(de.NumeroCartao);
                para.DataHoraTransacaoSuspeita = de.DataTransacaoSuspeitaMaisRecente.ToString("dd/MM/yyyy HH:mm:ss");
                para.Score = de.Score.ToString();
                para.ValorTransacoesSuspeitas = de.ValorTotalTransacoes.ToString("C");
                para.ValorTransacoesSuspeitasAprovadas = de.ValorTransacoesSuspeitasAprovadas.ToString("C");
                para.QuantidadeTransacoesSuspeitasAprovadas = de.QuantidadeTransacoesSuspeitasAprovadas.ToString();
                para.ValorTransacoesSuspeitasNegadas = de.ValorTransacoesSuspeitasNegadas.ToString("C");
                para.QuantidadeTransacoesSuspeitasNegadas = de.QuantidadeTransacoesSuspeitasNegadas.ToString();
                para.TipoCartao = ObterIndicadorTipoCartao(de.TipoCartao);

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.ConsultaTransacoesPorPeriodoSituacaoRelatorio> ObterConsultaTransacoesPorPeriodoSituacaoRelatorio(TransacaoEmissor[] lista)
        {
            List<Modelo.ConsultaTransacoesPorPeriodoSituacaoRelatorio> result = new List<Modelo.ConsultaTransacoesPorPeriodoSituacaoRelatorio>();

            foreach (TransacaoEmissor de in lista)
            {
                Modelo.ConsultaTransacoesPorPeriodoSituacaoRelatorio para = new Modelo.ConsultaTransacoesPorPeriodoSituacaoRelatorio();
                para.TipoAlarme = ObterTipoAlarme(de.TipoAlarme);
                para.TipoResposta = de.TipoResposta.NomeResposta;
                para.Cartao = de.NumeroCartao;
                para.DataHoraTransacao = de.DataTransacao.ToString("dd/MM/yyyy HH:mm:ss");
                para.DataHoraEnvioParaAnalise = de.DataEnvioAnalise.ToString("dd/MM/yyyy HH:mm:ss");
                para.Valor = de.Valor.ToString("C");
                para.Score = de.Score.ToString();
                para.Mcc = de.CodigoMCC + " - " + de.DescricaoMCC;
                para.Uf = de.UnidadeFederacao;
                para.TipoCartao = ObterTipoCartao(de.TipoCartao);
                para.Bandeira = de.Bandeira;
                para.Usuario = de.UsuarioAnalise;
                para.DataHoraAnalise = de.DataAnalise.ToString("dd/MM/yyyy HH:mm:ss");
                para.Comentario = de.ComentarioAnalise;

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.ConsultaTransacoesPorPeriodoUsuarioRelatorio> ObterConsultaTransacoesPorPeriodoUsuarioRelatorio(TransacaoEmissor[] lista)
        {
            List<Modelo.ConsultaTransacoesPorPeriodoUsuarioRelatorio> result = new List<Modelo.ConsultaTransacoesPorPeriodoUsuarioRelatorio>();

            foreach (TransacaoEmissor de in lista)
            {
                Modelo.ConsultaTransacoesPorPeriodoUsuarioRelatorio para = new Modelo.ConsultaTransacoesPorPeriodoUsuarioRelatorio();
                para.TipoAlarme = ObterTipoAlarme(de.TipoAlarme);
                para.TipoResposta = de.TipoResposta.NomeResposta;
                para.NumeroCartao = de.NumeroCartao;
                para.DataHoraTransacao = de.DataTransacao.ToString("dd/MM/yyyy HH:mm:ss");
                para.Valor = de.Valor.ToString("C");
                para.Score = de.Score.ToString();
                para.Mcc = de.CodigoMCC + " - " + de.DescricaoMCC;
                para.Uf = de.UnidadeFederacao;
                para.TipoCartao = ObterTipoCartao(de.TipoCartao);
                para.Bandeira = de.Bandeira;
                para.Usuario = de.UsuarioAnalise;
                para.DataHoraAnalise = de.DataAnalise.ToString("dd/MM/yyyy HH:mm:ss");
                para.Comentario = de.ComentarioAnalise;

                result.Add(para);
            }

            return result;
        }

        public static List<Modelo.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio> ObterTransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio(AgrupamentoTransacoesEstabelecimentoCartao[] lista)
        {
            List<Modelo.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio> result = new List<Modelo.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio>();

            foreach (AgrupamentoTransacoesEstabelecimentoCartao de in lista)
            {
                Modelo.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio para = new Modelo.TransacoesSuspeitasEstabelecimentoAgrupadasPorCartaoRelatorio();
                para.Cartao = UtilitariosHelper.ObterCartaoMascarado(de.NumeroCartao);
                para.ValorTotalTransacoesSuspeitas = de.ValorTotalTransacoes.ToString("C");
                para.QuantidadeTransacoesSuspeitas = de.QuantidadeTotalTransacoes.ToString();
                para.ValorTransacoesAprovadas = de.ValorTransacoesAprovadas.ToString("C");
                para.QuantidadeTransacoesAprovadas = de.QuantidadeTransacoesAprovadas.ToString();
                para.ValorTransacoesNegadas = de.ValorTransacoesSuspeitas.ToString("C");
                para.QuantidadeTransacoesNegadas = de.QuantidadeTransacoesSuspeitas.ToString();

                result.Add(para);
            }

            return result;
        }
        #endregion
    }
}
