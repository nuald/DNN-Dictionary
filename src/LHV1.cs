using System.Runtime.InteropServices;
using System;
using System.Globalization;
using System.Security.Permissions;

namespace Dictionary {

	[StructLayout(LayoutKind.Sequential, Size = 327), Serializable]
	public struct LHV1 { // Be_Config_LAME LAME header version 1
		public const uint MPEG1 = 1;
		public const uint MPEG2 = 0;

		// STRUCTURE INFORMATION
		public uint dwStructVersion;
		public uint dwStructSize;
		// BASIC ENCODER SETTINGS
		public uint dwSampleRate;		// SAMPLERATE OF INPUT FILE
		public uint dwReSampleRate;		// DOWNSAMPLERATE, 0=ENCODER DECIDES  
		public MpegMode nMode;				// Stereo, Mono
		public uint dwBitrate;			// CBR bitrate, VBR min bitrate
		public uint dwMaxBitrate;		// CBR ignored, VBR Max bitrate
		public LameQualityPreset nPreset;			// Quality preset
		public uint dwMpegVersion;		// MPEG-1 OR MPEG-2
		public uint dwPsyModel;			// FUTURE USE, SET TO 0
		public uint dwEmphasis;			// FUTURE USE, SET TO 0
		// BIT STREAM SETTINGS
		public int bPrivate;			// Set Private Bit (TRUE/FALSE)
		public int bCRC;				// Insert CRC (TRUE/FALSE)
		public int bCopyright;			// Set Copyright Bit (TRUE/FALSE)
		public int bOriginal;			// Set Original Bit (TRUE/FALSE)
		// VBR STUFF
		public int bWriteVBRHeader;	// WRITE XING VBR HEADER (TRUE/FALSE)
		public int bEnableVBR;			// USE VBR ENCODING (TRUE/FALSE)
		public int nVBRQuality;		// VBR QUALITY 0..9
		public uint dwVbrAbr_bps;		// Use ABR in stead of nVBRQuality
		public VbrMethod nVbrMethod;
		public int bNoRes;				// Disable Bit resorvoir (TRUE/FALSE)
		// MISC SETTINGS
		public int bStrictIso;			// Use strict ISO encoding rules (TRUE/FALSE)
		public ushort nQuality;			// Quality Setting, HIGH BYTE should be NOT LOW byte, otherwhise quality=5
		// FUTURE USE, SET TO 0, align strucutre to 331 bytes

		public LHV1(WaveFormat format, uint mpeBitRate) {
			IFormatProvider culture = CultureInfo.CurrentCulture;
			if (format.wFormatTag != (short)WaveFormats.Pcm) {
				throw new ArgumentOutOfRangeException(
					Convert.ToString("format", culture),
					Convert.ToString("Only PCM format supported", culture));
			}
			if (format.wBitsPerSample != 16) {
				throw new ArgumentOutOfRangeException(
					Convert.ToString("format", culture),
					Convert.ToString("Only 16 bits samples supported", culture));
			}
			dwStructVersion = 1;
			dwStructSize = GetConfigSize();
			dwMpegVersion = InitMpegVersion(format);
			dwSampleRate = (uint)format.nSamplesPerSec;				// INPUT FREQUENCY
			dwReSampleRate = 0;					// DON'T RESAMPLE
			nMode = InitMpegMode(format);
			ValidateBitRate(mpeBitRate, dwMpegVersion);
			dwBitrate = mpeBitRate;					// MINIMUM BIT RATE
			nPreset = LameQualityPreset.LqpNormalQuality;		// QUALITY PRESET SETTING
			dwPsyModel = 0;					// USE DEFAULT PSYCHOACOUSTIC MODEL 
			dwEmphasis = 0;					// NO EMPHASIS TURNED ON
			bOriginal = 1;					// SET ORIGINAL FLAG
			bWriteVBRHeader = 0;
			bNoRes = 0;					// No Bit resorvoir
			bCopyright = 0;
			bCRC = 0;
			bEnableVBR = 0;
			bPrivate = 0;
			bStrictIso = 0;
			dwMaxBitrate = 0;
			dwVbrAbr_bps = 0;
			nQuality = 0;
			nVbrMethod = VbrMethod.None;
			nVBRQuality = 0;
		}

		static uint GetConfigSize() {
			SecurityPermission permission =
				new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
			permission.Demand();
			return (uint)Marshal.SizeOf(typeof(Be_Config));
		}

		static MpegMode InitMpegMode(WaveFormat format) {
			MpegMode nMode;
			switch (format.nChannels) {
				case 1:
					nMode = MpegMode.Mono;
					break;
				case 2:
					nMode = MpegMode.Stereo;
					break;
				default:
					IFormatProvider culture = CultureInfo.CurrentCulture;
					throw new ArgumentOutOfRangeException(
						Convert.ToString("format", culture),
						Convert.ToString("Invalid number of channels", culture));
			}
			return nMode;
		}

		static bool FirstValidation(uint mpeBitRate) {
			switch (mpeBitRate) {
				case 32:
				case 40:
				case 48:
				case 56:
				case 64:
				case 80:
				case 96:
				case 112:
				case 128:
				case 160: //Allowed bit rates in MPEG1 and MPEG2
					return true;
				default:
					return false;
			}
		}

		static void ValidateBitRate(uint mpeBitRate, uint dwMpegVersion) {
			if (FirstValidation(mpeBitRate)) {
				return;
			}
			IFormatProvider culture = CultureInfo.CurrentCulture;
			switch (mpeBitRate) {
				case 192:
				case 224:
				case 256:
				case 320: //Allowed only in MPEG1
					if (dwMpegVersion != MPEG1) {
						throw new ArgumentOutOfRangeException(
							Convert.ToString("MpsBitRate", culture),
							Convert.ToString("Bit rate not compatible with input format",
							culture));
					}
					break;
				case 8:
				case 16:
				case 24:
				case 144: //Allowed only in MPEG2
					if (dwMpegVersion != MPEG2) {
						throw new ArgumentOutOfRangeException(
							Convert.ToString("MpsBitRate", culture),
							Convert.ToString("Bit rate not compatible with input format",
							culture));
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(
						Convert.ToString("MpsBitRate", culture),
						Convert.ToString("Unsupported bit rate", culture));
			}
		}

		static uint InitMpegVersion(WaveFormat format) {
			uint dwMpegVersion;
			switch (format.nSamplesPerSec) {
				case 16000:
				case 22050:
				case 24000:
					dwMpegVersion = MPEG2;
					break;
				case 32000:
				case 44100:
				case 48000:
					dwMpegVersion = MPEG1;
					break;
				default:
					IFormatProvider culture = CultureInfo.CurrentCulture;
					throw new ArgumentOutOfRangeException(
						Convert.ToString("format", culture),
						Convert.ToString("Unsupported sample rate", culture));
			}
			return dwMpegVersion;
		}
	}

}