#region Histórico do Arquivo
/*
(c) Copyright [2017] Redecard S.A.
Autor       : [Dhouglas Oliveira]
Empresa     : [Iteris]
Histórico   :
- [17/01/2017] – [Dhouglas Oliveira] – [Criação]
*/
#endregion
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.ServiceModel.Web;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Interface do serviço de usuário
    /// </summary>
    [ServiceContract]
    public interface IUsuarioServicoRest
    {
        #region Métodos de consulta

        /// <summary>
        /// Retorna a lista de páginas ao qual o usuário possui acesso
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Listagem de páginas que o usuário possui acesso</returns>
        [OperationContract(Name = "ConsultarPermissoes")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPermissoes/{codigoGrupoEntidade}/{codigoEntidade}/{login}/{codigoIdUsuario}")]
        ResponseBaseList<Servicos.Pagina> ConsultarPermissoes(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String login,
            String codigoIdUsuario);

        /// <summary>
        /// Consultar a data de último acesso do usuário
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="usuario">usuário</param>
        /// <returns>Data de último acesso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDataUltimoAcesso/{codigoEntidade}/{codigoGrupoEntidade}/{usuario}")]
        ResponseBaseItem<DateTime> ConsultarDataUltimoAcesso(
            String codigoEntidade,
            String codigoGrupoEntidade,
            String usuario);

        /// <summary>
        /// Consultar usuários pela código e senha informada
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorCodigoESenha")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorCodigoESenha")]
        ResponseBaseList<Servicos.Usuario> ConsultarPorCodigoESenha(
            String codigo,
            String senha,
            Servicos.Entidade entidade);

        /// <summary>
        /// Consultar usuários pela senha informada
        /// </summary>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorSenha")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorSenha")]
        ResponseBaseList<Servicos.Usuario> ConsultarPorSenha(
            String senha,
            Servicos.Entidade entidade);

        /// <summary>
        /// Verifica se o usuário especificado é um usuário válido no Portal Redecard
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaCriptografada">Senha criptopgrafada do usuário</param>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Validar")]
        BaseResponse Validar(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoUsuario,
            String senhaCriptografada,
            String pvKomerci);

        /// <summary>
        /// Retorna os dados de usuário e estabelecimento
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="_codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Servicos.Usuario</returns>
        [FaultContract(typeof(GeneralFault))]
        [OperationContract(Name = "ConsultarDadosUsuario")]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDadosUsuario/{codigoGrupoEntidade}/{codigoEntidade}/{codigoUsuario}")]
        ResponseBaseItem<Servicos.Usuario> Consultar(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoUsuario);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>      
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorCodigoEntidade")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorCodigoEntidade")]
        ResponseBaseList<Servicos.Usuario> Consultar(
            String codigo,
            Servicos.Entidade entidade);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorCodigo")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorCodigo/{codigo}")]
        ResponseBaseList<Servicos.Usuario> Consultar(
            String codigo);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorEntidade")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorEntidade")]
        ResponseBaseList<Servicos.Usuario> Consultar(
            Servicos.Entidade entidade);

        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail temporário
        /// </summary>
        /// <param name="emailTemporario">E-mail temporário do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorEmailTemporario/{emailTemporario}/{codigoGrupoEntidade}/{codigoEntidade}")]
        ResponseBaseItem<Servicos.Usuario> ConsultarPorEmailTemporario(
            String emailTemporario,
            String codigoGrupoEntidade,
            String codigoEntidade);

        /// <summary>
        /// Consulta o usuário de acordo com o email, status e código de entidade
        /// </summary>
        /// <param name="emailUsuario">email</param>
        /// <param name="codigoGrupoEntidade">Grupo entidade</param>
        /// <param name="codigoEntidade">Código entidades</param>
        /// <param name="status">Status</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorEmailPrincipalPorStatus")]
        ResponseBaseList<Servicos.Usuario> ConsultarPorEmailPrincipalPorStatus(
            String emailUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade,
            Int32[] status);

        /// <summary>
        /// Consulta o usuário de acordo com o email e código de entidade
        /// </summary>
        /// <param name="emailUsuario">email</param>
        /// <param name="codigoGrupoEntidade">Grupo entidade</param>
        /// <param name="codigoEntidade">Código entidades</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorEmailPrincipal/{emailUsuario}/{codigoGrupoEntidade}/{codigoEntidade}")]
        ResponseBaseItem<Servicos.Usuario> ConsultarPorEmailPrincipal(
            String emailUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "Consultar")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Consultar")]
        ResponseBaseList<Servicos.Usuario> Consultar();

        /// <summary>
        /// Consulta usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Usuario</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorID/{codigoIdUsuario}")]
        ResponseBaseItem<Servicos.Usuario> ConsultarPorID(
            String codigoIdUsuario);

        /// <summary>
        /// Consultar os Usuários com o cpf informado relacionados a Entidade
        /// </summary>
        /// <param name="cpfUsuario"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorCpf/{cpfUsuario}/{codigoEntidade}/{codigoGrupoEntidade}")]
        ResponseBaseList<Servicos.Usuario> ConsultarPorCpf(
            String cpfUsuario,
            String codigoEntidade,
            String codigoGrupoEntidade);

        /// <summary>
        /// Consulta o usuário por CPF e Status
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="status"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarPorCpfPrincipalPorStatus")]
        ResponseBaseList<Servicos.Usuario> ConsultarPorCpfPrincipalPorStatus(
            String cpf,
            String codigoGrupoEntidade,
            String codigoEntidade,
            Int32[] status);

        /// <summary>
        /// Consulta o menu de navegação do usuário no Portal de Serviços
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoIdUsuario">Código da entidade</param>
        /// <returns>Listagem de Menu</returns>
        [OperationContract(Name = "ConsultarMenu")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarMenu/{codigoUsuario}/{codigoGrupoEntidade}/{codigoEntidade}/{codigoIdUsuario}")]
        ResponseBaseList<Servicos.Menu> ConsultarMenu(
            String codigoUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoIdUsuario);

        /// <summary>
        /// Método de consulta de usuários para reenvio do Welcome
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários para reenvio do Welcome</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarReenvioWelcome")]
        ResponseBaseList<Servicos.Usuario> ConsultarReenvioWelcome();

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="hash">Hash de envio de e-mail</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioHash</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarHash")]
        ResponseBaseList<Servicos.UsuarioHash> ConsultarHash(
            String codigoIdUsuario,
            String status,
            String hash);

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioHash</returns>
        [OperationContract(Name = "ConsultarHash2")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarHash2")]
        ResponseBaseList<Servicos.UsuarioHash> ConsultarHash();

        /// <summary>
        /// Reinicia o hash de confirmação de e-mail.
        /// Exclui hash anterior (caso exista para o usuário) e insere um novo hash.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Hash</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ReiniciarHash")]
        ResponseBaseItem<Servicos.UsuarioHash> ReiniciarHash(
            String codigoIdUsuario);

        /// <summary>
        /// Método para inserir hash na base de dados a partir da camada de serviço
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracao">Dias corridos para expiração do hash</param>
        /// <param name="dataGeracaoHash">Data customizada em que o hash foi gerado</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "InserirHash")]
        ResponseBaseItem<Guid> InserirHash(
            String codigoIdUsuario,
            String status,
            String diasExpiracao,
            String dataGeracaoHash);

        #endregion

        #region Métodos da Confirmação positiva

        /// <summary>
        /// Incrementa a quantidade de confirmações positivas inválidas em 1 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "IncrementarQuantidadeConfirmacaoPositiva")]
        BaseResponse IncrementarQuantidadeConfirmacaoPositiva(
            String codigoIdUsuario);

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ReiniciarQuantidadeConfirmacaoPositiva")]
        BaseResponse ReiniciarQuantidadeConfirmacaoPositiva(
            String codigoIdUsuario);

        /// <summary>
        /// Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="cnpjEstabelecimento">CNPJ da entidade</param>        
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "ValidarConfirmacaoPositivaObrigatoria")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarConfirmacaoPositivaObrigatoria/{codigoEntidade}/{cnpjEstabelecimento}")]
        BaseResponse ValidarConfirmacaoPositivaObrigatoria(
            String codigoEntidade,
            String cnpjEstabelecimento);

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "ValidarConfirmacaoPositivaVariavel")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarConfirmacaoPositivaVariavel")]
        ResponseBaseList<Servicos.Pergunta> ValidarConfirmacaoPositivaVariavel(
            String codigoEntidade,
            List<Servicos.Pergunta> perguntas);

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços com múltiplas entidades.
        /// Caso retorne 0 a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Códigos da entidades</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Dicionário contendo listas de perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Dicionário com todos os retornos relacionado a cada entidade validada</returns>
        [OperationContract(Name = "ValidarConfirmacaoPositivaVariavelMultiplasEntidades")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarConfirmacaoPositivaVariavelMultiplasEntidades")]
        DicionarioPerguntasResponse ValidarConfirmacaoPositivaVariavel(
            Int32[] codigoEntidades,
            List<Servicos.Pergunta> perguntas);

        /// <summary>
        /// Verifica se o CPF/CNPJ informado paz parte da relação dos sócios vinculados ao PV informado
        /// </summary>
        /// <param name="codigoEntidade">Codigo da entidade (número do PV)</param>
        /// <param name="cpfCnpjSocio">CPF/CNPJ do sócio a ser verificado</param>
        /// <returns>
        ///     TRUE: CPF/CNPJ consta na relação dos sócios relacionados ao PV
        ///     FALSE: CPF/CNPJ não consta na relação dos sócios relacionados ao PV
        /// </returns>
        [OperationContract(Name = "ValidarCpfCnpjSocio")]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ValidarCpfCnpjSocio/{codigoEntidade}/{cpfCnpjSocio}")]
        ResponseBaseItem<Boolean> ValidarCpfCnpjSocio(
            String codigoEntidade,
            String cpfCnpjSocio);

        #endregion

        #region Dupla Custodia

        /// <summary>
        /// Aprovação da dupla custódia
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="nomeResponsavel">Nome do responsável da aprovação</param>
        /// <param name="tipoManutencao">Tipo de manutenção</param>
        /// <param name="nomeSistema">Nome do sistema origem</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AprovacaoDuplaCustodia")]
        BaseResponse AprovacaoDuplaCustodia(
            String codigoUsuario,
            String codigoEntidade,
            String codigoGrupoEntidade,
            String nomeResponsavel,
            String tipoManutencao,
            String nomeSistema);
        #endregion

        #region serviços passo 1 criação usuário

        /// <summary>
        /// VerificarSeUsuariosEstaoAguardandoConfirmacao
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "VerificarSeUsuariosEstaoAguardandoConfirmacao/{email}")]
        ResponseBaseList<Int32> VerificarSeUsuariosEstaoAguardandoConfirmacao(
            String email);

        /// <summary>
        /// GetUsuarioAguardandoConfirmacaoMaster
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "GetUsuarioAguardandoConfirmacaoMaster/{email}")]
        ResponseBaseList<Modelo.EntidadeServicoModel> GetUsuarioAguardandoConfirmacaoMaster(
            String email);

        #endregion

        #region Serviços passo 2 criação usuario

        /// <summary>
        /// CriarUsuarioVariosPvs
        /// </summary>
        /// <param name="entidadesSelecionadas"></param>
        /// <param name="emailExpira"></param>
        /// <param name="entidadesPossuemUsuMaster"></param>
        /// <param name="entidadesNPossuemUsuMaster"></param>
        /// <param name="hash"></param>
        /// <param name="codigoErro"></param>
        /// <param name="Mensagem"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "CriarUsuarioVariosPvs")]
        CriarUsuarioPvsResponse CriarUsuarioVariosPvs(
            Modelo.EntidadeServicoModel[] entidadesSelecionadas,
            String emailExpira);

        /// <summary>
        /// ConsultarEmailsUsuMaster
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarEmailsUsuMaster")]
        ResponseBaseList<Modelo.EntidadeServicoModel> ConsultarEmailsUsuMaster(
            Int32[] pvs,
            String cpf,
            String email);

        /// <summary>
        /// AtualizarStatusPorPvs
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <param name="status"></param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarStatusPorPvs")]
        BaseResponse AtualizarStatusPorPvs(
            Int32[] pvs,
            String cpf,
            String email,
            String status);

        #endregion

        /// <summary>
        /// ObterRetornoComum
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ObterRetornoComum")]
        ResponseBaseItem<String> ObterRetornoComum();

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="usuario">Modelo do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarSenha")]
        ResponseBaseItem<Int32> AtualizarSenha(Usuario usuario, String senha, Boolean pvKomerci, Boolean senhaTemporaria);

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="pvs">Pvs relacionados ao usuário que a senha será alterada</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <param name="atualizarStatus"></param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "AtualizarSenhaUsuarioNovoAcesso")]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarSenhaUsuarioNovoAcesso")]
        ResponseBaseItem<Int32> AtualizarSenhaUsuarioNovoAcesso(Int32 codigoIdUsuario,
            String senha,
            Boolean pvKomerci,
            int[] pvs,
            Boolean senhaTemporaria,
            Boolean? atualizarStatus = null);

        /// <summary>
        /// Atualizar e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param>
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash de envio de e-mail</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarEmail")]
        AlterarEmailResponse AtualizarEmail(Int32 codigoIdUsuario, String email, String diasExpiracaoEmail, String dataExpiracaoEmail);


        /// <summary>
        /// Atualisa o(s) usuário(s) para o status AguardandoConfirRecSenha e cria um hash de e-mail
        /// </summary>
        /// <param name="codigoIdUsuario">Identificador do usuário</param>
        /// <param name="email">Email</param>
        /// <param name="diasExpiracaoEmail">Dias de expiração do e-mail</param>
        /// <param name="dataExpiracaoEmail">Data de inicio</param>
        /// <param name="pvsSelecionados">PVs relacionados ao e-mail do usuário que</param>
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "AtualizarStatusParaAguardandoConfirRecSenha")]
        AtualizarRecuperacaoSenhaResponse AtualizarStatusParaAguardandoConfirRecSenha(Int32 codigoIdUsuario, string email, double? diasExpiracaoEmail, DateTime? dataExpiracaoEmail, int[] pvsSelecionados);
    }
}
