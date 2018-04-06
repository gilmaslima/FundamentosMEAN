using System;

namespace Redecard.Portal.Helper.DTO
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: 25/10/2010
    /// Descrição: DTO que representa um item de biblioteca com propriedades comuns
    /// </summary>
    public class ItemBiblioteca
    {
        public string Assunto { get; set; }
        public string Nome { get; set; }
        public string Titulo { get; set; }
        public string Pasta { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public string Url { get; set; }
        public string UrlPreview { get; set; }
        public string UrlAbsoluta { get; set; }
        public string Altura { get; set; }
        public string Largura { get; set; }
        public string TamanhoEmBytes {get;set;}
        public string Audiencia { get; set; }
        public string UrlRedirecionamento { get; set; }
    }
}