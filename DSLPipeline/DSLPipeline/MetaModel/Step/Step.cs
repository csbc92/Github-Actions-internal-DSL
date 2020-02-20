namespace DSLPipeline.MetaModel.Step
{
    public abstract class Step
    {
        public string Id { get; }
        public string Name { get; }

        public Step(string name)
        {
            Name = name;
        }
        
        public Step(string id, string name)
        {
            Id = id;
            Name = name;
        }
        
        /// <summary>
        /// A Job is considered to be equal if their Name and ID match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            Step other = (Step) obj;

            if (this.Id == null || other.Id == null)
            {
                return false;
            }
            
            return this.Id.Equals(other.Id);
        }
        
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}