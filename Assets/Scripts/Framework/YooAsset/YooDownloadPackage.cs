using Unity.Entities;
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void DownloadPackage(RefRW<YooComponent> yoo)
        {
            if (yoo.ValueRW.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(yoo.ValueRW.PackageID);
                if (packageSetting == null)
                {
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                    return;
                }
                var package = YooAssets.GetPackage(packageSetting.Name);
                if (package == null)
                {
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                    return;
                }
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
                if (downloader.TotalDownloadCount == 0)
                {
                    yoo.ValueRW.Status = EOperationStatus.Succeed;
                }
                else
                {
                    downloader.BeginDownload();
                    //downloader.DownloadErrorCallback = ;
                    //downloader.DownloadUpdateCallback = ;
                    packageSetting.operation = downloader;
                    yoo.ValueRW.Status = downloader.Status;
                }
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Succeed)
            {
                var packageSetting = GetPackageSetting(yoo.ValueRW.PackageID);
                if (packageSetting.operation == null)
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                else
                    yoo.ValueRW.Status = packageSetting.operation.Status;
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Failed)
            {
                yoo.ValueRW.PackageStatus = YooStatus.Error;
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Succeed)
            {
            }
        }
    }
}