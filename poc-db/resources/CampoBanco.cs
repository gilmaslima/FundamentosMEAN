/*
© Copyright 2014 Rede S.A.
Autor   : Jacques Domingos Freire de Sá
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using System;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Banco
    /// </summary>
    public class CampoBanco : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoBanco(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() {
            this.Controle.DdlNovoAcesso.Visible = true;
            this.Controle.TextBox.Visible = false;
            this.ConsultarBancos();
        }

        /// <summary>
        /// Valida conteúdo do controle, se Texto é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Value);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Texto preenchido no controle
        /// </summary>
        public String Texto
        {
            get
            {
                if (this.Controle.DdlNovoAcesso.SelectedItem != null)
                    return this.Controle.DdlNovoAcesso.SelectedItem.Text;

                return string.Empty;
            }
            set {
                var item = this.Controle.DdlNovoAcesso.Items.FindByText(value);
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// Retorna o valor selecionado no DropDownList
        /// </summary>
        public String Value
        {
            get
            {
                if (this.Controle.DdlNovoAcesso.SelectedItem != null)
                    return this.Controle.DdlNovoAcesso.SelectedItem.Value;

                return string.Empty;
            }
            set
            {
                var item = this.Controle.DdlNovoAcesso.Items.FindByValue(value);
                if (item != null)
                {
                    item.Selected = true;
                }
                else if (this.Controle.DdlNovoAcesso.Items.Count > 0)
                {
                    this.Controle.DdlNovoAcesso.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Consulta os bancos e preenche combo
        /// </summary>
        private void ConsultarBancos()
        {
            using (Logger Log = Logger.IniciarLog("Consultando bancos e preenchendo combos"))
            {
                using (var entidadeClient = new EntidadeServico.EntidadeServicoClient())
                {
                    var bancos = entidadeClient.ConsultarBancosConfirmacaoPositiva();
                    foreach (EntidadeServico.Banco banco in bancos)
                    {
                        this.Controle.DdlNovoAcesso.Items.Add(new ListItem(
                            string.Concat(banco.Codigo.ToString(), " - " , banco.Descricao), 
                            banco.Codigo.ToString()));
                    }
                }
            }
        }
    }
}