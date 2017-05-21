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
        private readonly TumblrDbContext db;

        public ViewerController(TumblrDbContext db)
        {
            this.db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var list = db.MediaSet
                      .Include(i => i.Media)
                      .Include(i => i.Posts)
                      .OrderBy(o1 => o1.Posts.OrderBy(o => o.ID).First().ID).Take(10); 
            
            return View(list);
        }
    }
}
