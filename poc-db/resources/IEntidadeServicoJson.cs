using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Data;
using Redecard.PN.DadosCadastrais.Servicos.Modelos;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Interface para o Serviço WCF para Consulta de dados da Entidade com retorno JSon
    /// </summary>
    [ServiceContract]
    public interface IEntidadeServicoJson
    {
        /// <summary>
        /// Consultar filiais do estabelecimento a partir do código de associação
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="tipoAssociacao"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarFiliais/{codigoEntidade}/{tipoAssociacao}")]
        List<Servicos.Filial> ConsultarFiliais(String codigoEntidade, String tipoAssociacao);

        /// <summary>
        /// Consultar filiais do estabelecimento informado
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarFiliais/{codigoEntidade}")]
        List<Servicos.Filial> ConsultarFiliaisEntidade(String codigoEntidade);

        /// <summary>
        /// Consultar dados do PV na base de dados do GE
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDadosPV/{codigoEntidade}")]
        Servicos.EntidadeResponse ConsultarDadosPV(String codigoEntidade);

        /// <summary>
        /// Consultar dados de todos os PVs na base de dados do GE e TG para réplica no PN
        /// </summary>
        /// <param name="codigoEntidade">Código da última Entidade em caso de rechamada</param>
        /// <returns>Modelo de Entidades preenchido com informações do Sybase GE, objeto de Status de Retorno e objeto de Rechamada</returns>
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarEstabelecimentos/{codigoEntidade}")]
        Servicos.ListaEntidadesResponse ConsultarEstabelecimentos(String codigoEntidade);

        /// <summary>
        /// Tipo de Tecnologia do Estabelecimento.
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns>
        /// 25 ou 26 ou 23 - Komerci
        /// 20 - Normal
        /// 0 - Erro
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarTecnologiaEstabelecimento/{codigoEntidade}")]
        Servicos.TecnologiaResponse ConsultarTecnologiaEstabelecimento(String codigoEntidade);

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
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarEAdquirencia/{codigoEntidade}")]
        Boolean ValidarPossuiEAdquirencia(String codigoEntidade);

        /// <summary>
        /// Consulta os PVs no PN, confere no GE e equaliza as bases GE-PN através do CPF/CNPJ.
        /// </summary>
        /// <param name="codigoTipoPessoa"></param>
        /// <param name="numeroCpfCnpj"></param>
        /// <returns>Lista de PVs</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarEntidadeGeCriarNoPN/{codigoTipoPessoa}/{numeroCpfCnpj}")]
        Servicos.ResponseBaseList<Modelo.EntidadeServicoModel> ConsultarEntidadeGeCriarNoPN(String codigoTipoPessoa, String numeroCpfCnpj);

        /// <summary>
        /// Consulta os e-mails dos PVs informados
        /// </summary>
        /// <param name="pvs">Arrau de PVs para consulta à base de dados</param>
        /// <returns>Dicionário contendo o e-mail para cada PV consultado</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarEmailPVs")]
        Servicos.ResponseBaseItem<Dictionary<int, string>> ConsultarEmailPVs(int[] pvs);

        /// <summary>
        /// ValidarConfirmacaoPositivaPrimeiroAcesso
        /// </summary>
        /// <param name="emailUsuarioAlteracao"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarConfirmacaoPositivaPrimeiroAcesso")]
        Servicos.ConfirmacaoPositivaPrimeiroAcessoResponse ValidarConfirmacaoPositivaPrimeiroAcesso(
            String emailUsuarioAlteracao,
            Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoRequest request);

        /// <summary>
        /// Incrementa a quantidade de erros de Confirmação Positva das Entidades informadas
        /// </summary>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <param name="emailUsuarioAlteracao">Email do usuário</param>
        /// <returns>Quantidade de Tentativas já realizadas por entidade</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades")]
        Servicos.ResponseBaseList<Modelo.Entidade> IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(int[] codigoEntidades, String emailUsuarioAlteracao);

        /// <summary>
        /// Consulta os dados de IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <returns>Informações de URLBack</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarURLBack/{codigoEntidade}/{codigoGrupoEntidade}")]
        Servicos.URLBackResponse ConsultarURLBack(String codigoEntidade, String codigoGrupoEntidade);

        /// <summary>
        /// Atualiza os IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Dados da Entidade do Komerci</param>
        /// <param name="dados">Dados gerais do URLBack para atualização</param>
        /// <param name="usuarioAlteracao">Usuário responsável pela atualização</param>
        /// <returns>Código de Erro da procedure</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarURLBack")]
        Servicos.URLBackResponse AtualizarURLBack(String codigoEntidade, Servicos.URLBack dados, String usuarioAlteracao);

        /// <summary>
        /// Verifica se os pvs relacionados ao CPF\CNPJ são apenas de filiais
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "PvsRelacionadosSaoFiliais/{cnpj}")]
        Servicos.ResponseBaseItem<Boolean> PvsRelacionadosSaoFiliais(String cnpj);

        /// <summary>
        /// Lista das alterações de Domicílio Bancário solicitadas pela Entidade em status Pendente
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações de alteração de Domicílio Bancário</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ListarDomiciliosAlterados/{codigoEntidade}")]
        Servicos.ResponseBaseList<Servicos.DadosDomiciolioBancario> ListarDomiciliosAlterados(String codigoEntidade);

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
        /// <returns>Usuários do PV com o Perfil solicitado</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarUsuariosPorPerfil/{codigoEntidade}/{codigoGrupoEntidade}/{tipoUsuario}")]
        Servicos.ResponseBaseList<Servicos.Usuario> ConsultarUsuariosPorPerfil(
            String codigoEntidade, String codigoGrupoEntidade, String tipoUsuario);


        /// <summary>
        /// Consulta os pvs por CPF com paginação no banco
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <param name="pagina">Pagina</param>
        /// <param name="retornarEmail"></param>
        /// <param name="qtdRegistros">Quantidade de registros por pagina</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPvPorCpfComPaginacao")]
        ListaPaginadaEntidadesResponse ConsultarPvPorCpfComPaginacao(Int64 cpf, int pagina, int qtdRegistros, bool retornarEmail, string pvsSelecionados = null, string filtroGenerico = null);

        /// <summary>
        /// Consulta os pvs por email
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="pvsSelecionados">Filtro pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPvPorEmail")]
        ListaPaginadaEntidadesResponse ConsultarPvPorEmail(string email, string pvsSelecionados = null, string filtroGenerico = null);

        #region Taxas

        /// <summary>
        /// Consulta os dados bancários de Crédito, Débito ou Voucher
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDado">Tipo de dados a ser consultado.
        ///                 C - Crédito
        ///                 D - Débito
        ///                 V - Voucher
        /// </param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarDadosBancarios/{codigoEntidade}/{tipoDados}")]
        DadosBancariosListRest ConsultarDadosBancarios(String codigoEntidade, String tipoDados);

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
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarProdutosFlex/{codigoEntidade}/{codigoCCA=null}/{codigoFeature=null}")]
        ProdutoFlexListRest ConsultarProdutosFlex(String codigoEntidade, String codigoCCA, String codigoFeature);

        /// <summary>
        /// Consulta os dados bancários de Crédito, Débito de forma resumida para bandeiras populares.
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Retorna a lista de Dados Bancários de forma resumida e para apenas algumas bandeiras.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ResumoTaxas/{codigoEntidade}")]
        ResumoListRest ResumoTaxas(String codigoEntidade);

        #endregion

        /// <summary>
        /// Retorna informações de condições comerciais caso não haja aceite realizado
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> ConsultarInformacaoComercial(Int64 numeroPv);

        /// <summary>
        /// Retorna informações de condições comerciais
        /// </summary>
        /// <param name="numeroPv"></param>
        /// <returns></returns>
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.ResponseBaseItem<Modelo.InformacaoComercial.InformacaoComercial> RecuperarInformacaoComercial(Int64 numeroPv);

        /// <summary>
        /// Altera o status do aceite de condições comerciais
        /// </summary>
        /// <param name="codigoUsuario"></param>
        /// <param name="numeroPv"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        BaseResponse AlterarStatusInformacaoComercial(Int64 codigoUsuario, Int64 numeroPv, String status);

        /// <summary>
        /// Consulta bancos cadastradados na base DR para confirmação positiva
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarBancosConfirmacaoPositiva")]
        Servicos.ResponseBaseList<Servicos.Banco> ConsultarBancosConfirmacaoPositivaRest();

        /// <summary>
        /// Consulta as perguntas aleatórias disponíveis para o estabelecimento informado
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarPerguntasAleatorias/{numeroPV}")]
        Servicos.ResponseBaseList<Servicos.Pergunta> ConsultarPerguntasAleatoriasRest(String numeroPV);

        /// <summary>
        /// Verifica, na listagem de PVs, se algum tem Komerci
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Se algum PV possui Komerci</returns>
        [OperationContract(Name = "PossuiKomerci")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
           Method = "POST",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "PossuiKomerci")]
        Servicos.ResponseBaseItem<Boolean> PossuiKomerciRest(List<Int32> pvs);

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="codigoEntidade">Id da entidade</param>
        /// <param name="codigoGrupoEntidade">Id do grupo da entidade</param>
        /// <returns>Entidade preenchida</returns>
        [OperationContract(Name = "Consultar")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "Consultar/{codigoEntidade}/{codigoGrupoEntidade}")]
        EntidadeConsultarResponse Consultar(String codigoEntidade, String codigoGrupoEntidade);

        /// <summary>
        /// Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dados">Modelo de Dados Gerais com informações atualizadas</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarDadosGerais")]
        ResponseBaseItem<Int32> AtualizarDadosGeraisRest(Int32 codigoEntidade, Servicos.DadosGerais dados);

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
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarEntidadePn")]
        Int32 ValidarEntidadePn(String entidades);
    }
}
