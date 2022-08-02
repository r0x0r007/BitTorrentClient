using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public static class BEncoding
{
    private static byte DictionaryStart = System.Text.Encoding.UTF8.GetBytes("d")[0];
    private static byte DictionaryEnd = System.Text.Encoding.UTF8.GetBytes("e")[0];
    private static byte ListStart = System.Text.Encoding.UTF8.GetBytes("l")[0];
    private static byte ListEnd = System.Text.Encoding.UTF8.GetBytes("e")[0];
    private static byte NumberStart = System.Text.Encoding.UTF8.GetBytes("i")[0];
    private static byte NumberEnd = System.Text.Encoding.UTF8.GetBytes("e")[0];
    private static byte ByteArrayDivider = System.Text.Encoding.UTF8.GetBytes(":")[0];
    
    public static object Decode(byte[] bytes)
    {
        IEnumerator<byte> enumerator = ((IEnumerable<byte>)bytes).GetEnumerator();
        enumerator.MoveNext();
        return DecodeNextObject(enumerator);
    }

    private static object DecodeNextObject(IEnumerator<byte> enumerator)
    {
        if (enumerator.Current == DictionaryStart) return DecodeDictionary(enumerator);
        if (enumerator.Current == ListStart) return DecodeList(enumerator);
        if (enumerator.Current == NumberStart) return DecodeNumber(enumerator);
        return DecodeByteArray(enumerator);
    }

    private static object DecodeNumber(IEnumerator<byte> enumerator)
    {
        List<byte> bytes = new List<byte>(9);
        //keep pulling bytes until we hit the end tag
        while (enumerator.MoveNext())
        {
            if (enumerator.Current == NumberEnd) break;
            bytes.Add(enumerator.Current);
        }
        string numAsString = Encoding.UTF8.GetString(bytes.ToArray());
        return Int64.Parse(numAsString);
    }

    public static object DecodeFile(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Unable to find file :" + path);
        byte[] bytes = File.ReadAllBytes(path);
        return BEncoding.Decode(bytes);
    }

}
