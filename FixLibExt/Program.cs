namespace Eastward {
	
	internal class Program {
		
		public static void Main ( string[] args ) {
			if ( args.Length < 1 ) {
				return;
			}

			string gamePath = args[ 0 ];
			Utils.FixLibExt ( gamePath );
		}
	}
}