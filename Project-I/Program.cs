
using Project_I;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using System.Transactions;

List<Tasks> list = new List<Tasks>();
List<Tasks> sortedList;
ListManager LM = new ListManager();
XmlSerializer serializer2 = new XmlSerializer(typeof(List<Tasks>));



try
{
    using (FileStream fileStream2 = File.OpenRead(@"ListTestXML.xml"))
    {
        list = (List<Tasks>)serializer2.Deserialize(fileStream2);
    }
}
catch (FileNotFoundException e)
{
    using (Stream fileStream = new FileStream(@"ListTestXML.xml", FileMode.Create, FileAccess.Write, FileShare.None))
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Tasks>));
        serializer.Serialize(fileStream, list);
    }
}

int counterDone = 0;
if (!(list == null || list.Count == 0))
{
    foreach (Tasks task in list)
    {
        if (task.status)
        {
            counterDone++;
        }
    }
}

Console.WriteLine("Welcome to ");
Console.WriteLine("You have " + (list.Count - counterDone) + " task(s) todo and " + counterDone + " task(s) are done");
Console.WriteLine("Pick an option:");
Console.WriteLine();

// added tasks for testing purposes
//list.Add(new Tasks("Städning", "2023-11-20", false, "städa badrum"));
//list.Add(new Tasks("Städning", "2023-10-25", true, "städa Köket"));
//list.Add(new Tasks("Städning", "2023-11-23", false, "städa vardagsrummet"));
//list.Add(new Tasks("Städning", "2023-11-09", false, "städa badrum igen"));
//list.Add(new Tasks("Cykel", "2023-10-19", false, "olja kedjan"));
//list.Add(new Tasks("Cykel", "2023-10-09", false, "byt ut sadeln"));

Console.ForegroundColor = ConsoleColor.Green;
Console.ResetColor();

int padding = 15;

