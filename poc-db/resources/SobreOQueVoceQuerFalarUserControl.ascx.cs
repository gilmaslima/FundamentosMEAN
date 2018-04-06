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

namespace Redecard.Portal.Aberto.WebParts.SobreOQueVoceQuerFalar
{
    /// <summary>
    /// Autor: Cristiano Dias
    /// Data da criação: 14/10/2010
    /// Descrição: Composição do WebPart de Listagem de Perguntas Frequentes para a página Fale Conosco
    /// </summary>
    public partial class SobreOQueVoceQuerFalarUserControl : UserControlBase
    {
        #region Variáveis
        private static string textoPadraoSelecione_Assunto = RedecardHelper.ObterResource("perguntasfrequentes_selecioneassunto");
        private static string textoPadraoSelecione_Area = RedecardHelper.ObterResource("perguntasfrequentes_selecionearea");
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
                this.CarregarAssunto();

                this.CarregarArea();

                this.CarregarPerguntasFrequentes();
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

        #region Comentado
        /// <summary>
        /// Edição de cada item de pergunta freqüente para atribuição do link para página de dúvidas freqüentes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void rptPerguntasFrequentes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        DTOPerguntaFrequente pergunta = e.Item.DataItem as DTOPerguntaFrequente;

        //        if (pergunta != null)
        //        {
        //            HyperLink lnkPaginaPerguntasFrequentes = e.Item.FindControl("lnkPaginaPerguntasFrequentes") as HyperLink;

        //            if (lnkPaginaPerguntasFrequentes != null)
        //                lnkPaginaPerguntasFrequentes.NavigateUrl = this.ObterURLPaginaPerguntasFrequentes(pergunta);
        //        }
        //    }
        //}
        #endregion
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private SobreOQueVoceQuerFalar WebPart
        {
            get
            {
                return this.Parent as SobreOQueVoceQuerFalar;
            }
        }

        /// <summary>
        /// Obtém os itens no página estipulados no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private int QuantidadePerguntasAMostrar
        {
            get
            {
                return this.WebPart.QuantidadePerguntasAMostrar;
            }
        }

        /// <summary>
        /// Propriedade Area obtem valores da URL e efetua listagem das 
        /// perguntas frequentes no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        public string Area
        {
            get
            {
                string paramArea = ChavesQueryString.Area;

                if (string.IsNullOrEmpty(Request.Params[paramArea]))
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[paramArea]);
            }
        }

