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
    public class TermFilter : Microsoft.Practices.EnterpriseLibrary.Logging.Filters.ILogFilter
    {
        private string term = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        public TermFilter(NameValueCollection attributes)
        {
            term = attributes.Get("ExcluirTermo");
        }
        /// <summary>
        /// Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool Filter(Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry log)
        {
            //está trazendo Redecard.PN.Comum.Logger.GravarLog
            return !log.Categories.Contains(term);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return "Term Filter"; }
        }
    }
}
