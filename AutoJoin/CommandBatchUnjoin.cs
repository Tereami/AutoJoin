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

    class CommandBatchUnjoin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<ElementId> selids = sel.GetElementIds().ToList();

            if(selids.Count == 0)
            {
                message = "Выберите элементы";
                return Result.Failed;
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отсоединение элементов");
                foreach (ElementId selId in selids)
                {
                    Element elem = doc.GetElement(selId);
                    List<ElementId> ids = JoinGeometryUtils.GetJoinedElements(doc, elem).ToList();

                    foreach (ElementId id in ids)
                    {
                        Element elem2 = doc.GetElement(id);
                        JoinGeometryUtils.UnjoinGeometry(doc, elem, elem2);
                    }
                    doc.Regenerate();
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
