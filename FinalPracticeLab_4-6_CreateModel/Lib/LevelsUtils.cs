using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_4_6_CreateModel
{
    internal class LevelsUtils
    {
        public static Level FindLevelByName(Document doc, string levelName) //перегрузка 1
        {

            List<Level> listLevel = new FilteredElementCollector(doc)//находит элементы - уровни
               .OfClass(typeof(Level))
               .OfType<Level>()
               .ToList();

            Level level = listLevel
                .Where(x => x.Name.Equals("Уровень 1"))
                .FirstOrDefault();

            return level;
        }
        public static Level FindLevelByName(List<Level> listLevel, string levelName)//перегрузка 2
        {

            Level level = listLevel
                .Where(x => x.Name.Equals(levelName))
                .FirstOrDefault();

            return level;
        }


    }
}
