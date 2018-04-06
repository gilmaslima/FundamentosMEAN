/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace Redecard.PN.Maximo.Agentes.Mapeadores
{
    internal static class Mapeador
    {
        /// <summary>
        /// Configura os mapeamentos AutoMapper bidirecionais entre as classes de modelo do 
        /// domínio da aplicação para modelos de serviço do Sistema Máximo.
        /// </summary>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método
        ///            18/11/2013 - Refatoração do método
        /// </remarks>
        public static void Configurar()
        {
            //Obtém o Type desta classe Mapeador
            Assembly assembly = typeof(Mapeador).Assembly;
            
            //Busca todas as classes do Assembly, que herdam de "Profile"
            Type[] profiles = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Profile))).ToArray();

            foreach (Type profileType in profiles)
            {
                //Adiciona profile na configuração do AutoMapper
                Profile profile = (Profile)Activator.CreateInstance(profileType);
                Mapper.AddProfile(profile);
            }
        }
    }
}