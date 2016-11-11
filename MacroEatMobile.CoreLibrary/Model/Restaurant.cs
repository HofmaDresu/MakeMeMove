using System;
using System.Collections.Generic;

namespace MacroEatMobile.Core
{
	public class Restaurant
	{
		public CgRestaurant CgRestaurant { get; set; }
		public RestaurantBrand RestaurantBrand { get; set; }
	}

	public class RestaurantBrand
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}

	public class CgRestaurant
	{
		public int Id { get; set; }

		public int OriginalSourceId { get; set; }
		public bool featured { get; set; }
		public string name { get; set; }
		public CgAddress address { get; set; }
		public string neighborhood { get; set; }
		public decimal? latitude { get; set; }
		public decimal? longitude { get; set; }
		public decimal? distance { get; set; }
		public string image { get; set; }
		public string phone_number { get; set; }
		public string fax_number { get; set; }
		public decimal? rating { get; set; }
		public string tagline { get; set; }
		public string profile { get; set; }
		public string website { get; set; }
		public bool has_video { get; set; }
		public bool has_offers { get; set; }

		public int user_review_count { get; set; }
		public string sample_categories { get; set; }
		public string impression_id { get; set; }        
		public List<CgTag> tags { get; set; }
		public string public_id { get; set; }
		public int business_operation_status { get; set; }
		public object scorecard { get; set; }
		public bool has_menu { get; set; }
		public string politanName { get; set; }

		public string FromSearchPhrase{get; set;}
		public int FromSearchPage { get; set; }
		public string FromSearchLocation { get; set; }
	}

	public class CgAddress
	{
		public string street { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string postal_code { get; set; }

	}

	public class CgTag
	{
		public int id { get; set; }
		public string name { get; set; }
		public bool primary { get; set; }

	}
}

