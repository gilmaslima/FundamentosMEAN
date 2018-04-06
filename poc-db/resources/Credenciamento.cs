using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using Redecard.PN.Credenciamento.Sharepoint.WFProdutos;

namespace Redecard.PN.Credenciamento.Sharepoint.Modelo
{
    /// <summary>
    /// Classe de Credenciamento utilizada para persistir dados entre os passos do Credenciamento
    /// </summary>
    [Serializable]
    public class Credenciamento
    {
        #region [ Dados Iniciais ]

        public Boolean PermitePatamar { get; set; }

        /// <summary>
        /// Indica se a proposta é uma duplicação
        /// </summary>
        public Boolean Duplicacao { get; set; }

        /// <summary>
        /// Indica se a proposta é um Recredenciamento
        /// </summary>
        public Boolean Recredenciamento { get; set; }

        /// <summary>
        /// Proposta recuperada do GE
        /// </summary>
        public Boolean RecuperadaGE { get; set; }

        /// <summary>
        /// Fase atual do Credenciamento
        /// </summary>
        public Int32? Fase { get; set; }

        /// <summary>
        /// Código de retorno do serviço do serasa
        /// </summary>
        public String RetornoSerasa { get; set; }

        /// <summary>
        /// Tipo de Comercialização (Número)
        /// </summary>
        public String TipoComercializacao { get; set; }

        /// <summary>
        /// Tipo de Comercialização (Descrição)
        /// </summary>
        public String TipoComercializacaoDescricao { get; set; }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        public String TipoPessoa { get; set; }

        /// <summary>
        /// CNPJ
        /// </summary>
        public String CNPJ { get; set; }

