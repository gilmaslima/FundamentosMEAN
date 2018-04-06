/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2
{
    public delegate void TamanhoPaginaEventHandler(Object sender, Int32 tamanhoPagina);

    public partial class TamanhoPagina : UserControl
    {
        public Int32 Tamanho
        { 
            get { return ddlRegistroPorPagina.SelectedValue.ToInt32(0); }
            set { ddlRegistroPorPagina.SelectedValue = value.ToString(); }
        }
        
        public event TamanhoPaginaEventHandler TamanhoPaginaChanged;
        
        protected void ddlRegistroPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TamanhoPaginaChanged != null)
                TamanhoPaginaChanged(this, Tamanho);
        }
    }
}
