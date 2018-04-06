#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [30/04/2012] – [André Rentes] – [Criação]
*/
#endregion
using Redecard.PN.DadosCadastrais.Modelo;
using Redecard.PN.DadosCadastrais.Modelo.Mensagens;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Interface de Entidade
    /// </summary>
    [ServiceContract]
    public interface IEntidadeServico
    {

        /// <summary>
        /// Validar se as Entidades existem no PN, caso não existam, incluir as mesmas na base
        /// </summary>
        /// <param name="entidades">Entidades separas por ';' para validação na base do PN
        /// <para>Cada elemento do conjunto de Entidades é separado por ';'</para>
        /// <para>Cada elemento de Entidades possui o Número do PV e o Nome da Entidade separados por ','</para>
        /// </param>
        /// <returns>
        /// <para>0 - Sucesso</para>
        /// <para>404 - Entidades não informadas</para>
        /// <para>425 - Erro de excessão no processamento das Entidades (rollback de todas as entidades inseridas)</para>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ValidarEntidadePn(String entidades);

        /// <summary>
        /// Validar se as Entidades existem no PN, caso não existam, consulta os dados no GE e inclui as mesmas na base
        /// </summary>
        /// <param name="entidades">Número das entidades separados por '|' para validação na base do PN
        /// <para>Cada elemento do conjunto de Entidades é separado por '|'</para>
        /// </param>
        /// <returns>
        /// <para>0 - Sucesso</para>
        /// <para>404 - Entidades não informadas</para>
        /// <para>410 - Erro na procedure ao atualizar os dados de uma Entidade (rollback do processamento da entidade atual)</para>
        /// <para>411 - Erro na procedure ao inserir os dados de uma Entidade (rollback do processamento da entidade atual)</para>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ValidarPvsExistentes(String entidades);

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="codigoRetornoIS"></param>
        /// <returns></returns>
        [FaultContract(typeof(GeneralFault))]
        [OperationContract(Name = "ConsultarDadosIS")]
        List<Servicos.Entidade> Consultar(Int32? codigo, out Int32 codigoRetornoIS);

        /// <summary>
        /// Consulta as perguntas aleatórias disponíveis para o estabelecimento informado
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Pergunta> ConsultarPerguntasAleatorias(Int32 numeroPV);

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="id">Id da entidade</param>
        /// <returns>Entidade preenchida</returns>
        [OperationContract(Name = "Consultar")]
        [FaultContract(typeof(GeneralFault))]
        List<Entidade> Consultar(Int32? codigo, Int32? codigoGrupoEntidade, out Int32 codigoRetornoIS, out Int32 codigoRetornoGE);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirGrupoFornecedor(Int32 codigoGrupo, String descricao, String funcional);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AlterarGrupoFornecedor(Int32 codigoGrupo, String descricao, String funcional);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoGrupo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 RemoverGrupoFornecedor(Int32 codigoGrupo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoRetorno"></param>
        /// <param name="codigoGrupoFornecedor"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.GrupoFornecedor> ConsultarGrupoFornecedor(out Int32 codigoRetorno, Int32? codigoGrupoFornecedor);

        /// <summary>
        /// Consulta grupo da entidade
        /// </summary>
        /// <param name="id">Id do grupo da entidade</param>
        /// <returns>Grupo da Entidade preenchido</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<GrupoEntidade> ConsultarGrupo(Int32? codigo, out Int32 codigoRetorno);

        /// <summary>
        /// Altera um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AtualizarGrupo(Servicos.GrupoEntidade grupoEntidade);

        /// <summary>
        /// Consulta grupo da entidade
        /// </summary>
        /// <param name="codigo">Id do grupo da entidade</param>
        /// <param name="useCache">Identificador de utilização dos dados armazenados em Cache</param>
        /// <returns>Grupo da Entidade preenchido</returns>
        [OperationContract(Name = "ConsultarGrupoCache")]
        [FaultContract(typeof(GeneralFault))]
        List<GrupoEntidade> ConsultarGrupo(Int32? codigo, Boolean useCache, out Int32 codigoRetorno);

        /// <summary>
        /// Excljui um Grupo de Entidade
        /// </summary>
        /// <param name="codigo">Código do Grupo de Entidade a ser excluido</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirGrupo(Int32 codigo);

        /// <summary>
        /// Consulta todos os Grupos de Entidade (incluindo inativos)
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.GrupoEntidade> ConsultarTodosGrupos(out Int32 codigoRetorno);

        /// <summary>
        /// Altera o Status de um Grupo de Entidade
        /// </summary>
        /// <param name="grupoEntidade">Objeto Grupo de Entidade atualizado</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AtualizarStatusGrupo(Servicos.GrupoEntidade grupoEntidade);

        /// <summary>
        /// Consultar endereços do Estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código do Grupo Entidade</param>
        /// <param name="tipoEndereco">Tipo de endereço buscado. E - Estabelecimento; C - Correspondência</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista de estabelecimentos de acordo com o Tipo de Endereço</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Endereco> ConsultarEndereco(Int32 codigoEntidade, String tipoEndereco, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar dados do PV na base de dados do GE
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.Entidade ConsultarDadosPV(out Int32 codigoRetorno, Int32 codigoEntidade);

        #region Consulta PVs por email/cpf
        /// <summary>
        /// Consulta pv por CPF
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="qtdEmailsPorCpf"></param>
        /// <param name="pvsSelecionados"></param>
        /// <param name="filtroGenerico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarPvPorCpf(Int64 cpf, out int codigoRetorno, out int qtdEmailsPorCpf, string pvsSelecionados = null, string filtroGenerico = null);

        /// <summary>
        /// Consulta PV por cpf com paginação
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="totalRows"></param>
        /// <param name="pagina"></param>
        /// <param name="qtdRegistros"></param>
        /// <param name="qtdEmailsPorCpf"></param>
        /// <param name="pvsSelecionados"></param>
        /// <param name="filtroGenerico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarPvPorCpfComPaginacao(Int64 cpf, out int codigoRetorno, out int totalRows, int pagina, int qtdRegistros, out int qtdEmailsPorCpf, bool retornarEmail, string pvsSelecionados = null, string filtroGenerico = null);

        /// <summary>
        /// Consulta PV por e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="pvSenhasIguais"></param>
        /// <param name="pvsSelecionados"></param>
        /// <param name="filtroGenerico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarPvPorEmail(string email, out int codigoRetorno, out bool? pvSenhasIguais, string pvsSelecionados = null, string filtroGenerico = null);

        /// <summary>
        /// Consulta PV por e-mail com paginação
        /// </summary>
        /// <param name="email"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="totalRows"></param>
        /// <param name="pagina"></param>
        /// <param name="qtdRegistros"></param>
        /// <param name="pvsSelecionados"></param>
        /// <param name="filtroGenerico"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarPvPorEmailComPaginacao(string email, out int codigoRetorno, out int totalRows, int pagina, int qtdRegistros, string pvsSelecionados = null, string filtroGenerico = null);

        #endregion

        #region Passo 1 primeiro acesso

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.Entidade[] ConsultarPvGePorCpfCnpj(out int codigoRetorno, long? cnpj, long? cpf);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        bool PvsRelacionadosSaoFiliais(out int codigoRetorno, long cnpj);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarEntidadeGeCriarNoPN(out int codigoRetorno, long? cpf, long? cnpj);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Dictionary<int, string> ConsultarEmailPVs(out int codigoRetorno, int[] pvs);

        #endregion

        #region Passo 2 primeiro acesso

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoResponse ValidarConfirmacaoPositivaPrimeiroAcesso(string emailUsuarioAlteracao,
                                                                                           Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoRequest request, 
                                                                                           out EntidadeServicoModel[] entidadesPossuemUsuario,
                                                                                           out EntidadeServicoModel[] entidadesPossuemMaster);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.Entidade> IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(out int codigoRetorno, int[] codigoEntidades, string emailUsuarioAlteracao);

        #endregion

        /// <summary>
        /// Consultar Tipo Pessoa do PV na base de dados do GE
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Char ConsultarTipoPessoaPV(out Int32 codigoRetorno, Int32 codigoEntidade);

        /// <summary>
        /// Consultar filiais do Estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="tipoAssociacao">ID do Tipo de Associação</param>
        /// <returns>Listagem de filiais do estabelecimento</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Filial> ConsultarFiliais(Int32 codigoEntidade, Int32 tipoAssociacao, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar filiais do Estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de filiais do estabelecimento</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Filial> ConsultarFiliaisEntidade(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Retorna a relação de entidade por CNPJ informado
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoAssociacao">Tipo de associação</param>
        /// <param name="CNPJ">CNPJ</param>
        /// <param name="codigoRetorno">Código de Retorno da procedure</param>
        /// <returns>List<Entidade></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Entidade> ConsultarPorCNPJ(Int32 codigoEntidade, Int32 tipoAssociacao, Double CNPJ, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta entidades associadas a um usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.Entidade> ConsultarPorUsuario(Int32 codigoIdUsuario, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta entidades associadas a um usuário por e-mail
        /// </summary>
        /// <param name="email">E-mail usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.Entidade> ConsultarPorEmail(String email, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta entidades associadas a um usuário por e-mail e senha criptografada Hash
        /// </summary>
        /// <param name="email">E-mail usuário</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.Entidade> ConsultarPorEmailSenhaHash(String email, String senha, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta entidades associadas a um usuário por CPF
        /// </summary>
        /// <param name="cpf">CPF do usuário</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.Entidade> ConsultarPorCpf(Int64 cpf, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta as seguintes informações do pv:
        /// tipo de estabelecimento
        /// centralizador/centralizado
        /// consignado/consignatário
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de Retorno da procedure</param>
        /// <returns>List<Modelo.Entidade></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.InformacaoPV> ConsultarInformacoesPV(Int32 codigoEntidade, out Int32 codigoRetorno);


        /// <summary>
        /// Consulta todas as propriedades de um estabelecimento no banco do GE (spge6002) e
        /// </summary>
        /// <returns>Entidade</returns>
        [OperationContract(Name = "ConsultarDadosCompletos")]
        [FaultContract(typeof(GeneralFault))]
        Servicos.Entidade Consultar(Int32 codigoEntidade, Boolean validarSenha, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta a entidade pelo número do terminal
        /// </summary>
        /// <param name="numeroTerminal">Número do terminal</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Entidade</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.Entidade ConsultarPorTerminal(String numeroTerminal, out Int32 codigoRetorno);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DadosGerais ConsultarDadosGerais(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Tipo de Tecnologia do Estabelecimento.
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns>
        /// 25 ou 26 - Komerci
        /// 20 - Normal
        /// 0 - Erro
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarTecnologiaEstabelecimento(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Atualiza os IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade para atualização</param>
        /// <param name="dados">Dados gerais do URLBack para atualização</param>
        /// <param name="usuarioAlteracao">Usuário responsável pela atualização</param>
        /// <returns>Código de Erro da procedure</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarURLBack(Int32 codigoEntidade, Servicos.URLBack dados, String usuarioAlteracao);

        /// <summary>
        /// Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dados">Modelo de Dados Gerais com informações atualizadas</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarDadosGerais(Int32 codigoEntidade, Servicos.DadosGerais dados);

        /// <summary>
        /// Consulta os dados de IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.URLBack ConsultarURLBack(Servicos.Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os dados bancários de Crédito ou Débito
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDado">Tipo de dados a ser consultado.
        /// c - Crédito
        /// d - Débito
        /// </param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DadosBancarios> ConsultarDadosBancarios(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta as tarifas de Transmissão
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDados">Tipo de dados a ser consultado.
        /// CR - Crédito
        /// DB - Débito
        /// </param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.Tarifas ConsultarTarifas(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno);

        /// <summary>
        /// Valida a Alteração dos dados do Domicílio Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dadosBancarios">Dados do Domicílio Bancário</param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Sim, há confirmação eletrônica
        /// N - Não, não há confirmação eletrônica
        /// </param>
        /// <param name="aguardarDocumento">Indica se ocorre espera de recebimento de Documento
        /// S - Sim. Não há Confirmação Eletrônica
        /// N - Não. Há Confirmação Eletrônica
        /// </param>
        /// <param name="codigoRetorno">Código de erro retornado pela procedure</param>
        /// <returns>
        /// True - Alteração Válida
        /// False - Alteração inválida
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ValidarAlteracaoDomicilioBancario(Int32 codigoEntidade, Servicos.DadosDomiciolioBancario dadosBancarios, String confirmacaoEletronica, String aguardarDocumento, out Int32 codigoRetorno);

        /// <summary>
        /// Grava as alterações do Domicílio Bancário
        /// </summary>
        /// <param name="numeroRequisicao">Número de Requisição gerado pela proc SPWM0123</param>
        /// <param name="numeroSolicitacao">Número de Solicitação gerado pela proc SPWM0123</param>
        /// <param name="tipoOperacao">Tipo da operação bancária: CR - Crédito; DB - Débito; CDC - Construcard</param>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="agencia">Agência</param>
        /// <param name="conta">Número da conta</param>
        /// <param name="aguardaDocumento">Indica necessidade do envio de documento
        /// S - Sem confirmação eletrônica
        /// N - Com confirmação eletrônica
        /// </param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Com confirmação eletrônica
        /// N - Sem confirmação eletrônica
        /// </param>
        /// <param name="canal">Canal de alteração</param>
        /// <param name="celula">Célula de alteração</param>
        /// <param name="tipoTransacao">Tipo da transação</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirAlteracaoDomicilioBancario(Int32 numeroRequisicao, Int32 numeroSolicitacao, String tipoOperacao, String codigoBanco, String agencia,
            String conta, String aguardaDocumento, String confirmacaoEletronica, String canal, String celula, String tipoTransacao);

        /// <summary>
        /// Grava uma nova solicitação de alteração de domicílio bancário
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="grupoSolicitacao">Grupo de Solicitação</param>
        /// <param name="cnpj">CNPJ</param>
        /// <param name="numeroRequisicao">Retorno com o número da Requisição</param>
        /// <param name="numeroSolicitacao">Retorno com o número da Solicitação</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirSolicitacaoAlteracaoDomicilioBancario(Int32 codigoEntidade, String grupoSolicitacao, String cnpj,
            out Int32 numeroRequisicao, out Int32 numeroSolicitacao);

        /// <summary>
        /// Inserir Grupo Entidade
        /// </summary>
        /// <param name="grupoEntidade"></param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void InserirGrupo(Servicos.GrupoEntidade grupoEntidade);

        /// <summary>
        /// Consulta os dados de Domicílios Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns>Lista de Domicílios Bancários</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DadosDomiciolioBancario> ConsultarDadosDomiciliosBancarios(Int32 codigoEntidade, out Boolean permissaoAlteracao, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar a lista de agências de um banco
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Agencia> ConsultarAgencias(Int32 codigoAgencia, Int32 codigoBanco, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta bancos cadastradados na base DR
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Banco> ConsultarBancos();

        /// <summary>
        /// Consulta bancos cadastradados na base DR para confirmação positiva
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Banco> ConsultarBancosConfirmacaoPositiva();

        /// <summary>
        /// Consulta os bancos com Confirmação Eletrônica disponível
        /// </summary>
        /// <returns>Lista de bancos com confirmação eletrônica</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Banco> ConsultarBancosConfirmacaoEletronica();

        /// <summary>
        /// Consulta as alterações de Domicílio Bancário solicitadas pela Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Última Solicitações de alteração de Domicílio Bancário</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DadosDomiciolioBancario> ConsultarDomiciliosAlterados(Int32 codigoEntidade);

        /// <summary>
        /// Lista das alterações de Domicílio Bancário solicitadas pela Entidade em status Pendente
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações de alteração de Domicílio Bancário</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DadosDomiciolioBancario> ListarDomiciliosAlterados(Int32 codigoEntidade);

        /// <summary>
        /// Cancelar a alteração de Domicílio Bancária solicitada
        /// </summary>
        /// <param name="numeroSolicitacao">Código da Solicitação de Alteração</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de retorno de erro da procedure</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 CancelarAlteracao(Int32 numeroSolicitacao, Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Inseri Log sobre entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <param name="tipo">Tipo de Log</param>
        /// <param name="valor">Valor do Log</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirLog(Servicos.Entidade entidade, String tipo, String valor);

        /// <summary>
        /// Recupera todas entidades a serem processadas pelo ISRobô
        /// --------------------------------------------------------------
        /// Histórico de Alteração
        ///   - 06/08/2014: Alteração do nome do método exposto
        ///     Autor: Denis Francisco Gracias Lucia - REDECARD 
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="qtdMaximaEntidades">Quantidade máxima de registros de Entidades a serem retornadas</param>
        [OperationContract(Name = "ListarEntidadesProcessamento")]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Entidade> Listar(out Int32 codigoRetorno, Int32 qtdMaximaEntidades);

        /// <summary>
        /// Alterar e-mail da entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AlterarEmail(Servicos.Entidade entidade);

        /// <summary>
        /// Consulta os Estados na base do SQL Server
        /// </summary>
        /// <returns>Lista de Estados na base SQL Server</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Estados> ConsultarEstados();


        /// <summary>
        /// Inserir Entidade
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirEntidade(Servicos.Entidade entidade);

        /// <summary>
        /// Alterar Entidade
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AlterarEntidade(Servicos.Entidade entidade);

        /// <summary>
        /// Excluir Entidade
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirEntidade(Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Consuta dados entidade EC
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        EntidadeEC ConsultarEntidadeEC(Int32? codigo, out Int32 codigoRetorno);

        /// <summary>
        /// Atualiza Entidade da base EC
        /// </summary>
        /// <param name="entidadeEC"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AlterarEntidadeEC(EntidadeEC entidadeEC);

        /// <summary>
        /// Inclui EntidadeEC
        /// </summary>
        /// <param name="entidadeEC"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirEntidadeEC(EntidadeEC entidadeEC);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirEntidadeGE(Servicos.Entidade entidade);

        /// <summary>
        /// Consulta o Fornecedor Distribuidor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Modelo.DistribuidorFornecedor</returns>
        [OperationContract(Name = "ConsultarDistribuidorFornecedor")]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DistribuidorFornecedor> ConsultarDistribuidorFornecedor(Int32 codigoEntidade, Int32 codigoGrupoEntidade, out Int32 codigoRetorno);

        /// Consulta o Fornecedor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Servicos.DistribuidorFornecedor</returns>
        [OperationContract(Name = "ConsultarFornecedor")]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DistribuidorFornecedor> ConsultarFornecedor(Int32? codigoEntidade, Int32? codigoGrupoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta o Fornecedor Distribuidor da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoRetorno">Código do Retorno</param>
        /// <returns>Servicos.DistribuidorFornecedor</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.DistribuidorFornecedor> ConsultarFornecedorDistribuidor(Int32 codigoEntidade, Int32 codigoGrupoEntidade, out Int32 codigoRetorno);


        /// <summary>
        /// Consulta distribuidores
        /// </summary>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeDistribuidor">Código Grupo Entidade Distribuidor</param>
        /// <param name="codigoEntidadeDistribuidor">Código Entidade Distribuidor</param>
        /// <param name="codigoGrupoFornecedor">Código Grupo Fornecedor</param>
        /// <param name="codigoRetorno">Código retorno</param>
        /// <returns>Lista de distribuidores</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Distribuidor> ConsultarDistribuidores(Int32? codigoGrupoEntidadeFornecedor, Int32? codigoEntidadeFornecedor,
            Int32? codigoGrupoEntidadeDistribuidor, Int32? codigoEntidadeDistribuidor, Int32? codigoGrupoFornecedor, out Int32 codigoRetorno);

        /// <summary>
        /// Exclusão de Distribuidor
        /// </summary>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeDistribuidor">Código Grupo Entidade do Distribuidor</param>
        /// <param name="codigoEntidadeDistribuidor">Código Entidade Distribuidor</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirDistribuidor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor,
            Int32 codigoGrupoEntidadeDistribuidor, Int32 codigoEntidadeDistribuidor);

        /// <summary>
        /// Inserção de Distribuidor.
        /// </summary>
        /// <param name="distribuidor">Distribuidor a ser inserido</param>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirDistribuidor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor,
            Servicos.Distribuidor distribuidor);

        /// <summary>
        /// Exclusão de Fornecedor
        /// </summary>
        /// <param name="codigoEntidadeFornecedor">Código Entidade do Fornecedor</param>
        /// <param name="codigoGrupoEntidadeFornecedor">Código Grupo Entidade do Fornecedor</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirFornecedor(Int32 codigoGrupoEntidadeFornecedor, Int32 codigoEntidadeFornecedor);

        /// <summary>
        /// Inserção de Fornecedor.
        /// </summary>
        /// <param name="fornecedor">Fornecedor</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirFornecedor(Servicos.Fornecedor fornecedor);

        /// <summary>
        /// Consultar Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoFornecedor">Código do Grupo de Fornecedor</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Associações de Grupo Fornecedor x Ramos de Atividade</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.GrupoFornecedorRamoAtividade> ConsultarGrupoFornecedorRamoAtividade(Int32 codGrupoFornecedor, out Int32 codigoRetorno);

        /// <summary>
        /// Exclusão de associação Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        /// <param name="codGrupoFornecedor">Código do grupo fornecedor</param>
        /// <param name="codGrupoRamoAtividade">Código do grupo ramo de atividade</param>
        /// <param name="codRamoAtividade">Código do ramo de atividade</param>
        /// <param name="localFlag">Flag de exclusão local/dual</param>
        /// <returns>Código de retorno.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirGrupoFornecedorRamoAtividade(
            Int32 codGrupoFornecedor, Int32? codGrupoRamoAtividade, Int32? codRamoAtividade, Int32? localFlag);

        /// <summary>
        /// Inserção de associação Grupo Fornecedor x Ramo de Atividade
        /// </summary>
        ///<param name="grupoFornecedorXRamo">Dados da associação Grupo Fornecedor x Ramo de Atividade</param>
        /// <param name="localFlag">Flag de exclusão local/dual</param>
        /// <returns>Código de retorno.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirGrupoFornecedorRamoAtividade(Servicos.GrupoFornecedorRamoAtividade grupoFornecedorXRamo, Int32? localFlag);

        /// <summary>
        /// Consulta os Ramos de Atividade
        /// </summary>
        /// <param name="codGrupoRamo">Código do grupo ramo</param>
        /// <param name="codRamoAtividade">Código do ramo atividade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Ramos de Atividades</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.RamoAtividade> ConsultarRamosAtividade(Int32? codGrupoRamo, Int32? codRamoAtividade, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta as novas entidades credenciadas ou recadastradas num determinado período
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns>Listagem de Modelo.Entidade com Entidades credenciadas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Entidade> ConsultarCredenciamentos(DateTime dataInicio, DateTime dataFim, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os Produtos Flex cadastrados para um estabelecimento
        /// </summary>
        /// <param name="codigoCCA">Código da CCA</param>
        /// <param name="codigoFeature">Código da Feature</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Retorna a lista dos Produtos Flex de um estabelecimento</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.ProdutoFlex> ConsultarProdutosFlex(Int32 codigoEntidade, Int32? codigoCCA, Int32? codigoFeature, out Int32 codigoRetorno);

        /// <summary>
        /// <para>Validar se o CPNJ/CPF e Código da Entidade existem na base do GE e do PN.</para>
        /// <para>Caso não exista na base do PN, cria a entidade no PN a partir dos dados do GE.</para>
        /// </summary>
        /// <param name="pvs">Códigos das Entidades</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Código de retorno
        /// <para>0 - Retorno OK</para>
        /// <para>!= 0 - Retorno NOK</para></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ValidarCriarEntidade(List<Int32> pvs, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Valida se o estabelecimento indicado possui o serviço EAdquirencia (E-Rede)
        /// </summary>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno
        /// <para>0 - Retorno OK</para>
        /// <para>!= 0 - Retorno NOK</para></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ValidarPossuiEAdquirencia(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Incrementa a quantidade de erros de Confirmação Positva da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Quantidade de Tentativas já realizadas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 IncrementarQuantidadeConfirmacaoPositiva(Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para o Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código Grupo da Entidade</param>
        /// <returns>Retorna Código de erro ao reiniar a qtd de tentativas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ReiniciarQuantidadeConfirmacaoPositiva(Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para múltiplas Entidades
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Listagem de Entidades</param>
        /// <returns>Retorna dicionário com o código de erro correspondente a cada entidade, ao reiniciar a qtd de tentativas</returns>
        [OperationContract(Name = "ReiniciarQuantidadeConfirmacaoPositivaMultiplasEntidades")]
        [FaultContract(typeof(GeneralFault))]
        Dictionary<Int32, Int32> ReiniciarQuantidadeConfirmacaoPositiva(Int32[] codigoEntidades, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Valida se a Entidade já possui algum Usuário Master
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="possuiMaster">
        /// <para>True - Se a Entidade possuir Usuário Master</para>
        /// <para>False - Se a Entidade não possuir Usuário Master</para>
        /// </param>
        /// <param name="possuiUsuario">
        /// <para>True - Se a Entidade possuir Usuário</para>
        /// <para>False - Se a Entidade não possuir Usuário</para>
        /// </param>
        /// <param name="possuiSenhaTemporaria">
        /// <para>True - Se a Entidade possuir Usuário com Senha Temporária</para>
        /// <para>False - Se a Entidade não possuir Usuário com Senha Temporária</para>
        /// </param>
        /// <returns>
        /// Código de retorno da consulta
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 PossuiUsuario(Int32 codigoEntidade, Int32 codigoGrupoEntidade, out Boolean possuiUsuario, out Boolean possuiMaster, out Boolean possuiSenhaTemporaria);

        /// <summary>
        /// Verificar se o PV informado possui usuários com a flag de Legado
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo Entidade</param>
        /// <returns>
        /// <para>True - Possui Usuário Legado</para>
        /// <para>False - Não possui Usuário Legado</para>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean PossuiUsuarioLegado(Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Consulta os usuários de um PV, filtrando-os pelo perfil
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código grupo entidade</param>
        /// <param name="tipoUsuario">
        /// <para>Tipo do Usuário:</para>
        /// <para> - "M": Master</para>
        /// <para> - "B": Básico</para>
        /// <para> - "P": Personalizado</para>
        /// </param>
        /// <param name="codigoRetorno">Código retorno da consulta</param>
        /// <returns>Usuários do PV com o Perfil solicitado</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Usuario> ConsultarUsuariosPorPerfil(
            Int32 codigoEntidade, Int32 codigoGrupoEntidade, Char tipoUsuario, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os usuários de um PV, filtrando-os pelo Status
        /// </summary>
        /// <param name="status">Status dos usuários que serão retornados</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código grupo entidade</param>
        /// <param name="codigoRetorno">Código retorno da consulta</param>
        /// <returns>Usuários do PV com o Status solicitado</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Usuario> ConsultarUsuariosPorStatus(
            Int32 codigoEntidade, Int32 codigoGrupoEntidade, Comum.Enumerador.Status status, out Int32 codigoRetorno);

        /// <summary>
        /// Verifica, na listagem de PVs, se algum tem Komerci
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Se algum PV possui Komerci</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean PossuiKomerci(List<Int32> pvs);

        /// <summary>
        /// Atualizar os Status de Acesso da Entidade no PN
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código de Grupo Entidade</param>
        /// <param name="codigoEntidade">Códido do Estabelecimento</param>
        /// <param name="statusEntidade">Status da Entidade</param>
        /// <param name="nomeResponsavel">E-mail/login do responsável pela atualização</param>
        /// <returns>Código de Retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarStatusAcesso(Int32 codigoGrupoEntidade, Int32 codigoEntidade, Comum.Enumerador.Status statusEntidade, String nomeResponsavel);

        /// <summary>
        /// Consulta se o PV está no programa de fidelização.
        /// </summary>
        /// <param name="NumeroPv">Número do PV</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarPVFidelidade(Int32 NumeroPv);

        /// <summary>
        /// Consulta dados bancarios credito/debito
        /// </summary>
        /// <param name="codigoEntidade">Numero PV</param>
        /// <param name="codigoRetorno">Codigo retorno da SP</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.DadosBancarios ConsultarDadosBancariosCadastro(Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Verifica se os estabelecimentos estão filiados à Rede.
        /// </summary>
        /// <param name="numerosCgcCpf">Lista contendo o número de CNPJ/CPF a serem verificados</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Dicionário contendo o número do CNPJ/CPF como chave, e o Status do estabelecimento, como valor.
        /// C: NÃO - CANCELADO; S: SIM, N: NÃO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Dictionary<Int64, String> ListarEstabelecimentosFiliados(List<Int64> numerosCgcCpf, out Int32 codigoRetorno);


        /// <summary>
        /// Lista metodos de acordo com o tipo
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<string> ListarMetodos(string dllPath, string className);

        /// <summary>
        /// Get num version
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        int GetNumVersion();

        /// <summary>
        /// Retorna informações de condições comerciais caso não haja aceite realizado
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> ConsultarInformacaoComercial(Int64 numeroPv);

        /// <summary>
        /// Retorna informações de condições comerciais
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> RecuperarInformacaoComercial(Int64 numeroPv);

        /// <summary>
        /// Altera o status do aceite de condições comerciais
        /// </summary>
        /// <param name="codigoUsuario"></param>
        /// <param name="usuario"></param>
        /// <param name="numeroPv"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        BaseResponse AlterarStatusInformacaoComercial(Int64 codigoUsuario, Int64 numeroPv, String status);
    }
}
