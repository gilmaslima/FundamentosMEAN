using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarExtratoDirfRetornoDTO
    {
        public string RazaoSocialEstabelecimento { get; set; }
        public string ComarcaEstabelecimento { get; set; }
        public string EnderecoEstabelecimento { get; set; }
        public string BairroEstabelecimento { get; set; }
        public string CidadeEstabelecimento { get; set; }
        public string EstadoEstabelecimento { get; set; }
        public int CepEstabelecimento { get; set; }
        public string CodigoMalaDiretaEstabelecimento { get; set; }
        public List<ConsultarExtratoDirfEstabelecimentoRetornoDTO> Estabelecimentos { get; set; }
        public List<ConsultarExtratoDirfEmissorRetornoDTO> Emissores { get; set; }
    }
}
