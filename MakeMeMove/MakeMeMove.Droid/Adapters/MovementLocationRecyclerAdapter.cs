using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using MakeMeMove.Droid.ViewHolders;
using MakeMeMove.Standard.Model;

namespace MakeMeMove.Droid.Adapters
{
    public class MovementLocationRecyclerAdapter : RecyclerView.Adapter
    {
        enum ViewTypes
        {
            MovementLocation,
            AddItem
        }

        public EventHandler<int> DeleteMovementLocationClicked;
        public EventHandler AddMovementLocationClicked;
        private List<MovementLocation> _movementLocationList;

        public MovementLocationRecyclerAdapter(List<MovementLocation> movementLocationList)
        {
            _movementLocationList = movementLocationList;
        }

        public void UpdateMovementLocationList(List<MovementLocation> exerciseList)
        {
            _movementLocationList = exerciseList;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is MovementLocationListViewHolder viewHolder)
            {
                var currentMovementLocation = _movementLocationList[position];

                viewHolder.LocationName.Text = currentMovementLocation.Name;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);

            if (((ViewTypes)viewType) == ViewTypes.MovementLocation)
            {
                var view = inflater.Inflate(Resource.Layout.MovementLocationListItem, parent, false);
                var movementLocationViewHolder = new MovementLocationListViewHolder(view);
                movementLocationViewHolder.DeleteMovementLocationClicked += (sender, i) => DeleteMovementLocationClicked?.Invoke(sender, _movementLocationList[i].Id);
                return movementLocationViewHolder;
            }
            else
            {
                var view = inflater.Inflate(Resource.Layout.AddMovementLocationListItem, parent, false);
                var addMovementLocationViewHolder = new AddMovementLocationListViewHolder(view);
                addMovementLocationViewHolder.AddMovementLocationClicked += (sender, args) => AddMovementLocationClicked?.Invoke(sender, args);
                return addMovementLocationViewHolder;
            }
        }

        public override int GetItemViewType(int position)
        {
            return (int)(position+1 < ItemCount ? ViewTypes.MovementLocation : ViewTypes.AddItem);
        }

        public override int ItemCount => _movementLocationList.Count + 1;
    }
}