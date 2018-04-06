using System.ComponentModel;

namespace Redecard.PN.GerencieExtrato.Core.Seguranca.Portal.Enumerador
{
	/// <summary>
	/// Enum da propriedade Status
	///     1	Usuário Ativo
	///     2	Usuário Bloqueado por senha inválida
	///     3	Usuário Bloqueado por recuperação de senha
	///     4	Usuário Bloqueado por recuperação de usuário
	///     5	Usuário Ativo com Liberação de Acesso Completo Bloqueada
	///     6	Usuário Aguardando confirmação de e-mail - Alteração de e-mail
	///     7	Usuário Ativo Aguardando confirmação de e-mail - Recuperação de senha
	///     8	Usuário Bloqueado Aguardando confirmação de e-mail - Recuperação de senha
	///     9	Usuário Aguardando confirmação de e-mail - Criação de usuário
	///     100	Entidade Ativa
	///     101	Entidade Bloqueada na confirmação positiva
	/// </summary>
	public enum Status
	{
		/// <summary>
		/// Não Definido
		/// </summary>
		[Description("Não Definido")]
		NaoDefinido = 0,

		/// <summary>
		/// Usuário Ativo
		/// </summary>
		[Description("Usuário Ativo")]
		UsuarioAtivo = 1,

		/// <summary>
		/// Usuário Bloqueado por senha inválida
		/// </summary>
		[Description("Usuário Bloqueado por senha inválida")]
		UsuarioBloqueadoSenhaInvalida = 2,

		/// <summary>
		/// Usuário Bloqueado por recuperação de senha
		/// </summary>
		[Description("Usuário Bloqueado por recuperação de senha")]
		UsuarioBloqueadoRecuperacaoSenha = 3,

		/// <summary>
		/// Usuário Bloqueado por recuperação de usuário
		/// </summary>
		[Description("Usuário Bloqueado por recuperação de usuário")]
		UsuarioBloqueadoRecuperacaoUsuario = 4,

		/// <summary>
		/// Usuário Ativo com Liberação de Acesso Completo Bloqueada
		/// </summary>
		[Description("Usuário Ativo com Liberação de Acesso Completo Bloqueada")]
		UsuarioAtivoLiberacaoAcessoCompletoBloqueada = 5,

		/// <summary>
		/// Usuário Aguardando confirmação de e-mail - Alteração de e-mail
		/// </summary>
		[Description("Usuário Aguardando confirmação de e-mail - Alteração de e-mail")]
		UsuarioAguardandoConfirmacaoAlteracaoEmail = 6,

		/// <summary>
		/// Usuário Ativo Aguardando confirmação de e-mail - Recuperação de senha
		/// </summary>
		[Description("Usuário Ativo Aguardando confirmação de e-mail - Recuperação de senha")]
		UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha = 7,

		/// <summary>
		/// Usuário Bloqueado Aguardando confirmação de e-mail - Recuperação de senha
		/// </summary>
		[Description("Usuário Bloqueado Aguardando confirmação de e-mail - Recuperação de senha")]
		UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha = 8,

		/// <summary>
		/// Usuário Aguardando confirmação de e-mail - Criação de usuário
		/// </summary>
		[Description("Usuário Aguardando confirmação de e-mail - Criação de usuário")]
		UsuarioAguardandoConfirmacaoCriacaoUsuario = 9,

		/// <summary>
		/// Usuário Aguardando confirmação de e-mail - Criação de usuário
		/// </summary>
		[Description("Usuário Aguardando confirmação de e-mail - Criação de usuário")]
		UsuarioAguardandoConfirmacaoMaster = 10,

		/// <summary>
		/// Usuário Aguardando confirmação de e-mail - Criação de usuário
		/// </summary>
		[Description("Resposta da confirmação positiva pendente")]
		RespostaIdPosPendente = 11,

		/// <summary>
		/// Entidade Ativa
		/// </summary>
		[Description("Entidade Ativa")]
		EntidadeAtiva = 100,

		/// <summary>
		/// Entidade Bloqueada na confirmação positiva
		/// </summary>
		[Description("Entidade Bloqueada na confirmação positiva")]
		EntidadeBloqueadaConfirmacaoPositiva = 101
	}
}
