using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Google.Protobuf;

namespace BasicSerialiseDeserialise
{
    public class ProtoTests
    {
        [Fact]
        public void SimpleSerialiseDeserialise()
        {
            byte[] data;
            var person = new Person()
            {
                Email = "jimmeh@gmail.com",
                Id = 2,
                Name = "jimmeh"
            };

            var phoneNumber = new Person.Types.PhoneNumber() { Number = "123645", Type = Person.Types.PhoneType.Mobile };
            person.Phone.Add(phoneNumber);

            using (MemoryStream stream = new MemoryStream())
            {
                // Save the person to a stream
                person.WriteTo(stream);
                data = stream.ToArray();
            }      

            var actual = Person.Parser.ParseFrom(data);

            actual.Email.Should().Be(person.Email);
            actual.Id.Should().Be(person.Id);
            actual.Name.Should().Be(person.Name);
            actual.Phone.Count.Should().Be(person.Phone.Count);
            actual.Phone[0].Should().Be(person.Phone[0]);
        }
    }
}
