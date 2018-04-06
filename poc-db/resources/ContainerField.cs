/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ContainerField
    /// </summary>
    [KnownType("GetKnownTypes")]
    [Serializable]
    [DataContract]
    public class ContainerField : Field
	{       
        /// <summary>
        /// Contstrutor ContainerField
        /// </summary>        
        public ContainerField()
        { 
        }

        /// <summary>
        /// Contstrutor ContainerField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public ContainerField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Valida se o formato do campo, está correto para gerar o valor da coleção.
        /// </summary>
        /// <param name="input"></param>
        public override void ValidateAndCanonicalize(String input)
		{
			base.ValidateAndCanonicalize(input);
			Int32 num = -1;

			if (this.value == null || String.Compare(input, "(null)") == 0)
			{
				return;
			}

			if (!input.TrimStart(new Char[] {' '}).StartsWith("length", StringComparison.OrdinalIgnoreCase))
			{
				this.value = null;
				return;
			}

			this.value = input.Replace(" ", "");
			if (this.value.StartsWith("length=", StringComparison.OrdinalIgnoreCase))
			{
				input = this.value.Substring("length=".Length);
				if (Int32.TryParse(input, out num) && num >= 0)
				{
					this.value = "length=" + input;
					return;
				}
			}
			this.value = null;
		}

        /// <summary>
        /// Retorna todo os tipos derivados do tipo ContainerField.
        /// </summary>
        /// <returns>Enumerable de tipos.</returns>
        public static IEnumerable<Type> GetKnownTypes()
        {
            var knownTypes = default(IEnumerable<Type>);
            Type baseType = typeof(ContainerField);
            var allTypes = default(IEnumerable<Type>);
            Assembly assembly = baseType.Assembly;

            allTypes = assembly.GetTypes().OfType<Type>();

            knownTypes = allTypes.Where(t => t != baseType && t.IsSubclassOf(baseType));
            return knownTypes;
        }
	}
}
