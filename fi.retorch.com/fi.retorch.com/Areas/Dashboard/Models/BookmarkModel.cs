using fi.retorch.com.Areas.Dashboard.EntityModels;
using fi.retorch.com.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Dashboard.Models
{
    public class BookmarkList
    {
        public List<BookmarkModel> Items { get; set; }

        public BookmarkList(Entities db, string userKey, bool? isActive = null)
        {
            Items = new List<BookmarkModel>();

            List<BookmarkEntity> list = BookmarkEntity.GetBookmarkList(db, userKey, isActive);
            
            list.ForEach(e => Items.Add(BookmarkModel.ConvertEntityToModel(e)));
        }
    }

    public class BookmarkModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Linked Text")]
        public string Text { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        
        public BookmarkModel() { }

        public BookmarkModel(Entities db, string userKey, int id)
        {
            BookmarkEntity entity = BookmarkEntity.GetBookmark(db, userKey, id);

            ConvertEntityToModel(entity, this);
        }

        public static BookmarkModel ConvertEntityToModel(BookmarkEntity entity, BookmarkModel model = null)
        {
            if (model == null)
                model = new BookmarkModel();

            if (entity != null && entity.Bookmark != null)
            {
                model.Id = entity.Bookmark.Id;
                model.URL = entity.Bookmark.URL;
                model.Text = entity.Bookmark.Text;
                model.IsActive = entity.Bookmark.IsActive;
            }

            return model;
        }

        public BookmarkEntity ConvertModelToEntity(bool isNew = false)
        {
            BookmarkEntity entity = new BookmarkEntity();

            entity.Bookmark = new Bookmark();
            entity.IsNew = isNew;

            if (!isNew)
                entity.Bookmark.Id = Id;
            entity.Bookmark.URL = URL;
            entity.Bookmark.Text = Text;
            entity.Bookmark.IsActive = IsActive;

            return entity;
        }

        public void Save(BookmarkModel model, Entities db, string userKey, bool isNew = false)
        {
            BookmarkEntity entity = ConvertModelToEntity(isNew);
            entity.Save(db, userKey);
        }

        public void Delete(Entities db, string userKey, int id)
        {
            BookmarkEntity entity = ConvertModelToEntity(false);
            entity.Delete(db, userKey);
        }
    }
}