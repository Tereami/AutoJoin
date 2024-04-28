#region License
/*Данный код опубликован под лицензией Creative Commons Attribution-ShareAlike.
Разрешено использовать, распространять, изменять и брать данный код за основу для производных в коммерческих и
некоммерческих целях, при условии указания авторства и если производные лицензируются на тех же условиях.
Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
Зуев Александр, 2021, все права защищены.
This code is listed under the Creative Commons Attribution-ShareAlike license.
You may use, redistribute, remix, tweak, and build upon this work non-commercially and commercially,
as long as you credit the author by linking back and license your new creations under the same terms.
This code is provided 'as is'. Author disclaims any implied warranty.
Zuev Aleksandr, 2021, all rigths reserved.*/
#endregion
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class CommandAutoCut : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new RbsLogger.Logger("AutoCut"));
            Trace.WriteLine("Start AutoCut");

            UIApplication uiApp = commandData.Application;

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;

            //Выбрать пустотный элемент для вырезания
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();
            Trace.WriteLine("Selected elements: " + ids.Count.ToString());

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
            Trace.WriteLine("Intersection elements: " + elems.Count.ToString());

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
            Trace.WriteLine("AutoCut complete");
            return Result.Succeeded;
        }
    }
}