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
    class CommandJoinByOrder : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();

            if (ids.Count == 0)
            {
                message = "Выберите элементы для соединения";
                return Result.Failed;
            }

            if(ids.Count > 100)
            {
                TaskDialog dialog = new TaskDialog("Предупреждение");
                dialog.MainInstruction = "Выделено большое количество элементов (" + ids.Count.ToString()
                    + "), их соединение может привести к снижение быстродействия модели. Продолжить?";
                dialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                if (dialog.Show() != TaskDialogResult.Ok)
                {
                    return Result.Cancelled;
                }
            }

            List<Element> elems = new List<Element>();
            foreach (ElementId id in ids)
            {
                Element elem = doc.GetElement(id);
                elems.Add(elem);
            }

            

            List<MyCategory> uniqCats = elems
                .Select(i => (BuiltInCategory)i.Category.Id.IntegerValue)
                .Distinct()
                .Select(i => new MyCategory(i))
                .ToList();

            FormSetJoinOrder form = new FormSetJoinOrder(uniqCats);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return Result.Cancelled;
            
            Dictionary<BuiltInCategory, int> categoriesPriority =
                form.Cats.ToDictionary(i => i._category, j => j.priority);

            int counter = 0;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Соединение " + elems.Count.ToString() + " элементов");

                foreach (Element elem1 in elems)
                {
                    foreach (Element elem2 in elems)
                    {
                        if (elem2.Equals(elem1)) continue;

                        bool alreadyJoined = JoinGeometryUtils.AreElementsJoined(doc, elem1, elem2);
                        bool isIntersects = Intersection.CheckElementsIsIntersect(doc, elem1, elem2);
                        if (!isIntersects && !alreadyJoined) continue;

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, elem1, elem2);
                        }
                        catch { }

                        bool isElemJoined = JoinGeometryUtils.AreElementsJoined(doc, elem1, elem2);
                        if (!isElemJoined) continue;

                        bool isFirstElemMain = JoinGeometryUtils.IsCuttingElementInJoin(doc, elem1, elem2);
                        int firstElemPriority = categoriesPriority[(BuiltInCategory)elem1.Category.Id.IntegerValue];
                        int secondElemPriority = categoriesPriority[(BuiltInCategory)elem2.Category.Id.IntegerValue];

                        if(isFirstElemMain && firstElemPriority > secondElemPriority)
                        {
                            JoinGeometryUtils.SwitchJoinOrder(doc, elem1, elem2);
                            counter++;
                        }    
                    }
                }

                doc.Regenerate();



                t.Commit();
            }

            if (counter == 0)
            {
                message = "Все соединения уже соответствуют заданному приоритету";
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }
    }
}
