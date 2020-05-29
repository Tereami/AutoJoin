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
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    class CommandAutoJoin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;

            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Выбрать элементы для соединения
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();

            if (ids.Count == 0)
            {
                message = "Выберите элементы для соединения";
                return Result.Failed;
            }

            List<Element> elems = new List<Element>();
            foreach (ElementId id in ids)
            {
                Element elem = doc.GetElement(id);

                elems.Add(elem);
            }

            bool isExecute = false;

            //каждый элемент в списке соединить с каждым элементом из списка
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Соединение " + elems.Count.ToString() + " элементов");

                foreach (Element elem1 in elems)
                {
                    foreach (Element elem2 in elems)
                    {
                        //если этот тот же элемент - пропустить
                        if (elem2.Equals(elem1)) continue;

                        //если элементы уже ранее соединены - пропустить
                        bool alreadyJoined = JoinGeometryUtils.AreElementsJoined(doc, elem1, elem2);
                        if (alreadyJoined) continue;

                        //проверить, пересекаются ли элементы, если не пересекаются - пропустить
                        bool isIntersects = Intersection.CheckElementsIsIntersect(doc, elem1, elem2);
                        if (!isIntersects) continue;

                        //соединить элементы
                        JoinGeometryUtils.JoinGeometry(doc, elem1, elem2);
                        isExecute = true;
                    }
                }

                t.Commit();
            }

            if (!isExecute)
            {
                message = "Элементы не имеют пересечения геометрии";
                return Result.Cancelled;
            }


            return Result.Succeeded;
        }
    }
}
