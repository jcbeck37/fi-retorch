using fi.retorch.com.Areas.Dashboard.Models;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.Linq;

namespace fi.retorch.com.Areas.Legacy.Models
{
    public class LegacyBookmarkList
    {
        public List<LegacyBookmarkModel> Items { get; set; }

        public LegacyBookmarkList(LegacyEntities db, int userId)
        {
            Items = new List<LegacyBookmarkModel>();
            
            var data = from act in db.act_links
                       where act.user_id == userId
                       select act;
            data.Distinct().ToList().ForEach(d => Items.Add(LegacyBookmarkModel.ConvertDataToModel(d)));
        }
    }

    public class LegacyBookmarkModel
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }

        public static LegacyBookmarkModel ConvertDataToModel(act_links data)
        {
            LegacyBookmarkModel model = new LegacyBookmarkModel();

            model.Id = data.link_id;
            model.URL = data.link_url;
            model.Text = data.link_title;
            model.IsActive = data.link_inactive == 0;

            return model;
        }

        public BookmarkModel ConvertLegacyToModel()
        {
            BookmarkModel model = new BookmarkModel();
            
            model.URL = URL;
            model.Text = Text;
            model.IsActive = IsActive;
            
            return model;
        }
    }
}