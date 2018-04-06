using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Modelo;
using Redecard.PN.Boston.Agentes;

namespace Redecard.PN.Boston.Negocio
{
    public class TransicoesBLL : RegraDeNegocioBase
    {
        public void GravarAtualizarProposta(Proposta proposta, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarProposta(proposta, out codRetorno);
        }

        public void GravarAtualizarEndereco(Endereco endereco, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarEndereco(endereco, out codRetorno);
        }

        public void GravarAtualizarProprietarios(List<Proprietario> proprietarios, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarProprietarios(proprietarios, out codRetorno);
        }

        public void GravarAtualizarTecnologia(Tecnologia tecnologia, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarTecnologia(tecnologia, out codRetorno);
        }

        public void GravarAtualizarDomicilioBancario(DomicilioBancario domicilioBancario, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarDomicilioBancario(domicilioBancario, out codRetorno);
        }

        public void GravarAtualizarProdutos(List<Produto> produtos, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarProdutos(produtos, out codRetorno);
        }

        public void GravarAtualizarPatamares(List<Patamar> patamares, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarPatamares(patamares, out codRetorno);
        }

        public void GravarAtualizarServicos(List<Servico> servicos, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarServicos(servicos, out codRetorno);
        }

        public void GravarAtualizarProdutosVan(List<ProdutoVan> produtosVan, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravarAtualizarProdutosVan(produtosVan, out codRetorno);
        }

        public void CancelaOcorrenciaCredenciamento(String usuarioOcorrencia, Int32 numSolicitacao, Int32 codCasoOcorrencia, String motivoCancelamento, String obsCancelamento, out Int32 codRetorno, out String descricaoRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.CancelaOcorrenciaCredenciamento(usuarioOcorrencia, numSolicitacao, codCasoOcorrencia, motivoCancelamento, obsCancelamento, out codRetorno, out descricaoRetorno);
        }

        public void AberturaOcorrenciaCredenciamento(String usuarioOcorrencia,
                Int32 numPontoVenda,
                String codTipoPessoa,
                String codTipoAmbiente,
                String numCNPJCPFCliente,
                String descricaoOcorrencia,
                out Int32 codCasoOcorrencia,
                out DateTime dataRequisicaoOcorrencia,
                out Int32 numRequisicaoOcorrencia,
                out Int32 numSolicitacao)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.AberturaOcorrenciaCredenciamento(usuarioOcorrencia,
                    numPontoVenda,
                    codTipoPessoa,
                    codTipoAmbiente,
                    numCNPJCPFCliente,
                    descricaoOcorrencia,
                    out codCasoOcorrencia,
                    out dataRequisicaoOcorrencia,
                    out numRequisicaoOcorrencia,
                    out numSolicitacao);
        }

