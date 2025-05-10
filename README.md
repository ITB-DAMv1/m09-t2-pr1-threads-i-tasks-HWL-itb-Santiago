# Dining Philosophers Problem

## Descripción del Problema
El problema de los filósofos comensales es un clásico problema de sincronización y concurrencia en sistemas operativos. En este problema, cinco filósofos se sientan alrededor de una mesa circular con un plato de espagueti y cinco palillos. Los filósofos alternan entre pensar y comer. Para comer, un filósofo necesita dos palillos (el de su izquierda y el de su derecha). El desafío es diseñar un algoritmo que permita a los filósofos comer sin que se produzcan deadlocks o inanición.

## Estrategia de Implementación

### Diseño Orientado a Objetos
La solución implementada utiliza un enfoque orientado a objetos para una mejor organización y mantenimiento del código:

1. **Clase Philosopher**:
   - Cada filósofo es una instancia independiente
   - Implementa su propio hilo de ejecución
   - Mantiene su estado (pensando, comiendo, esperando)
   - Gestiona sus propios palillos

2. **Clase Stick**:
   - Representa los recursos compartidos (palillos)
   - Implementa mecanismos de sincronización
   - Mantiene el estado de uso de cada palillo

### Prevención de Deadlocks
Para evitar deadlocks, implementamos la siguiente estrategia:

1. **Orden de Adquisición de Recursos**:
   - Los filósofos siempre toman los palillos en orden ascendente de ID
   - Esto rompe la simetría circular que causa deadlocks
   - El último filósofo en intentar tomar los palillos romperá el ciclo

2. **Timeout en la Adquisición**:
   - Implementamos un tiempo máximo de espera (15 segundos)
   - Si un filósofo no puede obtener los palillos en este tiempo, "muere de hambre"
   - Esto previene la inanición indefinida

### Manejo de Concurrencia

1. **Generación de Números Aleatorios**:
   ```csharp
   private static readonly Random random = new Random();
   private static readonly ThreadLocal<Random> threadRandom = 
       new ThreadLocal<Random>(() => new Random(random.Next()));
   ```
   - Usamos un `Random` estático compartido
   - Implementamos `ThreadLocal<Random>` para evitar colisiones de semilla
   - Cada hilo tiene su propia instancia de `Random` con semilla única

2. **Sincronización de Recursos**:
   ```csharp
   private readonly object stickLock = new object();
   ```
   - Usamos `lock` para proteger el acceso a recursos compartidos
   - Implementamos operaciones atómicas para tomar y soltar palillos
   - Evitamos condiciones de carrera en la escritura por consola

3. **Manejo de Estado**:
   - Los palillos mantienen su estado de forma thread-safe
   - Las operaciones de lectura/escritura están protegidas
   - Implementamos verificaciones de estado seguras

## Documentación Técnica

### Estructura del Proyecto
```
├── Program.cs              # Punto de entrada y configuración
├── Models/
│   ├── Philosopher.cs     # Implementación de filósofos
│   └── Stick.cs           # Implementación de palillos
└── README.md              # Este archivo
```

### Configuración
- Tiempo de pensamiento: 0.5 - 2.0 segundos
- Tiempo de comida: 0.5 - 1.0 segundos
- Tiempo máximo de espera: 15 segundos
- Tiempo entre intentos: 1 segundo

### Referencias Adicionales

#### 1 Libros
- Silberschatz, A., Galvin, P. B., i Gagne, G. (2018). Operating System Concepts. (10th ed.). Wiley.
- Tanenbaum, A. S. i Bos, H. (2014). Modern Operating Systems. (4th ed.). Pearson.
- Lea, D. (1999). Concurrent Programming in Java. (2nd ed.). Addison-Wesley Professional.

#### 2 Artículos Académicos
- Dijkstra, E. W. (1965). The Dining Philosophers Problem. EWD-310. Technological University Eindhoven.
- Chandy, K. M. i Misra, J. (1984). The Drinking Philosophers Problem. ACM Transactions on Programming Languages and Systems, 6(4), 632-646.
- Lehmann, D. i Rabin, M. O. (1981). A New Solution to the Dining Philosophers Problem. Information Processing Letters, 12(5), 227-229.

#### 3 Recursos Online
- Wikipedia contributors. (2024, 15 de març). Dining philosophers problem. Wikipedia. Recuperat el 15 de març 2024 de https://en.wikipedia.org/wiki/Dining_philosophers_problem
- Microsoft. (2024, 15 de març). Threading in C# and .NET. Microsoft Docs. Recuperat el 15 de març 2024 de https://docs.microsoft.com/en-us/dotnet/standard/threading/
- Oracle. (2024, 15 de març). Lesson: Concurrency. The Java™ Tutorials. Recuperat el 15 de març 2024 de https://docs.oracle.com/javase/tutorial/essential/concurrency/

## Ejecución
1. Compilar el proyecto
2. Ejecutar el programa
3. Observar la simulación en tiempo real
4. Usar Ctrl+C para detener la simulación

## Notas de Implementación
- La solución prioriza la prevención de deadlocks sobre la eficiencia
- Se implementa un sistema de timeout para evitar inanición
- La visualización en consola está optimizada para mostrar el estado de cada filósofo
- Se mantiene un registro de comidas y fallos para análisis 

## Desarrollo con Asistencia de IA

Durante el desarrollo de este proyecto, utilicé una IA para resolver dudas y mejorar el código. Aquí están algunas de las interacciones más relevantes:

### 1. Mejora de la Sincronización
**Pregunta**: "¿Cómo puedo evitar que los filósofos se bloqueen entre sí cuando intentan tomar los palillos?"

