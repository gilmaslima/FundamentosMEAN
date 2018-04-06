/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Redecard.PN.FMS.Sharepoint.Interfaces
{
    /// <summary>
    /// Interface forcenendo metodos e marcacao relativos a Exportacao dos dados nas telas de consulta e analise.
    
    /// </summary>
    public interface IPossuiExportacao
    {
        void Exportar();
    }
}
