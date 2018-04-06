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
    /// Este componente publica a classe RespostaAnaliseItem, que expõe propriedades para manipular dados de resposta de analise de item.
    /// </summary>
    [DataContract]
    public class RespostaAnaliseItem
    {
        [DataMember]
        public string NumeroEmissor { get; set; }
        [DataMember]
        public string Comentario { get; set; }
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public bool EhFraude { get; set; }
        [DataMember]
        public TipoResposta TipoResposta { get; set; }
        [DataMember]
        public long IdentificadorTransacao { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public TipoAlarme TipoAlarme { get; set; }
    }
}