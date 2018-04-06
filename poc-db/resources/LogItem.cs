using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Modelo
{
    public class LogItem
    {
        public DateTime Timestamp { get;set;}

        public Int32? SeverityID { get; set; }

        public String Severidade { get; set; }

        public String Mensagem { get; set; }

        public Guid ActivityID { get; set; }

        public Int32? CodigoEntidade { get; set; }

        public Int32? GrupoEntidade { get; set; }

        public String LoginUsuario { get; set; }

        public Int32? AssemblyID { get; set; }

        public String Assembly { get; set; }

        public String Classe { get; set; }

        public String Metodo { get; set; }

        public Int32? LinhaCodigo { get; set; }

        public Int32? ColunaCodigo { get; set; }

        public String Parametros { get; set; }

        public String Excecao { get; set; }

        public String Servidor { get; set; }

        public Int32 LogID { get; set; }
    }
}
