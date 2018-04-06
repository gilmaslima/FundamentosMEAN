using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using System.Transactions;
using Redecard.PN.Boston.Negocio;
using AutoMapper;
using System.Web.Configuration;

namespace Redecard.PN.Boston.Servicos
{
    public class TransicoesBoston : ServicoBase, ITransicoesBoston
    {
        public RetornoGravarAtualizarPasso1 GravarAtualizarPasso1(Char tipoPessoa, Int64 cnpjCpf, String codEquipamento, Int32 codCanal, Int32 codGrupoRamo, Int32 codRamoAtividade, String cnpjCpfProprietario)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Passo 1"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioServico, new
                {
                    tipoPessoa,
                    cnpjCpf,
                    codEquipamento,
                    codCanal,
                    codGrupoRamo,
                    codRamoAtividade
                });

                var retorno = new RetornoGravarAtualizarPasso1 { CodigoRetorno = 0 };

                try
                {
                    Int32 qtdePropostasPendentes = 0;
                    Int32 qtdePVsAtivos = 0;
                    Int32 codTipoEstabelecimento = 0;
                    Int32 numPdvMatriz = 0;
                    Char indMarketingDireto = ' ';

                    TransicoesBLL tranBLL = new TransicoesBLL();
                    Mapper.CreateMap<Modelo.DadosSerasa, DadosSerasa>();
                    Mapper.CreateMap<Modelo.ProprietarioSerasa, ProprietarioSerasa>();
                    Mapper.CreateMap<Modelo.PropostaPendente, PropostaPendente>();
                    Mapper.CreateMap<Modelo.DomicilioBancario, DomicilioBancario>();
                    Mapper.CreateMap<Modelo.Endereco, Endereco>();
                    Mapper.CreateMap<Modelo.Tecnologia, Tecnologia>();

                    // Consulta quantidade de propostas pendentes ou pvs ativos para tecnologia MPOS
                    tranBLL.ConsultaQuantidadePropostasPendentesEPVsAtivosPorEquipamento(tipoPessoa, cnpjCpf, codEquipamento, out qtdePropostasPendentes, out qtdePVsAtivos);

                    // Se qtde pvs ativos > 0 Retorna para página de impossível continuar
                    if (qtdePVsAtivos > 0)
                    {
                        retorno.CodigoRetorno = 650;
                        retorno.DescricaoRetorno = "Quantidade de PVs ativos > 0 para esse equipamento, impossível continuar.";
                        return retorno;
                    }

                    // Somente para pessoa Jurídica
                    if (tipoPessoa == 'J')
                    {
                        // Consulta Serasa
                        retorno.DadosSerasa = Mapper.Map<DadosSerasa>(tranBLL.ConsultaSerasaPJ(cnpjCpf.ToString()));

                        if (retorno.DadosSerasa.CodigoRetorno != 0)
                        {
                            // Se Consulta Serasa != 0 ir para página de impossível continuar
                            retorno.CodigoRetorno = 651;
                            retorno.DescricaoRetorno = "Nenhum dado retornado do Serasa, impossível continuar.";
                            return retorno;
                        }
                        else if (retorno.DadosSerasa.Proprietarios.FirstOrDefault(p => p.CPFCNPJ.CpfCnpjToLong() == cnpjCpfProprietario.CpfCnpjToLong()) == null)
                        {
                            // Caso o cnpj/cpf do proprietário não esteja na lista de sócios do SERASA ir para página de impossível continuar
                            retorno.CodigoRetorno = 655;
                            retorno.DescricaoRetorno = "Cnpj/Cpf do proprietário não esta na lista de sócios do SERASA, impossível continuar.";
                            return retorno;
                        }

                        // Faz De/Para CNAE
                        if (!tranBLL.ListaRamosAtividadesPorCnaeEquipamento(retorno.DadosSerasa.CNAE, codEquipamento, ref codGrupoRamo, ref codRamoAtividade))
                        {
                            retorno.CodigoRetorno = 652;
                            retorno.DescricaoRetorno = "Não foi possível realizar de/para do cnae, impossível continuar.";
                            return retorno;
                        }

                        retorno.DadosSerasa.CodigoGrupoAtuacao = codGrupoRamo;
                        retorno.DadosSerasa.CodigoRamoAtividade = codRamoAtividade;
                    }

                    if (qtdePropostasPendentes == 0)
                    {
                        // NOVA PROPOSTA

                        // Consulta tipo estabelecimento
                        tranBLL.ConsultaTipoEstabelecimento(tipoPessoa, cnpjCpf, null, out codTipoEstabelecimento, out numPdvMatriz);
                        retorno.CodTipoEstabelecimento = codTipoEstabelecimento;
                        retorno.NumPdvMatriz = numPdvMatriz;

                        // Valida se o ramo obtido permite digitação da proposta em função do Canal e tipo de pessoa selecionado
                        if (!tranBLL.ValidaCanalTipoPessoa(codGrupoRamo, codRamoAtividade, codCanal, tipoPessoa))
                        {
                            retorno.CodigoRetorno = 653;
                            retorno.DescricaoRetorno = "Ramo não permite digitação de proposta, impossível continuar.";
                            return retorno;
                        }

                        // Se Ramo target = N ir para página de impossível continuar
                        if (!tranBLL.ValidaRamoTarget(codGrupoRamo, codRamoAtividade, ref indMarketingDireto))
                        {
                            retorno.CodigoRetorno = 654;
                            retorno.DescricaoRetorno = "Ramo não target, impossível continuar.";
                            return retorno;
                        }

                        retorno.IndMarketingDireto = indMarketingDireto;
                    }
                    else
                    {
                        // RECUPERAÇÃO DE PROPOSTA JÁ EXISTENTE

                        // Consulta o número de sequência da proposta
                        retorno.NumSequencia = tranBLL.GetNumSequencia(tipoPessoa, cnpjCpf, codEquipamento);

                        // Consulta proposta pendente
                        retorno.PropostaPendente = Mapper.Map<PropostaPendente>(tranBLL.GetPropostaPendente(tipoPessoa, cnpjCpf, retorno.NumSequencia));

                        // Recuperar dados do endereço
                        var enderecos = tranBLL.GetEnderecos(tipoPessoa, cnpjCpf, retorno.NumSequencia, null);
                        retorno.EnderecoComercial = Mapper.Map<Endereco>(enderecos.FirstOrDefault(e => e.IndTipoEndereco == '1'));
                        retorno.EnderecoCorrespondencia = Mapper.Map<Endereco>(enderecos.FirstOrDefault(e => e.IndTipoEndereco == '2'));

                        // Consulta dados tecnologia
                        retorno.DadosTecnologia = Mapper.Map<Tecnologia>(tranBLL.GetDadosTecnologia(tipoPessoa, cnpjCpf, retorno.NumSequencia));

                        // Consulta dados do domicílio bancário
                        retorno.DomicilioBancarioCredito = Mapper.Map<DomicilioBancario>(tranBLL.GetDomicilioBancario(tipoPessoa, cnpjCpf, retorno.NumSequencia, 1));
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                return retorno;
            }
        }

