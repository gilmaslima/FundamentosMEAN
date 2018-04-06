using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.Modelo.Agente
{
	/// <summary>
	/// Interface de Agentes que implementarão os métodos de Listagem e Obtenção.
	/// </summary>
	/// <typeparam name="TEntidade">Tipo entidade</typeparam>
	/// <typeparam name="TChave">Tipo chave</typeparam>
	public interface IAgenteListarObter<TEntidade, TChave>
		where TEntidade : class, IEntidade<TChave>, new()
	{
		/// <summary>
		/// Metodo Listar
		/// </summary>
		/// <param name="entidade">entidade</param>
		List<TEntidade> Listar(TEntidade entidade);

		/// <summary>
		/// Metodo Obter
		/// </summary>
		/// <param name="entidade">entidade</param>
		TEntidade Obter(TEntidade entidade);
	}
}
