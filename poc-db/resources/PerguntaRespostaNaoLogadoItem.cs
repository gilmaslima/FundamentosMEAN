/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content.Public
{
    /// <summary>
    /// Item de Pergunta e Resposta
    /// </summary>
    public class PerguntaRespostaNaoLogadoItem : ContentItem
    {
        /// <summary>
        /// Id da pergunta
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Descrição da pergunta
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Subcategoria pai da pergunta
        /// </summary>
        public Int32 Subcategoria { get; set; }

        /// <summary>
        /// Palavra chave principal
        /// </summary>
        public String PalavraChave { get; set; }

        /// <summary>
        /// Palavras chaves
        /// </summary>
        public List<String> PalavrasChaves { get; set; }

        /// <summary>
        /// Url amigável
        /// </summary>
        public String UrlAmigavel { get; set; }

        /// <summary>
        /// Page title
        /// </summary>
        public String PageTitle { get; set; }

        /// <summary>
        /// Resposta resumida
        /// </summary>
        public String RespostaResumida { get; set; }

        /// <summary>
        /// Vídeos
        /// </summary>
        public List<String> Videos { get; set; }

        /// <summary>
        /// ItemType
        /// </summary>
        public override string ItemType
        {
            get { return "perguntaResposta"; }
        }
    }
}
