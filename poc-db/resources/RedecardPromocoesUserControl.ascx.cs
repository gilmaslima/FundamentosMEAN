using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper;
using System.Collections.Generic;
using Redecard.Portal.Helper.Web;
using System.Web;

namespace Redecard.Portal.Aberto.WebParts.RedecardPromocoes
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data da criação: 14/10/2010
    /// Descrição: Composição do WebPart de Conheça as Promoções, esta WebPart utiliza uma ToolPart customizada
    /// </summary>
    public partial class RedecardPromocoesUserControl : UserControlBase
    {
        #region Variáveis

        private static string textoPadraoSelecione =  RedecardHelper.ObterResource("conhecaPromocoes_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;

        private ITraducao<string, IList<Promocao>> tradutor = new TradutorDeStringParaPromocao();

        #endregion

        #region Propriedades Customizadas da WebPart

        protected IList<Promocao> ListaPromocao
        {
            get
            {
                if (string.IsNullOrEmpty(this.WebPart.Promocao))
                    return new List<Promocao>();
                else
                    return this.tradutor.Traduzir(this.WebPart.Promocao);
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private RedecardPromocoes WebPart
        {
            get
            {
                return this.Parent as RedecardPromocoes;
            }
        }

        #endregion

        #region Eventos

        protected override void OnLoad(EventArgs e)
        {
            this.CarregarPromocoes();

            base.OnLoad(e);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carrega a DropDownList de Promoções
        /// As promoções são obtidas através da ToolPart customizada para a WebPart de Promoções        
        /// </summary>
        private void CarregarPromocoes()
        {
            //Esvazia o controle
            this.slcPromocoes.Items.Clear();

            //Adiciona atributo onclick no button com o valor do item selecionado no combo
            this.btnOk.Attributes.Add("onclick", string.Format("window.location = {0}.options[{0}.selectedIndex].value ; return false;", this.slcPromocoes.UniqueID));

            //Adiciona um primeiro item no controle
            this.slcPromocoes.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Ordena lista de promoções pelo perfil
            this.ListaPromocao.OrderBy(p => p.Perfil);

            //Carrega o controle com os perfis
            this.ListaPromocao.ToList().ForEach(p => this.slcPromocoes.Items.Add(new ListItem(p.Perfil, p.Link)));            

            //Seleciona o mes atual, se informado
            ListItem liPromo = this.slcPromocoes.Items.FindByValue("");

            if (liPromo != null)
            {
                liPromo.Selected = true;
            }
            else
                this.slcPromocoes.SelectedValue = valorPadraoSelecione;
        }

        #endregion

    }
}
