/*
© Copyright 2017 Rede S.A.
Autor : Mário neto
Empresa : Iteris Consultoria e Software
*/

using Rede.PN.AtendimentoDigital.Modelo.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.EnderecoCepResponse
{
    /// <summary>
    /// Classe FavoriteResponse
    /// </summary>
    [DataContract]
    public sealed class EnderecoCepResponse
    {
        /// <summary>
        /// Endereco.
        /// </summary>
        [DataMember(Name = "endereco", EmitDefaultValue = false)]
        public String Endereco { get; set; }

        /// <summary>
        /// Bairro.
        /// </summary>
        [DataMember(Name = "bairro", EmitDefaultValue = false)]
        public String Bairro { get; set; }

        /// <summary>
        /// Cidade.
        /// </summary>
        [DataMember(Name = "cidade", EmitDefaultValue = false)]
        public String Cidade { get; set; }

        /// <summary>
        /// Estado (UF).
        /// </summary>
        [DataMember(Name = "uf", EmitDefaultValue = false)]
        public String Uf { get; set; }

        /// <summary>
        /// cep.
        /// </summary>
        [DataMember(Name = "cep", EmitDefaultValue = false)]
        public String Cep { get; set; }

        /// <summary>
        /// CEP único?.
        /// </summary>
        [DataMember(Name = "cepUnico", EmitDefaultValue = true)]
        public Boolean CepUnico { get; set; }
    }
}