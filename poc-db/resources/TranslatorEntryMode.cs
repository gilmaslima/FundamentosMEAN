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
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorEntryMode, e expõe métodos para traduzir os dados de modos de entrada do webservice
    /// </summary>
    public class TranslatorEntryMode
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de mode de entrada do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static EntryMode TranslateEntryModeWSMEntryModeBusiness(entryMode from)
        {
            EntryMode to = new EntryMode();
            to.Codigo = from.code;
            to.Descricao= from.description;

            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de mode de entrada do webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static entryMode TranslateEntryModeBusinessEntryModeWS(EntryMode from)
        {
            entryMode to = new entryMode();
            to.code = from.Codigo;
            to.description = from.Descricao;

            return to;
        }
    }
}
