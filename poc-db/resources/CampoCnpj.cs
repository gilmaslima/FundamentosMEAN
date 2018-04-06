/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using Redecard.PN.Comum;
using System.Text.RegularExpressions;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - CNPJ
    /// </summary>
    public class CampoCnpj : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoCnpj(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextBox.Attributes["alt"] = "cnpj";
        }
        
        /// <summary>
        /// Valida conteúdo do controle, se CNPJ é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = ValidarCnpj(this.Cnpj);

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
                    this.Controle.ExibirMensagemErro("CNPJ inválido");
                return false;
            }

            if (exibirMensagem)
                this.Controle.ExibirMensagemSucesso("CNPJ válido");
            return true;
        }

        /// <summary>
        /// Validação do CNPJ com máscara 00.000.000/0000-00
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarCnpj(String cnpj)
        {
			Int32[] multiplicador1 = new Int32[12] {5,4,3,2,9,8,7,6,5,4,3,2};
			Int32[] multiplicador2 = new Int32[13] {6,5,4,3,2,9,8,7,6,5,4,3,2};
			Int32 soma = default(Int32);
			Int32 resto = default(Int32);
			String digito = default(String);
			String tempCnpj = default(String);
            
            //CNPJ com máscara deve ter exatamente 18 caracteres
            if(cnpj.Length != 18)
                return false;

            //Verifica separadores
            if(cnpj[2] != '.' || cnpj[6] != '.' || cnpj[10] != '/' || cnpj[15] != '-')
                return false;

            //Remove caracteres especiais
            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
			if (cnpj.Length != 14)
			   return false;

            //Verifica se sobraram apenas números
            for(Int32 i=0; i<cnpj.Length; i++)
                if(!Char.IsNumber(cnpj[i]))
                    return false;

            //Valida dígito verificador
			tempCnpj = cnpj.Substring(0, 12);
			for(Int32 i=0; i<12; i++)
			   soma += tempCnpj[i].ToString().ToInt32() * multiplicador1[i];
			resto = (soma % 11);
			if ( resto < 2)
			   resto = 0;
			else
			   resto = 11 - resto;
			digito = resto.ToString();
			tempCnpj = tempCnpj + digito;
			
            soma = 0;

			for (int i = 0; i < 13; i++)
			   soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
			resto = (soma % 11);
			if (resto < 2)
			    resto = 0;
			else
			   resto = 11 - resto;
			digito = digito + resto.ToString();

			return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public String Cnpj
        {
            get { return this.Controle.Text; }
            set { this.Controle.Text = value; }
        }

        /// <summary>
        /// CNPJ convertido no formato Nullable<long>
        /// </summary>
        public long? LongCnpj
        {
            get
            {
                if (string.IsNullOrEmpty(Cnpj))
                    return null;

                string cnpjTratado = Regex.Replace(Cnpj, "[^0-9]", string.Empty);

                long longCnpj = 0;
                if (!long.TryParse(cnpjTratado, out longCnpj))
                    return null;

                return longCnpj;
            }
        }
    }
}
