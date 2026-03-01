
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void RequestVersion(ref YooComponent component)
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
                var operation = package.RequestPackageVersionAsync();
                packageSetting.operation = operation;
                component.Status = operation.Status;
            }
            else if (component.Status == EOperationStatus.Processing)
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
                component.PackageStatus = YooStatus.UpdateManifest;
                component.Status = EOperationStatus.None;
            }
        }
    }
}