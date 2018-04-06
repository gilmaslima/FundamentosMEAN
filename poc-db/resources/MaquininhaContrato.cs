using System;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Modelo para o contrato da oferta maquininha conta certa
    /// </summary>
    public class MaquininhaContrato
    {
        /// <summary>
        /// CNPJ
        /// </summary>
        public String Cnpj { get; set; }

        /// <summary>
        /// Código do canal itaú
        /// </summary>
        public String CodigoCanalItau { get; set; }

        /// <summary>
        /// Código do motivo da recusa do contrato
        /// </summary>
        public String CodigoMotivoRecusaContrato { get; set; }

        /// <summary>
        /// Código da oferta
        /// </summary>
        public String CodigoOfertaPacote { get; set; }

        /// <summary>
        /// Código da situação do contrato
        /// </summary>
        public String CodigoSituacaoContrato { get; set; }

        /// <summary>
        /// Código da tecnologia
        /// </summary>
        public String CodigoTecnologia { get; set; }

        /// <summary>
        /// Código do usuário que incluiu/alterou o contrato
        /// </summary>
        public String CodigoUsuarioInclusaoOuAlteracao { get; set; }

        /// <summary>
        /// Data da ativação do contrato
        /// </summary>
        public DateTime DataAtivacao { get; set; }

        /// <summary>
        /// Data do cancelamento do contrato
        /// </summary>
        public DateTime DataCancelamento { get; set; }

        /// <summary>
        /// Data do contrato
        /// </summary>
        public DateTime DataContrato { get; set; }

        /// <summary>
        /// Data fim de vigência do contrato
        /// </summary>
        public DateTime DataFimVigencia { get; set; }

        /// <summary>
        /// Data início de vigência do contrato
        /// </summary>
        public DateTime DataInicioVigencia { get; set; }

        /// <summary>
        /// Data da solicitação do cancelamento
        /// </summary>
        public DateTime DataSolicitacaoCancelamento { get; set; }

        /// <summary>
        /// Data da inclusão/alteração do contrato
        /// </summary>
        public DateTime DataUsuarioInclusaoAlteracao { get; set; }

        /// <summary>
        /// Descrição do contrato
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Descrição do canal do Itaú
        /// </summary>
        public String DescricaoCanalItau { get; set; }

        /// <summary>
        /// Descrição do motivo da recusa do contrato
        /// </summary>
        public String DescricaoMotivoRecusaContrato { get; set; }

        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        public String NumEstabelecimento { get; set; }

        /// <summary>
        /// Quantidade de terminais contratados no pacote
        /// </summary>
        public Int32 QtdTerminaisDoPacote { get; set; }

        /// <summary>
        /// Valor base dos demais terminais
        /// </summary>
        public Decimal ValorBaseDemaisTerminais { get; set; }

        /// <summary>
        /// Valor base do primeiro terminal
        /// </summary>
        public Decimal ValorBasePrimeiroTerminal { get; set; }

        /// <summary>
        /// Valor do pacote básico
        /// </summary>
        public Decimal ValorPacoteBasico { get; set; }

        /// <summary>
        /// Valor dos terminais
        /// </summary>
        public Decimal ValorTerminais { get; set; }
        
        /// <summary>
        /// Valor total do pacote
        /// </summary>
        public Decimal ValorTotalPacote { get; set; }
    }
}
