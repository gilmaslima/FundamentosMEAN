using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

public class MyMessageInspector : IClientMessageInspector
{
    public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
        // log response xml
        File.WriteAllText(@"c:\temp\responseXml.xml", reply.ToString());
    }

    public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
    {
        // log request xml
        File.WriteAllText(@"c:\temp\requestXml.xml", request.ToString());
        return null;
    }
}