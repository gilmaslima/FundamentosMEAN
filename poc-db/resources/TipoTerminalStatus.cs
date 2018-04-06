/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TipoTerminalStatus
    /// </summary>
    [DataContract]
    public enum TipoTerminalStatus
    {
        /// <summary>
        /// Aguardando coleta
        /// </summary>
        [EnumMember]
        AGUARDANDOCOLETA,

        /// <summary>
        /// Bad para reparo
        /// </summary>
        [EnumMember]
        BADPARAREPARO,

        /// <summary>
        /// Cancelado
        /// </summary>
        [EnumMember]
        CANCELADO,

        /// <summary>
        /// Desativado
        /// </summary>
        [EnumMember]
        DESATIVADO,

        /// <summary>
        /// Devolvido
        /// </summary>
        [EnumMember]
        DEVOLVIDO,

        /// <summary>
        /// Disp distribuição
        /// </summary>
        [EnumMember]
        DISPDISTRIBUICAO,

        /// <summary>
        /// Disp evento
        /// </summary>
        [EnumMember]
        DISPEVENTO,

        /// <summary>
        /// disp fct específica
        /// </summary>
        [EnumMember]
        DISPFCTESPECIFICA,

        /// <summary>
        /// Disp instalação
        /// </summary>
        [EnumMember]
        DISPINSTALACAO,

        /// <summary>
        /// Disp piloto
        /// </summary>
        [EnumMember]
        DISPPILOTO,

        /// <summary>
        /// Disp projeto
        /// </summary>
        [EnumMember]
        DISPPROJETO,

        /// <summary>
        /// Disp VIP
        /// </summary>
        [EnumMember]
        DISPVIP,

        /// <summary>
        /// Divergente
        /// </summary>
        [EnumMember]
        DIVERGENTE,

        /// <summary>
        /// Emana lis e fraude
        /// </summary>
        [EnumMember]
        EMANALISEFRAUDE,

        /// <summary>
        /// Em laboratório
        /// </summary>
        [EnumMember]
        EMLABORATORIO,

        /// <summary>
        /// Em produção
        /// </summary>
        [EnumMember]
        EMPRODUCAO,

        /// <summary>
        /// Em sala segura
        /// </summary>
        [EnumMember]
        EMSALASEGURA,

        /// <summary>
        /// Em trans CD
        /// </summary>
        [EnumMember]
        EMTRANSCD,

        /// <summary>
        /// Em trans CREd
        /// </summary>
        [EnumMember]
        EMTRANSCRED,

        /// <summary>
        /// em trans evento
        /// </summary>
        [EnumMember]
        EMTRANSEVENTO,

        /// <summary>
        /// Em trans lab
        /// </summary>
        [EnumMember]
        EMTRANSLAB,

        /// <summary>
        /// Em trans PV
        /// </summary>
        [EnumMember]
        EMTRANSPV,

        /// <summary>
        /// Em trans PV espec
        /// </summary>
        [EnumMember]
        EMTRANSPVESPEC,

        /// <summary>
        /// Em triagem
        /// </summary>
        [EnumMember]
        EMTRIAGEM,

        /// <summary>
        /// Fora de produção
        /// </summary>
        [EnumMember]
        FORADEPRODUCAO,

        /// <summary>
        /// Inativo
        /// </summary>
        [EnumMember]
        INATIVO,

        /// <summary>
        /// Inutilizado
        /// </summary>
        [EnumMember]
        INUTILIZADO,

        /// <summary>
        /// Lacrado
        /// </summary>
        [EnumMember]
        LACRADO,

        /// <summary>
        /// Liberado triagem
        /// </summary>
        [EnumMember]
        LIBERADOTRIAGEM,

        /// <summary>
        /// Montando kit
        /// </summary>
        [EnumMember]
        MONTANDOKIT,

        /// <summary>
        /// Não comissionado
        /// </summary>
        [EnumMember]
        NAOCOMISSIONADO,

        /// <summary>
        /// Não localizado
        /// </summary>
        [EnumMember]
        NAOLOCALIZADO,

        /// <summary>
        /// Não pronto
        /// </summary>
        [EnumMember]
        NAOPRONTO,

        /// <summary>
        /// Negociado
        /// </summary>
        [EnumMember]
        NEGOCIADO,

        /// <summary>
        /// Negociado evento
        /// </summary>
        [EnumMember]
        NEGOCIADOEVENTO,

        /// <summary>
        /// Número lógico divergente
        /// </summary>
        [EnumMember]
        NUMEROLOGICODIVERGENTE,

        /// <summary>
        /// Número série divergente
        /// </summary>
        [EnumMember]
        NUMEROSERIEDIVERGENTE,

        /// <summary>
        /// Ocorrência irregular
        /// </summary>
        [EnumMember]
        OCORRENCIAIRREGULAR,

        /// <summary>
        /// Pendente
        /// </summary>
        [EnumMember]
        PENDENTE,

        /// <summary>
        /// Perdido
        /// </summary>
        [EnumMember]
        PERDIDO,

        /// <summary>
        /// Pre-perda
        /// </summary>
        [EnumMember]
        PREPERDA,

        /// <summary>
        /// Recebimento pendente
        /// </summary>
        [EnumMember]
        RECEBIMENTOPENDENTE,

        /// <summary>
        /// Showroom
        /// </summary>
        [EnumMember]
        SHOWROOM,

        /// <summary>
        /// Trade in
        /// </summary>
        [EnumMember]
        TRADEIN
    }
}
