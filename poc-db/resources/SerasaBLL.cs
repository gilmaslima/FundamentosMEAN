using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Agentes;

namespace Redecard.PN.Credenciamento.Negocio
{
    public class SerasaBLL : RegraDeNegocioBase
    {
        public Modelo.PJ ConsultaSerasaPJ(String cnpj)
        {
            SerasaAG serasaAG = new SerasaAG();
            return serasaAG.ConsultaSerasaPJ(cnpj);
        }

        public Modelo.PF ConsultaSerasaPF(String cpf)
        {
            SerasaAG serasaAG = new SerasaAG();
            return serasaAG.ConsultaSerasaPF(cpf);
        }
    }
}
