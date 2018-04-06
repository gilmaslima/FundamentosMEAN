using Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto;
using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rede.PN.AtendimentoDigital.Core
{
	/// <summary>
	/// Esta classe provê um gerador de objetos.
	/// </summary>
	public sealed class GeradorObjeto : Singleton<GeradorObjeto>
	{
		#region Singleton

		/// <summary>
		/// Registra um objeto para uma interface
		/// </summary>
		/// <param name="tipoInterface">Interface a ser registrada para um objeto</param>
		/// <param name="tipoImplementacao">Tipo do objeto a ser utilizado</param>
		/// <param name="interceptadores">Coleção de interceptadores</param>
		/// <param name="propriedades">Propriedades do objeto</param>
		/// <param name="singleton">Determina se o objeto é singleton ou não</param>
		/// <param name="interceptado">Determina se o objeto é interceptado ou não</param>
		public static void RegistrarTipo(Type tipoInterface, Type tipoImplementacao, ColecaoInterceptador interceptadores, ColecaoPropriedade propriedades, Boolean singleton = false, Boolean interceptado = false)
		{
			GeradorObjeto.Instancia.RegistrarTipoInterno(
				tipoInterface,
				tipoImplementacao,
				interceptadores,
				propriedades,
				singleton,
				interceptado
			);
		}

		/// <summary>
		/// Obtem uma instância do tipo especificado por <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Tipo do objeto requisitado.</typeparam>
		/// <param name="nome">O nome do objeto registrado</param>
		/// <returns>A instância do objeto requisitado.</returns>
		public static T Obter<T>(params ResolverOverride[] injecoes)
		{
			return GeradorObjeto.Instancia.ObterInterno<T>(injecoes);
		}

		/// <summary>
		/// Define paramatros customizados para o recuperação do tipo definido por <param name="tipo"/>.
		/// </summary>
		/// <param name="tipo">Tipo do objeto requisitado.</param>
		/// <param name="injectionMembers">As configurações de injeção.</param>
		internal static void ConfigurarInjecaoPara(Type tipo, params InjectionMember[] injectionMembers)
		{
			GeradorObjeto.Instancia.ConfigurarInjecaoParaInterno(tipo, injectionMembers);
		}

		/// <summary>
		/// Recupera a primeira ocorrencia de registro para o tipo definido por <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Tipo do objeto requisitado.</typeparam>
		internal static ContainerRegistration RecuperarRegistro<T>()
		{
			return GeradorObjeto.Instancia.RecuperarRegistroInterno<T>();
		}

		#endregion

		#region Gerador Objeto

		private UnityContainer containerUnity;

		private GeradorObjeto()
		{
			// Criando o container do Unity
			this.containerUnity = new UnityContainer();

			//Para poder interceptar
			this.containerUnity.AddNewExtension<Interception>();

			// Registrando os tipos de objeto
			ColecaoObjeto objetos = ConfiguracoesGeradorObjeto.Configuracoes.Objetos;
			foreach (Objeto item in objetos)
				this.RegistrarTipoInterno(
					Type.GetType(item.Interface),
					Type.GetType(item.Implementacao),
					item.Interceptadores,
					item.Propriedades,
					item.Singleton,
					item.Interceptado
				);
		}

		/// <summary>
		/// Registra um objeto para uma interface
		/// </summary>
		/// <param name="tipoInterface">Interface a ser registrada para um objeto</param>
		/// <param name="tipoImplementacao">Tipo do objeto a ser utilizado</param>
		/// <param name="interceptadores">Coleção de interceptadores</param>
		/// <param name="propriedades">Propriedades do objeto</param>
		/// <param name="singleton">Determina se o objeto é singleton ou não</param>
		/// <param name="interceptado">Determina se o objeto é interceptado ou não</param>
		private void RegistrarTipoInterno(Type tipoInterface, Type tipoImplementacao, ColecaoInterceptador interceptadores, ColecaoPropriedade propriedades, Boolean singleton = false, Boolean interceptado = false)
		{
			List<InjectionMember> membrosInjetados = new List<InjectionMember>();

			if (interceptado || (interceptadores != null && interceptadores.Count > 0))
			{
				membrosInjetados.Add(new Interceptor<TransparentProxyInterceptor>());
				membrosInjetados.Add(new InterceptionBehavior<PolicyInjectionBehavior>());
			}

			// Injetando propriedades
			if (propriedades != null && propriedades.Count > 0)
				foreach (Propriedade propriedade in propriedades)
					membrosInjetados.Add(
						new InjectionProperty(
							propriedade.Nome,
							this.ObterValorPropriedade(propriedade, tipoImplementacao)
						)
					);

			// Registrando o tipo
			if (singleton)
				this.containerUnity.RegisterType(
					tipoInterface,
					tipoImplementacao,
					new ContainerControlledLifetimeManager(),
					membrosInjetados.ToArray()
				);
			else
				this.containerUnity.RegisterType(
					tipoInterface,
					tipoImplementacao,
					membrosInjetados.ToArray()
				);

			// Registrando os injetores do tipo
			if (interceptadores != null)
				foreach (Interceptador interceptador in interceptadores)
				{
					Type tipoInterceptador = Type.GetType(interceptador.TipoInterceptador);

					this.containerUnity.Configure<Interception>()
						.AddPolicy((tipoInterface.FullName + interceptador.Expressoes).GetHashCode().ToString())
						.AddMatchingRule(new MemberNameMatchingRule(interceptador.Expressoes))
						.AddMatchingRule(new TypeMatchingRule(tipoInterface))
						.AddCallHandler(
							tipoInterceptador,
							new ContainerControlledLifetimeManager(),
							this.ObterMembrosInjetadosInterceptador(interceptador, tipoInterceptador)
						);
				}
		}

		/// <summary>
		/// Recupera os membros injetados para o interceptador.
		/// </summary>
		/// <param name="interceptador">A configuração do interceptador.</param>
		/// <param name="tipoInterceptador">O tipo interceptador.</param>
		/// <returns></returns>
		private InjectionMember[] ObterMembrosInjetadosInterceptador(Interceptador interceptador, Type tipoInterceptador)
		{
			List<InjectionMember> membrosInjetados = new List<InjectionMember>();

			// Criando a definição para injeção com o construtor default
			membrosInjetados.Add(
				new InjectionConstructor()
			);

			// Injetando os valores configurados para as propriedades do interceptador
			if (interceptador.Propriedades != null && interceptador.Propriedades.Count > 0)
				foreach (Propriedade propriedade in interceptador.Propriedades)
					membrosInjetados.Add(
						new InjectionProperty(
							propriedade.Nome,
							this.ObterValorPropriedade(propriedade, tipoInterceptador)
						)
					);

			return membrosInjetados.ToArray();
		}

		/// <summary>
		/// Recupera o valor da propriedade, realizando a conversão de tipo necessária.
		/// </summary>
		/// <param name="configuracaoPropriedade">A configuração da propriedade.</param>
		/// <param name="tipoObjeto">O tipo do objeto que contém a propriedade.</param>
		/// <returns></returns>
		private Object ObterValorPropriedade(Propriedade configuracaoPropriedade, Type tipoObjeto)
		{
			PropertyInfo informacaoPropriedade = tipoObjeto.GetProperty(configuracaoPropriedade.Nome);
			if (informacaoPropriedade != null)
				if (informacaoPropriedade.PropertyType.IsEnum)
					return Enum.Parse(informacaoPropriedade.PropertyType, configuracaoPropriedade.Valor);
				else
					return Convert.ChangeType(configuracaoPropriedade.Valor, informacaoPropriedade.PropertyType);

			return null;
		}

		/// <summary>
		/// Obtem uma instância do tipo especificado por <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Tipo do objeto requisitado.</typeparam>
		/// <param name="nome">O nome do objeto registrado</param>
		/// <returns>A instância do objeto requisitado.</returns>
		private T ObterInterno<T>(params ResolverOverride[] injecoes)
		{
			return this.containerUnity.Resolve<T>(injecoes);
		}

		/// <summary>
		/// Define paramatros customizados para o recuperação do tipo definido por <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Tipo do objeto requisitado.</typeparam>
		/// <param name="injectionMembers">As configurações de injeção.</param>
		private void ConfigurarInjecaoParaInterno(Type tipo, params InjectionMember[] injectionMembers)
		{
			this.containerUnity.Configure<InjectedMembers>().ConfigureInjectionFor(tipo, injectionMembers);
		}

		/// <summary>
		/// Recupera a primeira ocorrencia de registro para o tipo definido por <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Tipo do objeto requisitado.</typeparam>
		private ContainerRegistration RecuperarRegistroInterno<T>()
		{
			return this.containerUnity.Registrations.FirstOrDefault(f => f.RegisteredType.Equals(typeof(T)));
		}

		#endregion
	}
}
