using System.Collections.Generic;

namespace ToastifyAPI.GitHub.Model
{
    public class CollectionData<T> : BaseModel where T : BaseModel
    {
        #region Public Properties

        public ICollection<T> Collection { get; set; }

        #endregion
    }
}