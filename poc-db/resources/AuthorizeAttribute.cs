/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Atributo customizado para controle de permissão de método dos handlers
    /// </summary>
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// Lista dos códigos de serviços que o usuário precisa possuir para o handler aceitar a requisição.
        /// Por padrão, utiliza o operador lógico OU: o usuário precisa possuir pelo menos um dos códigos.
        /// </summary>
        public List<Int32> CodigoServicos { get; private set; }

        /// <summary>
        /// Lista das URLs das páginas que o usuário precisa possuir para o handler aceitar a requisição.
        /// Por padrão, utiliza o operador lógico OU: o usuário precisa possuir pelo menos um dos códigos.
        /// </summary>
        public List<String> UrlPaginas { get; private set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        private AuthorizeAttribute()
        {
        }

        /// <summary>
        /// Construtor para validação por URL de página
        /// </summary>
        /// <param name="urlPagina">URL da página</param>
        /// <param name="urlPaginas">URLs das páginas</param>
        public AuthorizeAttribute(String urlPagina, params String[] urlPaginas)
        {
            this.CodigoServicos = null;
            this.UrlPaginas = new List<String>();
            
            this.UrlPaginas.Add(urlPagina);

            if (urlPaginas != null && urlPaginas.Length > 0)
                this.UrlPaginas.AddRange(urlPaginas);
        }

        /// <summary>
        /// Construtor para validação por código de serviço.
        /// </summary>
        /// <param name="codigoServico">Código do serviço</param>
        /// <param name="codigoServicos">Códigos dos serviços</param>
        public AuthorizeAttribute(Int32 codigoServico, params Int32[] codigoServicos)
        {
            this.UrlPaginas = null;
            this.CodigoServicos = new List<Int32>();

            this.CodigoServicos.Add(codigoServico);

            if (codigoServicos != null && codigoServicos.Length > 0)
                this.CodigoServicos.AddRange(codigoServicos);
        }
    }
}