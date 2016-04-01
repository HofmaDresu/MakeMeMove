using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Fragments
{
    public class ExerciseListFragment : BaseMainFragment
    {
        private Data _data;
        private List<ExerciseBlock> _exerciseBlocks;
        private RecyclerView _exerciseRecyclerView;
        private Button _addExerciseButton;

        public void Initialize(Data data)
        {
            _data = data;
            Title = "My Exercises";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Main_ExerciseList, container, false);

            _exerciseRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ExerciseList);
            _addExerciseButton = view.FindViewById<Button>(Resource.Id.AddExerciseButton);

            _addExerciseButton.Click += (sender, args) => StartActivity(new Intent(Activity, typeof(ManageExerciseActivity)));

            _exerciseRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            _exerciseBlocks = _data.GetExerciseBlocks();
            
            UpdateExerciseList();
        }

        private void EditExerciseClicked(object sender, int id)
        {
            var intent = new Intent(Activity, typeof(ManageExerciseActivity));
            intent.PutExtra(Constants.ExerciseId, id);
            StartActivity(intent);
        }

        private void DeleteExerciseClicked(object sender, int id)
        {
            var selectedExercise = _exerciseBlocks.FirstOrDefault(e => e.Id == id);
            if (selectedExercise != null)
            {
                var exerciseIndex = _exerciseBlocks.IndexOf(selectedExercise);
                _data.DeleteExerciseBlock(selectedExercise.Id);
                _exerciseBlocks = _data.GetExerciseBlocks();

                var adapter = (ExerciseRecyclerAdapter)_exerciseRecyclerView.GetAdapter();
                adapter.UpdateExerciseList(_exerciseBlocks);
                adapter.NotifyItemRemoved(exerciseIndex);
            }
        }

        private void UpdateExerciseList()
        {
            var exerciseListAdapter = new ExerciseRecyclerAdapter(_exerciseBlocks);
            exerciseListAdapter.DeleteExerciseClicked += DeleteExerciseClicked;
            exerciseListAdapter.EditExerciseClicked += EditExerciseClicked;
            exerciseListAdapter.EnableDisableClicked += EnableDisableClicked;
            _exerciseRecyclerView.SetAdapter(exerciseListAdapter);
        }

        private void EnableDisableClicked(object sender, int id)
        {
            var selectedExercise = _exerciseBlocks.FirstOrDefault(e => e.Id == id);
            if (selectedExercise != null)
            {
                var exerciseIndex = _exerciseBlocks.IndexOf(selectedExercise);
                selectedExercise.Enabled = !selectedExercise.Enabled;
                _data.UpdateExerciseBlock(selectedExercise);
                _exerciseBlocks = _data.GetExerciseBlocks();

                var adapter = (ExerciseRecyclerAdapter)_exerciseRecyclerView.GetAdapter();
                adapter.UpdateExerciseList(_exerciseBlocks);
                adapter.NotifyItemChanged(exerciseIndex);
            }
        }
    }
}