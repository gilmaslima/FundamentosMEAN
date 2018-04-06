using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Redecard.Portal.Helper.DTO;

namespace Redecard.Portal.Helper.Conversores
{
    /// <summary>
    /// Autor: Cristiano Martins Dias
    /// Data criação: 25/10/2010
    /// Descrição: Classe conversa de uma coleção de objeto SPFile para uma coleção de objetos ItemBiblioteca(DTO)
    /// </summary>
    public sealed class TradutorDeSPFileParaItemBiblioteca : ITraducao<IEnumerable<SPFile>,IEnumerable<ItemBiblioteca>>
    {
        public IEnumerable<SPFile> Traduzir(IEnumerable<ItemBiblioteca> itens)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Realiza a conversão de lista de SPFile para lista de ItemBiblioteca
        /// </summary>
        /// <param name="itens"></param>
        /// <returns></returns>
        public IEnumerable<ItemBiblioteca> Traduzir(IEnumerable<SPFile> itens)
        {
            IList<ItemBiblioteca> itensBiblioteca = new List<ItemBiblioteca>();

            foreach (SPFile spFile in itens)
                itensBiblioteca.Add(this.Traduzir(spFile));

            return itensBiblioteca;
        }

        /// <summary>
        /// Realiza a conversão em nível de item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ItemBiblioteca Traduzir(SPFile item)
        {
            ItemBiblioteca itemBiblioteca = new ItemBiblioteca();

            //Campo nome
            if(!string.IsNullOrEmpty(item.Name))
                itemBiblioteca.Nome = item.Name;

            //Campo título
            if(item.GetProperty("vti_title") != null)
                itemBiblioteca.Titulo = item.GetProperty("vti_title").ToString();

            //Campo descrição
            if (item.GetProperty("vti_description") != null)
                itemBiblioteca.Descricao = item.GetProperty("vti_description").ToString();

            //URL do item (relativa)
            if (!string.IsNullOrEmpty(item.Url))
            {
                itemBiblioteca.Url = item.Url;

                itemBiblioteca.UrlAbsoluta = SPContext.Current.Site.MakeFullUrl(item.Url);
            }

            //Campo customizado UrlImagem (imagem de preview)
            if (item.GetProperty("UrlImagem") != null)
            {
                string sUrl = string.Empty;
                // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
                // [URL], [Descrição]
                if (!string.IsNullOrEmpty(item.GetProperty("UrlImagem").ToString()))
                    itemBiblioteca.UrlPreview = new SPFieldUrlValue(item.GetProperty("UrlImagem").ToString()).Url;
            }

            //Pasta que contém o item
            if (!string.IsNullOrEmpty(item.ParentFolder.Name))
                itemBiblioteca.Pasta = item.ParentFolder.Name;

            //Assunto
            if (item.GetProperty("Subject") != null)
                itemBiblioteca.Assunto = item.GetProperty("Subject").ToString();

            //Data item
            if (item.GetProperty("vti_imgdate") != null)
                itemBiblioteca.Data = DateTime.Parse(item.GetProperty("vti_imgdate").ToString());

            //Tamanho
            itemBiblioteca.TamanhoEmBytes = item.Length.ToString();

            //Altura
            if (item.GetProperty("vti_lastheight") != null)
                itemBiblioteca.Altura = item.GetProperty("vti_lastheight").ToString();

            //Largura
            if (item.GetProperty("vti_lastwidth") != null)
                itemBiblioteca.Largura = item.GetProperty("vti_lastwidth").ToString();

            //Audiencia            
            if (item.GetProperty("Públicos-alvos") != null)
                itemBiblioteca.Audiencia = item.GetProperty("Públicos-alvos").ToString();

            //URL de Redirecionamento
            if (item.GetProperty("RedirectURL") != null)
            {
                string sUrl = string.Empty;
                // verificar campo de URL, no formato padrão, o sharepoint retorna o valor no seguinte formato:
                // [URL], [Descrição]
                if (!string.IsNullOrEmpty(item.GetProperty("RedirectURL").ToString()))
                    itemBiblioteca.UrlRedirecionamento = new SPFieldUrlValue(item.GetProperty("RedirectURL").ToString()).Url;                
            }
            
            return itemBiblioteca;
        }
    }
}