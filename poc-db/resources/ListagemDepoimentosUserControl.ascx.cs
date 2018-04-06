using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;
using System.Text;

namespace Redecard.Portal.Aberto.WebParts.ListagemDepoimentos
{
    /// <summary>
    /// Autor: Adriana Sena
    /// Data da criação: 05/10/2010
    /// Descrição: Composição do WebPart de Listagem de Depoimentos
    /// </summary>
    public partial class ListagemDepoimentosUserControl : UserControlBase
    {
        #region Variáveis
        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listagemdepoimentos_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;
        #endregion

        #region Eventos
        /// <summary>
        /// Carregamento da página e efetua listagem do
        /// depoimento no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.CarregarRamos();

                this.CarregarProdutoServicos();

                this.CarregarEstados();

                this.CarregarDepoimentos();
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Clique do OK para realização de consulta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect(this.ObterURLAutoRedirecionamento());
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemDepoimentos WebPart
        {
            get
            {
                return this.Parent as ListagemDepoimentos;
            }
        }

        /// <summary>
        /// Referência tipada ao user control de paginação
        /// </summary>
        private UserControlPaginador Paginador
        {
            get
            {
                return this.ucPaginadorHibrido as UserControlPaginador;
            }
        }

        /// <summary>
        /// Obtém os itens no página estipulados no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private int ItensPorPagina
        {
            get
            {
                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Propriedade Ramo obtem valores da URL e efetua listagem do
        /// depoimento no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        public string Ramo
        {
            get
            {
                string paramRamo = ChavesQueryString.Ramo;

                if (string.IsNullOrEmpty(Request.Params[paramRamo]))
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[paramRamo]);
            }
        }

        /// <summary>
        /// Propriedade ProdutoServico obtem valores da URL e efetua listagem do
        /// depoimento no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        public string ProdutoServico
        {
            get
            {
                string paramProdutoServico = ChavesQueryString.ProdutoServico;

                if (string.IsNullOrEmpty(Request.Params[paramProdutoServico]))
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[paramProdutoServico]);
            }
        }

        /// <summary>
        /// Propriedade Estado obtem valores da URL e efetua listagem do
        /// depoimento no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        public string Estado
        {
            get
            {
                string paramEstado = ChavesQueryString.Estado;

                if (string.IsNullOrEmpty(Request.Params[paramEstado]))
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[paramEstado]);
            }
        }

