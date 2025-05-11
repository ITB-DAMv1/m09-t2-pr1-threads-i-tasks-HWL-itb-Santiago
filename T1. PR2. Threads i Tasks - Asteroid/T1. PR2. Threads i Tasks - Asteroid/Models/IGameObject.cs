using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1._PR2._Threads_i_Tasks___Asteroid.Models
{
    public interface IGameObject
    {
        int Id { get; set; }
        string Name { get; set; }
        int Speed { get; set; }
        int PositionX { get; set; }
        int PositionY { get; set; }
        int DirectionX { get; set; }
        int DirectionY { get; set; }
        char Symbol { get; set; }
        void Move();
        void Draw();
        void ClearDraw();
        void Spawn();
    }
} 