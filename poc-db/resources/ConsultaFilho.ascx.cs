using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Emissores.Sharepoint.ArquivoEmissoresServico;
using System.ComponentModel;

namespace Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio
{
    public partial class ConsultaFilho : UserControlBase
    {
        static readonly DateTime INICIOVIGENCIA = new DateTime(2008, 08, 01);

        private static Int16 funcaoItem = 0;
        private static Int32 numeroItemPV = 0;
        private static decimal cnpjItem = 0;
        private static Int16 anoItem = 0;
        private static Int16 mesItem = 0;
        private static decimal faixaInicialFaturamentoItem;
        private static decimal faixaFinalFaturamentoItem;
        private static decimal fatorMultiplicadorItem;

        public delegate void Parametros(object[] parametros, EventArgs e);

        [Browsable(true)]
        public event Parametros OnItemSelecionado;


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void rptFilho_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                LinkButton lnkNumerooPV = e.Item.FindControl("lnkNumerooPV") as LinkButton;
                Literal ltlCpfCnpj = e.Item.FindControl("ltlCpfCnpj") as Literal;
                Literal ltlAgencia = e.Item.FindControl("ltlAgencia") as Literal;
                Literal ltlConta = e.Item.FindControl("ltlConta") as Literal;
                Literal ltlPeridodoTrava = e.Item.FindControl("ltlPeridodoTrava") as Literal;
                Literal ltlValorLiquido = e.Item.FindControl("ltlValorLiquido") as Literal;
                Literal ltlValorFinal = e.Item.FindControl("ltlValorFinal") as Literal;


