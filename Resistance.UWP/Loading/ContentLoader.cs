using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resistance.Loading
{
    class ContentLoader<T>
    {
        readonly String contentName;

        public ContentLoader(String name,LoadedDelegate<T> del)
        {
            this.contentName = name;

        }
    }
}
