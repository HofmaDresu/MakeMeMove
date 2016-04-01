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

namespace MakeMeMove.Droid.ViewHolders
{
    public class ExerciseListViewHolder: RecyclerView.ViewHolder
    {
        public TextView ExerciseTitle { get; }
        public Switch EnableDisableSwitch { get; }
        private readonly Button _editButton;
        private readonly Button _deleteButton;
        
        public EventHandler<int> DeleteExerciseClicked;
        public EventHandler<int> EditExerciseClicked;
        public EventHandler<int> EnableDisableClicked;

        public ExerciseListViewHolder(Android.Views.View view) 
            : base(view)
        {
            ExerciseTitle = view.FindViewById<TextView>(Resource.Id.ExerciseTitle);
            _editButton = view.FindViewById<Button>(Resource.Id.EditButton);
            _deleteButton = view.FindViewById<Button>(Resource.Id.DeleteButton);
            EnableDisableSwitch = EnableDisableSwitch = view.FindViewById<Switch>(Resource.Id.EnableDisableSwitch);

            SetUpEvents();
        }

        private void SetUpEvents()
        {
            _editButton.Click += (sender, args) => EditExerciseClicked?.Invoke(this, base.AdapterPosition);
            _deleteButton.Click += (sender, args) => DeleteExerciseClicked?.Invoke(this, base.AdapterPosition);
            EnableDisableSwitch.Click += (sender, args) => EnableDisableClicked?.Invoke(this, base.AdapterPosition);
        }
    }
}