/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
namespace Redecard.PN.FMS.Modelo
{
    /// <summary>
    /// Enumeração utilizado para tipos de situação de fraude.
    /// </summary>
    public enum SituacaoFraude
    {
        NaoAplicavel = 0,
        Fraude = 1,
        NaoFraude = 2,
        EmAnalise = 3
    }

}