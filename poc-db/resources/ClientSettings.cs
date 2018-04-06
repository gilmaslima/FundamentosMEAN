/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.SharePoint;
using System;
using System.Reflection;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ClientSettings
    /// </summary>
	public static class ClientSettings
	{
        /// <summary>
        /// clientAssembly
        /// </summary>
		private static Assembly clientAssembly;

        /// <summary>
        /// ClientAssembly
        /// </summary>
		public static Assembly ClientAssembly
		{
			get
			{
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    if (ClientSettings.clientAssembly == null)
                    {
                        String codeBase = (String)AppDomain.CurrentDomain.GetData("clientAssemblyPath");
                        ClientSettings.clientAssembly = Assembly.Load(new AssemblyName
                        {
                            CodeBase = codeBase
                        });
                    }
                });

				return ClientSettings.clientAssembly;
			}
		}

        /// <summary>
        /// Obtêm o Type baseado no nome do tipo, utilizando o ClientDomain carregado atualmente.
        /// </summary>
        /// <param name="typeName">Nome do tipo.</param>
        /// <returns>Objeto Type correspodente.</returns>
		public static Type GetType(String typeName)
		{
			Type type = ClientSettings.ClientAssembly.GetType(typeName);
			if (type == null)
			{
				type = Type.GetType(typeName);
				if (type == null)
				{
					AssemblyName[] referencedAssemblies = ClientSettings.ClientAssembly.GetReferencedAssemblies();
					AssemblyName[] array = referencedAssemblies;
                    for (Int32 i = 0; i < array.Length; i++)
					{
						AssemblyName assemblyRef = array[i];
						Assembly assembly = Assembly.Load(assemblyRef);
						if (assembly != null)
						{
							type = assembly.GetType(typeName);
							if (type != null)
							{
								break;
							}
						}
					}
				}
			}
			return type;
		}
	}
}
