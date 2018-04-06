using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class ProdutoParceiro
    {
        /// <summary>
        /// Nº máximo de refeições por dia
        /// </summary>
        [DataMember]
        public Int32 NumMaximoRefeicoes { get; set; }

        /// <summary>
        /// Área da loja m²
        /// </summary>
        [DataMember]
        public Int32 AreaLoja { get; set; }

        /// <summary>
        /// Área de atendimento público m²
        /// </summary>
        [DataMember]
        public Int32 AreaAtendimento { get; set; }

        /// <summary>
        /// Número de mesas
        /// </summary>
        [DataMember]
        public Int32 NumMesas { get; set; }

        /// <summary>
        /// Número de assentos
        /// </summary>
        [DataMember]
        public Int32 NumAssentos { get; set; }

        /// <summary>
        /// Quantidade de checkouts na loja
        /// </summary>
        [DataMember]
        public Int32 QtdCheckOutLoja { get; set; }

        /// <summary>
        /// Indicador Segunda a Sexta
        /// </summary>
        [DataMember]
        public Boolean IndcSegSex { get; set; }

        /// <summary>
        /// Indicador Sábado
        /// </summary>
        [DataMember]
        public Boolean IndcSabado { get; set; }

        /// <summary>
        /// Indicador Domingo
        /// </summary>
        [DataMember]
        public Boolean IndcDomingo { get; set; }

        /// <summary>
        /// Indicador Comercial
        /// </summary>
        [DataMember]
        public Boolean IndcComercial { get; set; }

        /// <summary>
        /// Indicador Noturno
        /// </summary>
        [DataMember]
        public Boolean IndcNoturno { get; set; }

        /// <summary>
        /// Indicador 24h
        /// </summary>
        [DataMember]
        public Boolean Indc24h { get; set; }

        /// <summary>
        /// Código do Canal
        /// </summary>
        [DataMember]
        public Int32? CodigoCanal { get; set; }

        /// <summary>
        /// Código do Celula
        /// </summary>
        [DataMember]
        public Int32? CodigoCelula { get; set; }

        /// <summary>
        /// Produto Voucher
        /// </summary>
        [DataMember]
        public Produto Produto { get; set; }

        /// <summary>
        /// Número Cnpj
        /// </summary>
        [DataMember]
        public Int64 NumeroCnpj { get; set; }

        /// <summary>
        /// Número Ponto de venda
        /// </summary>
        [DataMember]
        public Int32? NumeroPontoDeVenda { get; set; }

        /// <summary>
        /// Código Tipo Estabelecimento
        /// </summary>
        [DataMember]
        public Int32? CodigoTipoEstabelecimento { get; set; }

        /// <summary>
        /// Código Status Habilitação
        /// </summary>
        [DataMember]
        public Int32 StatusHabilitacao { get; set; }

        /// <summary>
        /// Número Ponto de venda
        /// </summary>
        [DataMember]
        public Int32? Banco { get; set; }

        /// <summary>
        /// Número Agência
        /// </summary>
        [DataMember]
        public Int32? Agencia { get; set; }

        /// <summary>
        /// Número Conta
        /// </summary>
        [DataMember]
        public String Conta { get; set; }

        /// <summary>
        /// Usuário
        /// </summary>
        [DataMember]
        public String UsuarioAlteracao { get; set; }

        /// <summary>
        /// Data Solicitação Credenciamento
        /// </summary>
        [DataMember]
        public DateTime DataSolicitacao { get; set; }

        /// <summary>
        /// Data Última Devolução
        /// </summary>
        [DataMember]
        public DateTime? DataUltimaDevolucao { get; set; }

        /// <summary>
        /// Data Cancelamento
        /// </summary>
        [DataMember]
        public DateTime? DataCancelamento { get; set; }

        /// <summary>
        /// Data Aprovação
        /// </summary>
        [DataMember]
        public DateTime? DataAprovacao { get; set; }

        /// <summary>
        /// Data Última Atualização
        /// </summary>
        [DataMember]
        public DateTime DataUltimaAtualizacao { get; set; }

        /// <summary>
        /// Motivo da Devolução
        /// </summary>
        [DataMember]
        public String MotivoDevolucao { get; set; }

        /// <summary>
        /// Motivo Cancelamento
        /// </summary>
        [DataMember]
        public String MotivoCancelamento { get; set; }
    }
}
