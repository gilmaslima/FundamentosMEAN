using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Boston.Sharepoint
{
    /// <summary>
    /// Classe do Comprovante de Venda
    /// </summary>
    [Serializable]
    public class ComprovanteVenda
    {
        /// <summary>
        /// NSU
        /// </summary>
        public String NSU { get; set; }

        /// <summary>
        /// TID
        /// </summary>
        public String TID { get; set; }

        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public String NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Nome do Estabelecimento
        /// </summary>
        public String NomeEstabelecimento { get; set; }

        /// <summary>
        /// Data de Pagamento
        /// </summary>
        public String DataPagamento { get; set; }

        /// <summary>
        /// Hora do Pagamento
        /// </summary>
        public String HoraPagamento { get; set; }

        /// <summary>
        /// Número de Autorização
        /// </summary>
        public String NumeroAutorizacao { get; set; }

        /// <summary>
        /// Tipo de Transação
        /// </summary>
        public String TipoTransacao { get; set; }

        /// <summary>
        /// Forma de Pagamento
        /// </summary>
        public String FormaPagamento { get; set; }

        /// <summary>
        /// Bandeira
        /// </summary>
        public String Bandeira { get; set; }

        /// <summary>
        /// Nome do Portador
        /// </summary>
        public String NomePortador { get; set; }

        /// <summary>
        /// Número do Cartão
        /// </summary>
        public String NumeroCartao { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        public String Valor { get; set; }

        /// <summary>
        /// Número de Parcelas
        /// </summary>
        public String NumeroParcelas { get; set; }

        /// <summary>
        /// Número do Pedido
        /// </summary>
        public String NumeroPedido { get; set; }
    }
}
