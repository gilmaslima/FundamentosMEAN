/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios
{
    /// <summary>
    /// Classe base para as WebParts das telas de Usuários
    /// </summary>
    public class UsuariosWebPartBase : WebPart
    {
        /// <summary>
        /// Modo de funcionamento da WebPart (Edicao ou Criacao).
        /// É reutilizada nas telas de Criação ou Edição de usuário
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Modo"),
        WebDescription("Modo de funcionamento da WebPart ('Edicao' ou 'Criacao' ou 'Aprovacao')")]
        public String Modo { get; set; }
    }
}
