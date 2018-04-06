/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml.Linq;
using Redecard.PN.Comum.HistoricoAtividadeServico;

namespace Redecard.PN.Comum
{   
    /// <summary>
    /// Classe auxiliar para Log e Histórico das Atividades no Portal
    /// </summary>
    public class Historico
    {
        #region [ Propriedades - Comparação ]

        /// <summary>
        /// Campos que apresentaram diferenças em seus valores.        
        /// </summary>
        protected List<String> Campos { get; set; }

        #endregion

        #region [ Construtores ]

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Historico()
        {
            this.Campos = new List<String>();
        }

        #endregion

        #region [ Métodos Estáticos ]

        #region [ Métodos Base - Gravação no Histórico de Atividades ]

        /// <summary>
        /// Método base para gravação de um registro de atividade
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Dados da Sessão (usuário responsável)</param>
        /// <param name="codigoAtividade">Código do Tipo de Atividade</param>
        /// <param name="xmlParametrosDetalhes">Detalhes e parâmetros adicionais</param>
        /// <returns>Código do registro criado no histórico de atividades</returns>
        public static Int64 Gravar(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoAtividade,
            params XElement[] xmlParametrosDetalhes)
        {
            return Gravar(codigoAtividade,
                sessaoUsuarioResponsavel.CodigoIdUsuario, 
                sessaoUsuarioResponsavel.NomeUsuario, 
                sessaoUsuarioResponsavel.Email,
                sessaoUsuarioResponsavel.TipoUsuario, sessaoUsuarioResponsavel.CodigoEntidade,
                sessaoUsuarioResponsavel.Funcional, sessaoUsuarioResponsavel.CodigoMatriz,
                xmlParametrosDetalhes);            
        }

        /// <summary>
        /// Método base para gravação de um registro de atividade
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Dados da Sessão (usuário responsável)</param>
        /// <param name="codigoAtividade">Código do Tipo de Atividade</param>
        /// <param name="xmlParametrosDetalhes">Detalhes e parâmetros adicionais</param>
        /// <returns>Código do registro criado no histórico de atividades</returns>
        private static Int64 Gravar(
            Sessao sessaoUsuarioResponsavel,
            Atividade codigoAtividade,
            params XElement[] xmlParametrosDetalhes)
        {
            return Gravar(sessaoUsuarioResponsavel, (Int32) codigoAtividade, xmlParametrosDetalhes);
        }

        /// <summary>
        /// Método base para gravação de um registro de atividade
        /// </summary>
        /// <param name="codigoEntidadeUsuarioResponsavel">Código da entidade do usuário responsável pela atividade</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário responsável pela atividade</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável pela atividade</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável pela atividade</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável pela atividade</param>
        /// <param name="codigoAtividade">Código do Tipo de Atividade</param>
        /// <param name="funcionalOperador">Funcional do operador responsável</param>
        /// <param name="codigoMatrizEntidadeUsuarioResponsavel">Código da matriz da entidade do usuário responsável</param>
        /// <param name="xmlParametrosDetalhes">Detalhes e parâmetros adicionais</param>
        /// <returns>Código do registro criado no histórico de atividades</returns>
        private static Int64 Gravar(
            Atividade codigoAtividade,
            Int32? codigoIdUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32? codigoEntidadeUsuarioResponsavel,
            String funcionalOperador,
            Int32? codigoMatrizEntidadeUsuarioResponsavel,
            params XElement[] xmlParametrosDetalhes)
        {
            return Gravar((Int32)codigoAtividade, codigoIdUsuarioResponsavel, nomeUsuarioResponsavel,
                emailUsuarioResponsavel, perfilUsuarioResponsavel, codigoEntidadeUsuarioResponsavel,
                funcionalOperador, codigoMatrizEntidadeUsuarioResponsavel, xmlParametrosDetalhes);
        }

        /// <summary>
        /// Método base para gravação de um registro de atividade
        /// </summary>
        /// <param name="codigoEntidadeUsuarioResponsavel">Código da entidade do usuário responsável pela atividade</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário responsável pela atividade</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável pela atividade</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável pela atividade</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável pela atividade</param>
        /// <param name="codigoAtividade">Código do Tipo de Atividade</param>
        /// <param name="xmlParametrosDetalhes">Detalhes e parâmetros adicionais</param>
        /// <param name="funcionalOperador">Funcional do operador responsável</param>
        /// <param name="codigoMatrizEntidadeUsuarioResponsavel">Código da matriz da entidade do usuário responsável</param>
        /// <returns>Código do registro criado no histórico de atividades</returns>
        public static Int64 Gravar(
            Int32 codigoAtividade,
            Int32? codigoIdUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32? codigoEntidadeUsuarioResponsavel,
            String funcionalOperador,
            Int32? codigoMatrizEntidadeUsuarioResponsavel,
            params XElement[] xmlParametrosDetalhes)
        {
            var codigoHistoricoAtividade = default(Int64);

            XElement xmlDetalhes = default(XElement);
            if (xmlParametrosDetalhes != null && xmlParametrosDetalhes.Length > 0)
                xmlDetalhes = new XElement("detalhes", xmlParametrosDetalhes.Where(xml => xml != null).ToArray());

            var historico = new HistoricoAtividadeServico.HistoricoAtividade
            {
                CodigoEntidade = codigoEntidadeUsuarioResponsavel,
                CodigoIdUsuario = codigoIdUsuarioResponsavel,
                CodigoTipoAtividade = codigoAtividade,
                Detalhes = xmlDetalhes != null ? xmlDetalhes.ToString() : null,
                EmailUsuario = emailUsuarioResponsavel,
                NomeUsuario = nomeUsuarioResponsavel,
                PerfilUsuario = ObterDescricaoPerfil(perfilUsuarioResponsavel),
                FuncionalOperador = funcionalOperador,
                Ip = ObterIpUsuario(),
                CodigoMatriz = codigoMatrizEntidadeUsuarioResponsavel
            };            

            using (Logger Log = Logger.IniciarLog("Gravação de Histórico"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<HistoricoAtividadeServicoClient>())
                        codigoHistoricoAtividade = ctx.Cliente.GravarHistorico(historico);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }

            return codigoHistoricoAtividade;
        }

