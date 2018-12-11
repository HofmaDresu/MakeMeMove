using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Java.Lang;
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

        public MainFragmentAdapter(FragmentManager fm, Context context) : base(fm)
        {
            _scheduleFragment = new ScheduleFragment();
            _scheduleFragment.Initialize(context.Resources.GetString(Resource.String.ScheduleFragmentTitle));

            _exerciseListFragment = new ExerciseListFragment();
            _exerciseListFragment.Initialize(context.Resources.GetString(Resource.String.ExerciseListFragmentTitle));
        }

        public override int Count => 2;
        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return _scheduleFragment;
                case 1:
                    return _exerciseListFragment;
                default:
                    throw new NotImplementedException();
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            var title = (GetItem(position) as BaseMainFragment)?.Title ?? "";
            return new Java.Lang.String(title);
        }
    }
}