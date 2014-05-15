using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace WopiHost.Controllers
{
    public class Owa
    {
        private const string VIEW = "view";

        public const string DISCOVERY_PATH = "~/App_Data/Discovery.xml";

        private readonly XElement discoveryRoot;

        public Owa()
            : this(HttpContext.Current.Server.MapPath(DISCOVERY_PATH))
        {
        }

        public Owa(string discovery)
        {
            var xml = XDocument.Load(discovery);

            discoveryRoot = xml.Element("wopi-discovery");
            if (discoveryRoot == null)
            {
                throw new ApplicationException("Bad wopi discovery file");
            }
        }

        private IEnumerable<XElement> GetActions(string action)
        {
            return discoveryRoot.Elements("net-zone").
                Single(nz => nz.Attribute("name").Value == "external-http").Elements("app").
                SelectMany(el => el.Elements("action").Where(a => a.Attribute("name").Value == action));
        }

        private string GetUrl(string extension, string method)
        {
            extension = extension.ToLower();

            var act = GetActions(method).
                FirstOrDefault(el => el.Attribute("ext") != null && el.Attribute("ext").Value == extension);

            return act == null ?
                null :
                Regex.Replace(act.Attribute("urlsrc").Value, "<.*>", "");
        }

        public string GetViewUrl(string extension)
        {
            return GetUrl(extension.TrimStart('.'), VIEW);
        }
    }
}