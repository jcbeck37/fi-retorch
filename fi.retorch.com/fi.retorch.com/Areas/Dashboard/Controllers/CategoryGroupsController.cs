using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using fi.retorch.com.Areas.Dashboard.Code.Base;
using fi.retorch.com.Areas.Dashboard.Code.Enums;
using System.Collections.Generic;

namespace fi.retorch.com.Areas.Dashboard.Controllers
{
    public class CategoryGroupsController : BaseController
    {
        // GET: Dashboard/CategoryGroups
        public ActionResult Index()
        {
            CategoryGroupList list = new CategoryGroupList(db, userKey);

            return View(list.Items);
        }

        // GET: Dashboard/CategoryGroups/Create
        public ActionResult Create()
        {
            var types = new Dictionary<int, string>();
            foreach (TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types;

            return View();
        }

        // POST: Dashboard/CategoryGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,TransferType")] CategoryGroupModel model)
        {
            if (ModelState.IsValid)
            {
                CategoryGroupModel.Save(model, db, userKey, true);
                
                return RedirectToAction("Index");
            }

            var types = new Dictionary<int, string>();
            foreach (TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types;

            return View(model);
        }

        // GET: Dashboard/CategoryGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            CategoryGroupModel model = CategoryGroupModel.Get(db, userKey, id.Value);

            if (model == null)
                return HttpNotFound();

            var types = new Dictionary<int, string>();
            foreach(TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types; // new SelectList(types, "Key", "Value", model.TransferType);

            return View(model);
        }

        // POST: Dashboard/CategoryGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TransferType")] CategoryGroupModel model)
        {
            if (ModelState.IsValid)
            {
                CategoryGroupModel.Save(model, db, userKey, false);

                return RedirectToAction("Index");
            }

            var types = new Dictionary<int, string>();
            foreach (TransferTypeEnum transferType in System.Enum.GetValues(typeof(TransferTypeEnum)))
            {
                types.Add((int)transferType, transferType.ToFriendlyString());
            }
            ViewBag.TransferTypes = types;

            return View(model);
        }

        // GET: Dashboard/CategoryGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            CategoryGroupModel model = CategoryGroupModel.Get(db, userKey, id.Value);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: Dashboard/CategoryGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryGroupModel.Delete(db, userKey, id);

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
