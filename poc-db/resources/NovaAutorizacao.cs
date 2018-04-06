using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{
    [Serializable]
    public class NovaAutorizacao
    {
        /// <summary>Número do Pedido</summary>
        public String NumeroPedido { get; set; }

        /// <summary>Valor da Transação</summary>
        public Decimal ValorTransacao { get; set; }

        /// <summary>Transação</summary>
        public Modelos.RegistroTransacaoHistoricRecurring Transacao { get; set; }
    }
}
