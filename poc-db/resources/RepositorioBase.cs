using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	/// <summary>
	/// Classe base para implementação de <see cref="IRepositorio{TEntidade, TChave}"/>.
	/// </summary>
	/// <typeparam name="TEntidade">O tipo de entidade que o repositório encapsula.</typeparam>
	/// <typeparam name="TChave">O tipo de chave que o repositório utiliza.</typeparam>
	public class RepositorioBase<TEntidade, TChave> : IRepositorio<TEntidade, TChave>
		where TEntidade : class, IEntidade<TChave>, new()
	{
		protected Database ContextoBanco { get; set; }

		protected DbTransaction Transacao { get; set; }

		/// <summary>
		/// Inicializa uma nova instância de repositório.
		/// </summary>
		public RepositorioBase()
			: this(null, null)
		{
		}

		/// <summary>
		/// Inicializa uma nova instância de repositório.
		/// </summary>
		/// <param name="nomeConexao">O nome da conexão a ser utilizado.</param>
		public RepositorioBase(String nomeConexao)
			: this(GestorBancoDados.ObterBancoDados(nomeConexao))
		{
		}

		/// <summary>
		/// Inicializa uma nova instância de repositório.
		/// </summary>
		/// <param name="contextoBanco">O contexto de banco já recuperado anteriormente.</param>
		internal RepositorioBase(Database contextoBanco)
			: this(contextoBanco, null)
		{
		}

		/// <summary>
		/// Inicializa uma instância de repositório.
		/// </summary>
		/// <param name="contextoBanco">O contexto de banco já recuperado anteriormente.</param>
		/// <param name="transacao">A transação já iniciada anteriormente.</param>
		internal RepositorioBase(Database contextoBanco, DbTransaction transacao)
		{
			this.ContextoBanco = contextoBanco ?? GestorBancoDados.ObterBancoDados(this.GetType());
			this.Transacao = transacao;
		}

		#region Comandos de CRUD

		/// <summary>
		/// Define a especificação do comando de inclusão.
		/// </summary>
		protected virtual EspecificacaoComando<TEntidade, TChave> EspecificacaoInserir { get { return null; } }

		/// <summary>
		/// Inclui uma instância transitória de <typeparamref cref="TEntidade"/> a ser
		/// rastreada e persistida pelo repositório.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		public TChave Inserir(TEntidade entidade)
		{
			// Validando se o comando foi definido
			if (this.EspecificacaoInserir == null)
				return default(TChave);

			// Recuperando a lista de parâmetros
			List<ParametroComando> parametros = null;
			if (this.EspecificacaoInserir.DefinirParametrosComando != null)
				parametros = this.EspecificacaoInserir.DefinirParametrosComando(entidade);

			// Executando o comando
			TChave chave = default(TChave);

			this.ExecutarComando(
				this.EspecificacaoInserir.Comando,
				this.EspecificacaoInserir.TipoComando,
				parametros,
				this.EspecificacaoInserir.ModoExecucao,
				(Object retorno) =>
				{
					if (this.EspecificacaoInserir.TratarRetornoComando != null)
						chave = (TChave)this.EspecificacaoInserir.TratarRetornoComando(retorno);
				},
				(parametrosRetorno) =>
				{
					if (this.EspecificacaoInserir.TratarParametrosComando != null)
						this.EspecificacaoInserir.TratarParametrosComando(parametrosRetorno);
				}
			);

			return chave;
		}

		/// <summary>
		/// Define a especificação do comando de exclusão.
		/// </summary>
		protected virtual EspecificacaoComando<TEntidade, TChave> EspecificacaoExcluir { get { return null; } }

		/// <summary>
		/// Marca uma instância de <typeparamref cref="TEntidade"/> para ser excluída.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		public void Excluir(TEntidade entidade)
		{
			// Validando se o comando foi definido
			if (this.EspecificacaoExcluir == null)
				return;

			// Recuperando a lista de parâmetros
			List<ParametroComando> parametros = null;
			if (this.EspecificacaoExcluir.DefinirParametrosComando != null)
				parametros = this.EspecificacaoExcluir.DefinirParametrosComando(entidade);

			// Executando o comando
			this.ExecutarComando(
				this.EspecificacaoExcluir.Comando,
				this.EspecificacaoExcluir.TipoComando,
				parametros,
				this.EspecificacaoExcluir.ModoExecucao,
				(Object retorno) =>
				{
					if (this.EspecificacaoExcluir.TratarRetornoComando != null)
						this.EspecificacaoExcluir.TratarRetornoComando(retorno);
				},
				(parametrosRetorno) =>
				{
					if (this.EspecificacaoExcluir.TratarParametrosComando != null)
						this.EspecificacaoExcluir.TratarParametrosComando(parametrosRetorno);
				}
			);
		}

		/// <summary>
		/// Define a especificação do comando de atualização.
		/// </summary>
		protected virtual EspecificacaoComando<TEntidade, TChave> EspecificacaoAtualizar { get { return null; } }

		/// <summary>
		/// Marca uma instância de <typeparamref cref="TEntidade"/> para ser atualizada.
		/// </summary>
		/// <param name="entidade">Uma instância de <typeparamref cref="TEntidade"/>.</param>
		public void Atualizar(TEntidade entidade)
		{
			// Validando se o comando foi definido
			if (this.EspecificacaoAtualizar == null)
				return;

			// Recuperando a lista de parâmetros
			List<ParametroComando> parametros = null;
			if (this.EspecificacaoAtualizar.DefinirParametrosComando != null)
				parametros = this.EspecificacaoAtualizar.DefinirParametrosComando(entidade);

			// Executando o comando
			this.ExecutarComando(
				this.EspecificacaoAtualizar.Comando,
				this.EspecificacaoAtualizar.TipoComando,
				parametros,
				this.EspecificacaoAtualizar.ModoExecucao,
				(Object retorno) =>
				{
					if (this.EspecificacaoAtualizar.TratarRetornoComando != null)
						this.EspecificacaoAtualizar.TratarRetornoComando(retorno);
				},
				(parametrosRetorno) =>
				{
					if (this.EspecificacaoAtualizar.TratarParametrosComando != null)
						this.EspecificacaoAtualizar.TratarParametrosComando(parametrosRetorno);
				}
			);
		}

		/// <summary>
		/// Define a especificação do comando de obtenção.
		/// </summary>
		protected virtual EspecificacaoComando<TEntidade, TChave> EspecificacaoObter { get { return null; } }

		/// <summary>
		/// Recupera a instância de <typeparamref cref="TEntidade"/> especificada por <typeparamref cref="TChave"/>.
		/// </summary>
		/// <param name="chave">Uma <typeparamref cref="TChave"/> utilizada para filtrar o resultado.</param>
		/// <returns>Uma instância de <typeparamref cref="TEntidade"/> que satisfaz o filtro especificado por <typeparamref cref="TChave"/>.</returns>
		public TEntidade Obter(TEntidade entidade)
		{
			// Validando se o comando foi definido
			if (this.EspecificacaoObter == null)
				return default(TEntidade);

			// Recuperando a lista de parâmetros
			List<ParametroComando> parametros = null;
			if (this.EspecificacaoObter.DefinirParametrosComando != null)
				parametros = this.EspecificacaoObter.DefinirParametrosComando(entidade);

			// Executando o comando
			this.ExecutarComando(
				this.EspecificacaoObter.Comando,
				this.EspecificacaoObter.TipoComando,
				parametros,
				this.EspecificacaoObter.ModoExecucao,
				(Object retorno) =>
				{
					if (this.EspecificacaoObter.TratarRetornoComando != null)
						entidade = (TEntidade)this.EspecificacaoObter.TratarRetornoComando(retorno);
				},
				(parametrosRetorno) =>
				{
					if (this.EspecificacaoObter.TratarParametrosComando != null)
						this.EspecificacaoObter.TratarParametrosComando(parametrosRetorno);
				}
			);

			return entidade;
		}

		#endregion Comandos de CRUD

		#region Execucao de Comandos

		/// <summary>
		/// Executa um comando sem parâmetros e em modo reader.
		/// </summary>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		protected void ExecutarReader(String textoComando, TipoComando tipo, Action<IDataReader> executado)
		{
			this.ExecutarReader(textoComando, tipo, null, executado);
		}

		/// <summary>
		/// Executa um comando em modo reader.
		/// </summary>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="parametros">A lista de parâmetros a serem uilizados na execução do comando.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		protected void ExecutarReader(String textoComando, TipoComando tipo, IEnumerable<ParametroComando> parametros, Action<IDataReader> executado)
		{
			this.ExecutarReader(textoComando, tipo, parametros, executado, null);
		}

		/// <summary>
		/// Executa um comando em modo reader.
		/// </summary>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="parametros">A lista de parâmetros a serem uilizados na execução do comando.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		/// <param name="retornoParametros">Definição da <see cref="Action"/> a ser chamada para tratamento dos parâmetros carregados após a execução.</param>
		protected void ExecutarReader(String textoComando, TipoComando tipo, IEnumerable<ParametroComando> parametros, Action<IDataReader> executado, Action<IEnumerable<ParametroComando>> retornoParametros)
		{
			this.ExecutarComando(
				textoComando,
				tipo,
				parametros,
				ModoExecucao.Reader,
				(Object retorno) =>
				{
					if (executado != null)
						executado((IDataReader)retorno);
				},
				retornoParametros
			);
		}

		/// <summary>
		/// Executa um comando, permitindo o tratamento de parâmetros após a execução.
		/// </summary>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="parametros">A lista de parâmetros a serem uilizados na execução do comando.</param>
		/// <param name="modoExecucao">O modo de execução definido por <see cref="ModoExecucao"/>.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		/// <param name="retornoParametros">Definição da <see cref="Action"/> a ser chamada para tratamento dos parâmetros carregados após a execução.</param>
		protected void ExecutarComando(string textoComando, TipoComando tipo, IEnumerable<ParametroComando> parametros, ModoExecucao modoExecucao, Action<object> executado, Action<IEnumerable<ParametroComando>> retornoParametros)
		{
			this.ExecutarComando(textoComando, tipo, parametros, modoExecucao, executado);

			if (retornoParametros != null)
				retornoParametros(parametros);
		}

		/// <summary>
		/// Executa um comando.
		/// </summary>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="parametros">A lista de parâmetros a serem uilizados na execução do comando.</param>
		/// <param name="modoExecucao">O modo de execução definido por <see cref="ModoExecucao"/>.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		protected virtual void ExecutarComando(String textoComando, TipoComando tipo, IEnumerable<ParametroComando> parametros, ModoExecucao modoExecucao, Action<Object> executado = null)
		{
			using (DbCommand comando = this.ObterComando(textoComando, tipo))
			{
				this.ExecutarComando(comando, parametros, modoExecucao, executado);

				// Atualizando os valores da lista de parâmetros
				foreach (ParametroComando parametroItem in parametros)
					parametroItem.Valor = this.ObterValorParametro(comando, parametroItem);
			}
		}

		/// <summary>
		/// Executa um comando.
		/// </summary>
		/// <param name="comando">O comando a ser executado.</param>
		/// <param name="parametros">A lista de parâmetros a serem uilizados na execução do comando.</param>
		/// <param name="modoExecucao">O modo de execução definido por <see cref="ModoExecucao"/>.</param>
		/// <param name="executado">Definição da <see cref="Action"/> a ser chamada após a execução do comando.</param>
		/// <remarks>
		/// Este método não realiza a liberação do comando (dispose) faça isso explicitamente após a execução ou utilize o bloco using.
		///
		/// EXEMPLO COM BLOCO USING:
		/// using (DbCommand comando = this.ObterComando("proc_listar_funcionalidade", TipoComando.StoredProcedure))
		/// {
		/// 	this.ExecutarComando(
		/// 		comando,
		/// 		new ParametroComando[]
		/// 		{
		/// 			new ParametroComando(DirecaoParametro.Entrada, "p_registro_inicial", DbType.Int32, 1),
		/// 			new ParametroComando(DirecaoParametro.Entrada, "p_registro_final", DbType.Int32, 100),
		/// 			new ParametroComando(DirecaoParametro.Saida, "p_registro_total", DbType.Int32, DBNull.Value),
		/// 			new ParametroComando(DirecaoParametro.Saida, "p_mensagem", DbType.String, DBNull.Value),
		/// 		},
		/// 		ModoExecucao.Reader,
		/// 		(Object retorno) =>
		/// 		{
		/// 			if (executado != null)
		/// 				executado((IDataReader)retorno);
		/// 		}
		/// 	);
		/// }
		///
		/// EXEMPLO COM DISPOSE EXPLÍCITO:
		/// DbCommand comando = null;
		/// try
		/// {
		/// 	comando = this.ObterComando("proc_listar_funcionalidade", TipoComando.StoredProcedure)
		/// 	this.ExecutarComando(
		/// 		comando,
		/// 		new ParametroComando[]
		/// 		{
		/// 			new ParametroComando(DirecaoParametro.Entrada, "p_registro_inicial", DbType.Int32, 1),
		/// 			new ParametroComando(DirecaoParametro.Entrada, "p_registro_final", DbType.Int32, 100),
		/// 			new ParametroComando(DirecaoParametro.Saida, "p_registro_total", DbType.Int32, DBNull.Value),
		/// 			new ParametroComando(DirecaoParametro.Saida, "p_mensagem", DbType.String, DBNull.Value),
		/// 		},
		/// 		ModoExecucao.Reader,
		/// 		(Object retorno) =>
		/// 		{
		/// 			if (executado != null)
		/// 				executado((IDataReader)retorno);
		/// 		}
		/// 	);
		/// }
		/// finally
		/// {
		/// 	if (comando!=null)
		/// 		comando.Dispose();
		/// }
		/// </remarks>
		protected virtual void ExecutarComando(DbCommand comando, IEnumerable<ParametroComando> parametros, ModoExecucao modoExecucao, Action<Object> executado = null)
		{
			// Incluindo parâmetro do comando
			this.AdicionarParametros(comando, parametros);

			// Executando o comando contra o banco
			switch (modoExecucao)
			{
				case ModoExecucao.DataSet:
					if (this.Transacao == null)
						using (DataSet dataSet = this.ContextoBanco.ExecuteDataSet(comando))
						{
							// Chamando delegate pós execução
							if (executado != null)
								executado(dataSet);
						}
					else
						using (DataSet dataSet = this.ContextoBanco.ExecuteDataSet(comando, this.Transacao))
						{
							// Chamando delegate pós execução
							if (executado != null)
								executado(dataSet);
						}

					break;

				case ModoExecucao.NonQuery:
					// Definindo o retorno
					Int32 linhasAfetadas = default(Int32);
					if (this.Transacao == null)
						linhasAfetadas = this.ContextoBanco.ExecuteNonQuery(comando);
					else
						linhasAfetadas = this.ContextoBanco.ExecuteNonQuery(comando, this.Transacao);

					// Chamando delegate pós execução
					if (executado != null)
						executado(linhasAfetadas);

					break;

				case ModoExecucao.Reader:
					if (this.Transacao == null)
						using (IDataReader dataReader = this.ContextoBanco.ExecuteReader(comando))
						{
							// Chamando delegate pós execução
							if (executado != null)
								executado(dataReader);
						}
					else
						using (IDataReader dataReader = this.ContextoBanco.ExecuteReader(comando, this.Transacao))
						{
							// Chamando delegate pós execução
							if (executado != null)
								executado(dataReader);
						}

					break;

				case ModoExecucao.Scalar:
					// Definindo o retorno
					Object retorno = default(Object);
					if (this.Transacao == null)
						retorno = this.ContextoBanco.ExecuteScalar(comando);
					else
						retorno = this.ContextoBanco.ExecuteScalar(comando, this.Transacao);

					// Chamando delegate pós execução
					if (executado != null)
						executado(retorno);

					break;

				default:
					throw new NotImplementedException(
						String.Format("Modo de execução '{0}' não implementado.",
							modoExecucao
						)
					);
			}
		}

		/// <summary>
		/// Recupera o comando para execução
		/// </summary>
		/// <param name="tipo">Tipo de comando definido por <see cref="TipoComando"/>.</param>
		/// <param name="textoComando">Uma stored procedure ou consulta SQL.</param>
		/// <returns></returns>
		internal DbCommand ObterComando(String textoComando, TipoComando tipo)
		{
			// Validando parâmetros
			if (String.IsNullOrWhiteSpace(textoComando))
				throw new ArgumentNullException("Um comando deve ser informado.");

			// Verificando os tipos de comando
			switch (tipo)
			{
				case TipoComando.Texto:
					return this.ContextoBanco.GetSqlStringCommand(textoComando);

				case TipoComando.StoredProcedure:
					return this.ContextoBanco.GetStoredProcCommand(textoComando);

				default:
					throw new NotImplementedException(
						String.Format("Tipo de comando '{0}' não implementado.",
							tipo
						)
					);
			}
		}

		/// <summary>
		/// Adiciona parâmetros ao comando.
		/// </summary>
		/// <param name="comando">O comando.</param>
		/// <param name="parametros">A lista de definição de parâmetros.</param>
		protected virtual void AdicionarParametros(DbCommand comando, IEnumerable<ParametroComando> parametros)
		{
			// Validando os parâmetros
			if (parametros == null)
				return;

			// Varendo os parâmetros e realizando a inclusão
			foreach (ParametroComando item in parametros)
			{
				// Verificando a direção do parâmetro
				switch (item.Direcao)
				{
					case DirecaoParametro.Entrada:
						this.ContextoBanco.AddInParameter(comando, item.Nome, item.TipoDadoBanco, item.Valor);
						break;

					case DirecaoParametro.Saida:
						this.ContextoBanco.AddOutParameter(comando, item.Nome, item.TipoDadoBanco, item.Tamanho);
						break;

					default:
						throw new NotImplementedException(
							String.Format("Direção do parâmetro '{0}' não implementada.",
								item.Direcao
							)
						);
				}
			}
		}

		#endregion Execucao de Comandos

		#region Funções de Auxiliares

		/// <summary>
		/// Reupera o valor de um parâmetro a partir da lista de retorno após execução.
		/// </summary>
		/// <typeparam name="TRetorno">O tipo de conversão do valor.</typeparam>
		/// <param name="retornoParametros">A lista de retorno após execução.</param>
		/// <param name="nomeParametro">O nome do parâmetro.</param>
		protected TRetorno ObterValorParametro<TRetorno>(IEnumerable<ParametroComando> retornoParametros, String nomeParametro)
		{
			object retorno = default(TRetorno);

			ParametroComando parametro = retornoParametros.FirstOrDefault(f => f.Nome.Equals(nomeParametro, StringComparison.OrdinalIgnoreCase));
			if (parametro != null && parametro.Valor != DBNull.Value)
				retorno = parametro.Valor;

			return (TRetorno)Convert.ChangeType(retorno, typeof(TRetorno));
		}

		/// <summary>
		/// Reupera o valor de um parâmetro de comando.
		/// </summary>
		/// <param name="comando">O comando executado.</param>
		/// <param name="parametro">O parâmetro a ser recuperado.</param>
		private object ObterValorParametro(DbCommand comando, ParametroComando parametro)
		{
			object retorno = this.ContextoBanco.GetParameterValue(comando, parametro.Nome);

			if (parametro != null && retorno != DBNull.Value)
				retorno = Convert.ChangeType(
					retorno,
					this.ObterTipoDadoObjeto(parametro.TipoDadoBanco)
				);

			return retorno;
		}

		private Type ObterTipoDadoObjeto(DbType tipoDadoBanco)
		{
			switch (tipoDadoBanco)
			{
				case DbType.Binary:
					return typeof(byte[]);

				case DbType.Byte:
					return typeof(byte);

				case DbType.Boolean:
					return typeof(bool);

				case DbType.Currency:
				case DbType.Decimal:
				case DbType.VarNumeric:
					return typeof(decimal);

				case DbType.Date:
				case DbType.DateTime:
				case DbType.Time:
				case DbType.DateTime2:
					return typeof(DateTime);

				case DbType.Double:
					return typeof(double);

				case DbType.Guid:
					return typeof(Guid);

				case DbType.Int16:
					return typeof(short);

				case DbType.Int32:
					return typeof(int);

				case DbType.Int64:
					return typeof(long);

				case DbType.SByte:
					return typeof(sbyte);

				case DbType.Single:
					return typeof(float);

				case DbType.UInt16:
					return typeof(ushort);

				case DbType.UInt32:
					return typeof(UInt32);

				case DbType.UInt64:
					return typeof(UInt64);

				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return typeof(char);

				case DbType.DateTimeOffset:
					return typeof(DateTimeOffset);

				default:
					return typeof(string);
			}
		}

		#endregion
	}
}
