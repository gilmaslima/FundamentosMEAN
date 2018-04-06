using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.Extrato.SharePoint.WebParts.Relatorios {

    public class SearchObject {

        public DateTime inicio { get; set; }

        public DateTime fim { get; set; }

        public Int32 tipoRelatorio { get; set; }

        public Int32 tipoVenda { get; set; }

        public Int32 tipoEstabelecimento { get; set; }

        public Int32[] estabelecimento { get; set; }

        public static SearchObject Deserialize(string json) {
            SearchObject obj = Activator.CreateInstance<SearchObject>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(),
                new DataContractJsonSerializerSettings() {
                    DateTimeFormat = new DateTimeFormat("yyyy-MM-dd")
                });
            obj = (SearchObject)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
