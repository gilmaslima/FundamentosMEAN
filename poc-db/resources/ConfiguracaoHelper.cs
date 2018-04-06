/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Hist�rico:
 - 18/12/2012 � William Resendes Raposo � Vers�o inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe ConfiguracaoHelper, que exp�e m�todos para manipular as configura��es de mensagens de erro.
    /// </summary>
    public static class ConfiguracaoHelper
    {
        /// <summary>
        /// Este m�todo � utilizado para obter o valor configura��o.
        /// </summary>
        /// <param name="nomeChave"></param>
        /// <returns></returns>
        public static string ObterValorConfiguracao(string nomeChave)
        {
            try
            {
                AppSettingsReader configDb = new System.Configuration.AppSettingsReader();
                return (string) configDb.GetValue(nomeChave, typeof(string));
            }
            catch (InvalidOperationException e)
            {
                return null;
            }

        }
        /// <summary>
        /// Este m�todo � utilizado para descobrir qual ambiente.
        /// </summary>
        /// <returns></returns>
        public static bool EhAmbienteLocal()
        {
            return (System.Environment.GetEnvironmentVariable("AMBIENTETESTE_RESOURCE", EnvironmentVariableTarget.Machine) != null);
        }
    }
}
