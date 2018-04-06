﻿/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe PesquisaTipoRespostaEnvio, que expõe propriedades para manipular dados envio de pesquisa de tipo de resposta. 
    /// </summary>
    [DataContract]
    public class PesquisaTipoRespostaEnvio
    {
        [DataMember]
        public string UsuarioLogin{get;set;}

        [DataMember]
        public int NumeroEmissor{get;set;}

        [DataMember]
        public int GrupoEntidade { get; set;}
    }
}