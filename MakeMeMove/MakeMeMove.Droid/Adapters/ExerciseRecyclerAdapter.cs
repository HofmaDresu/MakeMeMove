using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.ViewHolders;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Adapters
{
    public class ExerciseRecyclerAdapter : RecyclerView.Adapter
    {
        public EventHandler<Guid> DeleteExerciseClicked;
        public EventHandler<Guid> EditExerciseClicked;
        public EventHandler<Guid> EnableDisableClicked;
        private readonly List<ExerciseBlock> _exerciseList;
        private readonly Activity _activity;

        public ExerciseRecyclerAdapter(List<ExerciseBlock> exerciseList, Activity activity)
        {
            _exerciseList = exerciseList;
            _activity = activity;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (ExerciseListViewHolder) holder;
            var thisExercise = _exerciseList[position];
            viewHolder.EnableDisableSwitch.Checked = thisExercise.Enabled.GetValueOrDefault(true);
            viewHolder.ExerciseCount.Text = thisExercise.Quantity.ToString();
            viewHolder.ExerciseName.Text = thisExercise.CombinedName;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var view = inflater.Inflate(Resource.Layout.ExerciseListItem, parent, false);
            var exerciseListViewHolder = new ExerciseListViewHolder(view);
            exerciseListViewHolder.DeleteExerciseClicked += (sender, i) => DeleteExerciseClicked?.Invoke(sender, _exerciseList[i].Id.Value);
            exerciseListViewHolder.EditExerciseClicked += (sender, i) => EditExerciseClicked?.Invoke(sender, _exerciseList[i].Id.Value);
            exerciseListViewHolder.EnableDisableClicked += (sender, i) => EnableDisableClicked?.Invoke(sender, _exerciseList[i].Id.Value);
            return exerciseListViewHolder;
        }

        public override int ItemCount => _exerciseList.Count;
    }
}