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
    /// Este componente publica a classe ProdutividadePorData, que expõe propriedades para manipular dados de produtividade por data.
    /// </summary>
    public class ProdutividadePorData
    {
        public string UsuarioLogin { get; set; }
        public long QuantidadeCartoesAnalisados { get; set; }
        public decimal ValorFraude { get; set; }
        public long QuantidadeCartoesFraudulentos { get; set; }
        public long QuantidadeTransacoesFraudulentas { get; set; }
        public decimal ValorNaoFraude { get; set; }
        public long QuantidadeCartoesNaoFraudulentos { get; set; }
        public long QuantidadeTransacoesNaoFraudulentas { get; set; }
    }
}
