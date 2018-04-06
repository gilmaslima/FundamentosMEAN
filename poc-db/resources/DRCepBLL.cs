using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Agentes;

namespace Redecard.PN.Credenciamento.Negocio
{
    public class DRCepBLL : RegraDeNegocioBase
    {
        public Int32 BuscaLogradouro(String cep, ref String endereco, ref String bairro, ref String cidade, ref String uf)
        {
            DRCepAG cepAG = new DRCepAG();
            return cepAG.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
        }
    }
}
