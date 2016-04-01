using System;
using Android.App;
using Android.Runtime;
using Android.Support.V13.App;
using MakeMeMove.Droid.Fragments;

namespace MakeMeMove.Droid.Adapters
{
    public class MainFragmentAdapter : FragmentPagerAdapter
    {
        private readonly ScheduleFragment _scheduleFragment;
        private readonly ExerciseListFragment _exerciseListFragment;

        public MainFragmentAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainFragmentAdapter(FragmentManager fm, Data data) : base(fm)
        {
            _scheduleFragment = new ScheduleFragment();
            _scheduleFragment.Initialize(data);

            _exerciseListFragment = new ExerciseListFragment();
            _exerciseListFragment.Initialize(data);
        }

        public override int Count => 2;
        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 1:
                    return _scheduleFragment;
                case 2:
                    return _exerciseListFragment;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}