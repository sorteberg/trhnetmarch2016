using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rabbit.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string _recievedMessage;

        [TestMethod]
        public void TestMethod1()
        {
            const string Message = "hallo trh.net";
            MessageSender.Send(Message);

            //var reciever = new MessageReciever();
            //reciever.Recieved += (sender, args) => _recievedMessage = args.Data;
            //reciever.StartRecieving();
            //_recievedMessage.Should().Be(Message);
            //Console.Read();
        }
    }
}