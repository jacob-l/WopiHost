using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using WopiHost.Dto;

namespace WopiHost.Controllers.Api
{
    [RoutePrefix("wopi")]
    public class WopiController : ApiController
    {
        public const string FILES_FOLDER = "~/App_Data";

        public static string FilesFolder
        {
            get
            {
                return HttpContext.Current.Server.MapPath(FILES_FOLDER);
            }
        }

        private bool IsValidToken(Guid tokenId)
        {
            return true;
        }

        private void Validation(Guid tokenId, string fullFileName)
        {
            if (!IsValidToken(tokenId))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (!File.Exists(fullFileName))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        private static string GetChecksum(string filePath)
        {
            using (var stream = new BufferedStream(File.OpenRead(filePath), 1200000))
            {
                var checksum = SHA256.Create().ComputeHash(stream);
                return Convert.ToBase64String(checksum);
            }
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(FilesFolder, fileName);
        }

        [Route("files/{fileName}")]
        [HttpGet]
        public FileInfoDto CheckFileInfo(string fileName, [FromUri(Name = "access_token")] Guid tokenId)
        {
            var fullFileName = GetFullPath(fileName);
            Validation(tokenId, fullFileName);

            return new FileInfoDto
            {
                BaseFileName = fileName,
                OwnerId = "admin",
                ReadOnly = true,
                SHA256 = GetChecksum(fullFileName),
                Size = new FileInfo(fullFileName).Length,
                Version = 1
            };
        }

        [Route("files/{fileName}/contents")]
        [HttpGet]
        public HttpResponseMessage GetFile(string fileName, [FromUri(Name = "access_token")] Guid tokenId)
        {
            var fullFileName = GetFullPath(fileName);
            Validation(tokenId, fullFileName);

            var stream = new FileStream(fullFileName, FileMode.Open);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return result;
        }
    }
}