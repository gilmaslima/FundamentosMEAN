using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Modelo
{
    /// <summary>
    /// Classe Pessoa Física
    /// </summary>
    public class PF
    {
        /// <summary>
        /// Código de Retorno
        /// </summary>
        public String CodRetorno { get; set; }

        /// <summary>
        /// Número do CNPJ
        /// </summary>
        public String CNPJ { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        public String TipoPessoa { get; set; }

        /// <summary>
        /// Status Documento
        /// </summary>
        public String StatDocto { get; set; }

        /// <summary>
        /// Razão Social
        /// </summary>
        public String ComplGrafia { get; set; }

        /// <summary>
        /// Tipo de Logradouro
        /// </summary>
        public String TipoLogradouro { get; set; }

        /// <summary>
        /// Descrição do Logradouro
        /// </summary>
        public String DescLogradouro { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public String Numero { get; set; }

        /// <summary>
        /// Complemento
        /// </summary>
        public String Complemento { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        public String Bairro { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        public String Cidade { get; set; }

        /// <summary>
        /// UF
        /// </summary>
        public String UF { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        public String CEP { get; set; }

        /// <summary>
        /// DDD
        /// </summary>
        public String DDD { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public String Telefone { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        public String DataNascimento { get; set; }
    }
}
