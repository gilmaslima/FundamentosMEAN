using Rede.PN.AtendimentoDigital.Core.Config.LocalizadorServico;
using Rede.PN.AtendimentoDigital.Core.Padroes.ServiceLocator;
using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using System;

namespace Rede.PN.AtendimentoDigital.Core
{
	/// <summary>
	/// Esta classe provê um localizador de serviços comum.
	/// </summary>
	public sealed class LocalizadorServico : Singleton<LocalizadorServico>
	{
		#region Metodos Estaticos

		/// <summary>
		/// Obtem uma instância do tipo especificado por <typeparamref name="TServico"/>.
		/// </summary>
		/// <typeparam name="TServico">Tipo do objeto requisitado.</typeparam>
		/// <returns>A instância do serviço requisitado.</returns>
		public static TServico ObterServico<TServico>() where TServico : IServico
		{
			return LocalizadorServico.Instancia.ObterServicoInterno<TServico>();
		}

		#endregion

		#region Localizador Servico

		/// <summary>
		/// Construtor padrão. Registra os tipos de serviço configurados.
		/// </summary>
		private LocalizadorServico()
		{
			// Registrando os tipos de serviço
			ColecaoServico servicos = ConfiguracoesLocalizadorServico.Configuracoes.Servicos;
			foreach (Servico item in servicos)
				GeradorObjeto.RegistrarTipo(
					Type.GetType(item.Interface),
					Type.GetType(item.Implementacao),
					null,
					null,
					true,
					item.Interceptado
				);
		}

		/// <summary>
		/// Obtem uma instância do tipo especificado por <typeparamref name="TServico"/>.
		/// </summary>
		/// <typeparam name="TServico">Tipo do objeto requisitado.</typeparam>
		/// <returns>A instância do serviço requisitado.</returns>
		private TServico ObterServicoInterno<TServico>() where TServico : IServico
		{
			return GeradorObjeto.Obter<TServico>();
		}

		#endregion
	}
}
