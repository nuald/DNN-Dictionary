/********************************************************************************/
/*****        This file is part of Dictionary module for DNN.               *****/
/*****                                                                      *****/
/***** Dictionary module for DNN is free software: you can redistribute it  *****/
/***** and/or modify it under the terms of the GNU General Public License   *****/
/***** as published by the Free Software Foundation, either version 3 of    *****/
/***** the License, or (at your option) any later version.                  *****/
/*****                                                                      *****/
/***** Dictionary module for DNN is distributed in the hope that it will be *****/
/***** useful, but WITHOUT ANY WARRANTY; without even the implied warranty  *****/
/***** of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the      *****/
/***** GNU General Public License for more details.                         *****/
/*****                                                                      *****/
/***** You should have received a copy of the GNU General Public License    *****/
/***** along with Dictionary module for DNN. If not, see                    *****/
/***** <http://www.gnu.org/licenses/>.                                      *****/
/*****                                                                      *****/
/***** Copyright 2008 EELLC                                                 *****/
/********************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Dictionary.Common {

	/// <summary>
	/// Summary description for NativeMethods
	/// </summary>
	public static class NativeMethods {

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		/// <summary>
		/// This function is the first to call before starting an encoding stream.
		/// </summary>
		/// <param name="pbeConfig">Encoder settings</param>
		/// <param name="dwSamples">Receives the number of samples (not bytes, each sample is a SHORT) to send to each BeEncodeChunk() on return.</param>
		/// <param name="dwBufferSize">Receives the minimun number of bytes that must have the output(result) buffer</param>
		/// <param name="phbeStream">Receives the stream handle on return</param>
		/// <returns>On success: BeErrSuccessful</returns>
		[DllImport("Lame_enc.dll", EntryPoint = "beInitStream")]
		public static extern uint BeInitStream(Be_Config pbeConfig, ref uint dwSamples, ref uint dwBufferSize, ref uint phbeStream);

		/// <summary>
		/// Encodes a chunk of samples. Please note that if you have set the output to 
		/// generate mono SAPI files you must feed BeEncodeChunk() with mono samples
		/// </summary>
		/// <param name="hbeStream">Handle of the stream.</param>
		/// <param name="nSamples">Number of samples to be encoded for this call. 
		/// This should be identical to what is returned by BeInitStream(), 
		/// unless you are encoding the last chunk, which might be smaller.</param>
		/// <param name="pSamples">Pointer at the 16-bit signed samples to be encoded. 
		/// InPtr is used to pass any type of array without need of make memory copy, 
		/// then gaining in performance. Note that nSamples is not the number of bytes,
		/// but samples (is sample is a SHORT)</param>
		/// <param name="pOutput">Buffer where to write the encoded data. 
		/// This buffer should be at least of the minimum size returned by BeInitStream().</param>
		/// <param name="pdwOutput">Returns the number of bytes of encoded data written. 
		/// The amount of data written might vary from chunk to chunk</param>
		/// <returns>On success: BeErrSuccessful</returns>
		[DllImport("Lame_enc.dll", EntryPoint = "beEncodeChunk")]
		public static extern uint BeEncodeChunk(uint hbeStream, uint nSamples, IntPtr pSamples, [In, Out] byte[] pOutput, ref uint pdwOutput);
		

		/// <summary>
		/// This function should be called after encoding the last chunk in order to flush 
		/// the encoder. It writes any encoded data that still might be left inside the 
		/// encoder to the output buffer. This function should NOT be called unless 
		/// you have encoded all of the chunks in your stream.
		/// </summary>
		/// <param name="hbeStream">Handle of the stream.</param>
		/// <param name="pOutput">Where to write the encoded data. This buffer should be 
		/// at least of the minimum size returned by BeInitStream().</param>
		/// <param name="pdwOutput">Returns number of bytes of encoded data written.</param>
		/// <returns>On success: BeErrSuccessful</returns>
		[DllImport("Lame_enc.dll", EntryPoint = "beDeinitStream")]
		public static extern uint BeDeinitStream(uint hbeStream, [In, Out] byte[] pOutput, ref uint pdwOutput);

		/// <summary>
		/// Last function to be called when finished encoding a stream. 
		/// Should unlike BeDeinitStream() also be called if the encoding is canceled.
		/// </summary>
		/// <param name="hbeStream">Handle of the stream.</param>
		/// <returns>On success: BeErrSuccessful</returns>
		[DllImport("Lame_enc.dll", EntryPoint = "beCloseStream")]
		public static extern uint BeCloseStream(uint hbeStream);
	}
}