        /// <summary>
        /// Propriedade Assunto obtem valores da URL e efetua listagem das
        /// perguntas frequentes no carregamento da página deixando o valor selecionado se 
        /// existir.
        /// </summary>
        public string Assunto
        {
            get
            {
                string paramAssunto = ChavesQueryString.Assunto;

                if (string.IsNullOrEmpty(Request.Params[paramAssunto]))
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[paramAssunto]);
            }
        }

        /// <summary>
        /// Verifica se algum filtro de campo (Area,Assunto) foi informado
        /// </summary>
        private bool AlgumFiltroInformado
        {
            get
            {
                return !this.slcVisualize.SelectedItem.Value.Equals(valorPadraoSelecione) ||
                       !this.slcOrder.SelectedItem.Value.Equals(valorPadraoSelecione);
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Solicita o Area no repositório de perguntas frequentes
        /// </summary>
        private IList<string> ObterAreas()
        {
            IList<string> areas = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            areas = (from ar in repository.GetAllItems()
                                     select ar.Area).OrderBy(r => r).Distinct().ToList();
                        });
                }
            }
            catch (Exception)
            {
                // TODO: Redirecionar para uma página padrão de erro
            }

            return areas;
        }

        /// <summary>
        /// Configura e preenche o controle de lista de areas
        /// </summary>
        private void CarregarArea()
        {
            //Esvazia o controle
            this.slcVisualize.Items.Clear();

            //Obtém as areas para carregamento
            IList<string> areas = this.ObterAreas();

            //Carrega o controle com as areas
            if (areas != null)
                areas.ToList().ForEach(r => this.slcVisualize.Items.Add(new ListItem(r, r)));

            //Adiciona um primeiro item no controle
            this.slcVisualize.Items.Insert(0, new ListItem(textoPadraoSelecione_Area, valorPadraoSelecione));

            //Seleciona a area atual, se informado
            ListItem liArea = this.slcVisualize.Items.FindByValue(this.Area);
            if (liArea != null)
                liArea.Selected = true;
            else
                this.slcVisualize.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita assuntos no repositório de perguntas frequentes
        /// </summary>
        private IList<string> ObterAssunto()
        {
            IList<string> assuntos = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            assuntos = (from assunto in repository.GetAllItems()
                                        select assunto.Assunto).OrderBy(p => p).Distinct().ToList();
                        });
                }
            }
            catch (Exception)
            {
                // TODO: Redirecionar para uma página padrão de erro
            }

            return assuntos;
        }

        /// <summary>
        /// Configura e preenche o controle de lista de assunto
        /// </summary>
        private void CarregarAssunto()
        {
            //Esvazia o controle
            this.slcOrder.Items.Clear();

            //Obtém os assuntos para carregamento
            IList<string> assuntos = this.ObterAssunto();

            //Carrega o controle com os assuntos
            if (assuntos != null)
                assuntos.ToList().ForEach(p => this.slcOrder.Items.Add(new ListItem(p, p)));

            //Adiciona um primeiro item no controle
            this.slcOrder.Items.Insert(0, new ListItem(textoPadraoSelecione_Assunto, valorPadraoSelecione));

            //Seleciona o assunto atual, se informado
            ListItem liAssunto = this.slcOrder.Items.FindByValue(this.Assunto);
            if (liAssunto != null)
                liAssunto.Selected = true;
            else
                this.slcOrder.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Lista as perguntas frequentes do repositório
        /// Obtém ordenado por ordem decrescente de cadastro
        /// </summary>
        private IList<DTOPerguntaFrequente> ObterPerguntasFrequentes()
        {
            //Caso nenhum filtro tenha sido informado, evita a busca no repositório e retorna lista vazia
            if (!this.AlgumFiltroInformado)
                return new List<DTOPerguntaFrequente>(0);

            ListaPaginada<DTOPerguntaFrequente> perguntasFrequentes = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        perguntasFrequentes = new ListaPaginada<DTOPerguntaFrequente>(repository.GetItems
                            //Função de filtragem pelos campos opcionais Area e Assunto
                        (p => (p.Área.Equals(this.slcVisualize.SelectedItem.Text) || this.slcVisualize.SelectedItem.Value.Equals(valorPadraoSelecione)) &&
                              (p.Assunto.Equals(this.slcOrder.SelectedItem.Text) || this.slcOrder.SelectedItem.Value.Equals(valorPadraoSelecione)))
                            //Função para ordenação
                              .OrderByDescending(p => p.ID),
                       null, //Índice página
                       this.QuantidadePerguntasAMostrar); //Quantidade de itens a mostrar
                    });

                    return perguntasFrequentes;
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return perguntasFrequentes;
        }

        /// <summary>
        /// Solicita o carregamento das Perguntas Frequentes
        /// O método considera a regra de haver paginação ou não para popular a lista para o usuário
        /// </summary>
        private void CarregarPerguntasFrequentes()
        {
            //Popula a lista no repeater
            this.rptPerguntasFrequentes.DataSource = this.ObterPerguntasFrequentes();
            this.rptPerguntasFrequentes.DataBind();
        }

        /// <summary>
        /// Retorna a URL atual contendo os campos-filtro na query string para retorno das perguntas frequentes
        /// Isso é necessário pois o paginador utiliza estes parâmetros para auto-configuração incluindo estas querystrings
        /// </summary>
        /// <returns></returns>
        private string ObterURLAutoRedirecionamento()
        {
            //Obtém a URL atual desprezando os parâmetros Area, Assunto e Página
            string urlRedirecionamento = URLUtils.ObterURLAtual(parms => parms.ToUpper().Equals(ChavesQueryString.Area.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Assunto.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            if (!this.AlgumFiltroInformado)
            {
                if (urlRedirecionamento.EndsWith("?"))
                    return urlRedirecionamento.Substring(0, urlRedirecionamento.Length - 1);

                return urlRedirecionamento;
            }

            //As dropdownlists e o paginador se orientam pelas querystrings.
            //Então, executa um auto-redirecionamento passando como parâmetro as seleções dos campos-filtro
            //O paginador montará a URL com estes parâmetros + o número da página
            string url = string.Format("{0}{1}{2}{3}",
                         urlRedirecionamento,
                         urlRedirecionamento.IndexOf('?') == -1 ? "?" : string.Empty, //Verifica se precisa anexar no final da URL um ? para depois entrar os parâmetros
                         !this.slcVisualize.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Area + "=" + URLUtils.URLEncode(this.slcVisualize.SelectedItem.Text) + "&" : string.Empty,
                         !this.slcOrder.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Assunto + "=" + URLUtils.URLEncode(this.slcOrder.SelectedItem.Text) + "&" : string.Empty);

            //Remove o último & se houver
            if (url.Trim().LastIndexOf('&', url.Trim().Length - 1) != -1)
                return url.Substring(0, url.Trim().Length - 1);

            return url;
        }

        /// <summary>
        /// Com base no item de pergunta frequente,
        /// redireciona o usuário para a página de perguntas frequentes, que abrirá todas as perguntas com base na área e assunto o qual o item atual está vinculado
        /// </summary>
        /// <param name="perguntaFrequente"></param>
        /// <returns></returns>
        private string ObterURLPaginaPerguntasFrequentes(DTOPerguntaFrequente perguntaFrequente)
        {
            //SÓ PARA TESTE!!!!Endereço não é dinâmico!
            return string.Format("~/pt-BR/Atendimento/paginas/perguntasfrequentes.aspx?{0}={1}&{2}={3}", ChavesQueryString.Area, URLUtils.URLEncode(perguntaFrequente.Area), ChavesQueryString.Assunto, URLUtils.URLEncode(perguntaFrequente.Assunto));
        }
        #endregion
    }
}