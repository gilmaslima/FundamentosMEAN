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

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe PesquisarTransacoesPorTransacaoAssociadaEnvio, que expõe propriedades para manipular dados de envio da pesquisa de transações por transação associada.
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesPorTransacaoAssociadaEnvio
    {
        [DataMember]
        public long IdentificadorTransacao { get; set; }
        [DataMember]
        public int NumeroEmissor { get; set; }
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public int TempoBloqueio { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
        [DataMember]
        public CriterioOrdemTransacoesPorNumeroCartaoOuAssociada Criterio { get; set; }
        [DataMember]
        public OrdemClassificacao Ordem { get; set; }
    }
}