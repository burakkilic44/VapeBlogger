﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VapeBlogger.Data;
using Microsoft.EntityFrameworkCore;
using VapeBlogger.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VapeBlogger.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext context;
        public PostController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext filerContext)
        {
            ViewBag.Categories = context.Categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, Count = c.Posts.Count }).ToList();

            if (filerContext.RouteData.Values["id"] != null)
            {
                ViewBag.ActiveCategory = context.Categories.FirstOrDefault(c => c.Id.ToString() == filerContext.RouteData.Values["id"].ToString());
            }
            base.OnActionExecuting(filerContext);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? id)
        {
            ViewBag.Commentss = context.Comments.Include(s => s.Post).Where(k => k.IsPublished == true && k.PostId == id).ToList();
            ViewBag.Posts = context.Posts.Select(c => new PostsViewModel { Id = c.Id, Photo = c.Photo, Title = c.Title, CreateDate = c.CreateDate, Hits = c.Hits }).OrderByDescending(c => c.Hits).Take(5).ToList();
            var post = context.Posts.Include(i => i.Category).Include(c => c.Comments)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            post.Hits +=1;
            context.SaveChanges();
           

            return View(post);
        }
        
     
        [HttpPost]
        public JsonResult SendComment( string fullName, string article, int postId)
        {
           
            
            Comment c = new Comment();
            c.FullName = fullName;
            c.Article = article;
            c.PostId = postId;
            c.CreateDate = DateTime.Now;
            c.IsPublished = false;
          if(fullName != null && article != null) { 
            context.Add(c);
            context.SaveChanges();
            }
            return Json(true);
           
        }
       
    }
}