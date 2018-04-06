/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Fluent interface para comparação de valores entre duas instâncias do mesmo tipo.
    /// </summary>
    /// <typeparam name="T">Tipo do dado sendo comparado</typeparam>
    public class HistoricoComparacao<T> : Historico
    {
        /// <summary>
        /// Instância 1
        /// </summary>
        private T Objeto1 { get; set; }

        /// <summary>
        /// Instância 2
        /// </summary>
        private T Objeto2 { get; set; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="objeto1">Instância 1</param>
        /// <param name="objeto2">Instância 2</param>
        public HistoricoComparacao(T objeto1, T objeto2)
        {
            this.Objeto1 = objeto1;
            this.Objeto2 = objeto2;
        }

        /// <summary>
        /// Definição da descrição do campo que será comparado.
        /// </summary>
        /// <param name="funcaoPropriedade">Função para selecionar a propriedade</param>
        /// <param name="nomeCampo">Nome do campo</param>
        /// <returns>Fluent interface de comparação</returns>
        public HistoricoComparacao<T> Campo(Func<T, Object> funcaoPropriedade, String nomeCampo)
        {
            Object valor1 = funcaoPropriedade(Objeto1);
            Object valor2 = funcaoPropriedade(Objeto2);

            if (!Equals(valor1, valor2))
                Campos.Add(nomeCampo);

            return this;
        }
    }
}
