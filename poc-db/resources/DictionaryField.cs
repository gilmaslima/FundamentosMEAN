/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DictionaryField
    /// </summary>
    [Serializable]
    [DataContract]
    public class DictionaryField : ContainerField
	{
        /// <summary>
        /// Construtor DictionaryField
        /// </summary>
        public DictionaryField()
        { 
        }

        /// <summary>
        /// Construtor DictionaryField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public DictionaryField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        ///  Cria um objeto Dictionary baseado na lista de Fiels recebidos.
        /// </summary>
        /// <param name="typeName">Nome do tipo.</param>
        /// <param name="fields">Filds para popular o Dictionary</param>
        /// <param name="invalidList">Lista dos campos que não foram válidos para tipo de Dictionary.</param>
        /// <returns>Dictionary.</returns>
        public static Object CreateAndValidateDictionary(String typeName, Field[] fields, out List<Int32> invalidList)
		{

			Type type = DataContractAnalyzer.TypesCache[typeName];
			Object obj = Activator.CreateInstance(type);
			invalidList = new List<Int32>();

			if (fields != null)
			{
				MethodInfo method = type.GetMethod("Add");
				if (method == null)
				{
					return null;
				}

				int num = 0;
				for (Int32 i = 0; i < fields.Length; i++)
				{
					KeyValuePairField keyValuePairField = (KeyValuePairField)fields[i];
					if (keyValuePairField != null && keyValuePairField.IsValid)
					{
                        var array = new Object[2];
						Field[] childFields = keyValuePairField.GetChildFields();
						array[0] = childFields[0].CreateObject();
						array[1] = childFields[1].CreateObject();

                        method.Invoke(obj, array);

						num++;
					}
				}
			}
			return obj;
		}

        /// <summary>
        /// Cria um objeto do tipo Dictionary baseado no atributo value.
        /// </summary>
        /// <returns>Instância de um objeto Dictionary.</returns>
        public override Object CreateObject()
		{
			if (String.Compare(this.value, "(null)") == 0)
			{
				return null;
			}
			base.GetChildFields();
			List<Int32> list = null;
			return DictionaryField.CreateAndValidateDictionary(this.CurrentMember.TypeName, this.ChildFields, out list);
		}

        /// <summary>
        /// Valida se o input possui um formato válido de lenght da coleção.
        /// </summary>
        /// <param name="input">Valor para a validação.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			base.ValidateAndCanonicalize(input);
			if (this.value != null)
			{
				base.GetChildFields();
				this.Validate();
			}
		}

        /// <summary>
        /// Valida se é possivel criar um Dictionary com os ChildFields.
        /// </summary>
        /// <returns></returns>
        public IList<Int32> ValidateDictionary()
		{
            List<Int32> result = null;
			DictionaryField.CreateAndValidateDictionary(base.TypeName, this.ChildFields, out result);
			return result;
		}

        /// <summary>
        ///  Seta a propriedade IsValid de todos os fields inválidos para o tipo de Dictionary atual.
        /// </summary>
		private void Validate()
		{
			if (this.ChildFields != null)
			{
				Field[] childFields = this.ChildFields;
				for (Int32 i = 0; i < childFields.Length; i++)
				{
					KeyValuePairField keyValuePairField = (KeyValuePairField)childFields[i];
					keyValuePairField.IsValid = true;
				}
				IList<Int32> list = ServiceExecutor.ValidateDictionary(this, AppDomain.CurrentDomain);
				foreach (Int32 current in list)
				{
					((KeyValuePairField)this.ChildFields[current]).IsValid = false;
				}
			}
		}
	}
}
