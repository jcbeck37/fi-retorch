using fi.retorch.com.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace fi.retorch.com.Areas.Dashboard.EntityModels
{
    public class BookmarkEntity
    {
        public Bookmark Bookmark { get; set; }
        public bool IsNew { get; set; }

        public static List<BookmarkEntity> GetBookmarkList(Entities db, string userKey, bool? isActive = null)
        {
            List<BookmarkEntity> list = new List<BookmarkEntity>();

            var query = from b in db.Bookmarks
                                  where b.UserId == userKey && (isActive == null || b.IsActive == isActive.Value)
                                  select new BookmarkEntity
                                  {
                                      Bookmark = b
                                  };

            if (isActive.HasValue && isActive.Value)
                list = query.OrderBy(q => q.Bookmark.Text).ToList();
            else
                list = query.ToList();

            return list;
        }

        public static BookmarkEntity GetBookmark(Entities db, string userKey, int id)
        {
            var query = from b in db.Bookmarks
                        where b.UserId == userKey && b.Id == id
                        select new BookmarkEntity
                        {
                            Bookmark = b
                        };

            BookmarkEntity entity = query.FirstOrDefault();

            return entity;
        }

        public void Save(Entities db, string userKey)
        {
            if (IsNew)
            {
                Bookmark.UserId = userKey;
                Bookmark.DateCreated = DateTime.Now;
                
                db.Bookmarks.Add(Bookmark);
            }
            else
            {
                Bookmark data = db.Bookmarks.Where(cg => cg.Id == Bookmark.Id && cg.UserId == userKey).FirstOrDefault();

                if (data != null)
                {
                    data.URL = Bookmark.URL;
                    data.Text = Bookmark.Text;
                    data.IsActive = Bookmark.IsActive;

                    db.Entry(data).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
        }
        
        public void Delete(Entities db, string userKey)
        {
            Bookmark data = db.Bookmarks.Where(cg => cg.Id == Bookmark.Id && cg.UserId == userKey).FirstOrDefault();

            if (data != null)
            {
                db.Bookmarks.Remove(data);
                db.SaveChanges();
            }
        }
    }
}