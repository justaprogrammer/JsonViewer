using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace EPocalipse.Json.Viewer
{
    public class PhpSerializer
    {
        // Fields
        private const byte __0 = 0x30;
        private const byte __1 = 0x31;
        private const byte __a = 0x61;
        private const byte __b = 0x62;
        private const byte __C = 0x43;
        private const byte __Colon = 0x3a;
        private const byte __d = 100;
        private const byte __i = 0x69;
        private const byte __LeftB = 0x7b;
        private const byte __N = 0x4e;
        private static Hashtable __ns = new Hashtable();
        private const byte __O = 0x4f;
        private const byte __Quote = 0x22;
        private const byte __r = 0x72;
        private const byte __R = 0x52;
        private const byte __RightB = 0x7d;
        private const byte __s = 0x73;
        private const byte __Semicolon = 0x3b;
        private const byte __Slash = 0x5c;
        private const byte __U = 0x55;

        // Methods
        static PhpSerializer()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Module[] modules = assemblies[i].GetModules();
                for (int j = 0; j < modules.Length; j++)
                {
                    try
                    {
                        Type[] types = modules[j].GetTypes();
                        for (int k = 0; k < types.Length; k++)
                        {
                            if ((types[k].Namespace != null) && !__ns.ContainsKey(types[k].Namespace))
                            {
                                __ns[types[k].Namespace] = assemblies[i].FullName;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private PhpSerializer()
        {
        }

        private static Array ArrayListToArray(ArrayList obj, int rank, Type elementType)
        {
            int[] lengths = new int[rank];
            ArrayList list = obj;
            for (int i = 0; i < rank; i++)
            {
                lengths[i] = list.Count;
                if (list.Count <= 0)
                {
                    break;
                }
                list = list[0] as ArrayList;
            }
            Array dest = Array.CreateInstance(elementType, lengths);
            if (lengths[0] > 0)
            {
                int[] indices = new int[1];
                ArrayListToArray(obj, dest, indices, elementType);
            }
            return dest;
        }

        private static void ArrayListToArray(ArrayList source, Array dest, int[] indices, Type elementType)
        {
            int length = indices.Length;
            int dimension = length - 1;
            int[] array = new int[length + 1];
            indices.CopyTo(array, 0);
            int num3 = dest.GetLength(dimension);
            for (int i = 0; i < num3; i++)
            {
                if (dest.Rank == length)
                {
                    indices[length - 1] = i;
                    if (((source[i] == null) || (elementType == typeof (object))) ||
                        (elementType == source[i].GetType()))
                    {
                        dest.SetValue(source[i], indices);
                    }
                    else
                    {
                        dest.SetValue(ChangeType(source[i], elementType), indices);
                    }
                }
                else
                {
                    array[length - 1] = i;
                    ArrayListToArray(source[i] as ArrayList, dest, array, elementType);
                }
            }
        }

        private static Hashtable ArrayListToHashtable(ArrayList a)
        {
            int count = a.Count;
            Hashtable hashtable = new Hashtable(count);
            for (int i = 0; i < count; i++)
            {
                hashtable[i] = a[i];
            }
            return hashtable;
        }

        public static object ChangeType(object obj, Type destType)
        {
            if (obj == null)
            {
                return null;
            }
            Type type = obj.GetType();
            if (type == destType)
            {
                return obj;
            }
            if ((obj is ArrayList) && destType.IsArray)
            {
                return ArrayListToArray(obj as ArrayList, destType.GetArrayRank(), destType.GetElementType());
            }
            if ((obj is ArrayList) && (destType == typeof (Hashtable)))
            {
                return ArrayListToHashtable(obj as ArrayList);
            }
            if ((type.IsByRef || !destType.IsByRef) && (type.IsPointer || !destType.IsPointer))
            {
                return Convert.ChangeType(obj, destType);
            }
            return Convert.ChangeType(obj, destType.GetElementType());
        }

        private static object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
            }
            try
            {
                return Activator.CreateInstance(type, new object[] {0});
            }
            catch
            {
            }
            try
            {
                return Activator.CreateInstance(type, new object[] {false});
            }
            catch
            {
            }
            try
            {
                return Activator.CreateInstance(type, new object[] {""});
            }
            catch
            {
            }
            try
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                int num = 0;
                int num2 = fields.Length;
                while (num < num2)
                {
                    if (fields[num].FieldType == type)
                    {
                        return fields[num].GetValue(null);
                    }
                    num++;
                }
            }
            catch
            {
            }
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            int index = 0;
            int length = methods.Length;
            while (index < length)
            {
                if (methods[index].ReturnType == type)
                {
                    try
                    {
                        return methods[index].Invoke(null, null);
                    }
                    catch
                    {
                    }
                    try
                    {
                        return methods[index].Invoke(null, new object[] {0});
                    }
                    catch
                    {
                    }
                    try
                    {
                        return methods[index].Invoke(null, new object[] {false});
                    }
                    catch
                    {
                    }
                    try
                    {
                        return methods[index].Invoke(null, new object[] {""});
                    }
                    catch
                    {
                    }
                }
                index++;
            }
            ThrowError("Can't create the instance of " + type.FullName);
            return null;
        }

        private static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName, false, true);
            if (type != null)
            {
                return type;
            }
            foreach (DictionaryEntry entry in __ns)
            {
                type = Type.GetType(((string) entry.Key) + "." + typeName + ", " + ((string) entry.Value), false, true);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        private static object ReadArray(MemoryStream stream, Hashtable ht, ref int hv, Hashtable rt, Encoding encoding)
        {
            stream.Seek(1L, SeekOrigin.Current);
            int capacity = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            Hashtable hashtable = new Hashtable(capacity);
            ArrayList list = new ArrayList(capacity);
            int key = hv;
            rt.Add(key, false);
            long position = stream.Position;
            ht[(int) hv++] = hashtable;
            for (int i = 0; i < capacity; i++)
            {
                object obj2;
                switch (stream.ReadByte())
                {
                    case 0x55:
                        obj2 = ReadUnicodeString(stream);
                        break;

                    case 0x69:
                        obj2 = Convert.ToInt32(ReadInteger(stream));
                        break;

                    case 0x73:
                        obj2 = ReadString(stream, encoding);
                        break;

                    default:
                        ThrowError("Wrong Serialize Stream!");
                        return null;
                }
                object obj3 = UnSerialize(stream, ht, ref hv, rt, encoding);
                if (list != null)
                {
                    if ((obj2 is int) && (((int) obj2) == i))
                    {
                        list.Add(obj3);
                    }
                    else
                    {
                        list = null;
                    }
                }
                hashtable[obj2] = obj3;
            }
            if (list != null)
            {
                ht[key] = list;
                if ((bool) rt[key])
                {
                    hv = key + 1;
                    stream.Position = position;
                    for (int j = 0; j < capacity; j++)
                    {
                        int num6;
                        if (stream.ReadByte() == 0x69)
                        {
                            num6 = Convert.ToInt32(ReadInteger(stream));
                        }
                        else
                        {
                            ThrowError("Wrong Serialize Stream!");
                            return null;
                        }
                        list[num6] = UnSerialize(stream, ht, ref hv, rt, encoding);
                    }
                }
            }
            rt.Remove(key);
            stream.Seek(1L, SeekOrigin.Current);
            return ht[key];
        }

        private static bool ReadBoolean(MemoryStream stream)
        {
            stream.Seek(1L, SeekOrigin.Current);
            bool flag = stream.ReadByte() == 0x31;
            stream.Seek(1L, SeekOrigin.Current);
            return flag;
        }

        private static object ReadCustomObject(MemoryStream stream, Hashtable ht, ref int hv, Encoding encoding)
        {
            object obj2;
            stream.Seek(1L, SeekOrigin.Current);
            int count = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            string typeName = encoding.GetString(buffer);
            stream.Seek(2L, SeekOrigin.Current);
            int num2 = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            Type type = GetType(typeName);
            if (type != null)
            {
                obj2 = CreateInstance(type);
            }
            else
            {
                ThrowError("Type " + typeName + " is undefined!");
                return null;
            }
            ht[(int) hv++] = obj2;
            if (obj2 is Serializable)
            {
                byte[] buffer2 = new byte[num2];
                stream.Read(buffer2, 0, num2);
                ((Serializable) obj2).UnSerialize(buffer2);
            }
            else
            {
                stream.Seek((long) num2, SeekOrigin.Current);
            }
            stream.Seek(1L, SeekOrigin.Current);
            return obj2;
        }

        private static object ReadDouble(MemoryStream stream)
        {
            object obj2;
            stream.Seek(1L, SeekOrigin.Current);
            string s = ReadNumber(stream);
            switch (s)
            {
                case "NAN":
                    return (double) 1.0/(double) 0.0;

                case "INF":
                    return (double) 1.0/(double) 0.0;

                case "-INF":
                    return (double) -1.0/(double) 0.0;
            }
            bool flag = s.StartsWith("-");
            try
            {
                obj2 = uint.Parse(s);
            }
            catch
            {
                try
                {
                    obj2 = flag ? ((object) long.Parse(s)) : ((object) ulong.Parse(s));
                }
                catch
                {
                    try
                    {
                        obj2 = decimal.Parse(s);
                    }
                    catch
                    {
                        try
                        {
                            obj2 = float.Parse(s);
                        }
                        catch
                        {
                            obj2 = double.Parse(s);
                        }
                    }
                }
            }
            return obj2;
        }

        private static object ReadInteger(MemoryStream stream)
        {
            object obj2;
            stream.Seek(1L, SeekOrigin.Current);
            string s = ReadNumber(stream);
            bool flag = s.StartsWith("-");
            try
            {
                obj2 = flag ? ((object) sbyte.Parse(s)) : ((object) byte.Parse(s));
            }
            catch
            {
                try
                {
                    obj2 = flag ? ((object) short.Parse(s)) : ((object) ushort.Parse(s));
                }
                catch
                {
                    obj2 = int.Parse(s);
                }
            }
            return obj2;
        }

        private static object ReadNull(MemoryStream stream)
        {
            stream.Seek(1L, SeekOrigin.Current);
            return null;
        }

        private static string ReadNumber(MemoryStream stream)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = stream.ReadByte(); (i != 0x3b) && (i != 0x3a); i = stream.ReadByte())
            {
                builder.Append((char) i);
            }
            return builder.ToString();
        }

        private static object ReadObject(MemoryStream stream, Hashtable ht, ref int hv, Hashtable rt, Encoding encoding)
        {
            object obj2;
            stream.Seek(1L, SeekOrigin.Current);
            int count = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            string typeName = encoding.GetString(buffer);
            stream.Seek(2L, SeekOrigin.Current);
            int capacity = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            Type type = GetType(typeName);
            if (type != null)
            {
                try
                {
                    obj2 = CreateInstance(type);
                }
                catch
                {
                    obj2 = new Hashtable(capacity);
                }
            }
            else
            {
                obj2 = new Hashtable(capacity);
            }
            ht[(int) hv++] = obj2;
            for (int i = 0; i < capacity; i++)
            {
                string str2;
                int num5 = stream.ReadByte();
                if (num5 != 0x55)
                {
                    if (num5 != 0x73)
                    {
                        goto Label_00CE;
                    }
                    str2 = ReadString(stream, encoding);
                }
                else
                {
                    str2 = ReadUnicodeString(stream);
                }
                goto Label_00DA;
                Label_00CE:
                ThrowError("Wrong Serialize Stream!");
                return null;
                Label_00DA:
                if (str2.Substring(0, 1) == "\0")
                {
                    str2 = str2.Substring(str2.IndexOf("\0", 1) + 1);
                }
                if (obj2 is Hashtable)
                {
                    ((Hashtable) obj2)[str2] = UnSerialize(stream, ht, ref hv, rt, encoding);
                }
                else
                {
                    type.InvokeMember(str2,
                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.IgnoreCase, null, obj2,
                        new object[] {UnSerialize(stream, ht, ref hv, rt, encoding)});
                }
            }
            stream.Seek(1L, SeekOrigin.Current);
            MethodInfo info = null;
            try
            {
                info = obj2.GetType()
                    .GetMethod("__wakeup",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,
                        null, new Type[0], new ParameterModifier[0]);
            }
            catch
            {
            }
            if (info != null)
            {
                info.Invoke(obj2, null);
            }
            return obj2;
        }

        private static object ReadRef(MemoryStream stream, Hashtable ht, Hashtable rt)
        {
            stream.Seek(1L, SeekOrigin.Current);
            int key = int.Parse(ReadNumber(stream));
            if (rt.ContainsKey(key))
            {
                rt[key] = true;
            }
            return ht[key];
        }

        private static string ReadString(MemoryStream stream, Encoding encoding)
        {
            stream.Seek(1L, SeekOrigin.Current);
            int count = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            string str = encoding.GetString(buffer);
            stream.Seek(2L, SeekOrigin.Current);
            return str;
        }

        private static string ReadUnicodeString(MemoryStream stream)
        {
            stream.Seek(1L, SeekOrigin.Current);
            int capacity = int.Parse(ReadNumber(stream));
            stream.Seek(1L, SeekOrigin.Current);
            StringBuilder builder = new StringBuilder(capacity);
            for (int i = 0; i < capacity; i++)
            {
                int num2 = stream.ReadByte();
                if (num2 == 0x5c)
                {
                    char ch = (char) stream.ReadByte();
                    char ch2 = (char) stream.ReadByte();
                    char ch3 = (char) stream.ReadByte();
                    char ch4 = (char) stream.ReadByte();
                    builder.Append(
                        (char) int.Parse(string.Concat(new object[] {ch, ch2, ch3, ch4}), NumberStyles.HexNumber));
                }
                else
                {
                    builder.Append((char) num2);
                }
            }
            stream.Seek(2L, SeekOrigin.Current);
            return builder.ToString();
        }

        public static byte[] Serialize(object obj)
        {
            return Serialize(obj, Encoding.UTF8);
        }

        public static byte[] Serialize(object obj, Encoding encoding)
        {
            Hashtable ht = new Hashtable();
            int hv = 1;
            MemoryStream stream = new MemoryStream();
            stream.Seek(0L, SeekOrigin.Begin);
            Serialize(stream, obj, ht, ref hv, encoding);
            byte[] buffer = stream.ToArray();
            stream.Close();
            return buffer;
        }

        private static void Serialize(MemoryStream stream, object obj, Hashtable ht, ref int hv, Encoding encoding)
        {
            if (obj == null)
            {
                hv++;
                WriteNull(stream);
            }
            else if (obj is bool)
            {
                hv++;
                WriteBoolean(stream, ((bool) obj) ? ((byte) 0x31) : ((byte) 0x30));
            }
            else if (((obj is byte) || (obj is sbyte)) || (((obj is short) || (obj is ushort)) || (obj is int)))
            {
                hv++;
                WriteInteger(stream, Encoding.ASCII.GetBytes(obj.ToString()));
            }
            else if (((obj is uint) || (obj is long)) || ((obj is ulong) || (obj is decimal)))
            {
                hv++;
                WriteDouble(stream, Encoding.ASCII.GetBytes(obj.ToString()));
            }
            else if (obj is float)
            {
                hv++;
                if (float.IsNaN((float) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("NAN"));
                }
                else if (float.IsPositiveInfinity((float) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("INF"));
                }
                else if (float.IsNegativeInfinity((float) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("-INF"));
                }
                else
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes(obj.ToString()));
                }
            }
            else if (obj is double)
            {
                hv++;
                if (double.IsNaN((double) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("NAN"));
                }
                else if (double.IsPositiveInfinity((double) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("INF"));
                }
                else if (double.IsNegativeInfinity((double) obj))
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes("-INF"));
                }
                else
                {
                    WriteDouble(stream, Encoding.ASCII.GetBytes(obj.ToString()));
                }
            }
            else if ((obj is char) || (obj is string))
            {
                hv++;
                WriteString(stream, encoding.GetBytes(obj.ToString()));
            }
            else if (obj is Array)
            {
                if (ht.ContainsKey(obj.GetHashCode()))
                {
                    WritePointRef(stream, Encoding.ASCII.GetBytes(ht[obj.GetHashCode()].ToString()));
                }
                else
                {
                    ht.Add(obj.GetHashCode(), (int) hv++);
                    WriteArray(stream, obj as Array, ht, ref hv, encoding);
                }
            }
            else if (obj is ArrayList)
            {
                if (ht.ContainsKey(obj.GetHashCode()))
                {
                    WritePointRef(stream, Encoding.ASCII.GetBytes(ht[obj.GetHashCode()].ToString()));
                }
                else
                {
                    ht.Add(obj.GetHashCode(), (int) hv++);
                    WriteArrayList(stream, obj as ArrayList, ht, ref hv, encoding);
                }
            }
            else if (obj is Hashtable)
            {
                if (ht.ContainsKey(obj.GetHashCode()))
                {
                    WritePointRef(stream, Encoding.ASCII.GetBytes(ht[obj.GetHashCode()].ToString()));
                }
                else
                {
                    ht.Add(obj.GetHashCode(), (int) hv++);
                    WriteHashtable(stream, obj as Hashtable, ht, ref hv, encoding);
                }
            }
            else if (ht.ContainsKey(obj.GetHashCode()))
            {
                hv++;
                WriteRef(stream, Encoding.ASCII.GetBytes(ht[obj.GetHashCode()].ToString()));
            }
            else
            {
                ht.Add(obj.GetHashCode(), (int) hv++);
                WriteObject(stream, obj, ht, ref hv, encoding);
            }
        }

        private static void ThrowError(string message)
        {
            throw new UnSerializeException(message);
        }

        public static object UnSerialize(byte[] ss)
        {
            return UnSerialize(ss, null, Encoding.UTF8);
        }

        public static object UnSerialize(byte[] ss, Encoding encoding)
        {
            return UnSerialize(ss, null, encoding);
        }

        public static object UnSerialize(byte[] ss, Type type)
        {
            return UnSerialize(ss, type, Encoding.UTF8);
        }

        public static object UnSerialize(byte[] ss, Type type, Encoding encoding)
        {
            int hv = 1;
            MemoryStream stream = new MemoryStream(ss);
            stream.Seek(0L, SeekOrigin.Begin);
            object obj2 = UnSerialize(stream, new Hashtable(), ref hv, new Hashtable(), encoding);
            stream.Close();
            if ((type != null) && (obj2 != null))
            {
                obj2 = ChangeType(obj2, type);
            }
            return obj2;
        }

        private static object UnSerialize(MemoryStream stream, Hashtable ht, ref int hv, Hashtable rt, Encoding encoding)
        {
            switch (stream.ReadByte())
            {
                case 0x4e:
                    object obj2;
                    ht[(int) hv++] = obj2 = ReadNull(stream);
                    return obj2;

                case 0x4f:
                    return ReadObject(stream, ht, ref hv, rt, encoding);

                case 0x52:
                    return ReadRef(stream, ht, rt);

                case 0x55:
                    object obj7;
                    ht[(int) hv++] = obj7 = ReadUnicodeString(stream);
                    return obj7;

                case 0x43:
                    return ReadCustomObject(stream, ht, ref hv, encoding);

                case 0x61:
                    return ReadArray(stream, ht, ref hv, rt, encoding);

                case 0x62:
                    object obj3;
                    ht[(int) hv++] = obj3 = ReadBoolean(stream);
                    return obj3;

                case 100:
                    object obj5;
                    ht[(int) hv++] = obj5 = ReadDouble(stream);
                    return obj5;

                case 0x69:
                    object obj4;
                    ht[(int) hv++] = obj4 = ReadInteger(stream);
                    return obj4;

                case 0x72:
                    object obj8;
                    ht[(int) hv++] = obj8 = ReadRef(stream, ht, rt);
                    return obj8;

                case 0x73:
                    object obj6;
                    ht[(int) hv++] = obj6 = ReadString(stream, encoding);
                    return obj6;
            }
            ThrowError("Wrong Serialize Stream!");
            return null;
        }

        private static void WriteArray(MemoryStream stream, Array a, Hashtable ht, ref int hv, Encoding encoding)
        {
            if (a.Rank == 1)
            {
                int length = a.GetLength(0);
                byte[] bytes = Encoding.ASCII.GetBytes(length.ToString());
                int lowerBound = a.GetLowerBound(0);
                int upperBound = a.GetUpperBound(0);
                stream.WriteByte(0x61);
                stream.WriteByte(0x3a);
                stream.Write(bytes, 0, bytes.Length);
                stream.WriteByte(0x3a);
                stream.WriteByte(0x7b);
                for (int i = lowerBound; i <= upperBound; i++)
                {
                    WriteInteger(stream, Encoding.ASCII.GetBytes(i.ToString()));
                    Serialize(stream, a.GetValue(i), ht, ref hv, encoding);
                }
                stream.WriteByte(0x7d);
            }
            else
            {
                int[] indices = new int[1];
                WriteArray(stream, a, indices, ht, ref hv, encoding);
            }
        }

        private static void WriteArray(MemoryStream stream, Array a, int[] indices, Hashtable ht, ref int hv,
            Encoding encoding)
        {
            int length = indices.Length;
            int dimension = length - 1;
            int[] array = new int[length + 1];
            indices.CopyTo(array, 0);
            int num3 = a.GetLength(dimension);
            byte[] bytes = Encoding.ASCII.GetBytes(num3.ToString());
            int lowerBound = a.GetLowerBound(dimension);
            int upperBound = a.GetUpperBound(dimension);
            stream.WriteByte(0x61);
            stream.WriteByte(0x3a);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x3a);
            stream.WriteByte(0x7b);
            for (int i = lowerBound; i <= upperBound; i++)
            {
                WriteInteger(stream, Encoding.ASCII.GetBytes(i.ToString()));
                if (a.Rank == length)
                {
                    indices[length - 1] = i;
                    Serialize(stream, a.GetValue(indices), ht, ref hv, encoding);
                }
                else
                {
                    array[length - 1] = i;
                    WriteArray(stream, a, array, ht, ref hv, encoding);
                }
            }
            stream.WriteByte(0x7d);
        }

        private static void WriteArrayList(MemoryStream stream, ArrayList a, Hashtable ht, ref int hv, Encoding encoding)
        {
            int count = a.Count;
            byte[] bytes = Encoding.ASCII.GetBytes(count.ToString());
            stream.WriteByte(0x61);
            stream.WriteByte(0x3a);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x3a);
            stream.WriteByte(0x7b);
            for (int i = 0; i < count; i++)
            {
                WriteInteger(stream, Encoding.ASCII.GetBytes(i.ToString()));
                Serialize(stream, a[i], ht, ref hv, encoding);
            }
            stream.WriteByte(0x7d);
        }

        private static void WriteBoolean(MemoryStream stream, byte b)
        {
            stream.WriteByte(0x62);
            stream.WriteByte(0x3a);
            stream.WriteByte(b);
            stream.WriteByte(0x3b);
        }

        private static void WriteDouble(MemoryStream stream, byte[] d)
        {
            stream.WriteByte(100);
            stream.WriteByte(0x3a);
            stream.Write(d, 0, d.Length);
            stream.WriteByte(0x3b);
        }

        private static void WriteHashtable(MemoryStream stream, Hashtable h, Hashtable ht, ref int hv, Encoding encoding)
        {
            int count = h.Count;
            byte[] bytes = Encoding.ASCII.GetBytes(count.ToString());
            stream.WriteByte(0x61);
            stream.WriteByte(0x3a);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x3a);
            stream.WriteByte(0x7b);
            foreach (DictionaryEntry entry in h)
            {
                if (((entry.Key is byte) || (entry.Key is sbyte)) ||
                    (((entry.Key is short) || (entry.Key is ushort)) || (entry.Key is int)))
                {
                    WriteInteger(stream, Encoding.ASCII.GetBytes(entry.Key.ToString()));
                }
                else if (entry.Key is bool)
                {
                    WriteInteger(stream, new byte[] {((bool) entry.Key) ? ((byte) 0x31) : ((byte) 0x30)});
                }
                else
                {
                    WriteString(stream, encoding.GetBytes(entry.Key.ToString()));
                }
                Serialize(stream, entry.Value, ht, ref hv, encoding);
            }
            stream.WriteByte(0x7d);
        }

        private static void WriteInteger(MemoryStream stream, byte[] i)
        {
            stream.WriteByte(0x69);
            stream.WriteByte(0x3a);
            stream.Write(i, 0, i.Length);
            stream.WriteByte(0x3b);
        }

        private static void WriteNull(MemoryStream stream)
        {
            stream.WriteByte(0x4e);
            stream.WriteByte(0x3b);
        }

        private static void WriteObject(MemoryStream stream, object obj, Hashtable ht, ref int hv, Encoding encoding)
        {
            Type type = obj.GetType();
            if (type.IsSerializable)
            {
                byte[] bytes = encoding.GetBytes(type.Name);
                int length = bytes.Length;
                byte[] buffer = Encoding.ASCII.GetBytes(length.ToString());
                if (obj is Serializable)
                {
                    byte[] buffer3 = ((Serializable) obj).Serialize();
                    int num9 = buffer3.Length;
                    byte[] buffer4 = Encoding.ASCII.GetBytes(num9.ToString());
                    stream.WriteByte(0x43);
                    stream.WriteByte(0x3a);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.WriteByte(0x3a);
                    stream.WriteByte(0x22);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.WriteByte(0x22);
                    stream.WriteByte(0x3a);
                    stream.Write(buffer4, 0, buffer4.Length);
                    stream.WriteByte(0x3a);
                    stream.WriteByte(0x7b);
                    stream.Write(buffer3, 0, buffer3.Length);
                    stream.WriteByte(0x7d);
                }
                else
                {
                    FieldInfo[] fields;
                    MethodInfo info = null;
                    try
                    {
                        info = type.GetMethod("__sleep",
                            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                            BindingFlags.IgnoreCase, null, new Type[0], new ParameterModifier[0]);
                    }
                    catch
                    {
                    }
                    int num = 0;
                    if (info != null)
                    {
                        string[] strArray = (string[]) info.Invoke(obj, null);
                        fields = new FieldInfo[strArray.Length];
                        int num2 = 0;
                        int num3 = fields.Length;
                        while (num2 < num3)
                        {
                            fields[num2] = type.GetField(strArray[num2],
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                            num2++;
                        }
                    }
                    else
                    {
                        fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    }
                    int index = 0;
                    int num5 = fields.Length;
                    while (index < num5)
                    {
                        if (((fields[index] != null) && !fields[index].IsNotSerialized) &&
                            (!fields[index].IsInitOnly && !fields[index].IsLiteral))
                        {
                            num++;
                        }
                        index++;
                    }
                    byte[] buffer5 = Encoding.ASCII.GetBytes(num.ToString());
                    stream.WriteByte(0x4f);
                    stream.WriteByte(0x3a);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.WriteByte(0x3a);
                    stream.WriteByte(0x22);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.WriteByte(0x22);
                    stream.WriteByte(0x3a);
                    stream.Write(buffer5, 0, buffer5.Length);
                    stream.WriteByte(0x3a);
                    stream.WriteByte(0x7b);
                    int num6 = 0;
                    int num7 = fields.Length;
                    while (num6 < num7)
                    {
                        if (((fields[num6] != null) && !fields[num6].IsNotSerialized) &&
                            (!fields[num6].IsInitOnly && !fields[num6].IsLiteral))
                        {
                            if (fields[num6].IsPublic)
                            {
                                WriteString(stream, encoding.GetBytes(fields[num6].Name));
                            }
                            else if (fields[num6].IsPrivate)
                            {
                                WriteString(stream,
                                    encoding.GetBytes("\0" + fields[num6].DeclaringType.Name + "\0" + fields[num6].Name));
                            }
                            else
                            {
                                WriteString(stream, encoding.GetBytes("\0*\0" + fields[num6].Name));
                            }
                            Serialize(stream, fields[num6].GetValue(obj), ht, ref hv, encoding);
                        }
                        num6++;
                    }
                    stream.WriteByte(0x7d);
                }
            }
            else
            {
                WriteNull(stream);
            }
        }

        private static void WritePointRef(MemoryStream stream, byte[] p)
        {
            stream.WriteByte(0x52);
            stream.WriteByte(0x3a);
            stream.Write(p, 0, p.Length);
            stream.WriteByte(0x3b);
        }

        private static void WriteRef(MemoryStream stream, byte[] r)
        {
            stream.WriteByte(0x72);
            stream.WriteByte(0x3a);
            stream.Write(r, 0, r.Length);
            stream.WriteByte(0x3b);
        }

        private static void WriteString(MemoryStream stream, byte[] s)
        {
            int length = s.Length;
            byte[] bytes = Encoding.ASCII.GetBytes(length.ToString());
            stream.WriteByte(0x73);
            stream.WriteByte(0x3a);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x3a);
            stream.WriteByte(0x22);
            stream.Write(s, 0, s.Length);
            stream.WriteByte(0x22);
            stream.WriteByte(0x3b);
        }
    }



    public interface Serializable
    {
        // Methods
        byte[] Serialize();
        void UnSerialize(byte[] ss);
    }

    public class UnSerializeException : Exception
    {
        // Methods
        public UnSerializeException()
        {
        }

        public UnSerializeException(string message) : base(message)
        {
        }

        public UnSerializeException(string message, Exception inner) : base(message, inner)
        {
        }
    }






}
