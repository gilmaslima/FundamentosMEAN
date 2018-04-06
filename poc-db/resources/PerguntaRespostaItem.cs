/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item de conteúdo: Pergunta e Resposta
    /// </summary>
    public class PerguntaRespostaItem : ContentItem
    {
        /// <summary>
        /// Id da pergunta e resposta
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        public override String ItemType { get { return "perguntaResposta"; } }

        /// <summary>
        /// Resposta
        /// </summary>
        public String Resposta { get; set; }

        /// <summary>
        /// URL do Vídeo
        /// </summary>
        public String URLVideo { get; set; }

        /// <summary>
        /// Título Reduzido
        /// </summary>
        public String TituloReduzido { get; set; }
    }
}