        /// <summary>
        /// Verifica se algum filtro de campo (Ramo, Estado e Produto/Serviço foi informado)
        /// </summary>
        private bool AlgumFiltroInformado
        {
            get
            {
                return !this.slcBranch.SelectedItem.Value.Equals(valorPadraoSelecione) ||
                       !this.slcState.SelectedItem.Value.Equals(valorPadraoSelecione) ||
                       !this.slcProductServices.SelectedItem.Value.Equals(valorPadraoSelecione);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Solicita o Ramo no repositório de depoimento
        /// </summary>
        private IList<string> ObterRamos()
        {
            IList<string> ramo = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODepoimento, DepoimentosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            ramo = (from ram in repository.GetAllItems()
                                    select ram.Ramo).OrderBy(r => r).Distinct().ToList();
                        });
                }
            }
            catch (Exception)
            {
                // TODO: Redirecionar para uma página padrão de erro
            }

            return ramo;
        }

        /// <summary>
        /// Configura e preenche o controle de lista de ramos
        /// </summary>
        private void CarregarRamos()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Ramo e Página
            //string urlAtual = URLUtils.ObterURLAtual(ramo => ramo.Trim().ToUpper().Equals(ChavesQueryString.Ramo.ToUpper()) || ramo.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcBranch.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            //this.slcBranch.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Ramo, null));

            //Obtém os ramos para carregamento
            IList<string> ramos = this.ObterRamos();

            //Carrega o controle com os ramos
            if (ramos != null)
                ramos.ToList().ForEach(r => this.slcBranch.Items.Add(new ListItem(r, r)));

            //Adiciona um primeiro item no controle
            this.slcBranch.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona o ramo atual, se informado
            ListItem liRamo = this.slcBranch.Items.FindByValue(this.Ramo);
            if (liRamo != null)
                liRamo.Selected = true;
            else
                this.slcBranch.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita produto ou serviço no repositório de depoimento
        /// </summary>
        private IList<string> ObterProdutoServicos()
        {
            IList<string> produtosServicos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODepoimento, DepoimentosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            produtosServicos = (from produtoServicos in repository.GetAllItems()
                                              select produtoServicos.ProdutoServico).OrderBy(p => p).Distinct().ToList();
                        });
                }
            }
            catch (Exception)
            {
                // TODO: Redirecionar para uma página padrão de erro
            }

            return produtosServicos;
        }

        /// <summary>
        /// Configura e preenche o controle de lista de produto/serviço
        /// </summary>
        private void CarregarProdutoServicos()
        {
            //Obtém a URL pedindo para ignorar os parâmetros ProdutoServico e Página
            //string urlAtual = URLUtils.ObterURLAtual(prodServ => prodServ.Trim().ToUpper().Equals(ChavesQueryString.ProdutoServico.ToUpper()) || prodServ.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcProductServices.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            //this.slcProductServices.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.ProdutoServico, null));

            //Obtém os produtos/serviços para carregamento
            IList<string> produtoServicos = this.ObterProdutoServicos();

            //Carrega o controle com os ramos
            if (produtoServicos != null)
                produtoServicos.ToList().ForEach(p => this.slcProductServices.Items.Add(new ListItem(p, p)));

            //Adiciona um primeiro item no controle
            this.slcProductServices.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona o produto/serviço atual, se informado
            ListItem liProdutoServico = this.slcProductServices.Items.FindByValue(this.ProdutoServico);
            if (liProdutoServico != null)
                liProdutoServico.Selected = true;
            else
                this.slcProductServices.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita os estados no repositório de depoimento
        /// </summary>
        private IList<string> ObterEstados()
        {
            IList<string> estado = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODepoimento, DepoimentosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            estado = (from estados in repository.GetAllItems()
                                      select estados.Estado).OrderBy(e => e).Distinct().ToList();
                        });
                }
            }

            catch (Exception)
            {
                //TODO: Redirecionar para uma página padrão de erro
            }

            return estado;
        }

        /// <summary>
        /// Configura e preenche o controle de lista de produto/serviço
        /// </summary>
        private void CarregarEstados()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Estado e Página
            //string urlAtual = URLUtils.ObterURLAtual(estado => estado.Trim().ToUpper().Equals(ChavesQueryString.Estado.ToUpper()) || estado.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcState.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            //this.slcState.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Estado, null));

            //Obtém os produtos/serviços para carregamento
            IList<string> estados = this.ObterEstados();

            //Carrega o controle com os ramos
            if (estados  != null)
                estados.ToList().ForEach(e => this.slcState.Items.Add(new ListItem(e, e)));

            //Adiciona um primeiro item no controle
            this.slcState.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona o produto/serviço atual, se informado
            ListItem liEstado = this.slcState.Items.FindByValue(this.Estado);
            if (liEstado != null)
                liEstado.Selected = true;
            else
                this.slcState.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Lista e popula os depoimentos na tela
        /// </summary>
        private ListaPaginada<DTODepoimento> ObterDepoimentos()
        {
            //Caso nenhum filtro tenha sido informado, evita a busca no repositório e retorna lista vazia
            if(!this.AlgumFiltroInformado)
                return new ListaPaginada<DTODepoimento>(new List<DTODepoimento>(0),null,0);

            ListaPaginada<DTODepoimento> depoimentos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODepoimento, DepoimentosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        depoimentos = new ListaPaginada<DTODepoimento>(repository.GetItems
                        //Função de filtragem pelos campos opcionais Ramo, Produto/Serviço e Estado
                        (d => (d.Ramo.Equals(this.slcBranch.SelectedItem.Text) || this.slcBranch.SelectedItem.Value.Equals(valorPadraoSelecione)) &&
                              (d.ProdutoServico.Equals(this.slcProductServices.SelectedItem.Text) || this.slcProductServices.SelectedItem.Value.Equals(valorPadraoSelecione)) &&
                              (d.Estado.Equals(this.slcState.SelectedItem.Text) || this.slcState.SelectedItem.Value.Equals(valorPadraoSelecione)))
                        //Função para ordenação
                        .OrderByDescending(d => d.ID)
                        ,this.Paginador.Pagina
                        ,this.ItensPorPagina);
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return depoimentos;
        }

        /// <summary>
        /// Configura e preenche o controle de Depoimentos
        /// </summary>
        private void CarregarDepoimentos()
        {
            ListaPaginada<DTODepoimento> depoimentos = this.ObterDepoimentos();

            //Popula no repeater
            this.rptListagemDepoimentos.DataSource = depoimentos;
            this.rptListagemDepoimentos.DataBind();

            //Solicita a montagem do paginador
            this.Paginador.MontarPaginador(depoimentos.TotalItens, this.ItensPorPagina, null);
        }

        /// <summary>
        /// Retorna a URL atual contendo os campos-filtro na query string para retorno dos depoimentos
        /// Isso é necessário pois o paginador utiliza estes parâmetros para auto-configuração incluindo estas querystrings
        /// </summary>
        /// <returns></returns>
        private string ObterURLAutoRedirecionamento()
        {
            //Obtém a URL atual desprezando os parâmetros Estado, ProdutoServico, Ramo e Página
            string urlRedirecionamento = URLUtils.ObterURLAtual(parms => parms.ToUpper().Equals(ChavesQueryString.Estado.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.ProdutoServico.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Ramo.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            if (!this.AlgumFiltroInformado)
            {
                if (urlRedirecionamento.EndsWith("?"))
                    return urlRedirecionamento.Substring(0, urlRedirecionamento.Length - 1);

                return urlRedirecionamento;
            }

            //As dropdownlists e o paginador se orientam pelas querystrings.
            //Então, executa um auto-redirecionamento passando como parâmetro as seleções dos campos-filtro
            //O paginador montará a URL com estes parâmetros + o número da página
            string url = string.Format("{0}{1}{2}{3}{4}",
                         urlRedirecionamento,
                         urlRedirecionamento.IndexOf('?') == -1 ? "?" : string.Empty, //Verifica se precisa anexar no final da URL um ? para depois entrar os parâmetros
                         !this.slcBranch.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Ramo + "=" + URLUtils.URLEncode(this.slcBranch.SelectedItem.Text) + "&" : string.Empty,
                         !this.slcProductServices.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.ProdutoServico + "=" + URLUtils.URLEncode(this.slcProductServices.SelectedItem.Text) + "&" : string.Empty,
                         !this.slcState.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Estado + "=" + URLUtils.URLEncode(this.slcState.SelectedItem.Text) + "&" : string.Empty);

            //Remove o último & se houver
            if (url.Trim().LastIndexOf('&', url.Trim().Length - 1) != -1)
                return url.Substring(0, url.Trim().Length - 1);

            return url;
        }
        #endregion        
    }
}