        #endregion

        #region [ Histórico - Administração de Usuários ]

        /// <summary>
        /// Criação de usuário através da área de Administração de Usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela criação do novo usuário</param>
        /// <param name="codigoIdUsuarioCriado">Código ID do usuário criado</param>
        /// <param name="nomeUsuarioCriado">Nome do usuário criado</param>
        /// <param name="emailUsuarioCriado">E-mail do usuário criado</param>
        /// <param name="perfilUsuarioCriado">Perfil do usuário criado</param>
        /// <param name="estabelecimentos">Estabelecimentos do usuário criado</param>
        public static void CriacaoUsuario(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoIdUsuarioCriado,
            String nomeUsuarioCriado,
            String emailUsuarioCriado,
            String perfilUsuarioCriado,
            List<Int32> estabelecimentos)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.CriacaoUsuario,
                XmlCodigoIdUsuario(codigoIdUsuarioCriado),
                XmlNomeUsuario(nomeUsuarioCriado),
                XmlEmailUsuario(emailUsuarioCriado),
                XmlPerfilUsuario(perfilUsuarioCriado),
                XmlCampo("pvs", "Estabelecimentos", estabelecimentos.Select(pv => pv.ToString()).Join(", ", " e ")));
        }

        /// <summary>
        /// Alteração de dados de outro usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela alteração dos dados de outro usuário</param>
        /// <param name="codigoIdUsuarioAlterado">Código ID do usuário cujos dados foram alterados</param>
        /// <param name="nomeUsuarioAlterado">Nome do usuário cujos dados foram alterados</param>
        /// <param name="emailUsuarioAlterado">E-mail do usuário cujos dados foram alterados</param>
        /// <param name="perfilUsuarioAlterado">Perfil do usuário cujos dados foram alterados</param>
        /// <param name="dadosAlterados">Lista de dados alterados (nome da informação alterada. Ex: e-mail, celular, nome, etc.)</param>
        public static void AlteracaoDadosOutroUsuario(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoIdUsuarioAlterado,
            String nomeUsuarioAlterado,
            String emailUsuarioAlterado,
            String perfilUsuarioAlterado,
            params String[] dadosAlterados)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.AlteracaoCadastroOutroUsuario,
                XmlCodigoIdUsuario(codigoIdUsuarioAlterado),
                XmlNomeUsuario(nomeUsuarioAlterado),
                XmlEmailUsuario(emailUsuarioAlterado),
                XmlPerfilUsuario(perfilUsuarioAlterado),
                XmlDadosAlterados(dadosAlterados));
        }

        /// <summary>
        /// Desbloqueio de usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pelo desbloqueio</param>
        /// <param name="codigoIdUsuarioDesbloqueado">Código ID do usuário que foi desbloqueado</param>
        /// <param name="nomeUsuarioDesbloqueado">Nome do usuário que foi desbloqueado</param>
        /// <param name="emailUsuarioDesbloqueado">E-mail do usuário que foi desbloqueado</param>
        /// <param name="perfilUsuarioDesbloqueado">Perfil do usuário que foi desbloqueado</param>
        public static void DesbloqueioUsuario(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoIdUsuarioDesbloqueado,
            String nomeUsuarioDesbloqueado,
            String emailUsuarioDesbloqueado,
            String perfilUsuarioDesbloqueado)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.DesbloqueioUsuario,
                XmlCodigoIdUsuario(codigoIdUsuarioDesbloqueado),
                XmlNomeUsuario(nomeUsuarioDesbloqueado),
                XmlEmailUsuario(emailUsuarioDesbloqueado),
                XmlPerfilUsuario(perfilUsuarioDesbloqueado));
        }

        /// <summary>
        /// Exclusão de um usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável por realizar a exclusão</param>
        /// <param name="codigoIdUsuarioExcluido">Código ID do usuário excluido</param>
        /// <param name="nomeUsuarioExcluido">Nome do usuário excluído</param>
        /// <param name="emailUsuarioExcluido">E-mail do usuário excluído</param>
        /// <param name="perfilUsuarioExcluido">Perfil do usuário excluído</param>
        public static void ExclusaoUsuario(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoIdUsuarioExcluido,
            String nomeUsuarioExcluido,
            String emailUsuarioExcluido,
            String perfilUsuarioExcluido)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.ExclusaoUsuario,
                XmlCodigoIdUsuario(codigoIdUsuarioExcluido),
                XmlNomeUsuario(nomeUsuarioExcluido),
                XmlEmailUsuario(emailUsuarioExcluido),
                XmlPerfilUsuario(perfilUsuarioExcluido));
        }

        /// <summary>
        /// Exclusão de um usuário
        /// </summary>
        /// <param name="codigoEntidadeUsuarioResponsavel">Código do PV do usuário responsável por realizar a exclusão</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário responsável por realizar a exclusão</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável por realizar a exclusão</param>
        /// <param name="funcionalResponsavel">Funcional do usuário responsável por realizar a exclusão</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável por realizar a exclusão</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável por realizar a exclusão</param>
        /// <param name="codigoIdUsuarioExcluido">Código ID do usuário excluido</param>
        /// <param name="nomeUsuarioExcluido">Nome do usuário excluído</param>
        /// <param name="emailUsuarioExcluido">E-mail do usuário excluído</param>
        /// <param name="perfilUsuarioExcluido">Perfil do usuário excluído</param>
        public static void ExclusaoUsuario(
            Int32? codigoIdUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32? codigoEntidadeUsuarioResponsavel,
            String funcionalResponsavel,
            Int32 codigoIdUsuarioExcluido,
            String nomeUsuarioExcluido,
            String emailUsuarioExcluido,
            String perfilUsuarioExcluido)
        {
            Gravar(
                Atividade.ExclusaoUsuario,
                codigoIdUsuarioResponsavel,
                nomeUsuarioResponsavel,
                emailUsuarioResponsavel,
                perfilUsuarioResponsavel,
                codigoEntidadeUsuarioResponsavel,
                funcionalResponsavel,
                null,
                XmlCodigoIdUsuario(codigoIdUsuarioExcluido),
                XmlNomeUsuario(nomeUsuarioExcluido),
                XmlEmailUsuario(emailUsuarioExcluido),
                XmlPerfilUsuario(perfilUsuarioExcluido));
        }

        #endregion

        #region [ Histórico - Meu Usuário ]

        /// <summary>
        /// Alteração de dados do próprio usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário</param>        
        /// <param name="dadosAlterados">Lista de dados alterados (nome da informação alterada. Ex: e-mail, celular, nome, etc.)</param>
        public static void AlteracaoDadosUsuario(
            Sessao sessaoUsuarioResponsavel,
            params String[] dadosAlterados)
        {
            Gravar(
               sessaoUsuarioResponsavel,
               Atividade.AlteracaoCadastroProprioUsuario,
               XmlDadosAlterados(dadosAlterados));
        }

        /// <summary>
        /// Alteração de senha do próprio usuário
        /// </summary>
        /// <param name="sessaoUsuario">Sessão do usuário</param>
        public static void AlteracaoSenha(
            Sessao sessaoUsuario)
        {
            Gravar(sessaoUsuario, Atividade.AlteracaoCadastroProprioUsuario,
                XmlDadosAlterados(new[] { "senha" } ));
        }

        /// <summary>
        /// Alteração de e-mail do próprio usuário
        /// </summary>
        /// <param name="sessaoUsuario">Sessão do usuário</param>
        /// <param name="novoEmail">Novo e-mail para o qual o usuário alterou</param>
        public static void AlteracaoEmail(
            Sessao sessaoUsuario, 
            String novoEmail)
        {
            Gravar(sessaoUsuario, Atividade.AlteracaoCadastroProprioUsuario, 
                XmlDadosAlterados(new[] { "e-mail" }), XmlEmailUsuario(novoEmail));
        }

        #endregion

        #region [ Histórico - Aprovação de Acesso ]

        /// <summary>
        /// Solicitação de aprovação de acesso rejeitada
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela rejeição</param>
        /// <param name="codigoIdUsuarioRejeitado">Código ID do usuário cuja solicitação foi rejeitada</param>
        /// <param name="nomeUsuarioRejeitado">Nome do usuário cuja solicitação foi rejeitada</param>
        /// <param name="emailUsuarioRejeitado">E-mail do usuário cuja solicitação foi rejeitada</param>
        /// <param name="perfilUsuarioRejeitado">Perfil do usuário cuja solicitação foi rejeitada</param>
        public static void RejeicaoUsuario(
           Sessao sessaoUsuarioResponsavel,
           Int32 codigoIdUsuarioRejeitado,
           String nomeUsuarioRejeitado,
           String emailUsuarioRejeitado,
           String perfilUsuarioRejeitado)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.AprovacaoRejeicaoUsuario,
                XmlCampo("tipo", "Tipo", "Rejeição"),
                XmlCodigoIdUsuario(codigoIdUsuarioRejeitado),
                XmlNomeUsuario(nomeUsuarioRejeitado),
                XmlEmailUsuario(emailUsuarioRejeitado),
                XmlPerfilUsuario(perfilUsuarioRejeitado));
        }

        /// <summary>
        /// Aprovação de solicitação de acesso
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela aprovação</param>
        /// <param name="codigoIdUsuarioAprovado">Código ID do usuário cuja solicitação foi aprovada</param>
        /// <param name="nomeUsuarioAprovado">Nome do usuário cuja solicitação foi aprovada</param>
        /// <param name="emailUsuarioAprovado">E-mail do usuário cuja solicitação foi aprovada</param>
        /// <param name="perfilUsuarioAprovado">Perfil do usuário cuja solicitação foi aprovada</param>
        public static void AprovacaoUsuario(
           Sessao sessaoUsuarioResponsavel,
           Int32 codigoIdUsuarioAprovado,
           String nomeUsuarioAprovado,
           String emailUsuarioAprovado,
           String perfilUsuarioAprovado)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.AprovacaoRejeicaoUsuario,
                XmlCampo("tipo", "Tipo", "Aprovação"),
                XmlCodigoIdUsuario(codigoIdUsuarioAprovado),
                XmlNomeUsuario(nomeUsuarioAprovado),
                XmlEmailUsuario(emailUsuarioAprovado),
                XmlPerfilUsuario(perfilUsuarioAprovado));
        }

        #endregion

        #region [ Histórico - Área Aberta ]

        /// <summary>
        /// Criação de usuário para Acesso ao Portal (criação de usuário pela área aberta)
        /// </summary>
        /// <param name="codigoIdUsuarioCriado">Código ID do usuário solicitando acesso</param>
        /// <param name="nomeUsuarioCriado">Nome do usuário solicitando acesso</param>
        /// <param name="emailUsuarioCriado">E-mail do usuário solicitando acesso</param>
        /// <param name="perfilUsuarioCriado">Perfil do usuário solicitando acesso</param>
        /// <param name="codigoEntidade">Código da entidade em que o usuário solicita acesso</param>
        /// <param name="pvPossuiMaster">Se PV já possui usuários Master</param>
        /// <param name="pvPossuiUsuarios">Se PV já possui usuários</param>
        public static void CriacaoUsuario(
            Int32 codigoIdUsuarioCriado,
            String nomeUsuarioCriado,
            String emailUsuarioCriado,
            String perfilUsuarioCriado,
            Int32 codigoEntidade,
            Boolean pvPossuiMaster,
            Boolean pvPossuiUsuarios)
        {
            Gravar(
                Atividade.SolicitacaoAcesso,
                codigoIdUsuarioCriado,
                nomeUsuarioCriado,
                emailUsuarioCriado,
                perfilUsuarioCriado,
                codigoEntidade,
                null,
                null,
                XmlCampo("statusUsuario", "Status do usuário", "Já existe"),
                XmlCampo("pvPossuiMaster", "PV já possui usuário Master", pvPossuiMaster ? "Sim" : "Não"),
                XmlCampo("pvPossuiUsuarios", "PV já possui usuários", pvPossuiUsuarios ? "Sim" : "Não"));
        }

        /// <summary>
        /// Solicitação de acesso para o Portal (solicitação de criação de usuário pela área aberta)
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário solicitando acesso</param>
        /// <param name="nomeUsuario">Nome do usuário solicitando acesso</param>
        /// <param name="emailUsuario">E-mail do usuário solicitando acesso</param>
        /// <param name="perfilUsuario">Perfil do usuário solicitando acesso</param>
        /// <param name="codigoEntidade">Código da entidade em que o usuário solicita acesso</param>
        /// <param name="pvPossuiMaster">Se PV já possui usuários Master</param>
        /// <param name="pvPossuiUsuarios">Se PV já possui usuários</param>
        /// <param name="statusUsuario">Status do usuário (Já existe, confirmou e-mail, etc...)</param>
        public static void SolicitacaoCriacaoUsuario(
            Int32? codigoIdUsuario,
            String nomeUsuario,
            String emailUsuario,
            String perfilUsuario,
            Int32 codigoEntidade,
            Boolean pvPossuiMaster,
            Boolean pvPossuiUsuarios,
            String statusUsuario)
        {
            Gravar(
                Atividade.SolicitacaoAcesso,
                codigoIdUsuario,
                nomeUsuario,
                emailUsuario,
                perfilUsuario,
                codigoEntidade,
                null,
                null,
                XmlCampo("statusUsuario", "Status do usuário", statusUsuario),
                XmlCampo("pvPossuiMaster", "PV já possui usuário Master", pvPossuiMaster ? "Sim" : "Não"),
                XmlCampo("pvPossuiUsuarios", "PV já possui usuários", pvPossuiUsuarios ? "Sim" : "Não"));
        }

        /// <summary>
        /// Recuperação de Senha com sucesso
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário que solicitou a recuperação de senha</param>
        /// <param name="nomeUsuario">Nome do usuário que solicitou a recuperação de senha</param>
        /// <param name="emailUsuario">E-mail do usuário que solicitou a recuperação de senha</param>
        /// <param name="perfilUsuario">Perfil do usuário que solicitou a recuperação de senha</param>
        /// <param name="codigoEntidade">Código do Estabelecimento do usuário que solicitou a recuperação de senha</param>
        /// <param name="formaRecuperacao">Forma de recuperação utilizada na Recuperação de Senha</param>
        public static void RecuperacaoSenha(
            Int32 codigoIdUsuario,
            String nomeUsuario,
            String emailUsuario,
            String perfilUsuario,
            Int32 codigoEntidade,
            String formaRecuperacao)
        {
            Gravar(
                Atividade.RecuperacaoSenha,
                codigoIdUsuario,
                nomeUsuario,
                emailUsuario,
                perfilUsuario,
                codigoEntidade,
                null,
                null,
                XmlCampo("formaRecuperacao", "Forma de recuperação", formaRecuperacao));
        }

        /// <summary>
        /// Recuperação de Usuário com sucesso
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário que recuperou seu usuário de login com sucesso</param>
        /// <param name="nomeUsuario">Nome do usuário que recuperou seu usuário de login com sucesso</param>
        /// <param name="emailUsuario">E-mail do usuário que recuperou seu usuário de login com sucesso</param>
        /// <param name="perfilUsuario">Perfil do usuário que recuperou seu usuário de login com sucesso</param>
        /// <param name="codigoEntidade">Código da entidade do usuário que recuperou seu usuário de login com sucesso</param>
        public static void RecuperacaoUsuario(
            Int32 codigoIdUsuario,
            String nomeUsuario,
            String emailUsuario,
            String perfilUsuario,
            Int32 codigoEntidade)
        {
            Gravar(
                Atividade.RecuperacaoUsuario,
                codigoIdUsuario,
                nomeUsuario,
                emailUsuario,
                perfilUsuario,
                codigoEntidade,
                null,
                null);
        }

        /// <summary>
        /// Erro na Confirmação positiva
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário que errou a confirmação positiva</param>
        /// <param name="nomeUsuario">Nome do usuário que errou a confirmação positiva</param>
        /// <param name="emailUsuario">E-mail do usuário que errou a confirmação positiva</param>
        /// <param name="perfilUsuario">Perfil do usuário que errou a confirmação positiva</param>
        /// <param name="codigoEntidade">Código da entidade do usuário que errou a confirmação positiva</param>
        /// <param name="tipoConfirmacaoPositiva">
        /// <para>Tipo de Confirmação positiva:</para>
        /// <para> - Acesso Básico </para>
        /// <para> - Acesso Completo</para>
        /// <para> - Recuperação de Senha</para>
        /// <para> - Recuperação de Usuário</para>
        /// </param>
        /// <param name="dadosIncorretos">Lista de dados incorretos (nome da informação incorreta. Ex: e-mail, celular, nome, etc.)</param>
        public static void ErroConfirmacaoPositiva(
            Int32 codigoIdUsuario,
            String nomeUsuario,
            String emailUsuario,
            String perfilUsuario,
            Int32 codigoEntidade,
            String tipoConfirmacaoPositiva,
            params String[] dadosIncorretos)
        {
            Gravar(
                Atividade.ErroConfirmacaoPositiva,
                codigoIdUsuario,
                nomeUsuario,
                emailUsuario,
                perfilUsuario,
                codigoEntidade,
                null,
                null,
                XmlCampo("tipoConfirmacao", "Tipo", tipoConfirmacaoPositiva),
                XmlCampo("dados", "Dados incorretos", dadosIncorretos.Join(", ", " e ")));
        }

        /// <summary>
        /// Bloqueio do formulário de criação de usuário
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade bloqueada</param>
        /// <param name="codigoIdUsuario">Código ID do usuário que causou bloqueio do formulário</param>
        /// <param name="emailUsuario">E-mail do usuário que causou bloqueio do formulário</param>
        /// <param name="nomeUsuario">Nome do usuário que causou bloqueio do formulário</param>
        /// <param name="perfilUsuario">Perfil do usuário que causou bloqueio do formulário</param>
        public static void BloqueioFormularioSolicitacaoAcesso(
            Int32? codigoIdUsuario,
            String nomeUsuario,
            String emailUsuario,
            String perfilUsuario,
            Int32 codigoEntidade)
        {
            Gravar(
                Atividade.BloqueioFormularioSolicitacaoAcesso,
                codigoIdUsuario,
                nomeUsuario,
                emailUsuario,
                perfilUsuario,
                codigoEntidade,
                null,
                null);
        }

        #endregion

        #region [ Histórico - Login ]

        /// <summary>
        /// Bloqueio de usuário por erro de senha
        /// </summary>
        /// <param name="codigoIdUsuarioBloqueado">Código ID do usuário bloqueado</param>
        /// <param name="nomeUsuarioBloqueado">Nome do usuário bloqueado</param>
        /// <param name="emailUsuarioBloqueado">E-mail do usuário bloqueado</param>
        /// <param name="perfilUsuarioBloqueado">Perfil do usuário bloqueado</param>
        /// <param name="codigoEntidadeUsuarioBloqueado">Código do PV do usuário bloqueado</param>
        public static void BloqueioUsuarioErroSenha(
            Int32 codigoIdUsuarioBloqueado,
            String nomeUsuarioBloqueado,
            String emailUsuarioBloqueado,
            String perfilUsuarioBloqueado,
            Int32 codigoEntidadeUsuarioBloqueado)
        {
            Gravar(
                Atividade.BloqueioUsuarioErroSenha,
                codigoIdUsuarioBloqueado,
                nomeUsuarioBloqueado,
                emailUsuarioBloqueado,
                perfilUsuarioBloqueado,
                codigoEntidadeUsuarioBloqueado,
                null,
                null);
        }

        /// <summary>
        /// Login de usuário no Portal
        /// </summary>
        /// <param name="sessaoUsuario">Sessão do usuário que realizou Login no Portal</param>
        /// <param name="dataCriacao">Data de criação do usuário</param>
        public static void Login(
            Sessao sessaoUsuario,
            DateTime? dataCriacao)
        {
            var xmlDetalhes = new List<XElement>();

            xmlDetalhes.Add(XmlCampo("status", "Status", ObterDescricaoStatus(sessaoUsuario.CodigoStatus)));
            xmlDetalhes.Add(XmlCampo("ultimoAcesso", "Data do último acesso", sessaoUsuario.UltimoAcesso.ToString("dd/MM/yyyy HH:mm:ss")));
            if(dataCriacao.HasValue)
                xmlDetalhes.Add(XmlCampo("dataCriacao", "Data de criação", dataCriacao.Value.ToString("dd/MM/yyyy HH:mm:ss")));

            Gravar(
                sessaoUsuario,
                Atividade.Login,
                xmlDetalhes.ToArray());
        }

        #endregion

        #region [ Histórico - Envio de E-mail ]

        /// <summary>
        /// Envio de e-mail	pelo sistema pela Área Aberta do Portal
        /// (Solicitação de Acesso - Criação de Usuário, Recuperação de Senha, Recuperação de Usuário, etc)
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário</param>        
        /// <param name="tipoEmail">Identificação do tipo de e-mail enviado 
        /// <param name="codigoEntidadeUsuarioResponsavel">Código do Estabelecimento do usuário</param>
        /// <param name="codigoIdUsuarioResponsavel">Código ID do usuário que receberá o e-mail</param>
        /// <param name="emailUsuarioResponsavel">E-mail do usuário responsável pelo evento de envio de e-mail</param>
        /// <param name="nomeUsuarioResponsavel">Nome do usuário responsável pelo evento de envio de e-mail</param>
        /// <param name="perfilUsuarioResponsavel">Perfil do usuário responsável pelo evento de envio de e-mail</param>
        /// <param name="funcionalUsuarioResponsavel">Número da funcional do usuário responsável (no caso de Usuário Atendimento)</param>
        /// (Solicitação de Acesso, Confirmação, Informativo de Recuperação de Senha, etc...)</param>
        public static void EnvioEmail(
            Int32? codigoIdUsuarioResponsavel,
            String nomeUsuarioResponsavel,
            String emailUsuarioResponsavel,
            String perfilUsuarioResponsavel,
            Int32? codigoEntidadeUsuarioResponsavel,
            String funcionalUsuarioResponsavel,
            String tipoEmail,
            String emailDestinatario)
        {
            Gravar(
                Atividade.EnvioEmail,
                codigoIdUsuarioResponsavel,
                nomeUsuarioResponsavel,
                emailUsuarioResponsavel,
                perfilUsuarioResponsavel,
                codigoEntidadeUsuarioResponsavel,
                funcionalUsuarioResponsavel, //funcional
                null, //matriz
                XmlCampo("destinatario", "E-mail", emailDestinatario),
                XmlCampo("tipoEmail", "Tipo", tipoEmail),
                XmlCampo("pvs", "Estabelecimentos", codigoEntidadeUsuarioResponsavel.ToString()));
        }

        /// <summary>
        /// Envio de e-mail	pelo sistema para usuários autenticados
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário</param>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável</param>
        /// <param name="tipoEmail">Identificação do tipo de e-mail enviado 
        /// (Solicitação de Acesso, Confirmação, Informativo de Recuperação de Senha, etc...)</param>
        /// <param name="pvDestinatario">PV do destinatário</param>
        public static void EnvioEmail(
            Sessao sessaoUsuarioResponsavel,
            String tipoEmail,
            String emailDestinatario,
            List<Int32> pvDestinatario)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.EnvioEmail,
                XmlCampo("destinatario", "E-mail", emailDestinatario),
                XmlCampo("tipoEmail", "Tipo", tipoEmail),
                XmlCampo("pvs", "Estabelecimentos", pvDestinatario.Select(pv => pv.ToString()).Join(", ", " e ")));
        }

        #endregion

        #region [ Histórico - Área Fechada ]

        /// <summary>
        /// Alteração de cadastro de um estabelecimento
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela alteração dos dados do estabelecimento</param>
        /// <param name="dadosAlterados">Lista de dados alterados (nome da informação alterada. Ex: e-mail, celular, nome, etc.)</param>
        public static void AlteracaoDadosEstabelecimento(
            Sessao sessaoUsuarioResponsavel,
            params String[] dadosAlterados)
        {
            Gravar(
                sessaoUsuarioResponsavel,
                Atividade.AlteracaoCadastroEstabelecimento,
                XmlDadosAlterados(dadosAlterados));
        }
        
        /// <summary>
        /// Solicitação de Acesso Completo com sucesso
        /// </summary>
        /// <param name="sessaoUsuario">Sessão do usuário que realizou a solicitação de acesso completo com sucesso</param>
        public static void LiberacaoAcessoCompleto(
            Sessao sessaoUsuario)
        {
            Gravar(
                sessaoUsuario,
                Atividade.LiberacaoAcessoCompleto);
        }
               
        /// <summary>
        /// Realização de Serviço
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela realização do serviço</param>
        /// <param name="nomeServicoRealizado">
        /// Serviço realizado: RAV, Solicitação de Material, 2ª Via Extrato, 
        /// Cancelamento de Vendas, Comprovação de Vendas, Contratação de Consulta Cheque
        /// </param>
        public static void RealizacaoServico(
            Sessao sessaoUsuarioResponsavel,
            String nomeServicoRealizado)
        {
            Gravar(sessaoUsuarioResponsavel,
                 Atividade.RealizacaoServico,
                 XmlCampo("servico", "Serviço", nomeServicoRealizado));
        }

        /// <summary>
        /// Desbloqueio do formulário de criação de usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável</param>
        public static void DesbloqueioFormularioSolicitacaoAcesso(
            Sessao sessaoUsuarioResponsavel)
        {
            Gravar(sessaoUsuarioResponsavel, Atividade.DesbloqueioFormularioSolicitacaoAcesso);
        }
       
        #endregion

        #region [ Geração de Elementos Xml ]

        /// <summary>
        /// Gera XML padrão para armazenar o Código ID do Usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <returns>XML</returns>
        private static XElement XmlCodigoIdUsuario(Int32 codigoIdUsuario)
        {
            return new XElement("codigo", codigoIdUsuario);
        }

        /// <summary>
        /// Gera XML padrão para armazenar o nome do Usuário
        /// </summary>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <returns>XML</returns>
        private static XElement XmlNomeUsuario(String nomeUsuario)
        {
            return new XElement("nome", new XAttribute("label", "Nome"), nomeUsuario);
        }

        /// <summary>
        /// Gera XML padrão para armazenar o e-mail do Usuário
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuário</param>
        /// <returns>XML</returns>
        private static XElement XmlEmailUsuario(String emailUsuario)
        {
            return new XElement("email", new XAttribute("label", "E-mail"), emailUsuario);
        }

        /// <summary>
        /// Gera XML padrão para armazenar o perfil do Usuário
        /// </summary>
        /// <param name="perfilUsuario">Perfil do usuário</param>
        /// <returns>XML</returns>
        private static XElement XmlPerfilUsuario(String perfilUsuario)
        {
            return new XElement("perfil", new XAttribute("label", "Perfil"), ObterDescricaoPerfil(perfilUsuario));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        private static XElement XmlDadosAlterados(String[] dados)
        {
            var xmlDados = default(XElement);
            if (dados != null && dados.Length > 0)
                xmlDados = new XElement("dados", new XAttribute("label", "Dados alterados"),
                    dados.Join(", ", " e "));
            return xmlDados;
        }

        private static XElement XmlCampo(String tagName, String label, String valor)
        {
            return new XElement(tagName, new XAttribute("label", label), valor);
        }

        #endregion

        #region [ Métodos Privados / Auxiliares ]

        /// <summary>
        /// Obtém o IP da requisição atual
        /// </summary>
        private static String ObterIpUsuario()
        {
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                var request = httpContext.Request;
                if (request != null)
                    return (request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                               request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
            }
            return null;
        }

        /// <summary>
        /// Obtém a descrição do perfil
        /// </summary>
        private static String ObterDescricaoPerfil(String perfil)
        {
            if (String.Compare("M", perfil, true) == 0)
                return "Completo (Master)";
            else if (String.Compare("P", perfil, true) == 0)
                return "Personalizado";
            else if (String.Compare("B", perfil, true) == 0)
                return "Acesso Básico";
            else
                return perfil;
        }

        /// <summary>
        /// Obtém a descrição resumida do Status
        /// </summary>
        private static String ObterDescricaoStatus(Enumerador.Status status)
        {
            switch (status)
            {
                case Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                case Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                case Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                    return "Aguardando Confirmação";
                case Enumerador.Status.UsuarioAtivo:
                case Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                case Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                    return "Ativo";
                case Enumerador.Status.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha:
                case Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha:
                case Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                case Enumerador.Status.UsuarioBloqueadoSenhaInvalida:
                    return "Bloqueado";
                case Enumerador.Status.EntidadeAtiva:
                    return "Ativa";
                case Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva:
                    return "Bloqueada";
                default:
                    return status.ToString();
            }
        }

        #endregion       
    
        #endregion

        #region [ Métodos para implementação de "Fluent Interface" - Auxiliar para comparação de Atividades de Alteração ]
       
        /// <summary>
        /// Compara as propriedades entre duas instâncias de um mesmo modelo, identificando 
        /// quais possuem valores distintos.
        /// </summary>
        /// <typeparam name="T">Tipo do modelo</typeparam>
        /// <param name="obj1">Instância 1</param>
        /// <param name="obj2">Instância 2</param>
        /// <returns>Fluent interface de comparação</returns>
        public static HistoricoComparacao<T> CompararModelos<T>(T obj1, T obj2)
        {
            return new HistoricoComparacao<T>(obj1, obj2);
        }

        /// <summary>
        /// Fluent interface para comparação de campos.
        /// </summary>
        public static Historico Comparar
        {
            get { return new Historico(); }
        }

        /// <summary>
        /// Fluent interface para comparação dos valores entre duas variáveis.
        /// </summary>
        /// <typeparam name="T">Tipo da variável</typeparam>
        /// <param name="valor1">Valor 1</param>
        /// <param name="valor2">Valor 2</param>
        /// <param name="nomeCampo">Nome descritivo da variável</param>
        /// <returns>Fluent interface para comparação de campos.</returns>
        public Historico Campo<T>(T valor1, T valor2, String nomeCampo)
        {
            if (!Equals(valor1, valor2))
                Campos.Add(nomeCampo);

            return this;
        }

        /// <summary>
        /// Alteração de dados do próprio usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário</param>        
        public void AlteracaoDadosUsuario(
            Sessao sessaoUsuarioResponsavel)
        {
            Historico.AlteracaoDadosUsuario(sessaoUsuarioResponsavel, Campos.ToArray());
        }

        /// <summary>
        /// Alteração de dados de outro usuário
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela alteração dos dados de outro usuário</param>
        /// <param name="codigoIdUsuarioAlterado">Código ID do usuário cujos dados foram alterados</param>
        /// <param name="nomeUsuarioAlterado">Nome do usuário cujos dados foram alterados</param>
        /// <param name="emailUsuarioAlterado">E-mail do usuário cujos dados foram alterados</param>
        /// <param name="perfilUsuarioAlterado">Perfil do usuário cujos dados foram alterados</param>
        public void AlteracaoDadosOutroUsuario(
            Sessao sessaoUsuarioResponsavel,
            Int32 codigoIdUsuarioAlterado,
            String nomeUsuarioAlterado,
            String emailUsuarioAlterado,
            String perfilUsuarioAlterado)
        {
            Historico.AlteracaoDadosOutroUsuario(
                sessaoUsuarioResponsavel,
                codigoIdUsuarioAlterado,
                nomeUsuarioAlterado,
                emailUsuarioAlterado,
                perfilUsuarioAlterado,
                Campos.ToArray());
        }

        /// <summary>
        /// Alteração de cadastro de um estabelecimento
        /// </summary>
        /// <param name="sessaoUsuarioResponsavel">Sessão do usuário responsável pela alteração dos dados do estabelecimento</param>
        public void AlteracaoDadosEstabelecimento(
            Sessao sessaoUsuarioResponsavel)
        {
            Historico.AlteracaoDadosEstabelecimento(sessaoUsuarioResponsavel, Campos.ToArray());
        }

        /// <summary>
        /// Método auxiliar para comparação entre instâncias de um classe.
        /// </summary>
        /// <typeparam name="T">Tipo de dado</typeparam>
        /// <param name="valor1">Instância 1</param>
        /// <param name="valor2">Instância 2</param>
        /// <returns>Se são equivalentes ou não</returns>
        public static Boolean Equals<T>(T valor1, T valor2)
        {
            Type type = default(Type);

            if (valor1 == null && valor2 == null)
                return true;
            else if (valor1 != null)
                type = valor1.GetType();
            else
                type = valor2.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var c1 = valor1 as ICollection;
                var c2 = valor2 as ICollection;

                if (c1.Count != c2.Count)
                    return false;
                else
                {
                    Type listType = type.GetGenericArguments()[0];
                    var arr1 = Array.CreateInstance(listType, c1.Count);
                    var arr2 = Array.CreateInstance(listType, c2.Count);
                    c1.CopyTo(arr1, 0);
                    c2.CopyTo(arr2, 0);

                    for (Int32 index = 0; index < c1.Count; index++)
                    {
                        var v1 = Convert.ChangeType(arr1.GetValue(index), listType);
                        var v2 = Convert.ChangeType(arr2.GetValue(index), listType);
                        if (!Equals(v1, v2))
                            return false;
                    }

                    return true;
                }
            }
            else if (type.IsArray)
            {
                var a1 = valor1 as Array;
                var a2 = valor2 as Array;
                if (a1.Length != a2.Length)
                    return false;

                for (Int32 index = 0; index < a1.Length; index++)
                {
                    if (!Equals(a1.GetValue(index), a2.GetValue(index)))
                        return false;
                }
                return true;
            }
            else if (type.IsPrimitive || type.IsValueType ||
                type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(Boolean) ||
                type == typeof(Decimal) ||
                type == typeof(String))
            {
                return Object.Equals(valor1, valor2);
            }
            else
            {
                foreach (var prop in type.GetProperties())
                {
                    if (!Equals(prop.GetValue(valor1, null), prop.GetValue(valor2, null)))
                        return false;
                }
                return true;
            }
        }

        #endregion
    }    
}