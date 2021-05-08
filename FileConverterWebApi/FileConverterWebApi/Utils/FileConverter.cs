using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileConverterWebApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FileConverterWebApi.Utils
{
    public interface IFileConverter
    {
        public string Convert(IFormFile fileToConvert, FileFormat inputFormat, FileFormat outputFormat);
    }

    public enum FileFormat
    {
        Json,
        Xml
    }
    
    public class FileConverter : IFileConverter
    {
        readonly ILogger _logger;
        readonly List<Type> _serializerAcceptedTypes = new List<Type> { typeof(Document) };

        public FileConverter(ILogger logger)
        {
            _logger = logger;
        }

        public string Convert(IFormFile fileToConvert, FileFormat inputFormat, FileFormat outputFormat)
        {
            switch (inputFormat)
            {
                case FileFormat.Xml:
                    return ConvertFromXml(fileToConvert, outputFormat);
                case FileFormat.Json:
                    return ConvertFromJson(fileToConvert, outputFormat);
                default:
                    _logger.Log("Operation not implemented");
                    return "";
            }
        }

        string ConvertFromJson(IFormFile fileToConvert, FileFormat outputFormat)
        {
            switch (outputFormat)
            {
                case FileFormat.Xml:
                    return JsonToXml(fileToConvert);
                default:
                    _logger.Log("Operation not implemented");
                    return "";
            }
        }

        string ConvertFromXml(IFormFile fileToConvert, FileFormat outputFormat)
        {
            switch (outputFormat)
            {
                case FileFormat.Json:
                    return XmlToJson(fileToConvert);
                default:
                    _logger.Log("Operation not implemented");
                    return "";
            }
        }

        string XmlToJson(IFormFile fileToConvert)
        {
            try
            {
                var sourceStream = fileToConvert.OpenReadStream();
                var reader = new StreamReader(sourceStream);
                string input = reader.ReadToEnd();
                var xdoc = XDocument.Parse(input);

                var typeToSerialize = _serializerAcceptedTypes.Find(x => x.Name == xdoc.Root.Name);

                if (typeToSerialize != null)
                {
                    var doc = Activator.CreateInstance(typeToSerialize);
                    foreach (var property in typeToSerialize.GetProperties())
                    {
                        var propertyValueFromXml = xdoc.Root.Element(property.Name).Value;
                        if (propertyValueFromXml != default)
                            property.SetValue(doc, propertyValueFromXml);
                    }
                    var serializedDoc = JsonConvert.SerializeObject(doc);
                    return serializedDoc;
                }

                _logger.Log("Type inside xml is not supported");
                return default; 
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return default;
            }
        }

        string JsonToXml(IFormFile fileToConvert)
        {
            try
            {
                var sourceStream = fileToConvert.OpenReadStream();
                var reader = new StreamReader(sourceStream);
                var input = reader.ReadToEnd();
                var doc = JsonConvert.DeserializeObject(input, typeof(Document));

                XmlSerializer serializer = new XmlSerializer(typeof(Document));
                var writer = new StringWriter();
                serializer.Serialize(writer, doc);
                return writer.ToString();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return default;
            }
        }
    }
}
