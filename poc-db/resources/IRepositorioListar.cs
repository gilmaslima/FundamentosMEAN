using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio
{
	/// <summary>
	/// Interface de Repositórios que implementarão os métodos de Listagem.
	/// </summary>
	/// <typeparam name="TEntidade">Tipo entidade</typeparam>
	/// <typeparam name="TChave">Tipo chave</typeparam>
	public interface IRepositorioListar<TEntidade, TChave>
		where TEntidade : class, IEntidade<TChave>, new()
	{
		/// <summary>
		/// Realiza a recuperação de itens.
		/// </summary>
		/// <param name="entidade">Os parâmetros de filtro.</param>
		/// <param name="registroInicial">O número do registro inicial da consulta.</param>
		/// <param name="registroFinal">O número do registro final da consulta.</param>
		/// <param name="totalRegistros">O total de registros recuperados para os filtros.</param>
		List<TEntidade> Listar(TEntidade entidade, Int64 registroInicial, Int64 registroFinal, out Decimal totalRegistros);
	}
}
