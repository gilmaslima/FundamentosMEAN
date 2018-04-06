using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.HomePage.SharePoint
{
    [ServiceContract]
    public interface IDashboard
    {
        [OperationContract]
        [WebGet(UriTemplate = "dashboard", 
            ResponseFormat=WebMessageFormat.Json, 
            BodyStyle=WebMessageBodyStyle.Bare)]
        Task<Dashboard> Summary();
    }
}