using MFilesAPI;
using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IVaultService
  {
    Lazy<Vault> Vault { get; }
  }

  class VaultService : IVaultService
  {
    public Lazy<Vault> Vault { get; private set; }

    public VaultService(ISettingsService settingsService, IParametersService parametersService)
    {
      this.Vault = new Lazy<Vault>(() => CreateVault(settingsService.Settings, parametersService.Parameters));
    }

    private Vault CreateVault(SettingsModel settings, ParametersModel parameters)
    {
      /* use the parameters if specified, else fallback to the settings */
      string user = string.IsNullOrEmpty(parameters.User) ? settings.User : parameters.User;
      string password = string.IsNullOrEmpty(parameters.Password) ? settings.Password : parameters.Password;
      string vault = string.IsNullOrEmpty(parameters.Vault) ? settings.Vault : parameters.Vault;
      string networkAddress = string.IsNullOrEmpty(parameters.Server) ? settings.Server : parameters.Server;
      return CreateVault(user, password, vault, networkAddress);
    }

    private Vault CreateVault(string user, string password, string vault, string networkAddress = null)
    {
      /* login to the server and get the vault */
      var serverApp = new MFilesAPI.MFilesServerApplication();

      serverApp.Connect(
        AuthType: MFilesAPI.MFAuthType.MFAuthTypeSpecificMFilesUser,
        UserName: user,
        Password: password,
        NetworkAddress: networkAddress == string.Empty ? null : networkAddress
      );
      var vaultsOnServer = serverApp.GetVaults();

      Vault v = null;
      foreach (MFilesAPI.VaultOnServer vaultOnServer in vaultsOnServer)
      {
        if (vaultOnServer.Name == vault)
        {
          v = vaultOnServer.LogIn();
          break;
        }
      }

      return v;
    }
  }
}
