namespace AwARe.Visualizers
{
    public interface IVisualizable<In>
    {
        public IVisualizer<In> GetVisualizer();
    }
}
