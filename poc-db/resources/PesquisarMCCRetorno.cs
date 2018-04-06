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

namespace Redecard.PN.FMS.Servico.Modelo.Merchant
{
    /// <summary>
    /// Este componente publica a classe PesquisarMCCRetorno, que expõe propriedades para manipular dados de retorno de pesquisa de merchant category code
    /// </summary>
    public class PesquisarMCCRetorno
    {
        public List<Redecard.PN.FMS.Servico.Modelo.MCC> MCCs { get; set; }

    }
}