        /// <summary>
        /// CPF
        /// </summary>
        public String CPF { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        public String CEP { get; set; }

        /// <summary>
        /// Código do Canal
        /// </summary>
        public Int32 Canal { get; set; }

        /// <summary>
        /// Código da Celula
        /// </summary>
        public Int32 Celula { get; set; }

        /// <summary>
        /// Quantidade de PVs Ativos
        /// </summary>
        public Int32? QtdePVsAtivos { get; set; }

        /// <summary>
        /// Código tipo Movimento
        /// </summary>
        public Char CodTipoMovimento { get; set; }

        #region [ Tipo Estabelecimento ]

        /// <summary>
        /// Código do Tipo de Estabelecimento
        /// </summary>
        public Int32? CodTipoEstabelecimento { get; set; }

        /// <summary>
        /// Código do Tipo de Estabelecimento Matriz
        /// </summary>
        public Int32? CodTipoEstabMatriz { get; set; }

        /// <summary>
        /// Número do PDV
        /// </summary>
        public Int32? NumPdv { get; set; }

        /// <summary>
        /// Número do PDV Matriz
        /// </summary>
        public Int32? NumPdvMatriz { get; set; }

        #endregion

        #region [ Dados Canal ]

        /// <summary>
        /// Indíce exige participação integral
        /// </summary>
        public Char? ExigeParticipacaoIntegral { get; set; }

        #endregion

        #endregion

        #region [ Dados do Cliente ]

        /// <summary>
        /// Nome Completo
        /// </summary>
        public String NomeCompleto { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Código do Grupo do Ramo
        /// </summary>
        public Int32 GrupoRamo { get; set; }

        /// <summary>
        /// Código do Ramo de Atividade
        /// </summary>
        public Int32 RamoAtividade { get; set; }

        /// <summary>
        /// Número de Sequência
        /// </summary>
        public Int32? NumSequencia { get; set; }

        /// <summary>
        /// Razão Social
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Data de Fundação
        /// </summary>
        public DateTime DataFundacao { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        public String PessoaContato { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public String NomeEmail { get; set; }

        /// <summary>
        /// Site
        /// </summary>
        public String NomeHomePage { get; set; }

        /// <summary>
        /// Telefone 1 DDD
        /// </summary>
        public String NumDDD1 { get; set; }

        /// <summary>
        /// Telefone 1 número
        /// </summary>
        public Int32? NumTelefone1 { get; set; }

        /// <summary>
        /// Telefone 1 Ramal
        /// </summary>
        public Int32? Ramal1 { get; set; }

        /// <summary>
        /// Faz DDD
        /// </summary>
        public String NumDDDFax { get; set; }

        /// <summary>
        /// Fax Número
        /// </summary>
        public Int32? NumTelefoneFax { get; set; }

        /// <summary>
        /// Telefone 2 DDD
        /// </summary>
        public String NumDDD2 { get; set; }

        /// <summary>
        /// Telefone 2 Número
        /// </summary>
        public Int32? NumTelefone2 { get; set; }

        /// <summary>
        /// Telefone 2 Ramal
        /// </summary>
        public Int32? Ramal2 { get; set; }

        /// <summary>
        /// Número do EDV
        /// </summary>
        public Int32? EDV { get; set; }

        /// <summary>
        /// Número do CNAE
        /// </summary>
        public String CNAE { get; set; }

        /// <summary>
        /// Indicador de Extrato por Email
        /// </summary>
        public Char? IndExtratoEmail { get; set; }

        /// <summary>
        /// Proprietários
        /// </summary>
        public List<Proprietario> Proprietarios { get; set; }

        #region [ Dados de Ramos de Atividade ]
        
        /// <summary>
        /// Permite IATA
        /// </summary>
        public Char? PermIATA { get; set; }

        /// <summary>
        /// Indicador Ramo Target
        /// </summary>
        public Char? IndRamoTarget { get; set; }

        /// <summary>
        /// Indicador Permite Maquineta
        /// </summary>
        public Char? IndPermMaquineta { get; set; }

        /// <summary>
        /// Indicador Permite Duplicidade CNPJ
        /// </summary>
        public Char? IndPermDuplCNPJ { get; set; }

        /// <summary>
        /// Indicador Permite Duplicidade CPF
        /// </summary>
        public Char? IndPermDuplCPF { get; set; }

        /// <summary>
        /// Indicador Tipo de Moeda
        /// </summary>
        public Char? IndTipoMoeda { get; set; }

        /// <summary>
        /// Indicador Marketing Direto
        /// </summary>
        public Char? IndMarketingDireto { get; set; }

        /// <summary>
        /// Indicador Permite Cent. Mesmo Ramo
        /// </summary>
        public Char? IndPermCentMsmRamo { get; set; }

        #endregion

        #endregion

        #region [ Dados de Negócio ]

        /// <summary>
        /// Refazer negociação
        /// </summary>
        public Boolean RefazerNegociacao { get; set; }

        /// <summary>
        /// Tipo de Equipamento
        /// </summary>
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Valor do Aluguel do Equipamento utilizado para gravar no bd
        /// </summary>
        public Double ValorAluguel { get; set; }

        /// <summary>
        /// Valor do Aluguel utilizado na tela
        /// </summary>
        public Double ValorAluguelTela { get; set; }

        /// <summary>
        /// Código do Cenário
        /// </summary>
        public Int32? CodCenario { get; set; }

        /// <summary>
        /// Taxa de Adesão
        /// </summary>
        public Double? TaxaAdesao { get; set; }

        /// <summary>
        /// Código da Campanha
        /// </summary>
        public String CodCampanha { get; set; }        

        #region  [ Dados do Equipamento ]

        public Char? CodMascaraTnms { get; set; }
        public String CodTipoEquipamento { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public Char? IndFaturavel { get; set; }
        public Char? IndGeraFCT { get; set; }
        public Char? IndGeraOS { get; set; }
        public Char? IndPermAltEndereco { get; set; }
        public Char? IndTecnologiaCompartilhada { get; set; }
        public Char? IndVendaDigitadaReceptivo { get; set; }
        public Char? IndVendaTelemarketing { get; set; }
        public String NomeTipoEquipamento { get; set; }
        public Char? SituacaoTipoEquipamento { get; set; }
        public String TimeStampCanal { get; set; }
        public String UsuarioUltimaAtualizacao { get; set; }
        public Double? ValorDefaultAluguel { get; set; }
        public Double? ValorMinimoAluguel { get; set; }
        public Double? ValorMáximoAluguel { get; set; }

        #endregion

        #region [ Dados Produtos ]

        /// <summary>
        /// Lista de Produtos Crédito
        /// </summary>
        public List<ProdutosListaDadosProdutosPorRamoCanal> ProdutosCredito { get; set; }

        /// <summary>
        /// Lista de Produtos Débito
        /// </summary>
        public List<ProdutosListaDadosProdutosPorRamoCanal> ProdutosDebito { get; set; }

        /// <summary>
        /// Lista de Produtos Construcard
        /// </summary>
        public List<ProdutosListaDadosProdutosPorRamoCanal> ProdutosConstrucard { get; set; }

        /// <summary>
        /// Lista de Patamares
        /// </summary>
        public List<Patamar> Patamares { get; set; }

        #endregion

        #endregion

        #region [ Dados Endereço ]

        /// <summary>
        /// Dia de Início de Funcionamento
        /// </summary>
        public Int32 DiaInicioFuncionamento { get; set; }

        /// <summary>
        /// Dia Final de Funcionamento
        /// </summary>
        public Int32 DiaFimFuncionamento { get; set; }

        /// <summary>
        /// Hora Início de Funcionamento
        /// </summary>
        public Int32 HoraInicioFuncionamento { get; set; }

        /// <summary>
        /// Hora Final de Funcionamento
        /// </summary>
        public Int32 HoraFimFuncionamento { get; set; }

        /// <summary>
        /// Dia de Início de Instalação
        /// </summary>
        public Int32 DiaInicioInstalacao { get; set; }

        /// <summary>
        /// Dia Final de Instalação
        /// </summary>
        public Int32 DiaFimInstalacao { get; set; }

        /// <summary>
        /// Hora Início de Instalação
        /// </summary>
        public Int32 HoraInicioInstalacao { get; set; }

        /// <summary>
        /// Hora Final de Instalação
        /// </summary>
        public Int32 HoraFimInstalacao { get; set; }

        /// <summary>
        /// Nome Contado para Instalação
        /// </summary>
        public String NomeContatoInstalacao { get; set; }

        /// <summary>
        /// Número DDD para Instalação
        /// </summary>
        public String NumDDDInstalacao { get; set; }

        /// <summary>
        /// Telefone para Instalação
        /// </summary>
        public Int32 NumTelefoneInstalacao { get; set; }

        /// <summary>
        /// Ramal para Instalação
        /// </summary>
        public Int32 RamalInstalacao { get; set; }

        /// <summary>
        /// Endereço Comercial
        /// </summary>
        public Endereco EnderecoComercial = new Endereco();

        /// <summary>
        /// Endereço de Correspondencia
        /// </summary>
        public Endereco EnderecoCorrespondencia = new Endereco();

        /// <summary>
        /// Endereço de Instalção
        /// </summary>
        public Endereco EnderecoInstalacao = new Endereco();

        /// <summary>
        /// Endereço de Instalção igual ao comercial
        /// </summary>
        public Char EndInstIgualComer { get; set; }

        /// <summary>
        /// Observação
        /// </summary>
        public String Observacao { get; set; }

        /// <summary>
        /// Indica região loja
        /// </summary>
        public Char IndRegiaoLoja { get; set; }

        #region [ Dados da Filial ]

        /// <summary>
        /// Código da Filial
        /// </summary>
        public Int32? CodFilial { get; set; }


        /// <summary>
        /// Nome da Filial
        /// </summary>
        public String NomeFilial { get; set; }

        /// <summary>
        /// Código da Zona de Venda
        /// </summary>
        public Int32? CodZonaVenda { get; set; }

        /// <summary>
        /// Nome da Zona de Venda
        /// </summary>
        public String NomeZonaVenda { get; set; }

        #endregion

        #endregion

        #region [ Dados Operacionais ]

        /// <summary>
        /// Código Gerência
        /// </summary>
        public Char? CodGerencia { get; set; }

        /// <summary>
        /// Código Carteira
        /// </summary>
        public Int32? CodCarteira { get; set; }

        /// <summary>
        /// Horário de Funcionamento
        /// </summary>
        public Int32? HorarioFuncionamento { get; set; }

        /// <summary>
        /// Nome na Fatura
        /// </summary>
        public String NomeFatura { get; set; }

        /// <summary>
        /// Grupo Comercial
        /// </summary>
        public Int32? GrupoComercial { get; set; }

        /// <summary>
        /// Grupo Gerencial
        /// </summary>
        public Int32? GrupoGerencial { get; set; }

        /// <summary>
        /// Local de Pagamento
        /// </summary>
        public Int32? LocalPagamento { get; set; }

        /// <summary>
        /// Número da Centralizadora
        /// </summary>
        public Int32? Centralizadora { get; set; }

        /// <summary>
        /// Data de Cadastro da Proposta
        /// </summary>
        public DateTime DataCadastroProposta { get; set; }

        /// <summary>
        /// CPF do Vendedor
        /// </summary>
        public Int64? CPFVendedor { get; set; }

        #endregion

        #region [ Dados Bancários ]

        /// <summary>
        /// Código do Banco Crédito
        /// </summary>
        public Int32 CodBancoCredito { get; set; }

        /// <summary>
        /// Nome Banco Crédito
        /// </summary>
        public String NomeBancoCredito { get; set; }

        /// <summary>
        /// Agência Crédito
        /// </summary>
        public Int32 AgenciaCredito { get; set; }

        /// <summary>
        /// Conta Crédito
        /// </summary>
        public String ContaCredito { get; set; }

        /// <summary>
        /// Código do Banco Débito
        /// </summary>
        public Int32 CodBancoDebito { get; set; }

        /// <summary>
        /// Nome Banco Débito
        /// </summary>
        public String NomeBancoDebito { get; set; }

        /// <summary>
        /// Agência Débito
        /// </summary>
        public Int32 AgenciaDebito { get; set; }

        /// <summary>
        /// Conta Débito
        /// </summary>
        public String ContaDebito { get; set; }

        /// <summary>
        /// Código do Banco Construcard
        /// </summary>
        public Int32 CodBancoConstrucard { get; set; }

        /// <summary>
        /// Nome Banco Construcard
        /// </summary>
        public String NomeBancoConstrucard { get; set; }

        /// <summary>
        /// Agência Construcard
        /// </summary>
        public Int32 AgenciaConstrucard { get; set; }

        /// <summary>
        /// Conta Construcard
        /// </summary>
        public String ContaConstrucard { get; set; }

        #endregion

        #region [ Escolha Tecnologia ]

        /// <summary>
        /// Quantidade de terminais solicitados
        /// </summary>
        public Int32 QtdeTerminaisSolicitados { get; set; }

        /// <summary>
        /// Código Marca PDV
        /// </summary>
        public String CodMarcaPDV { get; set; }

        /// <summary>
        /// Nome da Marca do PDV
        /// </summary>
        public String NomeMarcaPDV { get; set; }

        /// <summary>
        /// Código do Software TEF
        /// </summary>
        public String CodSoftwareTEF { get; set; }

        /// <summary>
        /// Nome do Software TEF
        /// </summary>
        public String NomeSoftwareTEF { get; set; }

        /// <summary>
        /// Número Renpac
        /// </summary>
        public Int32 NroRenpac { get; set; }

        /// <summary>
        /// Evento
        /// </summary>
        public String CodEvento { get; set; }

        /// <summary>
        /// Ação Comercial
        /// </summary>
        public Int32? AcaoComercial { get; set; }

        /// <summary>
        /// Indicador Venda Digitada
        /// </summary>
        public Char? IndVendaDigitada { get; set; }

        #endregion

        #region [ Contratacao Serviços ]

        public List<Servico> Servicos = new List<Servico>();

        public List<ProdutosListaDadosProdutosVanPorRamo> ProdutosVan { get; set; }

        #endregion

        #region [ Confirmação de Dados ]

        public Int32? CodCasoOcorrencia { get; set; }
        public DateTime DataRequisicaoOcorrencia { get; set; }
        public Int32? NumRequisicaoOcorrencia { get; set; }
        public Int32? NumSolicitacao { get; set; }
        public String DescricaoScript { get; set; }

        public String DescricaoCanal { get; set; }
        public String DescricaoCelula { get; set; }
        public String DescricaoRamoAtividade { get; set; }

        public DateTime? DataScoreRisco { get; set; }

        #endregion
    }
}
