using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_I
{

    internal class ListManager
    {
        public ListManager()
        {
        }

        // Sorts by project and presents list with alternating rowcolors
        public void SortList(List<Tasks> list)
        {
            int padding = 15;
            List<Tasks> sortedList = list.OrderBy(x => x.project).ToList();
            Console.WriteLine("".PadRight(6) + "project".PadRight(padding) + "due date".PadRight(padding) + "status".PadRight(10) + "Title");
            string statusText;
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].status == true) { statusText = "Done"; }

                else { statusText = "Not done"; }

                Console.Write("{0,4}", "[" + i + "]");
                Console.WriteLine(sortedList[i].project.PadRight(padding).PadLeft(17) + sortedList[i].dueDate.PadRight(padding) + statusText.PadRight(10) + sortedList[i].title);
                if (i % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ResetColor();
                }
            }
            Console.ResetColor();
        }

    }
}
