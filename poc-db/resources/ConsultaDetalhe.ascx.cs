using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum;
using System.Linq;
using System.ServiceModel;
using System.ComponentModel;

namespace Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio
{
    public partial class ConsultaDetalhe : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void rptDetalhe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DetalheTrava item = e.Item.DataItem as DetalheTrava;

                Literal ltlPV = e.Item.FindControl("ltlPV") as Literal;
                Literal ltlCpfCnpj = e.Item.FindControl("ltlCpfCnpj") as Literal;
                Literal ltlAgencia = e.Item.FindControl("ltlAgencia") as Literal;
                Literal ltlConta = e.Item.FindControl("ltlConta") as Literal;
                Literal ltlPeridodoTrava = e.Item.FindControl("ltlPeridodoTrava") as Literal;
                Literal ltlDiasTrava = e.Item.FindControl("ltlDiasTrava") as Literal;
                Literal ltlValorLiquido = e.Item.FindControl("ltlValorLiquido") as Literal;
                Literal ltlValorCobranca = e.Item.FindControl("ltlValorCobranca") as Literal;
                Literal ltlTipoConta = e.Item.FindControl("ltlTipoConta") as Literal;

                ltlPV.Text = item.NumeroPV.ToString();
                ltlCpfCnpj.Text = item.CpfCnpj;
                ltlAgencia.Text = item.Agencia.ToString();
                ltlConta.Text = item.Conta;
                ltlDiasTrava.Text = item.DiasTrava;
                ltlPeridodoTrava.Text = item.PeriodoTrava;
                ltlValorLiquido.Text = item.ValorLiquido.ToString("N2");
                ltlValorCobranca.Text = item.ValorCobranca.ToString("N2");
                ltlTipoConta.Text = item.TipoConta;
            }

        }
        protected IEnumerable<Object> pgnDetalhe_ObterDados(Guid idPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer,
            out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {

            Logger.IniciarLog("pgnDetalhe_ObterDados");
            IEnumerable<Object> lstDadosRetorno = new List<Object>();
            quantidadeTotalRegistrosEmCache = 0;
            try
            {
                Int16 codRetorno = 0;
                String mensgemRetorno = string.Empty;


                DateTime inicioVigencia;
                Int16 funcao = 0;
                Int32 numeroPv = 0;
                Decimal cnpj = 0;
                String dataDe = string.Empty;
                String dataAte = string.Empty;
                Int16 codBanco = 0;
                Int32 codProduto = 0;
                Int16 anoComp = 0;
                Int16 mesComp = 0;
                Decimal faixaInicialFaturamento = 0;
                Decimal faixaFinalFaturamento = 0;
                Decimal fatorMultiplicador = 0;

                Logger.IniciarLog("ConsultaDetalhe - CarregarDetalhe");

                List<DetalheTrava> lstDados = new List<DetalheTrava>();

                using (var contexto = new ContextoWCF<HisServicoZpEmissores.HisServicoZpEmissoresClient>())
                {
                    if (!object.Equals(parametros, null) && parametros.Length > 0)
                    {
                        inicioVigencia = parametros[0].ToString().ToDate();
                        funcao = parametros[1].ToString().ToInt16();
                        numeroPv = parametros[2].ToString().ToInt16();
                        cnpj = parametros[3].ToString().ToDecimalNull(0).Value;
                        dataDe = parametros[4].ToString();
                        codBanco = parametros[5].ToString().ToInt16Null(0).Value;
                        codProduto = parametros[5].ToString().ToInt32Null(0).Value;
                        dataAte = parametros[5].ToString();
                        anoComp = parametros[6].ToString().ToInt16();
                        mesComp = parametros[7].ToString().ToInt16();
                        faixaInicialFaturamento = parametros[8].ToString().ToDecimalNull(0).Value;
                        faixaFinalFaturamento = parametros[9].ToString().ToDecimalNull(0).Value;
                        fatorMultiplicador = parametros[10].ToString().ToDecimalNull(0).Value;


                        if (inicioVigencia.CompareTo(new DateTime(anoComp, mesComp, 1)) > 0)
                        {
                            Logger.GravarLog("HisServicoZpEmissores.ConsultarInformacoesPVCobranca", new
                            {
                                idPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeTotalRegistrosEmCache,
                                funcao,
                                numeroPv,
                                cnpj,
                                dataDe,
                                dataAte,
                                codBanco,
                                codProduto,
                                anoComp,
                                mesComp,
                                faixaInicialFaturamento,
                                faixaFinalFaturamento,
                                fatorMultiplicador
                            });
                            //ConsultarInformacoesPVCobranca 381
                            List<HisServicoZpEmissores.InformacaoPVCobrada> lstRetorno = contexto.Cliente.ConsultarInformacoesPVCobranca(idPesquisa, registroInicial,
                                quantidadeRegistros, ref quantidadeTotalRegistrosEmCache,
                                out codRetorno, out mensgemRetorno, funcao, numeroPv, cnpj, dataDe, dataAte, codBanco, codProduto,
                                anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento, fatorMultiplicador);
                            if (codRetorno != 0)
                            {
                                base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultarInformacoesPVCobranca", codRetorno);
                            }

                            lstDados.AddRange(lstRetorno.Select(item => new DetalheTrava()
                            {

                                Agencia = item.CodigoAgencia,
                                Conta = item.NumeroConta,
                                CpfCnpj = item.Cnpj.ToString(),
                                DiasTrava = item.QuantidadeDias.ToString(),
                                NumeroPV = item.NumeroPV,
                                PeriodoTrava = String.Format("{0} a {1}", item.DataInicial, item.DataFinal),
                                TipoConta = item.Tipo,
                                ValorCobranca = item.ValorFaturamento,
                                ValorLiquido = item.ValCobranca

                            }));

                        }
                        else
                        {
                            Logger.GravarLog("HisServicoZpEmissores.ConsultarDetalheFatura", new
                            {
                                idPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeTotalRegistrosEmCache,
                                codBanco,
                                codProduto,
                                anoComp,
                                mesComp,
                                faixaInicialFaturamento,
                                faixaFinalFaturamento,
                                numeroPv
                            });
                            // ConsultarDetalheFatura 384
                            List<HisServicoZpEmissores.DetalheFatura> lstRetorno = contexto.Cliente.ConsultarDetalheFatura(idPesquisa, registroInicial, quantidadeRegistros,
                                ref quantidadeTotalRegistrosEmCache, out codRetorno, out mensgemRetorno, codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento,
                                faixaFinalFaturamento, numeroPv);
                            if (codRetorno != 0)
                            {
                                base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultarDetalheFatura", codRetorno);
                            }

                            lstDados.AddRange(lstRetorno.Select(item => new DetalheTrava()
                            {

                                Agencia = item.CodigoAgencia,
                                Conta = item.ContaCorrente,
                                CpfCnpj = item.Cnpj.ToString(),
                                DiasTrava = item.QuantidadeDias.ToString(),
                                NumeroPV = item.NumeroPV,
                                PeriodoTrava = String.Format("{0} a {1}", item.PeriodoInicial, item.PeriodoFinal),
                                TipoConta = item.Tipo,
                                ValorCobranca = 0,
                                ValorLiquido = item.ValorLiquido

                            }));
                        }

                        lstDadosRetorno = lstDados.Cast<object>();
                    }

                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("pgnDetalhe_ObterDados - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("pgnDetalhe_ObterDados - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            return lstDadosRetorno;
        }

        public void CarregarDetalhe(Guid guidPesquisa, DateTime inicioVigencia, Int16 funcao, Int32 numeroPV, decimal cnpj, string datade,
            string datate, Int16 ano, Int16 mes, decimal faixaInicialFaturamento, decimal faixaFinalFaturamento, decimal fatorMultiplicador)
        {
            pgnDetalhe.IdPesquisa = guidPesquisa;
            pgnDetalhe.Carregar(inicioVigencia, funcao, numeroPV, cnpj, datade, datate, ano, mes, faixaInicialFaturamento, faixaFinalFaturamento, fatorMultiplicador);
        }
    }
}
