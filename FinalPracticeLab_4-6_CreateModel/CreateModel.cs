using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreateModel : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> listLevel = new FilteredElementCollector(doc)//находит элементы - уровни
               .OfClass(typeof(Level))
               .OfType<Level>()
               .ToList();

            Level level1 = LevelsUtils.FindLevelByName(listLevel, "Уровень 1");
            Level level2 = LevelsUtils.FindLevelByName(listLevel, "Уровень 2");

            double width = UnitUtils.ConvertToInternalUnits(10000, DisplayUnitType.DUT_MILLIMETERS);
            double depth = UnitUtils.ConvertToInternalUnits(5000, DisplayUnitType.DUT_MILLIMETERS);

            List<XYZ> points = CornersUtils.GetCornersFromSize(width, depth);

            List<Wall> walls = WallsUtils.CreateWallsWithPoints(doc, level1, level2, points);

            string doorFamilySymbolName = "0915 x 2134 мм";
            string doorFamilyName = "M_Однопольные-Щитовые";

            FamilySymbol doorType = DoorsUtils.GetDoorType(doc, doorFamilySymbolName, doorFamilyName);

            DoorsUtils.AddDoor(doc, doorType, walls[3]);

            string windowFamilySymbolName = "0915 x 1220 мм";
            string windowFamilyName = "M_Неподвижный";

            FamilySymbol windowType = WindowUtils.GetWindowType(doc, windowFamilySymbolName, windowFamilyName);

            WindowUtils.AddWindow(doc, windowType, walls[0]);
            WindowUtils.AddWindow(doc, windowType, walls[1]);
            WindowUtils.AddWindow(doc, windowType, walls[2]);

            string roofFamilyType = "Типовая крыша - 300мм";
            string roofFamilyName = "Базовая крыша";

            RoofType roofType = RoofUtils.GetRoofType(doc, roofFamilyType, roofFamilyName);
            // FootPrintRoof footprintRoof = RoofUtils.CreateFootprintRoof(doc, level2, walls, roofType); // построение крыши по контуру основания

            ExtrusionRoof extrusionRoof = RoofUtils.CreateExtrusionRoof(doc, level2, walls[0], roofType); // построение крыши выдавливания

            return Result.Succeeded;
        }

    }
}
   