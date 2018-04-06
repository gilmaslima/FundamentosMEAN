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
using System.Text;

namespace Redecard.PN.FMS.Modelo
{
    /// <summary>
    /// Enumeração utilizado para critérios de seleção.
    /// </summary>
    public class CriteriosSelecao
    {
        public List<TipoAlarme> TipoAlarme { get; set; }
        public List<CriterioResultadoAutorizacao> ResultadoAutorizacao { get; set; }
        public CriterioClassificacao CriterioClassificacao { get; set; }
        public CriterioClassificacaoEstabelecimento CriterioClassificacaoEstabelecimento { get; set; }
        public long InicioFaixaScore { get; set; }
        public long FimFaixaScore { get; set; }
        public List<EntryMode> EntryModes { get; set; }
        public List<EntryMode> EntryModesSelecionados { get; set; }
        public List<MCC> MCCsSelecionados { get; set; }
        public List<Int64> EstabelecimentosSelecionados { get; set; }
        public List<FaixaBin> RangeBinsSelecionados { get; set; }
        public List<String> UF { get; set; }
        public List<String> UFsSelecionadas { get; set; }
        public List<TipoTransacao> TipoTransacaoSelecionadas { get; set; }
        public decimal ValorTransacaoInicial { get; set; }
        public decimal ValorTransacaoFinal { get; set; }
        public string Usuario { get; set; }
        public List<SituacaoCartao> SituacoesCartaoSelecionados { get; set; }

    }
}
