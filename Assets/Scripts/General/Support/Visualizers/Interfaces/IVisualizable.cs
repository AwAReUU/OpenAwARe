namespace AwARe.Visualization
{
    public interface IVisualizable<In>
    {
        public IVisualizer<In> GetVisualizer();
    }
}
