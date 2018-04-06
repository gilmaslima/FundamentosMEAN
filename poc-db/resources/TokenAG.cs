using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Agentes.FESetup;

namespace Redecard.PN.Boston.Agentes
{
    public class TokenAG : AgentesBase
    {
        public String GetTokenAnaliseRisco(String cpfCnpj, String nome, String sobrenome, DateTime dataFundacao, String email, String telefone1, String telefone2, String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos, Redecard.PN.Boston.Modelo.Endereco enderecoPrincipal, Redecard.PN.Boston.Modelo.Endereco enderecoEntrega, Redecard.PN.Boston.Modelo.Endereco enderecoCobranca)
        {
            using (var log = Logger.IniciarLog("Get Token Análise de Risco - Agente"))
            {
                String token = String.Empty;
                string name = System.Net.Dns.GetHostName();
                var host = System.Net.Dns.GetHostEntry(name);
                System.Net.IPAddress ip = host.AddressList.Where(n => n.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();

                using (var contexto = new ContextoWCF<SetupPluginFEClient>())
                {
                    var servicos = codServicos.ToArray();

                    Setup setup = new Setup
                    {
                        EmailCliente = email,
                        IpCliente = ip.ToString(), //IP do servidor de serviço
                        NomeCliente = nome, // Primeiro Nome
                        SobrenomeCliente = sobrenome, // Último Nome
                        Tel1Cliente = telefone1,
                        Tel2Cliente = telefone2, // Não obrigatório
                        CidadeEstab = enderecoPrincipal.Cidade,
                        DatNascCliente = dataFundacao,
                        CodDocCliente = 2,
                        NumDocCliente = cpfCnpj,
                        NumPdv = numPdv,
                        ValorTransacao = valorTransacao,
                        NumPedido = numPedido,
                        QtdParcela = qtdParcela,
                        UrlRetorno = urlRetorno,
                        lstServico = new List<FESetup.Servico>(),
                        lstEndereco = new List<Endereco>(),
                        lstProdutoRisco = new List<ProdutoRisco>() { 
                        new ProdutoRisco{ 
                            CodProdEstabRisco = "MobileRede01", 
                            DesProdEstabRisco = "Leitor tarja magnetica para smartphone ou tablets", 
                            CategoriaProdEstabRisco = "MPOS",
                            QuantidadeProdEstabRisco = 1,
                            ValorUnitarioProdEstabRisco = valorTransacao,
                            CodTipoProdRisco = "Low"
                        }
                    }
                    };

                    setup.lstEndereco.Add(new Endereco
                    {
                        Cep = String.Format("{0}{1}", enderecoPrincipal.CodigoCep, enderecoPrincipal.CodComplementoCep),
                        EndrCli = enderecoPrincipal.Logradouro,
                        ComplEndr = enderecoPrincipal.ComplementoEndereco,
                        Estado = enderecoPrincipal.Estado,
                        Cidade = enderecoPrincipal.Cidade,
                        Pais = "76",
                        TipoEndrCli = "P"
                    });

                    setup.lstEndereco.Add(new Endereco
                    {
                        Cep = String.Format("{0}{1}", enderecoEntrega.CodigoCep, enderecoEntrega.CodComplementoCep),
                        EndrCli = enderecoEntrega.Logradouro,
                        ComplEndr = enderecoEntrega.ComplementoEndereco,
                        Estado = enderecoEntrega.Estado,
                        Cidade = enderecoEntrega.Cidade,
                        Pais = "76",
                        TipoEndrCli = "E"
                    });

                    setup.lstEndereco.Add(new Endereco
                    {
                        Cep = String.Format("{0}{1}", enderecoCobranca.CodigoCep, enderecoCobranca.CodComplementoCep),
                        EndrCli = enderecoCobranca.Logradouro,
                        ComplEndr = enderecoCobranca.ComplementoEndereco,
                        Estado = enderecoCobranca.Estado,
                        Cidade = enderecoCobranca.Cidade,
                        Pais = "76",
                        TipoEndrCli = "C"
                    });

                    foreach (var servico in servicos)
                    {
                        setup.lstServico.Add(new FESetup.Servico { codTipoServicoConf = servico });
                    }

                    log.GravarLog(EventoLog.InicioAgente, setup);

                    var retorno = contexto.Cliente.setupPlugin(setup);
                    token = retorno.Token;
                }

                return token;
            }
        }

        public string GetToken(string numPdv, decimal valorTransacao, string numPedido, int qtdParcela, string urlRetorno, IEnumerable<int> codServicos)
        {
            using (var log = Logger.IniciarLog("Get Token - Agente"))
            {
                String token = String.Empty;

                using (var contexto = new ContextoWCF<SetupPluginFEClient>())
                {
                    var servicos = codServicos.ToArray();

                    Setup setup = new Setup
                    {
                        CodDocCliente = numPdv.ToInt32(),
                        NumPdv = numPdv,
                        ValorTransacao = valorTransacao,
                        NumPedido = numPedido,
                        QtdParcela = qtdParcela,
                        UrlRetorno = urlRetorno,
                        lstServico = new List<FESetup.Servico>()
                    };

                    foreach (var servico in servicos)
                    {
                        setup.lstServico.Add(new FESetup.Servico { codTipoServicoConf = servico });
                    }

                    log.GravarLog(EventoLog.InicioAgente, setup);

                    var retorno = contexto.Cliente.setupPlugin(setup);
                    token = retorno.Token;
                }

                return token;
            }
        }
    }
}