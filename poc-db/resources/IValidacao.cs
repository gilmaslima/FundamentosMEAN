using System;

namespace Redecard.Portal.Helper.Validacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Contrato genérico de implementação de objetos validadores para um determinado tipo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidacao<T>
    {
        /// <summary>
        /// Realização da validação de um objeto T com retorno de um objeto SumarioValidacao
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        SumarioValidacao Validar(T item);
    }
}
