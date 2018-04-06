/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo Endereco
    /// </summary>
    [DataContract]
    public class EnderecoRest
    {
        /// <summary>
        /// Logradouro
        /// </summary>
        [DataMember]
        public String Logradouro { get; set; }

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

        #region Estado
        /// <summary>
        /// Estado
        /// </summary>
        [IgnoreDataMember]
        public TipoUf EstadoCodigo { get; set; }

        /// <summary>
        /// Descrição do Estado
        /// </summary>
        [DataMember(Name = "Estado", EmitDefaultValue = false)]
        public String EstadoDescricao
        {
            get
            {
                return EstadoCodigo != null ? EstadoCodigo.ToString() : "";
            }
            set { this.EstadoDescricao = value; }
        }
        #endregion
        /// <summary>
        /// CEP
        /// </summary>
        [DataMember]
        public String Cep { get; set; }

        /// <summary>
        /// Ponto Referência
        /// </summary>
        [DataMember]
        public String PontoReferencia { get; set; }
    }
}
