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

        public EventHandler<int> DeleteMovementLocationClicked;

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
            _deleteButton.Click += (sender, args) => DeleteMovementLocationClicked?.Invoke(this, AdapterPosition);
        }
    }

    public class AddMovementLocationListViewHolder : RecyclerView.ViewHolder
    {
        public View MainView { get; }
        public EventHandler AddMovementLocationClicked;

        public AddMovementLocationListViewHolder(View view)
            : base(view)
        {
            MainView = view;

            SetUpEvents();
        }

        private void SetUpEvents()
        {
            MainView.Click += (sender, args) => AddMovementLocationClicked?.Invoke(this, null);
        }
    }
}