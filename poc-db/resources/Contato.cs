using System;

namespace Redecard.Portal.Helper.DTO
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data: 20/09/2010
    /// Descrição: DTO para campos de Contato
    /// </summary>
    public struct Contato
    {
        /// <summary>
        /// Flag que identifica se o contato é ou não cliente
        /// </summary>
        public string ECliente {
            get;
            set;
        }

        /// <summary>
        /// Campo nome
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Campo email
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Campo DDD
        /// </summary>
        public string DDD
        {
            get;
            set;
        }

        /// <summary>
        /// Campo telefone
        /// </summary>
        public string Telefone
        {
            get;
            set;
        }

        /// <summary>
        /// Campo número estabelecimento
        /// </summary>
        public string NumeroEstabelecimento
        {
            get;
            set;
        }

        /// <summary>
        /// Campo Mensagem
        /// </summary>
        public string Mensagem
        {
            get;
            set;
        }

        /// <summary>
        /// Campo Motivo do Contato
        /// </summary>
        public MotivoContato Motivo
        {
            get;
            set;
        }

        ///// <summary>
        ///// Campo CPF
        ///// </summary>
        //public string CPF
        //{
        //    get;
        //    set;
        //}
        ///// <summary>
        ///// Campo CNPJ
        ///// </summary>
        //public string CNPJ
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Campo CNPJCPF
        /// </summary>
        public string CNPJCPF
        {
            get;
            set;
        }

        /// <summary>
        /// Campo Razão Social
        /// </summary>
        public string RazaoSocial
        {
            get;
            set;
        }



        //Campo Tipo de Pessoa
        public string TipoPessoa
        {
            get;
            set;
        }

        /// <summary>
        /// Campo Número do Protocolo de Atendimento
        /// </summary>
        public string NumeroProtocoloAtendimento
        {
            get;
            set;
        }
    }
}