using Rede.PN.AtendimentoDigital.Core.Log;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Log
{
	/// <summary>
	/// Classe concreta que implementa a interface <see cref="IServicoLog"/> para gravar log utilizando o EnterpriseLibrary.
	/// </summary>
	public sealed class ServicoLogEnterpriseLibrary : IServicoLog
	{
		/// <summary>
		/// Grava um registro de log.
		/// </summary>
		/// <param name="idEvento">Valor de identificação do evento de log</param>
		/// <param name="categoria">Especifica a categoria de log a ser utilizada</param>
		/// <param name="prioridade">Grau de importância da mensagem de log</param>
		/// <param name="titulo">Título do log</param>
		/// <param name="mensagem">Mensagem a ser gravada no log</param>
		/// <param name="severidade">Gravidade do log</param>
		/// <param name="propriedades"></param>
		public void IncluirLog(Int32 idEvento, String categoria, Int32 prioridade, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			List<String> categorias = new List<String> { categoria };

			LogEntry logEntry = new LogEntry()
			{
				EventId = idEvento,
				Priority = prioridade,
				Message = mensagem,
				Severity = severidade,
				Categories = categorias,
				Title = titulo,
				ExtendedProperties = propriedades,
				TimeStamp = DateTime.Now
			};

           
                //Logger.Write(logEntry);
                //Logger.Writer.Dispose();
            
		}

		/// <summary>
		/// Grava um registro de auditoria.
		/// </summary>
		/// <param name="classe">Nome da classe que será auditada</param>
		/// <param name="metodo">Nome do método que será auditado</param>
		/// <param name="parametros">Parâmetros que serão auditados</param>
		/// <param name="retorno">Retorno do método auditado</param>
		/// <param name="categoria">Especifica a categoria de registro de auditoria a ser utilizada</param>
		/// <param name="titulo">Título do registro de auditoria</param>
		/// <param name="mensagem">Mensagem a ser gravada no registro de auditoria</param>
		/// <param name="severidade">Gravidade do registro de auditoria</param>
		/// <param name="propriedades"></param>
		public void IncluirAuditoria(String classe, String metodo, String parametros, Object retorno, String categoria, String titulo, String mensagem, TraceEventType severidade, IDictionary<String, Object> propriedades)
		{
			List<String> categorias = new List<String> { categoria };
			string entryMessage = string.Format("{0} {1} {2} {3} {4}",
				classe,
				metodo,
				titulo,
				mensagem,
				parametros
			);

			LogEntry logEntry = new LogEntry()
			{
				EventId = 999,
				Priority = 99,
				Message = entryMessage,
				Severity = severidade,
				Categories = categorias,
				Title = titulo,
				ExtendedProperties = propriedades,
				TimeStamp = DateTime.Now
			};

            //Logger.Write(logEntry);
            //Logger.Writer.Dispose();
		}
	}
}
