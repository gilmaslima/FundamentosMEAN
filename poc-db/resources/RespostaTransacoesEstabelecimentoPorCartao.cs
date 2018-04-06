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
    /// Este componente publica a classe RespostaTransacoesEstabelecimentoPorCartao, que expõe propriedades para manipular dados de resposta das transações por estabelecimento emitidas por cartão.
    /// </summary>
    public class RespostaTransacoesEstabelecimentoPorCartao
    {
        public long NumeroEstabelecimento { get; set; }
        public string NomeEstabelecimento { get; set; }
        public long QuantidadeRegistros { get; set; }
        public int QuantidadeHorasRecuperadas { get; set; }
        public int QuantidadeHorasPeriodo { get; set; }
        public List<AgrupamentoTransacoesEstabelecimentoCartao> ListaTransacoes { get; set; }

    }
}
