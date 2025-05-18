// Build a task scheduler
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PRN212_HW_1
{
    // Simple task priority enum
    public enum TaskPriority
    {
        Low,
        Normal,
        High
    }

    // Interface for task definition
    public interface IScheduledTask
    {
        string Name { get; }
        TaskPriority Priority { get; }
        TimeSpan Interval { get; }
        DateTime LastRun { get; }
        Task ExecuteAsync();
    }

    // A basic implementation of a scheduled task
    public class SimpleTask : IScheduledTask
    {
        private readonly Func<Task> _action;
        private DateTime _lastRun = DateTime.MinValue;

        public string Name { get; }
        public TaskPriority Priority { get; }
        public TimeSpan Interval { get; }

        public DateTime LastRun => _lastRun;

        public SimpleTask(string name, TaskPriority priority, TimeSpan interval, Func<Task> action)
        {
            Name = name;
            Priority = priority;
            Interval = interval;
            _action = action;
        }

        public async Task ExecuteAsync()
        {
            _lastRun = DateTime.Now;
            await _action();
        }
    }

    // The scheduler that students need to implement
    public class TaskScheduler
    {
        // TODO: Implement task queue/storage mechanism

        private List<IScheduledTask> _tasks;

        public TaskScheduler()
        {
            // TODO: Initialize your scheduler

            _tasks = new List<IScheduledTask>();
        }

        public void AddTask(IScheduledTask task)
        {
            // TODO: Add task to the scheduler


            _tasks.Add(task);
        }

        public void RemoveTask(string taskName)
        {
            // TODO: Remove task from the scheduler
            // https://stackoverflow.com/questions/10018957/how-to-remove-item-from-list-in-c
            _tasks.Remove(_tasks.Single(t => t.Name == taskName));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Implement the scheduling logic
            // - Run higher priority tasks first
            // - Only run tasks when their interval has elapsed since LastRun
            // - Keep running until cancellation is requested

            // https://stackoverflow.com/questions/20902248/sorting-a-list-in-c-sharp-using-list-sortcomparisont-comparison
            _tasks.Sort((t1, t2) => - t1.Priority.CompareTo(t2.Priority));

            for (int i = 0; !cancellationToken.IsCancellationRequested; i = (i + 1) % _tasks.Count)
            {
                if (DateTime.Now - _tasks[i].LastRun > _tasks[i].Interval)
                {
                    await _tasks[i].ExecuteAsync();
                }
            }

            // Technically, to get here, the cancellation must have been triggered,
            // if so, throwing OperationCanceledException might be appropriate.

            throw new OperationCanceledException();
        }

        public List<IScheduledTask> GetScheduledTasks()
        {
            return _tasks;
        }
    }

    class HW3
    {
        public static async Task Run(string[] args)
        {
            Console.WriteLine("Task Scheduler Demo");

            // Create the scheduler
            var scheduler = new TaskScheduler();

            // Add some tasks
            scheduler.AddTask(new SimpleTask(
                "High Priority Task",
                TaskPriority.High,
                TimeSpan.FromSeconds(2),
                async () => {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Running high priority task");
                    await Task.Delay(500); // Simulate some work
                }
            ));

            scheduler.AddTask(new SimpleTask(
                "Low Priority Task",
                TaskPriority.Low,
                TimeSpan.FromSeconds(4),
                async () => {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Running low priority task");
                    await Task.Delay(200); // Simulate some work
                }
            ));

            scheduler.AddTask(new SimpleTask(
                "Normal Priority Task",
                TaskPriority.Normal,
                TimeSpan.FromSeconds(3),
                async () => {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Running normal priority task");
                    await Task.Delay(300); // Simulate some work
                }
            ));

            // Create a cancellation token that will cancel after 20 seconds
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            // Or allow the user to cancel with a key press
            Console.WriteLine("Press any key to stop the scheduler...");

            // Run the scheduler in the background
            var schedulerTask = scheduler.StartAsync(cts.Token);

            // Wait for user input

            // Console.ReadKey() is a blocking operation, no matter if we jump back and forth with async/await,
            // the code will never get past this point. A rewrite based on an example from
            // https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/console-teleprompter will be used instead.
            //Console.ReadKey();
            //cts.Cancel();

            try
            {
                // Task.WhenAny itself does not propagate exception, exception handling in async only propagates if the task itself is awaited.
                var completed = await Task.WhenAny(schedulerTask, GetInput(cts));

                // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
                // "Near the end of the code snippet, notice the await finishedTask; expression.
                // The await Task.WhenAny expression doesn't wait on the finished task, but rather waits on the Task object returned by the Task.WhenAny method.
                // The result of the Task.WhenAny method is the completed (or faulted) task. The best practice is to wait on the task again, even when you know the task is complete.
                // In this manner, you can retrieve the task result, or ensure any exception that causes the task to fault is thrown."
                await completed;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Scheduler stopped by cancellation.");
            }

            Console.WriteLine("Scheduler demo finished!");
        }

        private static async Task GetInput(CancellationTokenSource cts)
        {
            Action work = () =>
            {
                Console.ReadKey();
                cts.Cancel();
            };

            await Task.Run(work);
            throw new OperationCanceledException();
        }
    }
}