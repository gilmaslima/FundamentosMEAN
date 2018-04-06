using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.Login.Modelo
{
    /// <summary>
    /// Lista Modelo de PVs Piloto
    /// </summary>
    public class PvPiloto
    {
        /// <summary>
        /// Número do PV Piloto
        /// </summary>
        public Int64 NumeroPV { get; set; }

        /// <summary>
        /// Indicador se está com o Piloto habilitado
        /// </summary>
        public Boolean Habilitar { get; set; }
    }
}
