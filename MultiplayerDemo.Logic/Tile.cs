
namespace MultiplayerDemo.Logic
{
    public class Tile : IEquatable<Tile?>
    {

        public const int Max = 6;

        public Tile(int v1, int v2)
        {
            Num1 = v1;
            Num2 = v2;
        }

        public Tile()
        {
            Num1 = Random.Shared.Next(1, Max + 1);
            Num2 = Random.Shared.Next(1, Max + 1);
        }

        public int Num1 { get; }
        public int Num2 { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Tile);
        }

        public bool Equals(Tile? other)
        {
            return other is not null &&
                   (
                       (Num1 == other.Num1 && Num2 == other.Num2) ||
                       (Num1 == other.Num2 && Num2 == other.Num1)
                   );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Num1, Num2);
        }

        public static bool operator ==(Tile? left, Tile? right)
        {
            return EqualityComparer<Tile>.Default.Equals(left, right);
        }

        public static bool operator !=(Tile? left, Tile? right)
        {
            return !(left == right);
        }

        public override string ToString() => $"[{Num1}|{Num2}]";
    }
}