using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Models;
using System.Net;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class BookmarksController : BaseController
    {
        // GET: Dashboard/Bookmarks
        public ActionResult Index()
        {
            // optional parameter isActive defaults to null, including both active/inactive
            BookmarkList list = new BookmarkList(db, userKey);

            return View(list.Items);
        }

        // GET: Dashboard/Bookmarks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Bookmarks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "URL,Text,IsActive")] BookmarkModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save(model, db, userKey, true);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Dashboard/Bookmarks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            BookmarkModel model = new BookmarkModel(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: Dashboard/Bookmarks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,URL,Text,IsActive")] BookmarkModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save(model, db, userKey, false);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Dashboard/Bookmarks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            BookmarkModel model = new BookmarkModel(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: Dashboard/Bookmarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookmarkModel model = new BookmarkModel(db, userKey, id);

            if (model == null)
                return HttpNotFound();

            model.Delete(db, userKey, id);

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
