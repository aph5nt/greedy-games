using Game.Minefield.Contracts.Model;

namespace Game.Minefield.Services
{
    public interface ICreateStateStrategy
    {
        State CreateState(Settings settings);
    }
}