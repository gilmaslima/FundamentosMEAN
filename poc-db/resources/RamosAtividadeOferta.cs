/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Ramos de Atividade da Oferta
    /// </summary>
    [DataContract]
    public class RamosAtividadeOferta
    {
        /// <summary>
        /// Código do Ramo de Atividade
        /// </summary>
        [DataMember]
        public Int64 CodigoRamoAtividade { get; set; }

        /// <summary>
        /// Descrição do Ramo de Atividade
        /// </summary>
        [DataMember]
        public String DescricaoRamoAtividade { get; set; }

        /// <summary>
        /// Código do Grupo Gerencial
        /// </summary>
        [DataMember]
        public Int64 CodigoGrupoGerencial { get; set; }
    }
}