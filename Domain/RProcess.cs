
namespace Domain;
public class RProcess
{
    public string ProcessName { get; set; }
    private object locker { get; set; } = new object();

    private TimeOnly _blockStartTime;

    public TimeOnly BlockStartTime
    {
        get { lock (locker) return _blockStartTime; }
        set { lock (locker) _blockStartTime = value; }
    }

    private TimeOnly _blockEndtTime;

    public TimeOnly BlockEndtTime
    {
        get { lock (locker) return _blockEndtTime; }
        set { lock (locker) _blockEndtTime = value; }
    }

    public RProcess()
    {
        BlockStartTime = TimeOnly.Parse("00:00:00");
        BlockEndtTime = TimeOnly.Parse("23:59:59");
        ProcessName = string.Empty;
    }

    public RProcess(string proc)
    {
        BlockStartTime = TimeOnly.Parse("00:00:00");
        BlockEndtTime = TimeOnly.Parse("23:59:59");
        ProcessName = proc;
    }

    public RProcess(string proc, TimeOnly blockStartTime, TimeOnly blockEndtTime)
    {
        ProcessName = proc;
        BlockStartTime = blockStartTime;
        BlockEndtTime = blockEndtTime;
    }
}