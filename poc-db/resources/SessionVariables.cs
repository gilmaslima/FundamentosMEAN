/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.Sustentacao.SharePoint.Helpers
{
    /// <summary>
    ///  Classe SessionVariables
    /// </summary>
    public static class SessionVariables
    {
        /// <summary>
        /// currentServices
        /// </summary>
        private const String currentServices = "CurrentServices";

        /// <summary>
        /// currentsResults
        /// </summary>
        private const String currentsResults = "CurrentsResults";

        /// <summary>
        /// currentsParameters
        /// </summary>
        private const String currentsParameters = "CurrentsParameters";
        
        /// <summary>
        /// CurrentServices
        /// </summary>
        public static String CurrentServices 
        {
            get { return currentServices; } 
        }

        /// <summary>
        /// CurrentsResults
        /// </summary>
        public static String CurrentsResults
        { 
            get { return currentsResults; } 
        }

        /// <summary>
        /// CurrentParameters
        /// </summary>
        public static String CurrentParameters
        {
            get { return currentsParameters; }
        }
    }
}
