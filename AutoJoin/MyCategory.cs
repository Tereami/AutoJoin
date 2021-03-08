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

        public MyCategory(BuiltInCategory cat)
        {
            _category = cat;
        }

        public override string ToString()
        {
            //string displayName = LabelUtils.GetLabelFor(_category);
            switch (_category)
            {
                case BuiltInCategory.OST_Walls:
                    return "Стены";
                case BuiltInCategory.OST_Windows:
                    return "Окна";
                case BuiltInCategory.OST_Doors:
                    return "Двери";
                case BuiltInCategory.OST_Floors:
                    return "Перекрытия";
                case BuiltInCategory.OST_Roofs:
                    return "Крыши";
                case BuiltInCategory.OST_Columns:
                    return "Колонны";
                case BuiltInCategory.OST_Stairs:
                    return "Лестницы";
                case BuiltInCategory.OST_GenericModel:
                    return "Обобщенные модели";
                case BuiltInCategory.OST_StructuralFoundation:
                    return "Фундамент несущей конструкции";
                case BuiltInCategory.OST_StructuralFraming:
                    return "Каркас несущий";
                case BuiltInCategory.OST_StructuralColumns:
                    return "Несущие колонны";
                case BuiltInCategory.OST_Parts:
                    return "Части";
            }
            return Enum.GetName(typeof(BuiltInCategory), _category);
        }
    }
}
