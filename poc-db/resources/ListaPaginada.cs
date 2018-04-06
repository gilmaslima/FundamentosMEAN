using System;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.Portal.Helper.Paginacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe que representa uma lista de objetos T é utilizada para retornar um subconjunto de uma fonte enumerável
    /// ListaPaginada&lt;Carro&gt;, ListaPaginada&lt;Produto&gt;,ListaPaginada&lt;Cliente&gt; etc...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ListaPaginada<T> : List<T>
    {
        /// <summary>
        /// Retorna o índice da página.
        /// Somente leitura.
        /// </summary>
        public int IndicePagina { get; private set; }

        /// <summary>
        /// Retorna o quantidade de itens por página.
        /// Somente leitura.
        /// </summary>
        public int ItensPorPagina { get; private set; }

        /// <summary>
        /// Retorna o total de páginas com base no cálculo de paginação.
        /// Somente leitura.
        /// </summary>
        public int TotalPaginas { get; private set; }

        /// <summary>
        /// Retorna o total de itens com base na fonte de dados.
        /// Somente leitura.
        /// </summary>
        public int TotalItens { get; private set; }

        /// <summary>
        /// Construtor padrão para a instância
        /// </summary>
        /// <param name="fonteDados">Qualquer objeto iterável - que implemente IEnumerable&lt;T&gt;</param>
        /// <param name="indicePagina">Índice da página (opcional) - Caso não seja informado, assume o valor 1</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        public ListaPaginada(IEnumerable<T> fonteDados, int? indicePagina, int itensPorPagina)
        {
            this.IndicePagina = !indicePagina.HasValue || indicePagina.Value <= 1 ? 1 : indicePagina.Value;
            this.ItensPorPagina = itensPorPagina;
            this.TotalItens = fonteDados.Count();
            this.TotalPaginas = (int)Math.Ceiling(this.TotalItens / (double)itensPorPagina);

            //Adiciona na lista os itens que pertencerão à pagina atual
            //Explicação com simulação:
            //Considerando os parâmetros com valor, respectivamente:
            //--> fonteDados = {3,4,6,5,8,7,4,3,2,5,6,7} (lista de inteiros)
            //--> indicePagina = 3
            //--> itensPorPagina = 4
            //A instrução abaixo pula/ignora (3 - 1) * 4 (ou seja, pula os oito primeiros) registros e pega os 4 registros seguintes
            //Portanto, itens para a página 3: {2,5,6,7}
            this.AddRange(fonteDados.Skip((this.IndicePagina - 1) * itensPorPagina).Take(itensPorPagina));
        }
    }
}