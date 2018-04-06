/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.SharePoint.Core.Enums;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Model.Content
{
    [Serializable]
    public class WarningsAtendimentoItem : ContentItem
    {
        /// <summary>
        /// Define um array de segmentos
        /// </summary>
        public String[] Segmentos { get; set; }
        /// <summary>
        /// Define um array de Números de PV.
        /// </summary>
        public String[] NumerosPV { get; set; }
        /// <summary>
        /// Define campo de texto.
        /// </summary>
        public String Texto { get; set; }
        /// <summary>
        /// Define o Tipo:
        /// Sucesso,
        /// Aviso,
        /// Erro,
        /// App
        /// </summary>
        public String Tipo { get; set; }
        /// <summary>
        /// Define a URL de Exibição.
        /// </summary>
        public String URLExibicao { get; set; }
        /// <summary>
        /// Define o Texto do Botão.
        /// </summary>
        public String TextoBotao { get; set; }
        /// <summary>
        /// Define a URL de Destino
        /// </summary>
        public String URLDestino { get; set; }
        /// <summary>
        /// Define a Data de Início da Exibição.
        /// </summary>
        public DateTime DataInicioExibicao { get; set; }
        /// <summary>
        /// Define a Data de término da Exibição.
        /// </summary>
        public DateTime DataFimExibicao { get; set; }
        /// <summary>
        /// Define o tipo do Item.
        /// </summary>
        public override String ItemType
        {
            get { return "warningsAtendimento"; }
        }
    }
}
