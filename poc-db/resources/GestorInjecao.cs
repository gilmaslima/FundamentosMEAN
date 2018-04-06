using Rede.PN.AtendimentoDigital.Core;
using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rede.PN.AtendimentoDigital.Servicos
{
	/// <summary>
	/// Esta classe provê um gestor de injeção.
	/// </summary>
	public sealed class GestorInjecao : Singleton<GestorInjecao>
	{
		#region Singleton

		/// <summary>
		/// Registra um objeto para uma interface
		/// </summary>
		public static void RegistrarObjetosConvencao()
		{
			GestorInjecao.Instancia.RegistrarObjetosConvencaoInterno();
		}

		#endregion Singleton

		#region Gestor de Injecao

		private readonly List<Type> interfacesRegistro;

		private GestorInjecao()
		{
			this.interfacesRegistro = new List<Type>();

			// Procurando interfaces de Repositório
			Type tipoRepositorio = Type.GetType("Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio.IRepositorio`2, Rede.PN.AtendimentoDigital.Modelo");
			if (tipoRepositorio != null)
				this.interfacesRegistro.Add(tipoRepositorio);

			Type tipoRepositorioListar = Type.GetType("Rede.PN.AtendimentoDigital.Modelo.Repositorio.IRepositorioListar`2, Rede.PN.AtendimentoDigital.Modelo");
			if (tipoRepositorioListar != null)
				this.interfacesRegistro.Add(tipoRepositorioListar);

			// Procurando interfaces de Agente
			Type tipoAgenteAtualizarInserir = Type.GetType("Rede.PN.AtendimentoDigital.Modelo.Agente.IAgenteAtualizarInserir`2, Rede.PN.AtendimentoDigital.Modelo");
			if (tipoAgenteAtualizarInserir != null)
				this.interfacesRegistro.Add(tipoAgenteAtualizarInserir);

			Type tipoAgenteListar = Type.GetType("Rede.PN.AtendimentoDigital.Modelo.Agente.IAgenteListar`2, Rede.PN.AtendimentoDigital.Modelo");
			if (tipoAgenteListar != null)
				this.interfacesRegistro.Add(tipoAgenteListar);

			Type tipoAgenteListarObter = Type.GetType("Rede.PN.AtendimentoDigital.Modelo.Agente.IAgenteListarObter`2, Rede.PN.AtendimentoDigital.Modelo");
			if (tipoAgenteListarObter != null)
				this.interfacesRegistro.Add(tipoAgenteListarObter);
		}

		private void RegistrarObjetosConvencaoInterno()
		{
			if (this.interfacesRegistro.Count <= 0)
				return;

			foreach (var tipo in this.interfacesRegistro)
				this.RegistrarConvencaoInterno(tipo);
		}

		private void RegistrarConvencaoInterno(Type tipoInterface)
		{
			// Recuperando o nome do assembly atual
			string assemblyNameServico = Assembly.GetExecutingAssembly().FullName;

			// Recuperando interfaces de repositórios que implementam a interface IRepositorio<TEntidade, TChave>
			string assemblyNameModelo = assemblyNameServico.Replace(".Servicos", ".Modelo");
			Assembly assemblyModelo = Assembly.Load(assemblyNameModelo);
			IEnumerable<Type> interfaces = assemblyModelo.GetTypes().Where(w =>
				w.GetInterfaces()
				 .Any(a => a.IsAssignableFrom(tipoInterface)
						|| (a.IsGenericType && a.GetGenericTypeDefinition() == tipoInterface))
			);

			// Recuperando implementações concretas dos repositórios
			var mapeamento = new Dictionary<Type, Type>();
#if DEBUG_ITERIS
			foreach (var item in this.ObterConcretas(assemblyNameServico.Replace(".Servicos", ".DadosMock"), interfaces))
				mapeamento.Add(item.Key, item.Value);

			interfaces = interfaces.Where(w => !mapeamento.ContainsKey(w));
#endif
			foreach (var item in this.ObterConcretas(assemblyNameServico.Replace(".Servicos", ".Dados"), interfaces))
				mapeamento.Add(item.Key, item.Value);

			// Registrando os mapeamentos de tipos
			foreach (var item in mapeamento.Where(w => w.Value != null))
				GeradorObjeto.RegistrarTipo(item.Key, item.Value, null, null);
		}

		private IEnumerable<KeyValuePair<Type, Type>> ObterConcretas(string assemblyNameDados, IEnumerable<Type> interfaces)
		{
			if (interfaces.Count() <= 0)
				return Enumerable.Empty<KeyValuePair<Type, Type>>();

			Assembly assemblyDados = Assembly.Load(assemblyNameDados);

			IEnumerable<Type> classes = assemblyDados.GetTypes().Where(w => w.IsClass);
			IEnumerable<KeyValuePair<Type, Type>> concretas = interfaces.Select(s =>
				new KeyValuePair<Type, Type>(
					s,
					classes.Where(w => w.GetInterfaces().Any(a => a == s)).FirstOrDefault()
				)
			);

			// Retornando mapeamentos e ignorando concretas não resolvidas
			return concretas.Where(w => w.Value != null);
		}

		#endregion Gestor de Injecao
	}
}
