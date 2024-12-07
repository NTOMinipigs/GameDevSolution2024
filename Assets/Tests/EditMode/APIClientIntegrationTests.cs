// using System.Collections.Generic;
// using NUnit.Framework;
//
// [TestFixture]
// public class APIClientIntegrationTests
// {
//     private APIClient _apiClient;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _apiClient = APIClient.Instance;
//     }
//
//     [Test]
//     public void CreatePlayerRequest_ShouldCreatePlayer()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//         var resources = new Dictionary<string, byte> { { "gold", 100 }, { "wood", 50 } };
//
//         // Act
//         var result = _apiClient.CreatePlayerRequest(playerName, resources).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(playerName, result.Name);
//         Assert.AreEqual(100, result.Resources["gold"]);
//         Assert.AreEqual(50, result.Resources["wood"]);
//     }
//
//     [Test]
//     public void GetUsersListRequest_ShouldReturnPlayers()
//     {
//         // Act
//         var result = _apiClient.GetUsersListRequest().GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.IsInstanceOf<List<APIClient.UserInventory>>(result);
//         Assert.IsTrue(result.Count > 0); // Ensure at least one user exists
//     }
//
//     [Test]
//     public void GetUserInventoryRequest_ShouldReturnPlayerInventory()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//
//         // Act
//         var result = _apiClient.GetUserInventoryRequest(playerName).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(playerName, result.Name);
//     }
//
//     [Test]
//     public void SetUserInventoryRequest_ShouldUpdateInventory()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//         var updatedResources = new Dictionary<string, int> { { "gold", 200 }, { "wood", 100 } };
//
//         // Act
//         var result = _apiClient.SetUserInventoryRequest(playerName, updatedResources).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(200, result.ResourcesChange["gold"]);
//         Assert.AreEqual(100, result.ResourcesChange["wood"]);
//     }
//
//     [Test]
//     public void DeletePlayerRequest_ShouldDeletePlayer()
//     {
//         // Arrange
//         string playerName = "playerToDelete";
//         _apiClient.CreatePlayerRequest(playerName, null).GetAwaiter().GetResult(); // Ensure player exists
//
//         // Act
//         _apiClient.DeletePlayerRequest(playerName).GetAwaiter().GetResult();
//
//         // Assert
//         var result = _apiClient.GetUserInventoryRequest(playerName).GetAwaiter().GetResult();
//         Assert.IsNull(result); // Should not exist anymore
//     }
//
//     [Test]
//     public void CreateShopRequest_ShouldCreateShop()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//         string shopName = "testShop";
//         var shopResources = new Dictionary<string, byte> { { "items", 10 }, { "gold", 5 } };
//
//         // Act
//         var result = _apiClient.CreateShopRequest(playerName, shopName, shopResources).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(shopName, result.Name);
//         Assert.AreEqual(10, result.Resources["items"]);
//         Assert.AreEqual(5, result.Resources["gold"]);
//     }
//
//     [Test]
//     public void GetShopsRequest_ShouldReturnShops()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//
//         // Act
//         var result = _apiClient.GetShopsRequest(playerName).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.IsInstanceOf<List<APIClient.ShopInfo>>(result);
//         Assert.IsTrue(result.Count > 0); // Ensure shops exist
//     }
//
//     [Test]
//     public void GetShopResourcesRequest_ShouldReturnShopResources()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//         string shopName = "testShop";
//
//         // Act
//         var result = _apiClient.GetShopResoursesRequest(playerName, shopName).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(shopName, result.Name);
//     }
//
//     [Test]
//     public void DeleteShop_ShouldDeleteShop()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//         string shopName = "shopToDelete";
//         _apiClient.CreateShopRequest(playerName, shopName, null).GetAwaiter().GetResult(); // Ensure shop exists
//
//         // Act
//         _apiClient.DeleteShop(playerName, shopName).GetAwaiter().GetResult();
//
//         // Assert
//         var result = _apiClient.GetShopResoursesRequest(playerName, shopName).GetAwaiter().GetResult();
//         Assert.IsNull(result); // Shop should not exist anymore
//     }
//
//     [Test]
//     public void CreateLogRequest_ShouldCreateLog()
//     {
//         // Arrange
//         string comment = "Resource log test";
//         string playerName = "testPlayer";
//         var resourceChanges = new Dictionary<string, int> { { "gold", 10 } };
//
//         // Act
//         var result = _apiClient.CreateLogRequest(comment, playerName, resourceChanges).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.AreEqual(comment, result.Comment);
//         Assert.AreEqual(playerName, result.PlayerName);
//         Assert.AreEqual(-10, result.ResourcesChanged["gold"]);
//     }
//
//     [Test]
//     public void ReadLogsRequest_ShouldReturnLogs()
//     {
//         // Arrange
//         string playerName = "testPlayer";
//
//         // Act
//         var result = _apiClient.ReadLogsRequest(playerName).GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.IsInstanceOf<List<APIClient.SendResourcesLog>>(result);
//         Assert.IsTrue(result.Count > 0); // Ensure logs exist
//     }
//
//     [Test]
//     public void GetGameLogs_ShouldReturnAllLogs()
//     {
//         // Act
//         var result = _apiClient.GetGameLogs().GetAwaiter().GetResult();
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.IsInstanceOf<List<APIClient.ShopLogs>>(result);
//         Assert.IsTrue(result.Count > 0); // Ensure logs exist
//     }
// }
