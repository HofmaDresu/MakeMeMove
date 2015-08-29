using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMeMove.Model
{
    public enum PreBuiltExersises
    {
        PushUps,
        SitUps,
        JumpingJacks,
        Squats
    }

    public class ExerciseBlock
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
