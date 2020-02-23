namespace DSLPipeline.MetaModel.Builders.v2.Interfaces
{
    public interface IBuild<T>
    {
        public void Build();
        
        public T Collect();
    }
}