using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    internal class DoorsUtils
    {
        public static FamilySymbol GetDoorType(Document doc, string doorFamilySymbolName, string doorFamilyName)
        {

            FamilySymbol doorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals(doorFamilySymbolName))//искомое значение FamilySymbol типоразмера
                .Where(x => x.FamilyName.Equals(doorFamilyName))
                .FirstOrDefault();


            return doorType;
        }
        public static FamilyInstance AddDoor(Document doc, FamilySymbol doorType, Wall wall)//добавляет дверь определенного типоразмера по центру стены
        {
            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            Transaction transaction = new Transaction(doc, "Вставка дверей");
            transaction.Start();

            if (!doorType.IsActive)
                doorType.Activate();

            FamilyInstance door = doc.Create.NewFamilyInstance(point, doorType, wall, StructuralType.NonStructural);

            transaction.Commit();
            return door;
        }


    }
}
