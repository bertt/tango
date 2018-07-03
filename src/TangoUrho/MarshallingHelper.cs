using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace App1
{
    public class MarshallingHelper
    {
        /// <summary>
        /// Adds the contents of an unmanaged struct array to a list.
        /// </summary>
        /// <param name="arrayPtr">A pointer to the unmanged array.</param>
        /// <param name="arrayLength">The length of the unmanaged array.</param>
        /// <param name="list">A list to append array elements to.</param>
        /// <typeparam name="T">The type contained by the unmanaged array.</typeparam>
        public static void AddUnmanagedStructArrayToList<T>(IntPtr arrayPtr, int arrayLength, List<T> list) where T : struct
        {
            if (arrayPtr == IntPtr.Zero || list == null)
            {
                return;
            }

            for (int i = 0; i < arrayLength; i++)
            {
                list.Add((T)Marshal.PtrToStructure(GetPtrToUnmanagedArrayElement<T>(arrayPtr, i), typeof(T)));
            }
        }

        /// <summary>
        /// Returns a pointer to an element within an unmanaged array.
        /// </summary>
        /// <returns>A pointer to the desired unmanaged array element.</returns>
        /// <param name="arrayPtr">A pointer to the start of the array.</param>
        /// <param name="arrayIndex">The index of the desired element pointer.</param>
        /// <typeparam name="T">The type contained by the unmanaged array.</typeparam>
        public static IntPtr GetPtrToUnmanagedArrayElement<T>(IntPtr arrayPtr, int arrayIndex) where T : struct
        {
            return new IntPtr(arrayPtr.ToInt64() + (Marshal.SizeOf(typeof(T)) * arrayIndex));
        }
    }
}