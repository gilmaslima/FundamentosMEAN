/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe EqualityComparer
    /// </summary>
    public class EqualityComparer : IEqualityComparer<Object>
    {
        /// <summary>
        /// Test if object x reference is equals objcect y reference.
        /// </summary>
        /// <param name="x">Object</param>
        /// <param name="y">Object</param>
        /// <returns>Comparer Result</returns>
        public new Boolean Equals(Object x, Object y)
        {
            return Object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// Get the Object Hash Code.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Hash Code.</returns>
        public Int32 GetHashCode(Object obj)
        {
            return obj.GetHashCode();
        }
    }
}