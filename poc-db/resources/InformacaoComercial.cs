using System;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de Informação Comercial.
    /// </summary>
    public class InformacaoComercial
    {
        /// <summary>
        /// Define o Número do Ponto de Venda.
        /// </summary>
        public Decimal? NumeroPDV { get; set; }

        /// <summary>
        /// Define o Código do Usuário.
        /// </summary>
        public Decimal? CodigoUsuario { get; set; }

        /// <summary>
        /// Define o Status do Aceite.
        /// </summary>
        public String StatusAceite { get; set; }

        /// <summary>
        /// Define a Data do Status do Aceite.
        /// </summary>
        public DateTime? DataStatusAceite { get; set; }

        /// <summary>
        /// Define a Razão Social.
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Define o Código do Ramo de Atividade.
        /// </summary>
        public Decimal? CodigoRamoAtividade { get; set; }

        /// <summary>
        /// Define a Descrição do Ramo de Atividade.
        /// </summary>
        public String DescricaoRamoAtividade { get; set; }

        /// <summary>
        /// Define o Responsável pelo Ponto de Venda.
        /// </summary>
        public String Responsavel { get; set; }

        /// <summary>
        /// Define o DDD do Telefone 1.
        /// </summary>
        public Decimal? DDD1 { get; set; }

        /// <summary>
        /// Define o Telefone 1.
        /// </summary>
        public Decimal? Telefone1 { get; set; }

        /// <summary>
        /// Define o Ramal1.
        /// </summary>
        public Decimal? Ramal1 { get; set; }

        /// <summary>
        /// Define o DDD do Telefone 2.
        /// </summary>
        public Decimal? DDD2 { get; set; }

        /// <summary>
        /// Define o Telefone 2.
        /// </summary>
        public Decimal? Telefone2 { get; set; }

        /// <summary>
        /// Define o Ramal2.
        /// </summary>
        public Decimal? Ramal2 { get; set; }

        /// <summary>
        /// Define o valor da Taxa de Adesão.
        /// </summary>
        public Decimal? ValorTaxaAdesao { get; set; }

        /// <summary>
        /// Define o valor do Servico do ZP
        /// </summary>
        public Decimal? ValorServico { get; set; }

        /// <summary>
        /// Define o valor da Taxa de Adesão.
        /// </summary>
        public Decimal? CodigoEstabelecimento { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Socio 
        /// definido pela propriedade <see cref="Socio"/>.
        /// </summary>
        public Socio[] Socios { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Endereco 
        /// definido pela propriedade <see cref="Endereco"/>.
        /// </summary>
        public Endereco[] Enderecos { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Domicílio Bancário 
        /// definido pela propriedade <see cref="DomicilioBancario"/>.
        /// </summary>
        public DomicilioBancario[] DomiciliosBancarios { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Serviço Contratado 
        /// definido pela propriedade <see cref="ServicoContratado"/>.
        /// </summary>
        public ServicoContratado[] ServicosContratados { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Serviço Contratado 
        /// definido pela propriedade <see cref="ServicoContratado"/>.
        /// </summary>
        public TerminalContratado[] Terminais { get; set; }

        /// <summary>
        /// Disponibiliza uma instância de Dados de Preco Unico 
        /// definido pela propriedade <see cref="PrecoUnico"/>.
        /// </summary>
        public PrecoUnico DadosPrecoUnico { get; set; }
    }
}
