using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid.ViewHolders
{
    public class ExerciseListViewHolder: RecyclerView.ViewHolder
    {
        public TextView ExerciseTitle { get; }
        public View EnableDisableSwitch { get; }
        public TextView ServiceStopped { get; }
        public TextView ServiceStarted { get; }
        public View MainView { get; }
        private readonly ImageView _editButton;
        private readonly ImageView _deleteButton;

        public EventHandler<int> DeleteExerciseClicked;
        public EventHandler<int> EditExerciseClicked;
        public EventHandler<int> EnableDisableClicked;

        public ExerciseListViewHolder(View view) 
            : base(view)
        {
            ExerciseTitle = view.FindViewById<TextView>(Resource.Id.ExerciseTitle);
            _editButton = view.FindViewById<ImageView>(Resource.Id.EditButton);
            _deleteButton = view.FindViewById<ImageView>(Resource.Id.DeleteButton);
            EnableDisableSwitch = EnableDisableSwitch = view.FindViewById(Resource.Id.EnableDisableToggle);
            ServiceStopped = view.FindViewById<TextView>(Resource.Id.ServiceStopped);
            ServiceStarted = view.FindViewById<TextView>(Resource.Id.ServiceStarted);
            MainView = view;

            SetUpEvents();
        }

        private void SetUpEvents()
        {
            _editButton.Click += (sender, args) => EditExerciseClicked?.Invoke(this, AdapterPosition);
            _deleteButton.Click += (sender, args) => DeleteExerciseClicked?.Invoke(this, AdapterPosition);
            EnableDisableSwitch.Click += (sender, args) => EnableDisableClicked?.Invoke(this, AdapterPosition);
        }
    }
}