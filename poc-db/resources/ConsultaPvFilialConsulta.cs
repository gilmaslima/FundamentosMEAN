using System;

namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
	/// <summary>
	/// Classe auxiliar para deserialização
	/// </summary>
	public class ConsultaPvFilialConsulta
    {
        public String Categoria { get; set; }
        public String Centralizador { get; set; }
        public Int32 Matriz { get; set; }
        public String Moeda { get; set; }
        public String NomeComerc { get; set; }
        public Int32 PontoVenda { get; set; }
        public Int32 TipoEstab { get; set; }
        public String Chave { get; set; }
    }
}
