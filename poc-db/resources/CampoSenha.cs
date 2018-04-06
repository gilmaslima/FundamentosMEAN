/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - Senha
    /// </summary>
    public class CampoSenha : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoSenha(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextBox.TextMode = TextBoxMode.Password;
            this.Controle.TextBox.MaxLength = 20;
            this.Controle.PainelSenha.Visible = true;
            //this.Controle.TextBox.PreRender += new EventHandler(TextBox_PreRender);
            
            //Chama no load, para gerar o conteúdo do box dos itens de segurança
            this.Validar(false);
        }

        /// <summary>
        /// Valida conteúdo do controle, se Texto é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Senha);
            var forcaSenha = default(String);
            var itensSeguranca = default(List<KeyValuePair<TipoMensagemNovoAcesso, String>>);
            Boolean atendeRequisitos = ValidarSenha(this.Senha, out forcaSenha, out itensSeguranca);

            //Cria mensagens dos itens de segurança
            this.Controle.ExibirMensagensSenhaItemSeguranca(forcaSenha, itensSeguranca);

            //Se não preenchido, não há necessidade de gerar painel de requisitos de senha
            if (!preenchido)
            {
                itensSeguranca.Clear();
                this.Controle.PainelSenha.Style.Add(HtmlTextWriterStyle.Display, "none");                
            }

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone("*");
                return false;
            }

            //Se preenchido, deve atender requisitos
            if (preenchido && !atendeRequisitos)
                return false;
                        
            return true;
        }

        /// <summary>
        /// Validação da senha, de acordo com os critérios de segurança
        /// </summary>
        /// <param name="senha">Senha</param>
        /// <param name="forcaSenha">Força da Senha calculada</param>
        /// <param name="itens">Itens de segurança atendidos/reprovados</param>
        /// <returns>Se a senha é válida ou não</returns>
        public static Boolean ValidarSenha(String senha, 
            out String forcaSenha, out List<KeyValuePair<TipoMensagemNovoAcesso, String>> itens)
        {         
            itens = new List<KeyValuePair<TipoMensagemNovoAcesso, String>>();
            forcaSenha = String.Empty;

            var maiusculas = senha.Count(c => Char.IsUpper(c));
            var minusculas = senha.Count(c => Char.IsLower(c));
            var numeros = senha.Count(c => Char.IsNumber(c));            
            var emBranco = senha.Count(c => Char.IsWhiteSpace(c));
            var tamanho = senha.Length;
            var invalidos = tamanho - maiusculas - minusculas - numeros - emBranco;

             //Qualquer caractere inválido, ou espaço em branco, invalida a senha
            if(emBranco > 0 || invalidos > 0)
                forcaSenha = "Inválida";        
            //Alta: ter mais do que 10 caracteres, 2 ou mais letras com “case” diferente e 2 ou mais números
            else if(tamanho > 10 && numeros >= 2 && maiusculas >= 2 && minusculas >= 2)
                forcaSenha = "Alta";
            //Média: entre 8 e 10 caracteres, 2 ou mais letras com “case” diferente e 2 ou mais números
            else if(tamanho >= 8 && numeros >= 2 && maiusculas >= 2 && minusculas >= 2)
                forcaSenha = "Média";
            //Fraca: Obedecer o mínimo das regras, ou seja, 8 caracteres, 1 letra com “case” 
            //diferente (7 maiúsculas e 1 minúscula) ou vice versa e somente 1 número
            else if(tamanho >= 8 && maiusculas >= 1 && minusculas >= 1 && numeros >= 1)
                forcaSenha = "Fraca";
            //Inválida: Sempre que um ou mais dos itens de segurança não forem obedecidos, 
            //não devendo o usuário prosseguir o cadastramento.
            else
                forcaSenha = "Inválida";

            itens.Add(CriarItemSeguranca(tamanho >= 6, "Possuir entre 6 e 20 caracteres"));
            itens.Add(CriarItemSeguranca(maiusculas >= 1 && minusculas >= 1, "Letras maiúsculas e minúsculas"));
            itens.Add(CriarItemSeguranca(numeros >= 1, "Pelo menos 1 número"));
            itens.Add(CriarItemSeguranca(emBranco == 0, "Não conter espaços em branco"));
            itens.Add(CriarItemSeguranca(invalidos == 0, "Não conter caracteres especiais"));

            return String.Compare("Inválida", forcaSenha) != 0;
        }

        /// <summary>
        /// Método auxiliar para instanciação da classe
        /// </summary>
        /// <param name="sucesso">Boolean indicando qual tipo de mensagem gerar</param>
        /// <param name="mensagem">Mensagem a ser incluída</param>
        /// <returns>Instância</returns>
        private static KeyValuePair<TipoMensagemNovoAcesso,String> CriarItemSeguranca(Boolean sucesso, String mensagem)
        {
            return new KeyValuePair<TipoMensagemNovoAcesso,String>(
                sucesso ? TipoMensagemNovoAcesso.Sucesso : TipoMensagemNovoAcesso.Erro, mensagem);
        }

        /// <summary>
        /// Senha
        /// </summary>
        public String Senha
        {
            get { return this.Controle.Text; }
            set { this.Controle.Text = value; }
        }

        /// <summary>
        /// Senha criptografada
        /// </summary>
        public String SenhaCriptografada
        {
            get { return Criptografar(this.Senha); }
        }

        /// <summary>
        /// Criptografar senha
        /// </summary>
        public static String Criptografar(String senha)
        {
            return EncriptadorSHA1.EncryptString(senha);
        }

        ///// <summary>
        ///// Solicitação de Canais: tratamento para exibir os caracteres no campo de senha.
        ///// Não recomendado pois expõe a senha no source da página.
        ///// </summary>
        //private void TextBox_PreRender(object sender, EventArgs e)
        //{
        //    this.Controle.TextBox.Attributes.Add("value", this.Senha);
        //}
    }
}