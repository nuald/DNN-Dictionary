//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2003 Idael Cardoso. 
//

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Dictionary {

	abstract class AudioWriter : BinaryWriter {
		
		protected WaveFormat _inputDataFormat;

		public AudioWriter(Stream output, WaveFormat inputDataFormat)
			: base(output, Encoding.ASCII) {
			_inputDataFormat = inputDataFormat;
		}

		protected abstract int GetOptimalBufferSize();

		protected virtual AudioWriterConfig GetWriterConfig() {
			return new AudioWriterConfig(_inputDataFormat);
		}

		/// <summary>
		/// Optimal size of the buffer used in each write operation to obtain best performance.
		/// This value must be greater than 0 
		/// </summary>
		public int OptimalBufferSize {
			get {
				return GetOptimalBufferSize();
			}
		}

		public override void Write(string value) {
			throw new NotSupportedException(
				Convert.ToString("Write(string value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(float value) {
			throw new NotSupportedException(
				Convert.ToString("Write(float value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(ulong value) {
			throw new NotSupportedException(
				Convert.ToString("Write(ulong value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(long value) {
			throw new NotSupportedException(
				Convert.ToString("Write(long value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(uint value) {
			throw new NotSupportedException(
				Convert.ToString("Write(uint value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(int value) {
			throw new NotSupportedException(
				Convert.ToString("Write(int value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(ushort value) {
			throw new NotSupportedException(
				Convert.ToString("Write(ushort value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(short value) {
			throw new NotSupportedException(
				Convert.ToString("Write(short value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(decimal value) {
			throw new NotSupportedException(
				Convert.ToString("Write(decimal value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(double value) {
			throw new NotSupportedException(
				Convert.ToString("Write(double value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(char[] chars, int index, int count) {
			throw new NotSupportedException(
				Convert.ToString("Write(char[] chars, int index, int count) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(char[] chars) {
			throw new NotSupportedException(
				Convert.ToString("Write(char[] chars) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(char ch) {
			throw new NotSupportedException(
				Convert.ToString("Write(char ch) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(sbyte value) {
			throw new NotSupportedException(
				Convert.ToString("Write(sbyte value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(byte value) {
			throw new NotSupportedException(
				Convert.ToString("Write(byte value) is not supported", 
				CultureInfo.CurrentCulture));
		}

		public override void Write(bool value) {
			throw new NotSupportedException(
				Convert.ToString("Write(bool value) is not supported", 
				CultureInfo.CurrentCulture));
		}
	}
}