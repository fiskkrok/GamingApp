using GamingApp.ApiService.Data.Models;

namespace GamingApp.ApiService.Data.SeedData;

public static class GameSeedData
{
    public static List<Game> GetSeedData(List<Category> categories)
    {
        return
        [
            new Game
            {
                Name = "Space Adventure",
                Description = "A thrilling journey through the galaxy.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(a => a.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Mystery Mansion",
                Description = "Solve puzzles and escape the haunted mansion.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(a => a.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Zombie Apocalypse",
                Description = "Survive the undead in a post-apocalyptic world.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Fantasy Quest",
                Description = "Embark on an epic quest in a magical realm.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Role-Playing")).Id
            },

            new Game
            {
                Name = "Racing Rivals",
                Description = "Compete against the best racers in high-speed challenges.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Alien Invasion",
                Description = "Defend Earth from an alien attack.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Dungeon Crawler",
                Description = "Explore dangerous dungeons filled with monsters.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Role-Playing")).Id
            },

            new Game
            {
                Name = "Stealth Assassin",
                Description = "Use stealth to eliminate your enemies in this tactical game.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Galactic Trader",
                Description = "Build your trading empire across the stars.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Simulation")).Id
            },

            new Game
            {
                Name = "Pirate Adventure",
                Description = "Sail the seas in search of treasure and adventure.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },

            new Game
            {
                Name = "Cyberpunk City",
                Description = "Navigate a futuristic city full of crime and mystery.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Medieval Siege",
                Description = "Build your army and lay siege to enemy castles.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Strategy")).Id
            },

            new Game
            {
                Name = "Monster Hunter",
                Description = "Track and hunt down mythical creatures.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Super Ninja",
                Description = "Master martial arts and defeat your enemies.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "City Builder",
                Description = "Design and manage your own city.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Simulation")).Id
            },

            new Game
            {
                Name = "Wild West Showdown",
                Description = "Experience the wild west in this fast-paced action game.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Underwater Exploration",
                Description = "Explore the deep sea and discover hidden treasures.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },
            new Game
            {
                Name = "Time Traveler",
                Description = "Travel through time to solve mysteries and change history.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },
            new Game
            {
                Name = "Survival Island",
                Description = "Survive on a deserted island by gathering resources and building shelter.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Survival")).Id
            },
            new Game
            {
                Name = "Air Combat",
                Description = "Engage in thrilling aerial dogfights.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            }
        ];
    }
}
