using Assets.Scripts.Framework;
using System;
using System.IO;
using UnityEngine;
using YooAsset;

/// <summary>
/// 文件流加密方式
/// </summary>
public class TestFileStreamEncryption : IEncryptionServices
{
    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        EncryptResult result = new EncryptResult();
        result.Encrypted = true;
        using (AesEncryptorStream bundleStream = new AesEncryptorStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read, GameMgr.KEY, GameMgr.IV))
        {
            byte[] buffer = new byte[bundleStream.Length];
            bundleStream.Read(buffer, 0, buffer.Length);
            result.EncryptedData = bundleStream.GetEncryedBytes();
        }
        return result;
    }
}

/// <summary>
/// 资源文件流解密类
/// </summary>
public class TestFileStreamDecryption : IDecryptionServices
{

    /// <summary>
    /// 同步方式获取解密的资源包对象
    /// </summary>
    DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new DecryptResult();
        try
        {
            AesDecryptorStream bundleStream = new AesDecryptorStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read, fileInfo.BundleName, GameMgr.KEY, GameMgr.IV);
            decryptResult.ManagedStream = bundleStream;
            //if(bundleStream.Length != 54500)
            decryptResult.Result = AssetBundle.LoadFromStream(bundleStream, 0u);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return decryptResult;
    }

    /// <summary>
    /// 异步方式获取解密的资源包对象
    /// </summary>
    DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new DecryptResult();
        try
        {
            AesDecryptorStream bundleStream = new AesDecryptorStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read, fileInfo.BundleName, GameMgr.KEY, GameMgr.IV);
            decryptResult.ManagedStream = bundleStream;
            decryptResult.CreateRequest = AssetBundle.LoadFromStreamAsync(bundleStream, 0u);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return decryptResult;
    }

    /// <summary>
    /// 后备方式获取解密的资源包
    /// 注意：当正常解密方法失败后，会触发后备加载！
    /// 说明：建议通过LoadFromMemory()方法加载资源包作为保底机制。
    /// </summary>
    DecryptResult IDecryptionServices.LoadAssetBundleFallback(DecryptFileInfo fileInfo)
    {
        Debug.LogError($"{fileInfo.BundleName}====================LoadAssetBundleFallback====================");
        byte[] fileData = File.ReadAllBytes(fileInfo.FileLoadPath);
        var assetBundle = AssetBundle.LoadFromMemory(fileData);
        DecryptResult decryptResult = new DecryptResult();
        decryptResult.Result = assetBundle;
        return decryptResult;
    }

    /// <summary>
    /// 获取解密的字节数据
    /// </summary>
    byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 获取解密的文本数据
    /// </summary>
    string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    private static uint GetManagedReadBufferSize()
    {
        return 1024;
    }
}

/// <summary>
/// WebGL平台解密类
/// 注意：WebGL平台支持内存解密
/// </summary>
public class TestWebFileMemoryDecryption : IWebDecryptionServices
{
    public WebDecryptResult LoadAssetBundle(WebDecryptFileInfo fileInfo)
    {
        /*
        byte[] copyData = new byte[fileInfo.FileData.Length];
        Buffer.BlockCopy(fileInfo.FileData, 0, copyData, 0, fileInfo.FileData.Length);

        for (int i = 0; i < copyData.Length; i++)
        {
            copyData[i] ^= BundleStream.KEY;
        }

        WebDecryptResult decryptResult = new WebDecryptResult();
        decryptResult.Result = AssetBundle.LoadFromMemory(copyData);
        return decryptResult;
        */

        //for (int i = 0; i < fileInfo.FileData.Length; i++)
        //{
        //    fileInfo.FileData[i] ^= BundleStream.KEY;
        //}

        WebDecryptResult decryptResult = new WebDecryptResult();
        decryptResult.Result = AssetBundle.LoadFromMemory(fileInfo.FileData);
        return decryptResult;
    }
}