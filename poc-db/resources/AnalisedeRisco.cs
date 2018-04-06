using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{

    public enum TipoDocumento
    {
        Passaporte = 1,
        RG = 2,
        CPF = 3,
        CarteiraHabilitacao = 4,
        RegistroFederalContribuintes = 16,
        TítuloEleitor = 17


    }
    [Serializable]
    public class AnalisedeRisco
    {
        public DadosGerais DadosGerais { get; set; }
        public DadosEntrega DadosEntrega { get; set; }
        public DadosCobranca DadosCobranca { get; set; }
        public DetalhesPedido DetalhesPedido { get; set; }
        public DadosIATA DadosIATA { get; set; }
    }

    /// <summary>
    /// Dados gerias da análise de riso
    /// </summary>
    [Serializable]
    public class DadosGerais
    {
        /// <summary>
        /// Código do cliente
        /// </summary>
        public String CodigoCliente { get; set; }
        /// <summary>
        /// Email do Cliente
        /// </summary>
        public String Email { get; set; }
        /// <summary>
        /// DDD do telefone
        /// </summary>
        public String DDDTelefone { get; set; }
        /// <summary>
        /// Número do Telefone
        /// </summary>
        public String Telefone { get; set; }
        /// <summary>
        /// DDD do celular
        /// </summary>
        public String DDDCelular { get; set; }
        /// <summary>
        /// Número do celular
        /// </summary>
        public String Celular { get; set; }
        /// <summary>
        /// Data de nascimento
        /// </summary>
        public String DataNascimento { get; set; }
        /// <summary>
        /// Número do documento apresentado
        /// </summary>
        public String Documento { get; set; }        
    }

    /// <summary>
    /// Dados de entrega da análise de risco
    /// </summary>
    [Serializable]
    public class DadosEntrega
    {
        /// <summary>
        /// Nome do destinatário
        /// </summary>
        public String NomeDestinatario { get; set; }
        /// <summary>
        /// Sobrenome do destinatário
        /// </summary>
        public String SobrenomeDestinatario { get; set; }
        /// <summary>
        /// Data da coleta
        /// </summary>
        public String DataColeta { get; set; }
        /// <summary>
        /// Indica se requer instalação
        /// </summary>
        public Boolean RequerInstalacao { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public Endereco Endereco { get; set; }

        /// <summary>
        /// Box Expandido
        /// </summary>
        public Boolean BoxExpandido { get; set; }
    }

    /// <summary>
    /// Dados de cobrança da análise de risco
    /// </summary>
    [Serializable]
    public class DadosCobranca
    {
        /// <summary>
        /// Endereço
        /// </summary>
        public Endereco Endereco { get; set; }

        /// <summary>
        /// Box Expandido
        /// </summary>
        public Boolean BoxExpandido { get; set; }
    }

    /// <summary>
    /// Dados do pedido da análise de risco
    /// </summary>
    [Serializable]
    public class DetalhesPedido
    {
        public List<Produto> Produtos { get; set; }

        /// <summary>
        /// Box Expandido
        /// </summary>
        public Boolean BoxExpandido { get; set; }
    }

    /// <summary>
    /// Dados do IATA da análise de risco
    /// </summary>
    [Serializable]
    public class DadosIATA
    {
        /// <summary>
        /// Código da companhia
        /// </summary>
        public String CodigoCompanhia { get; set; }        
        /// <summary>
        /// Número do Vôo
        /// </summary>
        public String NumeroVoo { get; set; }
        /// <summary>
        /// Base Tarifária
        /// </summary>
        public Int32 BaseTarifaria { get; set; }        
        /// <summary>
        /// Lista dos passageiros
        /// </summary>
        public List<DadosPassageiro> Passageiros { get; set; }
        /// <summary>
        /// CPF do portador do cartão
        /// </summary>
        public String CPF { get; set; }
        /// <summary>
        /// Nome do portador do cartão
        /// </summary>
        public String Nome { get; set; }
        /// <summary>
        /// Sobrenome do portador do cartão
        /// </summary>
        public String Sobrenome { get; set; }
        /// <summary>
        /// Box Expandido
        /// </summary>
        public Boolean BoxExpandido { get; set; }
    }

    /// <summary>
    /// Dados do produto
    /// </summary>
    [Serializable]
    public class Produto
    {
        /// <summary>
        /// Código do produto
        /// </summary>
        public String Codigo { get; set; }
        /// <summary>
        /// Nome do produto
        /// </summary>
        public String Nome { get; set; }
        /// <summary>
        /// Categoria do produto
        /// </summary>
        public String Categoria { get; set; }
        /// <summary>
        /// Quantidade pedida
        /// </summary>
        public Int16 Quantidade { get; set; }
        /// <summary>
        /// Preço unitário
        /// </summary>
        public Double PrecoUnitario { get; set; }
        /// <summary>
        /// Preço unitário sem pontuação
        /// </summary>
        public String PrecoUnitarioSemPontuacao 
        {
            get
            {
                return PrecoUnitario.ToString("N2", new System.Globalization.CultureInfo("pt-BR"))
                    .ToString().Replace(".", "").Replace(",", "");
            }
        }
        /// <summary>
        /// Grau de risco da transação
        /// </summary>
        public String GrauRisco { get; set; }
        /// <summary>
        /// Descrição dp grau de risco da transação
        /// </summary>
        public String GrauRiscoDescricao { get; set; }
    }

    /// <summary>
    /// Dados do Passageiro para IATA
    /// </summary>
    [Serializable]
    public class DadosPassageiro
    {
        /// <summary>
        /// Tipo de documento
        /// </summary>
        public String TipoDocumento { get; set; }
        /// <summary>
        /// Descrição do Tipo de documento
        /// </summary>
        public String TipoDocumentoDescricao { get; set; }
        /// <summary>
        /// Número do documento
        /// </summary>
        public String NumeroDocumento { get; set; }
        /// <summary>
        /// Tipo do documento
        /// </summary>
        public String TipoPassageiro { get; set; }
        /// <summary>
        /// Descrição do Tipo do documento
        /// </summary>
        public String TipoPassageiroDescricao { get; set; }
        /// <summary>
        /// Tipo do programa de fidelidade
        /// </summary>
        public String TipoProgramaFidelidade { get; set; }
        /// <summary>
        /// Descrição do Tipo do programa de fidelidade
        /// </summary>
        public String TipoProgramaFidelidadeDescricao { get; set; }
        /// <summary>
        /// País de origem
        /// </summary>
        public String PaisOrigem { get; set; }
        /// <summary>
        /// Descrição do País de origem
        /// </summary>
        public String PaisOrigemDescricao { get; set; }
        /// <summary>
        /// Código do programa de fidelidade
        /// </summary>
        public String CodigoProgramaFidelidade { get; set; }
    }
}
