using System;
using System.Collections.Concurrent;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    private ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

    public void Push(Action job)
    {
        _queue.Enqueue(job);
    }

    public void PopAll()
    {
        while (_queue.TryDequeue(out var job))
        {
            job.Invoke();
        }
    }
}