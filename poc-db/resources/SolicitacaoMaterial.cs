/*
© Copyright 2017 Rede S.A.
Autor : Mário de O. Neto
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Linq;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Rede.PN.AtendimentoDigital.SharePoint.MaterialVendaServico;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Handler para Inclusão e rotinas para solicitação de material(ais).
    /// </summary>
    public class SolicitacaoMaterial : HandlerBase
    {
        /// <summary>
        /// Url da página de Solicitação de Materiais
        /// </summary>
        private const String UrlSolicitacaoMaterial = "/sites/fechado/servicos/Paginas/pn_SolicitarMaterialVendas.aspx";

        /// <summary>
        /// Consulta solicitações em aberto.
        /// </summary>
        /// <returns>Retorna dados das solicitações em aberto</returns>
        [HttpGet]
        [Authorize(UrlSolicitacaoMaterial)]
        public HandlerResponse ConsultarSolicitacaoAberto()
        {
            using (Logger log = Logger.IniciarLog("Consulta Solicitação Material Aberto."))
            {
                try
                {
                    using (var contexto = new ContextoWCF<MaterialVendaServicoClient>())
                    {
                        MaterialVendaServico.Remessa[] remessas = contexto.Cliente.ConsultarProximasRemessas(base.Sessao.CodigoEntidade);

                        Object retorno = CheckarKitsPedidos(remessas);

                        return new HandlerResponse(retorno);
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    return new HandlerResponse(301, "Erro ao consultar Solicitação(ões) de Material(ais).",
                        new
                        {
                            Codigo = ex.Detail.Codigo,
                            CodeName = ex.Code != null ? ex.Code.Name : null,
                            CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                        }, null
                    );
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao consultar Solicitação(ões) de Material(ais).", ex);
                    return new HandlerResponse(HandlerBase.CodigoErro, "Erro ao consultar Solicitação(ões) de Material(ais).");
                }
            }
        }

        /// <summary>
        /// Incluir nova solicitação de material
        /// </summary>
        [HttpPost]
        [Authorize(UrlSolicitacaoMaterial)]
        public HandlerResponse IncluirSolicitacao()
        {
            using (Logger log = Logger.IniciarLog("Incluindo nova solicitação de material"))
            {
                try
                {
                    log.GravarMensagem("dados", base.Request["dados"]);
                    Core.SolicitacaoMaterial.SolicitacaoMaterial solicitacaoMaterial = new JavaScriptSerializer().Deserialize<Core.SolicitacaoMaterial.SolicitacaoMaterial>(Convert.ToString(base.Request["dados"].Trim()));

                    MaterialVendaServico.Endereco endereco = new MaterialVendaServico.Endereco
                    {
                        Bairro = solicitacaoMaterial.Endereco.Bairro,
                        CEP = solicitacaoMaterial.Endereco.Cep.Replace("-", String.Empty),
                        Cidade = solicitacaoMaterial.Endereco.Cidade,
                        Complemento = solicitacaoMaterial.Endereco.Complemento,
                        Contato = solicitacaoMaterial.Endereco.Contato,
                        DDDFax = String.IsNullOrWhiteSpace(solicitacaoMaterial.Endereco.Telefone) || solicitacaoMaterial.Endereco.Telefone.Length < 2 ? "" : solicitacaoMaterial.Endereco.Telefone.RemoverLetras().Substring(0, 2),
                        DDDTelefone = String.IsNullOrWhiteSpace(solicitacaoMaterial.Endereco.Telefone) || solicitacaoMaterial.Endereco.Telefone.Length < 2 ? "" : solicitacaoMaterial.Endereco.Telefone.RemoverLetras().Substring(0, 2),
                        DescricaoEndereco = solicitacaoMaterial.Endereco.Endereco,
                        Email = solicitacaoMaterial.Endereco.Email,
                        Fax = solicitacaoMaterial.Endereco.Telefone,
                        Numero = solicitacaoMaterial.Endereco.Numero,
                        Ramal = "0",
                        Site = ".",
                        Telefone = solicitacaoMaterial.Endereco.Telefone,
                        UF = solicitacaoMaterial.Endereco.Uf
                    };

                    List<MaterialVendaServico.Kit> kits = new List<MaterialVendaServico.Kit>();

                    //Os Kits serão incluídos conforme Cadastro do estabelecimento no GS...
                    //Para Cada Solicitação de materialm serão efetuados pedidos de todos os meteriais daquele tipo(s) escolhido(s) na tela.

                    if (solicitacaoMaterial.SolicitarKitBobina)
                    {
                        MaterialVendaServico.Kit[] kitsVendas = CarregarMateriaisVenda();

                        if (kitsVendas != null)
                        {
                            kits.AddRange(kitsVendas);
                        }
                    }

                    if (solicitacaoMaterial.SolicitarKitSinalizacao)
                    {
                        MaterialVendaServico.Kit[] kitsSinalizacao = CarregarKitsSinalizacao();
                        if (kitsSinalizacao != null)
                        {
                            kits.AddRange(kitsSinalizacao);
                        }
                    }

                    MaterialVendaServico.Motivo motivo = ConsultarMotivo();

                    kits.ForEach(f =>
                    {
                        f.Quantidade = 1;
                        f.Motivo = motivo;
                    });

                    Int32 codigoRetorno;
                    using (var contexto = new ContextoWCF<MaterialVendaServicoClient>())
                    {
                        codigoRetorno = contexto.Cliente.IncluirKit(kits.ToArray(), base.Sessao.CodigoEntidade, base.Sessao.NomeEntidade, base.Sessao.LoginUsuario, base.Sessao.NomeUsuario, true, endereco);

                        //Registro no histórico/log de atividades
                        Historico.RealizacaoServico(base.Sessao, "Solicitação de Material de Venda");

                        if (codigoRetorno == 0)
                        {
                            return new HandlerResponse(new
                             {
                                 sucesso = true,
                                 mensagem = ""
                             });
                        }
                        else
                        {
                            return new HandlerResponse(new
                            {
                                sucesso = false,
                                mensagem = "Erro ao Gerar Solicitação de Materiais"
                            });
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    return new HandlerResponse(301, "Erro ao incluir Solicitação(ões) de Material(ais).",
                        new
                        {
                            Codigo = ex.Detail.Codigo,
                            CodeName = ex.Code != null ? ex.Code.Name : null,
                            CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                        }, null
                    );
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao incluir Solicitação(ões) de Material(ais).", ex);
                    return new HandlerResponse(HandlerBase.CodigoErro, "Erro ao incluir Solicitação(ões) de Material(ais).");
                }
            }
        }

        /// <summary>
        /// Verifica se há solicitações para Kits de determinado tipo.
        /// </summary>
        private Object CheckarKitsPedidos(MaterialVendaServico.Remessa[] remessas)
        {
            MaterialVendaServico.Kit[] kitsVendas = CarregarMateriaisVenda();
            MaterialVendaServico.Kit[] kitsSinalizacao = CarregarKitsSinalizacao();

            Object retorno = new
            {
                kitBobinaSolicitado = remessas.Any(a => a.Kit != null && kitsVendas.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).ToUpper().Trim()).Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).ToUpper().Trim())),
                kitSinalizacaoSolicitado = remessas.Any(a => a.Kit != null && kitsSinalizacao.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).ToUpper().Trim()).Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).ToUpper().Trim()))
            };

            return retorno;
        }

        /// <summary>
        /// Carrega os materiais disponíveis para o estabelecimento
        /// </summary>
        private MaterialVendaServico.Kit[] CarregarMateriaisVenda()
        {
            using (Logger Log = Logger.IniciarLog("Carregando materias de venda"))
            {
                using (var contexto = new ContextoWCF<MaterialVendaServicoClient>())
                {
                    MaterialVendaServico.Kit[] remessas = contexto.Cliente.ConsultarKitsVendas(base.Sessao.CodigoEntidade);

                    return remessas != null && remessas.Length > 0 ? remessas : null;

                }
            }
        }

        /// <summary>
        /// Carrega os materiais de sinalização disponíveis para o estabelecimento
        /// </summary>
        private MaterialVendaServico.Kit[] CarregarKitsSinalizacao()
        {
            using (Logger Log = Logger.IniciarLog("Carregando kits de sinalização"))
            {
                using (var contexto = new ContextoWCF<MaterialVendaServicoClient>())
                {
                    MaterialVendaServico.Kit[] remessas = contexto.Cliente.ConsultarKitsSinalizacao(base.Sessao.CodigoEntidade);

                    return remessas != null && remessas.Length > 0 ? remessas : null;
                }
            }
        }
        
        /// <summary>
        /// Consulta motivo da solicitação material
        /// </summary>
        private MaterialVendaServico.Motivo ConsultarMotivo()
        {
            MaterialVendaServico.Motivo[] motivos;

            using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
            {
                motivos = client.Cliente.ConsultarMotivos();
            }

            return motivos != null && motivos.Any() ? motivos.First() : null;
        }
    }
}