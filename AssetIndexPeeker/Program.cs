using System;
using System.Collections.Generic;
using System.IO;
using LitJson;


namespace Eastward {

	/// <summary>
	/// 列出asset_index表中出现的所有AssetNode类型以及objectFiles字段类型
	/// </summary>
	internal class Program {
		
		public static void Main ( string[] args ) {

			if ( args.Length < 1 ) {
				return;
			}

			string gamePath = args[ 0 ];
			string configPackPath = Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "config.g" );
			
			// GPack.Unpack ( Path.Combine ( gamePath, Consts.CONTENT, Consts.GAME, "font.g" ) );
			// return;
			
			// shader/creature-camouflage.shader : {"type":"","objectFiles":{}}
			// var sourceDict = new Dictionary< string, JsonData > ();
			var types = new List< string > ();
			var objectFilesTypes = new List< string > ();

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
						var nodeType = jsonData[ key ][ Consts.ASSET_NODE_TYPE ].ToString ();
						if ( !types.Contains ( nodeType ) ) {
							types.Add ( nodeType );
						}

						foreach ( var objectFilesKey in jsonData[ key ][ Consts.ASSET_NODE_OBJECTFILES ].Keys ) {
							if ( !objectFilesTypes.Contains ( objectFilesKey ) ) {
								objectFilesTypes.Add ( objectFilesKey );
							}
						}
					}
					
				}
			}

			Console.WriteLine ( "AssetNode类型:" );
			foreach ( var type in types ) {
				Console.WriteLine ( $"\t{type}" );
			}
			
			Console.WriteLine ();
			
			Console.WriteLine ( "ObjectFiles类型:" );
			foreach ( var type in objectFilesTypes ) {
				Console.WriteLine ( $"\t{type}" );
			}
		}
		
		
	}

}