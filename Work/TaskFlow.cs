using System;
using System.Collections.Generic;
using System.Linq;

namespace SKO85Core.Work
{
    public class TaskFlow
    {
        public enum TaskFlowState
        {
            Idle = 0,
            Running = 1,
            Done = 2,
            Aborted = 3
        }
        public string Name { get; set; }
        public TaskFlowState State { get; set; }

        public List<Task> Tasks { get; set; }

        public TaskFlow()
        {
            State = TaskFlowState.Idle;
            Tasks = new List<Task>();

            TasksOnRunHandlers = new Dictionary<string, Func<Task, bool>>();
            TaskOnAbortHandlers = new Dictionary<string, Func<Task, bool>>();
            TaskOnDoneHandlers = new Dictionary<string, Func<Task, bool>>();
        }

        public void Run()
        {
            if (this.State == TaskFlowState.Idle)
            {
                this.State = TaskFlowState.Running;
            }

            if (this.State == TaskFlowState.Running)
            {
                var qTasks = Tasks.Where(t => t.State != Task.TaskState.Done && t.State != Task.TaskState.Aborted);

                // Get the next task that needs to be executed.
                var nextTask = qTasks.Where(t => t.IsParallel == false).FirstOrDefault();
                RunTask(nextTask);

                // Get the parallel tasks that need to be executed.
                var parallelTasks = qTasks.Where(t => t.IsParallel == true).ToList();
                foreach (var task in parallelTasks)
                {
                    RunTask(task);
                }
            }
        }

        private void RunTask(Task task)
        {
            if (task != null && task.State != Task.TaskState.Done && task.State != Task.TaskState.Aborted)
            {
                if (TasksOnRunHandlers.ContainsKey(task.Name))
                {
                    if (TasksOnRunHandlers[task.Name](task))
                    {
                        task.State = Task.TaskState.Done;
                    }
                }

                if (task.SubTasks != null && task.SubTasks.Count > 0)
                {
                    var subTasksDone = true;
                    var qSubTasksToRun = task.SubTasks.Where(t => t.State != Task.TaskState.Done && t.State != Task.TaskState.Aborted);
                    var nextSubTask = qSubTasksToRun.Where(t => t.IsParallel == false).FirstOrDefault();

                    if (nextSubTask != null)
                    {
                        RunTask(nextSubTask);
                        if (nextSubTask.State != Task.TaskState.Done)
                        {
                            subTasksDone = false;
                        }
                    }

                    // Get parallel tasks to execute.
                    var parallelSubTasks = qSubTasksToRun.Where(t => t.IsParallel == true).ToList();
                    foreach (var t in parallelSubTasks)
                    {
                        RunTask(t);
                        if (t.State != Task.TaskState.Done)
                        {
                            // If any of the subTasks is not finished, then set this to false so we can set this to the parent Task.
                            subTasksDone = false;
                        }
                    }

                    // Set the main tasks stat to Running if its sub-tasks are still not done.
                    if (subTasksDone == false)
                    {
                        task.State = Task.TaskState.Running;
                    }
                }

                if (task.State == Task.TaskState.Done)
                {
                    if (TaskOnDoneHandlers.ContainsKey(task.Name))
                    {
                        TaskOnDoneHandlers[task.Name](task);
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (var task in Tasks)
            {
                task.State = Task.TaskState.Idle;
            }
            this.State = TaskFlowState.Idle;
        }

        public bool Abort()
        {
            bool isJobAborted = true;
            return isJobAborted;
        }

        public void Add(Task task)
        {
            // We can only add tasks if there are handlers defined for them.
            if (TasksOnRunHandlers.ContainsKey(task.Name))
            {
                this.Tasks.Add(task);
            }
            else
            {
                throw new Exception(string.Format("Task with name '{0}' does not exists in the Task Flow with name '{1}'.", task.Name, this.Name));
            }
        }

        #region Job Events

        public void OnRun(Func<TaskFlow, bool> jobHandler)
        {
        }

        public void OnAbort(Func<TaskFlow, bool> jobHandler)
        {
        }

        public void OnStart(Func<TaskFlow, bool> taskHandler)
        {
        }

        #endregion Job Events

        #region Tasks Events

        public Dictionary<string, Func<Task, bool>> TasksOnRunHandlers { get; set; }
        public Dictionary<string, Func<Task, bool>> TaskOnAbortHandlers { get; set; }
        public Dictionary<string, Func<Task, bool>> TaskOnDoneHandlers { get; set; }

        public void On(string taskName, Func<Task, bool> taskHandler)
        {
            TasksOnRunHandlers[taskName] = taskHandler;
        }

        public void OnTaskAbort(string taskName, Func<Task, bool> taskHandler)
        {
            TaskOnAbortHandlers[taskName] = taskHandler;
        }

        public void OnTaskDone(string taskName, Func<Task, bool> taskHandler)
        {
            TaskOnDoneHandlers[taskName] = taskHandler;
        }

        #endregion Tasks Events
    }
}
