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
using Redecard.Portal.Helper;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Helper.Web.Controles;
using System.Web.Script.Services;
using System.Web.Services;
using Redecard.Portal.Helper.Validacao;

namespace Redecard.Portal.Aberto.WebParts.ListagemVideos
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ListagemVideosUserControl : UserControlBase
    {
        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {

            if (ValidarCampos())
            {
                //Obtém a URL pedindo para ignorar os parâmetros Mes e Página
                string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.DataDeInicio.ToUpper())
                                                        || gal.Trim().ToUpper().Equals(ChavesQueryString.DataDeFim.ToUpper())
                                                        || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

                string paramData = string.Format("window.location.href='{0}{1}{2}={3}&{4}={5}#{6}';",
                                    urlAtual,
                                    urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                    ChavesQueryString.DataDeInicio,
                                    DataDeInicio.Text,
                                    ChavesQueryString.DataDeFim,
                                    DataDeFim.Text,
                                    this.AncoraListagem);

                //Adiciona o evento ao controle Button 
                this.btnOK.Attributes.Add("onclick", paramData);

                this.CarregarVideos(DataDeInicio.Text + " 00:00:00", DataDeFim.Text + " 23:59:59");
            }

            base.OnLoad(e);
        }

        protected void lnkExibicoes_Click(object sender, EventArgs e)
        {
            GravaExecucaoVideo(((LinkButton)sender).CommandArgument.ToInt());
        }

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
        private ListagemVideos WebPart
        {
            get
            {
                return this.Parent as ListagemVideos;
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
        /// Acesso a Data De Inicio Selecionada
        /// </summary>
        private string PesquisaDataDeInicio
        {
            get
            {
                if (Request.Params[ChavesQueryString.DataDeInicio] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.DataDeInicio].ToString() + " 00:00:00");
            }
        }

        /// <summary>
        /// Acesso a Data De Fim Selecionada
        /// </summary>
        private string PesquisaDataDeFim
        {
            get
            {
                if (Request.Params[ChavesQueryString.DataDeFim] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.DataDeFim].ToString() + " 23:59:59");
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Atualiza no repositório a Lista de Vídeos com a quantidade de vídeos exibidos
        /// </summary>
        /// <param name="lID">ID do Vídeo</param>
        private int GravaExecucaoVideo(int lID)
        {
            int lReturn = 0;
            try
            {
                DTOVideo updVideo;

                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOVideo, VídeosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        updVideo = repository.GetItems(video => video.Id.Equals(lID)).FirstOrDefault();

                        if (updVideo != null)
                        {
                            updVideo.NumeroDeExibicoes++;

                            repository.Persist(updVideo);
                        }

                        lReturn = repository.GetItems(video => video.Id.Equals(lID.ToInt())).FirstOrDefault().NumeroDeExibicoes.ToInt();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return lReturn;
        }

        /// <summary>
        /// Carrega a lista de videos com base no período
        /// </summary>
        /// <param name="DataDe"></param>
        /// <param name="DataAte"></param>
        private void CarregarVideos(string DataDe, string DataAte)
        {            
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOVideo, VídeosItem>>())
                {
                    ListaPaginada<DTOVideo> videos;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        videos = new ListaPaginada<DTOVideo>(repository.GetItems(video => video.DataDeCriação.Value.CompareTo(DataDe.ToDateTime()) >= 0 &&
                                                                                          video.DataDeCriação.Value.CompareTo(DataAte.ToDateTime()) <= 0).OrderByDescending(video => video.ID), this.Paginador.Pagina, this.ItensPorPagina);

                        this.rptListagemVideos.DataSource = videos;
                        this.rptListagemVideos.DataBind();

                        if (videos.Count > 0)
                        {
                            this.Paginador.MontarPaginador(videos.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                        }
                        else
                        {
                            msgErro.Visible = true;
                            msgErro.Text = RedecardHelper.ObterResource("listagemVideos_NenhumItemEncontrado");
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
        /// Carrega a lista de videos com base no período
        /// </summary>
        private void CarregarVideos(string data)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOVideo, VídeosItem>>())
                {
                    ListaPaginada<DTOVideo> videos;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        videos = new ListaPaginada<DTOVideo>(repository.GetItems(video => video.DataDeCriação.Value.ToString("dd/MM/yyyy").Equals(data)).OrderByDescending(video => video.ID), this.Paginador.Pagina, this.ItensPorPagina);

                        this.rptListagemVideos.DataSource = videos;
                        this.rptListagemVideos.DataBind();

                        this.Paginador.MontarPaginador(videos.TotalItens, this.ItensPorPagina, this.AncoraListagem);
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Valida se os campos Data De e Data Até foram preenchidos e são datas validas
        /// </summary>
        /// <returns>True/False</returns>
        private bool ValidarCampos()
        {
            SumarioValidacao sumarioValidacao = new SumarioValidacao();

            if (string.IsNullOrEmpty(DataDeInicio.Text))
                sumarioValidacao.AdicionarInconsistencia(new Inconsistencia("", RedecardHelper.ObterResource("listagemVideos_DataDeInicio_NaoInformada")));
            else
            {
                if (DataDeInicio.Text.ToDateTime() == DateTime.MinValue)
                    sumarioValidacao.AdicionarInconsistencia(new Inconsistencia("", RedecardHelper.ObterResource("listagemVideos_DataDeInicio_Invalida")));
            }

            if (string.IsNullOrEmpty(DataDeFim.Text))
                sumarioValidacao.AdicionarInconsistencia(new Inconsistencia("", RedecardHelper.ObterResource("listagemVideos_DataDeFim_NaoInformada")));
            else
            { 
                if (DataDeFim.Text.ToDateTime() == DateTime.MinValue)
                    sumarioValidacao.AdicionarInconsistencia(new Inconsistencia("", RedecardHelper.ObterResource("listagemVideos_DataDeFim_Invalida")));
            }

            if (!sumarioValidacao.Valido)
            {
                msgErro.Visible = true;
                msgErro.Text = sumarioValidacao.Inconsistencias.FirstOrDefault().Mensagem;
            }
            else
                msgErro.Visible = false;

            return sumarioValidacao.Valido;
        }

        #endregion
       
    }
}
