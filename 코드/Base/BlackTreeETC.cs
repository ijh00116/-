using UnityEngine;

namespace BlackTree
{
    public static class BTETC
    {
		public static string googleTokenID;//googletokenid
		public static string userUUID;//googleuserid
		public static string backendUUID;//backenduuid
		public static bool isFirstUser=false;
		public static bool isGuestUser = false;
		public static bool eang = false;
		public static class Constant
		{
			public const float TouchScreenSensitive = 0.05f;
			public static readonly string[] numberAlpha ={
				"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
				"V", "W", "X", "Y", "Z",
				"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ",
				"AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
				"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
				"BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
				"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
				"BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
				"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
				"BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
				"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
				"BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
				"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
				"BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ"
			};
		}


		public static class Scene
		{
			
			public const string LoadingIntro = "LoadResources";

			public const string mainStagescene = "Main";

			public const string HaveToUpdate = "HaveToUpdate";
			public const string ServerFixScene = "ServerFixScene";

		}
	}
}
