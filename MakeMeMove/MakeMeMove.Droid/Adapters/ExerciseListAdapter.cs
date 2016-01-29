using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Adapters
{
    public class ExerciseListAdapter : BaseAdapter<ExerciseBlock>
    {
        public EventHandler<Guid> DeleteExerciseClicked;
        public EventHandler<Guid> EditExerciseClicked;
        private readonly List<ExerciseBlock> _exerciseList;
        private readonly Activity _activity;

        public ExerciseListAdapter(List<ExerciseBlock> exerciseList, Activity activity)
        {
            _exerciseList = exerciseList;
            _activity = activity;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var view = (LinearLayout)(_activity.LayoutInflater.Inflate(Resource.Layout.ExerciseListItem, parent, false));
            var thisExercise = _exerciseList[position];

            view.FindViewById<TextView>(Resource.Id.ExerciseCount).Text = thisExercise.Quantity.ToString();
            view.FindViewById<TextView>(Resource.Id.ExerciseName).Text = thisExercise.CombinedName;

            view.FindViewById<Button>(Resource.Id.DeleteButton).Click += (sender, args) => DeleteExerciseClicked?.Invoke(this, thisExercise.Id.Value);
            view.FindViewById<Button>(Resource.Id.EditButton).Click += (sender, args) => EditExerciseClicked?.Invoke(this, thisExercise.Id.Value);

            return view;
        }

        public override int Count => _exerciseList.Count;

        public override ExerciseBlock this[int position] => _exerciseList[position];
    }
}