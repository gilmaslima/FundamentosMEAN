
namespace Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio
{
	/// <summary>
	/// Define o contrato padrão que todos os componentes de repositório devem implementar.
	/// </summary>
	/// <typeparam name="TEntidade">O tipo de entidade que o repositório encapsula.</typeparam>
	/// <typeparam name="TChave">O tipo de chave que o repositório utiliza.</typeparam>
	public interface IRepositorio<TEntidade, TChave>
		where TEntidade : class, IEntidade<TChave>
	{
		#region Criação

		/// <summary>
		/// Inclui uma instância transitória de <typeparamref cref="TEntidade"/> a ser 
		/// rastreada e persistida pelo repositório.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		TChave Inserir(TEntidade entidade);

		#endregion

		#region Leitura

		/// <summary>
		/// Recupera a instância de <typeparamref cref="TEntidade"/> especificada por <typeparamref cref="TChave"/>.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		TEntidade Obter(TEntidade entidade);

		#endregion

		#region Atualização

		/// <summary>
		/// Marca uma instância de <typeparamref cref="TEntidade"/> para ser atualizada.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		void Atualizar(TEntidade entidade);

		#endregion

		#region Exclusão

		/// <summary>
		/// Marca uma instância de <typeparamref cref="TEntidade"/> para ser excluída.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		void Excluir(TEntidade entidade);

		#endregion
	}
}
