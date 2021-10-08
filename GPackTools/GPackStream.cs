using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImpromptuNinjas.ZStd;


namespace Eastward {

	
	public class GPackEntry {
		public string Name;
		public int offset;
		public int zip;
		public int size;
		public int zsize;

		public override string ToString () {
			return $"归档: {Name} offset: 0x{offset.ToString("x")} zip: {zip} size: {size} zsize: {zsize}";
		}
	}
	

	public class GPack : IDisposable {
		
		public static readonly byte[] MAGIC_NUMBER = new byte[] { 0x37, 0x6A };
		public int FilesCount;

		public Dictionary< GPackEntry, string > FileDictionary {
			get => _fileDictionary;
		}

		private FileStream _fileStream;
		private BinaryReader _binaryReader;
		private ZStdDecompressor _decompressor;
		private Dictionary< GPackEntry, string > _fileDictionary;


		public GPack ( FileStream stream ) {
			_fileStream = stream;
			_binaryReader = new BinaryReader ( stream );
			_decompressor = new ZStdDecompressor ();

			var magicNumber = _binaryReader.ReadBytes ( 4 );
			// if ( !magicNumber.Equals ( MAGIC_NUMBER ) ) {
			// 	Console.WriteLine ( "文件头不匹配" );
			// 	return;
			// }

			// var fileCountBytes = _binaryReader.ReadBytes ( 4 );
			// if ( BitConverter.IsLittleEndian ) {
			// 	Array.Reverse ( fileCountBytes );
			// }
			
			// FilesCount = BitConverter.ToInt32 ( fileCountBytes, 0 );
			FilesCount = _binaryReader.ReadInt32 ();
			
			_fileDictionary = new Dictionary< GPackEntry, string > ();

			for ( int i = 0; i < FilesCount; i++ ) {
				var entry = new GPackEntry ();
				entry.Name = ReadUTF8String ( _binaryReader );
				entry.offset = _binaryReader.ReadInt32 ();
				entry.zip = _binaryReader.ReadInt32 ();
				entry.size = _binaryReader.ReadInt32 ();
				entry.zsize = _binaryReader.ReadInt32 ();

				_fileDictionary.Add ( entry, entry.Name );
			}

			_binaryReader.BaseStream.Seek ( 0, SeekOrigin.Begin );
		}


		public GPackEntry GetEntry ( string entryPath ) {
			if ( _fileDictionary.ContainsValue ( entryPath ) ) {
				foreach ( var entry in _fileDictionary.Keys ) {
					if ( entry.Name.Equals ( entryPath ) ) {
						return entry;
					}
				}
			}

			return null;
		}


		public MemoryStream GetInputStream ( GPackEntry entry ) {
			switch ( entry.zip ) {
				case 0:
					_binaryReader.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
					return new MemoryStream ( _binaryReader.ReadBytes ( entry.size ) );
				case 2:
					_binaryReader.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
					byte[] result = new byte[ entry.size ];
					_decompressor.Decompress ( result, _binaryReader.ReadBytes ( entry.zsize ) );
					return new MemoryStream ( result );
				default:
					return null;
			}
		}


		public MemoryStream GetInputStream ( string entryPath ) {
			if ( _fileDictionary.ContainsValue ( entryPath ) ) {
				foreach ( var entry in _fileDictionary.Keys ) {
					if ( entry.Name.Equals ( entryPath ) ) {
						return GetInputStream ( entry );
					}
				}
			}

			return null;
		}


		public void Dispose () {
			_binaryReader.Dispose ();
			_decompressor.Dispose ();
		}


		public static void Unpack ( string path ) {
			
			using ( var fs = new FileStream ( path, FileMode.Open, FileAccess.Read ) ) {
				using ( var br = new BinaryReader ( fs ) ) {
					// var magicNumber = br.ReadBytes ( 4 );
					// if ( !magicNumber.Equals ( MAGIC_NUMBER ) ) {
					// 	Console.WriteLine ( "文件头不匹配" );
					// 	return;
					// }
					
					var gpackFile = new GPack ( fs );
					
					Console.WriteLine ( $"文件数量:{gpackFile.FilesCount}" );
					
					// 建立父目录
					var arcInfo = new FileInfo ( path );
					Utils.AffirmDir ( arcInfo.Directory.FullName, Path.GetFileNameWithoutExtension ( arcInfo.FullName ) + "/" + "dummy.dat" );

					var zstdDecompressor = new ZStdDecompressor ();

					foreach ( var entry in gpackFile._fileDictionary.Keys ) {

						var fileName = gpackFile._fileDictionary[ entry ];
						
						var realPath = Path.Combine ( arcInfo.Directory.FullName, Path.GetFileNameWithoutExtension ( arcInfo.FullName ), fileName );
						Utils.AffirmDir ( Path.Combine ( arcInfo.Directory.FullName, Path.GetFileNameWithoutExtension ( arcInfo.FullName ) ), fileName );
						
						switch ( entry.zip ) {
							case 0:
								Console.WriteLine ( $"{entry}" );
								Console.WriteLine ( $"Extracting to {realPath}...\n" );
								// br.BaseStream.Seek ( entry.offset, SeekOrigin.Current );
								// br.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
								// br.BaseStream.Seek ( entry.size, SeekOrigin.Current );
								br.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
								File.WriteAllBytes ( realPath, br.ReadBytes ( entry.size ) );
								break;
							case 2:
								Console.WriteLine ( $"{entry}" );
								Console.WriteLine ( $"Extracting(zstd) to {realPath}...\n" );
								// br.BaseStream.Seek ( entry.offset, SeekOrigin.Current );
								// br.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
								// br.BaseStream.Seek ( entry.zsize, SeekOrigin.Current );
								br.BaseStream.Seek ( entry.offset, SeekOrigin.Begin );
								byte[] result = new byte[ entry.size ];
								zstdDecompressor.Decompress ( result, br.ReadBytes ( entry.zsize ) );
								File.WriteAllBytes ( realPath, result );
								break;
						}
					}
					
					zstdDecompressor.Dispose ();
				}
			}
		}

		
		private static string ReadUTF8String ( BinaryReader input ) {
			List<byte> strBytes = new List<byte>();
			int b;
			while ( ( b = input.ReadByte () ) != 0x00 ) {
				strBytes.Add ( (byte)b );
			}
			return Encoding.UTF8.GetString ( strBytes.ToArray () );
		}
		
		
		public override string ToString () {
			StringBuilder builder = new StringBuilder ();
			builder.AppendLine ( $"归档文件数量: {_fileDictionary.Count}" );
			foreach ( var kv in _fileDictionary ) {
				builder.AppendLine ( kv.Key.ToString () );
			}
			return builder.ToString ();
		}
	}

}