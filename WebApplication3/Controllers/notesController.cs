using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class notesController : Controller
    {
        public List<Subjects> allSubs = new List<Subjects>();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: notes
        public ActionResult Index(int? ID)
        {
            if (ID != null)
            {
                Subjects Sub = db.Subjects.Where(x => x.ID == ID).FirstOrDefault();
                ViewBag.name = Sub.Title;
                return View(db.Notes.Where(x => x.SubjectID == ID));
            }  
            else
                return View(db.Notes.ToList());
        }

        // GET: notes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // GET: notes/Create

 
        public ActionResult Create(int? id)
        {
            ViewBag.ID = id;
            ViewBag.subject = db.Subjects.Where(x => x.ID == id).Select(x => x.Title).FirstOrDefault();
            return View();

        }

        // POST: notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(notes notes)
        {

            try
            {
                Subjects sub = new Subjects();
                sub = db.Subjects.Where(x => x.ID == notes.SubjectID).FirstOrDefault();
                sub.subjectNotes = new List<notes>();
                sub.subjectNotes.Add(notes);
                db.Notes.Add(notes);
                notes.datePosted = DateTime.Now;
                db.SaveChanges();
                ViewBag.subjects = new SelectList(db.Subjects.ToList());
                return RedirectToAction("Index", "Subjects");
            }
            catch
            {
                ViewBag.error = "That subject doesn't exist";
                return View();
            }
                
         
          
        }

        // GET: notes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // POST: notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(notes notes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Subjects");
            }
            return View(notes);
        }

        // GET: notes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // POST: notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            notes notes = db.Notes.Find(id);
            db.Notes.Remove(notes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

   
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
