/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Runtime.Serialization;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Config
{
    /// <summary>
    /// Configuração de um Subcard de um Card
    /// </summary>
    [DataContract]
    public class SubcardConfig
    {
        /// <summary>
        /// Nome do subcard
        /// </summary>
        [DataMember(Name = "nome")]
        public String Nome { get; set; }

        /// <summary>
        /// Lista das URLs das páginas que o usuário precisa possuir para o handler aceitar a requisição.
        /// Por padrão, utiliza o operador lógico OU: o usuário precisa possuir pelo menos um dos códigos.
        /// </summary>
        [DataMember(Name = "permissoesUrlPaginas")]
        public String[] PermissoesUrlPaginas { get; set; }

        /// <summary>
        /// Lista das URLs das páginas que o usuário precisa possuir para o handler aceitar a requisição.
        /// Por padrão, utiliza o operador lógico OU: o usuário precisa possuir pelo menos um dos códigos.
        /// </summary>
        [DataMember(Name = "permissoesCodigoServicos")]
        public Int32[] PermissoesCodigoServicos { get; set; }
    }
}