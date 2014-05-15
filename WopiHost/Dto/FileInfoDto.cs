
namespace WopiHost.Dto
{
    public class FileInfoDto
    {
        /// <summary>
        /// File name with extension but without path
        /// </summary>
        public string BaseFileName { get; set; }

        public string OwnerId { get; set; }

        public bool ReadOnly { get; set; }

        public string SHA256 { get; set; }

        public long Size { get; set; }

        public int Version { get; set; }
    }
}