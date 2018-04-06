using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;

namespace Redecard.Portal.Aberto.WebParts.ListagemCartoes {
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 12/10/2010
    /// Descrição: Composição do WebPart de Listagem de Cartões
    /// </summary>
    public partial class ListagemCartoesUserControl : UserControlBase {
        #region Variáveis

        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listagemCartoes_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;

        #endregion

        #region Propriedades
        /// <summary>
        /// Nome da âncora utilizada para posicionamento do usuário na página
        /// </summary>
        public string AncoraListagem {
            get {
                return "wptListagem";
            }
        }

        /// <summary>
        /// Obtém os itens no página estipulados no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private int ItensPorPagina {
            get {
                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemCartoes WebPart {
            get {
                return this.Parent as ListagemCartoes;
            }
        }

        /// <summary>
        /// Referência tipada ao user control de paginação
        /// </summary>
        private UserControlPaginador Paginador {
            get {
                return this.ucPaginadorHibrido as UserControlPaginador;
            }
        }

        /// <summary>
        /// Acesso ao tipo de cartão de benefício selecionado
        /// </summary>
        private string TipoCartaoBeneficio {
            get {
                //if (String.IsNullOrEmpty(Request.QueryString[ChavesQueryString.Tipo]))
                //    return "all";
                //return URLUtils.URLDecode(Request.QueryString[ChavesQueryString.Tipo]);
                return this.slcTypeCardBenefits.SelectedValue;
            }
        }

        /// <summary>
        /// Acesso ao tipo de cartão selecionado
        /// </summary>
        private Redecard.Portal.Aberto.Model.TipoDoCartão TipoDoCartao {
            get {
                return this.WebPart.TipoDoCartao;
            }
        }


        #endregion

        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            if (this.TipoDoCartao == TipoDoCartão.Benefício) {
                this.ResetPaginator();
                
                this.dvTypeCardBenefits.Visible = true;
                this.CarregarCartoes();
                this.CarregarCartoesPorTipoDeCartao(this.TipoCartaoBeneficio);
            }
            else {
                this.dvTypeCardBenefits.Visible = false;
                this.CarregarCartoesPorTipoDeCartao();
            }
            //lblTypeCard.Text = RedecardHelper.ObterResource("listagemCartoes_conhecacartoesde") + this.TipoDoCartao.ToString();
        }

        private void ResetPaginator() {
            if (object.ReferenceEquals(ViewState["_tipocartaobeneficio"], null))
                ViewState.Add("_tipocartaobeneficio", this.TipoCartaoBeneficio);
            else {
                string tipoCartaoBeneficioOld = ViewState["_tipocartaobeneficio"].ToString();
                if (!tipoCartaoBeneficioOld.Equals(this.TipoCartaoBeneficio))
                    pag.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptListagemCartoes_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            if (this.rptListagemCartoes.Items.Count < 1) {
                if (e.Item.ItemType == ListItemType.Footer) {
                    Label lblFooter = (Label)e.Item.FindControl("lblEmptyData");
                    lblFooter.Visible = true;
                }
            }
        }

        #endregion

        #region Métodos
        /// <summary>
        /// Monta uma string JavaScript de redirecionamento com base nos parâmetros informados
        /// </summary>
        /// <param name="urlAtual"></param>
        /// <param name="nomeParametro">Nome do parâmetro que será utilizado para o valor do item selecionado no controle. Exemplo: PAGINA=1</param>
        /// <param name="NomeAncora"></param>
        /// <returns></returns>
        protected virtual string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora) {
            return string.Format("window.location = '{0}{1}{2}=' + encodeURIComponent(this.options[this.selectedIndex].value) + '#" + nomeAncora + "';",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro);
        }

