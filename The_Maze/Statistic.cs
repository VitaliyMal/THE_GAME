using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace The_Maze
{
    public class Statistic
    {
        public int totalSteps;
        public int avgSteps;
        public int totalShortWaySteps;
        public int avgShortWaySteps;
        public int _length;

        public Statistic(int length)
        {
            totalSteps = 0;
            avgSteps = 0;
            totalShortWaySteps = 0;
            avgShortWaySteps = 0;
            _length = length;
        }

        public void avg()
        {
            this.avgSteps=this.totalSteps/_length;
            this.avgShortWaySteps = this.totalShortWaySteps / _length;
        }
    }
}
