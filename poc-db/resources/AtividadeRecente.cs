#region Histórico do Arquivo
/*
(c) Copyright [2017] Redecard S.A.
Autor       : [Rodrigo Rodrigues]
Empresa     : [Iteris]
Histórico   :
- [15/11/2017] – [Rodrigo Rodrigues] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe com dados de atividade recente do PV
    /// </summary>
    public class AtividadeRecente : Base
    {
        public Int32 CodigoAtividade { get; set; }

        public DateTime Data { get; set; }

        public String Email { get; set; }

        public String NomeUsuario { get; set; }

        public String PV { get; set; }

        public String IP { get; set; }

        public String Detalhes { get; set; }
    }
}