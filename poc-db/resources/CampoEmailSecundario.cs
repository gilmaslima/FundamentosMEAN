/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - E-mail Secundário
    /// </summary>
    public class CampoEmailSecundario : CampoEmail
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoEmailSecundario(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle()
        {
            this.Controle.MaxLength = 50;
        }

        /// <summary>
        /// Valida conteúdo do controle, se E-mail Secundário é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = ValidarFormatoEmail(this.Email);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }

            //Se preenchido e e-mail inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErro("Email inválido");
                return false;
            }

            //Validações se e-mail foi preenchido
            if (preenchido && valido)
            {
                String msgDominioInvalido = String.Concat(
                    "Domínio @{0} inválido<br/>",
                    "Por favor, insira outro e-mail");
                this.Controle.ExibirMensagemErro(msgDominioInvalido);

                this.Controle.RemoverMensagem();
                return true;
            }

            return true;
        }
    }
}
