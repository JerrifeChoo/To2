using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void DownloadPackage(ref YooComponent component)
        {
            if (component.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(component.PackageID);
                if (packageSetting == null)
                {
                    component.Status = EOperationStatus.Failed;
                    return;
                }
                var package = YooAssets.GetPackage(packageSetting.Name);
                if (package == null)
                {
                    component.Status = EOperationStatus.Failed;
                    return;
                }
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
                if (downloader.TotalDownloadCount == 0)
                {
                    component.Status = EOperationStatus.Succeed;
                }
                else
                {
                    downloader.BeginDownload();
                    //downloader.DownloadErrorCallback = ;
                    //downloader.DownloadUpdateCallback = ;
                    packageSetting.operation = downloader;
                    component.Status = downloader.Status;
                }
            }
            else if (component.Status == EOperationStatus.Succeed)
            {
                var packageSetting = GetPackageSetting(component.PackageID);
                if (packageSetting.operation == null)
                    component.Status = EOperationStatus.Failed;
                else
                    component.Status = packageSetting.operation.Status;
            }
            else if (component.Status == EOperationStatus.Failed)
            {
            }
            else if (component.Status == EOperationStatus.Succeed)
            {
            }
        }
    }
}