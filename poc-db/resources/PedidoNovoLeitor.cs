/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.Boston.Sharepoint.Modelo
{
    /// <summary>
    /// Parâmetros Mobile (Novo Pedido Leitor de Cartão)
    /// </summary>
    [Serializable]
    public class PedidoNovoLeitor
    {
        #region [ Dados dos Leitores ] 

        /// <summary>
        /// Quantidade Máxima de Parcelas CCM
        /// </summary>
        public Int32 QtdeMaximaParcelasCCM { get; set; }

        /// <summary>
        /// Taxa de Ativação CCM
        /// </summary>
        public Decimal TaxaAtivacaoCCM { get; set; }

        /// <summary>
        /// Quantidade Máxima de Parcelas CPA
        /// </summary>
        public Int32 QtdeMaximaParcelasCPA { get; set; }

        /// <summary>
        /// Taxa de Ativação CPA
        /// </summary>
        public Decimal TaxaAtivacaoCPA { get; set; }

        /// <summary>
        /// Quantidade Máxima de Parcelas CPC
        /// </summary>
        public Int32 QtdeMaximaParcelasCPC { get; set; }

        /// <summary>
        /// Taxa de Ativação CPC
        /// </summary>
        public Decimal TaxaAtivacaoCPC { get; set; }

        #endregion

        #region [ Dados do Endereço ]

        /// <summary>
        /// Endereço de Entrega: Bairro
        /// </summary>
        public String EnderecoBairro { get; set; }

        /// <summary>
        /// Endereço de Entrega: CEP
        /// </summary>
        public String EnderecoCEP { get; set; }

        /// <summary>
        /// Endereço de Entrega: Cidade
        /// </summary>
        public String EnderecoCidade { get; set; }

        /// <summary>
        /// Endereço de Entrega: Complemento
        /// </summary>
        public String EnderecoComplemento { get; set; }

        /// <summary>
        /// Endereço de Entrega: Estado
        /// </summary>
        public String EnderecoEstado { get; set; }

        /// <summary>
        /// Endereço de Entrega: Logradouro
        /// </summary>
        public String EnderecoLogradouro { get; set; }

        /// <summary>
        /// Endereço de Entrega: Número
        /// </summary>
        public String EnderecoNumero { get; set; }

        /// <summary>
        /// Endereço de Entrega: Tipo de Endereço
        /// </summary>
        public Char EnderecoTipoEndereco { get; set; }

        #endregion

        #region [ DataCash ]

        /// <summary>
        /// Código da entidade que será utilizada no DataCash
        /// </summary>
        public Int32 CodigoEntidadeDataCash { get; set; }

        #endregion
    }
}