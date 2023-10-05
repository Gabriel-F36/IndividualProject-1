using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Project_I
{
    [Serializable()]
    public class Tasks : ISerializable
    {
        public Tasks()
        {
        }
        public Tasks(string Project, string DueDate, bool Status, string Title)
        {
            title = Title;
            dueDate = DueDate;
            Status = false;
            status = Status;
            project = Project;
        }

        // properties

        public string title { get; set; }
        public string dueDate { get; set; }
        public bool status { get; set; }
        public string project { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("title", title);
            info.AddValue("dueDate", dueDate);
            info.AddValue("status", status);
            info.AddValue("project", project);
        }

        public Tasks(SerializationInfo info, StreamingContext context)
        {
            title = (string)info.GetValue("title", typeof(string));
            dueDate = (string)info.GetValue("dueDate", typeof(string));
            title = (string)info.GetValue("title", typeof(string));
            status = (bool)info.GetValue("title", typeof(string));

        }


    }
}
