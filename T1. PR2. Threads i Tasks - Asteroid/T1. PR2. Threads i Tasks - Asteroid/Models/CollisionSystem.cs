using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public class CollisionSystem
    {
        private static readonly object _lock = new object();
        private static CollisionSystem _instance;

        public static CollisionSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CollisionSystem();
                    }
                }
                return _instance;
            }
        }

        private CollisionSystem() { }

        public bool CheckCollision(IGameObject obj1, IGameObject obj2)
        {
            return obj1.PositionX == obj2.PositionX && obj1.PositionY == obj2.PositionY;
        }

        public void CheckCollisions(IEnumerable<IGameObject> objects, Action<IGameObject, IGameObject> onCollision)
        {
            var objectList = new List<IGameObject>(objects);
            for (int i = 0; i < objectList.Count; i++)
            {
                for (int j = i + 1; j < objectList.Count; j++)
                {
                    if (CheckCollision(objectList[i], objectList[j]))
                    {
                        onCollision?.Invoke(objectList[i], objectList[j]);
                    }
                }
            }
        }
    }
} 