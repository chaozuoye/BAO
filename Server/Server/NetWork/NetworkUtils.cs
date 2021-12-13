using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 网络工具类 <see langword="static"/>
/// </summary>
public static class NetworkUtils
{
    private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    private static float fixedUpdateTimeDelta = 1f / 60;
    public static float FixedUpdateTime
    {
        get { return fixedUpdateTimeDelta; }
    }
    /// <summary>
    /// obj -> bytes, 如果obj未被标记为 [Serializable] 则返回null
    /// </summary>
    public static byte[] Serialize(object obj)
    {
        //物体不为空且可被序列化
        if (obj == null || !obj.GetType().IsSerializable)
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            byte[] data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// bytes -> obj, 如果obj未被标记为 [Serializable] 则返回null
    /// </summary>
    public static T Deserialize<T>(byte[] data) where T : class
    {
        //数据不为空且T是可序列化的类型
        if (data == null || !typeof(T).IsSerializable)
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        /* MemoryStream stream = new MemoryStream(data);
         stream.Position = 0;
         BinaryFormatter de = new BinaryFormatter();

         stream.Seek(0, SeekOrigin.Begin);
         object newobj = de.Deserialize(stream);
         stream.Close();
         stream.Dispose();
         return newobj as T;*/
        Console.WriteLine("data.length："+data.Length);
        using (MemoryStream stream = new MemoryStream(data))
        {
            object obj = formatter.Deserialize(stream);
            return obj as T;
        }
    }

    /// <summary>
    /// 获取本机IPv4,获取失败则返回null
    /// </summary>
    public static string GetLocalIPv4()
    {
        string hostName = Dns.GetHostName(); //得到主机名
        IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
        for (int i = 0; i < iPEntry.AddressList.Length; i++)
        {
            //从IP地址列表中筛选出IPv4类型的IP地址
            if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            {
                return iPEntry.AddressList[i].ToString();
            }
        }
        return null;
    }

    /// <summary>
    /// 客户端时间
    /// </summary>
    /// <returns></returns>
    public static long ClientNow()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;
    }

    public static long ClientNowSeconds()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000000;
    }

    public static long Now()
    {
        return ClientNow();
    }

}