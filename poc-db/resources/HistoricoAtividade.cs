using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Redecard.PN.Comum;
using Redecard.PN.Modelo;
using System.Xml.Serialization;
using System.Xml;

namespace Redecard.PN.Negocio
{
    /// <summary>
    /// Classe de negócio para o Histórico/Log de Atividades
    /// </summary>
    public class HistoricoAtividade : RegraDeNegocioBase
    {
        /// <summary>
        /// Consulta os tipos de atividade existentes
        /// </summary>
        /// <param name="exibir">Opcional: se deve filtrar os tipos de atividade visíveis para consulta</param>
        /// <returns>Lista de tipos de atividade</returns>
        public List<TipoAtividade> Consultar(Boolean? exibir)
        {
            return new Data.HistoricoAtividade().Consultar(exibir);
        }

        /// <summary>
        /// Consulta o Histórico de Atividades
        /// </summary>
        /// <param name="codigoHistorico">Opcional: código do histórico de atividade</param>
        /// <param name="codigoEntidade">Opcional: código da entidade</param>
        /// <param name="codigoTipoAtividade">Opcional: código do tipo de atividade</param>
        /// <param name="dataInicio">Opcional: data de início</param>
        /// <param name="dataFim">Opcional: data de término</param>
        /// <param name="nomeUsuario">Opcional: nome do usuário responsável</param>
        /// <param name="emailUsuario">Opcional: email do usuário responsável</param>
        /// <param name="funcionalOperador">Opcional: Funcional do operador responsável</param>
        /// <param name="tipoAtividadeVisible">Opcional: se deve ser do tipo de atividade visível</param>
        /// <returns>Histórico de Atividade encontrado, dado os filtros</returns>
        public List<Modelo.HistoricoAtividade> ConsultarHistorico(
            Int64? codigoHistorico, 
            Int32? codigoEntidade, 
            Int64? codigoTipoAtividade, 
            DateTime? dataInicio, 
            DateTime? dataFim, 
            String nomeUsuario,
            String emailUsuario,
            String funcionalOperador,
            Boolean? tipoAtividadeVisible)
        {            
            List<Modelo.HistoricoAtividade> historico = new Data.HistoricoAtividade().ConsultarHistorico(
                codigoHistorico, codigoEntidade, codigoTipoAtividade, dataInicio, dataFim, 
                nomeUsuario, emailUsuario, funcionalOperador, tipoAtividadeVisible);

            PrepararDescricaoHistoricoAtividade(ref historico);

            return historico;
        }

        /// <summary>
        /// Grava um registro no histórico de atividades
        /// </summary>
        /// <param name="historico">Dados do registro do histórico de atividade</param>
        /// <returns>Código do registro do histórico de atividade criado</returns>
        public Int64 GravarHistorico(Modelo.HistoricoAtividade historico)
        {
            return new Data.HistoricoAtividade().GravarHistorico(historico);
        }

        /// <summary>
        /// Consulta os tipos de relatórios existentes
        /// </summary>
        /// <param name="ativo">Opcional: filtro de relatório pela flag "ativo"</param>
        /// <returns>Dicionário contendo os relatórios disponíveis</returns>
        public Dictionary<Int32, String> ConsultarTiposRelatorios(Boolean? ativo)
        {
            return new Data.HistoricoAtividade().ConsultarTiposRelatorios(ativo);
        }

        /// <summary>
        /// Consulta de Relatório do Histórico de Atividades
        /// </summary>
        /// <param name="codigoTipoRelatorio">Código do tipo de relatório</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="data">Data da consulta</param>
        /// <returns>Relatório</returns>
        public DataSet ConsultarRelatorio(Int32 codigoTipoRelatorio, DateTime? data, Int32? codigoEntidade)
        {
            return new Data.HistoricoAtividade().ConsultarRelatorio(codigoTipoRelatorio, data, codigoEntidade);
        }

        private static void PrepararDescricaoHistoricoAtividade(ref List<Modelo.HistoricoAtividade> historico)
        {
            List<TipoAtividade> tiposAtividade = new Data.HistoricoAtividade().Consultar(null);
            Dictionary<Int32, String> tipos = tiposAtividade.ToDictionary(tipo => tipo.Codigo, tipo => tipo.Descricao);
            
            if (historico != null && historico.Count > 0)
            {                
                foreach (Modelo.HistoricoAtividade item in historico)
                {
                    var descricao = String.Empty;
                    var valoresTags = new Dictionary<String, String>();                   

                    if(tipos.ContainsKey(item.CodigoTipoAtividade))
                        descricao = tipos[item.CodigoTipoAtividade];                    

                    if (!String.IsNullOrEmpty(item.Detalhes))
                    {
                        valoresTags = XElement.Parse(item.Detalhes).Descendants().ToDictionary(
                            elemento => elemento.Name.LocalName, elemento => elemento.Value);
                    }

                    valoresTags.Add("nome_responsavel", item.NomeUsuario);
                    valoresTags.Add("email_responsavel", item.EmailUsuario);
                    valoresTags.Add("perfil_responsavel", item.PerfilUsuario);

                    if (item.CodigoEntidade.HasValue)
                        valoresTags.Add("pv_responsavel", item.CodigoEntidade.Value.ToString());

                    descricao = Regex.Replace(descricao, @"##(.+?)##",
                        m => valoresTags.ContainsKey(m.Groups[1].Value) ? valoresTags[m.Groups[1].Value] : String.Empty);

                    item.Descricao = descricao;
                }
            }
        }

