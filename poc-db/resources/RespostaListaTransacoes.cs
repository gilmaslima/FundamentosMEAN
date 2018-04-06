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
    /// Este componente publica a classe RespostaListaTransacoes, que expõe propriedades para manipular dados de resposta da lista de transações.
    /// </summary>
    public class RespostaListaTransacoes
    {
        public List<TransacaoEmissor> ListaTransacoes { get; set; }
        public long QuantidadeRegistros { get; set; }

        public RespostaListaTransacoes()
        {
            QuantidadeRegistros = -1;
        }
    }
}
