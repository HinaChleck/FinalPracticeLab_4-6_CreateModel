using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    public class RoofUtils
    {
        public static RoofType GetRoofType(Document doc, string roofFamilyType, string roofFamilyName)
        {
            RoofType roofType = new FilteredElementCollector(doc)
                .OfClass(typeof(RoofType))
                .OfType<RoofType>()
                .Where(x => x.Name.Equals(roofFamilyType))
                //.Where(x => x.Name.Equals(roofFamilyName))
                .FirstOrDefault();
            return roofType;
        }
        public static FootPrintRoof CreateFootprintRoof(Document doc, Level level, List<Wall> walls, RoofType roofType)//создает крышу с основанием по выбранному контуру
        {
            double wallWidth = walls[0].Width;
            double dt = wallWidth / 2;
            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dt, -dt, 0));
            points.Add(new XYZ(-dt, dt, 0));
            points.Add(new XYZ(dt, dt, 0));
            points.Add(new XYZ(dt, -dt, 0));
            points.Add(new XYZ(-dt, -dt, 0));

            Application application = doc.Application;
            CurveArray footprint = application.Create.NewCurveArray();
            for (int i = 0; i < walls.Count; i++)
            {
                LocationCurve curve = walls[i].Location as LocationCurve;
                XYZ p1 = curve.Curve.GetEndPoint(0);
                XYZ p2 = curve.Curve.GetEndPoint(1);
                Line line = Line.CreateBound(p1 + points[i], p2 + points[i + 1]);
                footprint.Append(line);//с помощью метода расширения Curve получаем из LocationCurve саму Curve    
            }

            Transaction transaction = new Transaction(doc, "Построение крыши");
            transaction.Start();

            ModelCurveArray footPrintToModelCurveMapping = new ModelCurveArray();
            FootPrintRoof footprintRoof = doc.Create.NewFootPrintRoof(footprint, level, roofType, out footPrintToModelCurveMapping);

            //footprintRoof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM).Set("Уровень 2");//не работает. Базовый уровень не меняется
            footprintRoof.get_Parameter(BuiltInParameter.ROOF_LEVEL_OFFSET_PARAM).Set(UnitUtils.ConvertToInternalUnits(4000, DisplayUnitType.DUT_MILLIMETERS));

            foreach (ModelCurve m in footPrintToModelCurveMapping)
            {
                footprintRoof.set_DefinesSlope(m, true);
                footprintRoof.set_SlopeAngle(m, 0.5);
            }
            transaction.Commit();
            return footprintRoof;
        }

        public static ExtrusionRoof CreateExtrusionRoof(Document doc, Level level, Wall wall, RoofType roofType)//Создает крышу выдавливанием со свесом во все стороны по 2 дюйма с образующей в плоскости заданной стены
        {
            Application application = doc.Application;

            double wallWidth = wall.Width;
            double dt = wallWidth / 2;

            CurveArray profile = application.Create.NewCurveArray();
            LocationCurve curve = wall.Location as LocationCurve;
            XYZ point1 = curve.Curve.GetEndPoint(0);
            XYZ point2 = curve.Curve.GetEndPoint(1);
            XYZ midTopPoint = new XYZ((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point2.X - point1.X + point2.Y - point1.Y) / 2 + UnitUtils.ConvertToInternalUnits(4000, DisplayUnitType.DUT_MILLIMETERS));
            profile.Append(Line.CreateBound(new XYZ(point1.X - dt - 2, point1.Y - dt - 2, point1.Z + UnitUtils.ConvertToInternalUnits(4000, DisplayUnitType.DUT_MILLIMETERS)), midTopPoint));
            profile.Append(Line.CreateBound(midTopPoint, new XYZ(point2.X + dt + 2, point2.Y + dt + 2, point2.Z + UnitUtils.ConvertToInternalUnits(4000, DisplayUnitType.DUT_MILLIMETERS))));


            Transaction transaction = new Transaction(doc, "Построение крыши");
            transaction.Start();

            ReferencePlane refPlane = doc.Create.NewReferencePlane(new XYZ(0, 0, 0), new XYZ(0, 0, 40), new XYZ(0, 40, 0), doc.ActiveView);

            ExtrusionRoof extrusionRoof = doc.Create.NewExtrusionRoof(profile, refPlane, level, roofType, -UnitUtils.ConvertToInternalUnits(5000, DisplayUnitType.DUT_MILLIMETERS) - dt - 2, UnitUtils.ConvertToInternalUnits(5000, DisplayUnitType.DUT_MILLIMETERS) + dt + 2);

            //extrusionRoof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_OFFSET_PARAM).Set(UnitUtils.ConvertToInternalUnits(4000, DisplayUnitType.DUT_MILLIMETERS));//ничего не дает
            transaction.Commit();
            return extrusionRoof;
        }
    }
}
