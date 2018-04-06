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
    /// Este componente publica a classe RespostaListaFaixaBin, que expõe métodos para manipular as resposta de lista de faixa bin.
    /// </summary>
    public class RespostaListaFaixaBin
    {
        public List<FaixaBin> ListaFaixaBin { get; set; }
        public long QuantidadeRegistros { get; set; }
        /// <summary>
        /// Este método é utilizado para adquirir a quantidade de respostas da lista de faixa bin.
        /// </summary>
        public RespostaListaFaixaBin()
        {
            QuantidadeRegistros = -1;
        }
    }
}
