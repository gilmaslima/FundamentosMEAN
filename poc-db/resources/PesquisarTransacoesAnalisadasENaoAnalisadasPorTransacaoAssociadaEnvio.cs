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
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio, que expõe propriedades para manipular dados de envio de pesquisa de transações analisadas e não analisadas por transação associada.
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio
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
        public int PosicaoPrimeiroRegistro { get; set; }
        [DataMember]
        public int QuantidadeRegistros { get; set; }
        [DataMember]
        public bool RenovarContador { get; set; }
        [DataMember]
        public CriterioOrdemTransacoesPorNumeroCartaoOuAssociada Criterio { get; set; }
        [DataMember]
        public OrdemClassificacao Ordem { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
    }
}