using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.Model;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Helper;
using System.Collections.Generic;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Aberto.WebParts.FotosSlideShow
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 05/10/2010
    /// Descrição: SlideShow da Listagem de Fotos
    /// </summary>
    public partial class FotosSlideShowUserControl : UserControlBase
    {
        #region Propriedades

        public string MsgTitulo
        {
            get { return RedecardHelper.ObterResource("fotosSlideShow_Titulo"); }
        }

        public string MsgDescritivo
        {
            get { return RedecardHelper.ObterResource("fotosSlideShow_Descritivo"); }
        }

        /// <summary>
        /// Acesso à Galeria selecionada
        /// </summary>
        private string Galeria
        {
            get
            {
                if (Request.Params[ChavesQueryString.Galeria] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Galeria].ToString());
            }
        }

        private bool ExibirInfoFotos
        {
            get
            {
                return this.WebPart.ExibirInfoFoto;
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private FotosSlideShow WebPart
        {
            get
            {
                return this.Parent as FotosSlideShow;
            }
        }
        #endregion Propriedades

        #region Eventos

        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {            
            this.CarregarFotos(this.Galeria);

            base.OnLoad(e);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carrega a lista de fotos com base galeria apontada no controle de lista
        /// </summary>
        /// <param name="galeria"></param>
        private void CarregarFotos(string galeria)
        {
            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOFoto, FotosItem>>())
                {
                    List<DTOFoto> fotos;

                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        fotos = new List<DTOFoto>(repository.GetItems(foto => foto.Galeria.Equals(galeria))).OrderByDescending(foto => foto.ID).ToList();
                        this.rptSlideShow.DataSource = fotos;
                        this.rptSlideShow.DataBind();

                        //Pega primeira foto da galeria selecionada
                        DTOFoto firstFoto = fotos.FirstOrDefault();

                        if (firstFoto != null)
                        {
                            //Mostra Título e Descrição da primeira foto da galeria selecionada
                            this.GrupoInfoFoto.InnerHtml = string.Format("<p id=\"Titulo\"><strong>{0}: </strong>{1}</p><p id=\"Descricao\"><strong>{2}: </strong>{3}</p>",
                                                                        RedecardHelper.ObterResource("fotosSlideShow_Titulo"), firstFoto.Titulo,
                                                                        RedecardHelper.ObterResource("fotosSlideShow_Descritivo"), firstFoto.Descricao);

                            //Mostra primeira foto da lista quando uma galeria é selecionada
                            this.imgDefault.Src = firstFoto.Url;
                        }

                        //Mostra ou Oculta Informações de Foto
                        if (this.ExibirInfoFotos)
                            GrupoInfoFoto.Visible = true;
                        else
                            GrupoInfoFoto.Visible = false;                        

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
