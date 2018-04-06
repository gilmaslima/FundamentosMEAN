/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Confirmação de E-mail
    /// </summary>
    public class CampoConfirmacaoEmail : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoConfirmacaoEmail(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle()
        {
            this.Controle.MaxLength = 50;
        }

        /// <summary>
        /// Valida conteúdo do controle, se confirmação de e-mail é válido
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
            Boolean valido = String.Compare(this.ConfirmacaoEmail.ToUpper(), conteudoComparacao.ToUpper()) == 0;

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
                    this.Controle.ExibirMensagemErro("Campo de confirmação<br/>diferente do campo de e-mail");
                return false;
            }

            if (exibirMensagem)
            {
                if(preenchido)
                    this.Controle.ExibirMensagemSucesso("E-mail confirmado");
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
        /// Confirmação de E-mail
        /// </summary>
        public String ConfirmacaoEmail
        {
            get { return this.Controle.Text; }
            set { this.Controle.Text = value.ToString(); }
        }
    }
}
