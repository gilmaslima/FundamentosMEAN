using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Agentes.ISAutocredPF.InterfacePF;
using Redecard.PN.Credenciamento.Agentes.ISAutocredPJ.InterfacePJ;

namespace Redecard.PN.Credenciamento.Agentes
{
    public class SerasaAG : AgentesBase
    {
        /// <summary>
        /// Consulta dados de um cnpj na base do serasa
        /// </summary>
        /// <param name="cnpj"></param>
        public Modelo.PJ ConsultaSerasaPJ(String cnpj)
        {
            #region [ Declara os parâmetros de entrada e saída ]

            String pj_cod_retorno = "00";
            String pj_cnpj_cpf = cnpj.PadLeft(15, Char.Parse("0"));
            String pj_tipo_pess = "J";
            String pj_msg_retorno = String.Empty;
            String pj_stat_docto = String.Empty;
            String pj_compl_grafia = String.Empty;
            String pj_tp_logradouro = String.Empty;
            String pj_desc_logradouro = String.Empty;
            String pj_nro = String.Empty;
            String pj_complemento = String.Empty;
            String pj_bairro = String.Empty;
            String pj_cidade = String.Empty;
            String pj_uf = String.Empty;
            String pj_cep = String.Empty;
            String pj_ddd = String.Empty;
            String pj_tel = String.Empty;
            String pj_nome_fant = String.Empty;
            String pj_dt_fundacao = String.Empty;
            PJ_COD_CNAE_OCC[] pj_cod_cnae_occ;
            PJ_SOC_OCC[] pj_soc_occ;
            ISAutocredPJ.InterfacePJ.ClientContext clientContext = new ISAutocredPJ.InterfacePJ.ClientContext();
            clientContext.TransactionId = "IS99";
            clientContext.ProgramName = "WF050";

            #endregion

            #region [ Realiza a chamada os HIS ]

            using (InterfacePJClient client = new InterfacePJClient())
            {
                client.ConsultarPJ(
                    ref pj_cod_retorno,
                    ref pj_cnpj_cpf,
                    ref pj_tipo_pess,
                    out pj_msg_retorno,
                    out pj_stat_docto,
                    out pj_compl_grafia,
                    out pj_tp_logradouro,
                    out pj_desc_logradouro,
                    out pj_nro,
                    out pj_complemento,
                    out pj_bairro,
                    out pj_cidade,
                    out pj_uf,
                    out pj_cep,
                    out pj_ddd,
                    out pj_tel,
                    out pj_nome_fant,
                    out pj_dt_fundacao,
                    out pj_cod_cnae_occ,
                    out pj_soc_occ,
                    ref clientContext
                    );
            }

            #endregion

            #region [ Popula a classe de modelo para retorno ]

            Modelo.PJ retorno = new Modelo.PJ()
            {
                Bairro = pj_bairro,
                CEP = pj_cep,
                Cidade = pj_cidade,
                CNPJ = pj_cnpj_cpf,
                CodRetorno = pj_cod_retorno,
                Complemento = pj_complemento,
                ComplGrafia = pj_compl_grafia,
                DataFundacao = pj_dt_fundacao,
                DDD = pj_ddd,
                DescLogradouro = pj_desc_logradouro,
                NomeFantasia = pj_nome_fant,
                Numero = pj_nro,
                StatDocto = pj_stat_docto,
                Telefone = pj_tel,
                TipoLogradouro = pj_tp_logradouro,
                TipoPessoa = pj_tipo_pess,
                UF = pj_uf
            };

            retorno.Socios = new List<Modelo.Socio>();
            
            Int32 count = 0;
            while ( pj_soc_occ.Length > count && !String.IsNullOrEmpty(pj_soc_occ[count].PJ_NOME_SOC))
            {
                retorno.Socios.Add(new Modelo.Socio()
                {
                    CPF_CNPJ = pj_soc_occ[count].PJ_NRO_CPF_CNPJ,
                    Nome = pj_soc_occ[count].PJ_NOME_SOC,
                    Participacao = pj_soc_occ[count].PJ_PERC_PART_SOC,
                    TipoPessoa = pj_soc_occ[count].PJ_TIPO_PESS_SOC
                });

                count++;
            }

            retorno.CNAEs = new List<Modelo.CodigoCNAE>();
            count = 0;
            while ( pj_cod_cnae_occ.Length > count && !String.IsNullOrEmpty(pj_cod_cnae_occ[count].PJ_COD_CNAE))
            {
                retorno.CNAEs.Add(new Modelo.CodigoCNAE()
                {
                    CodCNAE = pj_cod_cnae_occ[count].PJ_COD_CNAE
                });

                count++;
            }

            #endregion

            return retorno;
        }

        /// <summary>
        /// Consulta dados de um cpf na base do serasa
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public Modelo.PF ConsultaSerasaPF(String cpf)
        {
            #region [ Declara os parâmetros de entrada e saída ]

            String pj_cod_retorno = "00";
            String pj_cnpj_cpf = cpf;
            String pj_tipo_pess = "F";
            String pj_msg_retorno = String.Empty;
            String pj_stat_docto = String.Empty;
            String pj_compl_grafia = String.Empty;
            String pj_tp_logradouro = String.Empty;
            String pj_desc_logradouro = String.Empty;
            String pj_nro = String.Empty;
            String pj_complemento = String.Empty;
            String pj_bairro = String.Empty;
            String pj_cidade = String.Empty;
            String pj_uf = String.Empty;
            String pj_cep = String.Empty;
            String pj_ddd = String.Empty;
            String pj_tel = String.Empty;
            String pj_dt_nasc = String.Empty;
            ISAutocredPF.InterfacePF.ClientContext clientContext = new ISAutocredPF.InterfacePF.ClientContext();
            clientContext.TransactionId = "IS99";
            clientContext.ProgramName = "WF050";

            #endregion

            #region [ Realiza a chamada ao HIS]

            using (InterfacePFClient client = new InterfacePFClient())
            {
                client.ConsultarPF(
                    ref pj_cod_retorno,
                    ref pj_cnpj_cpf,
                    ref pj_tipo_pess,
                    out pj_msg_retorno,
                    out pj_stat_docto,
                    out pj_compl_grafia,
                    out pj_tp_logradouro,
                    out pj_desc_logradouro,
                    out pj_nro,
                    out pj_complemento,
                    out pj_bairro,
                    out pj_cidade,
                    out pj_uf,
                    out pj_cep,
                    out pj_ddd,
                    out pj_tel,
                    out pj_dt_nasc,
                    ref clientContext
                    );
            }

            #endregion

            #region [ Popula a classe de modelo para retorno ]

            Modelo.PF retorno = new Modelo.PF()
            {
                Bairro = pj_bairro,
                CEP = pj_cep,
                Cidade = pj_cidade,
                CNPJ = pj_cnpj_cpf,
                CodRetorno = pj_cod_retorno,
                Complemento = pj_complemento,
                ComplGrafia = pj_compl_grafia,
                DataNascimento = pj_dt_nasc,
                DDD = pj_ddd,
                DescLogradouro = pj_desc_logradouro,
                Numero = pj_nro,
                StatDocto = pj_stat_docto,
                Telefone = pj_tel,
                TipoLogradouro = pj_tp_logradouro,
                TipoPessoa = pj_tipo_pess,
                UF = pj_uf
            };

            #endregion

            return retorno;
        }
    }
}
