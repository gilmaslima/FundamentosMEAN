using System;

namespace Rede.PN.AtendimentoDigital.Core.Padroes.Singleton
{
	/// <summary>
	/// Implementação básica de um componente Singleton.
	/// </summary>
	/// <remarks>
	/// A criação da instância é realizada apenas uma vez e pela chamada ao construtor padrão e sem parâmetros, durante a primeira chamada da propriedade Instancia.
	/// </remarks>
	public class Singleton<TObjeto>
		where TObjeto : class
	{
		/// <summary>
		/// Container da instância do componente Singleton.
		/// </summary>
		private static volatile TObjeto instancia;

		/// <summary>
		/// Bloqueador utilizado para garantir a sincronia entre threads concorrentes.
		/// </summary>
		static readonly Object bloqueador = new Object();

		/// <summary>
		/// Instância acessível na hierarquia de herança.
		/// Responsável por inicializar o container da instância do componetne Singleton.
		/// </summary>
		protected static TObjeto Instancia
		{
			get
			{
				if (instancia == null)
				{
					lock (bloqueador)
					{
						// Checagem dupla, necessária para mitigação de possíveis problemas na sincronização de threads
						if (instancia == null)
							instancia = Activator.CreateInstance(typeof(TObjeto), true) as TObjeto;

						// Validando se a instância foi criada corretamente
						if (instancia == null)
							throw new NullReferenceException(
								string.Format("Não foi possível criar uma instância do tipo '{0}'.", typeof(TObjeto))
							);
					}
				}

				return instancia;
			}
		}
	}
}
