namespace AwARe.Visualizers
{
    public interface IVisualizer<In>
    {
        public void Visualize(In toShow);
        public void Visualize();
    }
}

