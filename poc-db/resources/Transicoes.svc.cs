using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Negocio;
using AutoMapper;

namespace Redecard.PN.Credenciamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Transicoes" in code, svc and config file together.
    public class Transicoes : ServicoBase, ITransicoes
    {
        public void GravarAtualizarPasso1(Proposta proposta, Endereco endereco, Endereco enderecoCorrespondencia, DomicilioBancario domCredito, List<Proprietario> proprietarios, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Endereco, Modelo.Endereco>();
                    Mapper.CreateMap<Proprietario, Modelo.Proprietario>();
                    Mapper.CreateMap<DomicilioBancario, Modelo.DomicilioBancario>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if(codRetorno == 0)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endereco), out codRetorno);
                    if(codRetorno == 0)
                        tranBLL.GravarAtualizarProprietarios(Mapper.Map<List<Modelo.Proprietario>>(proprietarios), out codRetorno);

                    if (proposta.CodTipoEstabelecimento == 1)
                    {
                        if (codRetorno == 0 && enderecoCorrespondencia != null)
                            tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(enderecoCorrespondencia), out codRetorno);
                        if (codRetorno == 0 && domCredito != null)
                            tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domCredito), out codRetorno);
                    }

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso2(Proposta proposta, List<Proprietario> proprietarios, Endereco enderecoComercial,
            Endereco enderecoCorrespondencia, Endereco enderecoInstalacao, DomicilioBancario domCredito,
            List<Produto> produtosCredito, List<Produto> produtosDebito, 
            List<Produto> produtosConstrucard, List<Patamar> patamares, out int codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Proprietario, Modelo.Proprietario>();
                    Mapper.CreateMap<Endereco, Modelo.Endereco>();
                    Mapper.CreateMap<DomicilioBancario, Modelo.DomicilioBancario>();
                    Mapper.CreateMap<Produto, Modelo.Produto>();
                    Mapper.CreateMap<Patamar, Modelo.Patamar>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProprietarios(Mapper.Map<List<Modelo.Proprietario>>(proprietarios), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(enderecoComercial), out codRetorno);
                    if (codRetorno == 0 && enderecoCorrespondencia != null)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(enderecoCorrespondencia), out codRetorno);
                    if (codRetorno == 0 && enderecoInstalacao != null)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(enderecoInstalacao), out codRetorno);
                    if (codRetorno == 0 && domCredito != null)
                        tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domCredito), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosCredito), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosDebito), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosConstrucard), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarPatamares(Mapper.Map<List<Modelo.Patamar>>(patamares), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso3(Proposta proposta, List<Produto> produtosCredito, List<Produto> produtosDebito, 
            List<Produto> produtosConstrucard, List<Patamar> patamares, Tecnologia tecnologia, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    codRetorno = 0;
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Produto, Modelo.Produto>();
                    Mapper.CreateMap<Tecnologia, Modelo.Tecnologia>();
                    Mapper.CreateMap<Patamar, Modelo.Patamar>();

                    if (tranBLL.ExisteTecnologias((Char)proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp))
                        tranBLL.RemoverTodasTecnologias((Char)proposta.CodTipoPessoa, proposta.NumCnpjCpf, proposta.IndSeqProp, null, out codRetorno);

                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosCredito), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosDebito), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutos(Mapper.Map<List<Modelo.Produto>>(produtosConstrucard), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarPatamares(Mapper.Map<List<Modelo.Patamar>>(patamares), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarTecnologia(Mapper.Map<Modelo.Tecnologia>(tecnologia), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso4(Proposta proposta, Tecnologia tecnologia, Endereco endComercial,
            Endereco endCorrespondencia, Endereco endInstalacao, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Tecnologia, Modelo.Tecnologia>();
                    Mapper.CreateMap<Endereco, Modelo.Endereco>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarTecnologia(Mapper.Map<Modelo.Tecnologia>(tecnologia), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endComercial), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endCorrespondencia), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarEndereco(Mapper.Map<Modelo.Endereco>(endInstalacao), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso5(Proposta proposta, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso6(Proposta proposta, DomicilioBancario domBancarioCredito, DomicilioBancario domBancarioDebito,
            DomicilioBancario domBancarioConstrucard, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<DomicilioBancario, Modelo.DomicilioBancario>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0 && domBancarioCredito != null)
                        tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domBancarioCredito), out codRetorno);
                    if (codRetorno == 0 && domBancarioDebito != null)
                        tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domBancarioDebito), out codRetorno);
                    if (codRetorno == 0 && domBancarioConstrucard != null)
                        tranBLL.GravarAtualizarDomicilioBancario(Mapper.Map<Modelo.DomicilioBancario>(domBancarioConstrucard), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso7(Proposta proposta, Tecnologia tecnologia, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Tecnologia, Modelo.Tecnologia>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarTecnologia(Mapper.Map<Modelo.Tecnologia>(tecnologia), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public void GravarAtualizarPasso8(Proposta proposta, List<Servico> servicos, List<ProdutoVan> produtosVan, out Int32 codRetorno)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    TransicoesBLL tranBLL = new TransicoesBLL();

                    Mapper.CreateMap<Proposta, Modelo.Proposta>();
                    Mapper.CreateMap<Servico, Modelo.Servico>();
                    Mapper.CreateMap<ProdutoVan, Modelo.ProdutoVan>();

                    tranBLL.GravarAtualizarProposta(Mapper.Map<Modelo.Proposta>(proposta), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizatServicos(Mapper.Map<List<Modelo.Servico>>(servicos), out codRetorno);
                    if (codRetorno == 0)
                        tranBLL.GravarAtualizarProdutosVan(Mapper.Map<List<Modelo.ProdutoVan>>(produtosVan), out codRetorno);

                    if (codRetorno > 0)
                        ts.Dispose();
                    else
                        ts.Complete();
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