                DetalheFilho item = e.Item.DataItem as DetalheFilho;
                if (!object.Equals(item, null))
                {


                    lnkNumerooPV.Text = item.NumeroPV.ToString();
                    ltlCpfCnpj.Text = item.CpfCNPJ;
                    ltlAgencia.Text = item.Agencia.ToString();
                    ltlConta.Text = item.ContaCorrente.ToString();
                    ltlPeridodoTrava.Text = item.PeriodoTrava;
                    ltlValorLiquido.Text = item.ValorLiquido.ToString("N2");
                    ltlValorFinal.Text = item.ValorFinal.ToString("N2");



                    lnkNumerooPV.CommandArgument = string.Format("{0}", item.NumeroPV);
                }

            }
        }
        protected void lnkNumerooPV_onClick(object sender, EventArgs e)
        {
            LinkButton lnkNumerooPV = sender as LinkButton;

            if (!object.Equals(lnkNumerooPV, null))
            {
                if (!string.IsNullOrEmpty(lnkNumerooPV.CommandArgument))
                {

                    Int32 pv = lnkNumerooPV.CommandArgument.ToInt32Null(0).Value;

                    object[] parametros = new object[] { funcaoItem, pv, cnpjItem, anoItem, mesItem, faixaInicialFaturamentoItem, faixaFinalFaturamentoItem, fatorMultiplicadorItem };

                    if (!object.ReferenceEquals(this.OnItemSelecionado, null))
                        this.OnItemSelecionado(parametros, e);
                }

            }
        }

        public void CarregarDetalheFilho(Guid guidPesquisa, DateTime inicioVigencia, Int16 funcao, Int16 ano, Int16 mes, Int32 numeroPV, decimal cnpj, decimal faixaInicialFaturamento, decimal faixaFinalFaturamento, decimal fatorMultiplicador)
        {
            pgnFilho.IdPesquisa = guidPesquisa;

            funcaoItem = funcao;
            anoItem = ano;
            mesItem = mes;
            numeroItemPV = numeroPV;
            cnpjItem = cnpj;
            faixaInicialFaturamentoItem = faixaInicialFaturamento;
            faixaFinalFaturamentoItem = faixaFinalFaturamento;
            fatorMultiplicadorItem = fatorMultiplicador;

            pgnFilho.Carregar(inicioVigencia, funcao, ano, mes, numeroPV, cnpj, faixaInicialFaturamento, faixaFinalFaturamento, fatorMultiplicador);
        }

        protected IEnumerable<Object> pgnFilho_ObterDados(Guid idPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            Logger.IniciarLog("pgnFilho_ObterDados");
            //Objetos de retorno
            IEnumerable<Object> retorno = new List<Object>();
            quantidadeTotalRegistrosEmCache = 0;
            try
            {
                Int16 codRetorno = 0;
                using (var contexto = new ContextoWCF<HisServicoZpEmissores.HisServicoZpEmissoresClient>())
                {
                    List<DetalheFilho> lstFilho = new List<DetalheFilho>();
                    String mensagemRetorno = string.Empty;

                    //DateTime inicioVigencia;
                    //Int16 funcao;
                    //Int16 ano;
                    //Int16 mes;
                    //Int32 numeroPV;
                    //decimal cnpj;
                    //decimal faixaInicialFaturamento;
                    //decimal faixaFinalFaturamento;
                    //decimal fatorMultiplicador;


                    //inicioVigencia, funcao, ano, mes, numeroPV, cnpj, faixaInicialFaturamento, faixaFinalFaturamento, fatorMultiplicador 

                    if (!object.Equals(parametros, null) && parametros.Length > 0)
                    {
                        //inicioVigencia = parametros[0].ToString().ToDate();
                        funcaoItem = parametros[1].ToString().ToInt16();
                        anoItem = parametros[2].ToString().ToInt16();
                        mesItem = parametros[3].ToString().ToInt16();
                        numeroItemPV = parametros[4].ToString().ToInt16();
                        cnpjItem = parametros[5].ToString().ToDecimalNull(0).Value;
                        faixaInicialFaturamentoItem = parametros[6].ToString().ToDecimalNull(0).Value;
                        faixaFinalFaturamentoItem = parametros[7].ToString().ToDecimalNull(0).Value;
                        fatorMultiplicadorItem = parametros[8].ToString().ToDecimalNull(0).Value;



                        //qdo periodo selecionado menor que o inicio vigencia
                        if (INICIOVIGENCIA.CompareTo(new DateTime(anoItem, mesItem, 1)) > 0)
                        {
                            Logger.GravarLog("HisServicoZpEmissores.ConsultarInformacoesPVCobranca", new
                            {
                                idPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeTotalRegistrosEmCache,
                                Funcao = funcaoItem,
                                NumeroPV = numeroItemPV,
                                Cnpj = cnpjItem,
                                SessaoAtual.CodigoEntidade,
                                Ano = anoItem,
                                Mes = mesItem,
                                FaixaInicialFaturamento = faixaInicialFaturamentoItem,
                                FaixaFinalFaturamento = faixaFinalFaturamentoItem,
                                FatorMultiplicador = fatorMultiplicadorItem
                            });

                            List<HisServicoZpEmissores.InformacaoPVCobrada> lstRetorno = contexto.Cliente.ConsultarInformacoesPVCobranca(
                                idPesquisa, registroInicial, quantidadeRegistros, ref quantidadeTotalRegistrosEmCache,
                                out codRetorno, out mensagemRetorno,
                                funcaoItem, numeroItemPV, cnpjItem, string.Empty, string.Empty, (Int16)SessaoAtual.CodigoEntidade, 1, anoItem, mesItem, faixaInicialFaturamentoItem,
                                faixaFinalFaturamentoItem, fatorMultiplicadorItem);

                            if (codRetorno != 0)
                            {
                                base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultarInformacoesPVCobranca", codRetorno);
                            }
                            lstFilho.AddRange(lstRetorno.Select(item => new DetalheFilho()
                            {
                                Agencia = item.CodigoAgencia,
                                ContaCorrente = item.NumeroConta.ToInt32(0),
                                CpfCNPJ = item.Cnpj.ToString(),
                                NumeroPV = item.NumeroPV,
                                PeriodoTrava = String.Format("{0} a {1}", item.DataInicial, item.DataFinal),
                                ValorFinal = item.ValCobranca,
                                ValorLiquido = item.ValorFaturamento
                            }));
                        }
                        else
                        {
                            Logger.GravarLog("HisServicoZpEmissores.ConsultarInformacoesDetalhadas", new
                            {
                                idPesquisa,
                                registroInicial,
                                quantidadeRegistros,
                                quantidadeTotalRegistrosEmCache,
                                SessaoAtual.CodigoEntidade,
                                anoItem,
                                mesItem,
                                faixaInicialFaturamentoItem,
                                faixaFinalFaturamentoItem
                            });
                            List<HisServicoZpEmissores.InformacaoDetalhada> lstRetorno = contexto.Cliente.ConsultarInformacoesDetalhadas(idPesquisa, registroInicial, quantidadeRegistros,
                                ref quantidadeTotalRegistrosEmCache, out codRetorno, out mensagemRetorno,
                                (Int16)SessaoAtual.CodigoEntidade, 1, anoItem, mesItem, faixaInicialFaturamentoItem, faixaFinalFaturamentoItem);

                            if (codRetorno != 0)
                            {
                                base.ExibirPainelExcecao("HisServicoZpEmissores.ConsultarInformacoesDetalhadas", codRetorno);
                            }

                            lstFilho.AddRange(lstRetorno.Select(item => new DetalheFilho()
                            {
                                Agencia = item.CodigoAgencia,
                                ContaCorrente = item.ContaCorrente.ToInt32(0),
                                CpfCNPJ = item.Cnpj.ToString(),
                                NumeroPV = item.NumeroPV,
                                PeriodoTrava = String.Format("{0} a {1}", item.PeriodoInicial, item.PeriodoFinal),
                                ValorFinal = item.ValorFinalCobranca,
                                ValorLiquido = item.ValorLiquido
                            }));
                        }
                    }
                    retorno = lstFilho.Cast<object>();

                }
            }
            catch (FaultException<GeneralFault> ex)
            {
                Logger.GravarErro("pgnFilho_ObterDados - ", ex);
                ExibirPainelExcecao(ex.Message, 1);
                SharePointUlsLog.LogErro(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("pgnFilho_ObterDados - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            return retorno;
        }
    }
}
