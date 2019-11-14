//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace IngameScript.Tasks
//{
//    public class Job
//    {
//        public string Name { get; set; }
//        public List<Task> Tasks { get; set; }

//        public Job()
//        {
//            Tasks = new List<Task>();
//        }

//        public void Execute()
//        {
//            var task = Tasks.Where(c => !c.IsFinished).FirstOrDefault();
//            if (task != null)
//            {
//                task.Execute();
//            }
//        }
//    }
//}