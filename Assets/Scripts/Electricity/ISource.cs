using Electricity.Wires;

namespace Electricity
{
    public interface ISource
    {
        Wire Wire { get; set; }
    }
}