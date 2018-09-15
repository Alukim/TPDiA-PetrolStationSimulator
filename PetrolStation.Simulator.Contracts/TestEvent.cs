using PetrolStation.Infrastructure;

namespace PetrolStation.Simulator.Contracts
{
    public class TestEvent : IEvent
    {
        public TestEvent(string hello, int value)
        {
            Hello = hello;
            Value = value;
        }

        public string Hello { get; }

        public int Value { get; }
    }
}
