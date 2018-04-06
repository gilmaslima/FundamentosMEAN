/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/


using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe TimeSpanField
    /// </summary>
    [Serializable]
    [DataContract]
    public class TimeSpanField : Field
	{
        /// <summary>
        /// Construtor TimeSpanField
        /// </summary>
        public TimeSpanField()
        { 
        }

        /// <summary>
        /// Construtor TimeSpanField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public TimeSpanField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto do tipo TimeSpan
        /// </summary>
        /// <returns>Novo objeto TimeSpan.</returns>
		public override Object CreateObject()
		{
			return new TimeSpanConverter().ConvertFrom(this.value);
		}

        /// <summary>
        /// Valida se o input pode ser convertido para o tipo TimeSpan.
        /// </summary>
        /// <param name="input">Valor para a validação.</param>
		public override void ValidateAndCanonicalize(String input)
		{
            try
            {
                this.value = new TimeSpanConverter().ConvertFrom(input).ToString();
            }
            catch (FormatException)
            {
                this.value = null;                
                return;
            }
            catch (Exception ex)
            {
                this.value = null;                
                return; 
            }

			base.ValidateAndCanonicalize(this.value);
		}
	}
}
