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
    /// Configuração de um Card
    /// </summary>
    [DataContract]
    public class CardConfig
    {
        /// <summary>
        /// Nome do card
        /// </summary>
        [DataMember(Name = "nome")]
        public String Nome { get; set; }

        /// <summary>
        /// Termos que levam a busca a exibir o card
        /// </summary>
        [DataMember(Name = "termos")]
        public String[] Termos { get; set; }

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

        /// <summary>
        /// Subcards (caso existam). Um card pode possuir 0 ou N subcards, cada um com permissões distintas.
        /// Exemplo: card de extrato possui subcards/abas de vendas, valores pagos e lançamentos futuros
        /// </summary>
        [DataMember(Name = "subcards")]
        public SubcardConfig[] Subcards { get; set; }

        /// <summary>
        /// Prioridade do card.
        /// </summary>
        [DataMember(Name = "prioridade")]
        public Int32 Prioridade { get; set; }
    }
}