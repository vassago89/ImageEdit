using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Helpers.Command
{
    public class Command : ICommand
    {
        Action _undo;
        Action _redo;

        public Command(Action redo, Action undo)
        {
            _redo = redo;
            _undo = undo;
        }

        public void Redo()
        {
            _redo();
        }

        public void Undo()
        {
            _undo();
        }
    }

    public class Command<T> : ICommand
    {
        T _class;
        Action<T> _undo;
        Action<T> _redo;

        public Command(T @class, Action<T> redo, Action<T> undo)
        {
            _class = @class;

            _redo = redo;
            _undo = undo;
        }

        public void Redo()
        {
            _redo(_class);
        }

        public void Undo()
        {
            _undo(_class);
        }
    }

}