        public Int32 GravarAtualizarPasso2(Proposta proposta, List<Proprietario> proprietarios, Endereco endComercial, Endereco endCorrespondencia, Endereco endInstalacao, Tecnologia tecnologia)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Passo 2"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioServico, new
                {
                    proposta,
                    proprietarios,
                    endComercial,
                    endCorrespondencia,
                    endInstalacao,
                    tecnologia
                });

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        Int32 codRetorno = 0;
                        Int32? codFilial = null;
                        Int32? codZonaVenda = null;
                        List<Modelo.Proprietario> proprietariosOld;
                        List<Modelo.Produto> produtos = new List<Modelo.Produto>();

                        TransicoesBLL tranBLL = new TransicoesBLL();
                        Mapper.CreateMap<Proprietario, Modelo.Proprietario>();
                        Mapper.CreateMap<Proposta, Modelo.Proposta>();
                        Mapper.CreateMap<Endereco, Modelo.Endereco>();
                        Mapper.CreateMap<Tecnologia, Modelo.Tecnologia>();

                        // Recupera Valor do Aluguel
                        tecnologia.ValorEquipamento = tranBLL.GetValorAluguel("MPO", 'A', 'N');

                        // Recupera o indicador de venda digitada
                        tecnologia.IndHabilitaVendaDigitada = tranBLL.GetVendaDigitada((Int32)proposta.CodGrupoRamo, (Int32)proposta.CodRamoAtividade, proposta.CodTipoPessoa);

                        // Executa rotina que retorna dados da filial e da zona
                        String cep = String.Format("{0}{1}", endComercial.CodigoCep, endComercial.CodComplementoCep);
                        tranBLL.GetDadosFiliaisZonas('F', cep, 'R', ref codFilial, ref codZonaVenda);
                        proposta.CodFilial = codFilial;
                        proposta.CodZona = codZonaVenda;

                        // Inclui/Atualiza Proposta
                        tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);

                        if (codRetorno == 0)
                        {
                            // Busca Proprietários
                            proprietariosOld = tranBLL.GetProprietarios(proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp, null, null, null);

                            // Exclui todos os Proprietários
                            tranBLL.ExcluiTodosProprietarios(proprietariosOld, out codRetorno);
                        }

                        // Inclui/Atualiza Proprietarios
                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarProprietarios(Mapper.Map<List<Modelo.Proprietario>>(proprietarios), out codRetorno);

                        // Inclui/Atualiza endereços comercial, de instalação e de correspondência
                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endComercial), out codRetorno);

                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endCorrespondencia), out codRetorno);

                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endInstalacao), out codRetorno);

                        // Inclui/Atualiza Tecnologia
                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarTecnologia(Mapper.Map<Modelo.Tecnologia>(tecnologia), out codRetorno);

                        // Exclui todos os Produtos
                        if (codRetorno == 0)
                            tranBLL.ExcluiTodosProdutos(proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp, out codRetorno);

                        // Busca Produtos de Crédito para o ramo de atividade
                        if (codRetorno == 0)
                            produtos = tranBLL.GetProdutos(proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp,
                                proposta.UsuarioUltimaAtualizacao, 'S', null, null, null, (Int32)proposta.CodGrupoRamo, proposta.CodRamoAtividade.ToString(), (Int32)proposta.CodCanal);

                        // Inclui/Atualiza Produtos com patamares
                        if (codRetorno == 0)
                            tranBLL.GravarAtualizarProdutos(produtos, out codRetorno);

                        if (codRetorno == 0)
                        {
                            var patamares = tranBLL.PreenchePatamares(proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp,
                                proposta.UsuarioUltimaAtualizacao, produtos.Where(p => p.CodFeature == 3));
                            tranBLL.GravarAtualizarPatamares(patamares, out codRetorno);
                        }

                        if (codRetorno > 0)
                            ts.Dispose();
                        else
                            ts.Complete();

                        return codRetorno;
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                    }
                }
            }
        }

        public Int32 GravarAtualizarPasso3(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, DomicilioBancario domBancarioCredito)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Passo 3"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSeqProp,
                    domBancarioCredito
                });

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        Int32 codRetorno;
                        String mensagemRetorno = String.Empty;
                        TransicoesBLL tranBLL = new TransicoesBLL();
                        Mapper.CreateMap<DomicilioBancario, Modelo.DomicilioBancario>();

                        // Grava Dados Domicílio Bancário
                        tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domBancarioCredito), out codRetorno);

                        // Atualiza a fase filiação
                        if (codRetorno == 0)
                            codRetorno = tranBLL.AtualizaTaxaAtivacaoPropostaMPOS(codTipoPessoa, cnpjCpf, numSeqProp, 0, 2, "PORTAL");

                        if (codRetorno > 0)
                            ts.Dispose();
                        else
                            ts.Complete();

                        return codRetorno;
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                    }
                }
            }
        }

        public Int32 GravarAtualizarPasso4(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, ref Int32 numSolicitacao, String usuario, String tipoEquipamento, Int32 codBanco, Endereco endereco, out String descRetorno, out Int32 numPdv)
        {
            using (var log = Logger.IniciarLog("Gravar Atualizar Passo 4"))
            {
                // Grava parâmetros de entrada no log
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codTipoPessoa,
                    cnpjCpf,
                    numSequencia,
                    numSolicitacao,
                    usuario,
                    tipoEquipamento,
                    codBanco,
                    endereco
                });

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                try
                {
                    Int32 codRetorno = 0;
                    Int32 codCasoOcorrencia = 0;
                    DateTime dataRequisicaoOcorrencia = DateTime.MinValue;
                    Int32 numRequisicaoOcorrencia = 0;
                    String descricaoScript;
                    DateTime dataSituacaoScore = DateTime.MinValue;
                    String codTipoAmbiente = WebConfigurationManager.AppSettings["TipoAmbiente"].ToString();
                    Char indSituacaoProposta = default(Char);
                    numPdv = 0;

                    TransicoesBLL tranBLL = new TransicoesBLL();

                    // Recupera dados da proposta
                    var proposta = tranBLL.GetPropostaPendente(codTipoPessoa, cnpjCpf, numSequencia);
                    Int32 numTelefoneFax = proposta.NumTelefoneFax ?? 0;

                    // Recupera descrição do ramo de atividade
                    String descRamoAtividade = tranBLL.GetDescRamoAtividade(proposta.GrupoRamo, proposta.RamoAtividade);

                    // Recupera descrição do canal
                    String descCanal = tranBLL.GetDescCanal(null, proposta.Canal, "=");

                    // Recupera descrição da célula
                    String descCelula = tranBLL.GetDescCelula(proposta.Canal, proposta.Celula, null);

                    // Cancelar Ocorrência
                    if (numSolicitacao != 0)
                        tranBLL.CancelaOcorrenciaCredenciamento(usuario, numSolicitacao,
                            1, "Solicitação de Executar novamente Matriz de Risco via Portal", "CREDENCIAMENTO PORTAL", out codRetorno,
                            out descRetorno);

                    // Atualiza a fase filiação
                    if (codRetorno == 0)
                        codRetorno = tranBLL.AtualizaTaxaAtivacaoPropostaMPOS(codTipoPessoa, cnpjCpf, numSequencia, 0, 3, "PORTAL");

                    // Abertura Ocorrência
                    if (codRetorno == 0)
                        tranBLL.AberturaOcorrenciaCredenciamento(usuario, proposta.NumPdv,
                            codTipoPessoa.ToString(), codTipoAmbiente, cnpjCpf.ToString(), "FILIACAO", out codCasoOcorrencia,
                            out dataRequisicaoOcorrencia, out numRequisicaoOcorrencia, out numSolicitacao);

                    // Consulta Script
                    tranBLL.ConsultaScript(970, null, out descricaoScript);

                    // Calculo Score Risco
                    tranBLL.CalculoScoreRisco(cnpjCpf, codTipoPessoa, numSolicitacao,
                        proposta.DataFundacao, usuario, proposta.GrupoRamo, proposta.RamoAtividade, endereco.CodigoCep,
                        proposta.CodTipoEstabelecimento, proposta.Canal, tipoEquipamento, codBanco, 0, out dataSituacaoScore);

                    // Atualizar Ocorrência Score Risco
                    tranBLL.AtualizaOcorrenciaScoreRisco(cnpjCpf, codTipoPessoa, numSolicitacao, dataSituacaoScore, out codRetorno, out descRetorno);

                    // Atualizar Situação Proposta
                    if (codRetorno == 0)
                        tranBLL.AtualizaSituacaoProposta(codTipoPessoa, cnpjCpf, numSequencia, 'P', usuario, null,
                            proposta.NumPdv, 1, out codRetorno, out descRetorno);

                    // Gravar Ocorrência Credenciamento
                    if (codRetorno == 0)
                    {
                        tranBLL.GravaOcorrenciaCredenciamento(numRequisicaoOcorrencia, dataRequisicaoOcorrencia,
                            usuario, numSolicitacao, codCasoOcorrencia, "FILI9901",
                            cnpjCpf, numSequencia, proposta.RazaoSocial, codTipoPessoa, proposta.DataFundacao,
                            proposta.GrupoRamo, proposta.RamoAtividade, descRamoAtividade, proposta.RazaoSocial, endereco.Logradouro,
                            endereco.ComplementoEndereco, endereco.NumeroEndereco, endereco.Bairro, endereco.Cidade, endereco.Estado,
                            endereco.CodigoCep, endereco.CodComplementoCep, proposta.PessoaContato, proposta.NumDDD1, proposta.NumTelefone1,
                            proposta.Ramal1, proposta.NumDDDFax, numTelefoneFax, proposta.CodFilial, proposta.NumPdv,
                            '0', 'N', proposta.Canal, descCanal, proposta.Celula, descCelula, 99, 'N', descricaoScript, 'S',
                            out codRetorno, out descRetorno);
                    }

                    // Atualiza Ocorrência Proposta
                    if (codRetorno == 0)
                        tranBLL.AtualizaOcorrenciaProposta(codTipoPessoa, cnpjCpf, numSequencia,
                            numSolicitacao, DateTime.Now, usuario, out codRetorno, out descRetorno);

                    // Aguarda 15 seg
                    System.Threading.Thread.Sleep(30000);

                    // Consulta Situação Proposta
                    if (codRetorno == 0)
                    {
                        tranBLL.ConsultaPropostaCredenciamento(codTipoPessoa, cnpjCpf, numSequencia, out numPdv, out indSituacaoProposta);

                        // Caso situação não esteja F ou I ir para página de impossível continuar
                        if (indSituacaoProposta != 'F' && indSituacaoProposta != 'I')
                        {
                            codRetorno = tranBLL.AtualizaTaxaAtivacaoPropostaMPOS(codTipoPessoa, cnpjCpf, numSequencia, 0, 4, "PORTAL");
                            if (codRetorno == 0)
                                if (indSituacaoProposta == 'A')
                                    codRetorno = 699;
                                else
                                    codRetorno = 698;
                        }
                        else
                            // Atualiza a fase filiação
                            codRetorno = tranBLL.AtualizaTaxaAtivacaoPropostaMPOS(codTipoPessoa, cnpjCpf, numSequencia, 0, 5, "PORTAL");
                    }

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();

                    return codRetorno;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
                }
            }
        }

    }
}
