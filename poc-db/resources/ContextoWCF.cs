using System;
using System.ServiceModel;

namespace Rede.PN.ApiLogin.Core.Wcf
{
	/// <summary>
	/// Classe Contexto WCF
	/// </summary>
	public class ContextoWcf<TCliente> : IDisposable
		where TCliente : ICommunicationObject, new()
	{
		private TCliente cliente;

		/// <summary>
		/// Propriedade Cliente. Caso esteja nulo ou disposed, retorna exceção do tipo ObjectDisposedException.
		/// </summary>
		public TCliente Cliente
		{
			get
			{
				if (disposed || cliente == null)
					throw new ObjectDisposedException("ContextoWcf");

				return cliente;
			}
		}

		/// <summary>
		/// Construtor. Cria nova instância de TCliente.
		/// </summary>
		public ContextoWcf()
		{
			cliente = new TCliente();
		}

		#region Impementação IDisposable

		private Boolean disposed;

		/// <summary>
		/// Dispose do objeto.
		/// </summary>
		~ContextoWcf()
		{
			Dispose(false);
		}

		/// <summary>
		/// Dispose do objeto.
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose do objeto.
		/// </summary>
		/// <param name="disposing">bool</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposed || cliente == null)
				return;

			try
			{
				if (cliente.State != CommunicationState.Faulted)
					cliente.Close();
				else
					cliente.Abort();
			}
			catch (CommunicationException)
			{
				cliente.Abort();
			}
			catch (TimeoutException)
			{
				cliente.Abort();
			}
			catch (Exception)
			{
				cliente.Abort();
				throw;
			}
			finally
			{
				cliente = default(TCliente);
				disposed = true;
			}
		}
		#endregion
	}
}
