using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Microsoft.SharePoint;
using System.Linq;

namespace Redecard.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Redecard.PN.Credenciamento
{
    public enum TipoNavegacao
    {
        Credenciamento,
        Autocredenciamento
    }
    public partial class NavegacaoCredenciamento : UserControlBase
    {
        
        class ItemNavegacao
        {
            public String Nome { get; set; }
            public String Url { get; set; }

        }

        /// <summary>
        /// Etapa atual do credenciamento
        /// </summary>
        public Int32 Passo { get; set; }

        /// <summary>
        /// Etapa máxima que o usuário preencheu
        /// </summary>
        public Int32 PassoMax
        {
            get
            {
                if (Session["PassoMax"] == null)
                    return 1;

                return (Int32)Session["PassoMax"];
            }
            set
            {
                Session["PassoMax"] = value;
            }
        }

        public TipoNavegacao TipoNavegacao
        {
            get
            {
                if (ViewState["TipoNavegacao"] == null)
                    return TipoNavegacao.Credenciamento;
                else
                    return (TipoNavegacao)ViewState["TipoNavegacao"];

            }
            set { ViewState["TipoNavegacao"] = value; }
        }


        private List<ItemNavegacao> ItensCredenciamento = new List<ItemNavegacao>() { 
            new ItemNavegacao(){Nome="Dados<br />Iniciais",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadosiniciais.aspx"},
            new ItemNavegacao(){Nome="Dados<br />do Cliente",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadoscliente.aspx"},
            new ItemNavegacao(){Nome="Dados<br />do Negócio",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadosnegocio.aspx"},
            new ItemNavegacao(){Nome="Dados<br />do Endereço",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadosendereco.aspx"},
            new ItemNavegacao(){Nome="Dados<br />Operacionais",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadosoperacionais.aspx"},
            new ItemNavegacao(){Nome="Dados<br />Bancários",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_dadosbancarios.aspx"},
            new ItemNavegacao(){Nome="Dados de<br />Tecnologia",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_escolhatecnologia.aspx"},
            new ItemNavegacao(){Nome="Contratação<br />de Serviços",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_contracaoservicos.aspx"},
            new ItemNavegacao(){Nome="Confirmação<br />de Dados",Url=SPContext.Current.Web.ServerRelativeUrl+ "/paginas/pn_confirmacaodados.aspx"}
        
        };

        private List<ItemNavegacao> ItensAutCredenciamento = new List<ItemNavegacao>() { 
            new ItemNavegacao(){ Nome="Dados<br />Iniciais",            Url="/_layouts/autocredenciamento/DadosIniciais.aspx"},
            new ItemNavegacao(){ Nome="Escolha de<br />Tecnologia",     Url="/_layouts/autocredenciamento/EscolhaTecnologia.aspx"},
            new ItemNavegacao(){ Nome="Dados<br />do Cliente",          Url="/_layouts/autocredenciamento/DadosCliente.aspx"},
            new ItemNavegacao(){ Nome="Dados<br />Bancários",           Url="/_layouts/autocredenciamento/DadosBancarios.aspx"},
            new ItemNavegacao(){ Nome="Quantidade<br/>de Terminais",    Url="/_layouts/autocredenciamento/QuantidadeTerminais.aspx"},
            new ItemNavegacao(){ Nome="Contratação<br />de Serviços",   Url="/_layouts/autocredenciamento/ContratacaoServico.aspx"},                        
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (TipoNavegacao == Credenciamento.TipoNavegacao.Credenciamento)
                    rptNavegacao.DataSource = ItensCredenciamento;
                else
                    rptNavegacao.DataSource = ItensAutCredenciamento;

                rptNavegacao.DataBind();

                if (Passo > PassoMax)
                    PassoMax = Passo;

                //// Exibe o passo atual do credenciamento
                //switch (Passo)
                //{
                //    case 1: passo1.Attributes.CssStyle.Clear();
                //        passo1.Attributes.Add("class", "num");
                //        txtPasso1.Attributes.CssStyle.Clear();
                //        break;
                //    case 2: passo2.Attributes.CssStyle.Clear();
                //        passo2.Attributes.Add("class", "num");
                //        txtPasso2.Attributes.CssStyle.Clear();
                //        break;
                //    case 3: passo3.Attributes.CssStyle.Clear();
                //        passo3.Attributes.Add("class", "num");
                //        txtPasso3.Attributes.CssStyle.Clear();
                //        break;
                //    case 4: passo4.Attributes.CssStyle.Clear();
                //        passo4.Attributes.Add("class", "num");
                //        txtPasso4.Attributes.CssStyle.Clear();
                //        break;
                //    case 5: passo5.Attributes.CssStyle.Clear();
                //        passo5.Attributes.Add("class", "num");
                //        txtPasso5.Attributes.CssStyle.Clear();
                //        break;
                //    case 6: passo6.Attributes.CssStyle.Clear();
                //        passo6.Attributes.Add("class", "num");
                //        txtPasso6.Attributes.CssStyle.Clear();
                //        break;
                //    case 7: passo7.Attributes.CssStyle.Clear();
                //        passo7.Attributes.Add("class", "num");
                //        txtPasso7.Attributes.CssStyle.Clear();
                //        break;
                //    case 8: passo8.Attributes.CssStyle.Clear();
                //        passo8.Attributes.Add("class", "num");
                //        txtPasso8.Attributes.CssStyle.Clear();
                //        break;
                //}
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Navegação", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void rptNavegacao_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlGenericControl divPassoNumero = e.Item.FindControl("divPassoNumero") as HtmlGenericControl;
                HyperLink lnkPasso = (HyperLink)e.Item.FindControl("lnkPasso");
                Literal ltlDescricao = (Literal)e.Item.FindControl("ltlDescricao");
                HtmlGenericControl dvpasso = (HtmlGenericControl)e.Item.FindControl("dvpasso");
                HtmlGenericControl passo = (HtmlGenericControl)e.Item.FindControl("passo");

                ItemNavegacao item = e.Item.DataItem as ItemNavegacao;

                int idxLinha = e.Item.ItemIndex + 1;
                lnkPasso.NavigateUrl = item.Url;
                ltlDescricao.Text = item.Nome;

                if (Passo == idxLinha)
                {                    
                    dvpasso.Attributes.CssStyle.Clear();                    
                }

                divPassoNumero.Attributes["class"] = String.Format("passo{0}", idxLinha);
                if ((idxLinha <= Passo) || (idxLinha <= PassoMax))
                {
                    lnkPasso.Enabled = true;
                    passo.Attributes.CssStyle.Clear();
                    passo.Attributes.Add("class", "num");
                }
                else
                {
                    lnkPasso.Enabled = false;
                }
            }
        }

        /// <summary>Redireciona para o próximo passo, se existir</summary>
        /// <returns>Retorna falso caso não exista próximo passo</returns>
        public Boolean AvancarPasso()
        {
            var itens = new List<ItemNavegacao>();
            if(TipoNavegacao == Credenciamento.TipoNavegacao.Autocredenciamento)
                itens = ItensAutCredenciamento;
            else if(TipoNavegacao == Credenciamento.TipoNavegacao.Credenciamento)
                itens = ItensCredenciamento;
            
            //Verifica se possui próximo passo
            if (itens.Count >= Passo + 1)
            {
                //identifica e redireciona para próximo passo
                var proximoPasso = itens[Passo];
                Response.Redirect(proximoPasso.Url, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                return true;
            }
            else
            {
                //Não possui próximo passo
                //não faz nada e retorna false
                return false;
            }
        }

        /// <summary>Redireciona para o passo anterior, se existir</summary>
        /// <returns>Retorna falso caso não exista passo anterior</returns>
        public Boolean VoltarPasso()
        {
            var itens = new List<ItemNavegacao>();
            if (TipoNavegacao == Credenciamento.TipoNavegacao.Autocredenciamento)
                itens = ItensAutCredenciamento;
            else if (TipoNavegacao == Credenciamento.TipoNavegacao.Credenciamento)
                itens = ItensCredenciamento;

            //Verifica se possui passo anterior
            if (Passo - 1 > 0)
            {
                //identifica e redireciona para próximo passo
                var proximoPasso = itens[Passo - 2];
                Response.Redirect(proximoPasso.Url, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                return true;
            }
            else
            {
                //Não possui próximo passo
                //não faz nada e retorna false
                return false;
            }
        }
    }
}
