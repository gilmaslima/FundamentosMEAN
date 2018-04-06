/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Comum.EntidadeServico;
using System.Linq;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    public sealed class SessaoUsuario : HandlerBase
    {
        /// <summary>7
        /// ParametrosConfiguracaoRepository
        /// </summary>
        private readonly ParametrosConfiguracaoRepository parametrosConfiguracaoRepository;

        /// <summary>
        /// SessaoUsuario
        /// </summary>
        public SessaoUsuario()
        {
            this.parametrosConfiguracaoRepository = new ParametrosConfiguracaoRepository();
        }

        /// <summary>
        /// Implementação de handler para obtenção de dados do usuário.
        /// </summary>
        [HttpGet]
        public HandlerResponse ObterDadosSessaoUsuario()
        {
            return new HandlerResponse(new
            {
                codigo = Sessao.CodigoEntidade,
                codigoSalesforce = CryptoSalesforce.HashPV(Sessao.CodigoEntidade),
                email = Sessao.Email.RemoverEspacos(),
                empresa = Sessao.NomeEntidade.RemoverEspacos(),
                nome = Sessao.NomeUsuario.RemoverEspacos(),
                telefone = Formatador.Telefone(Sessao.DDDCelular, Sessao.Celular),
                codigoSegmento = Sessao.CodigoSegmento,

                features = new
                {
                    chat = new
                    {
                        //chat disponível apenas para clientes TOP Varejo
                        habilitado = EstabelecimentoTopVarejoChat(base.Sessao)
                    }
                },

                timestamp = new {
                    yyyy = DateTime.Today.ToString("yyyy"),
                    MM = DateTime.Today.ToString("MM"),
                    dd = DateTime.Today.ToString("dd"),
                    hh = DateTime.Now.ToString("hh"),
                    mm = DateTime.Now.ToString("mm"),
                    ss = DateTime.Now.ToString("ss")
                }
            });
        }

        /// <summary>
        /// Implementação de handler para obtenção do endereço do estabelecimento.
        /// </summary>
        /// <returns>
        /// Dados tratados.
        /// </returns>
        [HttpGet]
        public HandlerResponse ObterDadosEnderecoEstabelecimento()
        {
            using (Logger log = Logger.IniciarLog("Carregando endereço do estabelecimento"))
            {
                try
                {
                    Int32 CodigoRetorno = default(Int32);
                    Endereco[] enderecos = default(Endereco[]);

                    using (var entidadeCliente = new ContextoWCF<EntidadeServicoClient>())
                    {
                        enderecos = entidadeCliente.Cliente.ConsultarEndereco(out CodigoRetorno, Sessao.CodigoEntidade, "I");
                    }

                    if (enderecos == null || enderecos.Length == 0)
                    {
                        return new HandlerResponse(
                            CodigoRetorno, 
                            "EntidadeServico.ConsultarEndereco", null, 
                            "Erro ao consultar endereço do estabelecimento: nenhum estabelecimento encontrado.");
                    }

                    if (CodigoRetorno > 0)
                    {
                        return new HandlerResponse(
                            CodigoRetorno,
                            "EntidadeServico.ConsultarEndereco", null,
                            "Erro ao consultar endereço do estabelecimento.");
                    }

                    Endereco enderecoEstabelecimento = enderecos[0];

                    HandlerResponse response = new HandlerResponse();
                    response.Codigo = CodigoRetorno;
                    response.Mensagem = "sucesso";
                    response.Dados = new
                    {
                        endereco = enderecoEstabelecimento.EnderecoEstabelecimento.RemoverEspacos(),
                        numero = enderecoEstabelecimento.Numero.RemoverEspacos(),
                        complemento = enderecoEstabelecimento.Complemento.RemoverEspacos(),
                        bairro = enderecoEstabelecimento.Bairro.RemoverEspacos(),
                        CEP = enderecoEstabelecimento.CEP.RemoverEspacos(),
                        cidade = enderecoEstabelecimento.Cidade.RemoverEspacos(),
                        UF = enderecoEstabelecimento.UF.RemoverEspacos(),
                        contato = enderecoEstabelecimento.Contato.RemoverEspacos(),
                        telefoneEstabelecimento = Formatador.Telefone(enderecoEstabelecimento.Telefone),
                        codigo = Sessao.CodigoEntidade,
                        codigoSalesforce = CryptoSalesforce.HashPV(Sessao.CodigoEntidade),
                        email = Sessao.Email.RemoverEspacos(),
                        empresa = Sessao.NomeEntidade.RemoverEspacos(),
                        nome = Sessao.NomeUsuario.RemoverEspacos(),
                        telefone = Formatador.Telefone(Sessao.DDDCelular, Sessao.Celular),
                        codigoSegmento = Sessao.CodigoSegmento,

                        features = new
                        {
                            chat = new
                            {
                                //chat disponível apenas para clientes TOP Varejo
                                habilitado = EstabelecimentoTopVarejoChat(base.Sessao)
                            }
                        },

                        timestamp = new
                        {
                            yyyy = DateTime.Today.ToString("yyyy"),
                            MM = DateTime.Today.ToString("MM"),
                            dd = DateTime.Today.ToString("dd"),
                            hh = DateTime.Now.ToString("hh"),
                            mm = DateTime.Now.ToString("mm"),
                            ss = DateTime.Now.ToString("ss")
                        }
                    };
                    return response;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    return new HandlerResponse(
                        ex.Detail != null ? ex.Detail.Codigo : CodigoErro,
                        "Erro durante consulta do endereço do estabelecimento", null,
                        ex.Detail != null ? ex.Detail.Fonte : Fonte);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    return new HandlerResponse(
                        CodigoErro,
                        "Erro genérico durante consulta do endereço do estabelecimento", null,
                        Fonte);
                }
            }
        }

        /// <summary>
        /// Validação do acesso por URL
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HandlerResponse ValidarAcessoUrl()
        {
            String url = base.Request["url"];

            if (String.IsNullOrEmpty(url))
            {
                return new HandlerResponse(CodigoErro, "Url não informada.");
            }

            try
            {
                Boolean acesso = Sessao.UsuarioMaster() ||
                                 (Sessao.Servicos != null &&
                                  Sessao.Servicos.Flatten(itemMenu => itemMenu.Items)
                                                 .SelectMany(itemMenu => itemMenu.Paginas)
                                                 .Any(pagina => String.Compare(pagina.Url, url, true) == 0));

                return new HandlerResponse( new { acesso = acesso } );
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro durante validação de URL", ex);
                SharePointUlsLog.LogErro(ex);
                return new HandlerResponse(CodigoErro, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro genérico durante validação de URL", ex);
                SharePointUlsLog.LogErro(ex);
                return new HandlerResponse(CodigoErro, ex.Message);
            }
        }

        /// <summary>
        /// Verifica se o estabelecimento é Top Varejo
        /// </summary>
        /// <returns></returns>
        public Boolean EstabelecimentoTopVarejoEmail(SPWeb web, Sessao sessao)
        {
            String segmentosEmailResolutivo = CacheAtendimento.GetItem<String>("segmentosEmailResolutivo");
            if (String.IsNullOrWhiteSpace(segmentosEmailResolutivo))
            {
                ParametrosConfiguracao parametroConfiguracao = this.parametrosConfiguracaoRepository.Get(web, "segmentosEmailResolutivo");
                if (parametroConfiguracao != null && !String.IsNullOrWhiteSpace(parametroConfiguracao.Valor))
                {
                    segmentosEmailResolutivo = (parametroConfiguracao.Valor ?? String.Empty).ToUpper();
                    CacheAtendimento.AddItem("segmentosEmailResolutivo", segmentosEmailResolutivo);
                }
            }
            Char codigoSegmento = Char.ToUpper(sessao.CodigoSegmento);
            return ((segmentosEmailResolutivo ?? String.Empty).Contains(codigoSegmento));
        }

        /// <summary>
        /// Verifica se o estabelecimento é Top Varejo
        /// </summary>
        /// <returns></returns>
        public Boolean EstabelecimentoTopVarejoChat(Sessao sessao)
        {
            String segmentosGavetaChat = CacheAtendimento.GetItem<String>("segmentosGavetaChat");
            if (String.IsNullOrWhiteSpace(segmentosGavetaChat))
            {
                ParametrosConfiguracao parametroConfiguracao = this.parametrosConfiguracaoRepository.Get(base.CurrentSPContext.Web, "segmentosGavetaChat");
                if (parametroConfiguracao != null && !String.IsNullOrWhiteSpace(parametroConfiguracao.Valor))
                {
                    segmentosGavetaChat = (parametroConfiguracao.Valor ?? String.Empty).ToUpper();
                    CacheAtendimento.AddItem("segmentosGavetaChat", segmentosGavetaChat);
                }
            }

            Char codigoSegmento = Char.ToUpper(sessao.CodigoSegmento);
            return ((segmentosGavetaChat ?? String.Empty).Contains(codigoSegmento));                    
        }
    }
}