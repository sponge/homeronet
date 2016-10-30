namespace Homero.Core
{
    public interface IUser : ISendable
    {
        string Name { get; }

        string Mention { get; }
    }
}