/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe NumericField
    /// </summary>   
    [Serializable]
    [DataContract]    
    public class NumericField : Field
	{
        /// <summary>
        /// parseMethod
        /// </summary>
        private MethodInfo parseMethod;

        /// <summary>
        /// Construtor NumericField
        /// </summary>
        public NumericField()
        {

        }

        /// <summary>
        /// Construtor NumericField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public NumericField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
			this.parseMethod = Type.GetType(this.CurrentMember.TypeName).GetMethod("Parse", new Type[]
			{
				typeof(String),
				typeof(IFormatProvider)
			});
		}

        /// <summary>
        /// Cria um novo objeto numérico baseado no atributo value.
        /// </summary>
        /// <returns>Novo objeto numérico.</returns>
        public override Object CreateObject()
		{
            if (this.parseMethod == null)
            {
                this.parseMethod = Type.GetType(this.CurrentMember.TypeName).GetMethod("Parse", new Type[]
                {
                    typeof(String),
				    typeof(IFormatProvider)
    		    });
            }

			return this.parseMethod.Invoke(null, new Object[]
			{
				this.value,
				CultureInfo.CurrentUICulture
			});
		}

        /// <summary>
        /// Valida se o input pode ser convertido para o tipo numérico do CurrentMember.
        /// </summary>
        /// <param name="input">Valor para a validação.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			Type type = Type.GetType(this.CurrentMember.TypeName);
			Object[] arrayInput = new object[2];
			arrayInput[0] = input;
			Object[] arrayAux = arrayInput;

			MethodInfo method = type.GetMethod("TryParse", new Type[]
			{
				typeof(String),
				Type.GetType(this.CurrentMember.TypeName + "&")
			});

            var isNumeric = (Boolean)method.Invoke(null, arrayAux);

			if (isNumeric)
			{
				this.value = arrayAux[1].ToString();
				return;
			}

			this.value = null;
		}
	}
}
