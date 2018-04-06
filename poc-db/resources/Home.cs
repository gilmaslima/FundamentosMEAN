/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.WebParts.Home
{
    [ToolboxItemAttribute(false)]
    public class Home : Microsoft.SharePoint.WebPartPages.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Extrato.SharePoint.WebParts/Home/HomeUserControl.ascx";

        #region [ Propriedades da WebPart - Varejo ]

        /// <summary>
        /// Propriedade da WebPart para Configuração dos Atalhos do Box de Vendas
        /// da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoVendasAtalhos { get; set; }

        /// <summary>
        /// Propriedade da WebPart para Configuração dos Atalhos do Box de Recebimentos
        /// da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoRecebimentosAtalhos { get; set; }

        /// <summary>
        /// Propriedade da WebPart para Configuração dos Atalhos da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoAtalhos { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título RAV da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoTituloRav { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título Aviso da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoTituloAviso { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título do Banner Fidelidade para PV Fidelizado - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoTituloFidelizado { get; set; }

        /// <summary>
        /// Propriedade da WebPArt para armazenamento do Título do Banner Fidelidade para PV Elegível - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoTituloElegivel { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo RAV da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoConteudoRav { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo Aviso da HomePage Segmentada - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoConteudoAviso { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo do Banner Fidelidade para PV Fidelizado - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoConteudoFidelizado { get; set; }

        /// <summary>
        /// Propriedade da WebPArt para armazenamento do Conteúdo do Banner Fidelidade para PV Elegível - Varejo.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String VarejoConteudoElegivel { get; set; }


        #endregion

        #region [ Propriedades da WebPart - EMP/IBBA ]

        /// <summary>
        /// Propriedade da WebPart para Configuração dos Atalhos da HomePage Segmentada - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaAtalhos { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título RAV da HomePage Segmentada - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaTituloRav { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título Aviso da HomePage Segmentada - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaTituloAviso { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título do Banner Fidelidade para PV Fidelizado - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaTituloFidelizado { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Título do Banner Fidelidade para PV Elegível - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaTituloElegivel { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo RAV da HomePage Segmentada - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaConteudoRav { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo Aviso da HomePage Segmentada - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaConteudoAviso { get; set; }

        /// <summary>
        /// Propriedade da WebPart para armazenamento do Conteúdo do Banner Fidelidade para PV Fidelizado - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaConteudoFidelizado { get; set; }

        /// <summary>
        /// Propriedade da WebPArt para armazenamento do Conteúdo do Banner Fidelidade para PV Elegível - EMP/IBBA.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        public String EmpIbbaConteudoElegivel { get; set; }

        #endregion

        #region [ Métodos Sobrescritos ]

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        /// <summary>
        /// Customização da aba de configuração da WebPart,
        /// para inclusão da configuração dos atalhos dos boxes.
        /// </summary>
        public override ToolPart[] GetToolParts()
        {
            ToolPart[] allToolParts = new ToolPart[6];
            allToolParts[0] = new WebPartToolPart();

            allToolParts[1] = new CustomPropertyToolPart();

            allToolParts[2] = new AtalhosHomeToolPart("Varejo - Suas Vendas - Atalhos", 2,
                (config) => { this.VarejoVendasAtalhos = config; },
                () => { return this.VarejoVendasAtalhos; });

            allToolParts[3] = new AtalhosHomeToolPart("Varejo - Recebimentos - Atalhos", 2,
                (config) => { this.VarejoRecebimentosAtalhos = config; },
                () => { return this.VarejoRecebimentosAtalhos; });

            allToolParts[4] = new AtalhosHomeToolPart("Varejo - Atalhos", 6,
                (config) => { this.VarejoAtalhos = config; },
                () => { return this.VarejoAtalhos; });

            allToolParts[5] = new AtalhosHomeToolPart("EMP/IBBA - Atalhos", 5,
                (config) => { this.EmpIbbaAtalhos = config; },
                () => { return this.EmpIbbaAtalhos; });

            return allToolParts;
        }

        #endregion
    }
}