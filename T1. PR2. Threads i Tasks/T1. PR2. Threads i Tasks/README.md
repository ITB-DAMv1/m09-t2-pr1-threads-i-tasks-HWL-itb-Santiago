# Dining Philosophers Problem

## Documentación Técnica Detallada

### 1. Soluciones Clásicas al Problema

#### 1.1 Solución de Dijkstra (1965)
- **Concepto**: Uso de semáforos para controlar el acceso a los palillos
- **Implementación**:
  ```csharp
  private SemaphoreSlim[] sticks;
  private SemaphoreSlim room;
  
  // Inicialización
  sticks = new SemaphoreSlim[NUM_PHILOSOPHERS];
  for (int i = 0; i < NUM_PHILOSOPHERS; i++)
      sticks[i] = new SemaphoreSlim(1, 1);
  room = new SemaphoreSlim(NUM_PHILOSOPHERS - 1, NUM_PHILOSOPHERS - 1);
  ```
- **Ventajas**: Simple de entender, garantiza que al menos un filósofo puede comer
- **Desventajas**: Puede causar inanición

#### 1.2 Solución de Chandy/Misra (1984)
- **Concepto**: Asignación dinámica de prioridades
- **Implementación**:
  ```csharp
  private class Stick
  {
      private int owner;
      private bool dirty;
      
      public void Request(int philosopherId)
      {
          if (dirty)
          {
              owner = philosopherId;
              dirty = false;
          }
      }
  }
  ```
- **Ventajas**: Evita deadlocks e inanición
- **Desventajas**: Más compleja de implementar

#### 1.3 Solución con Monitor
- **Concepto**: Uso de `Monitor.Enter` y `Monitor.Exit`
- **Implementación**:
  ```csharp
  private object[] stickLocks;
  
  // Inicialización
  stickLocks = new object[NUM_PHILOSOPHERS];
  for (int i = 0; i < NUM_PHILOSOPHERS; i++)
      stickLocks[i] = new object();
  ```
- **Ventajas**: Más flexible que semáforos
- **Desventajas**: Requiere manejo cuidadoso de excepciones

### 2. Patrones de Diseño Aplicables

#### 2.1 Patrón Monitor
```csharp
public class Stick
{
    private readonly object monitor = new object();
    private bool isAvailable = true;
    
    public bool TryAcquire()
    {
        lock (monitor)
        {
            if (isAvailable)
            {
                isAvailable = false;
                return true;
            }
            return false;
        }
    }
}
```

#### 2.2 Patrón Resource Manager
```csharp
public class ResourceManager
{
    private readonly Dictionary<int, Stick> resources;
    private readonly object resourceLock = new object();
    
    public bool TryAcquireResources(int philosopherId)
    {
        lock (resourceLock)
        {
            // Lógica de adquisición de recursos
        }
    }
}
```

### 3. Estrategias de Prevención de Deadlocks

#### 3.1 Orden de Adquisición de Recursos
```csharp
public bool TryAcquireSticks()
{
    int firstStick = Math.Min(LeftStickId, RightStickId);
    int secondStick = Math.Max(LeftStickId, RightStickId);
    
    if (Monitor.TryEnter(stickLocks[firstStick], Timeout))
    {
        try
        {
            if (Monitor.TryEnter(stickLocks[secondStick], Timeout))
                return true;
        }
        finally
        {
            Monitor.Exit(stickLocks[firstStick]);
        }
    }
    return false;
}
```

#### 3.2 Timeout y Reintentos
```csharp
public bool TryAcquireWithTimeout(int timeout)
{
    var stopwatch = Stopwatch.StartNew();
    while (stopwatch.ElapsedMilliseconds < timeout)
    {
        if (TryAcquireSticks())
            return true;
        Thread.Sleep(100);
    }
    return false;
}
```

### 4. Métricas y Monitoreo

#### 4.1 Métricas de Rendimiento
```csharp
public class PhilosopherMetrics
{
    public int TotalMeals { get; private set; }
    public int FailedAttempts { get; private set; }
    public long TotalWaitingTime { get; private set; }
    public long MaxWaitingTime { get; private set; }
    
    public void RecordMeal()
    {
        Interlocked.Increment(ref TotalMeals);
    }
    
    public void RecordFailedAttempt(long waitingTime)
    {
        Interlocked.Increment(ref FailedAttempts);
        Interlocked.Add(ref TotalWaitingTime, waitingTime);
        Interlocked.Exchange(ref MaxWaitingTime, 
            Math.Max(MaxWaitingTime, waitingTime));
    }
}
```

#### 4.2 Monitoreo de Estado
```csharp
public class SimulationMonitor
{
    private readonly Philosopher[] philosophers;
    private readonly Timer monitorTimer;
    
    public SimulationMonitor(Philosopher[] philosophers)
    {
        this.philosophers = philosophers;
        monitorTimer = new Timer(MonitorCallback, null, 0, 1000);
    }
    
    private void MonitorCallback(object state)
    {
        // Análisis de estado y métricas
    }
}
```

### 5. Referencias Adicionales

#### 5.1 Libros
- Silberschatz, A., Galvin, P. B., i Gagne, G. (2018). Operating System Concepts. (10th ed.). Wiley.
- Tanenbaum, A. S. i Bos, H. (2014). Modern Operating Systems. (4th ed.). Pearson.
- Lea, D. (1999). Concurrent Programming in Java. (2nd ed.). Addison-Wesley Professional.

#### 5.2 Artículos Académicos
- Dijkstra, E. W. (1965). The Dining Philosophers Problem. EWD-310. Technological University Eindhoven.
- Chandy, K. M. i Misra, J. (1984). The Drinking Philosophers Problem. ACM Transactions on Programming Languages and Systems, 6(4), 632-646.
- Lehmann, D. i Rabin, M. O. (1981). A New Solution to the Dining Philosophers Problem. Information Processing Letters, 12(5), 227-229.

#### 5.3 Recursos Online
- Wikipedia contributors. (2024, 15 de març). Dining philosophers problem. Wikipedia. Recuperat el 15 de març 2024 de https://en.wikipedia.org/wiki/Dining_philosophers_problem
- Microsoft. (2024, 15 de març). Threading in C# and .NET. Microsoft Docs. Recuperat el 15 de març 2024 de https://docs.microsoft.com/en-us/dotnet/standard/threading/
- Oracle. (2024, 15 de març). Lesson: Concurrency. The Java™ Tutorials. Recuperat el 15 de març 2024 de https://docs.oracle.com/javase/tutorial/essential/concurrency/

### 6. Ejercicios Prácticos

#### 6.1 Ejercicio 1: Implementación Básica
Implementar una solución básica usando semáforos y comparar el rendimiento con diferentes números de filósofos.

#### 6.2 Ejercicio 2: Análisis de Deadlocks
Crear un escenario que cause deadlocks y analizar cómo la solución propuesta los evita.

#### 6.3 Ejercicio 3: Optimización
Implementar diferentes estrategias de adquisición de recursos y comparar su rendimiento.

### 7. Herramientas de Desarrollo

#### 7.1 Debugging
- Visual Studio Debugger
- Concurrency Visualizer
- Thread Debugging Tools

#### 7.2 Profiling
- Visual Studio Profiler
- dotTrace
- PerfView

#### 7.3 Testing
- Unit Testing con NUnit
- Stress Testing
- Concurrency Testing

Esta documentación proporciona una base sólida para entender y resolver el problema de los filósofos comensales, incluyendo implementaciones prácticas, patrones de diseño, y herramientas de desarrollo. 