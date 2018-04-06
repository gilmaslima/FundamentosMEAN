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
    /// Este componente publica a classe AgrupamentoProdutividadePorData, que expõe propriedades para manipular dados de agrupamento de produtividade por data.
    /// </summary>
    public class AgrupamentoProdutividadePorData
    {
        public List<ProdutividadePorData> ProdutividadePorData { get; set; }
        public long QuantidadeCartoesAnalisados { get; set; }
        public decimal ValorTotalFraude { get; set; }
        public long QuantidadeTotalCartoesFraudulentos { get; set; }
        public long QuantidadeTotalTransacoesFraudulentas { get; set; }
        public decimal ValorTotalNaoFraude { get; set; }
        public long QuantidadeTotalCartoesNaoFraudulentos { get; set; }
        public long QuantidadeTotalTransacoesNaoFraudulentas { get; set; }
        public System.DateTime Data { get; set; }
    }
}
