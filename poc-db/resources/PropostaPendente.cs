using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    /// <summary>
    /// Classe de Propostas Pendentes
    /// </summary>
    [Serializable]
    public class PropostaPendente
    {
        /// <summary>
        /// Tipo Pessoa
        /// </summary>
        public Char? TipoPessoa { get; set; }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int32? NroEstabelecimento { get; set; }

        /// <summary>
        /// CNPJ
        /// </summary>
        public Int64? CNPJ { get; set; }

        /// <summary>
        /// RAzão Social
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Ramo
        /// </summary>
        public String Ramo { get; set; }

        /// <summary>
        /// Tipo de Estabelecimento
        /// </summary>
        public Int32? TipoEstabelecimento { get; set; }

        /// <summary>
        /// Categoria
        /// </summary>
        public Char? Categoria { get; set; }

        /// <summary>
        /// Endereço Comercial
        /// </summary>
        public String EnderecoComercial { get; set; }

        /// <summary>
        /// Status da Proposta
        /// </summary>
        public Int32? StatusProposta { get; set; }

        /// <summary>
        /// Número de Sequência
        /// </summary>
        public Int32? NumSequencia { get; set; }
    }
}
