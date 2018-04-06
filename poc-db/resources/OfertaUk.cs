/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Negocio
{
    public class OfertaUk : RegraDeNegocioBase
    {
        /// <summary>
        /// Consultar Contratos/Ofertas através do UK
        /// </summary>
        /// <returns></returns>
        public List<Modelo.PlanoContas.Oferta> ConsultarOfertas(Int32 codigoEntidade)
        {
            try
            {
                List<Modelo.PlanoContas.Oferta> ofertas = default(List<Modelo.PlanoContas.Oferta>);
                
                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                ofertas = ofertaAgente.ConsultarOfertas(codigoEntidade);

                if (ofertas != null && ofertas.Count > 0)
                {
                    ofertas.RemoveAll(o => o.Origem == "ZP" && o.TipoOferta == "MDR");
                }

                return ofertas;
            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar o Contrato da Oferta
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta a consultar o Contrato</param>
        /// <param name="codigoProposta">Código de Proposta da Oferta</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        public Modelo.ContratoOferta ConsultarContratoOferta(Int32 codigoOferta, Int64 codigoProposta, Int64 codigoEntidade)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarContratoOferta(codigoOferta, codigoProposta, codigoEntidade);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Verificar se algum dos PVs passados possui Oferta de Taxa
        /// </summary>
        /// <param name="pvs">Lista de PVs para verificar</param>
        /// <returns>
        /// <para>True - 1 ou mais dos PVs possui Oferta de Taxa</para>
        /// <para>False - nenhum dos PVs possui Oferta de Taxa</para>
        /// </returns>
        public bool PossuiOfertaTaxa(List<int> pvs)
        {
            Boolean possui = false;

            Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();

            foreach (Int32 pv in pvs)
            {
                var ofertas = ofertaAgente.ConsultarOfertas(pv);

                if (!Object.ReferenceEquals(ofertas, null))
                {
                    ofertas = ofertas
                                .Where(o =>
                                        o.TipoOferta == "MDR")
                                        //&& o.Status == Modelo.PlanoContas.StatusOferta.Contratado)
                                .ToList();

                    if (ofertas.Count > 0)
                        possui = true;
                }
            }

            return possui;
        }

        /// <summary>
        /// Obter listagem de Sazonalidades da Oferta no UK
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta</param>
        /// <param name="codigoContrato">Código do Contrato</param>
        /// <param name="codigoEstruturaMeta">Código de Estrutura Meta</param>
        /// <returns></returns>
        public List<Modelo.SazonalidadeOferta> ConsultarSazonalizades(Int32 codigoOferta, Int64 codigoContrato, Int32 codigoEstruturaMeta)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarSazonalizades(codigoOferta, codigoContrato, codigoEstruturaMeta);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Listar os ramos da Oferta do Estabelecimento
        /// </summary>
        /// <param name="codigoOfeta"></param>
        /// <param name="codigoProposta"></param>
        /// <param name="numeroCnpj"></param>
        public List<Modelo.RamosAtividadeOferta> ConsultarRamosOferta(Int64 codigoOferta, Int64 codigoProposta, Int64 numeroCnpj)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarRamosOferta(codigoOferta, codigoProposta, numeroCnpj);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar as faixas de meta da Oferta
        /// </summary>
        /// <param name="contrato">informações do Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade selecionadada da Oferta</param>
        public List<Modelo.FaixaMetaOferta> ConsultarFaixasMeta(Modelo.ContratoOferta contrato, Modelo.SazonalidadeOferta sazonalidade)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarFaixasMeta(contrato, sazonalidade);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar as Taxas Crédito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List<Modelo.ProdutoBandeiraMeta></returns>
        public List<Modelo.ProdutoBandeiraMeta> ConsultarTaxasCredito(Modelo.ContratoOferta contrato,
            Modelo.SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa)
        {
            try
            {

                List<Modelo.TaxaMeta> taxas = default(List<Modelo.TaxaMeta>);
                List<Modelo.ProdutoBandeiraMeta> produtos = default(List<Modelo.ProdutoBandeiraMeta>);

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                taxas = ofertaAgente.ConsultarTaxasCredito(contrato, 
                                                          sazonalidade,
                                                          numeroEstabelecimento,
                                                          codigoRamo,
                                                          codigoFaixa);

                //Agrupa as Taxas por Bandeira
                var taxasAgrupadas = taxas.GroupBy(t => t.DescricaoBandeira)
                    .ToList();

                //Se houverem taxas, inicializa a lista de Produtos
                if (taxasAgrupadas.Count > 0)
                    produtos = new List<Modelo.ProdutoBandeiraMeta>();

                //Para cada agrupamento de Taxas, adiciona um novo Produto com Taxas
                foreach (var p in taxasAgrupadas)
                {
                    produtos.Add(new Modelo.ProdutoBandeiraMeta()
                    {
                        //CodigoBandeira = p.Key,
                        DescricaoBandeira = p.ToList()[0].DescricaoBandeira,
                        Taxas = p.ToList()
                    });
                }

                return produtos;

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar as Taxas Débito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List<Modelo.ProdutoBandeiraMeta></returns>
        public List<Modelo.ProdutoBandeiraMeta> ConsultarTaxasDebito(Modelo.ContratoOferta contrato,
            Modelo.SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa)
        {
            try
            {

                List<Modelo.TaxaMeta> taxas = default(List<Modelo.TaxaMeta>);
                List<Modelo.ProdutoBandeiraMeta> produtos = default(List<Modelo.ProdutoBandeiraMeta>);

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                taxas = ofertaAgente.ConsultarTaxasDebito(contrato,
                                                          sazonalidade,
                                                          numeroEstabelecimento,
                                                          codigoRamo,
                                                          codigoFaixa);

                //Agrupa as Taxas por Bandeira
                var taxasAgrupadas = taxas.GroupBy(t => t.DescricaoBandeira)
                    .ToList();

                //Se houverem taxas, inicializa a lista de Produtos
                if (taxasAgrupadas.Count > 0)
                    produtos = new List<Modelo.ProdutoBandeiraMeta>();

                //Para cada agrupamento de Taxas, adiciona um novo Produto com Taxas
                foreach (var p in taxasAgrupadas)
                {
                    produtos.Add(new Modelo.ProdutoBandeiraMeta()
                    {
                        //CodigoBandeira = p.Key,
                        DescricaoBandeira = p.Key,
                        Taxas = p.ToList()
                    });
                }

                return produtos;

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Listar os históricos da Oferta
        /// <param name="contrato">Contrato com as informações de Código de Proposta e Código de Oferta</param>
        /// </summary>
        public List<Modelo.HistoricoOferta> ConsultarHistoricoOferta(Modelo.ContratoOferta contrato)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarHistoricoOferta(contrato);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Listar os estabelecimentos dos históricos da Oferta no UK
        /// </summary>
        /// <param name="historico">Informações do Histórico da Ofeta</param>
        /// <returns></returns>
        public List<Modelo.EstabelecimentoHistoricoOferta> ConsultarEstabelecimentosOferta(Modelo.HistoricoOferta historico)
        {
            try
            {

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                return ofertaAgente.ConsultarEstabelecimentosOferta(historico);

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar as Taxas Crédito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        public List<Modelo.ProdutoBandeiraMeta> ConsultarTaxasCreditoHistorico(Modelo.HistoricoOferta historico)
        {
            try
            {

                List<Modelo.TaxaMeta> taxas = default(List<Modelo.TaxaMeta>);
                List<Modelo.ProdutoBandeiraMeta> produtos = default(List<Modelo.ProdutoBandeiraMeta>);

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                taxas = ofertaAgente.ConsultarTaxasCreditoHistorico(historico);

                //Agrupa as Taxas por Bandeira
                var taxasAgrupadas = taxas.GroupBy(t => t.DescricaoBandeira)
                    .ToList();

                //Se houverem taxas, inicializa a lista de Produtos
                if (taxasAgrupadas.Count > 0)
                    produtos = new List<Modelo.ProdutoBandeiraMeta>();

                //Para cada agrupamento de Taxas, adiciona um novo Produto com Taxas
                foreach (var p in taxasAgrupadas)
                {
                    produtos.Add(new Modelo.ProdutoBandeiraMeta()
                    {
                        //CodigoBandeira = p.Key,
                        DescricaoBandeira = p.Key,
                        Taxas = p.ToList()
                    });
                }

                return produtos;

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consultar as Taxas Débito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        public List<Modelo.ProdutoBandeiraMeta> ConsultarTaxasDebitoHistorico(Modelo.HistoricoOferta historico)
        {
            try
            {

                List<Modelo.TaxaMeta> taxas = default(List<Modelo.TaxaMeta>);
                List<Modelo.ProdutoBandeiraMeta> produtos = default(List<Modelo.ProdutoBandeiraMeta>);

                Agentes.OfertaUk ofertaAgente = new Agentes.OfertaUk();
                taxas = ofertaAgente.ConsultarTaxasDebitoHistorico(historico);

                //Agrupa as Taxas por Bandeira
                var taxasAgrupadas = taxas.GroupBy(t => t.DescricaoBandeira)
                    .ToList();

                //Se houverem taxas, inicializa a lista de Produtos
                if (taxasAgrupadas.Count > 0)
                    produtos = new List<Modelo.ProdutoBandeiraMeta>();

                //Para cada agrupamento de Taxas, adiciona um novo Produto com Taxas
                foreach (var p in taxasAgrupadas)
                {
                    produtos.Add(new Modelo.ProdutoBandeiraMeta()
                    {
                        //CodigoBandeira = p.Key,
                        DescricaoBandeira = p.Key,
                        Taxas = p.ToList()
                    });
                }

                return produtos;

            }
            catch (PortalRedecardException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

    }
}
