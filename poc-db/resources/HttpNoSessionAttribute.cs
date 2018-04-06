/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Atributo AllowAnonymous
    /// </summary>
    public class HttpNoSessionAttribute : Attribute
    {
        public Boolean AllowNoSession { get; set; }

        public HttpNoSessionAttribute()
        {
            this.AllowNoSession = true;
        }

        public HttpNoSessionAttribute(Boolean allowNoSession)
        {
            this.AllowNoSession = allowNoSession;
        }
    }
}
