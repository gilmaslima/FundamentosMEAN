using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// GRADE PARA LIQUIDACAO FINANCEIRA (SPB) AO BANCOS 
    /// BOOK : PV763CB        
    /// REG-COMM-INTE-OCC
    /// </summary>
    public class GradeLiquidacao : ModeloBase
    {
        /// <summary>
        /// TIPO DE REGISTRO.
        /// 01 - REGISTRO BANCO PRINCIPAL.
        /// 02 - REGISTRO BANCOS SECUNDARIOS.
        /// </summary>
        public int Tipo { get; set; }

        /// <summary>
        ///TIPO DE SOLICITACAO.
        ///B - BANCO PARTICIPANTE
        ///L - BANCO LIQUIDANDE
        /// </summary>
        public string TipoSolicitacao { get; set; }

        /// <summary>
        ///TIPO DE MOVIMENTACAO.
        ///C - VALOR A RECEBER
        ///D - VALOR A PAGAR
        /// </summary>
        public string TipoMovimentacao { get; set; }

        /// <summary>
        /// SALDO TOTAL PARA LIQUIDACAO.
        /// </summary>
        public decimal ValorSaldoLiquidacao { get; set; }

        /// <summary>
        /// CODIGO DA AGENCIA DO BANCO
        /// </summary>
        public string Agencia { get; set; }

        /// <summary>
        /// CODIGO DA CONTA DO BANCO.
        /// </summary>
        public string ContaCorrente { get; set; }
    }
}

