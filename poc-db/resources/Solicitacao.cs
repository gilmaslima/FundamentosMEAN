#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [27/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe modelo de solicitações
    /// </summary>
    public class Solicitacao
    {
        /// <summary>
        /// Número da solicitação
        /// </summary>
        public Int32 NumeroSolicitacao {get; set;}

        /// <summary>
        /// Código Caso
        /// </summary>
        public Int32 CodigoCaso {get; set;}

        /// <summary>
        /// Código da Entidade
        /// </summary>
        public Int32 CodigoEntidade {get; set;}

        /// <summary>
        /// Data de abertura da solicitação
        /// </summary>
        public DateTime DataSolicitacao {get; set;}

        /// <summary>
        /// Data de Encerramento da Solicitação
        /// </summary>
        public DateTime DataEncerramento {get; set;}

        /// <summary>
        /// Indicador da Situação da Solicitação
        /// </summary>
        public String SituacaoSolicitacao {get; set;}

        /// <summary>
        /// Tipo de ocorrência da solicitação
        /// </summary>
        public String TipoOcorrencia {get; set;}

        /// <summary>
        /// Nome da ocorrência de solicitação
        /// </summary>
        public String NomeOcorrencia {get; set;}

        /// <summary>
        /// Razão social do Estabelecimento
        /// </summary>
        public String RazaoSocialEstabelecimento {get; set;}

        /// <summary>
        /// Usuário responsável pela monitoria da solicitação
        /// </summary>
        public String UsuarioMonitoria {get; set;}

        /// <summary>
        /// Indicador de tipo do envio de carta. 
        /// </summary>
        public Int16 EnvioCarta { get; set; }

        /// <summary>
        /// Quantidade de Rechamadas
        /// </summary>
        public String QuantidadeRechamada { get; set; }

        /// <summary>
        /// Canal Resposta da Solicitação
        /// </summary>
        public String CanalResposta { get; set; }
    }
}


