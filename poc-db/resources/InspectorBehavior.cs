using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public class InspectorBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        throw new NotImplementedException();
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.MessageInspectors.Add(new MyMessageInspector());
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
        throw new NotImplementedException();
    }

    public void Validate(ServiceEndpoint endpoint)
    {
        throw new NotImplementedException();
    }
}