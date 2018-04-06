#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [17/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Representa um material na composição de um Kit
    /// </summary>
    [DataContract]
    public class Material
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int32 CodigoMaterial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String DescricaoMaterial { get; set; }
    }
}