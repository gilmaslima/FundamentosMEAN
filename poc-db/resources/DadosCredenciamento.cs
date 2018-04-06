using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Boston.Sharepoint
{
    /// <summary>
    /// Dados do Credenciamento
    /// </summary>
    [Serializable]
    public class DadosCredenciamento
    {
        #region [ Outros Dados ]

        /// <summary>
        /// Número de Sequência
        /// </summary>
        public Int32 NumeroSequencia { get; set; }

        /// <summary>
        /// Exige participação integral
        /// </summary>
        public Char ExigeParticipacaoIntegral { get; set; }

        /// <summary>
        /// Tipo do estabelecimento
        /// </summary>
        public Int32 CodTipoEstabelecimento { get; set; }

        /// <summary>
        /// Número do pdv da matriz
        /// </summary>
        public Int32 NumPdvMatriz { get; set; }

        /// <summary>
        /// CNAE
        /// </summary>
        public String CNAE { get; set; }

        /// <summary>
        /// Número da Ocorrência
        /// </summary>
        public Int32? NumOcorrencia { get; set; }

        /// <summary>
        /// Número do PV
        /// </summary>
        public Int32 NumPdv { get; set; }

        /// <summary>
        /// Indicador Marketing Direto
        /// </summary>
        public Char IndMarketingDireto { get; set; }

        /// <summary>
        /// Código Tipo Movimento
        /// </summary>
        public Char CodTipoMovimento { get; set; }

        /// <summary>
        /// Usuário Portal
        /// </summary>
        public String Usuario { get { return "PORTAL"; } }

        /// <summary>
        /// Canal
        /// </summary>
        public Int32 Canal { get; set; }

        /// <summary>
        /// Célula
        /// </summary>
        public Int32 Celula { get; set; }

        /// <summary>
        /// Código Equipamento
        /// </summary>
        public String CodEquipamento { get { return "MPO"; } }

        /// <summary>
        /// CCM Executada
        /// </summary>
        public Boolean CCMExecutada { get; set; }

        #endregion

        #region [ Dados Iniciais ]

        /// <summary>
        /// Número do CPF ou CNPJ
        /// </summary>
        public String CPF_CNPJ { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        public Char TipoPessoa { get; set; }

        /// <summary>
        /// Número do CPF ou CNPJ do Proprietário
        /// </summary>
        public String CPF_CNPJProprietario { get; set; }

        /// <summary>
        /// Descrição da Profissão
        /// </summary>
        public String DescricaoProfissao { get; set; }

        /// <summary>
        /// Código da Profissão
        /// </summary>
        public String CodigoProfissao { get; set; }

        /// <summary>
        /// Descrição de como conheceu o produto
        /// </summary>
        public String DescricaoComoConheceu { get; set; }

        /// <summary>
        /// Código de como conheceu o produto
        /// </summary>
        public String CodigoComoConheceu { get; set; }

        #endregion

        #region [ Dados Cliente ]

        /// <summary>
        /// Código do Grupo de Atuação
        /// </summary>
        public Int32 CodigoGrupoAtuacao { get; set; }

        /// <summary>
        /// Descrição do Grupo de Atuação
        /// </summary>
        public String DescricaoGrupoAtuacao { get; set; }

        /// <summary>
        /// Código do Ramo de Atividade
        /// </summary>
        public Int32 CodigoRamoAtividade { get; set; }

        /// <summary>
        /// Descrição do Ramo de Atividade
        /// </summary>
        public String DescricaoRamoAtividade { get; set; }

        /// <summary>
        /// Razão Social ou Nome Completo
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Nome Fantasia
        /// </summary>
        public String NomeFantasia { get; set; }

        /// <summary>
        /// Data de Fundação ou Data de Nascimento
        /// </summary>
        public DateTime DataFundacao { get; set; }

        /// <summary>
        /// Coleção de Proprietários
        /// </summary>
        public ICollection<Proprietario> Proprietarios { get; set; }

        /// <summary>
        /// Nome para Contato
        /// </summary>
        public String NomeContato { get; set; }

        /// <summary>
        /// DDD do telefone 1
        /// </summary>
        public String DDDTelefone1 { get; set; }

        /// <summary>
        /// Número do telefone 1
        /// </summary>
        public String NumeroTelefone1 { get; set; }

        /// <summary>
        /// ramal do telefone 1
        /// </summary>
        public String RamalTelefone1 { get; set; }

        /// <summary>
        /// DDD do telefone 2
        /// </summary>
        public String DDDTelefone2 { get; set; }

        /// <summary>
        /// Número do telefone 2
        /// </summary>
        public String NumeroTelefone2 { get; set; }

        /// <summary>
        /// Ramal do telefone 2
        /// </summary>
        public String RamalTelefone2 { get; set; }

        /// <summary>
        /// DDD do Fax
        /// </summary>
        public String DDDFax { get; set; }

        /// <summary>
        /// Número do Fax
        /// </summary>
        public String NumeroFax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Site
        /// </summary>
        public String Site { get; set; }

        /// <summary>
        /// Endereço Comercial
        /// </summary>
        public Endereco EnderecoComercial { get; set; }

        /// <summary>
        /// Endereço Correspondência
        /// </summary>
        public Endereco EnderecoCorrespondencia { get; set; }

        /// <summary>
        /// Endereço de Instalação
        /// </summary>
        public Endereco EnderecoInstalacao { get; set; }

        /// <summary>
        /// Dia inicial de funcionamento
        /// </summary>
        public String DiaFuncionamentoInicio { get; set; }

        /// <summary>
        /// Dia final de funcionamento
        /// </summary>
        public String DiaFuncionamentoFinal { get; set; }

        /// <summary>
        /// Horário inicial de funcionamento
        /// </summary>
        public String HorarioFuncionamentoInicio { get; set; }

        /// <summary>
        /// Horário final de funcionamento
        /// </summary>
        public String HorarioFuncionamentoFinal { get; set; }
        
        /// <summary>
        /// Estabelecimento localizado em Shopping
        /// </summary>
        public Boolean EstabelecimentoLocalizadoShoppingChecked { get; set; }

        /// <summary>
        /// Endereço comercial igual ao de correspondência
        /// </summary>
        public Boolean EnderecoComercialIgualCorrespondenciaChecked { get; set; }

        #endregion

        #region [ Dados Bancarios ]

        /// <summary>
        /// Código do Banco
        /// </summary>
        public String CodigoBanco { get; set; }

        /// <summary>
        /// Descrição do Banco
        /// </summary>
        public String DescricaoBanco { get; set; }

        /// <summary>
        /// Código da Agência
        /// </summary>
        public String CodigoAgencia { get; set; }

        /// <summary>
        /// Descrição da Agência
        /// </summary>
        public String DescricaoAgencia { get; set; }

        /// <summary>
        /// Número da Conta Corrente
        /// </summary>
        public String ContaCorrente { get; set; }

        #endregion

        #region [ Confirmacao Dados ]

        public Boolean ConcordoContratoAdesao { get; set; }

        #endregion

        #region [ Escolha Equipamento ]

        /// <summary>
        /// Taxa de Ativação
        /// </summary>
        public Decimal TaxaAtivacao { get; set; }

        /// <summary>
        /// Dados do Comprovante de Venda
        /// </summary>
        public ComprovanteVenda Comprovante { get; set; }

        #endregion

        #region [ Impossível Continuar ]

        public String CodigoErro { get; set; }

        #endregion
    }
}
