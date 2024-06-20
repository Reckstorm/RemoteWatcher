using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Domain;

public class Blocker
{
    private static Blocker _instance = null;
    private List<RProcess> _rProcessList { get; set; }
    private static object _locker { get; set; } = new object();
    private bool _running;
    public bool running
    {
        get { lock (_locker) return _running; }
        set { lock (_locker) _running = value; }
    }

    private Blocker()
    {
    }

    public static Blocker GetInstance(List<RProcess> rProcesses)
    {
        lock (_locker)
        {
            if (_instance != null) return _instance;
            _instance = new Blocker
            {
                _rProcessList = rProcesses
            };
            CheckIfBGRunning();
            // RegistryAgent.AddToStartup();
            return _instance;
        }
    }

    public static Blocker GetInstance()
    {
        lock (_locker)
        {
            return _instance;
        }
    }

    private static void CheckIfBGRunning()
    {
        if (_instance._rProcessList == null) return;
        Process temp = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Equals(AppDomain.CurrentDomain.FriendlyName));
        if (temp != null && temp.Id != Process.GetCurrentProcess().Id) temp.Kill();
    }

    public async Task StopBlock()
    {
        await Task.Run(() =>
        {
            if (running)
            {
                running = false;
            }
        });
    }

    public void RunBlock()
    {
        if (!running)
        {
            running = true;
            RestartIfRulesChanged();
            Task.Run(async () =>
            {
                while (running)
                {
                    var processes = Process.GetProcesses().ToList();
                    foreach (Process process in processes)
                    {
                        foreach (RProcess p in _rProcessList)
                        {
                            if (p.ProcessName.Equals(AppDomain.CurrentDomain.FriendlyName)) continue;
                            if (p.ProcessName.Equals(process.ProcessName)
                            && ((TimeOnly.Parse(DateTime.Now.ToLongTimeString()) <= p.BlockEndtTime
                            && TimeOnly.Parse(DateTime.Now.ToLongTimeString()) >= p.BlockStartTime) 
                            || (TimeOnly.Parse(DateTime.Now.ToLongTimeString()) >= p.BlockEndtTime
                            && p.BlockStartTime >= p.BlockEndtTime)))
                            {
                                foreach (Process temp in Process.GetProcessesByName(p.ProcessName))
                                {
                                    temp.Kill();
                                }
                                await WriteLogs(p.ProcessName);
                            }
                        }
                    }
                    Thread.Sleep(200);
                }
            });
        }
    }

    private async Task RestartBlocker()
    {
        await Task.Run(async () =>
        {
            await StopBlock();
            RunBlock();
        });
    }

    private async void RestartIfRulesChanged()
    {
        var rules = await RegistryAgent.GetRules();
        Task task = Task.Run(async () =>
        {
            while (running)
            {
                var newRules = await RegistryAgent.GetRules();
                if (newRules != rules)
                {
                    rules = newRules;
                    _rProcessList = JsonSerializer.Deserialize<List<RProcess>>(rules);
                    await RestartBlocker();
                }
                Thread.Sleep(200);
            }
        });
    }

    private async Task WriteLogs(string processName)
    {
        if (!Directory.Exists($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Logs"))
            Directory.CreateDirectory($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Logs");
        var path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Logs\\{DateTime.Now.ToShortDateString()}.txt";
        await File.AppendAllTextAsync(path, $"Killed: {processName} at {DateTime.Now.ToLongTimeString()}\n");
    }
}