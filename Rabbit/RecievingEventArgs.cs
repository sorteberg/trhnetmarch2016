using System;

namespace Rabbit
{
    public class RecievingEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}