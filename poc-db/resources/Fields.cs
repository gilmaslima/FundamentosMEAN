/*
© Copyright 2014 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Constants
{
    /// <summary>
    /// Campos/Propriedades das listas
    /// </summary>
    public static class Fields
    {
        #region Comuns 

        /// <summary>
        /// Field: ID
        /// </summary>
        public static String Id { get { return "ID"; }}

        /// <summary>
        /// Field: Title
        /// </summary>
        public static String Title { get { return "Title"; }}

        /// <summary>
        /// Field: Ordem
        /// </summary>
        public static String Ordem { get { return "Ordem"; }}

        /// <summary>
        /// Field: CanaisExibicao
        /// </summary>
        public static String CanaisExibicao { get { return "CanaisExibicao"; } }

        #endregion

        #region Dúvida Categoria

        /// <summary>
        /// Field: Icone
        /// </summary>
        public static String Icone { get { return "Icone"; }}        

        #endregion

        #region Dúvida Subcategoria

        /// <summary>
        /// Field: Categoria
        /// </summary>
        public static String Categoria { get { return "Categoria"; }}

        /// <summary>
        /// Field: CategoriaSubcategoria
        /// </summary>
        public static String CategoriaSubcategoria { get { return "CategoriaSubcategoria"; }}

        /// <summary>
        /// Field: CategoriaID
        /// </summary>
        public static String CategoriaID { get { return "CategoriaID"; }}

        /// <summary>
        /// Field: ExibeMenu
        /// </summary>
        public static String ExibeMenu { get { return "ExibeMenu"; }}

        /// <summary>
        /// Field: Resumo
        /// </summary>
        public static String Resumo { get { return "Resumo"; }}

        #endregion

        #region Dúvida Pergunta Resposta

        /// <summary>
        /// Field: DescricaoSubcategoria
        /// </summary>
        public static String DescricaoSubcategoria { get { return "DescricaoSubcategoria"; }}

        /// <summary>
        /// Field: TituloReduzido
        /// </summary>
        public static String TituloReduzido { get { return "TituloReduzido"; }}

        /// <summary>
        /// Field: Resposta
        /// </summary>
        public static String Resposta { get { return "Resposta"; }}

        /// <summary>
        /// Field: SubcategoriaTitle
        /// </summary>
        public static String SubcategoriaTitle { get { return "SubcategoriaTitle"; }}

        /// <summary>
        /// Field: SubCategoriaID
        /// </summary>
        public static String SubCategoriaID { get { return "SubCategoriaID"; }}

        /// <summary>
        /// Field: SubCategoriaExibeMenu
        /// </summary>
        public static String SubCategoriaExibeMenu { get { return "SubCategoriaExibeMenu"; }}

        /// <summary>
        /// Field: URLVideo
        /// </summary>
        public static String URLVideo { get { return "URLVideo"; }}

        #endregion

        #region Dúvida Frequente

        /// <summary>
        /// Field: IDPerguntaResposta
        /// </summary>
        public static String IDPerguntaResposta { get { return "IDPerguntaResposta"; }}

        #endregion

        #region Página Pergunta

        /// <summary>
        /// Field: TituloPrincipal
        /// </summary>
        public static String TituloPrincipal { get { return "TituloPrincipal"; }}

        /// <summary>
        /// Field: TextoPrincipal
        /// </summary>
        public static String TextoPrincipal { get { return "TextoPrincipal"; }}

        /// <summary>
        /// Field: Pergunta
        /// </summary>
        public static String Pergunta { get { return "Pergunta"; }}

        /// <summary>
        /// Field: PerguntaId
        /// </summary>
        public static String PerguntaId { get { return "PerguntaId"; }}

        /// <summary>
        /// Field: TituloSecundario
        /// </summary>
        public static String TituloSecundario { get { return "TituloSecundario"; }}

        /// <summary>
        /// Field: TextoSecundario
        /// </summary>
        public static String TextoSecundario { get { return "TextoSecundario"; }}

        /// <summary>
        /// Field: UrlPagina
        /// </summary>
        public static String URLPagina { get { return "UrlPagina"; }}

        /// <summary>
        /// Field: perguntaURLVideo
        /// </summary>
        public static String PerguntaURLVideo { get { return "PerguntaURLVideo"; }}

        #endregion

        #region Parâmetros Configuração

        /// <summary>
        /// Field: Descricao
        /// </summary>
        public static String Descricao { get { return "Descricao"; }}

        /// <summary>
        /// Field: Ativo
        /// </summary>
        public static String Ativo { get { return "Ativo"; }}

        /// <summary>
        /// Field: name
        /// </summary>
        public static String Name { get { return "name"; }}

        /// <summary>
        /// Field: Valor
        /// </summary>
        public static String Valor { get { return "Valor"; }}


       #endregion

        #region Atendimento Digital

        /// <summary>
        /// File
        /// </summary>
        public static String File { get { return "File"; }}
        
        /// <summary>
        /// FileName
        /// </summary>
        public static String FileName { get { return "FileName"; }}

        /// <summary>
        /// ByteFile
        /// </summary>
        public static String ByteFile { get { return "ByteFile"; }}        

        #endregion
        
        #region Warnings Atendimento
        /// <summary>
        /// Field: Titulo
        /// </summary>
        public static String Titulo { get { return "Titulo"; } }
        /// <summary>
        /// Field: Segmento
        /// </summary>
        public static String Segmento { get { return "Segmento"; } }
        /// <summary>
        /// Field: NumeroPV
        /// </summary>
        public static String NumeroPV { get { return "NumeroPV"; } }
        /// <summary>
        /// Field: Texto
        /// </summary>
        public static String Texto { get { return "Texto"; } }
        /// <summary>
        /// Field: Tipo
        /// </summary>
        public static String Tipo { get { return "Tipo"; } }
        /// <summary>
        /// Field: URLExibicao
        /// </summary>
        public static String URLExibicao { get { return "UrlExibicao"; } }
        /// <summary>
        /// Field: TextoBotao
        /// </summary>
        public static String TextoBotao { get { return "TextoBotao"; } }
        /// <summary>
        /// Field: Titulo
        /// </summary>
        public static String URLDestino { get { return "UrlDestino"; } }
        /// <summary>
        /// Field: DataInicioExibicao
        /// </summary>
        public static String DataInicioExibicao { get { return "DataInicioExibicao"; } }
        /// <summary>
        /// Field: DataFimExibicao
        /// </summary>
        public static String DataFimExibicao { get { return "DataFimExibicao"; } }
        #endregion Warnings Atendimento
    }
}
