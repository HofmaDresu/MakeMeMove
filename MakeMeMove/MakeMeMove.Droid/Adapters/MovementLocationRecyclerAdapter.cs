using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using MakeMeMove.Droid.ViewHolders;
using MakeMeMove.Model;
using MakeMeMove.Standard.Model;

namespace MakeMeMove.Droid.Adapters
{
    public class MovementLocationRecyclerAdapter : RecyclerView.Adapter
    {
        public EventHandler<int> DeleteMovementLocationClicked;
        private List<MovementLocation> _movementLocationList;

        public MovementLocationRecyclerAdapter(List<MovementLocation> movementLocationList)
        {
            _movementLocationList = movementLocationList;
        }

        public void UpdateExerciseList(List<MovementLocation> exerciseList)
        {
            _movementLocationList = exerciseList;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (MovementLocationListViewHolder) holder;
            var currentMovementLocation = _movementLocationList[position];

            viewHolder.LocationName.Text = currentMovementLocation.Name;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var view = inflater.Inflate(Resource.Layout.ExerciseListItem, parent, false);
            var exerciseListViewHolder = new MovementLocationListViewHolder(view);
            exerciseListViewHolder.DeleteExerciseClicked += (sender, i) => DeleteMovementLocationClicked?.Invoke(sender, _movementLocationList[i].Id);
            return exerciseListViewHolder;
        }

        public override int ItemCount => _movementLocationList.Count;
    }
}