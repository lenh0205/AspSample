==================================================================
# Blocking
* -> a thread is deemed **`blocked`** when **its execution is paused** for some reason, _such as when `Sleeping` or waiting for another to end via `Join` or `EndInvoke`_

* -> **a blocked thread** **`immediately 'yields' its processor time slice`**
* -> and from then on **`consumes no processor time`** until its blocking condition is satisfied
* -> we can **test for a thread being blocked** via its **`ThreadState`** property 
* _(Given that **a thread’s state** `may change in between testing its state` and then acting upon that information, this code is useful only in diagnostic scenarios)_

```cs
bool blocked = (someThread.ThreadState & ThreadState.WaitSleepJoin) != 0;
```

## context switch
* -> **`when a thread blocks or unblocks`**, the **operating system** performs **a context switch**
* -> this incurs **`an overhead of a few microseconds`**

## Unblocking 
* _Unblocking happens in one of four ways (the computer's power button doesn't count!):_
* -> by **`the blocking condition being satisfied`**
* -> by **`the operation timing out`** (_if a timeout is specified_)
* -> by being interrupted via **`Thread.Interrupt`**
* -> by being aborted via **`Thread.Abort`**

* _**a thread is not deemed blocked** if its execution is paused via the (deprecated) **`Suspend`** method_

==================================================================
# Blocking Versus Spinning
* -> sometimes **a thread must pause until a certain condition is met**
* -> **`signaling`** and **`locking`** constructs achieve this efficiently by **blocking until a condition is satisfied**
* -> however, there is a simpler alternative: **a thread can await a condition** by **`spinning in a polling loop`**

* => **spinning very briefly** can be **`effective when we expect a condition to be satisfied soon`** (perhaps within a few microseconds) 
* -> because it **avoids the `overhead` and `latency` of `a context switch`**
* -> the **.NET Framework** provides special methods and classes to assist (_these are covered in the `parallel programming`_)

```cs
while (!proceed);
// or
while (DateTime.Now < nextStartTime);
```

## Problem
* -> In general, this is very wasteful on processor time: as far as the CLR and operating system are concerned, the thread is performing an important calculation, and so gets allocated resources accordingly!

## Hybrid between 'blocking' and 'spinning'
* -> sometimes **a hybrid between 'blocking' and 'spinning'** is used
* -> although inelegant, this is (in general) **`far more efficient than outright spinning`**
* -> though problems can arise due to **concurrency issues** on the **`proceed`** flag
* => proper use of **`locking`** and **`signaling`** avoids this

```cs
while (!proceed) Thread.Sleep (10);
```