        /// <summary>
        /// Carrega a DropDownList de Tipo de Cartão
        /// Os tipos de cartão são obtidos a partir da listagem de cartões existentes no repositório, e as duplicatas são eliminadas.
        /// Por último a coleção é ordenada alfabeticamente
        /// </summary>
        private void CarregarCartoes() {
            //Obtém a URL pedindo para ignorar os parâmetros Tipo e Página
            //string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Tipo.ToUpper()) || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            //this.slcTypeCardBenefits.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            //this.slcTypeCardBenefits.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Tipo, this.AncoraListagem));

            //Obtém os tipos de cartões para carregamento
            IList<string> cartoes = this.ObterCartoes(this.TipoDoCartao);

            //Carrega o controle com os tipos de cartões
            if (!Page.IsPostBack)
                if (cartoes != null)
                    cartoes.ToList().ForEach(g => this.slcTypeCardBenefits.Items.Add(new ListItem(g, g)));

            //Adiciona um primeiro item no controle
            //this.slcTypeCardBenefits.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona o tipo de cartão atual, se informado
            ListItem liCartao = this.slcTypeCardBenefits.Items.FindByValue(this.TipoCartaoBeneficio);

            if (liCartao != null) {
                liCartao.Selected = true;
            }
            else
                this.slcTypeCardBenefits.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita os tipos de cartões com o Repositório de Cartões
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterCartoes(TipoDoCartão tipoCartao) {
            IList<DTOCartao> cartoes = null;
            List<string> cartoesBeneficios = null;

            try {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOCartao, CartõesItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        cartoes = (from car in repository.GetAllItems()
                                   where car.TipoDoCartao == tipoCartao && !String.IsNullOrEmpty(car.Beneficios)
                                   select car).ToList();
                    });
                }

                // Rodar as strings e separar os items que possuem mais de um beneficio selecionado
                cartoesBeneficios = new List<string>();
                foreach (DTOCartao cartao in cartoes) {
                    string[] _beneficios = cartao.Beneficios.Split(';');
                    foreach (string beneficio in _beneficios) {
                        if (!cartoesBeneficios.Contains(beneficio))
                            cartoesBeneficios.Add(beneficio);
                    }
                }
            }
            catch (Exception) {
                //TODO:Redirecionar para uma página padrão de erro
            }
            return cartoesBeneficios;
        }


        /// <summary>
        /// Carrega a lista de cartões com base no tipo de cartão e tipo de cartão de benefício informado
        /// </summary>
        /// <param name="tipoCartao"></param>
        private void CarregarCartoesPorTipoDeCartao(string tipoCartaoBeneficio) {
            if (tipoCartaoBeneficio.ToLowerInvariant().Equals("all"))
                this.CarregarCartoesPorTipoDeCartao();
            else {
                try {
                    using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOCartao, CartõesItem>>()) {
                        ListaPaginada<DTOCartao> cartoes;

                        AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate {
                            cartoes = new ListaPaginada<DTOCartao>(repository.GetItems(cartao => !String.IsNullOrEmpty(cartao.Beneficios) && cartao.Beneficios.Contains(this.TipoCartaoBeneficio) && cartao.TipoDoCartão.Equals(this.TipoDoCartao)).OrderBy(cartao => cartao.Ordem), this.pag.PaginaAtual, this.ItensPorPagina);

                            this.pag.TotalPaginas = cartoes.TotalPaginas;

                            this.rptListagemCartoes.DataSource = cartoes;
                            this.rptListagemCartoes.DataBind();

                            this.Paginador.MontarPaginador(cartoes.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                        });
                    }
                }
                catch (Exception) {
                    //TODO:Redirecionar para uma página padrão de erro
                }
            }
        }

        /// <summary>
        /// Carrega a lista de cartões com base no tipo de cartão informado
        /// </summary>        
        private void CarregarCartoesPorTipoDeCartao() {
            try {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOCartao, CartõesItem>>()) {
                    ListaPaginada<DTOCartao> cartoes;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        cartoes = new ListaPaginada<DTOCartao>(repository.GetItems(cartao => cartao.TipoDoCartão.Equals(this.TipoDoCartao)).OrderBy(cartao => cartao.Ordem), this.pag.PaginaAtual, this.ItensPorPagina);

                        this.pag.TotalPaginas = cartoes.TotalPaginas;

                        this.rptListagemCartoes.DataSource = cartoes;
                        this.rptListagemCartoes.DataBind();

                        this.Paginador.MontarPaginador(cartoes.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                    });
                }
            }
            catch (Exception) {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        #endregion
    }
}
