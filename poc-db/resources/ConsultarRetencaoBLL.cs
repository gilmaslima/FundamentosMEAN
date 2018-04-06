using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    public class ConsultarRetencaoBLL : AbstractPesquisaComTotalizadorBLL<ConsultarRetencaoEnvioDTO, BasicDTO, ConsultarRetencaoTotaisRetornoDTO>
    {
        private static readonly ConsultarRetencaoBLL instance = new ConsultarRetencaoBLL();

        private ConsultarRetencaoBLL()
        {
        }

        public static ConsultarRetencaoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        protected override RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarRetencaoTotaisRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, ConsultarRetencaoEnvioDTO envio)
        {
            ConsultarSuspensaoAG ag = new ConsultarSuspensaoAG();

            ConsultarRetencaoRetornoDTO dto = ag.ConsultarRetencao(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            //PR - ConsultarRetencaoNumeroProcessoRetornoDTO
            //DC - ConsultarRetencaoDetalheProcessoCreditoRetornoDTO
            //DD - ConsultarRetencaoDetalheProcessoDebitoRetornoDTO
            //D1 - ConsultarRetencaoDescricaoComValorRetornoDTO
            //D2 - ConsultarRetencaoDescricaoSemValorRetornoDTO

            List<BasicDTO> registros = new List<BasicDTO>();

            bool retornouDC = false;
            bool retornouDD = false;
            bool primeiroRegistroDC = true;
            bool primeiroRegistroDD = true;

            foreach (BasicDTO basicDTO in dto.Registros)
            {
                string tipoRegistro = basicDTO.TipoRegistro;

                if (tipoRegistro == "PR")
                {
                    retornouDC = false;
                    retornouDD = false;
                    primeiroRegistroDC = true;
                    primeiroRegistroDD = true;

                    registros.Add((ConsultarRetencaoNumeroProcessoRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "DC")
                {
                    retornouDC = true;

                    if (primeiroRegistroDC)
                    {
                        // adiciona o header.
                        registros.Add(ObterHeader("HDC1"));
                        registros.Add(ObterHeader("HDC2"));

                        primeiroRegistroDC = false;
                    }

                    registros.Add((ConsultarRetencaoDetalheProcessoCreditoRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "DD")
                {
                    retornouDD = true;

                    if (primeiroRegistroDD)
                    {
                        // adiciona o header.
                        registros.Add(ObterHeader("HDD1"));
                        registros.Add(ObterHeader("HDD2"));

                        primeiroRegistroDD = false;
                    }

                    registros.Add((ConsultarRetencaoDetalheProcessoDebitoRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "D1")
                {
                    ConsultarRetencaoDescricaoComValorRetornoDTO retornoDTO = (ConsultarRetencaoDescricaoComValorRetornoDTO)basicDTO;

                    if (retornouDC && !retornouDD)
                    {
                        retornoDTO.TipoRegistro = "DCT_D1";
                    }
                    else if (retornouDC && retornouDD)
                    {
                        retornoDTO.TipoRegistro = "DDT_D1";
                    }

                    registros.Add(retornoDTO);
                }
                else if (tipoRegistro == "D2")
                {
                    ConsultarRetencaoDescricaoSemValorRetornoDTO retornoDTO = (ConsultarRetencaoDescricaoSemValorRetornoDTO)basicDTO;

                    if (retornouDC && !retornouDD)
                    {
                        retornoDTO.TipoRegistro = "DCT_D2";
                    }
                    else if (retornouDC && retornouDD)
                    {
                        retornoDTO.TipoRegistro = "DDT_D2";
                    }

                    //registros.Add(retornoDTO);
                }
            }

            RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarRetencaoTotaisRetornoDTO> result = new RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarRetencaoTotaisRetornoDTO>();
            result.Registros = registros;
            result.Totalizador = dto.Totais;
            result.QuantidadeTotalRegistros = registros.Count;

            return result;
        }

        private BasicDTO ObterHeader(string tipoRegistro)
        {
            BasicDTO result = new BasicDTO();
            result.TipoRegistro = tipoRegistro;

            return result;
        }
    }
}
