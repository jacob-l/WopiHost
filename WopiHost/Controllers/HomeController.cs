using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WopiHost.Controllers.Api;
using WopiHost.Models;

namespace WopiHost.Controllers
{
    public class HomeController : Controller
    {
        private const string WOPI_CLIENT_PARAMS = "WOPISrc={0}&access_token={1}";

        public string GetQuery(string fileName, Guid tokenId)
        {
            var relativeFileUrl = new Uri(Request.Url,
                VirtualPathUtility.ToAbsolute("~/wopi/files/" + fileName));

            var fileUrl = HttpUtility.UrlEncode(relativeFileUrl.AbsoluteUri);
            return string.Format(WOPI_CLIENT_PARAMS, fileUrl, tokenId);
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            var owa = new Owa();
            var d = new DirectoryInfo(WopiController.FilesFolder);
            ViewBag.Files = d.GetFiles().Select(f => new
            {
                f,
                Url = owa.GetViewUrl(f.Extension)
            }).Where(fe => fe.Url != null).Select(fe => new WopiFile
            {
                Name = fe.f.Name,
                Url = fe.Url + GetQuery(fe.f.Name, Guid.NewGuid())
            });

            ViewBag.LocalhostWarning = Request.Url.Host == "localhost" ||
                Request.Url.Host == "127.0.0.1";

            return View();
        }
    }
}