/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.SolicitacaoMaterial
{
    /// <summary>
    /// Classe modelo para endereço da Slicitação de Material(ais).
    /// </summary>
    public class EnderecoSolicitacao
    {
        // <summary>
        /// Bairro 
        /// </summary>
        public String Bairro { get; set; }

        // <summary>
        /// CEP
        /// </summary>
        public String Cep { get; set; }

        // <summary>
        /// Cidade
        /// </summary>
        public String Cidade { get; set; }

        // <summary>
        /// Complemento
        /// </summary>
        public String Complemento { get; set; }

        // <summary>
        /// Nome do Contato 
        /// </summary>
        public String Contato { get; set; }

        // <summary>
        /// Endereço
        /// </summary>
        public String Endereco { get; set; }

        // <summary>
        /// E-mail
        /// </summary>
        public String Email { get; set; }

        // <summary>
        /// Número
        /// </summary>
        public Int32 Numero { get; set; }

        // <summary>
        /// Telefone
        /// </summary>
        public String Telefone { get; set; }

        // <summary>
        /// UF
        /// </summary>
        public String Uf { get; set; }



    }
}
