using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;

namespace Redecard.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Redecard.PN.Credenciamento
{
    public partial class ConfirmacaoDadosPJSimples : UserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Dados do Credenciamento que persistem na Sessão
        /// </summary>
        public Modelo.Credenciamento Credenciamento
        {
            get
            {
                if (Session["RedecardPNCredenciamento"] == null)
                    Session["RedecardPNCredenciamento"] = new Modelo.Credenciamento();

                return (Modelo.Credenciamento)Session["RedecardPNCredenciamento"];
            }
            set
            {
                Session["RedecardPNCredenciamento"] = value;
            }
        }

        #endregion

        #region [ Eventos da Página ]

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;
            if (!IsPostBack)
            {
                CarregaDados();
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        protected void rptProprietarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTituloProprietario = (Literal)e.Item.FindControl("ltlTituloProprietario");
                Literal ltlProprietario = (Literal)e.Item.FindControl("ltlProprietario");

                ltlTituloProprietario.Visible = (e.Item.ItemIndex == 0);

                String proprietario = ((Proprietario)e.Item.DataItem).Nome + " - " + ((Proprietario)e.Item.DataItem).Participacao + "%";
                ltlProprietario.Text = proprietario;
            }
        }

        protected void rptVendaCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxas = (Literal)e.Item.FindControl("ltlTaxas");
                Literal ltlQtdMinParcelas = (Literal)e.Item.FindControl("ltlQtdMinParcelas");
                Literal ltlQtdMAxParcelas = (Literal)e.Item.FindControl("ltlQtdMAxParcelas");


                //TODO: Alterar quando chegarem os serviços e os modelos corretos
                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlTipoVenda.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = item.ValorPrazoDefault.ToString();
                ltlTaxas.Text = item.ValorTaxaDefault.ToString();
                ltlQtdMinParcelas.Text = item.QtdeDefaultParcela.ToString();
                ltlQtdMAxParcelas.Text = item.QtdeMaximaParcela.ToString();
            }

        }

        protected void rptVendasDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxas = (Literal)e.Item.FindControl("ltlTaxas");
                Literal ltlQtdMinParcelas = (Literal)e.Item.FindControl("ltlQtdMinParcelas");
                Literal ltlQtdMAxParcelas = (Literal)e.Item.FindControl("ltlQtdMAxParcelas");

                //TODO: Alterar quando chegarem os serviços e os modelos corretos

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlTipoVenda.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = item.ValorPrazoDefault.ToString();
                ltlTaxas.Text = item.ValorTaxaDefault.ToString();
                ltlQtdMinParcelas.Text = item.QtdeDefaultParcela.ToString();
                ltlQtdMAxParcelas.Text = item.QtdeMaximaParcela.ToString();
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                WFProposta.RetornoErro erroProposta = AtualizaSituacaoProposta();

                if (erroProposta.CodigoErro == 0)
                {
                    //Limpa Sessão
                    Session["PassoMax"] = 0;

                    // Response.Redirect("pn_");
                    // oculta painel de dados e botões de voltar e confirmar
                    pnlDados.Visible = false;
                    spnConfirmar.Visible = false;
                    spnVoltar.Visible = false;
                    // TODO:Carregar dados para o painel de confirmação


                    //exibe painel de confirmação e botão de nova proposta e imprimir
                    spnImprimir.Visible = true;
                    spnNovaProposta.Visible = true;
                    pnlConfirmacao.Visible = true;
                }
                else
                    base.ExibirPainelExcecao(erroProposta.DescricaoErro, erroProposta.CodigoErro.ToString());
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnPropostas_Click(object sender, EventArgs e)
        {
            //TODO:Implementar exibição de proposta, falta layout

            //   Response.Redirect("");
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            //TODO: Implementar impressão
        }

        protected void btnNovaProposta_Click(object sender, EventArgs e)
        {
            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx");
        }

        #endregion

        #region [ Métodos Auxiliares ]

        private void CarregaDados()
        {
            if (Credenciamento.TipoPessoa == "J")
                ltlCnpj.Text = Credenciamento.CNPJ;
            ltlRazaoSocial.Text = Credenciamento.RazaoSocial;
            ltlCEP.Text = Credenciamento.EnderecoComercial.CEP;
            ltlEndereco.Text = Credenciamento.EnderecoComercial.Logradouro;
            ltlNumero.Text = Credenciamento.EnderecoComercial.Numero;
            ltlBairro.Text = Credenciamento.EnderecoComercial.Bairro;
            ltlCidade.Text = Credenciamento.EnderecoComercial.Cidade;
            ltlUF.Text = Credenciamento.EnderecoComercial.Estado;

            ltlEquipamento.Text = Credenciamento.CodTipoEquipamento;
            ltlQuantidade.Text = Credenciamento.QtdeTerminaisSolicitados.ToString();
            ltlValorAluguel.Text = String.Format(@"{0:c}", Credenciamento.ValorAluguel);

            rptProprietarios.DataSource = Credenciamento.Proprietarios;
            rptProprietarios.DataBind();

            rptVendaCredito.DataSource = Credenciamento.ProdutosCredito;
            rptVendaCredito.DataBind();

            rptVendasDebito.DataSource = Credenciamento.ProdutosDebito;
            rptVendasDebito.DataBind();
        }

        /// <summary>
        /// Atualiza a situação da proposta
        /// </summary>
        private WFProposta.RetornoErro AtualizaSituacaoProposta()
        {
            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    using (ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient())
                    {
                        Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                        Int64 numCNPJ = 0;
                        if (Credenciamento.TipoPessoa == "J")
                            Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                        else
                            Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                        Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                        Char indSituacaoProposta = 'P';
                        String usuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                        Int32? codMotivoRecusa = null;
                        Int32 numeroPontoVenda = 0;
                        Int32? indOrigemAtualizacao = 1;

                        WFProposta.RetornoErro[] retorno = client.AtualizaSituacaoProposta(
                            codTipoPessoa,
                            numCNPJ,
                            numSeqProp,
                            indSituacaoProposta,
                            usuarioUltimaAtualizacao,
                            codMotivoRecusa,
                            numeroPontoVenda,
                            indOrigemAtualizacao
                            );

                        return retorno[0];
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                throw fe;
            }
            catch (PortalRedecardException pe)
            {
                throw pe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
