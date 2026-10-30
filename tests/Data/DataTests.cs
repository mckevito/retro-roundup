using System.Linq;
using RetroRoundup.Core;

namespace RetroRoundup.Tests.Data;

/// <summary>The data tables load and make sense.</summary>
public class DataTests : TestCase
{
    public void TestTablesLoad()
    {
        Check(GameData.Load<StreetTable>(StreetTable.Path).Buildings.Count > 0, "street.json should list buildings");
        Check(GameData.Load<LookTable>(LookTable.Path).LineColor.Length == 3, "look.json line_color needs r, g, b");
        Check(GameData.Load<PlayerTable>(PlayerTable.Path).EyeHeightM > 0, "player.json needs an eye height");
    }

    public void TestBuildingsStayOffTheRoad()
    {
        var street = GameData.Load<StreetTable>(StreetTable.Path);
        foreach (var b in street.Buildings)
        {
            bool onRoad = b.X0 < street.RoadHalfWidthM && b.X1 > -street.RoadHalfWidthM;
            // A building may close the far end of the street, beyond the walkable part.
            Check(!(onRoad && b.Z0 < street.RoadBlockingAllowedBeyondM), $"building at x {b.X0}..{b.X1}, z {b.Z0} blocks the road");
        }
    }

    public void TestBuildingsAreWellFormed()
    {
        foreach (var b in GameData.Load<StreetTable>(StreetTable.Path).Buildings)
        {
            string where = $"building at x {b.X0}, z {b.Z0}";
            Check(b.X1 > b.X0 && b.Z1 > b.Z0 && b.Height > 0, where + " needs a positive size");
            Check(b.Floors >= 1 && b.BaysSide >= 1 && b.BaysFront >= 1, where + " needs floors and bays");
            Check(b.StreetSide is "left" or "right", where + ": street_side must be left or right");
            Check(b.BalconyBays.All(i => i >= 0 && i < b.BaysSide), where + " has a balcony bay outside its bays");
        }
    }
}
