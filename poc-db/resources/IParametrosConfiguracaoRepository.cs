/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// IAtendimentoDigitalRepository
    /// </summary>
    public interface IParametrosConfiguracao
    {

        /// <summary>
        /// Busca item de acordo com name
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        ParametrosConfiguracao Get(SPWeb web, String name);

        /// <summary>
        /// Busca todos os itens da lista
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        List<ParametrosConfiguracao> GetAll(SPWeb web);


    }
}
