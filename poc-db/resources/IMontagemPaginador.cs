using System;

namespace Redecard.Portal.Helper.Paginacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Contrato de montagem de estrutura de paginação HTML
    /// </summary>
    public interface IMontagemPaginador
    {
        string MontarPaginadorHTML(int totalItens, int itensPorPagina, int limiteNumeroPaginadores, int? paginaAtual, string ancora);
    }
}