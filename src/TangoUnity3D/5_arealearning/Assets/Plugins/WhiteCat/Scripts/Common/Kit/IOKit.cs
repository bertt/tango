//using System;
//using System.IO;
//using System.Text;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteCat
{
	/// <summary>
	/// IO 相关的方法
	/// </summary>
	public partial struct Kit
	{
		///// <summary>
		///// 将可序列化的对象序列化为字节数组
		///// </summary>
		///// <param name="obj"> 执行序列化的对象 </param>
		///// <param name="array"> 接收返回的序列化数组(操作失败返回 null) </param>
		///// <param name="count"> 接受返回的序列化数组的有效长度(操作失败返回 0) </param>
		///// <returns> 操作成功返回 true, 否则返回 false </returns>
		//public static bool Serialize(object obj, out byte[] array, out int count)
		//{
		//	try
		//	{
		//		using (MemoryStream ms = new MemoryStream())
		//		{
		//			new BinaryFormatter().Serialize(ms, obj);
		//			array = ms.GetBuffer();
		//			count = (int)ms.Length;
		//			return true;
		//		}
		//	}
		//	catch
		//	{
		//		array = null;
		//		count = 0;
		//		return false;
		//	}
		//}


		///// <summary>
		///// 将字节数组反序列化为可序列化的对象
		///// </summary>
		///// <param name="index"> 数组中序列化数据开始下标 </param>
		///// <param name="count"> 数组中序列化数据字节计数, 非正值表示直到数组尾部 </param>
		///// <returns> 如果反序列化成功, 返回反序列化获得的对象; 否则返回 null </returns>
		//public static object Deserialize(byte[] array, int index = 0, int count = 0)
		//{
		//	try
		//	{
		//		using (MemoryStream ms = new MemoryStream(array, index, count > 0 ? count : (array.Length - index)))
		//		{
		//			return new BinaryFormatter().Deserialize(ms);
		//		}
		//	}
		//	catch { return null; }
		//}


		/// <summary> 对字节数组加密或解密 </summary>
		/// <param name="key"> 密钥. 同一个密钥使用偶数次可以消除这个密钥对数据的影响 </param>
		/// <param name="array"> 执行加密或解密的数组 </param>
		/// <param name="index"> 数组开始加密或解密的下标 </param>
		/// <param name="count"> 需要加密或解密的字节总数, 非正值表示直到数组尾部 </param>
		/// <returns> 校验码. 如果同一个密钥在两次处理数据之间其他密钥处理次数都是偶数, 那么这两次返回的校验码是相同的. 校验码可以用来判断数据是否被修改 </returns>
		public static int EncryptDecrypt(uint key, byte[] array, int index = 0, int count = 0)
		{
			Random random = new Random(key);

			byte byte0 = (byte)random.Range(0, 256);
			byte byte1 = (byte)random.Range(0, 256);
			byte byte2 = byte0;
			byte byte3 = byte1;

			if (count > 0) count += index;
			else count = array.Length;

			for (int i = index; i < count; i++)
			{
				byte0 += array[i];
				byte1 -= array[i];

				array[i] ^= (byte)random.Range(0, 256);

				byte2 += array[i];
				byte3 -= array[i];
			}

			if (byte0 > byte2) Swap(ref byte0, ref byte2);
			if (byte1 < byte3) Swap(ref byte1, ref byte3);

			return (byte0 << 24) | (byte1 << 16) | (byte2 << 8) | (int)byte3;
		}


		///// <summary>
		///// 读文件
		///// </summary>
		///// <param name="filePath"> 文件完整路径 </param>
		///// <param name="read"> 自定义的读取方法, 使用 BinaryReader 的 Read 方法来完成 </param>
		//public static void ReadFile(string filePath, Action<BinaryReader> read, Encoding encoding = null)
		//{
		//	using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		//	{
		//		using (BinaryReader reader = encoding == null ? new BinaryReader(fs) : new BinaryReader(fs, encoding))
		//		{
		//			read(reader);
		//		}
		//	}
		//}


		///// <summary>
		///// 写文件
		///// </summary>
		///// <param name="filePath"> 文件完整路径 </param>
		///// <param name="write"> 自定义的写入方法, 使用 BinaryWriter 的 Write 方法来完成 </param>
		//public static void WriteFile(string filePath, Action<BinaryWriter> write, Encoding encoding = null)
		//{
		//	using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
		//	{
		//		using (BinaryWriter writer = encoding == null ? new BinaryWriter(fs) : new BinaryWriter(fs, encoding))
		//		{
		//			write(writer);
		//		}
		//	}
		//}


		///// <summary>
		///// 将一个可序列化的对象写入文件
		///// </summary>
		///// <param name="obj"> 需要写入的对象 </param>
		///// <param name="filePath"> 文件完整路径 </param>
		///// <param name="encrypt"> 是否需要加密 </param>
		///// <returns> 如果操作成功返回 true，否则返回 false </returns>
		//public static bool WriteObjectToFile(object obj, string filePath, bool encrypt = false)
		//{
		//	try
		//	{
		//		WriteFile(filePath, writer =>
		//		{
		//			byte[] array;
		//			int count;
		//			Serialize(obj, out array, out count);
		//			if (encrypt)
		//			{
		//				uint key = new Random().seed;
		//				int code = EncryptDecrypt(key, array, 0, count);
		//				writer.Write(key);
		//				writer.Write(array, 0, count);
		//				writer.Write(code);
		//			}
		//			else writer.Write(array, 0, count);
		//		});
		//		return true;
		//	}
		//	catch { return false; }
		//}


		///// <summary>
		///// 从文件中读取一个可序列化的对象
		///// </summary>
		///// <param name="filePath"> 文件完整路径 </param>
		///// <param name="decrypt"> 是否需要解密 </param>
		///// <returns> 如果操作成功返回对象，否则返回 null </returns>
		//public static object ReadObjectFromFile(string filePath, bool decrypt = false)
		//{
		//	try
		//	{
		//		byte[] array = null;
		//		ReadFile(filePath, reader =>
		//		{
		//			if (decrypt)
		//			{
		//				uint key = reader.ReadUInt32();
		//				array = reader.ReadBytes((int)reader.BaseStream.Length - 8);
		//				int code = reader.ReadInt32();
		//				if (EncryptDecrypt(key, array) != code) array = null;
		//			}
		//			else array = reader.ReadBytes((int)reader.BaseStream.Length);
		//		});
		//		return Deserialize(array);
		//	}
		//	catch { return null; }
		//}


		/// <summary>
		/// 将 Vector3 值写入字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 写入字节数组的开始下标, 操作完成后增加 12 </param>
		/// <param name="value"> 被写入的值 </param>
		public static void WriteToBuffer(IList<byte> buffer, ref int offset, Vector3 value)
		{
			Union8 union = new Union8();

			union.floatValue = value.x;
            union.WriteFloatTo(buffer, ref offset);

			union.floatValue = value.y;
			union.WriteFloatTo(buffer, ref offset);

			union.floatValue = value.z;
			union.WriteFloatTo(buffer, ref offset);
		}


		/// <summary>
		/// 从字节数组里读取 Vector3
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组里开始读取的下标, 操作完成后增加 12 </param>
		/// <returns> 读取的 Vector3 值 </returns>
		public static Vector3 ReadVector3FromBuffer(IList<byte> buffer, ref int offset)
		{
			Vector3 value = new Vector3();
			Union8 union = new Union8();

			union.ReadFloatFrom(buffer, ref offset);
			value.x = union.floatValue;

			union.ReadFloatFrom(buffer, ref offset);
			value.y = union.floatValue;

			union.ReadFloatFrom(buffer, ref offset);
			value.z = union.floatValue;

			return value;
        }


		/// <summary>
		/// 将 Quaternion 值写入字节数组
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 写入字节数组的开始下标, 操作完成后增加 16 </param>
		/// <param name="value"> 被写入的值 </param>
		public static void WriteToBuffer(IList<byte> buffer, ref int offset, Quaternion value)
		{
			Union8 union = new Union8();

			union.floatValue = value.x;
			union.WriteFloatTo(buffer, ref offset);

			union.floatValue = value.y;
			union.WriteFloatTo(buffer, ref offset);

			union.floatValue = value.z;
			union.WriteFloatTo(buffer, ref offset);

			union.floatValue = value.w;
			union.WriteFloatTo(buffer, ref offset);
		}


		/// <summary>
		/// 从字节数组里读取 Quaternion
		/// </summary>
		/// <param name="buffer"> 字节数组 </param>
		/// <param name="offset"> 从字节数组里开始读取的下标, 操作完成后增加 16 </param>
		/// <returns> 读取的 Quaternion 值 </returns>
		public static Quaternion ReadQuaternionFromBuffer(IList<byte> buffer, ref int offset)
		{
			Quaternion value = new Quaternion();
			Union8 union = new Union8();

			union.ReadFloatFrom(buffer, ref offset);
			value.x = union.floatValue;

			union.ReadFloatFrom(buffer, ref offset);
			value.y = union.floatValue;

			union.ReadFloatFrom(buffer, ref offset);
			value.z = union.floatValue;

			union.ReadFloatFrom(buffer, ref offset);
			value.w = union.floatValue;

			return value;
		}

	} // struct Kit

} // namespace WhiteCat