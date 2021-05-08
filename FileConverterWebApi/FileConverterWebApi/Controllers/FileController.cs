using System;
using System.IO;
using System.Text;
using FileConverterWebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FileConverterWebApi.Controllers
{
    [Route("api/file/")]
    public class XmlToJsonController : Controller
    {
        readonly IFileConverter _converter;
        readonly ILogger _logger;

        public XmlToJsonController(IFileConverter xmlToJsonConverter, ILogger logger)
        {
            _converter = xmlToJsonConverter;
            _logger = logger;
        }

        [HttpPost("convert/xmlToJson")]
        public IActionResult ConvertXmlToJson()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    Console.WriteLine(file.FileName);
                    var serializedFile = _converter.Convert(file, FileFormat.Xml, FileFormat.Json);
                    return new FileStreamResult(new MemoryStream((Encoding.UTF8).GetBytes(serializedFile)),
                        "application/octet-stream");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("convert/jsonToXml")]
        public IActionResult ConvertJsonToXml()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var serializedFile = _converter.Convert(file, FileFormat.Json, FileFormat.Xml);
                    return new FileStreamResult(new MemoryStream((Encoding.UTF8).GetBytes(serializedFile)),
                        "application/octet-stream");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("saveFile")]
        public IActionResult SaveFile()
        {
            try
            {
                if (Request.Headers.TryGetValue("x-file-path", out var outPath) && Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    using var stream = new FileStream(Path.Combine(outPath, file.FileName), FileMode.Create);
                    Request.Form.Files[0].CopyTo(stream);
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("loadFile")]
        public IActionResult LoadFile()
        {
            try
            {
                if (Request.Headers.TryGetValue("x-file-path", out var filePath))
                {
                    if (!System.IO.File.Exists(filePath))
                        return NoContent();

                    return new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read),
                        "application/octet-stream");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
                return StatusCode(500);
            }
        }
    }
}