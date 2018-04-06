/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Hist�rico:
 - 26/12/2012 � William Resendes Raposo � Vers�o inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Redecard.PN.FMS.Comum.Log
{
    /// <summary>
    /// Este componente publica a classe DirectReflector, � estendida de BaseReflector, que exp�e m�todos para manipular fun��es que utilziam reflector.
    /// </summary>
    class DirectReflector : BaseReflector
    {
        /// <summary>
        /// Este m�todo � utilizado para extrair valores de um membro, de uma determinada inst�ncia.
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
        /// Este m�todo � utilizado para extrair o handler de um determinado m�todo.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public override MethodHandler GetDelegate(MethodBase method)
        {
            return (instance, args) => method.Invoke(instance, args);
        }
    }
}
