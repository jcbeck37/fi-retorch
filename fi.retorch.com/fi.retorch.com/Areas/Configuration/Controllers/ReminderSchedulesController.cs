using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Configuration.Code.Base;
using fi.retorch.com.Areas.Configuration.Models;

namespace fi.retorch.com.Areas.Configuration.Controllers
{
    public class ReminderSchedulesController : BaseController
    {
        // GET: Configuration/ReminderSchedules
        public ActionResult Index()
        {
            ReminderScheduleList list = new ReminderScheduleList(db);

            return View(list.Items);
        }

        // GET: Configuration/ReminderSchedules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Configuration/ReminderSchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsActive,DateCreated")] ReminderScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                ReminderScheduleModel.Save(model, db, true);
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Configuration/ReminderSchedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ReminderScheduleModel model = ReminderScheduleModel.Get(db, id.Value);

            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Configuration/ReminderSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsActive,DateCreated")] ReminderScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                ReminderScheduleModel.Save(model, db, false);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Configuration/ReminderSchedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ReminderScheduleModel model = ReminderScheduleModel.Get(db, id.Value);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Configuration/ReminderSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReminderScheduleModel.Delete(db, id);

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
