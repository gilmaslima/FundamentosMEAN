using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Comum;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections.Concurrent;
using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    public class ServicoBaseExtrato : ServicoBase
    {
        /// <summary>
        /// Constantes com o nome único para uso em cache de Extratos Antecipados
        /// </summary>
        public const string CACHEANTECIPADOS = "HISServicoWAExtratoAntecipados";
        /// <summary>
        /// Constantes com o nome único para uso em cache de Extratos de Lançamentos Futuros
        /// </summary>
        public const string CACHELANCAMENTOSFUTUROS = "HISServicoWAExtratoLancamentosFuturos";
        /// <summary>
        /// Constantes com o nome único para uso em cache de Extratos de Valores Pagos
        /// </summary>
        public const string CACHEVALORESPAGOS = "HISServicoWAExtratoValoresPagos";
        /// <summary>
        /// Constantes com o nome único para uso em cache de Extratos de Vendas
        /// </summary>
        public const string CACHEVENDAS = "HISServicoWAExtratoVendas";

        /// <summary>
        /// Método comum para consulta Multi-Threaded (MT) de Relatórios, 
        /// com busca paralela de funções de totalizador e registros.</summary>        
        /// <param name="funcaoTotalizador">Função responsável pela consulta dos Totalizadores</param>
        /// <param name="funcaoRegistros">Função responsável pela consulta dos Registros</param>        
        protected Tuple<TTotalizador, List<TRegistro>> ConsultaMultiThread<TTotalizador, TRegistro>(
            Func<TTotalizador> funcaoTotalizador,
            Func<List<TRegistro>> funcaoRegistros)
            where TTotalizador : BasicContract
            where TRegistro : BasicContract
        {
            //Coleção thread-safe auxiliar para armazenar as exceções geradas 
            ConcurrentQueue<Exception> _excecoes = new ConcurrentQueue<Exception>();
            
            //Preparação da tarefa (Task) responsável pela consulta dos totalizadores
            var _taskTotalizador = Task.Factory.StartNew<TTotalizador>(() =>
            {
                try { return funcaoTotalizador(); }
                catch (Exception ex) { _excecoes.Enqueue(ex); return null; }
            });

            //Preparação da tarefa (Task) responsável pela consulta dos registros
            var _taskRegistros = Task.Factory.StartNew<List<TRegistro>>(() =>
            {
                try { return funcaoRegistros(); }
                catch (Exception ex) { _excecoes.Enqueue(ex); return null; }
            });

            //Aguarda término das duas tarefas (totalizadores e registros)
            Task.WaitAll(_taskTotalizador, _taskRegistros);

            //Caso tenha ocorrido alguma exceção, lança a primeira exceção gerada
            if (_excecoes.Count > 0)
            {
                var _excecao = _excecoes.First();
                if (_excecao is FaultException<GeneralFault>)
                    throw _excecao;
                else if (_excecao is PortalRedecardException)
                {
                    var _ex = _excecao as PortalRedecardException;
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(_ex.Codigo, _ex.Fonte), base.RecuperarExcecao(_ex));
                }                
                else
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(_excecao));
            }

            //Monta Tuple de retorno contendo o resultado da execução das funções
            return new Tuple<TTotalizador,List<TRegistro>>(_taskTotalizador.Result, _taskRegistros.Result);
        }

        /// <summary>
        /// Método comum para consulta Multi-Threaded (MT) de Relatórios, 
        /// com busca paralela de funções de totalizador e registros, para retorno REST.</summary>        
        /// <param name="funcaoTotalizador">Função responsável pela consulta dos Totalizadores</param>
        /// <param name="funcaoRegistros">Função responsável pela consulta dos Registros</param>      
        protected Tuple<TTotalizador, TRegistro> ConsultaMultiThreadRest<TTotalizador, TRegistro>(
            Func<TTotalizador> funcaoTotalizador,
            Func<TRegistro> funcaoRegistros)
            where TTotalizador : ResponseBase
            where TRegistro : ResponseBase
        {
            //Coleção thread-safe auxiliar para armazenar as exceções geradas 
            ConcurrentQueue<Exception> excecoes = new ConcurrentQueue<Exception>();

            //Preparação da tarefa (Task) responsável pela consulta dos totalizadores
            var taskTotalizador = Task.Factory.StartNew<TTotalizador>(() =>
            {
                try { return funcaoTotalizador(); }
                catch (TimeoutException ex) { excecoes.Enqueue(ex); return null; }
                catch (Exception ex) { excecoes.Enqueue(ex); return null; }
            });

            //Preparação da tarefa (Task) responsável pela consulta dos registros
            var taskRegistros = Task.Factory.StartNew<TRegistro>(() =>
            {
                try { return funcaoRegistros(); }
                catch (TimeoutException ex) { excecoes.Enqueue(ex); return null; }
                catch (Exception ex) { excecoes.Enqueue(ex); return null; }
            });

            //Aguarda término das duas tarefas (totalizadores e registros)
            Task.WaitAll(taskTotalizador, taskRegistros);

            //Caso tenha ocorrido alguma exceção, lança a primeira exceção gerada
            if (excecoes.Count > 0)
            {
                var excecao = excecoes.First();
                if (excecao is FaultException<GeneralFault>)
                    throw excecao;
                else if (excecao is PortalRedecardException)
                {
                    var ex = excecao as PortalRedecardException;
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                else
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(excecao));
            }

            //Monta Tuple de retorno contendo o resultado da execução das funções
            return new Tuple<TTotalizador, TRegistro>(taskTotalizador.Result, taskRegistros.Result);
        }
    }
}
