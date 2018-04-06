using System;

namespace Redecard.Portal.Helper.Paginacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe com parâmetros e valores padrões para aspectos relacionados a paginação no site
    /// </summary>
    public static class ParametrosGeraisPaginacao
    {
        public static int QuantidadeLimiteItensPaginadores = 5;
        public static int QuantidadeItensPorPagina = 5;
        public static int QuantidadeItensPorPaginaModoVerTodas = 20;
        public static bool PaginacaoHabilitada = true;
    }
}