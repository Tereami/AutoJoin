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
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;
#endregion

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    class CommandAutoJoin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("AutoJoin"));
            UIApplication uiApp = commandData.Application;

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;

            //Выбрать элементы для соединения
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();
            Debug.WriteLine("Selected elems: " + ids.Count.ToString());

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
                    Debug.WriteLine("Join elem id " + elem1.Id.IntegerValue.ToString() + " with... ");
                    foreach (Element elem2 in elems)
                    {
                        //если этот тот же элемент - пропустить
                        if (elem2.Equals(elem1)) continue;

                        Debug.WriteLine(" id " + elem2.Id.IntegerValue.ToString());
                        //если элементы уже ранее соединены - пропустить
                        bool alreadyJoined = JoinGeometryUtils.AreElementsJoined(doc, elem1, elem2);
                        if (alreadyJoined)
                        {
                            Debug.WriteLine("Elements are already joined");
                            continue;
                        }

                        //проверить, пересекаются ли элементы, если не пересекаются - пропустить
                        bool isIntersects = Intersection.CheckElementsIsIntersect(doc, elem1, elem2);
                        if (!isIntersects)
                        {
                            Debug.WriteLine("Elements dont have intersection");
                            continue;
                        }

                        //соединить элементы
                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, elem1, elem2);
                            isExecute = true;
                            Debug.WriteLine("Joined succesfully!");
                        }
                        catch(Exception ex) 
                        {
                            Debug.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }

                t.Commit();
            }

            if (!isExecute)
            {
                message = "Элементы не имеют пересечений или уже соединены";
                return Result.Cancelled;
            }

            Debug.WriteLine("AutoJoin success");
            return Result.Succeeded;
        }
    }
}
