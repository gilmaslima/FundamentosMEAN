/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Enumerador para os tipos de dispositivos
    /// </summary>
    [Flags]
    public enum Device
    {
        /// <summary>
        /// Todos os dispositivos
        /// </summary>
        [Description("")]
        AllDevices = 0,

        /// <summary>
        /// Web
        /// </summary>
        [Description("Exibir web")]
        Web = 1,

        /// <summary>
        /// Android
        /// </summary>
        [Description("Exibir Android")]
        Android = 2,

        /// <summary>
        /// iOS
        /// </summary>
        [Description("Exibir iOS")]
        iOS = 4
    }
}
