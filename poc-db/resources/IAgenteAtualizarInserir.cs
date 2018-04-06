using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;

namespace Rede.PN.AtendimentoDigital.Modelo.Agente
{
	/// <summary>
	/// Interface de Agentes que implementarão os métodos de Atualização e Inclusão.
	/// </summary>
	/// <typeparam name="TEntidade">Tipo entidade</typeparam>
	/// <typeparam name="TChave">Tipo chave</typeparam>
	public interface IAgenteAtualizarInserir<TEntidade, TChave>
		where TEntidade : class, IEntidade<TChave>, new()
	{
		/// <summary>
		/// Metodo Atualizar
		/// </summary>
		/// <param name="entidade">entidade</param>
		TChave Atualizar(TEntidade entidade);

		/// <summary>
		/// Metodo inserir
		/// </summary>
		/// <param name="entidade">entidade</param>
		/// <returns>Tipo</returns>
		TChave Inserir(TEntidade entidade);
	}
}
