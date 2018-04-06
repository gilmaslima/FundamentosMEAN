using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Redecard.Portal.Helper.Web.Controles
{
    /// <summary>
    /// Classe utilitária com métodos de extensão para o Portal
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Cria uma cópia de um objeto LiteralControl considerando seu Texto e ID
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static LiteralControl BasicClone(this LiteralControl literal)
        {
            LiteralControl clonedLiteralControl = new LiteralControl();
            clonedLiteralControl.Text = literal.Text;
            clonedLiteralControl.ID = literal.ID;

            return clonedLiteralControl;
        }

        /// <summary>
        /// Converte objeto em DateTime
        /// </summary>
        /// <param name="pObject">Objeto a ser convertido</param>
        /// <returns>Retorna objeto convertido em DateTime, caso não consiga retorna DateTime.MinValue</returns>
        public static DateTime ToDateTime(this object pObject)
        {
            if (pObject == null || pObject == DBNull.Value) return DateTime.MinValue;

            DateTime lReturn;

            if (DateTime.TryParse(pObject.ToString(), out lReturn))
                return lReturn;            
            else            
                return DateTime.MinValue;            
        }

        /// <summary>
        /// Converte objeto em Int
        /// </summary>
        /// <param name="pObject">Objeto a ser convertido</param>
        /// <returns>Retorna objeto convertido em Int, caso não consiga retorna 0</returns>
        public static int ToInt(this object pObject)
        {
            if (pObject == null || pObject == DBNull.Value) return 0;

            int lReturn;

            if (int.TryParse(pObject.ToString(), out lReturn))
                return lReturn;
            else
                return 0;
        }
    }
}
