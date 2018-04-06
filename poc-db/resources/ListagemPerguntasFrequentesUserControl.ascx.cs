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

namespace Redecard.Portal.Aberto.WebParts.ListagemPerguntasFrequentes
{
    /// <summary>
    /// Autor: Adriana Sena
    /// Data da criação: 11/10/2010
    /// Descrição: Composição do WebPart de Listagem de Perguntas Frequentes
    /// 
    /// Autor alteração: Cristiano M. Dias
    /// Data alteração: 04/11/2010
    /// Descrição: Adição da lógica para listagem de toda as perguntas do repositório ao acionar o botão Ver Todas
    /// Até antes, o botão Ver Todas listava todas as perguntas mas ainda considerando os filtro de campo e assunto.
    /// Agora, estes filtros são ignorados
    /// </summary>
    public partial class ListagemPerguntasFrequentesUserControl : UserControlBase
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

            this.ConfigurarBotaoVerTodas();

            base.OnLoad(e);
        }

        /// <summary>
        /// Clique do OK para realização de consulta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect(this.ObterURLAutoRedirecionamento(false));
        }

        /// <summary>
        /// Clique do botão Ver Todas
        /// Solicita a listagem de todas as Perguntas Frequentes do repositório
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVerTodas_Click(object sender, EventArgs e)
        {
            string sUrl = this.ObterURLAutoRedirecionamento(true);
            sUrl += "&showAllButton=false";
            Response.Redirect(sUrl);
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemPerguntasFrequentes WebPart
        {
            get
            {
                return this.Parent as ListagemPerguntasFrequentes;
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
        /// Obtém a quantidade de itens por página de acordo com a seguinte regra:
        /// Caso os filtros sejam ignorados (vide Propriedade IgnorarFiltros), ele retorna o número de itens estipulados
        /// para o modo de Visualização Ver Todas (botão Ver Todas) da propriedade this.WebPart.QuantidadeItensPorPaginaModoVerTodas.
        /// Se não, retorna o valor da propriedade this.WebPart.QuantidadeItensPorPagina
        /// Ambas estão estipuladas no controle WebPart que instancia este UserControl
        /// </summary>
        private int ItensPorPagina
        {
            get
            {
                /*
                int itensPorPagina;
                if(int.TryParse(Request.Params[ChavesQueryString.QtdeItens],out itensPorPagina))
                    return itensPorPagina;
                */

                if (this.IgnorarFiltros)
                    return this.WebPart.QuantidadeItensPorPaginaModoVerTodas;

                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Obtém a flag indicando se haverá paginação no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private bool PaginacaoHabilitada
        {
            get
            {
                return this.WebPart.PaginacaoHabilitada;
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
        /// Propriedade IgnorarFiltros - obtém valor da URL
        /// Regra: caso o filtro não seja informado ou seja inválido, a página NÃO ignora os filtros!
        /// Se informado, a página poderá ou não ignorar dependendo do valor informado:
        /// Se = 1 -> Ignora os filtros
        /// Qualquer outro valor -> Não ignora os filtros
        /// </summary>
        public bool IgnorarFiltros
        {
            get
            {
                string paramIgnorarFiltros = ChavesQueryString.IgnorarFiltros;

                if (string.IsNullOrEmpty(Request.Params[paramIgnorarFiltros]))
                    return false;

                int ignorarFiltros;
                if (!int.TryParse(Request.Params[paramIgnorarFiltros], out ignorarFiltros))
                    return false;

                return ignorarFiltros.Equals(1) ? true : false;
            }
        }

        /// <summary>
        /// Verifica se algum filtro de campo (Area,Assunto) foi informado
        /// </summary>
        private bool AlgumFiltroInformado
        {
            get
            {
                return !this.slcArea.SelectedItem.Value.Equals(valorPadraoSelecione) ||
                       !this.slcSubject.SelectedItem.Value.Equals(valorPadraoSelecione);
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
            this.slcArea.Items.Clear();

            //Obtém as areas para carregamento
            IList<string> areas = this.ObterAreas();

            //Carrega o controle com as areas
            if (areas != null)
                areas.ToList().ForEach(r => this.slcArea.Items.Add(new ListItem(r, r)));

            //Adiciona um primeiro item no controle
            this.slcArea.Items.Insert(0, new ListItem(textoPadraoSelecione_Area, valorPadraoSelecione));

            //Seleciona a area atual, se informado
            ListItem liArea = this.slcArea.Items.FindByValue(this.Area);
            if (liArea != null)
                liArea.Selected = true;
            else
                this.slcArea.SelectedValue = valorPadraoSelecione;
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
            this.slcSubject.Items.Clear();

            //Obtém os assuntos para carregamento
            IList<string> assuntos = this.ObterAssunto();

            //Carrega o controle com os assuntos
            if (assuntos != null)
                assuntos.ToList().ForEach(p => this.slcSubject.Items.Add(new ListItem(p, p)));

            //Adiciona um primeiro item no controle
            this.slcSubject.Items.Insert(0, new ListItem(textoPadraoSelecione_Assunto, valorPadraoSelecione));

            //Seleciona o assunto atual, se informado
            ListItem liAssunto = this.slcSubject.Items.FindByValue(this.Assunto);
            if (liAssunto != null)
                liAssunto.Selected = true;
            else
                this.slcSubject.SelectedValue = valorPadraoSelecione;
        }

        /* BACKUP
        /// <summary>
        /// Lista as perguntas frequentes do repositório
        /// Obtém ordenado por ordem decrescente de cadastro
        /// </summary>
        private IList<DTOPerguntaFrequente> ObterPerguntasFrequentes()
        {
            //Caso nenhum filtro tenha sido informado, evita a busca no repositório e retorna lista vazia
            if (!this.AlgumFiltroInformado)
                return new List<DTOPerguntaFrequente>(0);

            IList<DTOPerguntaFrequente> perguntasFrequentes = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        perguntasFrequentes = repository.GetItems
                            //Função de filtragem pelos campos opcionais Area e Assunto
                        (p => (p.Área.Equals(this.slcArea.SelectedItem.Text) || this.slcArea.SelectedItem.Value.Equals(valorPadraoSelecione)) &&
                              (p.Assunto.Equals(this.slcSubject.SelectedItem.Text) || this.slcSubject.SelectedItem.Value.Equals(valorPadraoSelecione)))
                            //Função para ordenação
                              .OrderByDescending(p => p.ID).ToList();
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
        */

        /// <summary>
        /// Lista as perguntas frequentes do repositório
        /// Obtém ordenado por ordem decrescente de cadastro
        /// </summary>
        private IList<DTOPerguntaFrequente> ObterPerguntasFrequentes()
        {
            IList<DTOPerguntaFrequente> perguntasFrequentes = null;

            if (!this.IgnorarFiltros)
            {
                //Caso nenhum filtro tenha sido informado, evita a busca no repositório e retorna lista vazia
                if (!this.AlgumFiltroInformado)
                    return new List<DTOPerguntaFrequente>(0);

                try
                {
                    using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                    {
                        AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            perguntasFrequentes = repository.GetItems
                                //Função de filtragem pelos campos opcionais Area e Assunto
                            (p => (p.Área.Equals(this.slcArea.SelectedItem.Text) || this.slcArea.SelectedItem.Value.Equals(valorPadraoSelecione)) &&
                                  (p.Assunto.Equals(this.slcSubject.SelectedItem.Text) || this.slcSubject.SelectedItem.Value.Equals(valorPadraoSelecione)))
                                //Função para ordenação
                                  .OrderByDescending(p => p.ID).ToList();
                        });

                        return perguntasFrequentes;
                    }
                }
                catch (Exception)
                {
                    //TODO:Redirecionar para uma página padrão de erro
                }
            }
            else //Ignora os filtros, listar tudo
            {
                try
                {
                    using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>())
                    {
                        AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate
                        {
                            perguntasFrequentes = repository.GetAllItems().OrderByDescending(p => p.ID).ToList();
                        });

                        return perguntasFrequentes;
                    }
                }
                catch (Exception)
                {
                    //TODO:Redirecionar para uma página padrão de erro
                }
            }

            return perguntasFrequentes;
        }

        /// <summary>
        /// Obtém a enésima página (N = parâmetro querystring número de Pagina) da lista de perguntas frequentes
        /// </summary>
        /// <returns></returns>
        private ListaPaginada<DTOPerguntaFrequente> ObterPaginaPerguntasFrequentes()
        {
            return new ListaPaginada<DTOPerguntaFrequente>(this.ObterPerguntasFrequentes(), this.Paginador.Pagina, this.ItensPorPagina);
        }

        /// <summary>
        /// Solicita o carregamento das Perguntas Frequentes
        /// O método considera a regra de haver paginação ou não para popular a lista para o usuário
        /// </summary>
        private void CarregarPerguntasFrequentes()
        {
            if (this.PaginacaoHabilitada)
            {
                //Obtém as perguntas do repositório de forma paginada
                ListaPaginada<DTOPerguntaFrequente> perguntasFrequentes = this.ObterPaginaPerguntasFrequentes();
                this.PopularPerguntasFrequentes(perguntasFrequentes);

                //Solicita a montagem do paginador
                this.Paginador.MontarPaginador(perguntasFrequentes.TotalItens, this.ItensPorPagina, null);
            }
            else
                //Obtém as perguntas do repositório sem levar em consideração montagem de paginação
                this.PopularPerguntasFrequentes(this.ObterPerguntasFrequentes());
        }

        /// <summary>
        /// Popula os itens de Perguntas Frequentes para o usuário
        /// </summary>
        /// <param name="perguntasFrequentes">Itens de Perguntas Frequentes</param>
        private void PopularPerguntasFrequentes(IEnumerable<DTOPerguntaFrequente> perguntasFrequentes)
        {
            //Popula a lista no repeater
            this.rptPerguntasFrequentes.DataSource = perguntasFrequentes;
            this.rptPerguntasFrequentes.DataBind();

            this.ConfigurarMensagemRetornoPesquisa(perguntasFrequentes.Count() > 0);
        }

        /*BACKUP
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
                         !this.slcArea.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Area + "=" + URLUtils.URLEncode(this.slcArea.SelectedItem.Text) + "&" : string.Empty,
                         !this.slcSubject.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Assunto + "=" + URLUtils.URLEncode(this.slcSubject.SelectedItem.Text) + "&" : string.Empty);

            //Remove o último & se houver
            if (url.Trim().LastIndexOf('&', url.Trim().Length - 1) != -1)
                return url.Substring(0, url.Trim().Length - 1);

            return url;
        }
        */

        /// <summary>
        /// Retorna a URL atual contendo os campos-filtro na query string para retorno das perguntas frequentes
        /// Isso é necessário pois o paginador utiliza estes parâmetros para auto-configuração incluindo estas querystrings
        /// </summary>
        /// <returns></returns>
        private string ObterURLAutoRedirecionamento(bool ignorarFiltros)
        {
            string urlRedirecionamento = string.Empty;
            string url = string.Empty;

            //Obtém a URL atual desprezando os parâmetros Area, Assunto, Página e IgnorarFiltros
            urlRedirecionamento = URLUtils.ObterURLAtual(parms => parms.ToUpper().Equals(ChavesQueryString.Area.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Assunto.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()) || parms.ToUpper().Equals(ChavesQueryString.IgnorarFiltros.ToUpper()));

            //Se deve ignorar os filtros, monta a url apenas com o próprio parâmetro ignorarfiltros=1 e pagina=[pagina atual]
            if (ignorarFiltros)
            {
                if (urlRedirecionamento.EndsWith("?"))
                    urlRedirecionamento = urlRedirecionamento.Substring(0, urlRedirecionamento.Length - 1);

                //Inclui apenas o parâmetro ignorarFiltros
                //O paginador montará a URL com este parâmetro + o número da página
                url = string.Format("{0}?{1}={2}",
                             urlRedirecionamento,
                             ChavesQueryString.IgnorarFiltros,
                             1);
            }
            else //Não ignorar os filtros
            {
                if (!this.AlgumFiltroInformado)
                {
                    if (urlRedirecionamento.EndsWith("?"))
                        return urlRedirecionamento.Substring(0, urlRedirecionamento.Length - 1);

                    return urlRedirecionamento;
                }

                //As dropdownlists e o paginador se orientam pelas querystrings.
                //Então, executa um auto-redirecionamento passando como parâmetro as seleções dos campos-filtro
                //O paginador montará a URL com estes parâmetros + o número da página
                url = string.Format("{0}{1}{2}{3}",
                             urlRedirecionamento,
                             urlRedirecionamento.IndexOf('?') == -1 ? "?" : string.Empty, //Verifica se precisa anexar no final da URL um ? para depois entrar os parâmetros
                             !this.slcArea.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Area + "=" + URLUtils.URLEncode(this.slcArea.SelectedItem.Text) + "&" : string.Empty,
                             !this.slcSubject.SelectedItem.Value.Equals(valorPadraoSelecione) ? ChavesQueryString.Assunto + "=" + URLUtils.URLEncode(this.slcSubject.SelectedItem.Text) + "&" : string.Empty);
            }

            //Remove o último & se houver
            if (url.Trim().LastIndexOf('&', url.Trim().Length - 1) != -1)
                url = url.Trim().Substring(0, url.Trim().Length - 1);

            return url;
        }

        /// <summary>
        /// Botão Ver Todas estará disponível quando algum filtro tiver sido apontado e paginação estiver habilitada
        /// Caso os filtros estejam sendo ignorados, basta a paginação estar habilitada
        /// Se não, a paginação deve estar habilitada e algum filtro deverá ter sido informado
        /// </summary>
        private void ConfigurarBotaoVerTodas()
        {
            if (Request["showAllButton"] == "false") {
                pnlVertodas.Visible = false;
            }
            //this.btnVerTodas.Enabled =
            //this.btnVerTodas.Visible = this.IgnorarFiltros ?
            //    /* Sim, ignorar -> */  this.PaginacaoHabilitada :
            //    /* Não -> */ this.AlgumFiltroInformado && this.PaginacaoHabilitada;
        }

        /// <summary>
        /// Informa o usuário caso a pesquisa não tenha retornado resultado
        /// </summary>
        /// <param name="algumItemRetornado"></param>
        private void ConfigurarMensagemRetornoPesquisa(bool algumItemRetornado)
        {
            //Considera somente casos em que os filtros não são ignorados e há filtros informados
            //A lógica evita que na primeira vez em que o usuário entrar na página, a mensagem "Nenhum item encontrado" seja mostrada
            if (!this.IgnorarFiltros && this.AlgumFiltroInformado)
                this.ltlMensagem.Text = algumItemRetornado ? string.Empty : RedecardHelper.ObterResource("listagemPerguntasFrequentes_PesquisaSemResultado");
        }
        #endregion
    }
}