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
    /// Este componente publica a classe RespostaListaMCC, que expõe métodos para manipular as resposta de lista de de registros merchant category code.
    /// </summary>
    public class RespostaListaMCC
    {
        public List<MCC> ListaMCC { get; set; }
        public long QuantidadeRegistros { get; set; }
        /// <summary>
        /// Este método é utilizado para obter a quantidade de registros da lista de registros merchant category code.
        /// </summary>
        public RespostaListaMCC()
        {
            QuantidadeRegistros = -1;
        }

    }
}
