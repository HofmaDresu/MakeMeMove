using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using MakeMeMove.Droid.ViewHolders;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Adapters
{
    public class ExerciseRecyclerAdapter : RecyclerView.Adapter
    {
        public EventHandler<int> DeleteExerciseClicked;
        public EventHandler<int> EditExerciseClicked;
        public EventHandler<int> EnableDisableClicked;
        private List<ExerciseBlock> _exerciseList;

        public ExerciseRecyclerAdapter(List<ExerciseBlock> exerciseList)
        {
            _exerciseList = exerciseList;
        }

        public void UpdateExerciseList(List<ExerciseBlock> exerciseList)
        {
            _exerciseList = exerciseList;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (ExerciseListViewHolder) holder;
            var thisExercise = _exerciseList[position];

            if (thisExercise.Enabled)
            {
                viewHolder.ServiceStarted.SetBackgroundResource(Resource.Drawable.CustomSwitchPositiveActive);
                viewHolder.ServiceStarted.SetText(Resource.String.SwitchOn);

                viewHolder.ServiceStopped.SetBackgroundResource(Android.Resource.Color.Transparent);
                viewHolder.ServiceStopped.Text = "";

                viewHolder.MainView.Enabled = true;
            }
            else
            {
                viewHolder.ServiceStarted.SetBackgroundResource(Android.Resource.Color.Transparent);
                viewHolder.ServiceStarted.Text = "";

                viewHolder.ServiceStopped.SetBackgroundResource(Resource.Drawable.CustomSwitchNegativeActive);
                viewHolder.ServiceStopped.SetText(Resource.String.SwitchOff);

                viewHolder.MainView.Enabled = false;
            }
            viewHolder.ExerciseTitle.Text = $"{thisExercise.Quantity} {thisExercise.CombinedName}";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var view = inflater.Inflate(Resource.Layout.ExerciseListItem, parent, false);
            var exerciseListViewHolder = new ExerciseListViewHolder(view);
            exerciseListViewHolder.DeleteExerciseClicked += (sender, i) => DeleteExerciseClicked?.Invoke(sender, _exerciseList[i].Id);
            exerciseListViewHolder.EditExerciseClicked += (sender, i) => EditExerciseClicked?.Invoke(sender, _exerciseList[i].Id);
            exerciseListViewHolder.EnableDisableClicked += (sender, i) => EnableDisableClicked?.Invoke(sender, _exerciseList[i].Id);
            return exerciseListViewHolder;
        }

        public override int ItemCount => _exerciseList.Count;
    }
}