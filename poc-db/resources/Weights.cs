/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Configuração de pesos das ocorrências
    /// </summary>
    public class Weights
    {
        /// <summary>
        /// Pesos das ocorrências no Título do conteúdo
        /// </summary>
        public Hit Title { get; set; }

        /// <summary>
        /// Pesos das ocorrências na Descrição do conteúdo
        /// </summary>
        public Hit Description { get; set; }
    }
}