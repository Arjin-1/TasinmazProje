using NetTopologySuite.Geometries;

namespace Tasinmaz.Entities.Concrete
{
    public class AnalysisGeometry
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Code { get; set; } = null!;

        public Geometry Geometry { get; set; } = null!;

        public double AreaM2 { get; set; }


        public bool IsUnionResult { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

