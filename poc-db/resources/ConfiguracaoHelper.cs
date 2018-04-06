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
using System.Configuration;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe ConfiguracaoHelper, que expõe métodos para manipular as configurações de mensagens de erro.
    /// </summary>
    public static class ConfiguracaoHelper
    {
        /// <summary>
        /// Este método é utilizado para obter o valor configuração.
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
        /// Este método é utilizado para descobrir qual ambiente.
        /// </summary>
        /// <returns></returns>
        public static bool EhAmbienteLocal()
        {
            return (System.Environment.GetEnvironmentVariable("AMBIENTETESTE_RESOURCE", EnvironmentVariableTarget.Machine) != null);
        }
    }
}
