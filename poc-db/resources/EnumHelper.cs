/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe EnumHelper, que expõe métodos para manipular as enjmerações utilizadas pelo helper.
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Este método é utilizado para converter enumerações
        /// </summary>
        /// <typeparam name="From"></typeparam>
        /// <typeparam name="To"></typeparam>
        /// <param name="enumArg"></param>
        /// <returns></returns>
        public static To EnumToEnum<From, To>(From enumArg)
        {

            if (!typeof(To).IsEnum) throw new Exception("Este metodo suporta apenas enums.");

            if (!typeof(From).IsEnum) throw new Exception("Este metodo suporta apenas enums");

            try
            {

                return (To)Enum.ToObject(typeof(To), enumArg);

            }

            catch
            {

                throw new Exception(string.Format("Erro durante a conversao do enum {0} de valor {1} para o enum {2} ",

                enumArg.ToString(),

                typeof(From).ToString(),

                typeof(To).ToString()));

            }

        }

    }

}
