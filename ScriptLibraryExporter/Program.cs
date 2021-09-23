using System;
using System.Collections.Generic;
using System.IO;
using LitJson;


namespace Eastward {
	
	// 将script.g包中lua源文件还原为原有文件名
	internal class Program {
		
		public static void Main ( string[] args ) {

			if ( args.Length < 1 ) {
				return;
			}

			string gamePath = args[ 0 ];
			string configPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "config.g" );
			string scriptPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "script.g" );
			
			// script/1090e2cb7c5114cbe242a7a0f88bad1d : behaviour/creature/AnimShuffle.lua
			var sourceDict = new Dictionary< string, string > ();

			// 从config.g读取script映射配置
			using ( var fs = new FileStream ( configPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var pack = new GPack ( fs ) ) {
				
				Console.WriteLine ( pack );
				
				var entry = pack.GetEntry ( Consts.SCRIPT_LIBRARY );
				if ( entry == null ) {
					Console.WriteLine ( $"{Consts.SCRIPT_LIBRARY} not found in config.g" );
					return;
				}
				
				using ( var s = pack.GetInputStream ( entry ) ) {
					var jsonData = JsonMapper.ToObject ( new StreamReader ( s ) );
					
					var lookupKeys = jsonData[ "export" ].Keys;
					foreach ( var key in lookupKeys ) {
						// 需要将"script/"前缀去掉
						string[] splited = jsonData[ "export" ][ key ].ToString ().Split ( '/' );
						sourceDict.Add ( splited[ 1 ], jsonData[ "source" ][ key ].ToString () );
					}
					
				}
			}
			
			// 从script.g解压映射后的文件名
			using ( var fs = new FileStream ( scriptPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var pack = new GPack ( fs ) ) {
				foreach ( var key in sourceDict.Keys ) {
					var entry = pack.GetEntry ( key );
					if ( entry == null ) {
						Console.WriteLine ( $"{key} not found in script.g" );
						continue;
					}

					Utils.AffirmDir ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME ), sourceDict[ key ] );

					using ( var s = pack.GetInputStream ( entry ) ) {
						
						var outFilePath = Utils.FixFilePath ( gamePath, sourceDict[ key ] );
						Console.Write ( $"out: {outFilePath} ..." );

						var fileStream = File.Create ( outFilePath );
						s.Seek ( 0, SeekOrigin.Begin );
						s.CopyTo ( fileStream );
						fileStream.Close ();
						
						Console.Write ( $"   ok!\n" );
					}
				}
			}
		}


		/*
		[Obsolete]
		public static void ZipMain ( string[] args ) {
			if ( args.Length < 1 ) {
				return;
			}

			string gamePath = args[ 0 ];
			string configPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "config.g" );
			string scriptPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "script.g" );
			
			// script/1090e2cb7c5114cbe242a7a0f88bad1d : behaviour/creature/AnimShuffle.lua
			var sourceDict = new Dictionary< string, string > ();

			// 从config.g读取script映射配置
			using ( var fs = new FileStream ( configPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var zf = new ZipFile ( fs ) ) {
				var ze = zf.GetEntry ( Consts.SCRIPT_LIBRARY );
				if ( ze == null ) {
					Console.WriteLine ( $"{Consts.SCRIPT_LIBRARY} not found in config.g" );
					return;
				}
				
				using ( var s = zf.GetInputStream ( ze ) ) {
					var jsonData = JsonMapper.ToObject ( new StreamReader ( s ) );
					
					var lookupKeys = jsonData[ "export" ].Keys;
					foreach ( var key in lookupKeys ) {
						// 需要将"script/"前缀去掉
						string[] splited = jsonData[ "export" ][ key ].ToString ().Split ( '/' );
						sourceDict.Add ( splited[ 1 ], jsonData[ "source" ][ key ].ToString () );
					}
					
				}
			}
			
			// 从script.g解压映射后的文件名
			using ( var fs = new FileStream ( scriptPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var zf = new ZipFile ( fs ) ) {
				foreach ( var key in sourceDict.Keys ) {
					var ze = zf.GetEntry ( key );
					if ( ze == null ) {
						Console.WriteLine ( $"{key} not found in script.g" );
						continue;
					}

					Utils.AffirmDir ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME ), sourceDict[ key ] );

					using ( var s = zf.GetInputStream ( ze ) ) {
						
						var outFilePath = Utils.FixFilePath ( gamePath, sourceDict[ key ] );
						Console.Write ( $"out: {outFilePath} ..." );

						var fileStream = File.Create ( outFilePath );
						s.Seek ( 0, SeekOrigin.Begin );
						s.CopyTo ( fileStream );
						fileStream.Close ();
						
						Console.Write ( $"   ok!\n" );
					}
				}
			}
		}
		*/
	}

}