while (true)
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("(1) Show task list (by date or project)");
    Console.WriteLine("(2) Add new task");
    Console.WriteLine("(3) Edit task (update, mark as done, remove");
    Console.WriteLine("(4) Save and quit");
    Console.ResetColor();

    string data = Console.ReadLine().Trim();

    // Option 1: Show sorted list
    if (data == "1")
    {
        // checking if list is empty
        if (list == null || list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No tasks to show");
            Console.ResetColor();
            Console.WriteLine();
            continue;
        }
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Choose sort option: 'd' to sort by due date | 'p' to sort by project");
        Console.ResetColor();
        string sortOption = Console.ReadLine().ToLower().Trim();


        if (sortOption == "p")
        {
            sortedList = list.OrderBy(x => x.project).ToList();
        }
        else if (sortOption == "d")
        {
            sortedList = list.OrderBy(x => x.dueDate).ToList();
        }
        else
        {
            Console.WriteLine("Showing unsorted list");
            sortedList = list;
        }
        Console.WriteLine("project".PadRight(padding) + "due date".PadRight(padding) + "status".PadRight(10) + "Title");
        string statusText;
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i].status == true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                statusText = "Done";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                statusText = "Not done";
            }
            Console.WriteLine(sortedList[i].project.PadRight(padding) + sortedList[i].dueDate.PadRight(padding) + statusText.PadRight(10) + sortedList[i].title);
            Console.ResetColor();
        }
        Console.WriteLine();
        continue;
    }

    // Option 2: Add new task
    if (data == "2")
    {
    redoTask:
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Need to add project, due date and title");
        Console.ResetColor();
        Console.WriteLine();

        Console.Write("Project: ");
        string project = Console.ReadLine().Trim();

    tryDate:
        Console.Write("Due date (yyyy-mm-dd): ");
        string dueDate = Console.ReadLine().Trim();

        if (!DateTime.TryParse(dueDate, out DateTime dateValue))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Please enter in the correct format (yyyy-mm-dd)");
            Console.ResetColor();
            goto tryDate;
        }

        Console.Write("Title: ");
        string title = Console.ReadLine().Trim();

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Press 'Enter' to confirm task, or 'q' to redo task");
        Console.ResetColor();

        data = Console.ReadLine().ToLower().Trim();
        if (data == "q")
        {
            goto redoTask;
        }

        list.Add(new Tasks(project, dateValue.ToString("yyyy-MM-dd"), false, title));

        Console.WriteLine();
        continue;

    }

    // Option 3: Edit tasks
    if (data == "3")
    {
        // checking if list is empty
        if (list == null || list.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No tasks to edit");
            Console.ResetColor();
            Console.WriteLine();
            continue;
        }
    tryEditOptionAgain:
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("What would you like to do? | Enter '1' to edit task, '2' to mark as done/not done, '3' to remove task | 'q' to go back to start");
        Console.ResetColor();
        string editOption = Console.ReadLine().ToLower().Trim();

        //EditOption 1: Edit task
        if (editOption == "1")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Write rownumber for the task you wish to edit");
            Console.ResetColor();

            // Sort by project and present list with alternating rowcolors
            LM.SortList(list);

        tryRowNumberAgain:
            string rowNumberStr = Console.ReadLine().Trim();
            try
            {
                sortedList = list.OrderBy(x => x.project).ToList();
                int rowNumber = Int32.Parse(rowNumberStr);
                Tasks q = sortedList[rowNumber];            // this line helps to catch out of range exception before asking for the paramater
            tryParamaterAgain:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("What pramater to edit? | Enter '1' for Project, '2' for Due date, '3' for Title | 'q' to go back to start");
                Console.ResetColor();
                string paramaterOption = Console.ReadLine().ToLower().Trim();

                //Option 1: parameter project
                if (paramaterOption == "1")
                {
                tryProject:
                    Console.WriteLine("Current project: " + q.project);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Write new projet:");
                    Console.ResetColor();
                    string newProject = Console.ReadLine().Trim();

                    if (newProject != "")
                    {
                        q.project = newProject;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Project edited");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("project name can't be empty or null");
                        Console.ResetColor();
                        goto tryProject;
                    }
                }


                //Option 2: parameter due date
                else if (paramaterOption == "2")
                {
                tryDueDate:
                    Console.WriteLine("Current due date: " + q.dueDate);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Write new due date (yyyy-MM-dd):");
                    Console.ResetColor();
                    string newDueDate = Console.ReadLine().Trim();
                    if (DateTime.TryParse(newDueDate, out DateTime dateValue))
                    {
                        q.dueDate = dateValue.ToString("yyyy-MM-dd");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Due date edited");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Date in wrong format, try again");
                        Console.ResetColor();
                        goto tryDueDate;
                    }
                }

                //Option 3: parameter Title
                else if (paramaterOption == "3")
                {
                tryTitle:
                    Console.WriteLine("Current title: " + q.title);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Write new title:");
                    Console.ResetColor();
                    string newtitle = Console.ReadLine().Trim();
                    if (newtitle != "")
                    {
                        q.title = newtitle;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("title edited");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("title name can't be empty or null");
                        Console.ResetColor();
                        goto tryTitle;
                    }
                }

                //Option 4 (q): go back to start
                else if (paramaterOption == "q")
                {
                    continue;
                }

                //No option selected
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("No valid option was pressed, try again");
                    Console.ResetColor();
                    goto tryParamaterAgain;
                }

            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(rowNumberStr + " is not a valid number, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Row " + rowNumberStr + " was not found, outside the list size, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            Console.WriteLine();
        }


        //EditOption 2: Mark as done/not done
        else if (editOption == "2")
        {
        tryRowNumberAgain:
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Write rownumber for the task you wish to mark done/not done");
            Console.ResetColor();
            LM.SortList(list);
            string rowNumberStr = Console.ReadLine().Trim();
            try
            {
                sortedList = list.OrderBy(x => x.project).ToList();
                int rowNumber = Int32.Parse(rowNumberStr);
                Tasks q = sortedList[rowNumber];            // this line helps to catch out of range exception before asking for the paramater
                q.status = !q.status;
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(rowNumberStr + " is not a valid number, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Row " + rowNumberStr + " was not found, outside the list size, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            Console.WriteLine();
        }

        //EditOption 3: Remove
        else if (editOption == "3")
        {
        tryRowNumberAgain:
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("To remove task enter rownumber | to remove all tasks enter 'all'");
            Console.ResetColor();

            LM.SortList(list);
            string rowNumberStr = Console.ReadLine().ToLower().Trim();
            if (rowNumberStr == "all")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Are you sure you want to delete all tasks? | 'N' / 'Y'");
                Console.ResetColor();
                string yesNo = Console.ReadLine().ToLower().Trim();
                if (yesNo == "y")
                {
                    list.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("All tasks has been removed");
                    Console.ResetColor();
                    continue;
                }
                else
                {
                    continue;
                }
            }
            try
            {
                list = list.OrderBy(x => x.project).ToList();
                int rowNumber = Int32.Parse(rowNumberStr);
                Tasks q = list[rowNumber];            // this line helps to catch out of range exception before asking for the paramater
                list.RemoveAt(rowNumber);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(q.project.PadRight(padding) + q.dueDate.PadRight(12) + q.title);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Has been removed");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(rowNumberStr + " is not a valid number, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Row " + rowNumberStr + " was not found, outside the list size, try again.");
                Console.ResetColor();
                goto tryRowNumberAgain;
            }
            Console.WriteLine();
        }

        //EditOption 4 (q): Go back to start
        else if (editOption == "q")
        {
            continue;
        }

        //No option selected
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No valid option was pressed, try again");
            Console.ResetColor();
            goto tryEditOptionAgain;
        }

    }

    // Option 4: Save and quit
    if (data == "4")
    {
        using (Stream fileStream = new FileStream(@"ListTestXML.xml", FileMode.Create, FileAccess.Write, FileShare.None))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Tasks>));
            serializer.Serialize(fileStream, list);
        }
        break;
    }

}

