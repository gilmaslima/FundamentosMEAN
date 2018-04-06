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
    /// Classe para implementação do CampoNovoAcesso - Celular
    /// </summary>
    public class CampoCelular : ICampoNovoAcesso
    {
        /// <summary>
        /// Regular Expression para validação de celulares nos formatos
        /// 8 dígitos "(99) 9999-9999" ou 9 dígitos "(99) 99999-9999".
        /// Considera máscaras
        /// </summary>
        private static Regex RegexCelular 
        { 
            get { return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4,5})\-(?<Parte2>[0-9]{4})$"); } 
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoCelular(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() { }

        /// <summary>
        /// Valida conteúdo do controle, se celular é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = ValidarCelular(this.Controle.Text);

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
                    this.Controle.ExibirMensagemErro("Celular inválido");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Valida número do celular com máscara 8 ou 9 dígitos
        /// </summary>
        /// <param name="celular">Celular</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarCelular(String celular)
        {
            return RegexCelular.Match(celular).Success;
        }

        /// <summary>
        /// DDD do celular
        /// </summary>
        public Int32? DDD
        {
            get
            {                
                Match match = RegexCelular.Match(this.Controle.Text);
                if (match.Success)
                    return match.Groups["DDD"].Value.ToInt32Null();
                else
                    return null;
            }
        }

        /// <summary>
        /// Número do celular (sem DDD)
        /// </summary>
        public Int32? Numero
        {
            get
            {
                Match match = RegexCelular.Match(this.Controle.Text);
                if (match.Success)
                    return String.Concat(match.Groups["Parte1"], match.Groups["Parte2"]).ToInt32Null();
                else
                    return null;
            }
        }

        /// <summary>
        /// Preenche controle com número completo do celular
        /// </summary>
        public void Celular(Int32? ddd, Int32? numero)
        {
            this.Controle.Text = AplicarMascara(ddd, numero);
        }

        /// <summary>
        /// Recupera número do celular
        /// </summary>
        /// <returns>Número do celular</returns>
        public String Celular()
        {
            return this.Controle.Text;
        }

        /// <summary>
        /// Aplica máscara de celular (99) 99999-9999
        /// </summary>
        public static String AplicarMascara(Int32? ddd, Int32? numero)
        {
            if (ddd.HasValue && numero.HasValue)
            {
                String numeroCelular = numero.Value.ToString();

                //Normaliza para quantidade mínima de 8 números, com zeros à esquerda
                if (numeroCelular.Length < 8)
                    numeroCelular = numeroCelular.PadLeft(8, '0');

                //Obtém a primeira parte do número (pré-hífen)
                String parte1 = numeroCelular.Substring(0, numeroCelular.Length > 8 ? 5 : 4);

                //Obtém a segunda parte do número (pós-hífen)
                String parte2 = numeroCelular.Substring(numeroCelular.Length - 4, 4);

                return String.Format("({0}) {1}-{2}", ddd.Value, parte1, parte2);
            }
            else
                return String.Empty;
        }
    }
}