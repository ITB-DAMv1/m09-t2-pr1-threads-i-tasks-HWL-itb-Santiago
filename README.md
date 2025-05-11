# Proyectos de Concurrencia: Filósofos Comensales y Juego de Asteroides

## Descripción General
Este repositorio contiene dos proyectos que demuestran diferentes aspectos de la programación concurrente en C#:
1. El problema clásico de los filósofos comensales
2. Un juego de Asteroides con programación concurrente

## 1. Problema de los Filósofos Comensales

### Descripción del Problema
El problema de los filósofos comensales es un clásico problema de sincronización y concurrencia en sistemas operativos. En este problema, cinco filósofos se sientan alrededor de una mesa circular con un plato de espagueti y cinco palillos. Los filósofos alternan entre pensar y comer. Para comer, un filósofo necesita dos palillos (el de su izquierda y el de su derecha). El desafío es diseñar un algoritmo que permita a los filósofos comer sin que se produzcan deadlocks o inanición.

### Estrategia de Implementación

#### Diseño Orientado a Objetos
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

## 2. Juego de Asteroides

### Descripción
Este proyecto implementa un juego clásico de Asteroides utilizando programación concurrente en C#. El juego demuestra el uso efectivo de threads y tasks para manejar diferentes aspectos del juego como el movimiento, la renderización y la entrada del usuario.

### Características Principales
- Sistema de doble buffer para renderizado suave
- Movimiento fluido de la nave y asteroides
- Sistema de colisiones en tiempo real
- Aumento progresivo de dificultad
- Pantallas de inicio y game over
- Gestión eficiente de recursos

### Estructura del Proyecto

#### Clases Principales

##### Game
- Clase principal que gestiona el estado del juego
- Implementa el patrón Singleton
- Maneja el bucle principal del juego
- Coordina las tareas de dibujo y movimiento
- Gestiona las transiciones entre estados (inicio, juego, game over)

##### Ship
- Representa la nave del jugador
- Maneja el movimiento horizontal
- Detecta colisiones con asteroides
- Implementa el sistema de respawn

##### Asteroid
- Representa los asteroides enemigos
- Sistema de movimiento diagonal
- Sistema de rastro visual
- Respawn automático al salir de la pantalla

##### GameBuffer
- Implementa el sistema de doble buffer
- Gestiona la renderización en consola
- Optimiza el rendimiento visual

##### InputSystem
- Maneja la entrada del usuario
- Implementa el patrón Singleton
- Procesa las teclas de forma asíncrona

### Patrones de Diseño Utilizados

#### Singleton
- Game
- GameBuffer
- InputSystem
- CollisionSystem

#### Observer
- Sistema de detección de colisiones
- Notificación de cambios de estado

#### State
- Gestión de estados del juego (inicio, juego, game over)

### Concurrencia

#### Tasks
- Task de dibujo (50ms)
- Task de movimiento (20ms)
- Task de entrada de usuario

#### Sincronización
- Locks para acceso a recursos compartidos
- Control de concurrencia en el buffer de juego
- Sincronización de estados

## Desarrollo con Asistencia de IA

### Filósofos Comensales

#### 1. Mejora de la Sincronización
**Pregunta**: "¿Cómo puedo evitar que los filósofos se bloqueen entre sí cuando intentan tomar los palillos?"

**Respuesta de la IA**:
```
Para evitar deadlocks, implementa estas estrategias:
1. Orden de adquisición de recursos: siempre toma los palillos en orden ascendente de ID
2. Timeout en la adquisición: si un filósofo no puede obtener los palillos en 15 segundos, "muere de hambre"
3. Usa lock para proteger el acceso a los palillos
```

**Resultado**: Implementé el orden de adquisición de recursos y el sistema de timeout, lo que resolvió los problemas de deadlock.

#### 2. Optimización de la Generación de Números Aleatorios
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

### Juego de Asteroides

#### 1. Sistema de Doble Buffer
**Pregunta**: "¿Cómo puedo implementar un sistema de doble buffer para evitar el parpadeo en la consola?"

**Respuesta de la IA**:
```
Para implementar un sistema de doble buffer efectivo, te recomiendo:
- Crear una clase `GameBuffer` que mantenga dos buffers
- Usar un buffer para dibujar y otro para mostrar
- Intercambiar los buffers después de cada frame
- Implementar el patrón Singleton para garantizar una única instancia
- Sincronizar el acceso con locks para evitar problemas de concurrencia
```

**Resultado**: Implementé el sistema de doble buffer que eliminó el parpadeo y mejoró significativamente la fluidez visual.

#### 2. Manejo de Entrada
**Pregunta**: "¿Cuál es la mejor manera de manejar la entrada del usuario sin bloquear el bucle principal?"

**Respuesta de la IA**:
```
La mejor solución es implementar un sistema asíncrono de entrada:
- Crear una clase `InputSystem` con un patrón Singleton
- Usar una cola para almacenar las teclas presionadas
- Implementar un método `TryGetInput` para procesar la entrada
- Manejar la entrada en una task separada
- Sincronizar el acceso a la cola de entrada
```

**Resultado**: El sistema de entrada asíncrono mejoró significativamente la responsividad del juego.

#### 3. Sistema de Colisiones
**Pregunta**: "¿Cómo puedo implementar un sistema de colisiones eficiente?"

**Respuesta de la IA**:
```
Para un sistema de colisiones eficiente:
- Implementar el patrón Observer para notificar colisiones
- Verificar colisiones solo cuando sea necesario
- Usar una copia de la lista de asteroides para evitar modificaciones durante la iteración
- Proporcionar feedback visual inmediato
- Manejar las colisiones de forma asíncrona
```

**Resultado**: El sistema de colisiones implementado es eficiente y proporciona feedback visual inmediato.

## Bibliografía

### 1. Libros
- Silberschatz, A., Galvin, P. B., i Gagne, G. (2018). Operating System Concepts. (10th ed.). Wiley.
- Tanenbaum, A. S. i Bos, H. (2014). Modern Operating Systems. (4th ed.). Pearson.
- Freeman, E., & Robson, E. (2004). "Head First Design Patterns". O'Reilly Media.
- Albahari, J., & Albahari, B. (2020). "C# 8.0 in a Nutshell". O'Reilly Media.
- Gamma, E., Helm, R., Johnson, R., & Vlissides, J. (1994). "Design Patterns: Elements of Reusable Object-Oriented Software". Addison-Wesley.

### 2. Artículos Académicos
- Dijkstra, E. W. (1965). The Dining Philosophers Problem. EWD-310. Technological University Eindhoven.
- Chandy, K. M. i Misra, J. (1984). The Drinking Philosophers Problem. ACM Transactions on Programming Languages and Systems, 6(4), 632-646.
- Lehmann, D. i Rabin, M. O. (1981). A New Solution to the Dining Philosophers Problem. Information Processing Letters, 12(5), 227-229.

### 3. Recursos Online
- Microsoft. (2023). "Task-based asynchronous programming". Microsoft Learn.
  https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
- Microsoft. (2023). "Threading in C#". Microsoft Learn.
  https://learn.microsoft.com/en-us/dotnet/standard/threading/
- Wikipedia contributors. (2024). Dining philosophers problem. Wikipedia.
  https://en.wikipedia.org/wiki/Dining_philosophers_problem

## Requisitos Técnicos
- .NET 6.0 o superior
- Consola de Windows
