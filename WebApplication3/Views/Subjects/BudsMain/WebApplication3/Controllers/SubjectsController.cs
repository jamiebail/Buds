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
    public class SubjectsController : Controller
    {

        public List<Subjects> allSubs = new List<Subjects>();
        private ApplicationDbContext db = new ApplicationDbContext();
        public IEnumerable<Subjects> searchedSubs = new List<Subjects>();
        public void assignNotes()
        {
            foreach (Subjects s in db.Subjects)
            {
                s.subjectNotes = new List<notes>();
                foreach (notes n in db.Notes)
                {
                    if (n.SubjectID == s.ID)
                    {
                        s.subjectNotes.Add(n);
                    }
                }
                s.subjectNotes = s.subjectNotes.Distinct().ToList();
                s.subjectNotes.Reverse();
                allSubs.Add(s);

            }
        }
        // GET: Subjects
        public ActionResult Index(string search)
        {
            if (search != string.Empty && search != null)
            {
                ViewBag.Search = search;
                ViewBag.searched = true;
                assignNotes();
                List<Subjects> subs = (allSubs.Where(x => x.Title.ToLower().Contains(search.ToLower()))).ToList();
                List <Subjects> toAdd = new List<Subjects>();
                foreach(Subjects s in db.Subjects.ToList())
                {
                    search = char.ToUpper(search[0]) + search.Substring(1);
                    s.Title = Regex.Replace(s.Title, search, "<span class=searchString>$&</span>", RegexOptions.IgnoreCase);
                    foreach (notes n in s.subjectNotes)
                    {
                        if (n.Body.ToLower().Contains(search.ToLower()) || n.Title.ToLower().Contains(search.ToLower()))
                        {
                            n.Title = Regex.Replace(n.Title, search, "<span class=searchString>$&</span>", RegexOptions.IgnoreCase);
                            search = char.ToLower(search[0]) + search.Substring(1);
                            n.Body = Regex.Replace(n.Body, search, "<span class=searchString>$&</span>", RegexOptions.IgnoreCase);
                            toAdd.Add(s);
                        }
                    }
                }
                subs.AddRange(toAdd);
                subs = subs.Distinct().ToList();
                return View(subs);
            }
            else
            {
                try
                {
                    ViewBag.searched = false;
                    assignNotes();
                    allSubs.Reverse();
                    return View(allSubs);
                }
                catch
                {
                    return View();
                }
            }
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subjects subjects = db.Subjects.Find(id);
            if (subjects == null)
            {
                return HttpNotFound();
            }
            return View(subjects);
        }

     


        [HttpPost]
        public ActionResult Search(string searchIn)
        {
            searchedSubs = db.Subjects.Where(x => x.Title.ToLower().Contains(searchIn.ToLower())).ToList();
            return RedirectToAction("Index", searchedSubs);
        }
        // GET: Subjects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Subjects subjects)
        {
            if (ModelState.IsValid)
            {
                subjects.postDate = DateTime.Now;
                db.Subjects.Add(subjects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subjects);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subjects subjects = db.Subjects.Find(id);
            if (subjects == null)
            {
                return HttpNotFound();
            }
            return View(subjects);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,postDate")] Subjects subjects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subjects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subjects);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subjects subjects = db.Subjects.Find(id);
            if (subjects == null)
            {
                return HttpNotFound();
            }
            return View(subjects);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            assignNotes();
            Subjects subjects = allSubs.Where(x => x.ID == id).FirstOrDefault();
            List<notes> deleteNotes= subjects.subjectNotes;
            db.Notes.RemoveRange(deleteNotes);
            db.Subjects.Remove(subjects);
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
