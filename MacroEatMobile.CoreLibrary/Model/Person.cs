using System;
using System.Collections.Generic;

namespace MacroEatMobile.Core
{
	public class Person
	{
		public Person()
		{
            DayAllocations = new List<DayAllocation>();
		}

	    private bool _enableMfp;

		public int Id { get; set; }
        public List<DayAllocation> DayAllocations { get; set; }

		private  string _userName;
		public string Username
		{
			get { return _userName;  } 
			set { _userName = value.ToLower();  }
		}

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public int WeightInLbs { get; set; }
        public int HeightInInches { get; set; }
        public string MfpUser { get; set; }
        public string MfpShareKey { get; set; }

        [Obsolete("Changed property to EnableMfp")]
	    public bool MfpSuppressAsk
	    {
	        get { return _enableMfp; }
	        set { _enableMfp = value; }
	    }
        public bool MfpEnabled
        {
            get { return _enableMfp; }
            set { _enableMfp = value; }
        }

        public List<Role> Roles { get; set; }
        public string JobActivityLevel { get; set; }
        public string DietPlan { get; set; }
        public string WeightGoal { get; set; }
        public string ActiveHours { get; set; }
	    public bool IsGuestUser => _userName == "guest@fudi.st";
	}
}

