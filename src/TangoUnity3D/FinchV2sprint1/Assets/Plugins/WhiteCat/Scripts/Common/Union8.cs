using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WhiteCat
{
	/// <summary>
	/// 8 字节联合体
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	[Serializable]
	public struct Union8
	{
		#region Values

		[FieldOffset(0)] [NonSerialized] public bool boolValue;
		[FieldOffset(0)] [NonSerialized] public bool bool0;
		[FieldOffset(1)] [NonSerialized] public bool bool1;
		[FieldOffset(2)] [NonSerialized] public bool bool2;
		[FieldOffset(3)] [NonSerialized] public bool bool3;
		[FieldOffset(4)] [NonSerialized] public bool bool4;
		[FieldOffset(5)] [NonSerialized] public bool bool5;
		[FieldOffset(6)] [NonSerialized] public bool bool6;
		[FieldOffset(7)] [NonSerialized] public bool bool7;

		[FieldOffset(0)] [NonSerialized] public byte byteValue;
		[FieldOffset(0)] [NonSerialized] public byte byte0;
		[FieldOffset(1)] [NonSerialized] public byte byte1;
		[FieldOffset(2)] [NonSerialized] public byte byte2;
		[FieldOffset(3)] [NonSerialized] public byte byte3;
		[FieldOffset(4)] [NonSerialized] public byte byte4;
		[FieldOffset(5)] [NonSerialized] public byte byte5;
		[FieldOffset(6)] [NonSerialized] public byte byte6;
		[FieldOffset(7)] [NonSerialized] public byte byte7;

		[FieldOffset(0)] [NonSerialized] public sbyte sbyteValue;
		[FieldOffset(0)] [NonSerialized] public sbyte sbyte0;
		[FieldOffset(1)] [NonSerialized] public sbyte sbyte1;
		[FieldOffset(2)] [NonSerialized] public sbyte sbyte2;
		[FieldOffset(3)] [NonSerialized] public sbyte sbyte3;
		[FieldOffset(4)] [NonSerialized] public sbyte sbyte4;
		[FieldOffset(5)] [NonSerialized] public sbyte sbyte5;
		[FieldOffset(6)] [NonSerialized] public sbyte sbyte6;
		[FieldOffset(7)] [NonSerialized] public sbyte sbyte7;

		[FieldOffset(0)] [NonSerialized] public char charValue;
		[FieldOffset(0)] [NonSerialized] public char char0;
		[FieldOffset(2)] [NonSerialized] public char char1;
		[FieldOffset(4)] [NonSerialized] public char char2;
		[FieldOffset(6)] [NonSerialized] public char char3;

		[FieldOffset(0)] [NonSerialized] public short shortValue;
		[FieldOffset(0)] [NonSerialized] public short short0;
		[FieldOffset(2)] [NonSerialized] public short short1;
		[FieldOffset(4)] [NonSerialized] public short short2;
		[FieldOffset(6)] [NonSerialized] public short short3;

		[FieldOffset(0)] [NonSerialized] public ushort ushortValue;
		[FieldOffset(0)] [NonSerialized] public ushort ushort0;
		[FieldOffset(2)] [NonSerialized] public ushort ushort1;
		[FieldOffset(4)] [NonSerialized] public ushort ushort2;
		[FieldOffset(6)] [NonSerialized] public ushort ushort3;

		[FieldOffset(0)] [NonSerialized] public int intValue;
		[FieldOffset(0)] [NonSerialized] public int int0;
		[FieldOffset(4)] [NonSerialized] public int int1;

		[FieldOffset(0)] [NonSerialized] public uint uintValue;
		[FieldOffset(0)] [NonSerialized] public uint uint0;
		[FieldOffset(4)] [NonSerialized] public uint uint1;

		[FieldOffset(0)] [NonSerialized] public float floatValue;
		[FieldOffset(0)] [NonSerialized] public float float0;
		[FieldOffset(4)] [NonSerialized] public float float1;

		[FieldOffset(0)]                 public long longValue;
		[FieldOffset(0)] [NonSerialized] public ulong ulongValue;
		[FieldOffset(0)] [NonSerialized] public double doubleValue;

		#endregion

		#region Construct & Implicit Operator

		/// <summary>
		/// 使用索引访问字节数据
		/// </summary>
		/// <param name="index"> 0~7 的索引值 </param>
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return byte0;
					case 1: return byte1;
					case 2: return byte2;
					case 3: return byte3;
					case 4: return byte4;
					case 5: return byte5;
					case 6: return byte6;
					case 7: return byte7;
					default: throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: byte0 = value; break;
					case 1: byte1 = value; break;
					case 2: byte2 = value; break;
					case 3: byte3 = value; break;
					case 4: byte4 = value; break;
					case 5: byte5 = value; break;
					case 6: byte6 = value; break;
					case 7: byte7 = value; break;
					default: throw new IndexOutOfRangeException();
				}
			}
		}

		public Union8(bool value) : this() { boolValue = value; }
		public Union8(byte value) : this() { byteValue = value; }
		public Union8(sbyte value) : this() { sbyteValue = value; }
		public Union8(char value) : this() { charValue = value; }
		public Union8(short value) : this() { shortValue = value; }
		public Union8(ushort value) : this() { ushortValue = value; }
		public Union8(int value) : this() { intValue = value; }
		public Union8(uint value) : this() { uintValue = value; }
		public Union8(float value) : this() { floatValue = value; }
		public Union8(long value) : this() { longValue = value; }
		public Union8(ulong value) : this() { ulongValue = value; }
		public Union8(double value) : this() { doubleValue = value; }

		public static implicit operator bool(Union8 value) { return value.boolValue; }
		public static implicit operator Union8(bool value) { return new Union8(value); }
		public static implicit operator byte(Union8 value) { return value.byteValue; }
		public static implicit operator Union8(byte value) { return new Union8(value); }
		public static implicit operator sbyte (Union8 value) { return value.sbyteValue; }
		public static implicit operator Union8(sbyte value) { return new Union8(value); }
		public static implicit operator char (Union8 value) { return value.charValue; }
		public static implicit operator Union8(char value) { return new Union8(value); }
		public static implicit operator short (Union8 value) { return value.shortValue; }
		public static implicit operator Union8(short value) { return new Union8(value); }
		public static implicit operator ushort (Union8 value) { return value.ushortValue; }
		public static implicit operator Union8(ushort value) { return new Union8(value); }
		public static implicit operator int (Union8 value) { return value.intValue; }
		public static implicit operator Union8(int value) { return new Union8(value); }
		public static implicit operator uint (Union8 value) { return value.uintValue; }
		public static implicit operator Union8(uint value) { return new Union8(value); }
		public static implicit operator float(Union8 value) { return value.floatValue; }
		public static implicit operator Union8(float value) { return new Union8(value); }
		public static implicit operator long (Union8 value) { return value.longValue; }
		public static implicit operator Union8(long value) { return new Union8(value); }
		public static implicit operator ulong (Union8 value) { return value.ulongValue; }
		public static implicit operator Union8(ulong value) { return new Union8(value); }
		public static implicit operator double (Union8 value) { return value.doubleValue; }
		public static implicit operator Union8(double value) { return new Union8(value); }

		#endregion

		#region ReadFrom & WriteTo

		/// <summary>
		/// 从字节数组复制数据
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 count </param>
		/// <param name="count"> 从字节数组复制数据的总字节数 </param>
		public void ReadFrom(IList<byte> buffer, ref int offset, int count)
		{
			int index = 0;
			while (index < count)
			{
				this[index++] = buffer[offset++];
            }
        }


		/// <summary>
		/// 将数据复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 count </param>
		/// <param name="count"> 复制数据到字节数组的总字节数 </param>
		public void WriteTo(IList<byte> buffer, ref int offset, int count)
		{
			int index = 0;
			while (index < count)
			{
				buffer[offset++] = this[index++];
			}
		}


		/// <summary>
		/// 从字节数组复制一个 bool 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 1 </param>
		public void ReadBoolFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
        }


		/// <summary>
		/// 将 bool 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 1 </param>
		public void WriteBoolTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
		}


		/// <summary>
		/// 从字节数组复制一个 byte 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 1 </param>
		public void ReadByteFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
		}


		/// <summary>
		/// 将 byte 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 1 </param>
		public void WriteByteTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
		}


		/// <summary>
		/// 从字节数组复制一个 sbyte 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 1 </param>
		public void ReadSByteFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
		}


		/// <summary>
		/// 将 sbyte 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 1 </param>
		public void WriteSByteTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
		}


		/// <summary>
		/// 从字节数组复制一个 char 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 2 </param>
		public void ReadCharFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
		}


		/// <summary>
		/// 将 char 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 2 </param>
		public void WriteCharTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
		}


		/// <summary>
		/// 从字节数组复制一个 short 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 2 </param>
		public void ReadShortFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
		}


		/// <summary>
		/// 将 short 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 2 </param>
		public void WriteShortTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
		}


		/// <summary>
		/// 从字节数组复制一个 ushort 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 2 </param>
		public void ReadUShortFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
		}


		/// <summary>
		/// 将 ushort 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 2 </param>
		public void WriteUShortTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
		}


		/// <summary>
		/// 从字节数组复制一个 int 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 4 </param>
		public void ReadIntFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
			byte2 = buffer[offset++];
			byte3 = buffer[offset++];
		}


		/// <summary>
		/// 将 int 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 4 </param>
		public void WriteIntTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
			buffer[offset++] = byte2;
			buffer[offset++] = byte3;
		}


		/// <summary>
		/// 从字节数组复制一个 uint 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 4 </param>
		public void ReadUIntFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
			byte2 = buffer[offset++];
			byte3 = buffer[offset++];
		}


		/// <summary>
		/// 将 uint 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 4 </param>
		public void WriteUIntTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
			buffer[offset++] = byte2;
			buffer[offset++] = byte3;
		}


		/// <summary>
		/// 从字节数组复制一个 float 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 4 </param>
		public void ReadFloatFrom(IList<byte> buffer, ref int offset)
		{
			byte0 = buffer[offset++];
			byte1 = buffer[offset++];
			byte2 = buffer[offset++];
			byte3 = buffer[offset++];
		}


		/// <summary>
		/// 将 float 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 4 </param>
		public void WriteFloatTo(IList<byte> buffer, ref int offset)
		{
			buffer[offset++] = byte0;
			buffer[offset++] = byte1;
			buffer[offset++] = byte2;
			buffer[offset++] = byte3;
		}


		/// <summary>
		/// 从字节数组复制一个 long 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 8 </param>
		public void ReadLongFrom(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				this[index++] = buffer[offset++];
			}
		}


		/// <summary>
		/// 将 long 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 8 </param>
		public void WriteLongTo(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				buffer[offset++] = this[index++];
			}
		}


		/// <summary>
		/// 从字节数组复制一个 ulong 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 8 </param>
		public void ReadULongFrom(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				this[index++] = buffer[offset++];
			}
		}


		/// <summary>
		/// 将 ulong 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 8 </param>
		public void WriteULongTo(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				buffer[offset++] = this[index++];
			}
		}


		/// <summary>
		/// 从字节数组复制一个 double 值
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组复制数据的开始下标, 操作完成后增加 8 </param>
		public void ReadDoubleFrom(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				this[index++] = buffer[offset++];
			}
		}


		/// <summary>
		/// 将 double 值复制到字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 复制数据到字节数组的开始下标, 操作完成后增加 8 </param>
		public void WriteDoubleTo(IList<byte> buffer, ref int offset)
		{
			int index = 0;
			while (index < 8)
			{
				buffer[offset++] = this[index++];
			}
		}


		/// <summary>
		/// 从流读取数据
		/// </summary>
		/// <param name="stream"> 数据流 </param>
		/// <param name="count"> 从流读取数据的总字节数 </param>
		public void ReadFrom(Stream stream, int count)
		{
			int read;
			for (int index = 0; index < count; index++)
			{
				if ((read = stream.ReadByte()) == -1)
				{
					throw new EndOfStreamException();
				}
				this[index] = (byte)read;
            }
		}


		/// <summary>
		/// 写入数据到流
		/// </summary>
		/// <param name="stream"> 数据流 </param>
		/// <param name="count"> 写入数据到流的总字节数 </param>
		public void WriteTo(Stream stream, int count)
		{
			for (int index = 0; index < count; index++)
			{
				stream.WriteByte(this[index]);
			}
		}

		#endregion

	} // struct Union8

} // namespace WhiteCat