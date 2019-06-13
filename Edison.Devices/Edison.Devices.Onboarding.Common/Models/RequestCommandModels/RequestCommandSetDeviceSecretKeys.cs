namespace Edison.Devices.Onboarding.Common.Models
{
    public class RequestCommandSetDeviceSecretKeys : RequestCommand
    {
        public string AccessPointPassword { get; set; }
        public string PortalPassword { get; set; }
        public string EncryptionKey { get; set; }
    }
}
