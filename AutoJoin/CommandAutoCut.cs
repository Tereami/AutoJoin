using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class CommandAutoCut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;

            //Выбрать пустотный элемент для вырезания
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();

            if (ids.Count == 0)
            {
                message = "Выберите элементы для соединения";
                return Result.Failed;
            }
            if (ids.Count > 1)
            {
                message = "Выберите только один элемент";
                return Result.Failed;
            }

            Element voidElem = doc.GetElement(ids.First());

            //получаю список элементов, которые пересекает данный элемент
            List<Element> elems = Intersection.GetAllIntersectionElements(doc, voidElem);

            if (elems == null)
            {
                message = "Элемент не имеет пересечений. Вырезание не выполнено";
                return Result.Failed;
            }

            //вырезаю элемент из данных элементов
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Вырезание геометрии");
                foreach (Element curElem in elems)
                {
                    Intersection.CutElement(doc, curElem, voidElem);
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}