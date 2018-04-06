using Rede.PN.CondicaoComercial.Core.Web.Controles.Portal;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.DadosBancarios
{
    public partial class DadosBancariosVoucherUserControl : UserControlBase
    {
        /// <summary>
        /// Relação das bandeiras que devem ser removidas da listagem
        /// </summary>
        private readonly Int32[] listBandeirasRemover = new Int32[] {

            // bandeiras Elo e Amex,
            //69, 70, 71,

            // bandeiras Redeshop e Avisa
            20, 22, 67,

            // bandeiras internacionais
            2, 4
        };

        /// <summary>
        /// Imagens das bandeiras
        /// </summary>
        private List<Business.BandeiraImagem> BandeirasImagens
        {
            get
            {
                if (ViewState["BandeirasImagens"] == null)
                    ViewState["BandeirasImagens"] = Business.ListaImagensBandeira.ObterImagensBandeiras();
                return (List<Business.BandeiraImagem>)ViewState["BandeirasImagens"];
            }
            set { ViewState["BandeirasImagens"] = value; }
        }
        /// <summary>
        /// Inicialiazação da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Voucher - Carregando página"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        CarregarDados();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
                }
            }
        }

        /// <summary>
        /// Carrega os Dados Bancários de Crédito na tela
        /// </summary>
        private void CarregarDados()
        {
            using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
            {
                //Código de retorno de erro de negócio na proc
                Int32 codRetornoDadosBancarios;

                //Consulta os dados bancários de Voucher da Entidade. 
                //Tipo dados "C" representa Crédito. "D" para buscar somente débito. "V" para buscar somente voucher
                var dadosBancariosVoucher = entidadeServico.ConsultarDadosBancarios(out codRetornoDadosBancarios, base.SessaoAtual.CodigoEntidade, "V");

                if (dadosBancariosVoucher.Length != 0)
                {
                    // Caso o código de retorno seja <> de 0 ocorreu um erro
                    if (codRetornoDadosBancarios > 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosBancarios", codRetornoDadosBancarios);
                    else
                    {
                        var bandeiras = MontarItensVoucher(dadosBancariosVoucher);

                        rptVoucher.DataSource = bandeiras;
                        rptVoucher.DataBind();

                        hdnContemRegistrosVoucher.Value = "1";
                    }
                }
                else
                {
                    pnlVoucher.Visible = false;
                    qdAvisoDadosBancarios.Visible = true;
                    qdAvisoDadosBancarios.TipoQuadro = TipoQuadroAviso.Aviso;
                    qdAvisoDadosBancarios.Mensagem = "Não há bandeiras contratadas para este estabelecimento";
                }
            }
        }

        private List<Business.Bandeira> MontarItensVoucher(EntidadeServico.DadosBancarios[] bandeirasVoucher)
        {
            var bandeirasVoucherLista = bandeirasVoucher.OrderBy(x =>
            {
                /* 
                 * JFR: define a ordenação das bandeiras, sendo:
                 * - 1º master
                 * - 2º visa
                 * - 3º demais */

                Int32 order = 2;

                if (x.DescricaoCartao.ToLower().Contains("master"))
                    order = 0;

                else if (x.DescricaoCartao.ToLower().Contains("visa"))
                    order = 1;

                return order;

            }).ToList();

            // SKU: Nao deve conter as bandeiras AMEX(69), ELO(70) e ELO(71)
            // JFR: não deve conter bandeiras internacionais
            bandeirasVoucherLista.RemoveAll(x => listBandeirasRemover.Any(b => String.Compare(x.CodigoCartao, b.ToString()) == 0));

            bandeirasVoucher = bandeirasVoucherLista.ToArray();

            List<Business.Bandeira> bandeiras = new List<Rede.PN.CondicaoComercial.SharePoint.Business.Bandeira>();

            Business.Bandeira bandeira = null;
            foreach (EntidadeServico.DadosBancarios dado in bandeirasVoucher)
            {
                if (!dado.CodigoCartao.ToInt32().Equals(61) && !dado.CodigoFEAT.ToInt32().Equals(88))
                {
                    // se é a primeira vez ou se ja nao é a sequencia da mesma bandeira
                    if ((bandeira == null) || (bandeira.Codigo != dado.CodigoCartao.ToInt32()))
                    {
                        if (bandeira != null)
                            bandeiras.Add(bandeira);

                        bandeira = new Business.Bandeira();
                        bandeira.Codigo = String.Format("{0}{1}", dado.CodigoCartao, dado.CodigoFEAT).ToInt32();
                    }

                    bandeira.Nome = dado.DescricaoFEAT;
                }
            }

            // adiciona a ultima bandeira
            if (bandeira != null)
                bandeiras.Add(bandeira);

            return bandeiras;
        }

        protected void rptVoucher_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Business.Bandeira bandeira = (Business.Bandeira)e.Item.DataItem;
                Business.BandeiraImagem imagem = this.BandeirasImagens.FirstOrDefault(b => b.Codigo == bandeira.Codigo);

                Image imgBandeira = (Image)e.Item.FindControl("imgBandeira");
                Literal ltrNomeBandeira = (Literal)e.Item.FindControl("ltrNomeBandeira");

                if (imagem != null)
                {
                    imgBandeira.ImageUrl = imagem.Url;
                    ltrNomeBandeira.Text = imagem.Descricao;
                }
                else
                {
                    imgBandeira.Visible = false;
                    ltrNomeBandeira.Text = bandeira.Nome;
                }
            }
        }
    }
}
