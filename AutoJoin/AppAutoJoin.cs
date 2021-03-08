using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

using System.Windows.Media.Imaging;

namespace AutoJoin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class AppAutoJoin : IExternalApplication
    {
        public static string assemblyPath = "";
        public static string iconsPath = "";

        public Result OnStartup(UIControlledApplication application)
        {
            assemblyPath = typeof(AppAutoJoin).Assembly.Location;
            iconsPath = Path.GetDirectoryName(assemblyPath) + "\\icons";

            string tabName = "Weandrevit";
            try { application.CreateRibbonTab(tabName); }
            catch { }

            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Соединение элементов");
            PushButton btnJoin = panel.AddItem(new PushButtonData(
                "Join",
                "Соединить\nвыбранные",
                assemblyPath,
                "AutoJoin.CommandAutoJoin")
                ) as PushButton;
            btnJoin.ToolTip = "Соединение всех выбранных элементов. Соединяются только элементы, имеющие пересечения.";

            PushButton btnCut = panel.AddItem(new PushButtonData(
                "Cut",
                "Вырезать\nиз всех",
                assemblyPath,
                "AutoJoin.CommandAutoCut")
                ) as PushButton;
            btnCut.ToolTip = "Вырезать пустотный элемент из всех элементов, которые он пересекает";
            ///btnCut.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "AutoJoinSelected.png")));

            PushButton btnCutSelected = panel.AddItem(new PushButtonData(
                "CutSelected",
                "Вырезать\nиз выбранных",
                assemblyPath,
                "AutoJoin.CommandAutoCutSelected")
                ) as PushButton;
            btnCutSelected.ToolTip = "Вырезать пустотный элемент из выбранных элементов";
            ///btnCutSelected.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "AutoJoinSelected.png")));


            PushButton btnCreateCope = panel.AddItem(new PushButtonData(
                "CreateCope",
                "Создать\nврезки",
                assemblyPath,
                "AutoJoin.CommandCreateCope")
                ) as PushButton;
            btnCutSelected.ToolTip = "Создать врезки для выделенных балок";


            PushButton btnDisallowJoin = panel.AddItem(new PushButtonData(
                "DisallowJoin",
                "Отсоединить\nконцы",
                assemblyPath,
                "AutoJoin.CommandBeamDisallowJoin")
                ) as PushButton;
            btnCutSelected.ToolTip = "Отменить соединение концов выделенных балок";


            PushButton btnJoinByOrder = panel.AddItem(new PushButtonData(
                "JoinByOrder",
                "Задать приоритет",
                assemblyPath,
                "AutoJoin.CommandJoinByOrder")
                ) as PushButton;




            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
