using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AzureManager
{

    private static AzureManager instance;
    private MobileServiceClient client;
    private IMobileServiceTable<DuckModel> duckTable;

    private AzureManager()
    {
        this.client = new MobileServiceClient("http://rubberorreal.azurewebsites.net");
        this.duckTable = this.client.GetTable<DuckModel>();
    }

    public MobileServiceClient AzureClient
    {
        get { return client; }
    }

    public static AzureManager AzureManagerInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new AzureManager();
            }

            return instance;
        }
    }

    public async Task<List<DuckModel>> GetDuckInformation()
    {
        return await this.duckTable.ToListAsync();
    }
    public async Task PostDuckInformation(DuckModel DuckModel)
    {
        await this.duckTable.InsertAsync(DuckModel);
    }

}