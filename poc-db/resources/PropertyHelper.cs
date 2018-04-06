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
using System.Collections;
using Redecard.PN.FMS.Comum.Log;

namespace Redecard.PN.FMS.Comum.Helpers
{
    /// <summary>
    /// Este componente publica a classe PropertyHelper, que expõe métodos para manipular as proprieddes do sistema de ajuda.
    /// </summary>
    public class PropertyHelper
    {
        /// <summary>
        /// Método que copia as propriedades de um vo origem para o destino
        /// ATENÇÃO: NÃO COPIA A LISTA, OU SEJA, CLASSES QUE IMPLEMENTAM ILIST OU IENUMERABLE
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopiaPropriedades<From, To>(From from, ref To to)
        {
            if (from == null)
                return;
            
            Type de = from.GetType();
            Type para = to.GetType();

            foreach (PropertyInfo prop in de.GetProperties())
            {
                object valor = prop.GetValue(from,null);

                PropertyInfo propriedadeEncontradaNoDestino = Array.Find(para.GetProperties(), p => p.Name == prop.Name);

                if(propriedadeEncontradaNoDestino != null)
                {
                    if (propriedadeEncontradaNoDestino.PropertyType.IsPrimitive || propriedadeEncontradaNoDestino.PropertyType == typeof(string) || propriedadeEncontradaNoDestino.PropertyType == typeof(DateTime) || propriedadeEncontradaNoDestino.PropertyType == typeof(Decimal))
                    {
                        propriedadeEncontradaNoDestino.SetValue(to, valor, null);
                    }
                    else if (propriedadeEncontradaNoDestino.PropertyType.IsEnum)
                    {
                        propriedadeEncontradaNoDestino.SetValue(to, Convert.ToInt32(valor), null);
                    }
                    else if ( ReflectionHelper.IsGenericList(propriedadeEncontradaNoDestino.PropertyType))
                    {

                        continue;
                    }
                    else
                    {
                        object valorClasseDefinidaProjeto = Activator.CreateInstance(propriedadeEncontradaNoDestino.PropertyType);

                        CopiaPropriedades<object, object>(valor, ref valorClasseDefinidaProjeto);

                        propriedadeEncontradaNoDestino.SetValue(to, valorClasseDefinidaProjeto, null);
                    }
                }                                
            }            
        }

         /// <summary>
        /// Método que copia as propriedades da lista para uma outra lista
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopiaLista<From, To>(List<From> from, List<To> to)
        {
            if (from == null || to == null)
                return;
              to.AddRange(from.ConvertAll<To>(new Converter<From, To>(
              delegate(From f)
                {
                   To t = (To)Activator.CreateInstance(typeof(To));
                   CopiaPropriedades<From, To>(f,ref t);
                   return t;
                })));
        }
    }
}
