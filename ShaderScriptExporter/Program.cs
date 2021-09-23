using System;
using System.Collections.Generic;
using System.IO;
using LitJson;


namespace Eastward {

	/// <summary>
	/// *解析asset_index表筛选type为shader_script的Node
	/// *筛选type为shader、shader_script、glsl的Node
	/// *导出shader源文件
	/// </summary>
	internal class Program {
		
		public static void Main ( string[] args ) {

			if ( args.Length < 1 ) {
				return;
			}

			string gamePath = args[ 0 ];
			string configPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "config.g" );
			string scriptPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "script.g" );
			
			// shader/creature-camouflage.shader : {"type":"","objectFiles":{}}
			var sourceDict = new Dictionary< string, JsonData > ();

			// 从config.g读取asset_index映射配置
			using ( var fs = new FileStream ( configPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var pack = new GPack ( fs ) ) {
				var entry = pack.GetEntry ( Consts.ASSET_INDEX );
				if ( entry == null ) {
					Console.WriteLine ( $"{Consts.ASSET_INDEX} not found in config.g" );
					return;
				}
				
				using ( var s = pack.GetInputStream ( entry ) ) {
					var jsonData = JsonMapper.ToObject ( new StreamReader ( s ) );
					
					var lookupKeys = jsonData.Keys;
					foreach ( var key in lookupKeys ) {
						switch ( jsonData[ key ][ Consts.ASSET_NODE_TYPE ].ToString () ) {
							case Consts.ASSET_NODE_TYPE_SHADER:
							case Consts.ASSET_NODE_TYPE_SHADER_SCRIPT:
							case Consts.ASSET_NODE_TYPE_GLSL:
								sourceDict.Add ( key, jsonData[ key ] );
								break;
						}
					}
					
				}
			}
			
			// 建立shader目录
			string shaderDir = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, Consts.ASSET_NODE_TYPE_SHADER );
			if ( !Directory.Exists ( shaderDir ) ) {
				Directory.CreateDirectory ( shaderDir );
			}
			
			// 从script.g解压映射后的文件名
			using ( var fs = new FileStream ( scriptPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var pack = new GPack ( fs ) ) {
				foreach ( var key in sourceDict.Keys ) {

					// 需要将"script/"前缀去掉
					string filename = null;
					if ( sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ].ContainsKey ( Consts.ASSET_NODE_OBJECT_FILES_DEF ) ) {
						filename =
							sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ][ Consts.ASSET_NODE_OBJECT_FILES_DEF ]
								.ToString ().Split ( '/' )[ 1 ];
					}
					if ( sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ].ContainsKey ( Consts.ASSET_NODE_OBJECT_FILES_SRC ) ) {
						filename =
							sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ][ Consts.ASSET_NODE_OBJECT_FILES_SRC ]
								.ToString ().Split ( '/' )[ 1 ];
					}

					if ( string.IsNullOrEmpty ( filename ) ) {
						continue;
					}
					
					var entry = pack.GetEntry ( filename );
					if ( entry == null ) {
						Console.WriteLine ( $"{filename} not found in script.g" );
						continue;
					}
					
					Utils.AffirmDir ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME ), key );
					
					using ( var s = pack.GetInputStream ( entry ) ) {
						
						var outFilePath = Utils.FixFilePath ( gamePath, key );
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
			
			// shader/creature-camouflage.shader : {"type":"","objectFiles":{}}
			var sourceDict = new Dictionary< string, JsonData > ();

			// 从config.g读取asset_index映射配置
			using ( var fs = new FileStream ( configPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var zf = new ZipFile ( fs ) ) {
				var ze = zf.GetEntry ( Consts.ASSET_INDEX );
				if ( ze == null ) {
					Console.WriteLine ( $"{Consts.ASSET_INDEX} not found in config.g" );
					return;
				}
				
				using ( var s = zf.GetInputStream ( ze ) ) {
					var jsonData = JsonMapper.ToObject ( new StreamReader ( s ) );
					
					var lookupKeys = jsonData.Keys;
					foreach ( var key in lookupKeys ) {
						switch ( jsonData[ key ][ Consts.ASSET_NODE_TYPE ].ToString () ) {
							case Consts.ASSET_NODE_TYPE_SHADER:
							case Consts.ASSET_NODE_TYPE_SHADER_SCRIPT:
							case Consts.ASSET_NODE_TYPE_GLSL:
								sourceDict.Add ( key, jsonData[ key ] );
								break;
						}
					}
					
				}
			}
			
			// 建立shader目录
			string shaderDir = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, Consts.ASSET_NODE_TYPE_SHADER );
			if ( !Directory.Exists ( shaderDir ) ) {
				Directory.CreateDirectory ( shaderDir );
			}
			
			// 从script.g解压映射后的文件名
			using ( var fs = new FileStream ( scriptPackPath, FileMode.Open, FileAccess.Read ) )
			using ( var zf = new ZipFile ( fs ) ) {
				foreach ( var key in sourceDict.Keys ) {

					// 需要将"script/"前缀去掉
					string filename = null;
					if ( sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ].ContainsKey ( Consts.ASSET_NODE_OBJECT_FILES_DEF ) ) {
						filename =
							sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ][ Consts.ASSET_NODE_OBJECT_FILES_DEF ]
								.ToString ().Split ( '/' )[ 1 ];
					}
					if ( sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ].ContainsKey ( Consts.ASSET_NODE_OBJECT_FILES_SRC ) ) {
						filename =
							sourceDict[ key ][ Consts.ASSET_NODE_OBJECTFILES ][ Consts.ASSET_NODE_OBJECT_FILES_SRC ]
								.ToString ().Split ( '/' )[ 1 ];
					}

					if ( string.IsNullOrEmpty ( filename ) ) {
						continue;
					}
					
					var ze = zf.GetEntry ( filename );
					if ( ze == null ) {
						Console.WriteLine ( $"{filename} not found in script.g" );
						continue;
					}
					
					Utils.AffirmDir ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME ), key );
					
					using ( var s = zf.GetInputStream ( ze ) ) {
						
						var outFilePath = Utils.FixFilePath ( gamePath, key );
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