        /// <summary>
        /// Consulta dos histórico de arquivos enviados
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador> ConsultarHistoricoEnvioArquivos(DateTime inicio, DateTime fim)
        {
            Int32 codigoAtidadeServico = 16;
            String nomeUsuario = "EnvioArquivosConciliador";

            ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador> historicos = default(ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador>);

            List<Modelo.HistoricoAtividade> historicoAtividade = new Data.HistoricoAtividade()
                .ConsultarHistorico(null, 
                null, 
                codigoAtidadeServico, 
                inicio, 
                fim,
                nomeUsuario, 
                null, 
                null, 
                null);

            if (historicoAtividade != null && historicoAtividade.Count > 0)
                historicoAtividade = historicoAtividade.Where(hist => hist.Timestamp >= inicio && hist.Timestamp <= fim).ToList();

            historicos = ExtrairLogsConciliador(historicoAtividade);

            return historicos;
        }

        /// <summary>
        /// Extrair os dados armazenados em XML do Histórico
        /// </summary>
        /// <param name="historicoAtividade"></param>
        /// <returns></returns>
        private ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador> ExtrairLogsConciliador(List<Modelo.HistoricoAtividade> historicoAtividade)
        {
            var lista = new ListaHistoricoConciliadorResponse<HistoricoEnvioArquivoConciliador>();
            XElement xmlHistorico = default(XElement);

            HistoricoEnvioArquivoConciliador historico = default(HistoricoEnvioArquivoConciliador);

            if (historicoAtividade != null && historicoAtividade.Count > 0)
            {
                lista.Historicos = new List<HistoricoEnvioArquivoConciliador>();

                foreach (Modelo.HistoricoAtividade item in historicoAtividade)
                {
                    if (!String.IsNullOrEmpty(item.Detalhes))
                    {
                        xmlHistorico = XElement.Parse(item.Detalhes);

                        historico = new HistoricoEnvioArquivoConciliador();

                        historico.CodigoErro = Convert.ToInt32(xmlHistorico.Element("CodigoErro").Value);
                        historico.DataHoraEntrada = xmlHistorico.Element("DataHoraEntrada").Value;
                        historico.DataHoraSaida = xmlHistorico.Element("DataHoraSaida").Value;
                        historico.Guid = item.Codigo;
                        historico.MensagemErro = xmlHistorico.Element("MensagemErro").Value;
                        historico.NomeArquivo = xmlHistorico.Element("NomeArquivo").Value;
                        historico.Status = Convert.ToInt16(xmlHistorico.Element("Status").Value);

                        lista.Historicos.Add(historico);
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public ListaHistoricoConciliadorResponse<HistoricoConciliador> ConsultarHistoricoContratacaoCancelamento(DateTime dataInicio, DateTime dataFim)
        {
            Int32 codigoAtidadeServico = 16;
            String nomeUsuario = "ContratacaoCancelamentoConciliador";

            ListaHistoricoConciliadorResponse<HistoricoConciliador> historicos = default(ListaHistoricoConciliadorResponse<HistoricoConciliador>);

            List<Modelo.HistoricoAtividade> historicoAtividade = new Data.HistoricoAtividade()
                .ConsultarHistorico(null,
                null,
                codigoAtidadeServico,
                dataInicio,
                dataFim,
                nomeUsuario,
                null,
                null,
                null);

            if (historicoAtividade != null && historicoAtividade.Count > 0)
                historicoAtividade = historicoAtividade.Where(hist => hist.Timestamp >= dataInicio && hist.Timestamp <= dataFim).ToList();

            historicos = ExtrairLogsConciliadorContratacao(historicoAtividade);

            return historicos;
        }

        /// <summary>
        /// Extrair os dados armazenados em XML do Histórico
        /// </summary>
        /// <param name="historicoAtividade"></param>
        /// <returns></returns>
        private ListaHistoricoConciliadorResponse<HistoricoConciliador> ExtrairLogsConciliadorContratacao(List<Modelo.HistoricoAtividade> historicoAtividade)
        {
            var lista = new ListaHistoricoConciliadorResponse<HistoricoConciliador>();
            XElement xmlHistorico = default(XElement);

            HistoricoConciliador historico = default(HistoricoConciliador);

            if (historicoAtividade != null && historicoAtividade.Count > 0)
            {
                lista.Historicos = new List<HistoricoConciliador>();

                foreach (Modelo.HistoricoAtividade item in historicoAtividade)
                {
                    if (!String.IsNullOrEmpty(item.Detalhes))
                    {
                        xmlHistorico = XElement.Parse(item.Detalhes);

                        historico = new HistoricoConciliador();

                        historico.CodigoErro = Convert.ToInt32(xmlHistorico.Element("CodigoErro").Value);
                        historico.DataHoraEntrada = xmlHistorico.Element("DataHoraEntrada").Value;
                        historico.DataHoraSaida = xmlHistorico.Element("DataHoraSaida").Value;
                        historico.Guid = item.Codigo;
                        historico.MensagemErro = xmlHistorico.Element("MensagemErro").Value;
                        historico.Login = xmlHistorico.Element("Login").Value;
                        historico.NumeroEstabelecimento = Convert.ToInt64(xmlHistorico.Element("NumeroEstabelecimento").Value);
                        historico.Status = Convert.ToInt16(xmlHistorico.Element("Status").Value);
                        historico.Etapa = Convert.ToInt16(xmlHistorico.Element("Etapa").Value);
                        historico.Servidor = xmlHistorico.Element("Servidor").Value;

                        lista.Historicos.Add(historico);
                    }
                }
            }

            return lista;
        }
    }
}