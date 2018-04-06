/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico
{
    /// <summary>
    /// Endereço da OS
    /// </summary>
    public class EnderecoOs
    {
        /// <summary>
        /// CEP do endereço de atendimento da OS
        /// </summary>
        public String Cep { get; set; }

        /// <summary>
        /// Logradouro do endereço de atendimento da OS
        /// </summary>
        public String Logradouro { get; set; }

        /// <summary>
        /// Número do endereço de atendimento da OS
        /// </summary>
        public String Numero { get; set; }

        /// <summary>
        /// Complemento do endereço de atendimento da OS
        /// </summary>
        public String Complemento { get; set; }

        /// <summary>
        /// Bairro do endereço de atendimento da OS
        /// </summary>
        public String Bairro { get; set; }

        /// <summary>
        /// Cidade do endereço de atendimento da OS
        /// </summary>
        public String Cidade { get; set; }

        /// <summary>
        /// Estado do endereço de atendimento da OS
        /// </summary>
        public String Estado { get; set; }

        /// <summary>
        /// Ponto de referência de atendimento da OS
        /// </summary>
        public String PontoReferencia { get; set; }
    }
}
