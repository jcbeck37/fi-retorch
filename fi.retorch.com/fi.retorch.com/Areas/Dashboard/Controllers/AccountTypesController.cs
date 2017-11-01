using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Areas.Dashboard.Code.Base;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class AccountTypesController : BaseController
    {
        // GET: Dashboard/AccountTypes
        public ActionResult Index()
        {
            AccountTypeList list = new AccountTypeList(db, userKey);

            return View(list.Items);
        }

        // GET: Dashboard/AccountTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/AccountTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsDebt,PositiveText,NegativeText")] AccountTypeModel model)
        {
            if (ModelState.IsValid)
            {
                AccountTypeModel.Save(model, db, userKey, true);
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Dashboard/AccountTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            AccountTypeModel model = AccountTypeModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Dashboard/AccountTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsDebt,PositiveText,NegativeText")] AccountTypeModel model)
        {
            if (ModelState.IsValid)
            {
                AccountTypeModel.Save(model, db, userKey, false);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Dashboard/AccountTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            AccountTypeModel model = AccountTypeModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Dashboard/AccountTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountTypeModel.Delete(db, userKey, id);

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
