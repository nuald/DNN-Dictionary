//  SEE  http://www.mp3dev.org/ FOR TECHNICAL AND COPYRIGHT INFORMATION REGARDING LAME PROJECT.
//
//  About Thomson and/or Fraunhofer patents:
//  Any use of this product does not convey a license under the relevant 
//  intellectual property of Thomson and/or Fraunhofer Gesellschaft nor imply 
//  any right to use this product in any finished end user or ready-to-use final 
//  product. An independent license for such use is required. 
//  For details, please visit http://www.mp3licensing.com.

using System;
using System.Globalization;
using System.IO;
using Dictionary.Common;

namespace Dictionary {

	//Wave Format
	public enum WaveFormats {
		None = 0,
		Pcm = 1,
		Float = 3
	}

	//SAPI
	/// <summary>
	/// Convert PCM audio data to PCM format
	/// The data received through the method write is assumed as PCM audio data. 
	/// This data is converted to SAPI format and written to the result stream. 
	/// </summary>
	class Mp3Writer : AudioWriter {
		bool closed;
		Be_Config _mp3Config;
		uint _hLameStream;
		uint _inputSamples;
		uint _outBufferSize;
		byte[] _inBuffer;
		int _inBufferPos;
		byte[] _outBuffer;

		/// <summary>
		/// Create a Mp3Writer with specific SAPI format
		/// </summary>
		/// <param name="output">Stream that will hold the SAPI resulting data</param>
		/// <param name="cfg">Writer Config</param>
		public Mp3Writer(Stream output, Mp3WriterConfig cfg)
			: this(output, cfg.Format, cfg.Mp3Config) {
		}

		/// <summary>
		/// Create a Mp3Writer with specific SAPI format
		/// </summary>
		/// <param name="output">Stream that will hold the SAPI resulting data</param>
		/// <param name="inputDataFormat">PCM format of input data</param>
		/// <param name="mp3Config">Desired SAPI config</param>
		public Mp3Writer(Stream output, WaveFormat inputDataFormat, Be_Config mp3Config)
			: base(output, inputDataFormat) {
			_mp3Config = mp3Config;
			uint lameResult = NativeMethods.BeInitStream(_mp3Config, ref _inputSamples, 
				ref _outBufferSize, ref _hLameStream);
			if (lameResult != LameEncDll.BeErrSuccessful) {
				throw new InvalidOperationException(
					String.Format(CultureInfo.CurrentCulture,
					"LameEncDll.BeInitStream failed with the error code {0}",
					lameResult));
			}
			_inBuffer = new byte[_inputSamples * 2]; //Input buffer is expected as short[]
			_outBuffer = new byte[_outBufferSize];
		}

		/// <summary>
		/// SAPI Config of final data
		/// </summary>
		Be_Config Mp3Config {
			get {
				return _mp3Config;
			}
		}

		protected override int GetOptimalBufferSize() {
			return _inBuffer.Length;
		}

		public override void Close() {
			if (!closed) {
				try {
					TryClose();
				} finally {
					NativeMethods.BeCloseStream(_hLameStream);
				}
			}
			closed = true;
			base.Close();
		}

		void TryClose() {
			uint encodedSize = 0;
			if (_inBufferPos > 0) {
				uint result = 
					LameEncDll.EncodeChunk(_hLameStream, _inBuffer, 0, 
					(uint)_inBufferPos, _outBuffer, ref encodedSize);
				if (result == LameEncDll.BeErrSuccessful && encodedSize > 0) {
					base.Write(_outBuffer, 0, (int)encodedSize);
				}
			}
			encodedSize = 0;
			uint resultForDeInit = 
				NativeMethods.BeDeinitStream(_hLameStream, _outBuffer, ref encodedSize);
			if (resultForDeInit == LameEncDll.BeErrSuccessful && encodedSize > 0) {
				base.Write(_outBuffer, 0, (int)encodedSize);
			}
		}


		/// <summary>
		/// Send to the compressor an array of bytes.
		/// </summary>
		/// <param name="buffer">Input buffer</param>
		/// <param name="index">Start position</param>
		/// <param name="count">Bytes to process. The optimal size, to avoid buffer copy, is a multiple of</param>
		public override void Write(byte[] buffer, int index, int count) {
			uint encodedSize = 0;
			while (count > 0) {
				if (!WriteAtPos(buffer, ref index, ref count, ref encodedSize)) {
					return;
				}
			}
		}

		bool WriteAtPos(byte[] buffer, ref int index, ref int count, ref uint encodedSize) {
			if (_inBufferPos > 0) {
				if (!WriteInMiddle(buffer, ref index, ref count, ref encodedSize)) {
					return false;
				}
			} else {
				if (!WriteInBegin(buffer, ref index, ref count, ref encodedSize)) {
					return false;
				}
			}
			return true;
		}

		bool WriteInBegin(byte[] buffer, ref int index, ref int count, ref uint encodedSize) {
			if (count >= _inBuffer.Length) {
				uint lameResult = LameEncDll.EncodeChunk(_hLameStream, buffer,
					index, (uint)_inBuffer.Length, _outBuffer, ref encodedSize);
				if (lameResult == LameEncDll.BeErrSuccessful && encodedSize > 0) {
					base.Write(_outBuffer, 0, (int)encodedSize);
				} else {
					return false;
				}
				count -= _inBuffer.Length;
				index += _inBuffer.Length;
			} else {
				Buffer.BlockCopy(buffer, index, _inBuffer, 0, count);
				_inBufferPos = count;
				index += count;
				count = 0;
			}
			return true;
		}

		bool WriteInMiddle(byte[] buffer, ref int index, ref int count, ref uint encodedSize) {
			int toCopy = Math.Min(count, _inBuffer.Length - _inBufferPos);
			Buffer.BlockCopy(buffer, index, _inBuffer, _inBufferPos, toCopy);
			_inBufferPos += toCopy;
			index += toCopy;
			count -= toCopy;
			if (_inBufferPos >= _inBuffer.Length) {
				_inBufferPos = 0;
				uint lameResult = LameEncDll.EncodeChunk(_hLameStream, _inBuffer,
					_outBuffer, ref encodedSize);
				if (lameResult == LameEncDll.BeErrSuccessful && encodedSize > 0) {
					base.Write(_outBuffer, 0, (int)encodedSize);
				} else {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Send to the compressor an array of bytes.
		/// </summary>
		/// <param name="buffer">The optimal size, to avoid buffer copy, is a multiple of</param>
		public override void Write(byte[] buffer) {
			Write(buffer, 0, buffer.Length);
		}

		protected override AudioWriterConfig GetWriterConfig() {
			return new Mp3WriterConfig(_inputDataFormat, Mp3Config);
		}
	}
}