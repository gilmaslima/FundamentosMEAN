using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Linq.Expressions;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Redecard.PN.Comum.TraceListeners
{
    /// <summary>
    /// Configuration object for a <see cref="CustomDatabaseTraceListener"/>.
    /// </summary>    
    public class CustomDatabaseTraceListenerData : TraceListenerData
    {        
        /// <summary>
        /// Initializes a <see cref="CustomDatabaseTraceListenerData"/>.
        /// </summary>
        public CustomDatabaseTraceListenerData()
            : base(typeof(CustomDatabaseTraceListener))
        {
            this.ListenerDataType = typeof(CustomDatabaseTraceListenerData);
        }

        /// <summary>
        /// Initializes a named instance of <see cref="CustomDatabaseTraceListenerData"/> with 
        /// name, stored procedure name, databse instance name, and formatter name.
        /// </summary>
        /// <param name="name">The name.</param>               
        public CustomDatabaseTraceListenerData(string name)
            : this(name, TraceOptions.None, SourceLevels.All)
        {
        }

        /// <summary>
        /// Initializes a named instance of <see cref="CustomDatabaseTraceListenerData"/> with 
        /// name, stored procedure name, databse instance name, and formatter name.
        /// </summary>
        /// <param name="name">The name.</param>        
        /// <param name="traceOutputOptions">The trace options.</param>
        /// <param name="filter">The filter to be applied</param>
        public CustomDatabaseTraceListenerData(string name,                                                  
                                                TraceOptions traceOutputOptions,
                                                SourceLevels filter)
            : base(name, typeof(CustomDatabaseTraceListener), traceOutputOptions, filter)
        {            
        }
                        
        /// <summary>
        /// Returns a lambda expression that represents the creation of the trace listener described by this
        /// configuration object.
        /// </summary>
        /// <returns>A lambda expression to create a trace listener.</returns>
        protected override Expression<Func<TraceListener>> GetCreationExpression()
        {
            return () => new CustomDatabaseTraceListener();
        }
    }    
}
