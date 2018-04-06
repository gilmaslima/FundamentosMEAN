using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper.Web;
using System.Collections.Generic;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web.Controles;

namespace Redecard.Portal.Aberto.WebParts.ListagemReleases
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 08/10/2010
    /// Descrição: Composição do WebPart de Listagem de Releases
    /// </summary>
    public partial class ListagemReleasesUserControl : UserControlBase
    {
        #region Variáveis

        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listagemReleases_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;

        #endregion

        #region Propriedades
        /// <summary>
        /// Nome da âncora utilizada para posicionamento do usuário na página
        /// </summary>
        public string AncoraListagem
        {
            get
            {
                return "wptListagem";
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
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemReleases WebPart
        {
            get
            {
                return this.Parent as ListagemReleases;
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
        /// Acesso ao Mes selecionado
        /// </summary>
        private string Mes
        {
            get
            {
                if (String.IsNullOrEmpty(Request.Params[ChavesQueryString.Mes]))
                {
                    return DateTime.Now.ToString("MM/yyyy");
                }
                else
                {
                    return URLUtils.URLDecode(Request.Params[ChavesQueryString.Mes].ToString());
                }
            }
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            switch (this.WebPart.tipoVisao)
            {
                case ListagemReleases.Visao.Resumo:
                    this.CarregarMeses();
                    this.CarregarReleases(this.Mes);
                    break;
                case ListagemReleases.Visao.Detalhe:
                    this.CarregarReleasesDetalhe();
                    break;
                default:
                    this.CarregarMeses();
                    this.CarregarReleases(this.Mes);
                    break;
            }

            // modificar modo de exibição
            this.HandlerShow();

            base.OnLoad(e);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// 
        /// </summary>
        protected void HandlerShow()
        {
            if (this.WebPart.tipoVisao == ListagemReleases.Visao.Resumo)
            {
                pnlResumo.Visible = true;
                pnlDetalhe.Visible = false;
            }
            else if (this.WebPart.tipoVisao == ListagemReleases.Visao.Detalhe)
            {
                pnlResumo.Visible = false;
                pnlDetalhe.Visible = true;
            }
        }

        /// <summary>
        /// Monta uma string JavaScript de redirecionamento com base nos parâmetros informados
        /// </summary>
        /// <param name="urlAtual"></param>
        /// <param name="nomeParametro">Nome do parâmetro que será utilizado para o valor do item selecionado no controle. Exemplo: PAGINA=1</param>
        /// <param name="nomeAncora"></param>
        /// <param name="objeto"></param>
        /// <returns></returns>
        protected virtual string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora, string objeto)
        {
            return string.Format("window.location.href='{0}{1}{2}=' + window.encodeURIComponent({3}.options[{3}.selectedIndex].value) + '#{4}'; return false;",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro,
                                objeto,
                                nomeAncora);
        }

        /// <summary>
        /// Carrega a DropDownList de Mes de Releases
        /// Os meses são obtidos a partir da listagem de releases existentes no repositório, e as duplicatas são eliminadas.
        /// Por último a coleção é ordenada alfabeticamente
        /// </summary>
        private void CarregarMeses()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Mes e Página
            string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Mes.ToUpper()) || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcMonths.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle Button ao selecionar um novo item
            this.btnOK.Attributes.Add("onclick", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Mes, this.AncoraListagem, this.slcMonths.UniqueID));

            //Obtém os meses de release para carregamento
            IList<string> meses = this.ObterMeses();

            //Carrega o controle com os meses
            if (meses != null)
                meses.ToList().ForEach(g => this.slcMonths.Items.Add(new ListItem(string.Format("{0} / {1}", g.ToDateTime().ToString("MMMM"), g.ToDateTime().ToString("yyyy")).ToUpper(), g)));

            //Adiciona um primeiro item no controle
            this.slcMonths.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona o mes atual, se informado
            ListItem liMes = this.slcMonths.Items.FindByValue(this.Mes) != null ? this.slcMonths.Items.FindByValue(this.Mes) : this.slcMonths.Items.FindByValue(DateTime.Now.ToString("MM/yyyy"));

            if (liMes != null)
            {
                liMes.Selected = true;
                lblMonth.Text = liMes.Text == textoPadraoSelecione ? "" : liMes.Text;
            }
            else
                this.slcMonths.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita os meses de releases com o Repositório de Releases
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterMeses()
        {
            IList<string> meses = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTORelease, ReleasesItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        meses = (from rel in repository.GetAllItems()
                                 select rel.Data.Value.ToString("MM/yyyy")).OrderBy(r => r).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return meses;
        }

        /// <summary>
        /// Carrega a lista de releases com base no mes apontado no controle de lista
        /// </summary>
        /// <param name="data"></param>
        private void CarregarReleases(string data)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTORelease, ReleasesItem>>())
                {
                    ListaPaginada<DTORelease> releases;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        releases = new ListaPaginada<DTORelease>(repository.GetItems(release => release.Data.Value.ToString("MM/yyyy").Equals(data)).OrderByDescending(release => release.ID), this.Paginador.Pagina, this.ItensPorPagina);

                        if (releases.Count > 0)
                        {
                            this.rptListagemReleases.DataSource = releases;
                            this.rptListagemReleases.DataBind();

                            this.Paginador.MontarPaginador(releases.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                        }
                        else
                        {
                            ltEmptyResumo.Visible = true;
                        }
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Carrega a lista de releases para visão detalhe, exibe apenas a quantidade de itens informada na propriedade da WebPart
        /// </summary>        
        private void CarregarReleasesDetalhe()
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTORelease, ReleasesItem>>())
                {
                    List<DTORelease> releases = null;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        releases = new List<DTORelease>(repository.GetAllItems().Take(this.WebPart.QuantidadeItensPorPagina).OrderBy(release => release.ID));
                    });

                    if (releases.Count > 0)
                    {
                        this.rptListagemReleasesDetalhe.DataSource = releases;
                        this.rptListagemReleasesDetalhe.DataBind();
                    }
                    else
                    {
                        ltEmptyDetalhe.Visible = true;
                    }
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }
        #endregion
    }
}
