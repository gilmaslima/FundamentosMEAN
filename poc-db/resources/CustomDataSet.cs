using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos
{
    [CollectionDataContract]
    public class CustomDataSet : List<CustomDataTable> { }

    [DataContract]
    public class CustomDataTable
    {
        [DataMember]
        public List<String> Columns { get; set; }

        [DataMember]
        public List<CustomDataRow> Rows { get; set; }
    }

    [CollectionDataContract]
    public class CustomDataRow : List<String> { }
}