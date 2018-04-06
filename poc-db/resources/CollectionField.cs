/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe CollectionField
    /// </summary>
    [Serializable]
    [DataContract]
	public class CollectionField : ContainerField
	{
        /// <summary>
        /// Construtor CollectionField
        /// </summary>
        public CollectionField()
        { 
        }

        /// <summary>
        /// Construtor CollectionField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public CollectionField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto Collection com todos os seus sub-tipos.
        /// </summary>
        /// <returns>Objeto do tipo collection.</returns>
        public override Object CreateObject()
		{
			if (String.IsNullOrWhiteSpace(this.value))
			{
				return null;
			}

			base.GetChildFields();
			Type type = ClientSettings.GetType(this.CurrentMember.TypeName);
            Object obj = Activator.CreateInstance(type);

			if (this.ChildFields != null)
			{
				MethodInfo method = type.GetMethod("Add");
				Field[] childFields = this.ChildFields;
				for (Int32 i = 0; i < childFields.Length; i++)
				{
					Field field = childFields[i];
                    method.Invoke(obj, new Object[]
					{
						field.CreateObject()
					});
				}
			}

			return obj;
		}
	}
}
