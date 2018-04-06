#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Alexandre Shiroma]
Empresa     : [Iteris]
Histórico   :
- [24/07/2012] – [Alexandre Shiroma] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Classe Base para os modelos do serviço
    /// </summary>
    [DataContract]
    public class ModeloBase
    {
        /// <summary>
        /// Código do Modelo
        /// </summary>
        [DataMember]
        public virtual Int32 Codigo { get; set; }

        /// <summary>
        /// Descrição do Modelo
        /// </summary>
        [DataMember]
        public virtual String Descricao { get; set; }
    }
}