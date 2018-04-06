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
    public interface IAtendimentoDigitalRepository
    {

        /// <summary>
        /// Busca os arquivos de acordo com a extensão da pasta especificada
        /// </summary>
        /// <param name="web"></param>
        /// <param name="urlFolder">Ex: AtendimentoDigital/email/faleconosco</param>
        /// <param name="fileExtension">png</param>
        /// <returns>List<AtendimentoDigital></returns>
        List<AtendimentoDigital> Get(SPWeb web, String urlFolder, String fileExtension);

    }
}
