/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Número do Estabelecimento
    /// </summary>
    public class CampoEstabelecimento : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoEstabelecimento(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextBox.Attributes["alt"] = "pv";
            
            //Controle com o ícone de ajuda de Estabelecimento
            this.Controle.PainelAjuda.Visible = this.Controle.ExibirIconeAjuda;
        }

        /// <summary>
        /// Valida conteúdo do controle, se Número Estabelecimento é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {            
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Int32? pv = this.NumeroEstabelecimento;

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }
         
            //Se preenchido e número inválido, erro
            if (preenchido && !pv.HasValue)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErro("Estabelecimento inválido");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int32? NumeroEstabelecimento
        {
            get { return this.Controle.Text.ToInt32Null(); }
            set { this.Controle.Text = value.ToString(); }
        }
    }
}