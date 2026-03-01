
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem
    {
        private partial void RequestVersion(YooComponent componet)
        {
            if (componet.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(componet.PackageID);
                if (packageSetting == null)
                    return;
                var package = YooAssets.GetPackage(packageSetting.Name);
                if (package == null)
                    return;
                var operation = package.RequestPackageVersionAsync();
                if (operation.Status != EOperationStatus.Succeed)
                {
                    componet.Status = operation.Status;
                }
                else
                {
                    packageSetting.Version = operation.PackageVersion;
                    componet.PackageStatus = YooStatus.UpdateManifest;
                    componet.Status = EOperationStatus.None;
                }
            }
        }
    }
}