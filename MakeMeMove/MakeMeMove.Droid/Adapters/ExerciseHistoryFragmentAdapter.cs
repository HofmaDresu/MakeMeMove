using System;
using System.Collections.Generic;
using System.IO;
using Android.Runtime;
using Java.Lang;
using MakeMeMove.Droid.Fragments;
using MakeMeMove.Model;
using SQLite;
using Environment = System.Environment;
using Fragment = Android.App.Fragment;
using FragmentManager = Android.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V13.App.FragmentStatePagerAdapter;

namespace MakeMeMove.Droid.Adapters
{
    public class ExerciseHistoryFragmentAdapter : FragmentStatePagerAdapter
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));
        private DateTime _minimumHistoryDate;

        public ExerciseHistoryFragmentAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            _minimumHistoryDate = _data.GetMinimumExerciseHistoryDate();
        }

        public ExerciseHistoryFragmentAdapter(FragmentManager fm) : base(fm)
        {
            _minimumHistoryDate = _data.GetMinimumExerciseHistoryDate();
        }

        public override int Count => (int)(DateTime.Now.Date - _minimumHistoryDate.Date).TotalDays + 1;
        public override Fragment GetItem(int position)
        {
            var exerciseHistoryForDay = GetExerciseHistoryForPosition(position);
            return new ExerciseHistoryDayFragment(exerciseHistoryForDay);
        }

        private List<ExerciseHistory> GetExerciseHistoryForPosition(int position)
        {
            var targetDate = GetTargetDate(position);
            var exerciseHistoryForDay = _data.GetExerciseHistoryForDay(targetDate);
            return exerciseHistoryForDay;
        }

        private DateTime GetTargetDate(int position)
        {
            return DateTime.Now.Date.AddDays(-1 * ((Count-1) - position));
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(GetTargetDate(position).ToShortDateString());

        }
    }
}