/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    /// <summary>
    /// Armazena se a transação foi selecionada e/ou alterada.
    /// </summary>
    [Serializable]
    public class TransacaoSuspeita : Servico.FMS.TransacaoEmissor
    {
        public bool TransacaoSelecionada { get; set; }
        public bool TransacaoAlterada { get; set; }
    }
}
