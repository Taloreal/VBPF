namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;

    [Serializable]
    public class DailyRoutine
    {
        public SortedList Tasks = new SortedList();

        public bool CheckConflicts(Task task)
        {
            foreach (Task task2 in this.Tasks.Values)
            {
                if (task2.ID != task.ID)
                {
                    for (int i = task.StartPeriod; i < (task.StartPeriod + task.Duration); i++)
                    {
                        for (int j = task2.StartPeriod; j < (task2.StartPeriod + task2.Duration); j++)
                        {
                            if ((i % 0x30) == (j % 0x30))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            foreach (Task task2 in this.Tasks.Values)
            {
                if ((task.Building != task2.Building) && ((task.StartPeriod == task2.EndPeriod) || (task.EndPeriod == task2.StartPeriod)))
                {
                    return false;
                }
            }
            return true;
        }

        public void CheckHoursConflict(Task task)
        {
            foreach (Task task2 in this.Tasks.Values)
            {
                if (task2.ID != task.ID)
                {
                    for (int i = task.StartPeriod; i < (task.StartPeriod + task.Duration); i++)
                    {
                        for (int j = task2.StartPeriod; j < (task2.StartPeriod + task2.Duration); j++)
                        {
                            if ((i % 0x30) == (j % 0x30))
                            {
                                throw new SimApplicationException(A.R.GetString("Those hours conflict with other activities in your daily routine. Please adjust other activities to make room."));
                            }
                        }
                    }
                }
            }
            foreach (Task task2 in this.Tasks.Values)
            {
                if ((task.Building != task2.Building) && ((task.StartPeriod == task2.EndPeriod) || (task.EndPeriod == task2.StartPeriod)))
                {
                    throw new SimApplicationException(A.R.GetString("You must allow at least a half hour between activities at different locations. Please adjust your schedule."));
                }
            }
        }

        public Task GetCurrentTask()
        {
            Task task = null;
            foreach (Task task2 in this.Tasks.Values)
            {
                if (task2.StartPeriod < task2.EndPeriod)
                {
                    if (((task2.StartPeriod <= A.ST.Period) && (A.ST.Period < task2.EndPeriod)) && (task2.DayLastStarted != A.ST.Day))
                    {
                        task = task2;
                        task.DayLastStarted = A.ST.Day;
                    }
                }
                else if ((task2.StartPeriod <= A.ST.Period) && (task2.DayLastStarted != A.ST.Day))
                {
                    task = task2;
                    task.DayLastStarted = A.ST.Day;
                }
                else if ((A.ST.Period < task2.EndPeriod) && (task2.DayLastStarted != A.ST.Now.AddDays(-1.0).Day))
                {
                    task = task2;
                    task.DayLastStarted = A.ST.Now.AddDays(-1.0).Day;
                }
            }
            return task;
        }

        public DailyRoutine MakeCopy()
        {
            return new DailyRoutine { Tasks = (SortedList) this.Tasks.Clone() };
        }

        public Task PriorTask(Task task)
        {
            int num = this.Tasks.IndexOfValue(task);
            if (num == 0)
            {
                return (Task) this.Tasks.GetByIndex(this.Tasks.Count - 1);
            }
            return (Task) this.Tasks.GetByIndex(num - 1);
        }
    }
}

