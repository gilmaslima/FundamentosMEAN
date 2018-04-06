using System;
using System.Collections.Generic;


namespace Redecard.PN.DadosCadastrais.Modelo
{
    public class Endereco
    {
        private String _cep;
        private String _bairro;
        private String _cidade;
        private String _contato;
        private String _complemento;
        private String _endereco;
        private String _numero;
        private String _telefone;
        private String _uf;
        
        /// <summary>
        /// Bairro do endereço 
        /// </summary>
        public String Bairro 
        {
            get
            {
                return this._bairro.ToUpper();
            }
            set
            {
                _bairro = value;
            }
        }

        /// <summary>
        /// CEP do endereço
        /// </summary>
        public String CEP
        {
            get
            {
                return _cep.ToUpper();
            }
            set
            {
                _cep = value;
            }
        }

        /// <summary>
        /// Cidade do endereço
        /// </summary>
        public String Cidade
        {
            get
            {
                return _cidade.ToUpper();
            }
            set
            {
                _cidade = value;
            }
        }

        /// <summary>
        /// Contato do Estabelecimento
        /// </summary>
        public String Contato
        {
            get
            {
                return _contato.ToUpper();
            }
            set
            {
                _contato = value;
            }
        }

        /// <summary>
        /// Complemento do endereço
        /// </summary>
        public String Complemento 
        {
            get
            {
                return _complemento.ToUpper();
            }
            set
            {
                _complemento = value;
            }
        }

        /// <summary>
        /// Endereço do estabelecimento
        /// </summary>
        public String EnderecoEstabelecimento
        {
            get
            {
                return _endereco.ToUpper();
            }
            set
            {
                _endereco = value;
            }
        }

        /// <summary>
        /// Número do endereço
        /// </summary>
        public String Numero 
        {
            get
            {
                return _numero.ToUpper();
            }
            set
            {
                _numero = value;
            }
        }

        /// <summary>
        /// Telefone do endereço
        /// </summary>
        public String Telefone
        {
            get
            {
                return _telefone.ToUpper();
            }
            set
            {
                _telefone = value;
            }
        }

        /// <summary>
        /// UF do endereço
        /// </summary>
        public String UF 
        {
            get
            {
                return _uf.ToUpper();
            }
            set
            {
                _uf = value;
            }
        }

        /// <summary>
        /// Tipo do estabelecimento. E = Estabelecimento; C = Correspondência
        /// </summary>
        public String TipoEstabelecimento { get; set; }
    }
}
