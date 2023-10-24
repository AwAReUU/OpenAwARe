namespace AwARe.Visualization
{
    public interface IVisualizer<In>
    {
        public void Visualize(In toShow);
        public void Visualize();
    }
}

