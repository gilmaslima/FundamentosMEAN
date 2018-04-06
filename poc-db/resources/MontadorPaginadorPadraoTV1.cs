using System;
using System.Text;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Helper.Paginacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe de implementação do Montador de Paginação padrão com base no layout de paginação das telas HTML enviadas pela TV1
    /// Monta dinamicamente o HTML de paginação seguindo HTML proposto pela TV1
    /// </summary>
    public class MontadorPaginadorPadraoTV1 : MontadorPaginadorBase
    {
        /// <summary>
        /// Construtor padrão
        /// É passado à classe base a URL atual de acesso sem o parâmetro de paginação. Para maiores detalhes de como essa URL é obtida, consulte o método URLUtils.ObterUrlAtual
        /// </summary>
        /// <param name="nomeParametroPagina">Nome da chave query string de paginação</param>
        public MontadorPaginadorPadraoTV1(string nomeParametroPagina) : base(URLUtils.ObterURLAtual(p => p.Trim().ToUpper().Equals(nomeParametroPagina.ToUpper())), nomeParametroPagina) { }

        /// <summary>
        /// Implementa o HTML de paginação
        /// </summary>
        /// <param name="totalItens">Quantidade total de itens. É utilizada para realização do cálculo do total de páginas</param>
        /// <param name="itensPorPagina">Quantidade de itens a mostrar por página. É utilizada para realização do cálculo do total de páginas</param>
        /// <param name="limiteNumeroPaginadores">Quando o número de páginas é maior do que este parâmetro, sinais de reticências são colocadas no início e/ou fim do paginador. Essas reticências previnem estouro de layout</param>
        /// <param name="paginaAtual">Número da página atual (opcionao). Se não informada, assume o valor 1</param>
        /// <param name="ancora">Algum nome de âncora a ser anexada a URL dos paginadores (opcional)</param>
        /// <returns></returns>
        public override string MontarPaginadorHTML(int totalItens, int itensPorPagina, int limiteNumeroPaginadores, int? paginaAtual, string ancora)
        {
            //Descobre o total de páginas
            int totalDePaginas = this.CalcularTotalPaginas(totalItens, itensPorPagina);

            //Se der 1, não haverá paginação
            if (totalDePaginas <= 1)
                return string.Empty;

            //Assume 1 caso paginaAtual não foi informado
            int pagina = paginaAtual.HasValue ? paginaAtual.Value : 1;

            //O que é OffSet? OffSet é o número que, dependendo do contexto, é maior ou menor que a quantidade de itens numeradores dividido por 2 que deverão aparecer no paginador
            //É o valor que vai definir se deve renderizar reticências que indicará um número de página anterior ou posterior ao primeiro ou último, respectivamente, item do paginador quando o total de páginas
            //excede do número limite de paginadores (limiteNumeroPaginadores). É isso que evita estouro de layout, quebra de linha, etc...
            //Supondo que:
            //paginaAtual = 5
            //totalItens = 27
            //itensPorPagina = 3
            //limiteNumeroPaginadores = 5
            //offSetResultante = 2
            //totalPaginas = 9
            //O programa essencialmente pega a parte inteira da divisão do limite (5) por 2 e com isso, 
            //na hora da montagem dos paginadores númericos (consulte #region Números da paginação no else) verifica se, com base na página atual, ele precisará ou não incluir
            //as reticências no início e/ou no final dos numeradores.
            //A seguinte "pergunta" é feita nesta hora: paginaAtual(5) - offSetResultante(2) é maior do que 1 ?
            //Se sim, então inclua reticências com valor de página (paginaAtual(5) - offSetResultante(2) - 1)
            //E aí por último pergunta-se: paginaAtual(5) + offSetResultante(2) é menor que o totalDePaginas(9)? Se sim, então inclua reticências com valor de página (paginaAtual(5) + offSetResultante(2) + 1)
            //O resultado do HTML deve ser o seguinte: < ... 3 4 5 6 7 ... > em que as reticências da esquerda representam o número 2 e as da direita a número 8 do paginador
            int offSetPaginador = 0;

            if (limiteNumeroPaginadores % 2 == 0)
                offSetPaginador = limiteNumeroPaginadores / 2 - 1;
            else
                offSetPaginador = (int)Math.Floor((double)limiteNumeroPaginadores / 2D);

            StringBuilder sbPaginadorHTML = new StringBuilder();

            sbPaginadorHTML.Append("<ul class=\"paginate\">");

            #region Botão link Anterior
            //Constrói no início do paginador o botão "<" que, quando se trata da página 1, é só item de texto sem funcionalidade
            //mas, a partir da página 2 em diante é um link para a página anterior
            if (totalDePaginas > 1 && pagina > 1)
                sbPaginadorHTML.AppendFormat("<li class=\"previous\"><a class=\"png\" href=\"{0}\" title=\"&lt;\">&lt;</a></li>", this.MontarURLComParametroPagina(pagina - 1,ancora));
            else
                sbPaginadorHTML.Append("<li class=\"previous\"><a class=\"png\" href=\"#\" title=\"&lt;\">&lt;</a></li>");
            #endregion

            #region Números da paginação
            if (totalDePaginas <= limiteNumeroPaginadores)
            {
                for (int p = 1; p <= totalDePaginas; p++)
                {
                    sbPaginadorHTML.Append("<li>");

                    if (p.Equals(pagina))
                        sbPaginadorHTML.AppendFormat("<a href=\"#\" class=\"selected\" title=\"{0}\">{0}</a>", p);
                    else
                        sbPaginadorHTML.AppendFormat("<a href=\"{0}\" title=\"{1}\">{1}</a>", this.MontarURLComParametroPagina(p,ancora), p);

                    sbPaginadorHTML.Append("</li>");
                }
            }
            //Quando o número de páginas excede do limite de números que representam as páginas,
            //Vide explicação mais acima sobre offSetResultante e reticências
            else
            {
                if (pagina - offSetPaginador > 1)
                    sbPaginadorHTML.AppendFormat("<li><a href=\"{0}\" title=\"{1}\">...</a></li>", this.MontarURLComParametroPagina(pagina - offSetPaginador - 1,ancora), pagina - offSetPaginador - 1);

                //Começa do número 1 quando [paginaAtual - offSetPaginador] resulta em valor <= 0
                if (pagina - offSetPaginador <= 0) 
                {
                    int biggestPagerNumber = 0;
                    for (int p = 1; p <= limiteNumeroPaginadores; p++)
                    {
                        sbPaginadorHTML.Append("<li>");

                        if (p.Equals(pagina))
                            sbPaginadorHTML.AppendFormat("<a href=\"#\" class=\"selected\" title=\"{0}\">{0}</a>", p);
                        else
                            sbPaginadorHTML.AppendFormat("<a href=\"{0}\" title=\"{1}\">{1}</a>", this.MontarURLComParametroPagina(p,ancora), p);

                        sbPaginadorHTML.Append("</li>");

                        biggestPagerNumber = p; //Grava o maior número após fim da iteração
                    }

                    if (totalDePaginas > biggestPagerNumber)
                        sbPaginadorHTML.AppendFormat("<li><a href=\"{0}\" title=\"{1}\">...</a></li>", this.MontarURLComParametroPagina(biggestPagerNumber + 1,ancora), biggestPagerNumber + 1);
                }
                //Caso contrário
                else
                {
                    //Começa a partir do número [Pagina Atual - OffSet de Página] e vai até [Pagina Atual + OffSet de Página, desde que não ultrapasse o nro. total de páginas]
                    for (int p = pagina - offSetPaginador; p <= pagina + offSetPaginador && p <= totalDePaginas; p++)
                    {
                        sbPaginadorHTML.Append("<li>");

                        if (p.Equals(pagina))
                            sbPaginadorHTML.AppendFormat("<a href=\"#\" class=\"selected\" title=\"{0}\">{0}</a>", p);
                        else
                            sbPaginadorHTML.AppendFormat("<a href=\"{0}\" title=\"{1}\">{1}</a>", this.MontarURLComParametroPagina(p,ancora), p);

                        sbPaginadorHTML.Append("</li>");
                    }

                    if (totalDePaginas > pagina + offSetPaginador)
                        sbPaginadorHTML.AppendFormat("<li><a href=\"{0}\" title=\"{1}\">...</a></li>", this.MontarURLComParametroPagina(pagina + offSetPaginador + 1,ancora), pagina + offSetPaginador + 1);
                }
            }
            #endregion

            #region Botão link Próximo
            //Constrói no final do paginador o botão ">" que, quando se trata da última página, é só item de texto sem funcionalidade
            //mas, a caso seja penúltima ou anterior, é um link para a próxima página
            if (pagina < totalDePaginas)
                sbPaginadorHTML.AppendFormat("<li class=\"next\"><a class=\"png\" href=\"{0}\" title=\"&gt;\">&gt;</a></li>", this.MontarURLComParametroPagina(pagina + 1,ancora));
            else
                sbPaginadorHTML.Append("<li class=\"next\"><a class=\"png\" href=\"#\" title=\"&gt;\">&gt;</a></li>");
            #endregion

            //Fecha o paginador
            sbPaginadorHTML.Append("</ul>");

            //Retorna o HTML do paginador
            return sbPaginadorHTML.ToString();
        }
    }
}