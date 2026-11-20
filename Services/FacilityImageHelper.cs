using SportsBookingApp.Models;

namespace SportsBookingApp.Services;

public static class FacilityImageHelper
{
    // Comprehensive high-resolution sports venue photography mapped across multiple keywords
    private static readonly Dictionary<string, string[]> SportImageMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["football"] = new[]
        {
            "https://images.unsplash.com/photo-1529900248461-9056a3bf663d?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1574629810360-7efbbe195018?auto=format&fit=crop&w=1000&q=80"
        },
        ["soccer"] = new[]
        {
            "https://images.unsplash.com/photo-1529900248461-9056a3bf663d?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1574629810360-7efbbe195018?auto=format&fit=crop&w=1000&q=80"
        },
        ["turf"] = new[]
        {
            "https://images.unsplash.com/photo-1529900248461-9056a3bf663d?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80"
        },
        ["pitch"] = new[]
        {
            "https://images.unsplash.com/photo-1529900248461-9056a3bf663d?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80"
        },
        ["badminton"] = new[]
        {
            "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1613918108466-292b78a8ef95?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1599474924187-334a4ae5bd3c?auto=format&fit=crop&w=1000&q=80"
        },
        ["shuttle"] = new[]
        {
            "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1613918108466-292b78a8ef95?auto=format&fit=crop&w=1000&q=80"
        },
        ["tennis"] = new[]
        {
            "https://images.unsplash.com/photo-1595435934249-5df7ed86e1c0?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1622279457486-62dcc4a431d6?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?auto=format&fit=crop&w=1000&q=80"
        },
        ["basketball"] = new[]
        {
            "https://images.unsplash.com/photo-1546519638-68e109498ffc?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1519861531473-9200262188bf?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1574623452334-1e0ac2b3ccb4?auto=format&fit=crop&w=1000&q=80"
        },
        ["hoop"] = new[]
        {
            "https://images.unsplash.com/photo-1546519638-68e109498ffc?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1519861531473-9200262188bf?auto=format&fit=crop&w=1000&q=80"
        },
        ["swimming"] = new[]
        {
            "https://images.unsplash.com/photo-1576013551627-0cc20b96c2a7?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1519315901367-f34ff9154487?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1530549387789-4c1017266635?auto=format&fit=crop&w=1000&q=80"
        },
        ["pool"] = new[]
        {
            "https://images.unsplash.com/photo-1576013551627-0cc20b96c2a7?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1519315901367-f34ff9154487?auto=format&fit=crop&w=1000&q=80"
        },
        ["aquatic"] = new[]
        {
            "https://images.unsplash.com/photo-1576013551627-0cc20b96c2a7?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1519315901367-f34ff9154487?auto=format&fit=crop&w=1000&q=80"
        },
        ["cricket"] = new[]
        {
            "https://images.unsplash.com/photo-1531415074868-036b1c57e32b?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1587280501635-68a0e82cd5ff?auto=format&fit=crop&w=1000&q=80"
        },
        ["gym"] = new[]
        {
            "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1571902943202-507ec2618e8f?auto=format&fit=crop&w=1000&q=80"
        },
        ["fitness"] = new[]
        {
            "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?auto=format&fit=crop&w=1000&q=80"
        },
        ["workout"] = new[]
        {
            "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1571902943202-507ec2618e8f?auto=format&fit=crop&w=1000&q=80"
        },
        ["squash"] = new[]
        {
            "https://images.unsplash.com/photo-1599474924187-334a4ae5bd3c?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?auto=format&fit=crop&w=1000&q=80"
        },
        ["volleyball"] = new[]
        {
            "https://images.unsplash.com/photo-1612872087720-bb876e2e67d1?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1592656094267-764a45160876?auto=format&fit=crop&w=1000&q=80"
        },
        ["table tennis"] = new[]
        {
            "https://images.unsplash.com/photo-1534158914592-062992fbe900?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1609710228159-0fa9bd7c0827?auto=format&fit=crop&w=1000&q=80"
        },
        ["ping pong"] = new[]
        {
            "https://images.unsplash.com/photo-1534158914592-062992fbe900?auto=format&fit=crop&w=1000&q=80"
        },
        ["track"] = new[]
        {
            "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1552674605-db6ffd4facb5?auto=format&fit=crop&w=1000&q=80"
        },
        ["athletics"] = new[]
        {
            "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=1000&q=80"
        },
        ["hall"] = new[]
        {
            "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1599474924187-334a4ae5bd3c?auto=format&fit=crop&w=1000&q=80"
        },
        ["pavilion"] = new[]
        {
            "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80"
        },
        ["arena"] = new[]
        {
            "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80"
        },
        ["centre"] = new[]
        {
            "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&w=1000&q=80"
        },
        ["center"] = new[]
        {
            "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80"
        },
        ["court"] = new[]
        {
            "https://images.unsplash.com/photo-1595435934249-5df7ed86e1c0?auto=format&fit=crop&w=1000&q=80",
            "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=1000&q=80"
        }
    };

    // 12 curated, vibrant, high-definition sports arena images for deterministic fallback rotation
    private static readonly string[] UniversalSportsGallery = new[]
    {
        "https://images.unsplash.com/photo-1529900248461-9056a3bf663d?auto=format&fit=crop&w=1000&q=80", // 3G Turf Pitch
        "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=1000&q=80", // Badminton Court
        "https://images.unsplash.com/photo-1595435934249-5df7ed86e1c0?auto=format&fit=crop&w=1000&q=80", // Tennis Court
        "https://images.unsplash.com/photo-1576013551627-0cc20b96c2a7?auto=format&fit=crop&w=1000&q=80", // Swimming Pool
        "https://images.unsplash.com/photo-1546519638-68e109498ffc?auto=format&fit=crop&w=1000&q=80", // Basketball Arena
        "https://images.unsplash.com/photo-1531415074868-036b1c57e32b?auto=format&fit=crop&w=1000&q=80", // Cricket Ground
        "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&w=1000&q=80", // Gym / Fitness Center
        "https://images.unsplash.com/photo-1599474924187-334a4ae5bd3c?auto=format&fit=crop&w=1000&q=80", // Squash Court
        "https://images.unsplash.com/photo-1612872087720-bb876e2e67d1?auto=format&fit=crop&w=1000&q=80", // Volleyball Court
        "https://images.unsplash.com/photo-1534158914592-062992fbe900?auto=format&fit=crop&w=1000&q=80", // Table Tennis
        "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80", // Floodlit Stadium
        "https://images.unsplash.com/photo-1517649763962-0c623266ddc0?auto=format&fit=crop&w=1000&q=80"  // Sports Hall
    };

    public static string GetFacilityImage(string typeName, string facilityName = null, int facilityId = 0)
    {
        var images = GetImagesForFacility(typeName, facilityName);
        if (images != null && images.Length > 0)
        {
            int seed = facilityId != 0 ? facilityId : Math.Abs((facilityName ?? typeName ?? "0").GetHashCode());
            return images[seed % images.Length];
        }

        int fallbackIndex = (facilityId > 0 ? facilityId : Math.Abs((facilityName ?? "0").GetHashCode())) % UniversalSportsGallery.Length;
        return UniversalSportsGallery[fallbackIndex];
    }

    public static string[] GetFacilityGallery(string typeName, string facilityName = null)
    {
        var images = GetImagesForFacility(typeName, facilityName);
        if (images != null && images.Length >= 3) return images;

        // Ensure 3 distinct photos
        return new[]
        {
            images != null && images.Length > 0 ? images[0] : UniversalSportsGallery[0],
            UniversalSportsGallery[1],
            UniversalSportsGallery[10]
        };
    }

    private static string[] GetImagesForFacility(string typeName, string facilityName)
    {
        string combined = $"{typeName} {facilityName}".ToLowerInvariant();

        foreach (var kvp in SportImageMap)
        {
            if (combined.Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    public static string GetSportIcon(string typeName)
    {
        string t = (typeName ?? "").ToLowerInvariant();
        if (t.Contains("tennis")) return "bi-dribbble";
        if (t.Contains("football") || t.Contains("soccer") || t.Contains("turf") || t.Contains("pitch")) return "bi-globe-americas";
        if (t.Contains("basketball") || t.Contains("hoop")) return "bi-circle";
        if (t.Contains("swim") || t.Contains("pool") || t.Contains("aquatic")) return "bi-water";
        if (t.Contains("badminton") || t.Contains("shuttle")) return "bi-feather";
        if (t.Contains("cricket")) return "bi-shield-shaded";
        if (t.Contains("gym") || t.Contains("fitness") || t.Contains("workout")) return "bi-heart-pulse-fill";
        if (t.Contains("squash")) return "bi-grid-3x3-gap-fill";
        if (t.Contains("volleyball")) return "bi-sun-fill";
        if (t.Contains("table tennis") || t.Contains("ping pong")) return "bi-disc-fill";
        if (t.Contains("track") || t.Contains("athletics")) return "bi-speedometer2";
        return "bi-trophy-fill";
    }

    public static string[] GetAmenities(string typeName)
    {
        string t = (typeName ?? "").ToLowerInvariant();
        var list = new List<string> { "Free Parking", "Changing Rooms", "Drinking Water" };

        if (t.Contains("football") || t.Contains("turf") || t.Contains("pitch") || t.Contains("tennis") || t.Contains("cricket"))
        {
            list.Add("LED Floodlights");
            list.Add("Locker Facilities");
        }
        else if (t.Contains("swim") || t.Contains("pool") || t.Contains("aquatic"))
        {
            list.Add("Heated Water");
            list.Add("Showers & Lockers");
            list.Add("Lifeguard on Duty");
        }
        else if (t.Contains("gym") || t.Contains("fitness") || t.Contains("workout"))
        {
            list.Add("Air-conditioned");
            list.Add("Free Wi-Fi");
            list.Add("Certified Trainer");
        }
        else if (t.Contains("badminton") || t.Contains("basketball") || t.Contains("squash"))
        {
            list.Add("Wooden Flooring");
            list.Add("Equipment Rental");
            list.Add("High Ceiling");
        }

        return list.ToArray();
    }
}
