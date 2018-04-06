using System;

namespace Redecard.Portal.Helper.Paginacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe base para Montador de Paginador
    /// Provê funções comuns web para auxílio na montagem do paginador
    /// Não pode ser instanciada
    /// </summary>
    public abstract class MontadorPaginadorBase : IMontagemPaginador
    {
        /// <summary>
        /// Retorna a URL base informada na instanciação de um MontadorPaginador
        /// </summary>
        public string URLBase
        {
            get;
            protected set;
        }

        /// <summary>
        /// Retorna o nome do parâmetro a ser utilizado como querystring na url paginada
        /// </summary>
        public string NomeParametroPagina
        {
            get;
            protected set;
        }

        /// <summary>
        /// Construtor padrão
        /// É necessário informar a URL para montagem da mesmo com parâmetros de paginação
        /// A variável nomeParametroPagina é nome da chave querystring que se deseja utilizar para montagem da URL
        /// </summary>
        /// <param name="URLBase"></param>
        /// <param name="nomeParametroPagina"></param>
        public MontadorPaginadorBase(string URLBase, string nomeParametroPagina)
        {
            this.NomeParametroPagina = nomeParametroPagina;
            this.URLBase = URLBase;
        }

        /// <summary>
        /// Devolve a URL com o parâmetro de paginação e uma âncora(opcional)
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="ancora"></param>
        /// <returns></returns>
        protected virtual string MontarURLComParametroPagina(int pagina, string ancora)
        {
            return string.Format("{0}{1}{2}={3}{4}",
                                                this.URLBase,
                                                this.URLBase.IndexOf('?') == -1 ? "?" : string.Empty,
                                                this.NomeParametroPagina,
                                                pagina,
                                                !string.IsNullOrEmpty(ancora) ? "#" + ancora : string.Empty);
        }

        /// <summary>
        /// Efetua o cálculo de quantidade de páginas com base no total de itens e itens por página desejados
        /// </summary>
        /// <param name="totalItens"></param>
        /// <param name="itensPorPagina"></param>
        /// <returns></returns>
        protected virtual int CalcularTotalPaginas(int totalItens, int itensPorPagina)
        {
            if (totalItens <= 0)
                return 0;

            if (itensPorPagina <= 0)
                return 1;

            return totalItens % itensPorPagina != 0 ? totalItens / itensPorPagina + 1 : totalItens / itensPorPagina;
        }

        #region IMontagemPaginador Members
        /// <summary>
        /// Função de implementação do paginador HTML
        /// Membros que herdarem desta classe deverão desenvolver este método
        /// </summary>
        /// <param name="totalItens"></param>
        /// <param name="itensPorPagina"></param>
        /// <param name="limiteNumeroPaginadores"></param>
        /// <param name="paginaAtual"></param>
        /// <param name="ancora"></param>
        /// <returns></returns>
        public abstract string MontarPaginadorHTML(int totalItens, int itensPorPagina, int limiteNumeroPaginadores, int? paginaAtual, string ancora);

        #endregion
    }
}