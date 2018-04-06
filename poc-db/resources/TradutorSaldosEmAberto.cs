using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public class TradutorSaldosEmAberto
    {
        public static DadosConsultaSaldosEmAbertoDTO TraduzirEnvioConsultarSaldosEmAberto(Modelo.DadosConsultaSaldosEmAberto origem)
        {
            DadosConsultaSaldosEmAbertoDTO retorno = new DadosConsultaSaldosEmAbertoDTO()
            {
                DataInicial = origem.DataInicial,
                DataFinal = origem.DataFinal,
                Estabelecimentos = origem.Estabelecimentos,
                CodigoSolicitacao = origem.CodigoSolicitacao
            };
            return retorno;

        }

        public static RetornoConsultaSaldosEmAberto TraduzirRetornoConsultaSaldosEmAberto(RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO> dados)
        {
            RetornoConsultaSaldosEmAberto retorno = new RetornoConsultaSaldosEmAberto();

            retorno.Detalhe = new List<BaseDetalhe>();

            //retorno.TotalBandeiraMes = dados.Totalizador.to;
            // List<ItemDetalheSaldosEmAberto> lstItems = new List<ItemDetalheSaldosEmAberto>();
            // List<TotalBandeiraMesSaldosEmAberto> lstTotalBandeiraMes = new List<TotalBandeiraMesSaldosEmAberto>();
            //DetalheMesSaldosEmAberto itemRetorno;
            ItemDetalheSaldosEmAberto itemCopia = null;

            foreach (BasicDTO item in dados.Registros)
            {
                string tipoRegistro = item.TipoRegistro;
                switch (tipoRegistro)
                {
                    case "DT":

                        ItemDetalheSaldosEmAbertoDTO itemDetalhe = (item as ItemDetalheSaldosEmAbertoDTO);
                        ItemDetalheSaldosEmAberto itemDT = new ItemDetalheSaldosEmAberto()
                        {
                            Tipo = item.TipoRegistro,
                            CodigoAgencia = itemDetalhe.CodigoAgencia,
                            CodigoBanco = itemDetalhe.CodigoBanco,
                            CodigoBandeira = itemDetalhe.CodigoBandeira,
                            CodigoEstabelecimento = itemDetalhe.CodigoEstabelecimento,
                            ContaCorrente = itemDetalhe.ContaCorrente,
                            DataReferencia = itemDetalhe.DataReferencia,
                            ValorBruto = itemDetalhe.ValorBruto,
                            ValorLiquido = itemDetalhe.ValorLiquido
                        };

                        retorno.Detalhe.Add(itemDT);
                        if (Object.ReferenceEquals(itemCopia, null))
                        {
                            itemCopia = itemDT;
                            itemCopia.QuantidadeDetalhe = 1;
                        }
                        else
                        {
                            itemCopia.QuantidadeDetalhe++;
                        }
                        break;
                    case "T1":
                        TotalBandeiraMesSaldosEmAbertoDTO totalmes = (item as TotalBandeiraMesSaldosEmAbertoDTO);

                        retorno.Detalhe.Add(new TotalBandeiraMesSaldosEmAberto()
                        {
                            Tipo = item.TipoRegistro,
                            CodigoBandeira = totalmes.CodigoBandeira,
                            DataReferencia = totalmes.DataReferencia,
                            DescricaoBandeira = totalmes.DescricaoBandeira,
                            ValorLiquido = totalmes.ValorLiquido
                        });

                        //  lstItems = new List<ItemDetalheSaldosEmAberto>();
                        itemCopia = null;
                        break;
                    case "T2":
                        TotalMesSaldosEmAbertoDTO totalMes = (item as TotalMesSaldosEmAbertoDTO);

                        retorno.Detalhe.Add(new DetalheMesSaldosEmAberto()
                        {
                            Tipo = item.TipoRegistro,
                            ValorLiquido = totalMes.ValorLiquido,
                            DataReferencia = totalMes.DataReferencia
                        });
                        itemCopia = null;
                        //}
                        //        itemRetorno);
                        //        lstTotalBandeiraMes = new List<TotalBandeiraMesSaldosEmAberto>();
                        break;
                }

            }

            retorno.TotalBandeiras = ObterTotaisBandeira(dados.Totalizador);
            retorno.QuantidadeTotalRegistros = dados.QuantidadeTotalRegistros;
            return retorno;
        }


        ItemDetalheSaldosEmAberto ObterItensBandeiraMes(ItemDetalheSaldosEmAbertoDTO dados)
        {
            ItemDetalheSaldosEmAberto retorno = new ItemDetalheSaldosEmAberto()
            {
                CodigoAgencia = dados.CodigoAgencia,
                CodigoBanco = dados.CodigoBanco,
                CodigoBandeira = dados.CodigoBandeira,
                CodigoEstabelecimento = dados.CodigoEstabelecimento,
                ContaCorrente = dados.ContaCorrente,
                DataReferencia = dados.DataReferencia,
                ValorBruto = dados.ValorBruto,
                ValorLiquido = dados.ValorLiquido
            };

            return retorno;
        }
        private static TotaisPorBandeiraSaldosEmAberto ObterTotaisBandeira(TotalizadorPorBandeiraSaldosEmAbertoDTO item)
        {
            TotaisPorBandeiraSaldosEmAberto total = new TotaisPorBandeiraSaldosEmAberto();

            BandeiraTotalSaldosEmAberto bandeiraItem;
            total.QuantidadeTotalBandeiras = item.QuantidadeBandeiras;

            total.TotalLiquido = item.ValorLiquido;

            total.TotaisBandeiras = new List<BandeiraTotalSaldosEmAberto>();

            foreach (TotalPorBandeiraSaldosEmAbertoDTO bandeira in item.TotaisBandeira)
            {
                bandeiraItem = new BandeiraTotalSaldosEmAberto()
                {
                    CodigoBandeira = bandeira.CodigoBandeira,
                    DescricaoBandeira = bandeira.DescricaoBandeira,
                    TotalBandeira = bandeira.TotalBandeira
                };

                total.TotaisBandeiras.Add(bandeiraItem);
            }



            return total;

        }


        public static List<PeriodoDisponivel> TraduzirRetornoConsultarPeriodosDisponiveis(RetornoPesquisaSemTotalizadorDTO<PeriodoDisponivelDTO> dados)
        {
            List<PeriodoDisponivel> retorno = new List<PeriodoDisponivel>();
            PeriodoDisponivel periodo;
            foreach (PeriodoDisponivelDTO item in dados.Registros)
            {
                periodo = new PeriodoDisponivel()
                {
                    DataPeriodoInicial = item.DataPeriodoInicial,
                    DataPeriodoFinal = item.DataPeriodoFinal,
                    CodigoSolicitacao = item.CodigoSolicitacao
                };

                retorno.Add(periodo);
            }

            return retorno;
        }
    }
}