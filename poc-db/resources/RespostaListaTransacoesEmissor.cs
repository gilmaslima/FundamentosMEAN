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
    /// Este componente publica a classe RespostaListaTransacoesEmissor, que expõe propriedades para manipular dados de resposta da lista de transações por emissor.
    /// </summary>
    public class RespostaListaTransacoesEmissor
    {
        public List<TransacaoEmissor> ListaTransacoesEmissor { get; set; }
        public TipoRespostaListaEmissor TipoRespostaListaEmissor { get; set; }
        public int SegundosRestanteBloqueio { get; set; }
    }
}
