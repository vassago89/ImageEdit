using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Helpers.Command
{
    public interface ICommand
    {
        void Redo();
        void Undo();
    }
}
