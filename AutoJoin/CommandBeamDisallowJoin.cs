using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandBeamDisallowJoin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            if (sel.GetElementIds().Count == 0)
            {
                TaskDialog.Show("Ошибка", "Выберите балки");
                return Result.Cancelled;
            }

            List<FamilyInstance> beams = sel.GetElementIds()
                .Select(i => doc.GetElement(i))
                .Cast<FamilyInstance>()
                .ToList();

            int count = 0;
            int errCount = 0;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отмена соединений балок");
                foreach (FamilyInstance beam in beams)
                {
                    try
                    {
                        Autodesk.Revit.DB.Structure
                            .StructuralFramingUtils.DisallowJoinAtEnd(beam, 0);
                        Autodesk.Revit.DB.Structure
                            .StructuralFramingUtils.DisallowJoinAtEnd(beam, 1);

                        count++;
                    }
                    catch { errCount++; }
                } 

                t.Commit();
            }

            TaskDialog.Show("Отмена соединения", "Обработано балок: " + count + "; ошибок: " + errCount);



                return Result.Succeeded;
        }
    }
}
