/*
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
    /// Este componente publica a classe PesquisarCriteriosSelecaoPorUsuarioLoginRetorno, que expõe propriedades para manipular dados de retorno da pesquisa de critérios de seleção por usuário logado.
    /// </summary>
    [DataContract]
    public class PesquisarCriteriosSelecaoPorUsuarioLoginRetorno
    {
        [DataMember]
        public List<TipoAlarme> TipoAlarme { get; set; }
        [DataMember]
        public List<CriterioResultadoAutorizacao> ResultadoAutorizacao { get; set; }
        [DataMember]
        public CriterioClassificacao CriterioClassificacao { get; set; }
        [DataMember]
        public long InicioFaixaScore { get; set; }
        [DataMember]
        public long FimFaixaScore { get; set; }
        [DataMember]
        public List<EntryMode> EntryModes { get; set; }
        [DataMember]
        public List<EntryMode> EntryModesSelecionados { get; set; }
        [DataMember]
        public List<MCC> MCCsSelecionados { get; set; }
        [DataMember]
        public List<Int64> EstabelecimentosSelecionados { get; set; }
        [DataMember]
        public List<FaixaBin> RangeBinsSelecionados { get; set; }
        [DataMember]
        public List<String> UF { get; set; }
        [DataMember]
        public List<String> UFsSelecionadas { get; set; }
        [DataMember]
        public List<TipoTransacao> TipoTransacaoSelecionadas { get; set; }
        [DataMember]
        public decimal ValorTransacaoInicial { get; set; }
        [DataMember]
        public decimal ValorTransacaoFinal { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public List<SituacaoCartao> SituacoesCartaoSelecionados { get; set; }

    }
}