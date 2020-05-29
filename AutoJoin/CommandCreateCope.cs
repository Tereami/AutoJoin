using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    class CommandCreateCope : IExternalCommand
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



            using (Transaction t = new Transaction(doc))
            {

                //проверяю пересечение каждой балки с каждой балкой
                foreach(FamilyInstance f1 in beams)
                {
                    t.Start("Создание врезки для id " + f1.Id);
                    foreach (FamilyInstance f2 in beams)
                    {
                        if (f1.Id == f2.Id) continue;

                        try
                        {
                            bool check = Intersection.CheckElementsIsIntersect(doc, f1, f2);
                            if (check == false) continue;

                            StructuralInstanceUsage su1 = f1.StructuralUsage;

                            int p1 = f1.LookupParameter("Приоритет врезки").AsInteger();
                            int p2 = f2.LookupParameter("Приоритет врезки").AsInteger();

                            if (p1 < p2)
                            {
                                f2.AddCoping(f1);
                            }
                            else
                            {
                                f1.AddCoping(f2);
                            }
                        }
                        catch { }
                    }
                    t.Commit();
                }
            }



            return Result.Succeeded;
        }
    }
}
