
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem
	{
        private partial void ClearCacheBundle(YooComponent componet)
        {
            var packageSetting = GetPackageSetting(componet.PackageID);
            if (packageSetting == null)
                return;
            var package = YooAssets.GetPackage(packageSetting.Name);
            if (package == null)
                return;
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            //operation.Completed += Operation_Completed;
        }
    }
}