using System.Collections.Generic;

namespace ToastifyAPI.GitHub.Model
{
    public class CollectionData<T> : BaseModel where T : BaseModel
    {
        public ICollection<T> Collection { get; set; }
    }
}