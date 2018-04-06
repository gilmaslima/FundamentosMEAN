/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – Rodrigo Locoseli – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Modelo.CadastroIPs
{
    /// <summary>
    /// Retorno da Manutencao de IP
    /// </summary>
    public class ManutencaoRetorno
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
    }
}
