using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileConverterWebApi.Controllers;
using FileConverterWebApi.Utils;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace FileConverterWebApiTest
{
    public class FileControllerTest
    {
        private XmlToJsonController _controller;
        private Mock<IFileConverter> _fileConverterMock;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _fileConverterMock = new Mock<IFileConverter>();
            _fileConverterMock
                .Setup(x => x.Convert(It.IsAny<IFormFile>(), It.IsAny<FileFormat>(), It.IsAny<FileFormat>()))
                .Returns("Done");
            _loggerMock = new Mock<ILogger>();
            _loggerMock.Setup(x => x.Log(It.IsAny<string>()));

            _controller = new XmlToJsonController(_fileConverterMock.Object, _loggerMock.Object);
        }

        [Test]
        public void JsonToXml_OK()
        {
            _controller.ControllerContext = ContextWithFile();

            var result = _controller.ConvertJsonToXml();
            result.Should().BeOfType(typeof(FileStreamResult));
        }
        
        [Test]
        public void XmlToJson_OK()
        {
            _controller.ControllerContext = ContextWithFile();

            var result = _controller.ConvertXmlToJson();
            result.Should().BeOfType(typeof(FileStreamResult));
        }
        
        [Test]
        public void JsonToXml_NoFileProvided()
        {
            _controller.ControllerContext = ContextWithoutFile();

            var result = _controller.ConvertJsonToXml();
            result.Should().BeOfType(typeof(BadRequestResult));
        }
        
        [Test]
        public void XmlToJson_NoFileProvided()
        {
            _controller.ControllerContext = ContextWithoutFile();

            var result = _controller.ConvertXmlToJson();
            result.Should().BeOfType(typeof(BadRequestResult));
        }
        
        [Test]
        public void JsonToXml_Exception()
        {
            _controller.ControllerContext = ContextWithFile();
            _fileConverterMock
                .Setup(x => x.Convert(It.IsAny<IFormFile>(), It.IsAny<FileFormat>(), It.IsAny<FileFormat>()))
                .Throws(new Exception());
            
            var result = _controller.ConvertJsonToXml();
            result.Should().BeOfType(typeof(StatusCodeResult));
            (result as StatusCodeResult).Should().BeEquivalentTo(new StatusCodeResult(500));
            _loggerMock.Invocations.Count.Should().Be(1);
        }
        
        [Test]
        public void XmlToJson_Exception()
        {
            _fileConverterMock
                .Setup(x => x.Convert(It.IsAny<IFormFile>(), It.IsAny<FileFormat>(), It.IsAny<FileFormat>()))
                .Throws(new Exception());
            _controller.ControllerContext = ContextWithFile();

            var result = _controller.ConvertXmlToJson();
            result.Should().BeOfType(typeof(StatusCodeResult));
            (result as StatusCodeResult).Should().BeEquivalentTo(new StatusCodeResult(500));
            _loggerMock.Invocations.Count.Should().Be(1);
        }

        ControllerContext ContextWithFile()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(),
                new FormFileCollection
                {
                    new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Mock file content")), 0, 0, "Data",
                        "test.any")
                });
            return new ControllerContext(new ActionContext(httpContext, new RouteData(),
                new ControllerActionDescriptor()));
        }
        
        ControllerContext ContextWithoutFile()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(),
                new FormFileCollection());
            return new ControllerContext(new ActionContext(httpContext, new RouteData(),
                new ControllerActionDescriptor()));
        }
    }
}