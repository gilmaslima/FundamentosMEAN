using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Collections.Specialized;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// 
    /// </summary>
    [ConfigurationElementType(typeof(CustomLogFilterData))]
    public class TimeFilter : Microsoft.Practices.EnterpriseLibrary.Logging.Filters.ILogFilter
    {
        private string horaInicio;
        private string horaFim;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        public TimeFilter(NameValueCollection attributes)
        {
            horaInicio = attributes.Get("HoraInicio");
            horaFim = attributes.Get("HoraFim");
        }
        /// <summary>
        /// Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool Filter(Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry log)
        {
            DateTime horaAtual = DateTime.Now;
            return !(TimeSpan.Compare(Convert.ToDateTime(horaInicio).TimeOfDay, horaAtual.TimeOfDay) == -1
                && TimeSpan.Compare(horaAtual.TimeOfDay, Convert.ToDateTime(horaFim).TimeOfDay) == -1);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return "Time Filter"; }
        }
    }
}
