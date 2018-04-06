/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe KeyValuePairField
    /// </summary>
    [Serializable]
    [DataContract]
    public class KeyValuePairField : Field
	{
        /// <summary>
        /// duplicateKeyMark
        /// </summary>
		private const String duplicateKeyMark = "[ # ]";

        /// <summary>
        /// isValid
        /// </summary>
		private Boolean isValid = true;

        /// <summary>
        /// IsValid
        /// </summary>
        [DataMember]
        public Boolean IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				this.isValid = value;
			}
		}

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public override String Name
		{
			get
			{
				if (this.isValid)
				{
					return base.Name;
				}
				return "[ # ]";
			}
			set
			{
				base.Name = value;
			}
		}


        /// <summary>
        /// Construtor KeyValuePairField.
        /// </summary>
        public KeyValuePairField()
        {
        }

        /// <summary>
        /// Construtor KeyValuePairField.
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public KeyValuePairField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Criar um objeto do tipo KeyValuePair baseado no atributo value.
        /// </summary>
        /// <returns>Novo objeto KeyValuePair.</returns>
        public override Object CreateObject()
		{
			base.GetChildFields();
			Type type = DataContractAnalyzer.TypesCache[this.CurrentMember.TypeName];
			return Activator.CreateInstance(type, new Object[]
			{
				this.ChildFields[0].CreateObject(),
				this.ChildFields[1].CreateObject()
			});
		}
	}
}
