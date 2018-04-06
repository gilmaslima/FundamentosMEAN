using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Agentes;
using Redecard.PN.Boston.Modelo;

namespace Redecard.PN.Boston.Negocio
{
    public class TokenBLL : RegraDeNegocioBase
    {
        public String GetTokenAnaliseRisco(String cpfCnpj, String nome, String sobrenome, DateTime dataFundacao, String email, String telefone1, String telefone2, String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos, Endereco enderecoPrincipal, Endereco enderecoEntrega, Endereco enderecoCobranca)
        {
            var tokenAG = new TokenAG();
            return tokenAG.GetTokenAnaliseRisco(cpfCnpj, nome, sobrenome, dataFundacao, email, telefone1, telefone2, numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos, enderecoPrincipal, enderecoEntrega, enderecoCobranca);
        }

        public String GetToken(String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos)
        {
            var tokenAG = new TokenAG();
            return tokenAG.GetToken(numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos);
        }
    }
}
