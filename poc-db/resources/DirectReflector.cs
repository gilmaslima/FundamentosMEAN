/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Redecard.PN.FMS.Comum.Log
{
    /// <summary>
    /// Este componente publica a classe DirectReflector, é estendida de BaseReflector, que expõe métodos para manipular funções que utilziam reflector.
    /// </summary>
    class DirectReflector : BaseReflector
    {
        /// <summary>
        /// Este método é utilizado para extrair valores de um membro, de uma determinada instância.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override object GetValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).GetGetMethod().Invoke(instance, null);
            if (member is FieldInfo)
                return (member as FieldInfo).GetValue(instance);
            throw new NotImplementedException();
        }
        /// <summary>
        /// Este método é utilizado para extrair o handler de um determinado método.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public override MethodHandler GetDelegate(MethodBase method)
        {
            return (instance, args) => method.Invoke(instance, args);
        }
    }
}
