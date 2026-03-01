using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem
    {
        private partial void DownloadPackage(YooComponent componet)
        {
            if (componet.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(componet.PackageID);
                if (packageSetting == null)
                    return;
                var package = YooAssets.GetPackage(packageSetting.Name);
                if (package == null)
                    return;
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
                if (downloader.TotalDownloadCount == 0)
                {
                }
                else
                {
                    //downloader.DownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
                    //downloader.DownloadUpdateCallback = PatchEventDefine.DownloadUpdate.SendEventMessage;
                    downloader.BeginDownload();
                }
            }
        }
    }
}