**Respuesta de la IA**:
```
Para evitar deadlocks, implementa estas estrategias:
1. Orden de adquisición de recursos: siempre toma los palillos en orden ascendente de ID
2. Timeout en la adquisición: si un filósofo no puede obtener los palillos en 15 segundos, "muere de hambre"
3. Usa lock para proteger el acceso a los palillos
```

**Resultado**: Implementé el orden de adquisición de recursos y el sistema de timeout, lo que resolvió los problemas de deadlock.

### 2. Optimización de la Generación de Números Aleatorios
**Pregunta**: "¿Por qué mis filósofos están pensando y comiendo exactamente el mismo tiempo?"

**Respuesta de la IA**:
```
El problema es que estás creando una nueva instancia de Random en cada llamada. En su lugar, usa:
```csharp
private static readonly Random random = new Random();
private static readonly ThreadLocal<Random> threadRandom = 
    new ThreadLocal<Random>(() => new Random(random.Next()));
```
Esto evita colisiones de semilla entre hilos.
```

**Resultado**: La implementación de ThreadLocal<Random> resolvió el problema de la generación de números aleatorios.

### 3. Mejora de la Visualización
**Pregunta**: "¿Cómo puedo hacer que la salida en consola sea más clara y organizada?"

**Respuesta de la IA**:
```
Para mejorar la visualización, implementa este código en Program.cs:

```csharp
// Configuración de la consola
Console.Title = "Dining Philosophers Simulation";
Console.Clear();
Console.WindowHeight = 40;
Console.BufferHeight = 40;

// Dibujar el encabezado
Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                        DINING PHILOSOPHERS SIMULATION                      ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");

// Mostrar configuración
Console.WriteLine("\nConfiguración:");
Console.WriteLine($"  • Tiempo de pensamiento: {MIN_THINK_TIME/1000.0} - {MAX_THINK_TIME/1000.0} segundos");
Console.WriteLine($"  • Tiempo de comida: {MIN_EAT_TIME/1000.0} - {MAX_EAT_TIME/1000.0} segundos");
Console.WriteLine($"  • Tiempo máximo de espera: {STARVING_TIME/1000.0} segundos");

// Mostrar controles
Console.WriteLine("\nControles:");
Console.WriteLine("  • Presiona 'P' para pausar/reanudar");
Console.WriteLine("  • Presiona 'Q' para salir");
Console.WriteLine("  • Ctrl+C para detener la simulación");

// Preparar sección de filósofos
Console.WriteLine("\nEstado de los Filósofos:");
Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║ ID │ Estado      │ Tiempo    │ Comidas │ Fallos │ Palillos                ║");
Console.WriteLine("╟────┼─────────────┼───────────┼─────────┼────────┼─────────────────────────╢");

// Dejar espacio para los filósofos
for (int i = 0; i < NUM_PHILOSOPHERS; i++)
{
    Console.WriteLine("║    │             │           │         │        │                         ║");
}

Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
```

Y en Philosopher.cs, implementa este método para actualizar el estado:

```csharp
private void UpdateStatusLine()
{
    lock (consoleLock)
    {
        Console.SetCursorPosition(0, 12 + Id);
        Console.Write($"║ {Id,2} │ ");
        
        // Estado con color
        Console.ForegroundColor = State switch
        {
            PhilosopherState.Thinking => ConsoleColor.Blue,
            PhilosopherState.Eating => ConsoleColor.Green,
            PhilosopherState.Waiting => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };
        Console.Write($"{State,-11}");
        Console.ResetColor();
        
        // Resto de la información
        Console.Write($" │ {TimeInState/1000,7:F1}s │ {MealsEaten,7} │ {FailedAttempts,6} │ ");
        
        // Mostrar palillos con colores
        if (LeftStick != null)
        {
            Console.ForegroundColor = LeftStick.IsUsedBy(Id) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("L");
            Console.ResetColor();
        }
        if (RightStick != null)
        {
            Console.ForegroundColor = RightStick.IsUsedBy(Id) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("R");
            Console.ResetColor();
        }
        Console.WriteLine("                         ║");
    }
}
```

Esto creará una interfaz con:
1. Un encabezado claro con el título
2. Sección de configuración con los tiempos
3. Sección de controles
4. Tabla de filósofos con:
   - ID
   - Estado (con colores)
   - Tiempo en el estado actual
   - Número de comidas
   - Intentos fallidos
   - Estado de los palillos (verde = en uso, rojo = libre)

**Resultado**: La implementación de este código resultó en una interfaz mucho más clara y profesional, donde:
- Cada filósofo tiene su propia línea fija en la consola
- Los estados se muestran con colores diferentes (azul = pensando, verde = comiendo, amarillo = esperando)
- Los palillos se muestran con colores (verde = en uso, rojo = libre)
- La información está organizada en una tabla con bordes
- No hay solapamiento de información entre filósofos

### 4. Manejo de Recursos
**Pregunta**: "¿Cómo puedo hacer que el código sea más seguro en términos de concurrencia?"

**Respuesta de la IA**:
```
Implementa estas mejoras:
1. Haz las propiedades de Stick privadas y usa métodos para acceder
2. Usa lock consistentemente para todas las operaciones de lectura/escritura
3. Implementa verificaciones de estado seguras
4. Añade un método IsUsedBy para verificar el uso de palillos
```

**Resultado**: El código es ahora más robusto y seguro en términos de concurrencia.

### Lecciones Aprendidas
1. La importancia de la sincronización en programas concurrentes
2. Cómo evitar deadlocks mediante el orden de adquisición de recursos
3. La necesidad de manejar correctamente la generación de números aleatorios en hilos
4. La importancia de una buena visualización para el debugging
5. Cómo usar locks efectivamente para proteger recursos compartidos

Esta interacción con la IA me ayudó a entender mejor los conceptos de concurrencia y a implementar una solución más robusta y eficiente. 