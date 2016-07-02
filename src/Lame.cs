//  LAME ( LAME Ain't an Mp3 Encoder ) 
//  You must call the fucntion "BeVersion" to obtain information  like version 
//  numbers (both of the DLL and encoding engine), release date and URL for 
//  lame_enc's homepage. All this information should be made available to the 
//  user of your product through a dialog box or something similar.
//  You must see all information about LAME project and legal license infos at 
//  http://www.mp3dev.org/  The official LAME site
//
//  About Thomson and/or Fraunhofer patents:
//  Any use of this product does not convey a license under the relevant 
//  intellectual property of Thomson and/or Fraunhofer Gesellschaft nor imply 
//  any right to use this product in any finished end user or ready-to-use final 
//  product. An independent license for such use is required. 
//  For details, please visit http://www.mp3licensing.com.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Dictionary.Common;
using System.Security.Permissions;
using System.Reflection; 

namespace Dictionary {

	public enum VbrMethod : int {
		None = -1,
		Default = 0,
		Old = 1,
		New = 2,
		Mtrh = 3,
		Abr = 4
	}

	/* MPEG modes */
	public enum MpegMode : int {
		Stereo = 0,
		JointStereo,
		DualChannel,   /* LAME doesn't supports this! */
		Mono,
		NotSet,
		MaxIndicator   /* Don't use this! It's used for sanity checks. */
	}

	public enum LameQualityPreset : int {
		LqpNoPreset = -1,
		// QUALITY PRESETS
		LqpNormalQuality = 0,
		LqpLowQuality = 1,
		LqpHighQuality = 2,
		LqpVoiceQuality = 3,
		LqpR3Mix = 4,
		LqpVeryHighQuality = 5,
		LqpStandard = 6,
		LqpFastStandard = 7,
		LqpExtreme = 8,
		LqpFastExtrem = 9,
		LqpInsane = 10,
		LqpAbr = 11,
		LqpCbr = 12,
		LqpMedium = 13,
		LqpFastMedium = 14,
		// NEW PRESET VALUES
		LqpPhone = 1000,
		LqpSW = 2000,
		LqpAM = 3000,
		LqpFM = 4000,
		LqpVoice = 5000,
		LqpRadio = 6000,
		LqpTape = 7000,
		LqpHifi = 8000,
		LqpCD = 9000,
		LqpStudio = 10000
	}

	/// <summary>
	/// Lame_enc DLL functions
	/// </summary>
	public static class LameEncDll {
		//Error codes
		public const uint BeErrSuccessful = 0;
		public const uint BeErrInvalidFormat = 1;
		public const uint BeErrInvalidFormatParameters = 2;
		public const uint BeErrNoMoreHandles = 3;
		public const uint BeErrInvalidHandle = 4;

		/// <summary>
		/// Encodes a chunk of samples. Samples are contained in a byte array
		/// </summary>
		/// <param name="hbeStream">Handle of the stream.</param>
		/// <param name="buffer">Bytes to encode</param>
		/// <param name="index">Position of the first byte to encode</param>
		/// <param name="nBytes">Number of bytes to encode (not samples, samples are two byte lenght)</param>
		/// <param name="pOutput">Buffer where to write the encoded data.
		/// This buffer should be at least of the minimum size returned by BeInitStream().</param>
		/// <param name="pdwOutput">Returns the number of bytes of encoded data written. 
		/// The amount of data written might vary from chunk to chunk</param>
		/// <returns>On success: BeErrSuccessful</returns>
		internal static uint EncodeChunk(uint hbeStream, byte[] buffer, int index, uint nBytes, byte[] pOutput, ref uint pdwOutput) {
			uint res;
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			try {
				IntPtr ptr = (IntPtr)(handle.AddrOfPinnedObject().ToInt32() + index);
				res = NativeMethods.BeEncodeChunk(hbeStream, nBytes / 2/*Samples*/, ptr, pOutput, ref pdwOutput);
			} finally {
				handle.Free();
			}
			return res;
		}
		/// <summary>
		/// Encodes a chunk of samples. Samples are contained in a byte array
		/// </summary>
		/// <param name="hbeStream">Handle of the stream.</param>
		/// <param name="buffer">Bytes to encode</param>
		/// <param name="pOutput">Buffer where to write the encoded data.
		/// This buffer should be at least of the minimum size returned by BeInitStream().</param>
		/// <param name="pdwOutput">Returns the number of bytes of encoded data written. 
		/// The amount of data written might vary from chunk to chunk</param>
		/// <returns>On success: BeErrSuccessful</returns>
		internal static uint EncodeChunk(uint hbeStream, byte[] buffer, byte[] pOutput, ref uint pdwOutput) {
			return EncodeChunk(hbeStream, buffer, 0, (uint)buffer.Length, pOutput, ref pdwOutput);
		}
		
		
	}
}
