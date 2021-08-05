using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Helpers.Command
{
    public interface ICommandStack
    {
        bool CanRedo { get; }
        bool CanUndo { get; }

        void Redo();
        void Undo();

        void Push(ICommand command);
        void Clear();
    }
}
