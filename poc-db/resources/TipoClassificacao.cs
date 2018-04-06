/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoClassificação
    /// </summary>
    [DataContract]
    public enum TipoClassificacao
    {
        /// <summary>
        /// Caçapos
        /// </summary>
        [EnumMember]
        CAÇAPOS,

        /// <summary>
        /// Eventos desinstalação de grande evento
        /// </summary>
        [EnumMember]
        EVENTOSDESINSTALAÇÃODEGRANDEEVENTO,

        /// <summary>
        /// Eventos desintalação de pequeno evento
        /// </summary>
        [EnumMember]
        EVENTOSDESINSTALAÇÃODEPEQUENOEVENTO,

        /// <summary>
        /// Eventos instalação em grande evento
        /// </summary>
        [EnumMember]
        EVENTOSINSTALAÇÃOEMGRANDEEVENTO,

        /// <summary>
        /// Eventos instalação pequeno evento
        /// </summary>
        [EnumMember]
        EVENTOSINSTALAÇÃOEMPEQUENOEVENTO,

        /// <summary>
        /// Eventos troca de evento
        /// </summary>
        [EnumMember]
        EVENTOSTROCADEEVENTO,

        /// <summary>
        /// Logística avanço
        /// </summary>
        [EnumMember]
        LOGÍSTICAAVANÇO,

        /// <summary>
        /// Logística avanço para laboratório
        /// </summary>
        [EnumMember]
        LOGÍSTICAAVANÇOPARALABORATÓRIO,

        /// <summary>
        /// Logística inventário
        /// </summary>
        [EnumMember]
        LOGÍSTICAINVENTÁRIO,

        /// <summary>
        /// Logística reversa
        /// </summary>
        [EnumMember]
        LOGÍSTICALOGÍSTICAREVERSA,

        /// <summary>
        /// Logística reversa de laboratório
        /// </summary>
        [EnumMember]
        LOGÍSTICALOGÍSTICAREVERSADELABORATÓRIO,

        /// <summary>
        /// Logística recebimento
        /// </summary>
        [EnumMember]
        LOGÍSTICARECEBIMENTO,

        /// <summary>
        /// Logística reintegração
        /// </summary>
        [EnumMember]
        LOGÍSTICAREINTEGRAÇÃO,
        
        /// <summary>
        /// Logística transferência CD
        /// </summary>
        [EnumMember]
        LOGÍSTICATRANSFERENCIACD,

        /// <summary>
        /// Logística transferência EPS
        /// </summary>
        [EnumMember]
        LOGÍSTICATRANSFERENCIAEPS,

        /// <summary>
        /// Ocorrência irregular Caçapos
        /// </summary>
        [EnumMember]
        OCORRÊNCIAIRREGULARCAÇAPOS,

        /// <summary>
        /// Ocorrência irregular furto qualificado
        /// </summary>
        [EnumMember]
        OCORRÊNCIAIRREGULARFURTOQUALIFICADO,

        /// <summary>
        /// Ocorrência irregular furto simples
        /// </summary>
        [EnumMember]
        OCORRÊNCIAIRREGULARFURTOSIMPLES,

        /// <summary>
        /// Ocorrência irregular perda de inventário
        /// </summary>
        [EnumMember]
        OCORRÊNCIAIRREGULARPERDADEINVENTÁRIO,

        /// <summary>
        /// Ocorrência irregular projeto de reintegração
        /// </summary>
        [EnumMember]
        OCORRÊNCIAIRREGULARPROJETODEREINTEGRAÇÃO,

        /// <summary>
        /// Outros serviços alteração endereço
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSALTERAÇÃODEENDEREÇO,

        /// <summary>
        /// Outros serviços alteração de filiação
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSALTERAÇÃODEFILIAÇÃO,

        /// <summary>
        /// Outros serviços desativação
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSDESATIVAÇÃO,

        /// <summary>
        /// Outros serviços instalação de komerci
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSINSTALAÇÃODEKOMERCI,

        /// <summary>
        /// Outros serviços instalação de mobile
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSINSTALAÇÃODEMOBILE,

        /// <summary>
        /// Outros serviços instalação de PDV dedicado
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSINSTALAÇÃODEPDVDEDICADO,

        /// <summary>
        /// Outros serviços instalação de PDV discado
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSINSTALAÇÃODEPDVDISCADO,

        /// <summary>
        /// Outros serviços reativação
        /// </summary>
        [EnumMember]
        OUTROSSERVIÇOSREATIVAÇÃO,

        /// <summary>
        /// Projetos carga de terminal
        /// </summary>
        [EnumMember]
        PROJETOSCARGADETERMINAL,

        /// <summary>
        /// Projetos coleta de equipamento
        /// </summary>
        [EnumMember]
        PROJETOSCOLETADEEQUIPAMENTO,

        /// <summary>
        /// Projetos desinstalação de equipamento
        /// </summary>
        [EnumMember]
        PROJETOSDESINSTALAÇÃODEEQUIPAMENTO,

        /// <summary>
        /// Projetos inicialização de terminal
        /// </summary>
        [EnumMember]
        PROJETOSINICIALIZAÇÃODETERMINAL,

        /// <summary>
        /// Projetos instalação de equipamento
        /// </summary>
        [EnumMember]
        PROJETOSINSTALAÇÃODEEQUIPAMENTO,

        /// <summary>
        /// Projetos de carga
        /// </summary>
        [EnumMember]
        PROJETOSPROJETODECARGA,

        /// <summary>
        /// Projeto de inicialização
        /// </summary>
        [EnumMember]
        PROJETOSPROJETODEINICIALIZAÇÃO,
        /// <summary>
        /// Projetos troca de equipamento
        /// </summary>
        [EnumMember]
        PROJETOSTROCADEEQUIPAMENTO,

        /// <summary>
        /// Projetos troca em lote alteração de tecnologia
        /// </summary>
        [EnumMember]
        PROJETOSTROCAEMLOTEALTERAÇÃODETECNOLOGIA,

        /// <summary>
        /// Serviços de campo coleta de equipamento
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOCOLETADEEQUIPAMENTO,

        /// <summary>
        /// Serviços de campo desativação de PDV
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPODESATIVAÇÃODEPDV,

        /// <summary>
        /// Serviços de campo desinstalação
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPODESINSTALAÇÃO,
        
        /// <summary>
        /// Serviços de campo desinstalação de inativos
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPODESINSTALAÇÃODEINATIVOS,

        /// <summary>
        /// Serviços de campo desinstalação de pos poo
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPODESINSTALAÇÃODEPOSPOO,

        /// <summary>
        /// Serviços de campo desinstalação em campo
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPODESINSTALAÇÃOSEMCAMPO,

        /// <summary>
        /// Serviços de campo instalação de pinpad discado
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOINSTALAÇÃODEPINPADDISCADO,

        /// <summary>
        /// Serviços de campo instalação de pinpad
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOINSTALAÇÃODEPINPAD,

        /// <summary>
        /// Serviços de campo instalação de pospoo
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOINSTALAÇÃODEPOSPOO,

        /// <summary>
        /// Serviços de campo ocorrência irregular
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOOCORRÊNCIAIRREGULAR,

        /// <summary>
        /// Serviços de campo retirada de pinpad discado
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPORETIRADADEPINPADDISCADO,

        /// <summary>
        /// Serviços de campo retirada de pinpad
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPORETIRADADEPINPAD,

        /// <summary>
        /// Serviços de campo troca de equipamento
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOTROCADEEQUIPAMENTO,

        /// <summary>
        /// Serviços de campo troca de partes
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOTROCADEPARTES,

        /// <summary>
        /// Serviços de campo troca de pinpad discado
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOTROCADEPINPADDISCADO,

        /// <summary>
        /// Serviços de campo troca de pinpad
        /// </summary>
        [EnumMember]
        SERVIÇOSDECAMPOTROCADEPINPAD
    }
}
