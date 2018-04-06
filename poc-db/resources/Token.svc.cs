using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Negocio;
using AutoMapper;

namespace Redecard.PN.Boston.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Token" in code, svc and config file together.
    public class Token : ServicoBase, IToken
    {
        public String GetTokenAnaliseRisco(String cpfCnpj, String nome, String sobrenome, DateTime dataFundacao, String email, String telefone1, String telefone2, String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos, Endereco enderecoPrincipal, Endereco enderecoEntrega, Endereco enderecoCobranca)
        {
            using (var log = Logger.IniciarLog("Get Token Análise de Risco"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    nome,
                    sobrenome,
                    dataFundacao,
                    email,
                    telefone1,
                    telefone2,
                    numPdv,
                    valorTransacao,
                    numPedido,
                    qtdParcela,
                    urlRetorno,
                    enderecoPrincipal,
                    enderecoEntrega,
                    enderecoCobranca
                });

                String token = String.Empty;
                var tokenBLL = new TokenBLL();

                Mapper.CreateMap<Endereco, Modelo.Endereco>();

                try
                {
                    token = tokenBLL.GetTokenAnaliseRisco(cpfCnpj, nome, sobrenome, dataFundacao, email, telefone1, telefone2, numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos, Mapper.Map<Modelo.Endereco>(enderecoPrincipal), Mapper.Map<Modelo.Endereco>(enderecoEntrega), Mapper.Map<Modelo.Endereco>(enderecoCobranca));
                }
                catch (Exception ex)
                {
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimAgente, new { token });

                return token;
            }
        }


        public String GetToken(String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<int> codServicos)
        {
            using (var log = Logger.IniciarLog("Get Token"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numPdv,
                    valorTransacao,
                    numPedido,
                    qtdParcela,
                    urlRetorno,
                });

                String token = String.Empty;
                var tokenBLL = new TokenBLL();

                try
                {
                    token = tokenBLL.GetToken(numPdv, valorTransacao, numPedido, qtdParcela, urlRetorno, codServicos);
                }
                catch (Exception ex)
                {
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimAgente, new { token });

                return token;
            }
        }
    }
}
