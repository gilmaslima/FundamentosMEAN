/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoOSSituação
    /// </summary>
    [DataContract]
    public enum TipoOSSituacao
    {
        /// <summary>
        /// Agendada
        /// </summary>
        [EnumMember]
        AGENDADA,

        /// <summary>
        /// Aguardando aprovação
        /// </summary>
        [EnumMember]
        AGUARDANDOAPROVACAO,

        /// <summary>
        /// Aguardando carga assistida
        /// </summary>
        [EnumMember]
        AGUARDANDOCARGAASSISTIDA,

        /// <summary>
        /// Aguardadndo documentação
        /// </summary>
        [EnumMember]
        AGUARDANDODOCUMENTACAO,

        /// <summary>
        /// Aguardando inicialização assistida
        /// </summary>
        [EnumMember]
        AGUARDANDOINICIALIZACAOASSISTIDA,

        /// <summary>
        /// Aguardando liberação
        /// </summary>
        [EnumMember]
        AGUARDANDOLIBERACAO,

        /// <summary>
        /// Aguardando negociação
        /// </summary>
        [EnumMember]
        AGUARDANDONEGOCIACAO,

        /// <summary>
        /// Aguardando negociação manual
        /// </summary>
        [EnumMember]
        AGUARDANDONEGOCIACAOMANUAL,

        /// <summary>
        /// Aguardando recebimento
        /// </summary>
        [EnumMember]
        AGUARDANDORECEBIMENTO,

        /// <summary>
        /// Aprovada
        /// </summary>
        [EnumMember]
        APROVADA,

        /// <summary>
        /// Ativos faltando
        /// </summary>
        [EnumMember]
        ATIVOSFALTANDO,

        /// <summary>
        /// Ativos recebidos
        /// </summary>
        [EnumMember]
        ATIVOSRECEBIDOS,

        /// <summary>
        /// Ativos sobrando
        /// </summary>
        [EnumMember]
        ATIVOSSOBRANDO,

        /// <summary>
        /// Cancelada
        /// </summary>
        [EnumMember]
        CANCELADA,

        /// <summary>
        /// Carga assistida
        /// </summary>
        [EnumMember]
        CARGAASSISTIDA,

        /// <summary>
        /// Carga automática
        /// </summary>
        [EnumMember]
        CARGAAUTOMATICA,

        /// <summary>
        /// Coleta liberada
        /// </summary>
        [EnumMember]
        COLETALIBERADA,

        /// <summary>
        /// Coleta manual
        /// </summary>
        [EnumMember]
        COLETAMANUAL,

        /// <summary>
        /// Coleta manual falhou
        /// </summary>
        [EnumMember]
        COLETAMANUALFALHOU,

        /// <summary>
        /// Criada
        /// </summary>
        [EnumMember]
        CRIADA,

        /// <summary>
        /// Desinstalação manual
        /// </summary>
        [EnumMember]
        DESINSTALACAOMANUAL,

        /// <summary>
        /// Desinstalação manual falhou
        /// </summary>
        [EnumMember]
        DESINSTALACAOMANUALFALHOU,

        /// <summary>
        /// Devolvida
        /// </summary>
        [EnumMember]
        DEVOLVIDA,

        /// <summary>
        /// Em campo
        /// </summary>
        [EnumMember]
        EMCAMPO,

        /// <summary>
        /// Em recebimento
        /// </summary>
        [EnumMember]
        EMRECEBIMENTO,

        /// <summary>
        /// Em trânsito
        /// </summary>
        [EnumMember]
        EMTRANSITO,

        /// <summary>
        /// Encaminhada
        /// </summary>
        [EnumMember]
        ENCAMINHADA,

        /// <summary>
        /// Encerrada
        /// </summary>
        [EnumMember]
        ENCERRADA,

        /// <summary>
        /// Enviada
        /// </summary>
        [EnumMember]
        ENVIADA,

        /// <summary>
        /// Inicialização assistida
        /// </summary>
        [EnumMember]
        INICIALIZACAOASSISTIDA,

        /// <summary>
        /// Inicialização falhou
        /// </summary>
        [EnumMember]
        INICIALIZACAOFALHOU,

        /// <summary>
        /// Negociação falhou
        /// </summary>
        [EnumMember]
        NEGOCIACAOFALHOU,

        /// <summary>
        /// Negociada
        /// </summary>
        [EnumMember]
        NEGOCIADA,

        /// <summary>
        /// Pendente
        /// </summary>
        [EnumMember]
        PENDENTE,

        /// <summary>
        /// Produção manual
        /// </summary>
        [EnumMember]
        PRODUCAOMANUAL,

        /// <summary>
        /// Produção manual falhou
        /// </summary>
        [EnumMember]
        PRODUCAOMANUALFALHOU,

        /// <summary>
        /// Realizando carga
        /// </summary>
        [EnumMember]
        REALIZANDOCARGA,

        /// <summary>
        /// Recebida
        /// </summary>
        [EnumMember]
        RECEBIDA,

        /// <summary>
        /// Sem discrepâncias
        /// </summary>
        [EnumMember]
        SEMDISCREPANCIAS,

        /// <summary>
        /// Terminal não encontrado
        /// </summary>
        [EnumMember]
        TERMINALNAOENCONTRADO
    }
}
