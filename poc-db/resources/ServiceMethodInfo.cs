/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceMethodInfo
    /// </summary>
	[Serializable]
    [DataContract]
	public class ServiceMethodInfo
    {
        #region [Attributes and Properties]

        /// <summary>
        /// endpoint
        /// </summary>
        private readonly ClientEndpointInfo endpoint;

        /// <summary>
        /// isOneWay
        /// </summary>
        private readonly Boolean isOneWay;

        /// <summary>
        /// methodName
        /// </summary>
        [DataMember]
        private readonly String methodName;

        /// <summary>
        /// invalidParameters
        /// </summary>
        private List<TypeMemberInfo> invalidParameters;

        /// <summary>
        /// otherParameters
        /// </summary>
        private List<TypeMemberInfo> otherParameters = new List<TypeMemberInfo>();

        /// <summary>
        /// parameters
        /// </summary>
        private List<TypeMemberInfo> parameters = new List<TypeMemberInfo>();

        /// <summary>
        /// Endpoint
        /// </summary>
        public ClientEndpointInfo Endpoint
        {
            get
            {
                return this.endpoint;
            }
        }

        /// <summary>
        /// InputParameters
        /// </summary>
        public IList<TypeMemberInfo> InputParameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// InvalidMembers
        /// </summary>
        public List<TypeMemberInfo> InvalidMembers
        {
            get
            {
                if (this.invalidParameters == null)
                {
                    this.invalidParameters = new List<TypeMemberInfo>();
                    this.parameters.Find(new Predicate<TypeMemberInfo>(this.CheckAndSaveInvalidMembers));
                    this.otherParameters.Find(new Predicate<TypeMemberInfo>(this.CheckAndSaveInvalidMembers));
                }
                return this.invalidParameters;
            }
        }

        /// <summary>
        /// IsOneWay
        /// </summary>
        public Boolean IsOneWay
        {
            get
            {
                return this.isOneWay;
            }
        }

        /// <summary>
        /// MethodName
        /// </summary>
        public String MethodName
        {
            get
            {
                return this.methodName;
            }
        }

        /// <summary>
        /// OtherParameters
        /// </summary>
        public IList<TypeMemberInfo> OtherParameters
        {
            get
            {
                return this.otherParameters;
            }
        }

        /// <summary>
        /// Valid
        /// </summary>
        public Boolean Valid
        {
            get
            {
                return this.InvalidMembers.Count == 0;
            }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Construtor ServiceMethodInfo
        /// </summary>
        /// <param name="endpoint">Objeto ClientEnpointInfo.</param>
        /// <param name="methodName">Nome do método.</param>
        /// <param name="isOneWay">Tipo do método.</param>
        public ServiceMethodInfo(ClientEndpointInfo endpoint, String methodName, Boolean isOneWay)
        {
            this.endpoint = endpoint;
            this.methodName = methodName;
            this.isOneWay = isOneWay;
        }
        
        #endregion

        #region [Methods]

        /// <summary>
        /// Cria array Field com uma instância de cada parameter.
        /// </summary>
        /// <returns>Array de Field.</returns>
        public Field[] GetFields()
        {
            Field[] fields = new Field[this.parameters.Count];
            Int32 index = 0;

            foreach (TypeMemberInfo current in this.parameters)
            {
                fields[index] = FieldFactory.CreateAssociateField(current);
                fields[index].SetServiceMethodInfo(this);
                index++;
            }

            return fields;
        }

        /// <summary>
        /// Verifica se o member informado é inválido.
        /// </summary>
        /// <param name="member">Objeto TyperMemberInfo que será verificado.</param>
        /// <returns>Resultado da verificação.</returns>
        private Boolean CheckAndSaveInvalidMembers(TypeMemberInfo member)
        {
            if (member.IsInvalid)
            {
                this.invalidParameters.Add(member);
            }

            return false;
        }
        
        #endregion
	}
}
