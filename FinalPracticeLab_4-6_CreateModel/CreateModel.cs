using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
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

            double width = UnitUtils.ConvertToInternalUnits(12000, DisplayUnitType.DUT_MILLIMETERS);
            double depth = UnitUtils.ConvertToInternalUnits(6000, DisplayUnitType.DUT_MILLIMETERS);

            List<XYZ> points = CornersUtils.GetCornersFromSize(width, depth);

            List<Wall> walls = WallsUtils.CreateWallsWithPoints(doc, level1, level2, points);

            return Result.Succeeded;
        }


    }
}
   