        public void ConsultaScript(Int32? codigoScript, Char? indEmissaoCarta, out String descricaoScript)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ConsultaScript(codigoScript, indEmissaoCarta, out descricaoScript);
        }

        public void CalculoScoreRisco(Int64 numCNPJ, Char codTipoPessoa, Int32 numOcorrencia, DateTime dataFundacao, String usuario, Int32 codGrupoRamo, Int32 codRamoAtivididade, String codCEP, Int32 codTipoEstabelecimento, Int32 codCanal, String codTipoEquipamento, Int32 codBanco, Int32 codServico, out DateTime dataSituacaoScore)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.CalculoScoreRisco(numCNPJ, codTipoPessoa, numOcorrencia, dataFundacao, usuario, codGrupoRamo, codRamoAtivididade, codCEP, codTipoEstabelecimento, codCanal, codTipoEquipamento, codBanco, codServico, out dataSituacaoScore);
        }

        public void AtualizaOcorrenciaScoreRisco(Int64 numCNPJ, Char codTipoPessoa, Int32 numOcorrencia, DateTime dataSituacaoScore, out Int32 codRetorno, out String descricaoRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.AtualizaOcorrenciaScoreRisco(numCNPJ, codTipoPessoa, numOcorrencia, dataSituacaoScore, out codRetorno, out descricaoRetorno);
        }

        public void AtualizaSituacaoProposta(Char codTipoPessoa, Int64 numCNPJ, Int32 numSeqProp, Char indSituacaoProposta, String usuarioUltimaAtualizacao, Int32? codMotivoRecusa, Int32 numeroPontoVenda, Int32? indOrigemAtualizacao, out Int32 codRetorno, out String descricaoRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.AtualizaSituacaoProposta(
                        codTipoPessoa,
                        numCNPJ,
                        numSeqProp,
                        indSituacaoProposta,
                        usuarioUltimaAtualizacao,
                        codMotivoRecusa,
                        numeroPontoVenda,
                        indOrigemAtualizacao,
                        out codRetorno,
                        out descricaoRetorno);
        }

        public Int32 AtualizaTaxaAtivacaoPropostaMPOS(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, Decimal? valorAtivacao, Int32? codFaseFiliacao, String usuarioUltimaAtualizacao)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.AtualizaTaxaAtivacaoPropostaMPOS(
                        codTipoPessoa,
                        cnpjCpf,
                        numSeqProp,
                        valorAtivacao,
                        codFaseFiliacao,
                        usuarioUltimaAtualizacao);
        }

        public void GravaOcorrenciaCredenciamento(Int32 numRequisicaoOcorrencia, DateTime dataRequisicaoOcorrencia,
                    String usuarioOcorrencia, Int32 numSolicitacao, Int32 codCasoOcorrencia, String codTipoOcorrencia,
                    Int64 numCNPJCPF, Int32 numSeqProposta, String razaoSocial, Char codTipoPessoa, DateTime dataFundacao,
                    Int32 codGrupoRamo, Int32 codRamoAtividade, String descRamoAtividade, String nomeFatura, String logradouro,
                    String complementoEndereco, String numeroEndereco, String bairro, String cidade, String estado,
                    String codigoCep, String codComplementoCep, String pessoaContato, String numDDD1, Int32 numTelefone1,
                    Int32 ramal1, String numDDDFax, Int32 numTelefoneFax, Int32 codFilial, Int32 numeroPontoVenda,
                    Char codCategoriaPontoVenda, Char indPropostaEmissor, Int32 codCanal, String descCanal, Int32 codCelula,
                    String descCelula, Int32 codPesoTarget, Char indProntaInstalacao, String textoScript, Char indMatrizRisco,
                    out Int32 codRetorno, out String descricaoRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GravaOcorrenciaCredenciamento(numRequisicaoOcorrencia, dataRequisicaoOcorrencia, usuarioOcorrencia,
                    numSolicitacao, codCasoOcorrencia, codTipoOcorrencia, numCNPJCPF, numSeqProposta, razaoSocial,
                    codTipoPessoa, dataFundacao, codGrupoRamo, codRamoAtividade, descRamoAtividade, nomeFatura,
                    logradouro, complementoEndereco, numeroEndereco, bairro, cidade, estado, codigoCep, codComplementoCep,
                    pessoaContato, numDDD1, numTelefone1, ramal1, numDDDFax, numTelefoneFax, codFilial, numeroPontoVenda,
                    codCategoriaPontoVenda, indPropostaEmissor, codCanal, descCanal, codCelula, descCelula, codPesoTarget,
                    indProntaInstalacao, textoScript, indMatrizRisco, out codRetorno, out descricaoRetorno);
        }

        public void AtualizaOcorrenciaProposta(Char codTipoPessoa, Int64 numCNPJ, Int32 numSeqProp, Int32 numOcorrencia, DateTime dataAberturaOcorrencia, String usuarioUltimaAtualizacao, out Int32 codRetorno, out String descricaoRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.AtualizaOcorrenciaProposta(codTipoPessoa, numCNPJ, numSeqProp, numOcorrencia, dataAberturaOcorrencia, usuarioUltimaAtualizacao, out codRetorno, out descricaoRetorno);
        }

        public void ConsultaQuantidadePropostasPendentesEPVsAtivosPorEquipamento(Char codTipoPessoa, Int64 numCNPJ, String codEquipamento, out Int32 qtdePropostasPendentes, out Int32 qtdePVsAtivos)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ConsultaQuantidadePropostasPendentesEPVsAtivosPorEquipamento(codTipoPessoa, numCNPJ, codEquipamento, out qtdePropostasPendentes, out qtdePVsAtivos);
        }

        public DadosSerasa ConsultaSerasaPJ(String cnpj)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.ConsultaSerasaPJ(cnpj);
        }

        public void ConsultaTipoEstabelecimento(Char tipoPessoa, Int64 cpfCnpj, Int32? numPdv, out Int32 codTipoEstabelecimento, out Int32 numPdvMatriz)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ConsultaTipoEstabelecimento(tipoPessoa, cpfCnpj, numPdv, out codTipoEstabelecimento, out numPdvMatriz);
        }

        public Boolean ValidaCanalTipoPessoa(Int32 codGrupoRamo, Int32 codRamoAtividade, Int32 codCanal, Char tipoPessoa)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.ValidaCanalTipoPessoa(codGrupoRamo, codRamoAtividade, codCanal, tipoPessoa);
        }

        public Boolean ListaRamosAtividadesPorCnaeEquipamento(String codCNAE, String codEquipamento, ref Int32 codGrupoRamo, ref Int32 codRamoAtividade)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.ListaRamosAtividadesPorCnaeEquipamento(codCNAE, codEquipamento, ref codGrupoRamo, ref codRamoAtividade);
        }

        public Boolean ValidaRamoTarget(Int32 codGrupoRamo, Int32 codRamoAtividade, ref Char indMarketingDireto)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.ValidaRamoTarget(codGrupoRamo, codRamoAtividade, ref indMarketingDireto);
        }

        public Int32 GetNumSequencia(Char codTipoPessoa, Int64 cnpjCpf, String codEquipamento)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetNumSequencia(codTipoPessoa, cnpjCpf, codEquipamento);
        }

        public PropostaPendente GetPropostaPendente(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetPropostaPendente(codTipoPessoa, cnpjCpf, numSequencia);
        }

        public List<Endereco> GetEnderecos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Char? indTipoEndereco)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetEnderecos(codTipoPessoa, cnpjCpf, numSequencia, indTipoEndereco);
        }

        public DomicilioBancario GetDomicilioBancario(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Int32 indTipoOperacaoProd)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetDomicilioBancario(codTipoPessoa, cnpjCpf, numSequencia, indTipoOperacaoProd);
        }

        public Tecnologia GetDadosTecnologia(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetDadosTecnologia(codTipoPessoa, cnpjCpf, numSequencia);
        }

        public Double GetValorAluguel(String codTipoEquipamento, Char? situacao, Char? indEquipamentoCompartilhado)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetValorAluguel(codTipoEquipamento, situacao, indEquipamentoCompartilhado);

        }

        public Char GetVendaDigitada(Int32 codGrupoRamo, Int32 codRamoAtivididade, Char codTipoPessoa)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetVendaDigitada(codGrupoRamo, codRamoAtivididade, codTipoPessoa);
        }

        public void GetDadosFiliaisZonas(Char tipoOperacao, String codCEP, Char codTipoCep, ref Int32? codFilial, ref Int32? codZonaVenda)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.GetDadosFiliaisZonas(tipoOperacao, codCEP, codTipoCep, ref codFilial, ref codZonaVenda);
        }

        public void ExcluiTodosProdutos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ExcluiTodosProdutos(codTipoPessoa, cnpjCpf, numSequencia, out codRetorno);
        }

        public List<Proprietario> GetProprietarios(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, Char? codTipoPesProprietario, Int64? numCNPJCPFProprietario, Char? indBuscaProprietario)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetProprietarios(codTipoPessoa, cnpjCpf, numSequencia, codTipoPesProprietario, numCNPJCPFProprietario, indBuscaProprietario);
        }

        public void ExcluiTodosProprietarios(List<Proprietario> proprietarios, out Int32 codRetorno)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ExcluiTodosProprietarios(proprietarios, out codRetorno);
        }

        public List<Produto> GetProdutos(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, String usuario, Char? indicadorCredito, Char? indicadorDebito, Char? indicadorVoucher, Char? indicadorPrivate, Int32 codGrupoRamo, String codRamoAtivididade, Int32 codCanalOrigem)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetProdutos(codTipoPessoa, cnpjCpf, numSequencia, usuario, indicadorCredito, indicadorDebito, indicadorVoucher, indicadorPrivate, codGrupoRamo, codRamoAtivididade, codCanalOrigem);
        }

        public String GetDescRamoAtividade(Int32 codGrupoRamo, Int32 codRamoAtividade)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetDescRamoAtividade(codGrupoRamo, codRamoAtividade);
        }

        public String GetDescCanal(Char? indSituacaoCanal, Int32 codCanal, String indSinalizacao)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetDescCanal(indSituacaoCanal, codCanal, indSinalizacao);
        }

        public String GetDescCelula(Int32 codCanal, Int32 codCelula, Int32? codAgencia)
        {
            TransicoesAG tranAG = new TransicoesAG();
            return tranAG.GetDescCelula(codCanal, codCelula, codAgencia);
        }

        public List<Modelo.Patamar> PreenchePatamares(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, String usuario, IEnumerable<Produto> patamares)
        {
            List<Modelo.Patamar> retorno = new List<Modelo.Patamar>();

            foreach (var patamar in patamares)
            {
                retorno.Add(new Modelo.Patamar
                {
                    codTipoPessoa = codTipoPessoa,
                    numCNPJ = cnpjCpf,
                    numSeqProp = numSeqProp,
                    codCca = patamar.CodCca,
                    codFeature = patamar.CodFeature,
                    codRegimePatamar = patamar.CodRegimePadrao,
                    indTipoOperacaoProd = patamar.IndTipoOperacaoProd,
                    patamarFinal = patamar.ValorLimiteParcela.ToString().ToInt32(),
                    patamarInicial = 2,
                    sequenciaPatamar = 1,
                    taxaPatamar = patamar.TaxaPadrao,
                    usuario = usuario
                });
            }

            return retorno;
        }

        public void ConsultaPropostaCredenciamento(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, out Int32 numPdv, out Char indSituacaoProposta)
        {
            TransicoesAG tranAG = new TransicoesAG();
            tranAG.ConsultaPropostaCredenciamento(codTipoPessoa, cnpjCpf, numSeqProp, out numPdv, out indSituacaoProposta);
        }
    }
}
