/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Classe responsável por guardar as informações da resposta da requisição do Handler genérico.
    /// </summary>
    public class HandlerResponse
    {        
        /// <summary>
        /// Código do resultado da execução (0: ok).
        /// </summary>
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Mensagem do resultado da execução
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// Dados de retorno da execução
        /// </summary>
        public Object Dados { get; set; }

        /// <summary>
        /// Mensagem de detalhes, em caso de erro
        /// </summary>
        public String DetalhesErro { get; set; }

        #region [ Construtores ]

        /// <summary>
        /// Construtor simples.
        /// </summary>
        public HandlerResponse() { }

        /// <summary>
        /// Construtor completo.
        /// </summary>
        /// <param name="codigo">Código de retorno</param>
        /// <param name="mensagem">Mensagem de retorno</param>
        /// <param name="dados">Dados de retorno</param>
        /// <param name="detalhesErro">Detalhes do erro</param>
        public HandlerResponse(Int32 codigo, String mensagem, Object dados, String detalhesErro)
        {
            this.Codigo = codigo;
            this.Mensagem = mensagem;
            this.Dados = dados;
            this.DetalhesErro = detalhesErro;
        }

        /// <summary>
        /// Construtor padrão em caso de sucesso.
        /// </summary>
        /// <param name="dados">Dados de retorno</param>
        public HandlerResponse(Object dados)
        {
            this.Codigo = 0;
            this.Mensagem = "sucesso";
            this.Dados = dados;
        }

        /// <summary>
        /// Construtor simples, para tipo void.
        /// </summary>
        /// <param name="codigo">Código de retorno</param>
        /// <param name="mensagem">Mensagem de retorno</param>
        public HandlerResponse(Int32 codigo, String mensagem)
            : this(codigo, mensagem, null, null) { }

        #endregion
    }
}