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
    /// Este componente publica a classe RespostaTransacoesEstabelecimento, que expõe propriedades para manipular dados de resposta de transações por estabelecimento.
    /// </summary>
    public class RespostaTransacoesEstabelecimento
    {
        public long QuantidadeTransacoes { get; set; }
        public int QuantidadeHorasRecuperadas { get; set; }
        public int QuantidadeHorasTotalPeriodo { get; set; }
        public List<AgrupamentoTransacaoEstabelecimento> ListaTransacoes { get; set; }
    }
}
