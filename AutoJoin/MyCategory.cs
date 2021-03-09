using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace AutoJoin
{
    public class MyCategory
    {
        public BuiltInCategory _category;
        public int priority;
        public string Name;

        public MyCategory(Document doc, BuiltInCategory bic)
        {
            _category = bic;

            Category cat = Category.GetCategory(doc, bic);
            Name = cat.Name;
        }

        public override string ToString()
        {
            //string displayName = LabelUtils.GetLabelFor(_category);

            return Name;
        }
    }
}
