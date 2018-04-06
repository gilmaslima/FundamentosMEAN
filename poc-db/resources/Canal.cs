using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Request.Servicos
{
    /// <summary>
    /// Enumerador de Canal de Recebimento
    /// </summary>
    /// <remarks>
    /// Encapsula o canal de recebimento retornado pelo Serviço HIS do Book:<br/>
    /// - Book BXA380 / Programa XA380 / TranID IS63
    /// </remarks>
    [DataContract]
    public enum CanalRecebimento
    {
        /// <summary>Não definido</summary>
        [EnumMember]
        NaoDefinido                 = 0,
        /// <summary>Carta para PV - Correio</summary>
        [EnumMember]
        CartaParaPV_Correio         = 1,
        /// <summary>Fax para Centralizadora</summary>
        [EnumMember]
        FaxParaCentralizadora       = 2,
        /// <summary>Carta para Centralizadora</summary>
        [EnumMember]
        CartaParaCentralizadora     = 3,
        /// <summary>Rel. para Centralizadora - Fax</summary>
        [EnumMember]
        RelParaCentralizadora_Fax   = 4,
        /// <summary>Rel. para Centralizadora - Papel</summary>
        [EnumMember]
        RelParaCentralizadora_Papel = 5,
        /// <summary>Não gerar</summary>
        [EnumMember]
        NaoGera                     = 6,
        /// <summary>Fax para PV - Formato Carta</summary>
        [EnumMember]
        FaxParaPV_FormatoCarta      = 7,
        /// <summary>Internet</summary>
        [EnumMember]
        Internet                    = 8
    }

    /// <summary>Enumerador de Canal de Envio</summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BXA780 / Programa XA780 / TranID IS39<br/>
    /// - Book BXD201CB / Programa XD201 / TranID XDS1
    /// </remarks>    
    [DataContract]
    public enum CanalEnvio
    {
        /// <summary>Indefinido</summary>        
        [EnumMember]        
        Indefinido     = 0,
        /// <summary>Correio</summary>
        [EnumMember]
        Correio         = 1,
        /// <summary>Fax</summary>
        [EnumMember]
        Fax             = 2,
        /// <summary>E-mail</summary>
        [EnumMember]
        Email           = 3,
        /// <summary>Bloqueado Envio</summary>
        [EnumMember]
        BloqueadoEnvio  = 4,
        /// <summary>Não Envia</summary>
        [EnumMember]        
        NaoEnvia = 6
    }

    /// <summary>Classe modelo de Canal para armazenamento das informações do Canal de Recebimento de um PV.</summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book BXA380 / Programa XA380 / TranID IS63
    /// </remarks>
    [DataContract]
    public class Canal
    {
        /// <summary>Canal de recebimento</summary>
        [DataMember]
        public CanalRecebimento CanalRecebimento { get; set; }

        /// <summary>Descrição do canal</summary>
        [DataMember]
        public String DescricaoCanal { get; set; }
    }
}