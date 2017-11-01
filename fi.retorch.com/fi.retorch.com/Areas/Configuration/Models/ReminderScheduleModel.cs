using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace fi.retorch.com.Areas.Configuration.Models
{
    public class ReminderScheduleList
    { 
        private Entities Database { get; set; }

        public List<ReminderScheduleModel> Items { get; set; }

        public ReminderScheduleList()
        {
        }

        public ReminderScheduleList(Entities database, bool? active = null)
        {
            Database = database;
            Items = new List<ReminderScheduleModel>();

            List<ReminderSchedule> data = Database.ReminderSchedules.Where(t => t.IsActive == active || active == null).ToList();
            data.ForEach(d => Items.Add(ReminderScheduleModel.ConvertDataToModel(d)));
        }

        public static List<ReminderScheduleModel> ConvertDataToList(List<ReminderSchedule> data)
        {
            List<ReminderScheduleModel> list = new List<ReminderScheduleModel>();
            data.ForEach(d => list.Add(ReminderScheduleModel.ConvertDataToModel(d)));

            return list;
        }
    }

    public class ReminderScheduleModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Reminder Schedule")]
        public string Name { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        
        public static ReminderScheduleModel ConvertDataToModel(ReminderSchedule data) {
            ReminderScheduleModel model = null;

            if (data != null)
            {
                model = new ReminderScheduleModel();

                model.Id = data.Id;
                model.Name = data.Name;
                model.IsActive = data.IsActive;
            }

            return model;
        }

        public static ReminderScheduleModel Get(Entities db, int id)
        {
            ReminderSchedule data = db.ReminderSchedules.Where(cg => cg.Id == id).FirstOrDefault();

            return ConvertDataToModel(data);
        }

        public static void Save(ReminderScheduleModel model, Entities db, bool isNew = false)
        {
            ReminderSchedule data = null;

            if (isNew)
            {
                data = new ReminderSchedule();
                data.DateCreated = DateTime.Now;

                data.Name = model.Name;
                data.IsActive = model.IsActive;

                db.ReminderSchedules.Add(data);
            }
            else
            {
                data = db.ReminderSchedules.Where(cg => cg.Id == model.Id).FirstOrDefault();

                data.Name = model.Name;
                data.IsActive = model.IsActive;

                db.Entry(data).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        public static void Delete(Entities db, int id)
        {
            ReminderSchedule data = db.ReminderSchedules.Where(cg => cg.Id == id).FirstOrDefault();
            db.ReminderSchedules.Remove(data);
            db.SaveChanges();
        }
    }
}