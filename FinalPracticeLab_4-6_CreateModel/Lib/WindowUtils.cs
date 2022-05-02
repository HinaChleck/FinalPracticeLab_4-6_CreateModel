using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    internal class WindowUtils
    {

        public static FamilySymbol GetWindowType(Document doc, string windowFamilySymbolName, string windowFamilyName)
        {
            FamilySymbol windowType = new FilteredElementCollector(doc)
           .OfClass(typeof(FamilySymbol))
           .OfCategory(BuiltInCategory.OST_Windows)
           .OfType<FamilySymbol>()
           .Where(x => x.Name.Equals(windowFamilySymbolName))//искомое значение FamilySymbol типоразмера
           .Where(x => x.FamilyName.Equals(windowFamilyName))//c этой строкой не работает. windowType в итоге null
           .FirstOrDefault();

            return windowType;
        }
        public static FamilyInstance AddWindow(Document doc, FamilySymbol windowType, Wall wall)//добавляет окно определенного типоразмера по центру стены
        {
            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            Transaction transaction = new Transaction(doc, "Вставка окон");
            transaction.Start();

            if (!windowType.IsActive)
                windowType.Activate();

            FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, StructuralType.NonStructural);
            window.LookupParameter("Высота нижнего бруса").Set(UnitUtils.ConvertToInternalUnits(920, DisplayUnitType.DUT_MILLIMETERS));

            transaction.Commit();
            return window;
        }
    }
}
