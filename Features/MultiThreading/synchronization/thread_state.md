
# ThreadState
* -> we can query **`a thread's execution status`** via its **ThreadState** property
* -> this returns **`a flags enum of type ThreadState`**, which combines **three "layers" of data in a bitwise fashion**
* -> however, **most values are redundant, unused, or deprecated**

## Note
* -> the **ThreadState** property is **`useful for diagnostic purposes`**
* -> but **`unsuitable for synchronization`**, because **a thread's state may change in between testing ThreadState** and **acting on that information**

## common used value
* -> the following code strips a "ThreadState" to one of the **four most useful values**: **`Unstarted`**, **`Running`**, **`WaitSleepJoin`**, and **`Stopped`**

```cs
public static ThreadState SimpleThreadState (ThreadState ts)
{
  return ts & (ThreadState.Unstarted |
               ThreadState.WaitSleepJoin |
               ThreadState.Stopped);
}
```

