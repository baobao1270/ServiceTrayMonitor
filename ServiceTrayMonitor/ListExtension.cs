using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Joseph.ServiceTrayMonitor
{
    public static class ListExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> list)
        {
            ObservableCollection<T> observableCollection = new ObservableCollection<T>();
            foreach (var item in list)
            {
                observableCollection.Add(item);
            }

            return observableCollection;
        }
    }
}