using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo.Configuracao
{
    public class GrupoAVS
    {
        public Int32 IDGrupo { get; set; }
        public String Grupo { get; set; }
        public Boolean NotMatch { get; set; }
    }
}
