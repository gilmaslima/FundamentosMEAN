using Redecard.PN.DadosCadastrais.Servicos.Modelos;
using Redecard.PN.DadosCadastrais.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Redecard.PN.DadosCadastrais.Servicos.Operacoes
{
    public class OperacaoResumoTaxa
    {
        public static ResumoTaxa[] ObterResumoTaxas(List<DadosBancarios> listaCredito, List<DadosBancarios> listaDebito)
        {
            List<ResumoTaxa> retorno = new List<ResumoTaxa>();
            
            //Dicionario com os cartões do resumo.
            List<DicionarioBandeiraProduto> dicionariobandeiraProduto = ObterDicionarioBandeiraProduto();
            IDictionary<string, string> modalidades = ObterModalidades();

            // SKU: Nao deve conter as bandeiras AMEX(69), ELO(70) e ELO(71)
            #region regra
            listaCredito.RemoveAll(x =>
                    String.Compare(x.CodigoCartao, "61", true) == 0
                    || String.Compare(x.CodigoCartao, "69", true) == 0
                    || String.Compare(x.CodigoCartao, "70", true) == 0
                    || String.Compare(x.CodigoCartao, "71", true) == 0
                    || String.Compare(x.CodigoCartao, "88", true) == 0 );

            // SKU: Nao deve conter as bandeiras AMEX(69), ELO(70) e ELO(71)
            listaDebito.RemoveAll(x =>
                    String.Compare(x.CodigoCartao, "69", true) == 0
                    || String.Compare(x.CodigoCartao, "70", true) == 0
                    || String.Compare(x.CodigoCartao, "71", true) == 0 );

            List<DadosBancarios> listaTemp = new List<DadosBancarios>();
            listaTemp.AddRange(listaCredito);
            listaTemp.AddRange(listaDebito);
            #endregion

            foreach (var dado in listaTemp.GroupBy(g => g.DescricaoCartao)){
                    
                    if(dicionariobandeiraProduto.All(a => String.Compare(a.Produto, dado.Key, true) == 0)) continue;
                    
                    DadosBancarios resumo = dado.First();
                    DicionarioBandeiraProduto dicionario = dicionariobandeiraProduto.FirstOrDefault(f => String.Compare(f.Produto, dado.Key, true) == 0);
			
		    if(dicionario == null) continue;

                retorno.Add(new ResumoTaxa{
                    Agencia = resumo .CodigoAgencia,
                    Banco = resumo .NomeBanco,
                    Bandeira = dicionario.Bandeira,
                    Conta = resumo.ContaAtualizada,
                    Ordem = dicionario.Ordem,
                    Produto = dicionario.Produto,
                    ProdutoDescricao = dicionario.Descricao,
                    ProdutoSegmento = dicionario.Segmento,
                    TaxaAntiga = dado.Select(s => new TaxaAntiga{
                        Modalidade = modalidades[s.DescricaoFEAT.ToUpper()],
                        Taxas = new List<Taxas>{new Taxas{
                            Prazo = s.TemTarifa ? s.PercentualTarifa : s.PercentualTaxa,
                            Taxa = s.Taxa,
                            ParcelasMin = s.MinimoParcelas,
                            ParcelasMax = s.MaximoParcelas
                        }}.ToArray()
                    }).ToArray()
                });
            }
                return retorno.ToArray();
        }

        //Modalidade das taxas
        private static IDictionary<string, string> ObterModalidades()
        {
            IDictionary<string, string> retorno = new Dictionary<string, string>();
            retorno.Add("ROTATIVO", "à vista");
            retorno.Add("A VISTA", "à vista");
            retorno.Add("PARCELADO C/JUROS", "parcelado com juros");
            retorno.Add("PARCELADO S/JUROS", "parcelado sem juros");
            retorno.Add("PRE-DATADO", "pré-datado");
            return retorno;
        }

        //Espelho da Lista de bandeiras / Produtos
        private static List<DicionarioBandeiraProduto> ObterDicionarioBandeiraProduto()
        {
 	        return new List<DicionarioBandeiraProduto>{
                new DicionarioBandeiraProduto{
                    Bandeira = "MASTERCARD",
                    Descricao = "mastercard nacional",
                    Ordem = 1,
                    Produto = "MCL - MASTERCARD LOCAL",
                    Segmento = "crédito"
                },
                new DicionarioBandeiraProduto{
                    Bandeira = "VISA",
                    Descricao = "visa nacional",
                    Ordem = 2,
                    Produto = "VISA CREDITO",
                    Segmento = "crédito"
                },
                new DicionarioBandeiraProduto{
                    Bandeira = "MAESTRO",
                    Descricao = "maestro",
                    Ordem = 3,
                    Produto = "MAESTRO",
                    Segmento = "débito"
                },
                new DicionarioBandeiraProduto{
                    Bandeira = "VISA ELECTRON",
                    Descricao = "visa electron",
                    Ordem = 4,
                    Produto = "VISA ELECTRON",
                    Segmento = "débito"
                }
            };
        }
    }
}