using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets
{

    public class ConcurrentQueue<T>
    {
        Queue<T> queue=new Queue<T>();
        public void Enqueue(T data)
        {
            lock (queue)
            {
                queue.Enqueue(data);
            }
        }

        public T Dequeue()
        {
            T result;
            lock (queue)
            {
                result=queue.Dequeue();
            }
            return result;
        }
        public bool IsEmpty
        {
            get
            {
                lock (queue)
                {
                    return queue.Count == 0;
                }
            }
        }

    }

    public interface ITask
    {
        void Run();
    }


    class Task<T> : ITask
    {
        public T Result;
        public Func<T> Execute;
        public bool Executed = false;

        public Task(Func<T> execute)
        {
            this.Execute = execute;
        }

        public void Run()
        {
            Result = Execute();
            Executed = true;
        }

        public T Wait()
        {
            while (!Executed) Thread.Sleep(1);
            return Result;
        }
    }
}
