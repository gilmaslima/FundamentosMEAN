/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DataTableField
    /// </summary>
    [Serializable]
    [DataContract]
    public class DataTableField : Field
    {
        #region [Attributes and Properties]
        /// <summary>
        /// dataTableValue
        /// </summary>
        [DataMember]
        private readonly DataTable dataTableValue;

        /// <summary>
        /// DataTableValue
        /// </summary>
        public DataTable DataTableValue 
        {
            get { return this.dataTableValue; }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Construtor DataTableField
        /// </summary>
        public DataTableField()
        {
 
        }

        /// <summary>
        /// Construtor DataTableField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        /// <param name="obj">Objeto do tipo DataTable.</param>
        public DataTableField(TypeMemberInfo declaredMember, Object obj)
            : base(declaredMember, obj)
        {
            if (obj != null)
                this.dataTableValue = ((DataTable)obj).Copy();
        } 

        #endregion

        #region [Methods]
        
        /// <summary>
        /// Cria um objeto do tipo DataTable baseado no atributo value.
        /// </summary>
        /// <returns>Instância de um objeto do tipo DataTable.</returns>
        public override Object CreateObject()
        {
            if (String.Compare(this.value, "(null)") == 0)
            {
                return null;
            }

            if (this.dataTableValue != null)
            {
                return this.dataTableValue;
            }

            Type type = ClientSettings.GetType(this.CurrentMember.TypeName);
            Object obj = Activator.CreateInstance(type);

            return obj;
        }

        #endregion


    }
}