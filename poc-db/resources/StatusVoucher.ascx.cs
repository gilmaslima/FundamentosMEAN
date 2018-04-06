using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Rede.PN.MultivanAlelo.Sharepoint.GEProdutoParceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Rede.PN.MultivanAlelo.Core.Web.Controles.Portal;
using System.Web.UI;

namespace Rede.PN.MultivanAlelo.Sharepoint.ControlTemplates.Voucher
{
    public partial class StatusVoucher : UserControlBase
    {
        #region [Propriedades]

        /// <summary>
        /// Quantidade de Itens por página
        /// </summary>
        private Int32 QuantidadeItensPagina
        {
            get { return Int32.MaxValue; }
        }

        #endregion

        #region [Eventos do controle]

        /// <summary>
        /// Carregamento do controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                qdAviso.Visible = false;
                consultaPV.Width = new Unit(135);
            }
        }

        /// <summary>
        /// Realizar a consulta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Realizar a consulta de status dos vouchers de solicitações da Alelo"))
            {
                try
                {
                    if (Sessao.Contem())
                    {
                        DateTime dataInicial = default(DateTime);
                        DateTime dataFinal = default(DateTime);

                        if (!DateTime.TryParse(txtDataInicio.Text, out dataInicial)
                            || !DateTime.TryParse(txtDataTermino.Text, out dataFinal))
                        {
                            base.ExibirPainelExcecao("Período da solicitação é inválido.", CODIGO_ERRO);
                            return;
                        }

                        String cnpjCpf = String.Empty;
                        String radicalCnpjCpf = String.Empty;

                        if (!String.IsNullOrWhiteSpace(txtCnpjCpf.Text))
                        {
                            cnpjCpf = txtCnpjCpf.Text.RemoverCaracteresEspeciais();
                            radicalCnpjCpf = cnpjCpf.Substring(0, 7);

                            String radicalEstabelecimentoCorrente = SessaoAtual.CNPJEntidade.Substring(0, 7);
                            if (!radicalEstabelecimentoCorrente.Equals(radicalCnpjCpf))
                            {
                                pnlResultado.Visible = false;
                                qdAviso.Visible = true;
                                qdAviso.Titulo = "Aviso";
                                qdAviso.Mensagem = "O CNPJ inserido não é matriz ou filial.";
                                qdAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                            }
                            else
                                this.CarregarSolicitacoes(consultaPV.PVsSelecionados, cnpjCpf.ToInt64Null(), dataInicial, dataFinal);

                        }
                        else
                        {
                            if (consultaPV.PVsSelecionados == null || consultaPV.PVsSelecionados.Count <= 0)
                            {
                                pnlResultado.Visible = false;
                                qdAviso.Visible = true;
                                qdAviso.Titulo = "Aviso";
                                qdAviso.Mensagem = "Selecione ao menos um ponto de venda, matriz ou filial.";
                                qdAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                            }
                            else
                                this.CarregarSolicitacoes(consultaPV.PVsSelecionados, null, dataInicial, dataFinal);
                        }
                    }
                }
                catch (FaultException<GEProdutoParceiro.ModelosErroServicos> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    if (ex.Detail.CodErro.HasValue)
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.CodErro.Value);
                    else
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Fechar a consulta de solicitações
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Fechar a consulta de solicitações"))
            {
                try
                {
                    Response.Redirect("/sites/fechado/servicos/Paginas/pn_ConsultaSolicitacoes.aspx", true);
                }
                catch (NullReferenceException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Renderizar as informações da solicitação de voucher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repSolicitacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Renderizar as informações da solicitação de voucher"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                    {
                        if (e.Item.DataItem != null)
                        {
                            Literal ltrDataHora = e.Item.FindControl("ltrDataHora") as Literal;
                            Literal ltrCpfCnpj = e.Item.FindControl("ltrCpfCnpj") as Literal;
                            Literal ltrCredenciamento = e.Item.FindControl("ltrCredenciamento") as Literal;
                            Literal ltrBandeira = e.Item.FindControl("ltrBandeira") as Literal;
                            Literal ltrStatus = e.Item.FindControl("ltrStatus") as Literal;
                            Literal ltrNumEstabelecimento = e.Item.FindControl("ltrNumEstabelecimento") as Literal;

                            SolicitacaoVoucher solicitacao = e.Item.DataItem as SolicitacaoVoucher;

                            ltrDataHora.Text = String.Format("{0} - {1}",
                                                            solicitacao.DataHora.ToString("dd/MM/yyyy"),
                                                            solicitacao.DataHora.ToString("HH:mm"));
                            ltrCredenciamento.Text = solicitacao.TipoCredenciamento;
                            ltrBandeira.Text = solicitacao.Bandeira;
                            ltrStatus.Text = solicitacao.Status;

                            String cnpj = String.Format(@"{0:00\.000\.000\/0000\-00}", solicitacao.CpfCnpj);

                            ltrNumEstabelecimento.Text = solicitacao.NumeroEstabelecimento.ToString();
                            ltrCpfCnpj.Text = cnpj;
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Carregar Solicitações no repeater
        /// </summary>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        private void CarregarSolicitacoes(List<Int32> codigosEstabelecimento, Int64? cnpjCpf, DateTime dataInicial, DateTime dataFinal)
        {
            using (var servico = new ContextoWCF<ServicoPortalGEProdutoParceiroClient>())
            {
                List<ProdutoParceiro> listaProdutoParceiro = new List<ProdutoParceiro>();

                dataFinal = dataFinal.AddDays(1).AddMilliseconds(-1);
                Int32 registroInicial = 1;
                Int32 registroFinal = QuantidadeItensPagina;

                List<Int64> pdvs = new List<Int64>();

                pdvs = cnpjCpf.HasValue ? null : codigosEstabelecimento.Select(p => (Int64)p).ToList();

                //Método retorna dados de produto do Parceiro
                listaProdutoParceiro = servico.Cliente.ConsultaProdutoParceiro(pdvs, cnpjCpf, dataInicial, dataFinal, registroInicial, registroFinal);

                if (listaProdutoParceiro != null && listaProdutoParceiro.Count > 0)
                {
                    List<SolicitacaoVoucher> solicitacoes = this.SelecionarSolicitacoes(listaProdutoParceiro);

                    if (solicitacoes != null && solicitacoes.Count > 0)
                    {
                        repSolicitacoes.DataSource = solicitacoes;
                        repSolicitacoes.DataBind();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblSolicitacoes', 1, 10, 5);", true);

                        qdAviso.Visible = false;
                        pnlResultado.Visible = true;
                    }
                    else
                    {
                        pnlResultado.Visible = false;
                        qdAviso.Visible = true;
                        qdAviso.Titulo = "Aviso";
                        qdAviso.Mensagem = "Não foram encontradas solicitações de Credenciamento Voucher.";
                        qdAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                    }
                }
                else
                {
                    pnlResultado.Visible = false;
                    qdAviso.Visible = true;
                    qdAviso.Titulo = "Aviso";
                    qdAviso.Mensagem = "Não foram encontradas solicitações de Credenciamento Voucher.";
                    qdAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                }
            }
        }

        /// <summary>
        /// Seleciona os dados de Produtos Parceito para gerar um modelo apenas de Solicitações
        /// </summary>
        /// <param name="produtos">Lista de Produtos Parceiro</param>
        /// <returns>Lista de Solicitações</returns>
        private List<SolicitacaoVoucher> SelecionarSolicitacoes(List<ProdutoParceiro> produtos)
        {
            List<SolicitacaoVoucher> solicitacoes = new List<SolicitacaoVoucher>();

            foreach (ProdutoParceiro produto in produtos)
            {
                if (produto.Produto != null)
                {
                    solicitacoes.Add(new SolicitacaoVoucher()
                    {
                        DataHora = produto.DataSolicitacaoCadastro,
                        CpfCnpj = produto.NumeroCPFCNPJ.Value,
                        NumeroEstabelecimento = produto.NumeroPDV,
                        TipoCredenciamento = produto.Produto.DescricaoCca,
                        Bandeira = produto.Produto.DescricaoFeature,
                        Status = produto.DescricaoSituacaoRegistro
                    });
                }
            }

            return solicitacoes;
        }
        #endregion
    }
}
