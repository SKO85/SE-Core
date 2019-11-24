using SKO85Core.Serialization;
using System.Collections.Generic;

namespace SKO85Core.Work
{
    public class Task
    {
        public enum TaskState
        {
            Idle = 0,
            Running = 1,
            Done = 2,
            Aborted = 3
        }

        public string Name { get; set; }
        public bool IsParallel { get; set; }
        public TaskState State { get; set; }
        public Dictionary<string, Field> Data { get; set; }
        public List<Task> SubTasks { get; set; }

        public Task(string name, Dictionary<string, Field> data, List<Task> subTasks = null, bool isParallel = false)
        {
            this.Name = name;
            this.IsParallel = isParallel;
            this.State = TaskState.Idle;
            this.Data = data;

            if (subTasks != null)
                this.SubTasks = subTasks;
            else
                this.SubTasks = new List<Task>();
        }
    }
}
