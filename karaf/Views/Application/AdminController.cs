using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace karaf.Views.Application
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult BundleLogs()
        {
            return View();
        }
        public ActionResult Features()
        {
            return View();
        }
    }
}
