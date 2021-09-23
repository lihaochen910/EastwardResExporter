using System;
using System.IO;

namespace Eastward {
	
	public static class Utils {
		
		// complete lua ext
		// fix something_ -> something.lua
		public static void FixLibExt ( string gamePath ) {
			if ( !Directory.Exists ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, Consts.LIB ) ) ) {
				return;
			}
			foreach ( string file in Directory.EnumerateFiles ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, Consts.LIB ),
				"*.*", SearchOption.AllDirectories ) ) {
				if ( file.EndsWith ( "_" ) ) {
					var name = file.Substring ( 0, file.Length - 1 );
					File.Move ( file, $"{name}.lua" );
					Console.WriteLine ( $"{name}.lua" );
				}
			}
		}


		// if path not exist, create it
		// gamePath: "P:\Games\Eastward\content\game" filePath: "shader/TestScript/Basic.shader_script"
		// if not exist will create P:\Games\Eastward\content\game\shader\TestScript
		public static void AffirmDir ( string basePath, string filePath ) {
			string[] paths = filePath.Split ( '/' );
			string tempPath = basePath;
			for ( int i = 0; i < paths.Length - 1; i++ ) {
				tempPath = Path.Combine ( tempPath, paths[ i ] );
				if ( !Directory.Exists ( tempPath ) ) {
					Directory.CreateDirectory ( tempPath );
					Console.WriteLine ( $"创建目录: {tempPath}" );
				}
			}
		}


		// script/something -> P:\Games\Easteard\content\game\script\something
		public static string FixFilePath ( string gamePath, string filePath ) {
			string[] paths = filePath.Split ( '/' );
			string tempPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME );
			for ( int i = 0; i < paths.Length; i++ ) {
				tempPath = Path.Combine ( tempPath, paths[ i ] );
			}

			return tempPath;
		}
	}
}