using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FileConverterWebApi.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace FileConverterWebApiTest
{
    public class FileConverterTest
    {
        const string TestXml =
            "<?xmlversion=\"1.0\"encoding=\"utf-16\"?><Documentxmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Title>TestTitle</Title><Text>TestText</Text></Document>";

        const string TestJson = "{\"Title\":\"TestTitle\",\"Text\":\"TestText\"}";

        readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        FileConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new FileConverter(_loggerMock.Object);
        }

        [Test]
        public void JsonToXml()
        {
            var fileMock = new Mock<IFormFile>();

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(TestJson));
            fileMock.Setup(m => m.OpenReadStream()).Returns(ms);

            var output = _converter.Convert(fileMock.Object, FileFormat.Json, FileFormat.Xml);
            Regex.Replace(output, @"\s+", "").Should().Be(TestXml);
        }

        [Test]
        public void XmlToJson()
        {
            var fileMock = new Mock<IFormFile>();

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(
                "<?xml version=\"1.0\"?><Document><Title>TestTitle</Title><Text>TestText</Text></Document>"));
            fileMock.Setup(m => m.OpenReadStream()).Returns(ms);

            var output = _converter.Convert(fileMock.Object, FileFormat.Xml, FileFormat.Json);
            output.Should().Be(TestJson);
        }

        [Test]
        public void XmlToXml_Error()
        {
            var fileMock = new Mock<IFormFile>();
            var output = _converter.Convert(fileMock.Object, FileFormat.Xml, FileFormat.Xml);
            output.Should().Be("");

            _loggerMock.Invocations.Count.Should().Be(1);
        }
    }
}