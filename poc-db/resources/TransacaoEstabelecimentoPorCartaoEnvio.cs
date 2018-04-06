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
    /// Este componente publica a classe TransacaoEstabelecimentoEnvio, que expõe propriedades para manipular dados de envio de transação por estabelecimento.
    /// </summary>
    [DataContract]
    public class TransacaoEstabelecimentoEnvio
    {
        [DataMember]
        public long NumeroEstabelecimento {get;set;}
        [DataMember]
        public int NumeroEmissor { get; set; }
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public int PrimeiroRegistro { get; set; }
        [DataMember]
        public int QuantidadeMaximaRegistros { get; set; }
        [DataMember]
        public CriterioOrdemTransacoesAgrupadasPorEstabelecimento ModoClassificacao { get; set; }
        [DataMember]
        public OrdemClassificacao Ordem { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoTransacao { get; set; }

    }
}