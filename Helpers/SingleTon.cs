using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Helpers
{
    abstract class SingleTon<T> where T : class, new()
    {
        private static T _instance;
        public static T Instance => _instance ?? (_instance = new T());
    }

    abstract class BindableSingleTon<T> : BindableBase where T : class, new()
    {
        private static T _instance;
        public static T Instance => _instance ?? (_instance = new T());
    }
}
