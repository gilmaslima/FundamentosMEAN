/*
© Copyright 2014 Rede S.A.
Autor   : Jacques Domingos Freire de Sá
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - ContaCorrente
    /// </summary>
    public class CampoContaCorrente : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoContaCorrente(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() { }

        /// <summary>
        /// Valida conteúdo do controle, se Texto é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);

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
            get { return this.Controle.Text; }
            set { this.Controle.Text = value; }
        }
    }
}