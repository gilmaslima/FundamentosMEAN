/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico
{
    /// <summary>
    /// Horário da OS
    /// </summary>
    public class HorarioOs
    {
        /// <summary>
        /// Dia (index equivalente à Enum para dias da semanaa )
        /// </summary>
        public Int32 Dia { get; set; }

        /// <summary>
        /// Horario início
        /// </summary>
        public Int32 HoraDas { get; set; }

        /// <summary>
        /// Horário fim
        /// </summary>
        public Int32 HoraAs { get; set; }
    }
}
