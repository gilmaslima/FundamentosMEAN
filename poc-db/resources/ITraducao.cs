using System;

namespace Redecard.Portal.Helper.Conversores
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Contrato para implementação de conversão de um tipo T1 para um tipo T2
    /// De->Para
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface ITraducao<T1, T2>
    {
        T1 Traduzir(T2 item);
        T2 Traduzir(T1 item);
    }
}