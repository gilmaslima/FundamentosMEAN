using Redecard.PN.OutrosServicos.SharePoint.Modelos;
using Redecard.PN.OutrosServicos.SharePoint.NkPlanoContasServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Redecard.PN.OutrosServicos.SharePoint.Business
{
    /// <summary>
    /// Classe com métodos auxiliares referente à tela de oferta preço único
    /// </summary>
    public static class PrecoUnicoBusiness
    {
        /// <summary>CultureInfo pt-BR</summary>
        private static readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Obtém a lista dos itens de preço único em formato visualizável
        /// </summary>
        /// <param name="listPlanos">Dados fornecidos pelo serviço</param>
        /// <param name="codigoEntidade">Número do estabelecimento</param>
        /// <returns>Lista com os dados de preço único para exibição</returns>
        public static IEnumerable<PrecoUnicoView> GetPrecoUnicoViewData(List<PlanoPrecoUnico> listPlanos, Int32 codigoEntidade)
        {
            PrecoUnicoView p = new PrecoUnicoView();
            foreach (var plano in listPlanos)
            {
                p.AnoMesReferencia = plano.AnoMesReferencia.ToString("dd/MM/yyyy");
                p.DataInicioApuracao = plano.DataInicioApuracao.HasValue ? plano.DataInicioApuracao.Value.ToString("dd/MM/yyyy", ptBr) : "-";
                p.DataFimApuracao = plano.DataFimApuracao.HasValue ? plano.DataFimApuracao.Value.ToString("dd/MM/yyyy", ptBr) : "-";
                p.NomePlano = plano.NomePlano;
                p.ValorMensalidade = plano.ValorMensalidade.ToString("n", ptBr);
                p.ValorLimiteFaturamentoContratado = plano.ValorLimiteFaturamentoContratado.ToString("n", ptBr);
                p.ValorFaturamentoApurado = String.Format("{0}", plano.ValorFaturamentoApurado.ToString("C2", ptBr));
                p.ValorLimiteFaturamentoContratadoProRata = String.Format("{0}", plano.ValorLimiteFaturamentoContratadoProRata.ToString("C2", ptBr));
                p.ValorExcedenteContratado = String.Format("{0}", plano.ValorExcedenteContratado.ToString("C2", ptBr));
                p.ValorMensalidadeCobrada = String.Format("{0}", plano.ValorMensalidadeCobrada.ToString("C2", ptBr));
                p.ValorCobradoPeloExcedente = String.Format("{0}", plano.ValorCabradoPeloExcedente.ToString("C2", ptBr));
                p.Total = String.Format("{0}", plano.Total.ToString("C2", ptBr));
                p.CodigoEntidade = codigoEntidade.ToString();

                // complementa descrição do equipamento caso seja POO ou POS
                p.Equipamentos = plano.Equipamentos.Select(eq =>
                {
                    String descricao = String.Empty;

                    if (String.Compare("POO", eq.Tipo, true) == 0)
                    {
                        descricao = String.Concat(eq.Tipo, " (sem fio)");
                    }
                    else if (String.Compare("POS", eq.Tipo, true) == 0)
                    {
                        descricao = String.Concat(eq.Tipo, " (com fio)");
                    }
                    else
                    {
                        descricao = eq.Tipo;
                    }

                    return String.Format("{0} {1}", eq.QtdTerminais, descricao);

                }).ToArray();

                yield return p;
            }
        }
    }
}
