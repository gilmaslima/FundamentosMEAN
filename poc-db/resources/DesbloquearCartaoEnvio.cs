/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe DesbloquearCartaoEnvio, que expõe propriedades para manipular dados de envio de desbloqueio de cartão.
    /// </summary>
    [DataContract]
    public class DesbloquearCartaoEnvio
    {
        [DataMember]
        public int NumeroEmissor {get; set;}
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public long IdentificadorTransacao { get; set; }

    }
}