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
    /// Enumeração utilizado para tipos de resposta da lista de emissor.
    /// </summary>
    public enum TipoRespostaListaEmissor
    {
        Ok = 0,
        CartaoEmAnalisePorOutroUsuario = 1,
        NaoExistemTransacoes = 2,
        NaoExistemTransacoesAlarmadas = 3,
        CartaoJaAnalisado = 4
    }
}
