using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;


[TestFixture]
public class APIClientIntegrationTests
{
    private APIClient _apiClient;

    [SetUp]
    public void SetUp()
    {
        _apiClient = APIClient.Instance;
    }

    [Test]
    public async Task CreatePlayerRequest_ShouldCreatePlayer()
    {
        // Arrange
        string playerName = "testPlayer";
        var resources = new Dictionary<string, byte> { { "gold", 100 }, { "wood", 50 } };

        // Act
        var result = await _apiClient.CreatePlayerRequest(playerName, resources);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(playerName, result.Name);
        Assert.AreEqual(100, result.Resources["gold"]);
        Assert.AreEqual(50, result.Resources["wood"]);
    }

    [Test]
    public async Task GetUsersListRequest_ShouldReturnPlayers()
    {
        // Act
        var result = await _apiClient.GetUsersListRequest();

        // Assert
        Assert.NotNull(result);
        Assert.IsInstanceOf<List<APIClient.UserInventory>>(result);
        Assert.IsTrue(result.Count > 0); // Ensure at least one user exists
    }

    [Test]
    public async Task GetUserInventoryRequest_ShouldReturnPlayerInventory()
    {
        // Arrange
        string playerName = "testPlayer";

        // Act
        var result = await _apiClient.GetUserInventoryRequest(playerName);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(playerName, result.Name);
    }

    [Test]
    public async Task SetUserInventoryRequest_ShouldUpdateInventory()
    {
        // Arrange
        string playerName = "testPlayer";
        var updatedResources = new Dictionary<string, byte> { { "gold", 200 }, { "wood", 100 } };

        // Act
        var result = await _apiClient.SetUserInventoryRequest(playerName, updatedResources);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.ResourcesChange["gold"]);
        Assert.AreEqual(100, result.ResourcesChange["wood"]);
    }

    [Test]
    public async Task DeletePlayerRequest_ShouldDeletePlayer()
    {
        // Arrange
        string playerName = "playerToDelete";
        await _apiClient.CreatePlayerRequest(playerName, null); // Ensure player exists

        // Act
        await _apiClient.DeletePlayerRequest(playerName);

        // Assert
        var result = await _apiClient.GetUserInventoryRequest(playerName);
        Assert.IsNull(result); // Should not exist anymore
    }

    [Test]
    public async Task CreateShopRequest_ShouldCreateShop()
    {
        // Arrange
        string playerName = "testPlayer";
        string shopName = "testShop";
        var shopResources = new Dictionary<string, byte> { { "items", 10 }, { "gold", 5 } };

        // Act
        var result = await _apiClient.CreateShopRequest(playerName, shopName, shopResources);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(shopName, result.Name);
        Assert.AreEqual(10, result.Resources["items"]);
        Assert.AreEqual(5, result.Resources["gold"]);
    }

    [Test]
    public async Task GetShopsRequest_ShouldReturnShops()
    {
        // Arrange
        string playerName = "testPlayer";

        // Act
        var result = await _apiClient.GetShopsRequest(playerName);

        // Assert
        Assert.NotNull(result);
        Assert.IsInstanceOf<List<APIClient.ShopInfo>>(result);
        Assert.IsTrue(result.Count > 0); // Ensure shops exist
    }

    [Test]
    public async Task GetShopResourcesRequest_ShouldReturnShopResources()
    {
        // Arrange
        string playerName = "testPlayer";
        string shopName = "testShop";

        // Act
        var result = await _apiClient.GetShopResoursesRequest(playerName, shopName);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(shopName, result.Name);
    }
    
    

    [Test]
    public async Task DeleteShop_ShouldDeleteShop()
    {
        // Arrange
        string playerName = "testPlayer";
        string shopName = "shopToDelete";
        await _apiClient.CreateShopRequest(playerName, shopName, null); // Ensure shop exists

        // Act
        await _apiClient.DeleteShop(playerName, shopName);

        // Assert
        var result = await _apiClient.GetShopResoursesRequest(playerName, shopName);
        Assert.IsNull(result); // Shop should not exist anymore
    }

    [Test]
    public async Task CreateLogRequest_ShouldCreateLog()
    {
        // Arrange
        string comment = "Resource log test";
        string playerName = "testPlayer";
        var resourceChanges = new Dictionary<string, int> { { "gold", -10 } };

        // Act
        var result = await _apiClient.CreateLogRequest(comment, playerName, resourceChanges);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(comment, result.Comment);
        Assert.AreEqual(playerName, result.PlayerName);
        Assert.AreEqual(-10, result.ResourcesChanged["gold"]);
    }

    [Test]
    public async Task ReadLogsRequest_ShouldReturnLogs()
    {
        // Arrange
        string playerName = "testPlayer";

        // Act
        var result = await _apiClient.ReadLogsRequest(playerName);

        // Assert
        Assert.NotNull(result);
        Assert.IsInstanceOf<List<APIClient.SendResourcesLog>>(result);
        Assert.IsTrue(result.Count > 0); // Ensure logs exist
    }

    [Test]
    public async Task GetGameLogs_ShouldReturnAllLogs()
    {
        // Act
        var result = await _apiClient.GetGameLogs();

        // Assert
        Assert.NotNull(result);
        Assert.IsInstanceOf<List<APIClient.ShopLogs>>(result);
        Assert.IsTrue(result.Count > 0); // Ensure logs exist
    }
}

