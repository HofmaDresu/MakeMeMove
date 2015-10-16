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
        Squats,
        CalfRaises,
        Lunges
    }

    public class ExerciseBlock
    {
        private Guid? _id;

        public Guid Id
        {
            get
            {
                if (!_id.HasValue)
                {
                    _id = Guid.NewGuid();
                }
                return _id.Value;
            }
            set { _id = value; }
        }

        public string IdString => Id.ToString();

        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
