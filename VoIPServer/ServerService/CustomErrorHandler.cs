using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace ServerServiceLibrary
{
    public class CustomErrorHandler : IErrorHandler
    {
        private const int SOCKET_CLOSE_EXCEPTION = -2146233087;

        public bool HandleError(Exception error)
        {
            if(error.HResult != SOCKET_CLOSE_EXCEPTION)
            {
                StackTrace trace = new StackTrace(error, true);
                StackFrame stackFrame = trace.GetFrame(trace.FrameCount - 1);
                int lineNumber = stackFrame.GetFileLineNumber();
                string fileName = stackFrame.GetFileName();

                Console.WriteLine("caught exception in file {0} at line {1} : \n{2}", lineNumber, fileName, error.Message);
            }

            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
        {

        }
    }

    public class ErrorHandlerExtension : Attribute, IServiceBehavior
    {
        private IErrorHandler GetInstance()
        {
            return new CustomErrorHandler();
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandlerInstance = GetInstance();
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(errorHandlerInstance);
            }
        }
    }
}
