using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Objects.Entities
{
    internal class Wall : Entity
    {
        public Wall(RectangleF CurrentSquare) : base(CurrentSquare)
        {
            curSquare = CurrentSquare;            
        }
    }
}
