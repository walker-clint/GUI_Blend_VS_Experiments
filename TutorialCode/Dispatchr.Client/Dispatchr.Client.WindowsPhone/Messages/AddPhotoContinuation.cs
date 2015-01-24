using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Activation;

namespace Dispatchr.Client.Messages
{
    public class AddPhotoContinuation : PubSubEvent<FileOpenPickerContinuationEventArgs> { }
}
