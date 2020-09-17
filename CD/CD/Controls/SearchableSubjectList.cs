using Xamarin.Forms.Internals;
using CD.Models;
namespace CD.Controls
{
    /// <summary>
    /// This class extends the behavior of the SfListView control to filter the items in Songs PlayList page based on text.
    /// </summary>
    [Preserve(AllMembers = true)]

    public class SearchableSubjectList : SearchableListView
    {
        #region Method

        /// <summary>
        /// Filtering the list view items based on the search text.
        /// </summary>
        /// <param name="obj">The list view item</param>
        /// <returns>Returns the filtered item</returns>
        public override bool FilterContacts(object obj)
        {
            if (base.FilterContacts(obj))
            {
                var taskInfo = obj as Models.Subject;

                if (taskInfo == null || string.IsNullOrEmpty(taskInfo.SubjectName))
                {
                    return false;
                }

                return taskInfo.SubjectName.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant());
            }
            return false;
        }
        #endregion
    }
}
