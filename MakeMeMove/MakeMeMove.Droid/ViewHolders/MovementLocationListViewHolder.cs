using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid.ViewHolders
{
    public class MovementLocationListViewHolder : RecyclerView.ViewHolder
    {
        public TextView LocationName { get; }
        public View MainView { get; }
        private readonly ImageView _deleteButton;

        public EventHandler<int> DeleteExerciseClicked;

        public MovementLocationListViewHolder(View view) 
            : base(view)
        {
            LocationName = view.FindViewById<TextView>(Resource.Id.MovementLocationTitle);
            _deleteButton = view.FindViewById<ImageView>(Resource.Id.DeleteButton);
            MainView = view;

            SetUpEvents();
        }

        private void SetUpEvents()
        {
            _deleteButton.Click += (sender, args) => DeleteExerciseClicked?.Invoke(this, AdapterPosition);
        }
    }
}