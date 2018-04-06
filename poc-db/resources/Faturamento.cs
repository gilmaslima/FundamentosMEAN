using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Classe representando um Faturamento de um Mês/Ano Referência do Plano de Contas
    /// </summary>
    public class Faturamento
    {
        /// <summary>
        /// Código do Estabelecimento
        /// </summary>
        public Int32 NumeroPV { get; set; }

        /// <summary>
        /// Mês de Referência
        /// </summary>
        public Int16 MesReferencia { get; set; }

        /// <summary>
        /// Ano de Referência
        /// </summary>
        public Int16 AnoReferencia { get; set; }

        /// <summary>
        /// Valor de Faturamento
        /// </summary>
        public Decimal ValorFaturamento { get; set; }

        /// <summary>
        /// Valor de Aluguel
        /// </summary>
        public Decimal ValorAluguel { get; set; }

        /// <summary>
        /// Agência de Recebimento
        /// </summary>
        public Int32 AgenciaRecebimento { get; set; }

        /// <summary>
        /// Conta de Recebimento
        /// </summary>
        public String ContaRecebimento { get; set; }

        /// <summary>
        /// Banco de Recebimento
        /// </summary>
        public Int32 BancoRecebimento { get; set; }

        /// <summary>
        /// Status Elegibilidade
        /// </summary>
        public StatusElegibilidade StatusElegibilidade { get; set; }

        /// <summary>
        /// Status Elegibilidade 
        /// Caso retorno do mainframe não seja esperado - StatusElegibilidade=NAO_IDENTIFICADO
        /// </summary>
        public Int16 StatusElegibilidadeInt { get; set; }
    }
}
