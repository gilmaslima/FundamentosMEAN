using Rede.PN.AtendimentoDigital.Core.Padroes.ServiceLocator;
using System;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados
{
	/// <summary>
	/// Interface generica de execução do SyncP2.
	/// </summary>
	public interface IServicoSyncPass : IServico
	{
		/// <summary>
		/// Obtem a senha por meio de chamada ao serviço do SyncPass
		/// </summary>
		/// <param name="arquivoSyncPass">Nome do arquivo, sem extenção.</param>
		/// <returns>Retorna uma string contendo a senha do banco especificado pelo parâmetro <paramref name="arquivoSyncPass"/>.</returns>
		String ObterSenha(String arquivoSyncPass);
	}
}
