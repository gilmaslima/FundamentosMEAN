/*
© Copyright 2017 Rede S.A.
Autor : LUE
Empresa : Iteris Consultoria e Software
*/
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// Implementa interface IWarningsAtendimento
    /// </summary>
    public interface IWarningsAtendimento
    {
        /// <summary>
        /// Obtém uma lista de warnings.
        /// </summary>
        /// <param name="web">SPWeb.</param>
        /// <returns>Lista de Warnings do Atendimento Digital.</returns>
        List<WarningsAtendimentoEntity> GetAll(SPWeb web);
    }
}
