/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{
    /// <summary>
    /// Parâmetros de Retorno Mobile (Novo Pedido Leitor de Cartão)
    /// </summary>
    [Serializable]
    public class PedidoNovoLeitorRetorno
    {
        /// <summary>
        /// Pedido novo Leitor
        /// </summary>
        public PedidoNovoLeitor Parametros { get; set; }

        /// <summary>
        /// Código do Retorno
        /// </summary>
        public Int32 CodigoRetorno { get; set; }

        /// <summary>
        /// Fonte
        /// </summary>
        public String Fonte { get; set; }

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        public String Mensagem { get; set; }

        #region [ Retorno Transação DataCash ]

        /// <summary>
        /// Data da Confirmação da transação
        /// </summary>
        public DateTime DataConfirmacao { get; set; }

        /// <summary>
        /// Data da Pré-Autorização
        /// </summary>
        public DateTime DataPreAutorizacao { get; set; }

        /// <summary>
        /// Data da Transação
        /// </summary>
        public DateTime DataTransacao { get; set; }

        /// <summary>
        /// Forma Pagamento
        /// </summary>
        public String FormaPagamento { get; set; }

        /// <summary>
        /// Hora da Confirmação
        /// </summary>
        public String HoraConfirmacao { get; set; }

        /// <summary>
        /// Hora da Pré Autorização
        /// </summary>
        public String HoraPreAutorizacao { get; set; }

        /// <summary>
        /// Hora da Transação
        /// </summary>
        public DateTime HoraTransacao { get; set; }

        /// <summary>
        /// Número Sequencial Único
        /// </summary>
        public String NSU { get; set; }

        /// <summary>
        /// Número do Pedido
        /// </summary>
        public String NumeroPedido { get; set; }

        /// <summary>
        /// Número do Autorização
        /// </summary>
        public String NumeroAutorizacao { get; set; }

        /// <summary>
        /// ID da Transação
        /// </summary>
        public String TID { get; set; }

        /// <summary>
        /// Tipo de Transação
        /// </summary>
        public String TipoTransacao { get; set; }

        /// <summary>
        /// Validade da Pré-Autorização
        /// </summary>
        public DateTime ValidadePreAutorizacao { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        public Decimal Valor { get; set; }

        /// <summary>
        /// Valor formatado
        /// </summary>
        public String ValorFormatado { get; set; }

        /// <summary>
        /// Valor da Pré-Autorização
        /// </summary>
        public Decimal ValorPreAutorizacao { get; set; }

        /// <summary>
        /// Dados Cartão: Ano Validade
        /// </summary>
        public String CartaoAnoValidade { get; set; }

        /// <summary>
        /// Dados Cartão: Bandeira
        /// </summary>
        public String CartaoBandeira { get; set; }

        /// <summary>
        /// Dados Cartão: Código de Segurança
        /// </summary>
        public String CartaoCodigoSeguranca { get; set; }

        /// <summary>
        /// Dados Cartão: Mês Validade
        /// </summary>
        public String CartaoMesValidade { get; set; }

        /// <summary>
        /// Dados Cartão: Nome do Portador
        /// </summary>
        public String CartaoNomePortador { get; set; }

        /// <summary>
        /// Dados Cartão: Número
        /// </summary>
        public String CartaoNumero { get; set; }

        /// <summary>
        /// Dados Cartão: Número Criptografado
        /// </summary>
        public String CartaoNumeroCriptografado { get; set; }

        /// <summary>
        /// Dados Cartão: Número Formatado
        /// </summary>
        public String CartaoNumeroFormatado { get; set; }

        /// <summary>
        /// Dados Cartão: Parcelas
        /// </summary>
        public String CartaoParcelas { get; set; }

        /// <summary>
        /// Dados Cartão: Validade
        /// </summary>
        public String CartaoValidade { get; set; }

        #endregion

        #region [ Retorno Inclusão WF ]

        /// <summary>
        /// Código de retorno do WF
        /// </summary>
        public Int32 CodigoRetornoWf { get; set; }

        /// <summary>
        /// Mensagem de retorno do WF
        /// </summary>
        public String MensagemWf { get; set; }

        #endregion

        #region [ Dados do Pedido ]

        /// <summary>
        /// Quantidade de equipamentos CCM
        /// </summary>
        public Int32 QuantidadeCCM { get; set; }

        /// <summary>
        /// Quantidade de equipamentos CPA
        /// </summary>
        public Int32 QuantidadeCPA { get; set; }

        /// <summary>
        /// Quantidade de equipamentos CPC
        /// </summary>
        public Int32 QuantidadeCPC { get; set; }

        #endregion
    }
}