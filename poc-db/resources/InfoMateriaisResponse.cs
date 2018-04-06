/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Representa Response para o método ConsultarInfoMateriais - que retorna as informações pertinentes à solicitação de materiais.
    /// </summary>
    [DataContract]
    public class InfoMateriaisResponse
    {
        /// <summary>
        /// Possui Solicitação Material Aberta?
        /// </summary>
        [DataMember]
        public Boolean PossuiKitsMateriais { get; set; }

        /// <summary>
        /// Possui Solicitação Material Aberta?
        /// </summary>
        [DataMember]
        public Boolean PossuikitsSinalizacao{ get; set; }
        
        /// <summary>
        /// Possui Solicitação Material Aberta?
        /// </summary>
        [DataMember]
        public Boolean PossuiSolicitacaoMaterialAberta { get; set; }
        
        /// <summary>
        /// Possui Solicitação Material Aberta?
        /// </summary>
        [DataMember]
        public Boolean PossuiSolicitacaoSinalizacaoAberta { get; set; }
    }
}