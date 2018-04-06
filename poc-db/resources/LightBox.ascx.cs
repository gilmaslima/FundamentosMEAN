/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// Controle para exibição de LightBox
    /// </summary>
    [ParseChildren(true)]
    public partial class LightBox : UserControl, INamingContainer
    {
        #region [ Variáveis ]

        /// <summary>
        /// ITemplate conteúdo
        /// </summary>
        private ITemplate conteudo;

        /// <summary>
        /// ITemplate footer
        /// </summary>
        private ITemplate footer;

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Conteúdo do LightBox
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(LightBox))]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate Conteudo 
        {
            get { return conteudo; }
            set { conteudo = value; }
        }

        /// <summary>
        /// Conteúdo do Footer do LightBox
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(LightBox))]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate Footer 
        {
            get { return footer; }
            set { footer = value; }
        }

        /// <summary>
        /// Título do LightBox
        /// </summary>
        public String Titulo
        {
            get { return ltrTitulo.Text; }
            set { ltrTitulo.Text = value; }
        }

        /// <summary>
        /// Classe CSS aplicada ao LightBox
        /// </summary>
        public String CssClass 
        { 
            set
            {
                List<String> classes = pnlModal.CssClass.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<String> novasClasses = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                classes.AddRange(novasClasses);
                pnlModal.CssClass = String.Join(" ", classes.Distinct().ToArray());
            }
        }

        /// <summary>
        /// Exibe ou não o Botão Fechar Modal (Close Modal)
        /// </summary>
        public Boolean ExibirBotaoFechar
        {
            set { hlkCloseModal.Visible = value; }
            get { return hlkCloseModal.Visible; }
        }

        #endregion

        #region [ Métodos Sobrescritos ]

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            //Instancia os conteúdos customizados no placeholder do controle
            if(Conteudo != null)
                Conteudo.InstantiateIn(pnlConteudo);
            if(Footer != null)
                Footer.InstantiateIn(pnlFooter);

            //Armazena ID e ClientID no controle
            pnlControle.Attributes["clientid"] = this.ClientID;
            pnlControle.Attributes["ucid"] = this.ID;            
        }

        #endregion


        #region [ Métodos Públicos ]

        /// <summary>
        /// Exibe o controle no retorno do PostBack
        /// </summary>
        public void Exibir()
        {
            pnlControle.Attributes["exibirOnLoad"] = Boolean.TrueString.ToLower();
        }

        #endregion
    }
}