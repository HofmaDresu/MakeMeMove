﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using SQLite;

namespace MakeMeMove.Model
{
    public enum PreBuiltExersises
    {
        PushUps,
        SitUps,
        JumpingJacks,
        Squats,
        CalfRaises,
        Lunges,
        Custom
    }

    [Table("ExerciseBlocks")]
    public class ExerciseBlock
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        [Ignore]
        public string CombinedName => string.IsNullOrWhiteSpace(Name) ? Type.Humanize() : Name;


        
        public PreBuiltExersises Type { get; set; }
        public int Quantity { get; set; }
        public bool Enabled { get; set; }

        public static List<ExerciseBlock> CreateDefaultExercises()
        {
            return new List<ExerciseBlock>
            {
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.PushUps.Humanize(),
                    Type = PreBuiltExersises.PushUps,
                    Quantity = 10,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.SitUps.Humanize(),
                    Type = PreBuiltExersises.SitUps,
                    Quantity = 10,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.JumpingJacks.Humanize(),
                    Type = PreBuiltExersises.JumpingJacks,
                    Quantity = 10,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.Squats.Humanize(),
                    Type = PreBuiltExersises.Squats,
                    Quantity = 10,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.CalfRaises.Humanize(),
                    Type = PreBuiltExersises.CalfRaises,
                    Quantity = 10,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.Lunges.Humanize(),
                    Type = PreBuiltExersises.Lunges,
                    Quantity = 5,
                    Enabled = true
                }
            };
        } 
    }
}
