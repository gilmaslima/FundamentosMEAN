using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.Credenciamento.Servicos
{
    /// <summary>
    /// Classe Pessoa Jurídica
    /// </summary>
    [DataContract]
    public class PJ
    {
        /// <summary>
        /// Código de Retorno
        /// </summary>
        [DataMember]
        public String CodRetorno { get; set; }

        /// <summary>
        /// Número do CNPJ
        /// </summary>
        [DataMember]
        public String CNPJ { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        [DataMember]
        public String TipoPessoa { get; set; }

        /// <summary>
        /// Status Documento
        /// </summary>
        [DataMember]
        public String StatDocto { get; set; }

        /// <summary>
        /// Razão Social
        /// </summary>
        [DataMember]
        public String ComplGrafia { get; set; }

        /// <summary>
        /// Tipo de Logradouro
        /// </summary>
        [DataMember]
        public String TipoLogradouro { get; set; }

        /// <summary>
        /// Descrição do Logradouro
        /// </summary>
        [DataMember]
        public String DescLogradouro { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        [DataMember]
        public String Numero { get; set; }

        /// <summary>
        /// Complemento
        /// </summary>
        [DataMember]
        public String Complemento { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        [DataMember]
        public String Bairro { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [DataMember]
        public String Cidade { get; set; }

        /// <summary>
        /// UF
        /// </summary>
        [DataMember]
        public String UF { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        [DataMember]
        public String CEP { get; set; }

        /// <summary>
        /// DDD
        /// </summary>
        [DataMember]
        public String DDD { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        [DataMember]
        public String Telefone { get; set; }

        /// <summary>
        /// Nome Fantasia
        /// </summary>
        [DataMember]
        public String NomeFantasia { get; set; }

        /// <summary>
        /// Data de Fundação
        /// </summary>
        [DataMember]
        public String DataFundacao { get; set; }

        /// <summary>
        /// Lista de CNAEs
        /// </summary>
        [DataMember]
        public List<CodigoCNAE> CNAEs { get; set; }

        /// <summary>
        /// Lista de Sócios
        /// </summary>
        [DataMember]
        public List<Socio> Socios { get; set; }
    }

    /// <summary>
    /// Classe CNAE
    /// </summary>
    [DataContract]
    public class CodigoCNAE
    {
        /// <summary>
        /// Código do CNAE
        /// </summary>
        [DataMember]
        public String CodCNAE { get; set; }
    }

    /// <summary>
    /// Classe de Sócios
    /// </summary>
    [DataContract]
    public class Socio
    {
        /// <summary>
        /// Nome do Sócio
        /// </summary>
        [DataMember]
        public String Nome { get; set; }

        /// <summary>
        /// CPF/CNPJ do Sócio
        /// </summary>
        [DataMember]
        public String CPF_CNPJ { get; set; }

        /// <summary>
        /// Particípação do Sócio
        /// </summary>
        [DataMember]
        public String Participacao { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        [DataMember]
        public String TipoPessoa { get; set; }
    }
}
