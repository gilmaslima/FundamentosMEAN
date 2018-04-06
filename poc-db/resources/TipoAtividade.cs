/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// Classe modelo para Tipo de Atividade do Histórico de Atividades
    /// </summary>
    [DataContract]
    public class TipoAtividade
    {
        /// <summary>
        /// Código do Tipo da Atividade
        /// </summary>
        [DataMember]
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Título do Tipo de Atividade
        /// </summary>
        [DataMember]
        public String Titulo { get; set; }

        /// <summary>
        /// Descrição do Tipo de Atividade
        /// </summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>
        /// Flag exibir
        /// </summary>
        [DataMember]
        public Boolean Exibir { get; set; }
    }
}