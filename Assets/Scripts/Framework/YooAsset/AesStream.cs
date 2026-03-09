using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 加密流
/// 只对头部1024位进行加密
/// </summary>
public class AesEncryptorStream : FileStream
{
    private readonly Aes aes;
    private readonly ICryptoTransform cryptoTransform;
    private bool isCrypted = true;
    private readonly byte[] encryedData;
    private int encryedLength;
    private readonly string filePath;

    public AesEncryptorStream(string path, FileMode mode, FileAccess access, FileShare share, byte[] key, byte[] iv) : base(path, mode, access, share)
    {
        filePath = path;
        aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        cryptoTransform = aes.CreateEncryptor(key, iv);
        encryedData = new byte[Length];
    }

    public AesEncryptorStream(string path, FileMode mode, byte[] key, byte[] iv) : base(path, mode)
    {
        filePath = path;
        aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        cryptoTransform = aes.CreateEncryptor(key, iv);
        encryedData = new byte[Length];
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            aes.Dispose();
            cryptoTransform.Dispose();
        }
        base.Dispose(disposing);
    }

    public override int Read(byte[] array, int offset, int count)
    {
        var position = Position;
        var index = base.Read(array, offset, count);
        if (!isCrypted && position == 0)
        {
            int blockCount = 1024 / 16;
            if (blockCount > 0)
            {
                encryedLength = blockCount * 16;
                var encryed = new byte[encryedLength];
                for (int i = 0; i < blockCount; i++)
                {
                    int bytesWritten = cryptoTransform.TransformBlock(array, i * 16, 16, encryed, i * 16);

                }
                //int bytesWritten = cryptoTransform.TransformBlock(array, 0, encryedLength, encryed, 0);
                cryptoTransform.TransformFinalBlock(new byte[0], 0, 0);
                Array.Copy(encryed, 0, encryedData, 0, encryedLength);
            }
        }
        return index;
    }

    public byte[] GetEncryedBytes()
    {
        var fileData = File.ReadAllBytes(filePath);
        for (int i = encryedLength; i < fileData.Length; i++)
            encryedData[i] = fileData[i];
        return encryedData;
    }
}

/// <summary>
/// 解密流
// TODO: fixedme 调用LoadFromaStream会加载头部一段二进制，根据这个做的头部加密。解密还原assetBundle二进制头部1024位加密字节，部分assetBundle必然会导致闪退目前暂未定位到具体原因，怀疑是不是某些类型有其他验证导致的。
// TODO: 猜想：assetBundle的随机读取，大概可能是根据数据块的头尾位置定位过去的，若正确可参考AssetStudio头文件读取，解读出各数据块头尾位置并记录，当读取到对应位置分内容块进行解密优化全文件加解密性能
/// </summary>
public class AesDecryptorStream : FileStream
{
    private Aes aes;
    private ICryptoTransform cryptoTransform;
    private bool isCreypted = true;
    private bool isFirst;
    private string bundleName;

    public AesDecryptorStream(string path, FileMode mode, FileAccess access, FileShare share, string BundleName, byte[] key, byte[] iv) : base(path, mode, access, share)
    {
        bundleName = BundleName;
        aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        cryptoTransform = aes.CreateDecryptor(key, iv);
    }
    public AesDecryptorStream(string path, FileMode mode, byte[] key, byte[] iv) : base(path, mode)
    {
        aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        cryptoTransform = aes.CreateDecryptor(key, iv);
    }

    protected override void Dispose(bool disposing)
    {
        Debug.LogError($"Dispose==========================={bundleName} {disposing}");
        //if (disposing)
        //{
            aes.Dispose();
            cryptoTransform.Dispose();
        //}
        base.Dispose(disposing);
    }

    public override int Read(byte[] array, int offset, int count)
    {
        var position = Position;
        if (!isFirst)
        {
            isFirst = true;
            Debug.LogError($"{bundleName}, {position}");
        }
        var index = base.Read(array, offset, count);
        if (!isCreypted || position == 0)
            Debug.LogError($"{bundleName}, {position}, {Length}, {isCreypted}");
        //if (Length == 54500) { }
        //    isCreypted = true;
        if (!isCreypted && position == 0)
        {
            int blockCount = (int)1024 / 16;
            //GameManager.WriteLogToFile($"Read {blockCount}");
            if (blockCount > 0)
            {
                var decryedLength = blockCount * 16;
                var decryed = new byte[decryedLength];
                //for (int i = 0; i < blockCount; i++)
                //{
                //    cryptoTransform.TransformBlock(array, i * 16, 16, decryed, i * 16);
                //    int bytesWritten = cryptoTransform.TransformBlock(array, i * 16, 16, decryed, i * 16);
                //}
                int bytesWritten = cryptoTransform.TransformBlock(array, 0, decryedLength, decryed, 0);
                //byte[] final = cryptoTransform.TransformFinalBlock(decryed, 0, decryedLength);
                //Debug.LogError($"{final.Length}");
                //if (bytesWritten > 0)
                Array.Copy(decryed, 0, array, 0, decryedLength);
            }
            isCreypted = true;
        }
        return index;
    }
}
