#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [09/05/2012] – [André Garcia] – [Criação]
*/
#endregion
using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Interface do serviço de usuário
    /// </summary>
    [ServiceContract]
    public interface IUsuarioServico
    {

        #region Métodos utilizados na Intranet

        /// <summary>
        /// Aprovar Log de dupla custódia - Utilizado na intranet
        /// </summary>
        /// <param name="codigoLog">Código do Log</param>
        /// <param name="usuarioAprovador">Usuário aprovador do Log</param>
        /// <returns>Retorno da aprovação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AprovarLog(Int32 codigoLog, String usuarioAprovador);

        /// <summary>
        /// Consultar log por data - histórico - Intranet
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno que será retornado</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <returns>Listagem de log</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Log> ConsultarLogPorData(out Int32 codigoRetorno, DateTime? dataInicial, DateTime? dataFinal);

        /// <summary>
        /// Consultar log por data - histórico - Intranet
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno que será retornado</param>
        /// <returns>Listagem de log</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Log> ConsultarLog(out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os usuários que ainda não foram aprovados pela dupla custódia - Intranet
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="_codigoRetorno">Código de retorno</param>
        /// <returns>Retorna listagem de usuários que ainda não foram aprovados pela dupla custódia</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Usuario> ConsultarUsuariosPendentes(Int32? codigoGrupoEntidade, Int32? codigoEntidade, String codigoUsuario, out Int32 _codigoRetorno);

        /// <summary>
        /// Consulta servicos - Podendo ser por usuário, entidade ou todas
        /// Utilizado na Intranet e por compatibilidade com o código IS não migrado
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo da entdiade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <param name="_codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioServicoModelo</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.UsuarioServicoModelo> ConsultarUsuariosServico(Int32? codigoGrupoEntidade, Int32? codigoEntidade, string codigoUsuario, Int32? codigoServico, out Int32 _codigoRetorno);

        #endregion

        #region Métodos utilizados no Serviço PCI

        /// <summary>
        /// Consultar usuários que estão com a data de último acesso perto da expiração
        /// Utilizado no serviço PCI
        /// </summary>
        /// <returns>Listagem de usuários</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> ConsultarUsuariosComDataBloqueioProximo();

        /// <summary>
        /// Consultar usuários com hashes de e-mail expirados para processamento no ISRobô
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Usuario> ConsultarHashesExpirados();

        #endregion

        #region Métodos utilizados no Serviço ISRobo

        /// <summary>
        /// Inclui um usuário para a entidade especificada pelo ISRobo
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="descricaoUsuario">Descrição do usuário</param>
        /// <param name="tipoUsuario">Tipo de usuário</param>
        /// <param name="senhaCriptografadaTemporaria">Senha criptografada temporária</param>
        /// <param name="senhaCriptografadaDefinitiva">Senha criptografada definitiva</param>
        /// <param name="nomeResponsavelUltimaAlteracao">Nome do responsável pela última atualização</param>
        /// <param name="pvKomerci">Indicador se o PV possui Komerci</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "Inserir")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Inserir(String codigoUsuario,
            Int32 codigoEntidade,
            Int32 codigoGrupoEntidade,
            String descricaoUsuario,
            String tipoUsuario,
            String senhaCriptografadaTemporaria,
            String senhaCriptografadaDefinitiva,
            String nomeResponsavelUltimaAlteracao,
            Boolean pvKomerci = false);

        #endregion

        #region Métodos de atualização

        /// <summary>
        /// Método responsável pela criação/atualização de um perfil de usuário no Sybase
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <param name="codigoEntidades">Código da Entidade</param>
        /// <param name="login">Login do Usuário </param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="tipoUsuario">Tipo do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Serviços do usuário</param>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <returns>Códito do retorno</returns>
        [OperationContract(Name = "Atualizar")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Atualizar(Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos,
            Int32 codigoIdUsuario);

        /// <summary>
        /// Método responsável pela criação/atualização de um perfil de usuário no Sybase
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <param name="codigoEntidades">Código da Entidade</param>
        /// <param name="login">Login do Usuário </param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="tipoUsuario">Tipo do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Serviços do usuário</param>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="email">E-mail</param>
        /// <param name="emailSecundario">E-mail secundário</param>
        /// <param name="cpf">CPF</param>
        /// <param name="dddCelular">DDD do celular</param>
        /// <param name="celular">Celular</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param> 
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash para envio de e-mail</param>
        /// <returns>Códito do retorno</returns>
        [OperationContract(Name = "Atualizar2")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Atualizar(Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos,
            Int32 codigoIdUsuario,
            String email,
            String emailSecundario,
            Int64? cpf,
            Int32? dddCelular,
            Int32? celular,
            Redecard.PN.Comum.Enumerador.Status? status,
            Int32? diasExpiracaoEmail,
            DateTime? dataExpiracaoEmail,
            out Guid hashEmail);

        /// <summary>
        /// Método responsável pela atualização das permissões de usuário
        /// </summary>
        /// <param name="codigoServicos">Códigos dos serviços separados por ","</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "AtualizarPermissoes")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarPermissoes(String codigoServicos, Int32 codigoIdUsuario);

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="usuario">Modelo do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "AtualizarSenha")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarSenha(Usuario usuario, String senha, Boolean pvKomerci, Boolean senhaTemporaria);

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário(login)</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "AtualizarSenhaUsuario")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarSenha(Int32 codigoGrupoEntidade,
            Int32 codigoEntidade,
            String codigoUsuario, 
            Int32 codigoIdUsuario, 
            String senha, 
            Boolean pvKomerci, 
            Boolean senhaTemporaria);


        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="pvs">Pvs relacionados ao usuário que a senha será alterada</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "AtualizarSenhaUsuarioNovoAcesso")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarSenha(Int32 codigoIdUsuario,
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
        Int32 AtualizarEmail(Int32 codigoIdUsuario, String email, Int32? diasExpiracaoEmail,
            DateTime? dataExpiracaoEmail, out Guid hashEmail);

        /// <summary>
        /// Atualizar status do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param>
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash de envio de e-mail</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarStatus(Int32 codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status status, double? diasExpiracaoEmail,
            DateTime? dataExpiracaoEmail, out Guid hashEmail);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AtualizarStatusParaAguardandoConfirRecSenha(Int32 codigoIdUsuario, string email, double? diasExpiracaoEmail, DateTime? dataExpiracaoEmail, int[] pvsSelecionados, out Guid hashEmail);

        /// <summary>
        /// Confirma atualização do e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConfirmarAtualizacaoEmail(Int32 codigoIdUsuario);

        /// <summary>
        /// Atualizar de um perfil de usuário no PNDB através da Intranet
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarUsuario(Usuario usuario);

        /// <summary>
        /// Atualizar de um perfil de usuário no PNDB através da Intranet
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="sistema"></param>
        /// <returns></returns>
        [OperationContract(Name = "AtualizarUsuarioMaster")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarUsuario(Usuario usuario, string sistema);

        /// <summary>
        /// Desfaz as alterações de um usuário cujo hash da alteração tenha expirado
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoOperacao">Código indicador de qual operação será executada para desfazer as alterações:
        /// <para>'RL' - Resetar Migração Usuário Legado</para>
        /// <para>'RE' - Resetar Alteração de E-mail</para>
        /// </param>
        /// <returns>Código de retorno:
        /// <para>404 - Usuário não encontrado</para>
        /// <para>99 - Erro genérico</para>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 DesfazerAlteracoesExpiradas(Int32 codigoIdUsuario, String codigoOperacao);

        /// <summary>
        /// Método responsável pela atualização da flag de exibição da
        /// mensagem de Acesso Completo
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="exibirMensagem">Exibir mensagem</param>
        /// <returns>Código do retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarFlagExibicaoMensagemLiberacaoAcessoCompleto(
            Int32 codigoIdUsuario,
            Boolean exibirMensagem);

        /// <summary>
        /// Altera o status do usuario por PVS
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="exibirMensagem">Exibir mensagem</param>
        /// <returns>Código do retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AlterarParaStatusPendenteIdPos(Int32 codigoIdUsuario, string email, Int64? cpf, int[] pvs);
        #endregion

        #region Métodos de inclusão

        /// <summary>
        /// Inserir usuário
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="nomeUsuario">Nome do Usuário</param>
        /// <param name="tipoUsuario">Tipo do Usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Código de serviços do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "InserirUsuario")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Inserir(
            Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos);

        /// <summary>
        /// Inserir usuário
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidades">Códigos das Entidades separados por ","</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="nomeUsuario">Nome do Usuário</param>
        /// <param name="tipoUsuario">Tipo do Usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Código de serviços do usuário separados por ","</param>
        /// <param name="email">E-mail</param>
        /// <param name="emailSecundario">E-mail secundário</param>
        /// <param name="cpf">CPF</param>
        /// <param name="dddCelular">DDD do celular</param>
        /// <param name="celular">Celular</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="origem">Origem do usuário 'A' = ABERTA / F = FECHADA</param>
        /// <param name="codigoIdUsuario">Código Id usuário</param>
        /// <param name="hashEmail">Hash para envio de e-mail</param>
        /// <returns>Código de retorno ou erro</returns>
        [OperationContract(Name = "InserirUsuario2")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Inserir(
            Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos,
            String email,
            String emailSecundario,
            Int64? cpf,
            Int32? dddCelular,
            Int32? celular,
            Redecard.PN.Comum.Enumerador.Status status,
            String origem,
            out Int32 codigoIdUsuario,
            out Guid hashEmail);

        #endregion

        #region Métodos de exclusão

        /// <summary>
        /// Remove um usuário para a entidade especificada - Intranet
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "Excluir")]
        [FaultContract(typeof(GeneralFault))]
        Int32 Excluir(String codigoUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Remove um serviço do usuário
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoServico">Código so serviço</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirUsuarioServico(String codigoUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade, Int32 codigoServico);

        /// <summary>
        /// Excluir usuários em lote - Novo Acesso
        /// </summary>
        /// <param name="codigos">Código dos usuários separados por ","</param>
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "ExcluirEmLoteNovoAcesso")]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirEmLote(String codigos);


        /// <summary>
        /// Exclui usuários em lote. - Método para funcoinalidade na tela de edição de usuário padrão(Diferentemente do Novo Acesso)
        /// </summary>
        /// <param name="codigos">Códigos dos usuários que serão excluídos. Exemplo: Login1|Login2|Login3</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo de entidade</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirEmLote(String codigos, Int32 codigoEntidade, Int32 codigoGrupoEntidade);

        /// <summary>
        /// Exluir todos usuários de uma entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirTodosUsuariosMaster(Servicos.Entidade entidade);

        /// <summary>
        /// Exclui hash de email por guid.
        /// </summary>
        /// <param name="hash">hash</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirHashPorGuid(Guid hash);

        /// <summary>
        /// Excluir o hash de envio de e-mail 
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirHash(Int32 codigoIdUsuario);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ExcluirUsuariosOperacionais();
        #endregion

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
        List<Servicos.Pagina> ConsultarPermissoes(out Int32 codigoRetorno, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String login, Int32 codigoIdUsuario);

        /// <summary>
        /// Consultar a data de último acesso do usuário
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="usuario">usuário</param>
        /// <returns>Data de último acesso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DateTime ConsultarDataUltimoAcesso(Int32 codigoEntidade, Int32 codigoGrupoEntidade, String usuario);

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
        List<Usuario> ConsultarPorCodigoESenha(String codigo, String senha, Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários pela senha informada
        /// </summary>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorSenha")]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> ConsultarPorSenha(String senha, Entidade entidade, out Int32 codigoRetorno);

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
        Int32 Validar(Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario, String senhaCriptografada, Boolean pvKomerci);

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
        Servicos.Usuario Consultar(Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario, out Int32 _codigoRetorno);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>      
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorCodigoEntidade")]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> Consultar(String codigo, Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorCodigo")]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> Consultar(String codigo, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarPorEntidade")]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> Consultar(Entidade entidade, out Int32 codigoRetorno);

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
        Usuario ConsultarPorEmailTemporario(String emailTemporario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, out Int32 codigoRetorno);

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
        Usuario[] ConsultarPorEmailPrincipalPorStatus(String emailUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, int[] status, out Int32 codigoRetorno);

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
        Usuario ConsultarPorEmailPrincipal(String emailUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "Consultar")]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> Consultar(out Int32 codigoRetorno);

        /// <summary>
        /// Consulta usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Usuario</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Usuario ConsultarPorID(Int32 codigoIdUsuario, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar os Usuários com o cpf informado relacionados a Entidade
        /// </summary>
        /// <param name="cpfUsuario"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Usuario> ConsultarPorCpf(Int64 cpfUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade);

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
        Usuario[] ConsultarPorCpfPrincipalPorStatus(long cpf, Int32 codigoGrupoEntidade, Int32 codigoEntidade, int[] status, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta o menu de navegação do usuário no Portal de Serviços
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Listagem de Menu</returns>
        [OperationContract(Name = "ConsultarMenu")]
        [FaultContract(typeof(GeneralFault))]
        List<Menu> ConsultarMenu(String codigoUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, Int32 codigoIdUsuario);

        /// <summary>
        /// Método de consulta de usuários para reenvio do Welcome
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários para reenvio do Welcome</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Usuario> ConsultarReenvioWelcome(out Int32 codigoRetorno);

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
        List<Servicos.UsuarioHash> ConsultarHash(Int32? codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status? status, Guid? hash, out Int32 codigoRetorno);

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioHash</returns>
        [OperationContract(Name = "ConsultarHash2")]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.UsuarioHash> ConsultarHash(out Int32 codigoRetorno);

        /// <summary>
        /// Reinicia o hash de confirmação de e-mail.
        /// Exclui hash anterior (caso exista para o usuário) e insere um novo hash.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Hash</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.UsuarioHash ReiniciarHash(Int32 codigoIdUsuario, out Int32 codigoRetorno);

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
        Guid InserirHash(Int32 codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status status, Double diasExpiracao, DateTime? dataGeracaoHash);

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
        Int32 IncrementarQuantidadeConfirmacaoPositiva(Int32 codigoIdUsuario);

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ReiniciarQuantidadeConfirmacaoPositiva(Int32 codigoIdUsuario);

        /// <summary>
        /// Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="cnpjEstabelecimento">CNPJ da entidade</param>        
        /// <returns>Código de retorno</returns>
        [OperationContract(Name = "ValidarConfirmacaoPositivaObrigatoria")]
        [FaultContract(typeof(GeneralFault))]
        Int32 ValidarConfirmacaoPositivaObrigatoria(Int32 codigoEntidade, Decimal cnpjEstabelecimento);

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
        Int32 ValidarConfirmacaoPositivaVariavel(Int32 codigoEntidade, List<Servicos.Pergunta> perguntas, out List<Servicos.Pergunta> perguntasIncorretas);

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
        bool ValidarConfirmacaoPositivaVariavel(Int32[] codigoEntidades, List<Servicos.Pergunta> perguntas, out Dictionary<Int32, List<Modelo.Pergunta>> perguntasIncorretas);

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
        Boolean ValidarCpfCnpjSocio(Int32 codigoEntidade, Int64 cpfCnpjSocio);

        #endregion

        #region Métodos de tratamento de erro no login

        /// <summary>
        /// Consulta erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <returns>Modelo TrataErroUsuarioLogin</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TrataErroUsuarioLogin ConsultarErroUsuarioLogin(String chave);

        /// <summary>
        /// Inseri erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <param name="codigo">Código de retorno do erro</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void InserirErroUsuarioLogin(String chave, Int32 codigo);

        /// <summary>
        /// Exclui erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <param name="codigo">Código de retorno do erro</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ExcluirErroUsuarioLogin(String chave, Int32 codigo);

        /// <summary>
        /// Incrementa a quantidade de senhas inválidas em 1 para o usuário especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 IncrementarQuantidadeSenhaInvalida(int codigoIdUsuario);

        /// <summary>
        /// Atualiza a quantidade de senhas inválidas para 0 para o usuário
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ReiniciarQuantidadeSenhaInvalida(Int32 codigoIdUsuario);

        #endregion

        #region Métodos dos usuários komerci

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários do Komerci</returns>
        [OperationContract(Name = "ConsultarKomerciPorCodigoEntidade")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerci(String codigo, Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param> 
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarKomerciPorCodigo")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerci(String codigo, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarKomerciPorEntidade")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerci(Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarKomerci")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerci(out Int32 codigoRetorno);

        /// <summary>
        /// Inclusão de usuário do Komerci
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 1 - Usuário já existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract(Name = "InserirKomerci")]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirKomerci(UsuarioKomerci usuario, String senha);

        /// <summary>
        /// Alteração de senha do Usuário do Komerci
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Nova senha do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract(Name = "AtualizarKomerci")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarKomerci(UsuarioKomerci usuario, String senha);

        /// <summary>
        /// Exclui usuário(s) do Komerci em lote.
        /// </summary>
        /// <param name="codigos">Códigos dos usuários que serão excluídos. Exemplo: Codigo1|Codigo2|Codigo3</param>
        /// <param name="entidade">Entidade que esses usuários pertencem. Necessário o código da entidade e o código do grupo da entidade</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirKomerciEmLote(String codigos, Servicos.Entidade entidade);

        /// <summary>
        /// Consulta login KO usuários do sistema Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno);

        #endregion

        #region Métodos dos usuários komerci EC

        /// <summary>
        /// Consulta os Usuários do Komerci da base EC por Código e Entidade
        /// </summary>
        /// <param name="codigo">Código Usuário Komerci</param>
        /// <param name="entidade">Entidade do Usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista de Usuários do Komerci</returns>
        [OperationContract(Name = "ConsultarKomerciPorCodigoEntidadeEC")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerciEC(String codigo, Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os Usuários do Komerci da base EC  por Código
        /// </summary>
        /// <param name="codigo">Código Usuário Komerci</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista de Usuários do Komerci</returns>
        [OperationContract(Name = "ConsultarKomerciPorCodigoEC")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerciEC(String codigo, out Int32 codigoRetorno);

        /// <summary>
        /// Consulta os Usuários do Komerci da base EC  por Entidade
        /// </summary>
        /// <param name="entidade">Entidade do Usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Lista de Usuários do Komerci</returns>
        [OperationContract(Name = "ConsultarKomerciPorEntidadeEC")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerciEC(Entidade entidade, out Int32 codigoRetorno);

        /// <summary>
        /// Consultar usuários do Komerci da base EC 
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        [OperationContract(Name = "ConsultarKomerciEC")]
        [FaultContract(typeof(GeneralFault))]
        List<UsuarioKomerci> ConsultarKomerciEC(out Int32 codigoRetorno);

        /// <summary>
        /// Inclusão de usuário do Komerci da base EC 
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 1 - Usuário já existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract(Name = "InserirKomerciEC")]
        [FaultContract(typeof(GeneralFault))]
        Int32 InserirKomerciEC(UsuarioKomerci usuario);

        /// <summary>
        /// Alteração de senha do Usuário do Komerci da base EC 
        /// </summary>
        /// <param name="usuario">Usuário do Komerci</param>
        /// <param name="senha">Nova senha do usuário</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract(Name = "AtualizarKomerciEC")]
        [FaultContract(typeof(GeneralFault))]
        Int32 AtualizarKomerciEC(UsuarioKomerci usuario);

        /// <summary>
        /// Exclui usuário(s) do Komerci da base EC  em lote.
        /// </summary>
        /// <param name="codigos">Códigos dos usuários que serão excluídos. Exemplo: Codigo1|Codigo2|Codigo3</param>
        /// <param name="entidade">Entidade que esses usuários pertencem. Necessário o código da entidade e o código do grupo da entidade</param>
        /// <returns>Retorna identificador caso
        /// 0 - OK
        /// 2 - Usuário não existe
        /// 99 - Erro não previsto
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ExcluirKomerciEmLoteEC(String codigos, Servicos.Entidade entidade, string nomeResponsavel);

        #endregion

        #region Dupla Custodia

        /// <summary>
        /// Aprovação da dupla custódia
        /// </summary>
        /// <param name="codigoUsuario">Códoigo do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="nomeResponsavel">Nome do responsável da aprovação</param>
        /// <param name="tipoManutencao">Tipo de manutenção</param>
        /// <param name="nomeSistema">Nome do sistema origem</param>
        /// <returns>Código de retorno</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 AprovacaoDuplaCustodia(String codigoUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade, String nomeResponsavel, String tipoManutencao, String nomeSistema);
        #endregion

        #region serviços passo 1 criação usuário

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        int[] VerificarSeUsuariosEstaoAguardandoConfirmacao(string email);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] GetUsuarioAguardandoConfirmacaoMaster(string email);

        #endregion

        #region Serviços passo 2 criação usuario

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        bool CriarUsuarioVariosPvs(Modelo.EntidadeServicoModel[] entidadesSelecionadas, double emailExpira, out Modelo.EntidadeServicoModel[] entidadesPossuemUsuMaster, out Modelo.EntidadeServicoModel[] entidadesNPossuemUsuMaster, out Guid hash, out int codigoErro, out String Mensagem);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.EntidadeServicoModel[] ConsultarEmailsUsuMaster(int[] pvs, long cpf, string email);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AtualizarStatusPorPvs(int[] pvs, long cpf, string email, Redecard.PN.Comum.Enumerador.Status status);

        #endregion

        [OperationContract]
        string ObterRetornoComum();

    }
}
