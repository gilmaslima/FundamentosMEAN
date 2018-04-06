#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [26/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe de ocorrências de solicitações
    /// </summary>
    public class Ocorrencia
    {
        /// <summary>
        /// Código do tipo de Ocorrência
        /// </summary>
        public String CodigoTipoOcorrencia { get; set;}

        /// <summary>
        /// Nome da ocorrência
        /// </summary>
        public String NomeOcorrencia { get; set; }
    }
}
