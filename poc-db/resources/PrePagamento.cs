using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    /// <summary>
    /// Classe com os dados dos Pré-Pagamento
    /// </summary>
    [DataContract]
    public class PrePagamento
    {
        /// <summary>
        /// Data de processamento referente ao pagamento que será efetuado para a Redecard.
        /// </summary>
        [DataMember]
        public DateTime DataProcessamento { get; set; }
        
        /// <summary>
        /// Data de vencimento referente ao pagamento que será efetuado para a Redecard.
        /// </summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Valor que será pago aos Emissores
        /// </summary>
        [DataMember]
        public Double ValorFEE { get; set; }

        /// <summary>
        /// Valor bruto a receber pelos pagamentos
        /// </summary>
        [DataMember]
        public Double ValorBruto { get; set; }

        /// <summary>
        /// Valor do Líquido que a Redecard irá receber: valor já calculado.
        /// Calculado através do Valor Bruto (bruto das vendas da data de vencimento) - Valor FEE.
        /// </summary>
        [DataMember]
        public Double ValorPagarReceber { get; set; }

        /// <summary>
        /// Valor saldo já antecipado
        /// </summary>
        [DataMember]
        public Double SaldoAntecipado { get; set; }

        /// <summary>
        /// Valor líquido do saldo em estoque do que será pago pela Redecard.
        /// Valor calculado pelo Valor Valor Pagar/Receber - Saldo Antecipado
        /// </summary>
        [DataMember]
        public Double SaldoEstoque { get; set; }

        /// <summary>
        /// Bandeira associada ao pré-pagamento
        /// </summary>
        [DataMember]
        public Bandeira Bandeira { get; set; }

        /// <summary>
        /// Banco associada ao pré-pagamento
        /// </summary>
        [DataMember]
        public Emissor Banco { get; set; }


        /// <summary>
        /// Número da linha do arquivo de Carga
        /// </summary>
        [DataMember]
        public Int32 NumeroLinha { get; set; }

        /// <summary>
        /// Confirmação se o registro de Pré-Pagamento foi carregado corretamente
        /// </summary>
        [DataMember]
        public Boolean ConfirmacaoCarga { get; set; }

        /// <summary>
        /// Mensagem de erro que ocorreu após a carga
        /// </summary>
        [DataMember]
        public String MensagemErroCarga { get; set; }
    }
}