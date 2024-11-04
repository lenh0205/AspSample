> execution time là gì ?
> làm sao ước lượng được những "priority" này chiếm bao nhiêu so với các thread khác ?

# Thread 'Priority'
* -> **a thread's `Priority` property** determines how much **`execution time` it gets relative to other `active threads`** in the operating system

```cs
enum ThreadPriority { Lowest, BelowNormal, Normal, AboveNormal, Highest }
```

## Usage
* -> this becomes **`relevant only` when multiple threads are `simultaneously active`**
* -> it must be cafeful to **elevate a thread’s priority** — it can lead to problems such as **`resource starvation for other threads`**

## Real-time
* -> elevating a thread's priority **doesn't make it capable of performing real-time work**, because it’s still **`throttled by the application's process priority`**
* -> to perform real-time work, we **`must also elevate the process priority`** using the **`Process` class in `System.Diagnostics`**

```cs
using (Process p = Process.GetCurrentProcess())
  p.PriorityClass = ProcessPriorityClass.High;
```

## Process priorty
* -> **ProcessPriorityClass.High** is actually one notch short of the highest priority: **`Realtime`**
* -> **setting a process priority to Realtime** instructs the OS that we **`never want the process to yield CPU time to another process`**
* -> if our **program enters an accidental infinite loop**, we might find **`even the operating system locked out`** (_with nothing short of the power button left to rescue you!_)
* => for this reason, **`High` is usually the best choice for real-time applications**

## Case: real-time application has a user interface
* -> **elevating the process priority** gives **`screen updates excessive CPU time`**, **`slowing down the entire computer`** (_particularly if the UI is complex_)
* => **`lowering the main thread's priority`** in conjunction with **`raising the process’s priority`** ensures that the **real-time thread doesn’t get preempted by screen redraws**

### CPU time starvation
* -> but this doesn’t solve the problem of **`starving other applications of CPU time`**, because the **operating system will still allocate disproportionate resources to the process as a whole**
* => an ideal solution is to **have the `real-time worker` and `user interface` run as `separate applications` with different `process priorities`**, communicating via **`Remoting or memory-mapped files`** 
* => **`Memory-mapped files`** are ideally suited to this task

### For handling hard real-time requirements
* -> even with **an elevated process priority**, there’s a limit to the suitability of the managed environment in handling **`hard real-time requirements`**
* -> in addition to the issues of latency introduced by **automatic garbage collection**, the operating system may present **additional challenges** — even for unmanaged applications 
* => that are best solved with **`dedicated hardware`** or a **`specialized real-time platform`**