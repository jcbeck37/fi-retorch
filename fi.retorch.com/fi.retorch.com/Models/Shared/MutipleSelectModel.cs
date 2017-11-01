using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Models
{
    public class MultipleSelectData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public static MultipleSelectData ConvertDataToModel(int id, bool isActive)
        {
            MultipleSelectData model = new MultipleSelectData();
            model.Id = id;
            model.IsActive = isActive;
            return model;
        }
    }

    public class MultipleSelectListModel
    {
        public static List<MultipleSelectModel> ConvertDataToModel(List<MultipleSelectData> data, List<MultipleSelectData> selected)
        {
            List<MultipleSelectModel> model = new List<MultipleSelectModel>();

            data.ForEach(d => model.Add(MultipleSelectModel.ConvertDataToModel(d.Id, d.Name, selected.Exists(s => s.Id == d.Id && s.IsActive == true))));

            return model;
        }
    }

    public class MultipleSelectModel
    {
        [Key]
        public int SelectionId { get; set; }
        public string SelectionName { get; set; }
        public bool IsChecked { get; set; }

        public static MultipleSelectModel ConvertDataToModel(int id, string name, bool isActive)
        {
            MultipleSelectModel model = new MultipleSelectModel();

            model.SelectionId = id;
            model.SelectionName = name;
            model.IsChecked = isActive;

            return model;
        }
    }
}