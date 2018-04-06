/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Text;
using Rede.PN.AtendimentoDigital.Modelo.Structure;
using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;

namespace Rede.PN.AtendimentoDigital.Modelo.Entidades
{
    /// <summary>
    /// Entidade de acesso aos dados de Endereco
    /// </summary>
    public class EntidadeEnderecoCep : EntidadeBase, IEntidade<EnderecoCepChave>
    {
        /// <summary>
        /// Chave
        /// </summary>
        public EnderecoCepChave Chave { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public String Endereco { get; set; }
        
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
        public String Uf { get; set; }

        /// <summary>
        /// Cep
        /// </summary>
        public String Cep { get; set; }

        /// <summary>
        /// CEP único?
        /// </summary>
        public Boolean CepUnico { get; set; }
    }
}