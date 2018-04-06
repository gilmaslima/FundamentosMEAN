/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Text.RegularExpressions;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Telefone
    /// </summary>
    public class CampoTelefone : ICampoNovoAcesso
    {
        /// <summary>
        /// Regular Expression para validação de celulares nos formatos
        /// 8 dígitos "(99) 9999-9999" ou 9 dígitos "(99) 99999-9999".
        /// Considera máscaras
        /// </summary>
        private static Regex RegexTelefone
        {
            get { return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4})\-(?<Parte2>[0-9]{4})$"); }
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoTelefone(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextBox.Attributes["alt"] = "tel";
        }

        /// <summary>
        /// Valida conteúdo do controle, se telefone é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = ValidarTelefone(this.Controle.Text);

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
                    this.Controle.ExibirMensagemErro("Telefone inválido");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida número do telefone com máscara
        /// </summary>
        /// <param name="celular">Telefone</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarTelefone(String telefone)
        {
            return RegexTelefone.Match(telefone).Success;
        }

        /// <summary>
        /// DDD do telefone
        /// </summary>
        public Int32? DDD
        {
            get
            {
                Match match = RegexTelefone.Match(this.Controle.Text);
                if (match.Success)
                    return match.Groups["DDD"].Value.ToInt32Null();
                else
                    return null;
            }
        }

        /// <summary>
        /// Número do telefone (sem DDD)
        /// </summary>
        public Int32? Telefone
        {
            get
            {
                Match match = RegexTelefone.Match(this.Controle.Text);
                if (match.Success)
                    return String.Concat(match.Groups["Parte1"], match.Groups["Parte2"]).ToInt32Null();
                else
                    return null;
            }
        }
    }
}