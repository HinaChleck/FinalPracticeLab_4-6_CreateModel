using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    internal class WallsUtils
    {
        public static List<Wall> CreateWallsWithPoints(Document doc, Level level1, Level level2, List<XYZ> points)//создает стены по точкам из списка по порядку высотой от одного уровня ло другого
        {
            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();

            for (int i = 0; i < (points.Count - 1); i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);//можно было использовать перегрузку, в котором можно указать высоту стены, но мы привяжем ко воторому уровню методом ниже
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }

            transaction.Commit();
            return walls;
        }

    }
}
