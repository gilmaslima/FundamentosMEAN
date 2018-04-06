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
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using Redecard.PN.FMS.Comum.Log;

namespace Redecard.PN.FMS.Comum
{
    public class UtilHelper
    {

        /// <summary>
        /// Método que realiza a serialização da classe para gravação no log.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="separadorCampos"></param>
        /// <returns></returns>
        public static string SerializarDados(object graph)
        {
            Serializer serializer = new Serializer(new DirectReflector());
            return serializer.Serialize(graph);
        }

    }
}
