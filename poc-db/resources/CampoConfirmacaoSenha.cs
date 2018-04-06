/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Redecard.PN.OutrasEntidades
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Confirmação de Senha
    /// </summary>
    public class CampoConfirmacaoSenha : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoConfirmacaoSenha(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextMode = TextBoxMode.Password;
            this.Controle.TextBox.MaxLength = 20;
            //this.Controle.TextBox.PreRender += new EventHandler(TextBox_PreRender);
        }

        /// <summary>
        /// Valida conteúdo do controle, se Texto é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            var conteudoComparacao = default(String);

            if (ControleAssociado is TextBox)
                conteudoComparacao = ((TextBox)ControleAssociado).Text;
            else if (ControleAssociado is CampoNovoAcesso)
                conteudoComparacao = ((CampoNovoAcesso)ControleAssociado).Text;

            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = String.Compare(this.ConfirmacaoSenha, conteudoComparacao) == 0;

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }

            //Se preenchido e número inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErro("Senhas não confirmada");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Controle comparado
        /// </summary>
        public Control ControleAssociado
        {
            get { return this.Controle.ControleAssociado; }
        }

        /// <summary>
        /// Confirmação de Senha
        /// </summary>
        public String ConfirmacaoSenha
        {
            get { return this.Controle.Text; }
            set { this.Controle.Text = value; }
        }

        ///// <summary>
        ///// Solicitação de Canais: tratamento para exibir os caracteres no campo de senha.
        ///// Não recomendado pois expõe a senha no source da página.
        ///// </summary>
        //private void TextBox_PreRender(object sender, EventArgs e)
        //{
        //    this.Controle.TextBox.Attributes.Add("value", this.ConfirmacaoSenha);
        //}
    }
}