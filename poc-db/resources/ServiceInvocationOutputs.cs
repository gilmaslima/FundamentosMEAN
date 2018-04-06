/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceInvocationOutputs
    /// </summary>
	[Serializable]
	public class ServiceInvocationOutputs
	{
        /// <summary>
        /// serviceInvocationResult
        /// </summary>
		private readonly Field[] serviceInvocationResult;

        /// <summary>
        /// ServiceInvocationResult
        /// </summary>
        public Field[] ServiceInvocationResult
        {
            get
            {
                return this.serviceInvocationResult;
            }
        }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        public String ErrorMessage { get; set; }

        /// <summary>
        /// HasError
        /// </summary>
        public Boolean HasError { get; set; }

        /// <summary>
        /// Construtor ServiceInvocationOutputs
        /// </summary>
        /// <param name="serviceInvocationResult">Array de Field com os resultados retornados pela chamada do serviço.</param>
		public ServiceInvocationOutputs(Field[] serviceInvocationResult)
		{
			this.serviceInvocationResult = serviceInvocationResult;
		}

        /// <summary>
        /// Construtor ServiceInvocationOutputs
        /// </summary>
        public ServiceInvocationOutputs()
        {
 
        }
	}
}
