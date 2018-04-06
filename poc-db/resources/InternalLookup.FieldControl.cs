#region Used Namespaces
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
#endregion

namespace Redecard.Portal.Aberto.CustomFields
{
    /// <summary>
    ///   Classe responsável pela UI do campo.
    /// </summary>
    internal sealed class InternalLookupFieldControl : TextField
    {
        #region Fields
        /// <summary>
        ///   DropDownList onde o usuário pode selecionar um dos valores já existentes.
        /// </summary>
        private DropDownList _lookup;

        /// <summary>
        ///   TextBox onde o usuário pode escrever um novo valor.
        /// </summary>
        private TextBox _textBox;

        /// <summary>
        ///   Label onde será disponibilizado o valor atual.
        /// </summary>
        private Label _valueForDisplay;
        #endregion

        #region Properties
        /// <summary>
        ///   Retorna o nome do template de renderização padrão para o controle.
        /// </summary>
        /// <returns>
        ///   O nome de um template de renderização em um arquivo .ascx armazenado na pasta 
        ///   "C:\program files\common files\microsoft shared\web server extensions\14\template\controltemplates"
        /// </returns>
        protected override string DefaultTemplateName
        {
            get { return ControlMode == SPControlMode.Display ? DisplayTemplateName : "InternalLookupFieldControl"; }
        }

        /// <summary>
        ///   Retorna ou armazena o nome do template de renderização que pode ser usado pelo controle para
        ///   renderizar no formulário de exibição.
        /// </summary>
        /// <returns>
        ///   O nome do template de renderização
        /// </returns>
        public override string DisplayTemplateName
        {
            get { return "InternalLookupFieldControlForDisplay"; }
            set { base.DisplayTemplateName = value; }
        }

        /// <summary>
        ///   Retorna ou armazena o valor na UI do controle.
        /// </summary>
        /// <returns>
        ///   Um objeto que representa o valor do campo que aparece na UI.
        /// </returns>
        public override object Value
        {
            get
            {
                EnsureChildControls();
                return base.Value;
            }
            set
            {
                EnsureChildControls();
                base.Value = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        ///   Cria os controles que participam da UI do campo.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (Field == null) return;

            //Renderiza os controles filhos completamente.
            base.CreateChildControls();

            // Associa os controles do arquivo .ascx com os campos criados na classe.
            textBox = (TextBox) TemplateContainer.FindControl("TextField");
            _textBox = (TextBox) TemplateContainer.FindControl("InternalLookupTextBox");
            _lookup = (DropDownList) TemplateContainer.FindControl("InternalLookupDropDownList");
            _valueForDisplay = (Label) TemplateContainer.FindControl("InternalLookupValueForDisplay");

            if (ControlMode != SPControlMode.Display)
            {
                if (!Page.IsPostBack)
                {
                    //Insere uma chamada a uma função javascript que garante a alteração entre os campos pelo usuário.
                    _textBox.Attributes.Add("onfocus",
                                            string.Format("javascript:OnFocus('{0}')",
                                                          TemplateContainer.FindControl("RdbInternalLookupTextBox").ClientID));
                    _lookup.Attributes.Add("onfocus",
                                           string.Format("javascript:OnFocus('{0}')",
                                                         TemplateContainer.FindControl("RdbInternalLookupDropDownList").ClientID));

                    //Preenche o DropDownList com os valores já armazenados no campo por outros itens.
                    FillLookup();

                    //Caso seja o formulário de edição, então persiste o valor correto no DropDownList.
                    if (ControlMode == SPControlMode.Edit)
                    {
                        ((RadioButton) TemplateContainer.FindControl("RdbInternalLookupDropDownList")).Checked = true;
                        if (!object.ReferenceEquals(ItemFieldValue, null))
                            _lookup.SelectedValue = ItemFieldValue.ToString();
                    }
                }
            }
            else // Controle foi carregado no formulário de exibição.
            {
                //Disponibiliza o valor atual do campo.
                if (!object.ReferenceEquals(ItemFieldValue, null))
                    _valueForDisplay.Text = ItemFieldValue.ToString();
            }
        }

        /// <summary>
        ///   Atualiza o valor do campo com o que foi escolhido pelo usuário.
        /// </summary>
        public override void UpdateFieldValueInItem()
        {
            //Verifica qual radio está selecionado e preenche o valor do campo.
            //OBS.:O campo textbox é o principal pois é utilizado pelo pai TextField.
            textBox.Text = ((RadioButton) TemplateContainer.FindControl("RdbInternalLookupTextBox")).Checked
                               ? _textBox.Text
                               : _lookup.SelectedValue;

            base.UpdateFieldValueInItem();
        }

        /// <summary>
        ///   Preenche o DropDownList com os valores já inseridos em outros itens no
        ///   mesmo campo.
        /// </summary>
        private void FillLookup()
        {
            //Busca por valores já inseridos, retira os repetidos.
            var fullData = (List.Items.Cast<SPListItem>().Select(item => item[Field.Title] as string)).Distinct().ToList();
            //Coloca em ordem alfabética.
            fullData.Sort();

            //Adiciona no DropDownList.
            fullData.ForEach(data => _lookup.Items.Add(data));
        }
        #endregion
    }
}