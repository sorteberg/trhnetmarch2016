using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rabbit.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string Message = "hallo trh.net";
            MessageSender.Send(Message);

            var reciever = new MessageReciever();
            var message = reciever.Recieve();
            message.Should().Be(Message);
        }
    }
}