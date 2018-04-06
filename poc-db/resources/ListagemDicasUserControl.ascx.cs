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

namespace Redecard.Portal.Aberto.WebParts.ListagemDicas
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 10/10/2010
    /// Descrição: Composição do WebPart de Listagem de Dicas
    /// </summary>
    public partial class ListagemDicasUserControl : UserControlBase
    {
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

        private Redecard.Portal.Aberto.Model.TipoDaDica TipoDaDica
        {
            get
            {
                return this.WebPart.TipoDaDica;
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemDicas WebPart
        {
            get
            {
                return this.Parent as ListagemDicas;
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
        /// Acesso à categoria selecionada
        /// </summary>
        private string Categoria
        {
            get
            {
                if (Request.Params[ChavesQueryString.Categoria] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Categoria].ToString());
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
            if (!String.IsNullOrEmpty(Request.Form["ClickID"]))
            {
                this.AtualizarExibicoes();
            }
            else 
            {
                this.CarregarDicas();
            }
            base.OnLoad(e);
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
        protected virtual string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora)
        {
            return string.Format("window.location.href='{0}{1}{2}=' + window.encodeURIComponent(this.options[this.selectedIndex].value) + '#" + nomeAncora + "';",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro);
        }

        /// <summary>
        /// Carrega a lista de dicas por Tipo de Dica (Komerci ou Foneshop)
        /// </summary>        
        private void CarregarDicas()
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODica, DicasItem>>())
                {
                    ListaPaginada<DTODica> dicas;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        dicas = new ListaPaginada<DTODica>(repository.GetItems(dica => dica.TipoDaDica.Equals(TipoDaDica)).OrderByDescending(dica => dica.ID), this.Paginador.Pagina, this.ItensPorPagina);

                        //Altera a data de publicação recuperada na lista para a data da publicação no formato:
                        //Publicado no dia {dia (dd)} de {mes (mmm)} de {ano (yyyy)}
                        dicas.ForEach(dica => dica.DataPublicacao =
                                      string.Format(RedecardHelper.ObterResource("listagemDicas_datapublicacao"),
                                                    dica.DataPublicacao.ToDateTime().ToString("dd"),
                                                    dica.DataPublicacao.ToDateTime().ToString("MMMM"),
                                                    dica.DataPublicacao.ToDateTime().ToString("yyyy")));

                        this.rptListagemDicas.DataSource = dicas;
                        this.rptListagemDicas.DataBind();

                        this.Paginador.MontarPaginador(dicas.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Atualizar a quantidade de exibição da dica (Komerci ou Foneshop)
        /// </summary>        
        private void AtualizarExibicoes()
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODica, DicasItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        DTODica dica = repository.GetItems(item => item.Id.ToString().Equals(Request.Form["ClickID"])).FirstOrDefault();
                        dica.NumeroDeExibicoes += 1;
                        repository.Persist(dica);
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Carrega a lista de dicas com base na categoria informada
        /// </summary>
        /// <param name="categoria"></param>
        private void CarregarDicas(string categoria)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTODica, DicasItem>>())
                {
                    ListaPaginada<DTODica> dicas;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        dicas = new ListaPaginada<DTODica>(repository.GetItems(dica => dica.Categoria.Equals(categoria) && dica.TipoDaDica.Equals(TipoDaDica)).OrderByDescending(dica => dica.ID), this.Paginador.Pagina, this.ItensPorPagina);

                        //Altera a data de publicação recuperada na lista para a data da publicação no formato:
                        //Publicado no dia {dia (dd)} de {mes (mmm)} de {ano (yyyy)}
                        dicas.ForEach(dica => dica.DataPublicacao =
                                      string.Format(RedecardHelper.ObterResource("listagemDicas_datapublicacao"),
                                                    dica.DataPublicacao.ToDateTime().ToString("dd"),
                                                    dica.DataPublicacao.ToDateTime().ToString("MmMM"),
                                                    dica.DataPublicacao.ToDateTime().ToString("yyyy")));

                        this.rptListagemDicas.DataSource = dicas;
                        this.rptListagemDicas.DataBind();

                        this.Paginador.MontarPaginador(dicas.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                    });
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
