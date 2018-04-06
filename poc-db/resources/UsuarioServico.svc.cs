#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [09/05/2012] – [André Garcia] – [Criação]
 * 
 * 
(c) Copyright [2014] Rede
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [28/04/2014] – [André Rentes] – [ALteração da estrutura, comentários nos métodos, atualização novo acesso]
*/
#endregion

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;

using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Modelo;

namespace Redecard.PN.DadosCadastrais.Servicos
{

    /// <summary>
    /// Serviço para gerenciamento dos Usuários
    /// </summary>
    public class UsuarioServico : ServicoBase, IUsuarioServico
    {

        #region Métodos utilizados na Intranet

        /// <summary>
        /// Aprovar Log de dupla custódia - Utilizado na intranet
        /// </summary>
        /// <param name="codigoLog">Código do Log</param>
        /// <param name="usuarioAprovador">Usuário aprovador do Log</param>
        /// <returns>Retorno da aprovação</returns>
        public Int32 AprovarLog(Int32 codigoLog, String usuarioAprovador)
        {
            using (Logger Log = Logger.IniciarLog("Aprovar registro de Log de cadastro de usuários no banco PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    Negocio.Usuario _usuario = new Negocio.Usuario();
                    var result = _usuario.AprovarLog(codigoLog, usuarioAprovador);

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar log por data - histórico - Intranet
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno que será retornado</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <returns>Listagem de log</returns>
        public List<Servicos.Log> ConsultarLogPorData(out Int32 codigoRetorno, DateTime? dataInicial, DateTime? dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Buscar registros de Log de cadastro de usuários no banco PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    Negocio.Usuario _usuario = new Negocio.Usuario();
                    var result = _usuario.ConsultarLog(out codigoRetorno, dataInicial, dataFinal);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Mapper.CreateMap<Modelo.Log, Servicos.Log>();
                    List<Servicos.Log> logServicos = null;

                    foreach (Modelo.Log log in result)
                    {
                        if (object.ReferenceEquals(logServicos, null))
                            logServicos = new List<Servicos.Log>();
                        logServicos.Add(Mapper.Map<Modelo.Log, Servicos.Log>(log));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return logServicos;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar log por data - histórico - Intranet
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno que será retornado</param>
        /// <returns>Listagem de log</returns>
        public List<Servicos.Log> ConsultarLog(out Int32 codigoRetorno)
        {
            return this.ConsultarLogPorData(out codigoRetorno, null, null);
        }

        /// <summary>
        /// Consulta os usuários que ainda não foram aprovados pela dupla custódia - Intranet
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Retorna listagem de usuários que ainda não foram aprovados pela dupla custódia</returns>
        public List<Usuario> ConsultarUsuariosPendentes(int? codigoGrupoEntidade, int? codigoEntidade, string codigoUsuario, out int codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    var usuarios = new List<Servicos.Usuario>();
                    List<Modelo.Usuario> lstModeloEntidade = null;


                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoEntidade, codigoUsuario });

                    var negocioUsuario = new Negocio.Usuario();
                    lstModeloEntidade = negocioUsuario.ConsultarUsuariosPendentes(codigoGrupoEntidade, codigoEntidade, codigoUsuario, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { _codigoRetorno = codigoRetorno, lstModeloEntidade });

                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();

                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    foreach (var modeloUsuario in lstModeloEntidade)
                    {
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { usuarios });
                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta servicos - Podendo ser por usuário, entidade ou todas
        /// Utilizado na Intranet e por compatibilidade com o código IS não migrado
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo da entdiade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioServicoModelo</returns>
        public List<Servicos.UsuarioServicoModelo> ConsultarUsuariosServico(Int32? codigoGrupoEntidade, Int32? codigoEntidade, string codigoUsuario, Int32? codigoServico, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    var usuarios = new List<Servicos.UsuarioServicoModelo>();
                    List<Modelo.UsuarioServicoModelo> lstModeloEntidade = null;


                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoEntidade, codigoUsuario, codigoServico });

                    var negocioUsuario = new Negocio.Usuario();
                    lstModeloEntidade = negocioUsuario.ConsultarUsuariosServico(codigoGrupoEntidade, codigoEntidade, codigoUsuario, codigoServico, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { _codigoRetorno = codigoRetorno, lstModeloEntidade });

                    Mapper.CreateMap<Modelo.UsuarioServicoModelo, Servicos.UsuarioServicoModelo>();

                    foreach (var modeloUsuario in lstModeloEntidade)
                    {
                        usuarios.Add(Mapper.Map<Modelo.UsuarioServicoModelo, Servicos.UsuarioServicoModelo>(modeloUsuario));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { usuarios });
                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos utilizados no Serviço PCI

        /// <summary>
        /// Consultar usuários que estão com a data de último acesso perto da expiração
        /// Utilizado no serviço PCI
        /// </summary>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> ConsultarUsuariosComDataBloqueioProximo()
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários com data de bloqueio próximo do dia atual"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Usuario> modeloUsuarios = null;
                    var usuarios = new List<Servicos.Usuario>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuarios = negocioUsuario.ConsultarUsuariosComDataBloqueioProximo();

                    Log.GravarLog(EventoLog.RetornoNegocio);

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { usuarios });
                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar usuários com hashes de e-mail expirados para processamento no ISRobô
        /// </summary>
        /// <returns></returns>
        public List<Servicos.Usuario> ConsultarHashesExpirados()
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários com hashes de e-mail expirados para processamento no ISRobô"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Usuario> modeloUsuarios = null;
                    var usuarios = new List<Servicos.Usuario>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuarios = negocioUsuario.ConsultarHashesExpirados();

                    Log.GravarLog(EventoLog.RetornoNegocio, modeloUsuarios);

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { usuarios });

                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public Int32 Inserir(String codigoUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade, String descricaoUsuario,
            String tipoUsuario, String senhaCriptografadaTemporaria, String senhaCriptografadaDefinitiva, String nomeResponsavelUltimaAlteracao,
            Boolean pvKomerci = false)
        {
            using (Logger Log = Logger.IniciarLog("Inclui um usuário para a entidade especificada"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoUsuario, codigoEntidade, codigoGrupoEntidade, descricaoUsuario, tipoUsuario, senhaCriptografadaTemporaria, senhaCriptografadaDefinitiva, nomeResponsavelUltimaAlteracao });

                    Negocio.Usuario usuario = new Negocio.Usuario();
                    var result = usuario.Inserir(codigoUsuario, codigoEntidade, codigoGrupoEntidade, descricaoUsuario, tipoUsuario, senhaCriptografadaTemporaria, senhaCriptografadaDefinitiva, nomeResponsavelUltimaAlteracao, pvKomerci);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        #region Métodos de atualização

        /// <summary>
        /// Método responsável pela criação/atualização de um perfil de usuário no Sybase
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <param name="codigoEntidades">Códigos das Entidades separados por ","</param>
        /// <param name="login">Login do Usuário </param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="tipoUsuario">Tipo do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Serviços do usuário separados por ","</param>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <returns>Códito do retorno</returns>
        public Int32 Atualizar(Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos,
            Int32 codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela criação/atualização de um perfil de usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        codigoIdUsuario
                    });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();
                    var result = negocioUsuario.Atualizar(
                        pvKomerci,
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        codigoIdUsuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Método responsável pela criação/atualização de um perfil de usuário no Sybase
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <param name="codigoEntidades">Códigos das Entidades separados por ","</param>
        /// <param name="login">Login do Usuário </param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="tipoUsuario">Tipo do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Serviços do usuário separados por ","</param>
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
        public Int32 Atualizar(Boolean pvKomerci,
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
            out Guid hashEmail)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela criação/atualização de um perfil de usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        codigoIdUsuario,
                        email,
                        emailSecundario,
                        cpf,
                        dddCelular,
                        celular,
                        diasExpiracaoEmail
                    });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();
                    var result = negocioUsuario.Atualizar(
                        pvKomerci,
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        codigoIdUsuario,
                        email,
                        emailSecundario,
                        cpf,
                        dddCelular,
                        celular,
                        status,
                        diasExpiracaoEmail,
                        dataExpiracaoEmail,
                        out hashEmail);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método responsável pela atualização das permissões de usuário
        /// </summary>
        /// <param name="codigoServicos">Códigos dos serviços separados por ","</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarPermissoes(String codigoServicos, Int32 codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela atualização das permissões de usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoServicos, codigoIdUsuario });
                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.AtualizarPermissoes(codigoServicos, codigoIdUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="usuario">Modelo do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarSenha(Usuario usuario, String senha, Boolean pvKomerci, Boolean senhaTemporaria)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza a senha do usuário na Confirmação Positiva"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Usuario, Modelo.Usuario>();
                    Mapper.CreateMap<Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<GrupoEntidade, Modelo.GrupoEntidade>();
                    Mapper.CreateMap<TipoEntidade, Modelo.TipoEntidade>();
                    Mapper.CreateMap<Status, Modelo.Status>();

                    Modelo.Usuario modeloUsuario = Mapper.Map<Modelo.Usuario>(usuario);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloUsuario, senha });
                    var negocioUsuario = new Negocio.Usuario();
                    var result = negocioUsuario.AtualizarSenha(modeloUsuario, senha, pvKomerci, senhaTemporaria);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarSenha(Int32 codigoGrupoEntidade,
            Int32 codigoEntidade,
            String codigoUsuario, 
            Int32 codigoIdUsuario, 
            String senha, 
            Boolean pvKomerci, 
            Boolean senhaTemporaria)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela atualização da senha de um usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoUsuario, senha, pvKomerci, senhaTemporaria, codigoIdUsuario });

                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.AtualizarSenha(codigoGrupoEntidade, codigoEntidade, codigoUsuario, codigoIdUsuario, senha, pvKomerci, senhaTemporaria);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarSenha(Int32 codigoIdUsuario,
                                    String senha,
                                    Boolean pvKomerci,
                                    int[] pvs,
                                    Boolean senhaTemporaria,
                                    Boolean? atualizarStatus = null
            )
        {
            int idRetorno = 0;

            using (Logger Log = Logger.IniciarLog("Método responsável pela atualização da senha de um usuário no SQL"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario, senha, pvKomerci, senhaTemporaria });

                    idRetorno = new Negocio.Usuario().AtualizarSenha(codigoIdUsuario, senha, pvKomerci, pvs, senhaTemporaria, atualizarStatus);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { idRetorno });

                    return idRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualizar e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param>
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash de envio de e-mail</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarEmail(Int32 codigoIdUsuario, String email, Int32? diasExpiracaoEmail,
            DateTime? dataExpiracaoEmail, out Guid hashEmail)
        {
            Int32 codigoRetorno = 0;

            using (Logger Log = Logger.IniciarLog("Atualiza e-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario, email });

                    var negocioUsuario = new Negocio.Usuario();
                    codigoRetorno = negocioUsuario.AtualizarEmail(codigoIdUsuario, email, diasExpiracaoEmail, dataExpiracaoEmail, out hashEmail);

                    Log.GravarLog(EventoLog.FimServico, new { codigoIdUsuario, email, hashEmail, codigoRetorno });

                    return codigoRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualisa o(s) usuário(s) para o status AguardandoConfirRecSenha e cria um hash de e-mail
        /// </summary>
        /// <param name="codigoIdUsuario">Identificador do usuário</param>
        /// <param name="email">Email</param>
        /// <param name="diasExpiracaoEmail">Dias de expiração do e-mail</param>
        /// <param name="dataExpiracaoEmail">Data de inicio</param>
        /// <param name="pvsSelecionados">PVs relacionados ao e-mail do usuário que</param>
        /// <param name="hashEmail">Hash de email</param>
        public void AtualizarStatusParaAguardandoConfirRecSenha(Int32 codigoIdUsuario, string email, double? diasExpiracaoEmail, DateTime? dataExpiracaoEmail,int[] pvsSelecionados, out Guid hashEmail)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável por atualizar o status do usuário e criar uma hash de recuperação de senha"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario, diasExpiracaoEmail, dataExpiracaoEmail, pvsSelecionados });

                    new Negocio.Usuario().AtualizarStatusParaAguardandoConfirRecSenha(codigoIdUsuario, email, diasExpiracaoEmail, dataExpiracaoEmail, pvsSelecionados, out hashEmail);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { hashEmail });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualizar status do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param>
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash de envio de e-mail</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarStatus(Int32 codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status status, double? diasExpiracaoEmail,
            DateTime? dataExpiracaoEmail, out Guid hashEmail)
        {
            Int32 codigoRetorno = 0;

            using (Logger Log = Logger.IniciarLog("Atualiza e-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario, status });

                    var negocioUsuario = new Negocio.Usuario();
                    codigoRetorno = negocioUsuario.AtualizarStatus(codigoIdUsuario, status,
                        diasExpiracaoEmail, dataExpiracaoEmail, out hashEmail);

                    Log.GravarLog(EventoLog.FimServico, new { codigoIdUsuario, status, hashEmail, codigoRetorno });

                    return codigoRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Confirma atualização do e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 ConfirmarAtualizacaoEmail(Int32 codigoIdUsuario)
        {
            try
            {
                Negocio.Usuario usuarioDados = new Negocio.Usuario();
                return usuarioDados.ConfirmarAtualizacaoEmail(codigoIdUsuario);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Atualizar de um perfil de usuário no PNDB através da Intranet
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public Int32 AtualizarUsuario(Usuario usuario)
        {
            using (Logger Log = Logger.IniciarLog("Criar/atualizar de um perfil de usuário no PNDB através da Intranet"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuario });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();


                    Mapper.CreateMap<Servicos.Usuario, Modelo.Usuario>();
                    Modelo.Usuario _usuario = Mapper.Map<Servicos.Usuario, Modelo.Usuario>(usuario);

                    var result = negocioUsuario.AtualizarUsuario(_usuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualizar de um perfil de usuário no PNDB através da Intranet
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="sistema"></param>
        /// <returns></returns>
        public Int32 AtualizarUsuario(Usuario usuario, string sistema)
        {

            using (Logger Log = Logger.IniciarLog("Criar/atualizar de um perfil de usuário no PNDB através da Intranet"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuario, sistema });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();


                    Mapper.CreateMap<Servicos.Usuario, Modelo.Usuario>();
                    Modelo.Usuario _usuario = Mapper.Map<Servicos.Usuario, Modelo.Usuario>(usuario);

                    var result = negocioUsuario.AtualizarUsuario(_usuario, sistema);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Desfaz as alterações de um usuário cujo hash da alteração tenha expirado
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoOperacao">Código indicador de qual operação será executada para desfazer as alterações:
        /// <para>'RL' - Resetar Migração Usuário Legado</para>
        /// <para>'RE' - Resetar Alteração de E-mail</para>
        /// <para>'RS' - Resetar Recuperação de Senha</para>
        /// </param>
        /// <returns>Código de retorno:
        /// <para>404 - Usuário não encontrado</para>
        /// <para>99 - Erro genérico</para>
        /// </returns>
        public Int32 DesfazerAlteracoesExpiradas(Int32 codigoIdUsuario, String codigoOperacao)
        {
            using (Logger Log = Logger.IniciarLog("Desfaz as alterações de um usuário cujo hash da alteração tenha expirado"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario, codigoOperacao });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                    var result = negocioUsuario.DesfazerAlteracoesExpiradas(codigoIdUsuario, codigoOperacao);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método responsável pela atualização da flag de exibição da
        /// mensagem de Acesso Completo
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="exibirMensagem">Exibir mensagem</param>
        /// <returns>Código do retorno</returns>
        public Int32 AtualizarFlagExibicaoMensagemLiberacaoAcessoCompleto(
            Int32 codigoIdUsuario,
            Boolean exibirMensagem)
        {
            using (Logger log = Logger.IniciarLog("Atualiza flag de exibição de mensagem de liberação de acesso completo"))
            {
                try
                {
                    var usuarioNegocio = new Negocio.Usuario();
                    return usuarioNegocio.AtualizarFlagExibicaoMensagemLiberacaoAcessoCompleto(
                        codigoIdUsuario, exibirMensagem);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }            
            }
        }

        /// <summary>
        /// Altera o status do usuario por PVS
        /// </summary>
        /// <param name="codigoIdUsuario"></param>
        /// <param name="email"></param>
        /// <param name="cpf"></param>
        /// <param name="pvs"></param>
        public void AlterarParaStatusPendenteIdPos(Int32 codigoIdUsuario, string email, Int64? cpf, int[] pvs)
        {
            using (Logger log = Logger.IniciarLog("Atualiza o status dos usuarios por PVS"))
            {
                try
                {
                    new Negocio.Usuario().AlterarParaStatusPendenteIdPos(codigoIdUsuario, email, cpf, pvs);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos de inclusão

        /// <summary>
        /// Inserir usuário
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidades">Códigos das Entidades separado por ","</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="nomeUsuario">Nome do Usuário</param>
        /// <param name="tipoUsuario">Tipo do Usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Código de serviços do usuário separado por ","</param>
        /// <returns>Código de retorno</returns>
        public Int32 Inserir(
            Boolean pvKomerci,
            Int32 codigoGrupoEntidade,
            String codigoEntidades,
            String login,
            String nomeUsuario,
            String tipoUsuario,
            String senha,
            String codigoServicos)
        {
            using (Logger Log = Logger.IniciarLog("Inserir usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos
                    });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();
                    var result = negocioUsuario.Inserir(pvKomerci, codigoGrupoEntidade, codigoEntidades, login, nomeUsuario, tipoUsuario,
                         senha, codigoServicos);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inserir usuário
        /// </summary>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidades">Código da Entidade</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="nomeUsuario">Nome do Usuário</param>
        /// <param name="tipoUsuario">Tipo do Usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="codigoServicos">Código de serviços do usuário</param>
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
        public Int32 Inserir(
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
            out Guid hashEmail)
        {
            using (Logger Log = Logger.IniciarLog("Inserir usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        email,
                        emailSecundario,
                        cpf,
                        dddCelular,
                        celular,
                        status,
                        origem
                    });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                    var result = negocioUsuario.Inserir(pvKomerci,
                        codigoGrupoEntidade,
                        codigoEntidades,
                        login,
                        nomeUsuario,
                        tipoUsuario,
                        senha,
                        codigoServicos,
                        email,
                        emailSecundario,
                        cpf,
                        dddCelular,
                        celular,
                        status,
                        origem,
                        out codigoIdUsuario,
                        out hashEmail);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos de exclusão

        /// <summary>
        /// Remove um usuário para a entidade especificada - Intranet
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <returns>Código de retorno</returns>
        public Int32 Excluir(String codigoUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Remove um usuário para a entidade especificada"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoUsuario, codigoEntidade, codigoGrupoEntidade });

                    Negocio.Usuario _dados = new Negocio.Usuario();
                    var result = _dados.Excluir(codigoUsuario, codigoEntidade, codigoGrupoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Remove um serviço do usuário
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoServico">Código so serviço</param>
        /// <returns>Código de retorno</returns>
        public int ExcluirUsuarioServico(string codigoUsuario, int codigoEntidade, int codigoGrupoEntidade, int codigoServico)
        {
            using (Logger Log = Logger.IniciarLog("Remove um usuário para a entidade especificada"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoUsuario, codigoEntidade, codigoGrupoEntidade });

                    Negocio.Usuario _dados = new Negocio.Usuario();
                    var result = _dados.ExcluirUsuarioServico(codigoUsuario, codigoEntidade, codigoGrupoEntidade, codigoServico);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Excluir usuários em lote - Novo Acesso
        /// </summary>
        /// <param name="codigos">Código dos usuários separados por ","</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirEmLote(String codigos)
        {
            using (Logger Log = Logger.IniciarLog("Excluir usuários em lote - Novo Acesso"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.Usuario();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigos });
                    Int32 result = negocioUsuario.ExcluirEmLote(codigos);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui usuários em lote. - Método para funcoinalidade na tela de edição de usuário padrão(Diferentemente do Novo Acesso)
        /// </summary>
        /// <param name="codigos">Códigos dos usuários que serão excluídos. Exemplo: Login1|Login2|Login3</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo de entidade</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirEmLote(String codigos, Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Excluir usuários em lote"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.Usuario();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigos });
                    Int32 result = negocioUsuario.ExcluirEmLote(codigos, codigoEntidade, codigoGrupoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exluir todos usuários de uma entidade
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirTodosUsuariosMaster(Servicos.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Exluir todos usuários de uma entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.Usuario();

                    Modelo.Entidade modeloEntidade = null;

                    // Convert Business Entity para Data Contract Entity
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { modeloEntidade });
                    var result = negocioUsuario.ExcluirTodosUsuariosMaster(modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    // Exclui usuários
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Excluir o hash de envio de e-mail 
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirHashPorGuid(Guid hash)
        {
            using (Logger Log = Logger.IniciarLog("Exclui o hash de envio de e-mail "))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { hash });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                    var result = negocioUsuario.ExcluirHashPorGuid(hash);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Excluir o hash de envio de e-mail 
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirHash(Int32 codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Exclui o hash de envio de e-mail "))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });

                    Negocio.Usuario negocioUsuario = new Negocio.Usuario();
                    
                    var result = negocioUsuario.ExcluirHash(codigoIdUsuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui os usuários operacionais de acordo com o critério abaixo:
        ///    operacional..:	3 Dias	Usuários criados nos PVs para testes de ocorrências
        ///    interno......:	1 Mês	Usuários criados para testes mais longos EX: torreec@userede.com.br
        ///    monitoramento:	Nunca	Usuários criados para monitoramento do Pingdom
        /// </summary>
        /// <returns></returns>
        public Boolean ExcluirUsuariosOperacionais()
        {
            using (Logger Log = Logger.IniciarLog("Exclui o hash de envio de e-mail "))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.Usuario usuario = new Negocio.Usuario();
                    Boolean result = usuario.ExcluirUsuariosOperacionais();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));               
                }
            }
        }

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
        public List<Servicos.Pagina> ConsultarPermissoes(out Int32 codigoRetorno, Int32 codigoGrupoEntidade, Int32 codigoEntidade, String login, Int32 codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Retorna a lista de páginas que o usuário pode acessar no Portal de Serviços"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    List<Servicos.Pagina> paginas = null;

                    Negocio.Usuario negocio = new Negocio.Usuario();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoIdUsuario });
                    List<Modelo.Pagina> paginasModelo = negocio.ConsultarPermissoes(out codigoRetorno, codigoGrupoEntidade, codigoEntidade, login, codigoIdUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, paginasModelo });

                    if (!object.ReferenceEquals(paginasModelo, null) && paginasModelo.Count > 0)
                    {
                        Mapper.CreateMap(typeof(Modelo.Pagina), typeof(Servicos.Pagina));
                        paginas = new List<Pagina>();
                        paginasModelo.ForEach(delegate(Modelo.Pagina pagina)
                        {
                            Servicos.Pagina _pagina = Mapper.Map<Servicos.Pagina>(pagina);
                            paginas.Add(_pagina);
                        });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { paginas });
                    return paginas;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar a data de último acesso do usuário
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="usuario">usuário</param>
        /// <returns>Data de último acesso</returns>
        public DateTime ConsultarDataUltimoAcesso(Int32 codigoEntidade, Int32 codigoGrupoEntidade, String usuario)
        {
            using (Logger Log = Logger.IniciarLog("Consultar data último acesso"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade, usuario });
                    var negocioUsuario = new Negocio.Usuario();
                    var result = negocioUsuario.ConsultarDataUltimoAcesso(codigoEntidade, codigoGrupoEntidade, usuario);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar usuários pela código e senha informada
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> ConsultarPorCodigoESenha(String codigo, 
            String senha, 
            Entidade entidade, 
            out Int32 codigoRetorno)
        {
            return this.Consultar(codigo, senha, entidade, out codigoRetorno);
        }

        /// <summary>
        /// Consultar usuários pela senha informada
        /// </summary>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> ConsultarPorSenha(String senha, Entidade entidade, out Int32 codigoRetorno)
        {
            return this.Consultar(null, senha, entidade, out codigoRetorno);
        }

        /// <summary>
        /// Consultar os Usuários com o cpf informado relacionados a Entidade
        /// </summary>
        /// <param name="cpfUsuario"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        public List<Usuario> ConsultarPorCpf(Int64 cpfUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Usuario> modeloUsuarios = null;
                    var usuarios = new List<Servicos.Usuario>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { cpfUsuario, codigoEntidade, codigoGrupoEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuarios = negocioUsuario.ConsultarPorCpf(cpfUsuario, codigoEntidade, codigoGrupoEntidade);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { modeloUsuarios });

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { usuarios });
                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Entidade que o usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> Consultar(String codigo, String senha, Entidade entidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Usuario> modeloUsuarios = null;
                    var usuarios = new List<Servicos.Usuario>();
                    Modelo.Entidade modeloEntidade = null;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();                    
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.TipoEntidade, Modelo.TipoEntidade>();
                        Mapper.CreateMap<Servicos.Status, Modelo.Status>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo, senha, modeloEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuarios = negocioUsuario.Consultar(codigo, senha, modeloEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuarios });

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { usuarios });
                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> Consultar(String codigo, out Int32 codigoRetorno)
        {
            return this.Consultar(codigo, null, null, out codigoRetorno);
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>      
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> Consultar(String codigo, Entidade entidade, out Int32 codigoRetorno)
        {
            return this.Consultar(codigo, null, entidade, out codigoRetorno);
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> Consultar(Entidade entidade, out Int32 codigoRetorno)
        {
            return this.Consultar(null, null, entidade, out codigoRetorno);
        }

       /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail temporário
        /// </summary>
        /// <param name="emailTemporario">E-mail temporário do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public Usuario ConsultarPorEmailTemporario(String emailTemporario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuário por e-mail temporário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Usuario modeloUsuario = null;
                    var usuario = default(Servicos.Usuario);
                    Modelo.Entidade modeloEntidade = new Modelo.Entidade {
                        Codigo = codigoEntidade,
                        GrupoEntidade = new Modelo.GrupoEntidade { Codigo = codigoGrupoEntidade }
                    };

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { emailTemporario, modeloEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuario = negocioUsuario.ConsultarPorEmailTemporario(emailTemporario, modeloEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuario });

                    // Converte Business Entity para Data Contract Entity
                    usuario = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario);
                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuario });
                    return usuario;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os usuários por CPF e por Status
        /// </summary>
        /// <param name="emailUsuario"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="status"></param>
        /// <param name="codigoRetorno"></param>
        /// <returns></returns>
        public Usuario[] ConsultarPorCpfPrincipalPorStatus(long cpf, Int32 codigoGrupoEntidade, Int32 codigoEntidade, int[] status, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuário por CPF"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Usuario[] modeloUsuario = null;
                    Modelo.Entidade modeloEntidade = null;

                    var usuario = default(Servicos.Usuario[]);
                    if (codigoEntidade > 0)
                    {
                        modeloEntidade = new Modelo.Entidade
                        {
                            Codigo = codigoEntidade,
                            GrupoEntidade = new Modelo.GrupoEntidade { Codigo = codigoGrupoEntidade }
                        };
                    }

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { cpf = cpf, modeloEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuario = negocioUsuario.ConsultarPorCpfPrincipalPorStatus(cpf, modeloEntidade, status, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuario });

                    // Converte Business Entity para Data Contract Entity
                    usuario = Mapper.Map<Modelo.Usuario[], Servicos.Usuario[]>(modeloUsuario);
                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuario });
                    return usuario;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public Usuario[] ConsultarPorEmailPrincipalPorStatus(String emailUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, int[] status, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuário por e-mail principal"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Usuario[] modeloUsuario = null;
                    Modelo.Entidade modeloEntidade = null;

                    var usuario = default(Servicos.Usuario[]);
                    if (codigoEntidade > 0)
                    {
                        modeloEntidade = new Modelo.Entidade
                        {
                            Codigo = codigoEntidade,
                            GrupoEntidade = new Modelo.GrupoEntidade { Codigo = codigoGrupoEntidade }
                        };
                    }

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { emailTemporario = emailUsuario, modeloEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuario = negocioUsuario.ConsultarPorEmailPrincipalPorStatus(emailUsuario, modeloEntidade, status, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuario });

                    // Converte Business Entity para Data Contract Entity
                    usuario = Mapper.Map<Modelo.Usuario[], Servicos.Usuario[]>(modeloUsuario);
                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuario });
                    return usuario;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public Usuario ConsultarPorEmailPrincipal(String emailUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuário por e-mail principal"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Usuario modeloUsuario = null;
                    Modelo.Entidade modeloEntidade = null;

                    var usuario = default(Servicos.Usuario);
                    if (codigoEntidade > 0)
                    {
                        modeloEntidade = new Modelo.Entidade
                        {
                            Codigo = codigoEntidade,
                            GrupoEntidade = new Modelo.GrupoEntidade { Codigo = codigoGrupoEntidade }
                        };
                    }

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { emailTemporario = emailUsuario, modeloEntidade });

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuario = negocioUsuario.ConsultarPorEmailPrincipal(emailUsuario, modeloEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuario });

                    // Converte Business Entity para Data Contract Entity
                    usuario = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario);
                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuario });
                    return usuario;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        
        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<Usuario> Consultar(out Int32 codigoRetorno)
        {
            return this.Consultar(null, null, null, out codigoRetorno);
        }

        /// <summary>
        /// Retorna os dados de usuário e estabelecimento
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Servicos.Usuario</returns>
        public Servicos.Usuario Consultar(Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario, out int codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela atualização das permissões de usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                codigoRetorno = 0;

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoEntidade, codigoUsuario, _codigoRetorno = codigoRetorno });
                    Negocio.Usuario negocio = new Negocio.Usuario();
                    Modelo.Usuario resultado = negocio.Consultar(codigoGrupoEntidade, codigoEntidade, codigoUsuario, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { resultado });

                    if (!object.ReferenceEquals(resultado, null))
                    {
                        Log.GravarLog(EventoLog.FimServico, new { resultado });

                        Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                        Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                        Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                        Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                        Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                        Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                        Servicos.Usuario usuario = Mapper.Map<Servicos.Usuario>(resultado);

                        if (!object.ReferenceEquals(usuario, null))
                        {
                            List<Menu> menuNavegacao = this.ConsultarMenu(codigoUsuario, codigoGrupoEntidade, codigoEntidade, resultado.CodigoIdUsuario);
                            usuario.Menu = menuNavegacao.ToArray();

                            List<Pagina> paginasPermissoes = this.ConsultarPermissoes(out codigoRetorno, codigoGrupoEntidade, codigoEntidade, codigoUsuario, resultado.CodigoIdUsuario);
                            if (codigoRetorno == 0 && !object.ReferenceEquals(paginasPermissoes, null))
                                usuario.Paginas = paginasPermissoes.ToArray();
                        }

                        return usuario;
                    }
                    else
                    {
                        String _mensagem = "Código de Erro: " + codigoRetorno.ToString();
                        Log.GravarLog(EventoLog.FimServico, new { _mensagem });
                        return null;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método usado para validar um usuário no Portal Redecard
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário</param>
        /// <param name="senhaCriptografada">Senha do usuário criptografada</param>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <returns>Número de retorno</returns>
        public Int32 Validar(Int32 codigoGrupoEntidade, Int32 codigoEntidade, String codigoUsuario, String senhaCriptografada, Boolean pvKomerci)
        {
            using (Logger Log = Logger.IniciarLog("Método usado para validar o usuário especificado no Portal Redecard"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoGrupoEntidade, codigoEntidade, codigoUsuario, senhaCriptografada, pvKomerci });

                    Negocio.Usuario _usuario = new Negocio.Usuario();
                    var result = _usuario.Validar(codigoGrupoEntidade, codigoEntidade, codigoUsuario, senhaCriptografada, pvKomerci);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Usuario</returns>
        public Usuario ConsultarPorID(Int32 codigoIdUsuario, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.Usuario modeloUsuario = null;
                    var usuarios = new List<Servicos.Usuario>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });

                    var negocioUsuario = new Negocio.Usuario();

                    modeloUsuario = negocioUsuario.ConsultarPorID(codigoIdUsuario, out codigoRetorno);

                    Log.GravarLog(EventoLog.FimServico, new { modeloUsuario });

                    // Converte Business Entity para Data Contract Entity
                    return Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario); ;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o menu de navegação do usuário no Portal de Serviços
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Listagem de Menu</returns>
        public List<Menu> ConsultarMenu(String codigoUsuario, Int32 codigoGrupoEntidade, Int32 codigoEntidade, Int32 codigoIdUsuario)
        {
            try
            {
                Negocio.Menu menuInfo = new Negocio.Menu();
                List<Modelo.Menu> menuItems = menuInfo.Consultar(codigoUsuario, codigoGrupoEntidade, codigoEntidade, codigoIdUsuario);
                List<Menu> servicosMenu = new List<Menu>();

                if (menuItems.Count > 0)
                {
                    // Converter de Modelo.Menu para Servico.Menu

                    foreach (Modelo.Menu item in menuItems)
                    {
                        Servicos.Menu root = new Servicos.Menu()
                        {
                            Codigo = item.Codigo,
                            Texto = item.Texto,
                            Observacoes = item.Observacoes,
                            FlagMenu = item.FlagMenu,
                            ServicoBasico = item.ServicoBasico,
                            FlagFooter = item.FlagFooter
                        };

                        item.Paginas.ForEach(delegate(Modelo.PaginaMenu pagina)
                        {
                            root.Paginas.Add(new PaginaMenu()
                            {
                                TextoBotao = pagina.TextoBotao,
                                Url = pagina.Url,
                                Navegacao = pagina.Navegacao
                            });
                        });

                        if (item.Items.Count > 0)
                            this.CarregarMenuFilhos(root, item);

                        servicosMenu.Add(root);
                    }
                }

                return servicosMenu;
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Método de consulta de usuários para reenvio do Welcome
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários para reenvio do Welcome</returns>
        public List<Servicos.Usuario> ConsultarReenvioWelcome(out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método de consulta de usuários para reenvio do Welcome"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    List<Modelo.Usuario> modeloUsuarios = null;
                    var usuarios = new List<Servicos.Usuario>();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var negocioUsuario = new Negocio.Usuario();
                    modeloUsuarios = negocioUsuario.ConsultarReenvioWelcome(out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuarios });


                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.Usuario, Servicos.Usuario>(modeloUsuario));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { usuarios });

                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>listagem de UsuarioHash</returns>
        public List<Servicos.UsuarioHash> ConsultarHash(Int32? codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status? status, Guid? hash, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método de consulta de hash para envio de e-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    var usuariosHash = new List<Servicos.UsuarioHash>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var negocioUsuario = new Negocio.Usuario();
                    var modeloUsuariosHash = negocioUsuario.ConsultarHash(codigoIdUsuario, status, hash, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuariosHash });

                    Log.GravarLog(EventoLog.FimServico, new { modeloUsuariosHash });

                    foreach (var usuarioHash in modeloUsuariosHash)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuariosHash.Add(Mapper.Map<Modelo.UsuarioHash, Servicos.UsuarioHash>(usuarioHash));
                    }

                    return usuariosHash;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioHash</returns>
        public List<Servicos.UsuarioHash> ConsultarHash(out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método de consulta de hash para envio de e-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    var usuariosHash = new List<Servicos.UsuarioHash>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var negocioUsuario = new Negocio.Usuario();
                    var modeloUsuariosHash = negocioUsuario.ConsultarHash(null, null, null, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuariosHash });

                    Log.GravarLog(EventoLog.FimServico, new { modeloUsuariosHash });

                    foreach (var usuarioHash in modeloUsuariosHash)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuariosHash.Add(Mapper.Map<Modelo.UsuarioHash, Servicos.UsuarioHash>(usuarioHash));
                    }

                    return usuariosHash;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Reinicia o hash de confirmação de e-mail.
        /// Exclui hash anterior (caso exista para o usuário) e insere um novo hash.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="status">Status</param>
        /// <param name="emailExpira">Indica se o e-mail expira</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Hash</returns>
        public Servicos.UsuarioHash ReiniciarHash(Int32 codigoIdUsuario, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método de reinicio de hash de envio de e-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    var modeloUsuarioHash = new Negocio.Usuario().ReiniciarHash(
                        codigoIdUsuario, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuarioHash });

                    var usuarioHash = Mapper.Map<Servicos.UsuarioHash>(modeloUsuarioHash);

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, usuarioHash });

                    return usuarioHash;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método para inserir hash na base de dados a partir da camada de serviço
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracao">Dias corridos para expiração do hash</param>
        /// <param name="dataGeracaoHash">Data customizada em que o hash foi gerado</param>
        /// <returns></returns>
        public Guid InserirHash(Int32 codigoIdUsuario, Redecard.PN.Comum.Enumerador.Status status, Double diasExpiracao, DateTime? dataGeracaoHash)
        {
            using (Logger Log = Logger.IniciarLog("Método para inserção de hash de envio de e-mail"))
            {
                try
                {
                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    Guid hash = new Negocio.Usuario().InserirHash(codigoIdUsuario, status, diasExpiracao, dataGeracaoHash);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { hash });
                    Log.GravarLog(EventoLog.FimServico, new { hash });

                    return hash;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Converte objeto Modelo.Menu para Servicos.Menu
        /// </summary>
        /// <param name="root">Objeto Servicos.Menu</param>
        /// <param name="item">Objeto Modelo.Menu</param>
        private void CarregarMenuFilhos(Servicos.Menu root, Modelo.Menu item)
        {
            foreach (Modelo.Menu menuItem in item.Items)
            {
                Servicos.Menu subItem = new Servicos.Menu()
                {
                    Codigo = menuItem.Codigo,
                    Texto = menuItem.Texto,
                    Observacoes = menuItem.Observacoes,
                    FlagMenu = menuItem.FlagMenu,
                    FlagFooter = menuItem.FlagFooter
                };

                menuItem.Paginas.ForEach(delegate(Modelo.PaginaMenu pagina)
                {
                    subItem.Paginas.Add(new PaginaMenu()
                    {
                        TextoBotao = pagina.TextoBotao,
                        Url = pagina.Url,
                        Navegacao = pagina.Navegacao
                    });
                });

                if (menuItem.Items.Count > 0)
                    this.CarregarMenuFilhos(subItem, menuItem);

                root.Items.Add(subItem);
            }
        }

        #endregion

        #region Métodos dos usuários komerci

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários do Komerci</returns>
        public List<UsuarioKomerci> ConsultarKomerci(String codigo, Entidade entidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar usuários do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    List<Modelo.UsuarioKomerci> modeloUsuarios = null;
                    var usuarios = new List<Servicos.UsuarioKomerci>();
                    Modelo.Entidade modeloEntidade = null;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.UsuarioKomerci, Servicos.UsuarioKomerci>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo, modeloEntidade });
                    var negocioUsuario = new Negocio.UsuarioKomerci();
                    modeloUsuarios = negocioUsuario.Consultar(codigo, modeloEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuarios });

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.UsuarioKomerci, Servicos.UsuarioKomerci>(modeloUsuario));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { usuarios });


                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }
        
        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param> 
        /// <returns>Listagem de usuários</returns>
        public List<UsuarioKomerci> ConsultarKomerci(String codigo, out Int32 codigoRetorno)
        {
            return this.ConsultarKomerci(codigo, null, out codigoRetorno);
        }

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public List<UsuarioKomerci> ConsultarKomerci(Entidade entidade, out Int32 codigoRetorno)
        {
            return this.ConsultarKomerci(null, entidade, out codigoRetorno);
        }

        /// <summary>
        /// Consultar usuários do Komerci
        /// </summary>
        /// <returns>Listagem de usuários</returns>
        public List<UsuarioKomerci> ConsultarKomerci(out Int32 codigoRetorno)
        {
            return this.ConsultarKomerci(null, null, out codigoRetorno);
        }

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
        public Int32 AtualizarKomerci(UsuarioKomerci usuario, String senha)
        {
            using (Logger Log = Logger.IniciarLog("Alteração de senha do Usuário do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.UsuarioKomerci negocioUsuario = new Negocio.UsuarioKomerci();

                    Mapper.CreateMap<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>();
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.Menu, Modelo.Menu>();
                    Mapper.CreateMap<Servicos.Pagina, Modelo.Pagina>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    Modelo.UsuarioKomerci usuarioKomerci =
                            Mapper.Map<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>(usuario);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuarioKomerci, senha });
                    var result = negocioUsuario.Atualizar(usuarioKomerci, senha);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public Int32 InserirKomerci(UsuarioKomerci usuario, String senha)
        {
            using (Logger Log = Logger.IniciarLog("Inclusão de usuário do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.UsuarioKomerci negocioUsuario = new Negocio.UsuarioKomerci();

                    Mapper.CreateMap<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>();
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.Menu, Modelo.Menu>();
                    Mapper.CreateMap<Servicos.Pagina, Modelo.Pagina>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    Modelo.UsuarioKomerci usuarioKomerci =
                            Mapper.Map<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>(usuario);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuarioKomerci, senha });
                    var result = negocioUsuario.Inserir(usuarioKomerci, senha);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public Int32 ExcluirKomerciEmLote(String codigos, Servicos.Entidade entidade)
        {
            using (Logger Log = Logger.IniciarLog("Exclui usuário(s) do Komerci em lote."))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.UsuarioKomerci();

                    Modelo.Entidade modeloEntidade = null;

                    // Convert Business Entity para Data Contract Entity
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigos, modeloEntidade });
                    var result = negocioUsuario.ExcluirEmLote(codigos, modeloEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    // Exclui usuários
                    return result;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta login KO usuários do sistema Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="senhaUsuario">Senha do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Código de retorno</returns>
        public void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Login KO usuários do sistema Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.UsuarioKomerci();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoUsuario });

                    negocioUsuario.AutenticarUsuarioKO(codigoEntidade, codigoUsuario, senhaUsuario, out codigoRetorno, out mensagemRetorno);
                    
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, mensagemRetorno });
                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, mensagemRetorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos de tratamento de erro no login

        /// <summary>
        /// Consulta erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <returns>Modelo TrataErroUsuarioLogin</returns>
        public Servicos.TrataErroUsuarioLogin ConsultarErroUsuarioLogin(String chave)
        {
            Modelo.TrataErroUsuarioLogin modeloTrataErroUsuarioLogin = null;
            var trataErroUsuarioLogin = new Servicos.TrataErroUsuarioLogin();
            using (Logger Log = Logger.IniciarLog("Consultar erro usuário login"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { chave });
                    var negocioEntidade = new Negocio.Usuario();


                    modeloTrataErroUsuarioLogin = negocioEntidade.ConsultarErroUsuarioLogin(chave);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { modeloTrataErroUsuarioLogin });

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.TrataErroUsuarioLogin, Servicos.TrataErroUsuarioLogin>();

                    var result = Mapper.Map<Modelo.TrataErroUsuarioLogin, Servicos.TrataErroUsuarioLogin>(modeloTrataErroUsuarioLogin);
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    // Converte Business Entity para Data Contract Entity
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Inseri erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <param name="codigo">Código de retorno do erro</param>
        public void InserirErroUsuarioLogin(String chave, Int32 codigo)
        {
            using (Logger Log = Logger.IniciarLog("Iniserir erro usuário login"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { chave, codigo });
                    var negocioEntidade = new Negocio.Usuario();
                    negocioEntidade.InserirErroUsuarioLogin(chave, codigo);

                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);


                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui erro do login do usuário - Utilizado quando o usuário erra o login
        /// </summary>
        /// <param name="chave">Chave - GrupoEntidade;PV;usuario - Ex. 1;10528083;usuariopci62</param>
        /// <param name="codigo">Código de retorno do erro</param>
        public void ExcluirErroUsuarioLogin(String chave, Int32 codigo)
        {
            using (Logger Log = Logger.IniciarLog("Excluir erro usuário login"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { chave, codigo });
                    var negocioEntidade = new Negocio.Usuario();
                    negocioEntidade.ExcluirErroUsuarioLogin(chave, codigo);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico);

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos da Confirmação positiva

        /// <summary>
        /// Incrementa a quantidade de confirmações positivas inválidas em 1 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 IncrementarQuantidadeConfirmacaoPositiva(int codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Incrementa a quantidade de confirmações positivas inválidas em 1 "))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });
                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.IncrementarQuantidadeConfirmacaoPositiva(codigoIdUsuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        public Int32 ReiniciarQuantidadeConfirmacaoPositiva(int codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza a quantidade de confirmações positivas inválidas para 0 "))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario});

                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.ReiniciarQuantidadeConfirmacaoPositiva(codigoIdUsuario);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Incrementa a quantidade de senhas inválidas em 1 para o usuário especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        public Int32 IncrementarQuantidadeSenhaInvalida(int codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Incrementa a quantidade de senhas inválidas em 1 para o usuário especificado nos paramêtros"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });

                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.IncrementarQuantidadeSenhaInvalida(codigoIdUsuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza a quantidade de senhas inválidas para 0 para o usuário
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        public Int32 ReiniciarQuantidadeSenhaInvalida(Int32 codigoIdUsuario)
        {
            using (Logger Log = Logger.IniciarLog("Atualiza a quantidade de senhas inválidas para 0 para o usuário"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoIdUsuario });

                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.ReiniciarQuantidadeSenhaInvalida(codigoIdUsuario);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="cnpjEstabelecimento">CNPJ da entidade</param>
        /// <returns>Código de retorno</returns>
        public Int32 ValidarConfirmacaoPositivaObrigatoria(Int32 codigoEntidade, Decimal cnpjEstabelecimento)
        {
            using (Logger Log = Logger.IniciarLog("Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoEntidade,
                        cnpjEstabelecimento
                    });

                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.ValidarConfirmacaoPositivaObrigatoria(
                         codigoEntidade,
                         cnpjEstabelecimento);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Código de retorno</returns>
        public Int32 ValidarConfirmacaoPositivaVariavel(Int32 codigoEntidade, List<Servicos.Pergunta> perguntas, out List<Servicos.Pergunta> perguntasIncorretas)
        {
            using (Logger Log = Logger.IniciarLog("Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.Usuario negocio = new Negocio.Usuario();

                    Mapper.CreateMap<Servicos.Pergunta, Modelo.Pergunta>();
                    Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();
                    List<Modelo.Pergunta> _perguntas = new List<Modelo.Pergunta>();
                    var _perguntasIncorretas = default(List<Modelo.Pergunta>);

                    foreach (Servicos.Pergunta pergunta in perguntas)
                    {
                        _perguntas.Add(Mapper.Map<Modelo.Pergunta>(pergunta));
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoEntidade,
                        _perguntas
                    });

                    var result = negocio.ValidarConfirmacaoPositivaVariavel(
                         codigoEntidade,
                         (_perguntas.Count > 0 ? _perguntas : null),
                         out _perguntasIncorretas);

                    perguntasIncorretas = Mapper.Map<List<Servicos.Pergunta>>(_perguntasIncorretas);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { result, perguntasIncorretas });

                    Log.GravarLog(EventoLog.FimServico, new { result, perguntasIncorretas });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços com múltiplas entidades.
        /// Caso retorne 0 a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Códigos da entidades</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Dicionário contendo listas de perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Dicionário com todos os retornos relacionado a cada entidade validada</returns>
        public bool ValidarConfirmacaoPositivaVariavel(Int32[] codigoEntidades, List<Servicos.Pergunta> perguntas, out Dictionary<Int32, List<Modelo.Pergunta>> perguntasIncorretas)
        {
            using (Logger Log = Logger.IniciarLog("Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.Usuario negocio = new Negocio.Usuario();

                    Mapper.CreateMap<Servicos.Pergunta, Modelo.Pergunta>();
                    Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();
                    List<Modelo.Pergunta> _perguntas = new List<Modelo.Pergunta>();
                    var _perguntasIncorretas = default(Dictionary<Int32, List<Modelo.Pergunta>>);

                    foreach (Servicos.Pergunta pergunta in perguntas)
                    {
                        _perguntas.Add(Mapper.Map<Modelo.Pergunta>(pergunta));
                    }

                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        codigoEntidades,
                        _perguntas
                    });

                    var result = negocio.ValidarConfirmacaoPositivaVariavel(
                         codigoEntidades,
                         (_perguntas.Count > 0 ? _perguntas : null),
                         out _perguntasIncorretas);

                    perguntasIncorretas = Mapper.Map<Dictionary<Int32, List<Modelo.Pergunta>>>(_perguntasIncorretas);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result, perguntasIncorretas });
                    Log.GravarLog(EventoLog.FimServico, new { result, perguntasIncorretas });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Verifica se o CPF/CNPJ informado paz parte da relação dos sócios vinculados ao PV informado
        /// </summary>
        /// <param name="codigoEntidade">Codigo da entidade (número do PV)</param>
        /// <param name="cpfCnpjSocio">CPF/CNPJ do sócio a ser verificado</param>
        /// <returns>
        ///     TRUE: CPF/CNPJ consta na relação dos sócios relacionados ao PV
        ///     FALSE: CPF/CNPJ não consta na relação dos sócios relacionados ao PV
        /// </returns>
        public Boolean ValidarCpfCnpjSocio(Int32 codigoEntidade, Int64 cpfCnpjSocio)
        {
            using (Logger Log = Logger.IniciarLog("Valida se CPF/CNPJ do sócio está relacionado ao PV informado"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, cpfCnpjSocio });
                    Negocio.Usuario negocio = new Negocio.Usuario();
                    var result = negocio.ValidarCpfCnpjSocio(codigoEntidade, cpfCnpjSocio);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });
                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Métodos dos usuários komerci EC

        /// <summary>
        /// Consulta usuário komerci EC
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários komerci EC</returns>
        public List<UsuarioKomerci> ConsultarKomerciEC(String codigo, Entidade entidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Método responsável pela atualização das permissões de usuário no Sybase"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {

                    List<Modelo.UsuarioKomerci> modeloUsuarios = null;
                    var usuarios = new List<Servicos.UsuarioKomerci>();
                    Modelo.Entidade modeloEntidade = null;

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.UsuarioKomerci, Servicos.UsuarioKomerci>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigo, modeloEntidade });

                    var negocioUsuario = new Negocio.UsuarioKomerci();
                    modeloUsuarios = negocioUsuario.ConsultarUsuarioEC(codigo, modeloEntidade, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloUsuarios });

                    foreach (var modeloUsuario in modeloUsuarios)
                    {
                        // Converte Business Entity para Data Contract Entity
                        usuarios.Add(Mapper.Map<Modelo.UsuarioKomerci, Servicos.UsuarioKomerci>(modeloUsuario));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { usuarios });

                    return usuarios;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta usuário komerci EC
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários komerci EC</returns>
        public List<UsuarioKomerci> ConsultarKomerciEC(String codigo, out Int32 codigoRetorno)
        {
            return this.ConsultarKomerciEC(codigo, null, out codigoRetorno);
        }

        /// <summary>
        /// Consulta usuário komerci EC
        /// </summary>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários komerci EC</returns>
        public List<UsuarioKomerci> ConsultarKomerciEC(Entidade entidade, out Int32 codigoRetorno)
        {
            return this.ConsultarKomerciEC(null, entidade, out codigoRetorno);
        }

        /// <summary>
        /// Consulta usuário komerci EC
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários komerci EC</returns>
        public List<UsuarioKomerci> ConsultarKomerciEC(out Int32 codigoRetorno)
        {
            return this.ConsultarKomerciEC(null, null, out codigoRetorno);
        }

        /// <summary>
        /// Inseri um usuário komerci EC
        /// </summary>
        /// <param name="usuario">Modelo UsuarioKomerci</param>
        /// <returns>Código de retorno</returns>
        public Int32 InserirKomerciEC(UsuarioKomerci usuario)
        {
            using (Logger Log = Logger.IniciarLog("Inserir Komerci EC"))
            {
                Log.GravarLog(EventoLog.InicioServico);


                try
                {
                    Negocio.UsuarioKomerci negocioUsuario = new Negocio.UsuarioKomerci();

                    Mapper.CreateMap<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>();
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.Menu, Modelo.Menu>();
                    Mapper.CreateMap<Servicos.Pagina, Modelo.Pagina>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    Modelo.UsuarioKomerci usuarioKomerci =
                            Mapper.Map<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>(usuario);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuarioKomerci });
                    var result = negocioUsuario.InserirUsuarioEC(usuarioKomerci);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza um usuário komerci EC
        /// </summary>
        /// <param name="usuario">Modelo UsuarioKomerci</param>
        /// <returns>Código de retorno</returns>
        public Int32 AtualizarKomerciEC(UsuarioKomerci usuario)
        {
            using (Logger Log = Logger.IniciarLog("Atualizar Komerci EC"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Negocio.UsuarioKomerci negocioUsuario = new Negocio.UsuarioKomerci();

                    Mapper.CreateMap<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>();
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.Menu, Modelo.Menu>();
                    Mapper.CreateMap<Servicos.Pagina, Modelo.Pagina>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    Modelo.UsuarioKomerci usuarioKomerci =
                            Mapper.Map<Servicos.UsuarioKomerci, Modelo.UsuarioKomerci>(usuario);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { usuarioKomerci });
                    var result = negocioUsuario.AtualizarUsuarioEC(usuarioKomerci);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui usuários komerci EC
        /// </summary>
        /// <param name="codigos">Códigos dos usuários separados por ","</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="nomeResponsavel">Nome do responsável pela exclusão(Origem) Ex: IS, PN</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirKomerciEmLoteEC(String codigos, Entidade entidade, String nomeResponsavel)
        {
            using (Logger Log = Logger.IniciarLog("Excluir Komerci Em Lote EC"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var negocioUsuario = new Negocio.UsuarioKomerci();

                    Modelo.Entidade modeloEntidade = null;

                    // Convert Business Entity para Data Contract Entity
                    if (entidade != null)
                    {
                        Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                        Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                        // Convert Data Contract Entity para Business Entity
                        modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                    }
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigos, modeloEntidade, nomeResponsavel });
                    var result = negocioUsuario.ExcluirEmLoteUsuarioEC(codigos, modeloEntidade, nomeResponsavel);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { result });

                    Log.GravarLog(EventoLog.FimServico, new { result });


                    // Exclui usuários
                    return result;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), ex.Message);
                }
            }
        }

        #endregion
 
        #region Dupla custodia

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
        public Int32 AprovacaoDuplaCustodia(String codigoUsuario,
            Int32 codigoEntidade,
            Int32 codigoGrupoEntidade,
            String nomeResponsavel,
            String tipoManutencao,
            String nomeSistema)
        {
            using (Logger Log = Logger.IniciarLog("Aprovação Dupla Custódia"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {

                    int retorno = 0;

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoUsuario, codigoEntidade, codigoGrupoEntidade, nomeResponsavel, tipoManutencao, nomeSistema});

                    var negocioUsuario = new Negocio.Usuario();
                    retorno = negocioUsuario.AprovacaoDuplaCustodia(codigoUsuario, codigoEntidade, codigoGrupoEntidade, nomeResponsavel, tipoManutencao, nomeSistema);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { retorno });
                    Log.GravarLog(EventoLog.FimServico, new { retorno });

                    return retorno ;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Passo 1 Criação de usuário

        /// <summary>
        /// Verificar Se Usuarios Estao Aguardando Confirmacao
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <returns></returns>
        public int[] VerificarSeUsuariosEstaoAguardandoConfirmacao(string email)
        {
            int[] result;

            using (Logger Log = Logger.IniciarLog("Serviço Get VerificarSeUsuariosEstaoAguardandoConfirmacao"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                try
                {
                    result = negocioUsuario.VerificarSeUsuariosEstaoAguardandoConfirmacao(email);

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Obter Usuario Aguardando Confirmacao Master
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] GetUsuarioAguardandoConfirmacaoMaster(string email)
        {
            Modelo.EntidadeServicoModel[] result;

            using (Logger Log = Logger.IniciarLog("Serviço Get Usuario Aguardando Confirmacao Master"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                try
                {
                    result = negocioUsuario.GetUsuarioAguardandoConfirmacaoMaster(email);

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region Passo 2 criação usuário

        /// <summary>
        /// Cria Usuario Varios Pvs
        /// </summary>
        /// <param name="entidadesSelecionadas"></param>
        /// <param name="emailExpira"></param>
        /// <param name="entidadesPossuemUsuMaster"></param>
        /// <param name="entidadesNPossuemUsuMaster"></param>
        /// <param name="hash"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public bool CriarUsuarioVariosPvs(
            Modelo.EntidadeServicoModel[] entidadesSelecionadas, 
            double emailExpira, 
            out Modelo.EntidadeServicoModel[] entidadesPossuemUsuMaster, 
            out Modelo.EntidadeServicoModel[] entidadesNPossuemUsuMaster, 
            out Guid hash, 
            out int codigoRetorno,
            out String mensagem)
        {
            using (Logger Log = Logger.IniciarLog("Criar Usuario para Varios Pvs"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                try
                {
                    negocioUsuario.CriarUsuarioVariosPvs(entidadesSelecionadas, emailExpira, out entidadesPossuemUsuMaster, out entidadesNPossuemUsuMaster, out hash, out codigoRetorno, out mensagem);
                    return true;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consultar Emails Usu Master
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Modelo.EntidadeServicoModel[] ConsultarEmailsUsuMaster(int[] pvs, long cpf, string email)
        {
            Modelo.EntidadeServicoModel[] result;

            using (Logger Log = Logger.IniciarLog("Criar Usuario para Varios Pvs"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                try
                {
                    result =  negocioUsuario.ConsultarEmailsUsuMaster( pvs,  cpf,  email);

                    return result;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Atualiza Status Por Pvs
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <param name="status"></param>
        public void AtualizarStatusPorPvs(int[] pvs, long cpf, string email, Redecard.PN.Comum.Enumerador.Status status)
        {
            Modelo.EntidadeServicoModel[] result;

            using (Logger Log = Logger.IniciarLog("Atualizar Status por Pvs"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Usuario negocioUsuario = new Negocio.Usuario();

                try
                {
                    negocioUsuario.AtualizarStatusPorPvs(pvs, cpf, email, status);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        /// <summary>
        /// Obtem o retorno da comum
        /// </summary>
        /// <returns></returns>
        public string ObterRetornoComum()
        {
            return Util.ObterRetornoComum();
        }

    }
}