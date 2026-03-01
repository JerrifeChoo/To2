using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem
    {
        private partial void UpdateManifest(YooComponent componet) 
        {
            var packageSetting = GetPackageSetting(componet.PackageID);
            if (packageSetting == null)
                return;
            var package = YooAssets.GetPackage(packageSetting.Name);
            if (package == null)
                return;
            var operation = package.UpdatePackageManifestAsync(packageSetting.Version);
            //if (operation.Status != EOperationStatus.Succeed)
            //{
            //    Debug.LogWarning(operation.Error);
            //}
            //else
            //{
            //}
        }
    }
}