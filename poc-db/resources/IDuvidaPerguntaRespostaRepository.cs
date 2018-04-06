/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// IDuvidaPerguntaRespostaRepository
    /// </summary>
    public interface IDuvidaPerguntaRespostaRepository
    {
        /// <summary>
        /// </summary>
        /// <param name="web"></param>
        /// <returns>List<DuvidaCategoria></returns>
        List<DuvidaPerguntaResposta> GetAllItems(SPWeb web, String canalExibicao = null);
    }
}
