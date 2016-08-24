using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MacroEatMobile.Core
{
	public class JsonSerializer
	{
		public static string Serialize(object objectToSerialize)
		{
			//return new RestSharp.Serializers.JsonSerializer ().Serialize (objectToSerialize);
			return JsonConvert.SerializeObject(objectToSerialize);
		}

		public static T Deserialize<T>(string stringToDeserialize)
		{
			return JsonConvert.DeserializeObject<T> (stringToDeserialize);
		}
	}
}

