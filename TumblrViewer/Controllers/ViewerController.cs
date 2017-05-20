using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TumblrTruck.DB;
using Microsoft.EntityFrameworkCore;

namespace TumblrViewer.Controllers
{
    public class ViewerController : Controller
    {
        private readonly TumblrDbContext context;

        public ViewerController(TumblrDbContext context)
        {
            this.context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var list = context.MediaSet
                              .Include(i => i.Media)
                              .Include(i => i.Posts)
                              .OrderBy(o => o.Key).Take(10);
            
            return View(list);
        }
    }
}
