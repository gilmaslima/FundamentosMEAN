/*
© Copyright 2014 Rede S.A.
Autor : Jacques Domingos Freire de Sá
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - CPF
    /// </summary>
    public class CampoCpfCnpj : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoCpfCnpj(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle()
        {
            this.Controle.TextBox.Attributes["alt"] = "CPF/CNPJ";
            this.Controle.TextBox.CssClass += " campo-cpf-cnpj";
        }

        /// <summary>
        /// Valida conteúdo do controle, se CPF/CNPJ é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = false;

            // se o campo é obrigatório e não estiver preenchido
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }

            if (this.IsCpf)
            {
                // verifica se o CPF está correto
                valido = ValidarCpf(this.Controle.Text);
            }
            else
            {
                // verifica se o CNPJ está correto
                valido = ValidarCnpj(this.Controle.Text);
            }

            // se estiver preenchido e o conteúdo estiver inválido
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErro("CPF/CNPJ inválido");
                return false;
            }

            if (exibirMensagem)
                this.Controle.ExibirMensagemSucesso("CPF/CNPJ válido");
            return true;
        }

        /// <summary>
        /// Validação do CPF com máscara 000.000.000-00
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarCpf(String cpf)
        {
            return CampoCpf.ValidarCpf(cpf);
        }

        /// <summary>
        /// Validação do CNPJ com máscara 00.000.000/0000-00
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarCnpj(String cnpj)
        {
            return CampoCnpj.ValidarCnpj(cnpj);
        }

        /// <summary>
        /// Cpf
        /// </summary>
        public Int64? ValorCpfCnpj
        {
            get
            {
                return new String(this.Controle.Text.Where(c => Char.IsNumber(c)).ToArray()).ToInt64Null();
            }
            set
            {
                this.Controle.Text = AplicarMascara(value);
            }
        }

        /// <summary>
        /// Retorna se o valor informado é de CPF
        /// </summary>
        public bool IsCpf
        {
            get
            {
                return (ValorCpfCnpj ?? 0).ToString().Length <= 11;
            }
        }

        /// <summary>
        /// Retorna se o valor informado é de CNPJ
        /// </summary>
        public bool IsCnpj
        {
            get
            {
                return (ValorCpfCnpj ?? 0).ToString().Length > 11;
            }
        }

        /// <summary>
        /// Aplica máscara CPF (999.999.999-99)
        /// </summary>
        public static String AplicarMascara(Int64? cpf)
        {
            if (cpf.HasValue)
            {
                String valor = cpf.Value.ToString();

                if (valor.Length > 11)
                {
                    valor = cpf.Value.ToString().PadLeft(14, '0');
                    valor = valor.Insert(2, ".");
                    valor = valor.Insert(6, ".");
                    valor = valor.Insert(10, "/");
                    valor = valor.Insert(15, "-");
                }
                else
                {
                    valor = cpf.Value.ToString().PadLeft(11, '0');
                    valor = valor.Insert(3, ".");
                    valor = valor.Insert(7, ".");
                    valor = valor.Insert(11, "-");
                }

                return valor;
            }
            else
                return String.Empty;
        }
    }
}