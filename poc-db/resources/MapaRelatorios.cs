using System;

namespace Redecard.PN.Extrato.SharePoint.Helper
{
    public class MapaRelatorios
    {
        /// <summary>
        /// Método para obter o mapeamento dos relatórios de extrato com os lançamentos de conta corrente
        /// </summary>
        /// <param name="tipoLancamento">Tipo do Lançamento</param>
        /// <returns>Tipo do relatório</returns>
        public static Int32? ObterMapaTipoRelatorio(Int32 tipoLancamento)
        {
            //<option value="0">Vendas</option>
            //<option value="1">Valores Pagos</option>
            //<option value="4">Lançamentos Futuros</option>
            //<option value="2">Ordens de Crédito</option>
            //<option value="3">Pagamentos Ajustados</option>
            //<option value="5">Antecipações</option>
            //<option value="6">Débitos e Desagendamentos</option>
            //<option value="7">Serviços</option>
            //<option value="8">Créditos Suspensos, Retidos e Penhorados</option>
            //<option selected="selected" value="13">Conta Corrente</option>

            Int32? tipo = default(Int32?);

            switch (tipoLancamento)
            {
                case 30:
                    tipo = 0;
                    break;
                case 31:
                    tipo = 0;
                    break;
                case 20:
                    tipo = 1;
                    break;
                case 36:
                    tipo = 1;
                    break;
                case 38:
                    tipo = 5;
                    break;
                case 39:
                    tipo = 3;
                    break;
                case 21:
                    tipo = 6;
                    break;
                case 22:
                    tipo = 6;
                    break;
                case 23:
                    tipo = 6;
                    break;
                case 24:
                    tipo = 6;
                    break;
                case 25:
                    tipo = 6;
                    break;
                case 26:
                    tipo = 6;
                    break;
                case 27:
                    tipo = 6;
                    break;
                case 47:
                    tipo = 6;
                    break;
                case 48:
                    tipo = 6;
                    break;
                case 51:
                    tipo = 8;
                    break;
                case 52:
                    tipo = 8;
                    break;
                case 53:
                    tipo = 8;
                    break;
                case 54:
                    tipo = 8;
                    break;
                case 55:
                    tipo = 8;
                    break;
                case 56:
                    tipo = 8;
                    break;
                case 57:
                    tipo = 6;
                    break;
                case 58:
                    tipo = 6;
                    break;
                default:
                    tipo = null;
                    break;
            }

            return tipo;
        }

        /// <summary>
        /// Método para obter o mapeamento dos tipos de venda com os lançamentos de conta corrente
        /// </summary>
        /// <param name="tipoLancamento">Tipo do Lançamento</param>
        /// <returns>Tipo de venda</returns>
        public static Int32? ObterMapaTipoVenda(Int32 tipoLancamento)
        {
            //tipo = 1 - Débito
            //tipo = 0 - Crédito
            //tipo = 3 - Todos
            Int32? tipo = default(Int32?);

            switch (tipoLancamento)
            {
                case 30:
                    tipo = 0;
                    break;
                case 31:
                    tipo = 1;
                    break;
                case 20:
                    tipo = 0;
                    break;
                case 36:
                    tipo = 1; 
                    break;
                case 38:
                    tipo = 0;
                    break;
                case 39:
                    tipo = 0;
                    break;
                case 21:
                    tipo = 0;
                    break;
                case 22:
                    tipo = 0;
                    break;
                case 23:
                    tipo = 0;
                    break;
                case 24:
                    tipo = 0;
                    break;
                case 25:
                    tipo = 0;
                    break;
                case 26:
                    tipo = 0;
                    break;
                case 27:
                    tipo = 0;
                    break;
                case 47:
                    tipo = 0;
                    break;
                case 48:
                    tipo = 0;
                    break;
                case 51:
                    tipo = 0;
                    break;
                case 52:
                    tipo = 0;
                    break;
                case 53:
                    tipo = 0;
                    break;
                case 54:
                    tipo = 0;
                    break;
                case 55:
                    tipo = 0;
                    break;
                case 56:
                    tipo = 0;
                    break;
                case 57:
                    tipo = 0;
                    break;
                case 58:
                    tipo = 0;
                    break;
                default:
                    tipo = 0;
                    break;
            }

            return tipo;
        }
    }
}
