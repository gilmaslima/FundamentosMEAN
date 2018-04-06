#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [30/04/2012] – [André Rentes] – [Criação]
- [26/11/2015] – [Rodrigo Rodrigues] – Migração para novo projeto (Redecard.PN.Sustentacao.Servicos)
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.Servicos
{
    /// <summary>
    /// Classe padrão de Exceção do serviço
    /// </summary>
    [DataContract]
    public class GeneralFault
    {
        public GeneralFault(Int32 codigo, String fonte)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }
                
        /// <summary>
        /// Nome do serviço que ocorreu o erro
        /// </summary>
        [DataMember]
        public String Fonte { get; set; }

        /// <summary>
        /// Código da exceção
        /// </summary>
        [DataMember]
        public Int32 Codigo { get; set; }
    }
    
}