using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Helpers.Command
{
    class CommandStack : BindableBase, ICommandStack
    {
        private RoundStack<ICommand> _redoStack;
        private RoundStack<ICommand> _undoStack;

        private bool _canRedo;
        public bool CanRedo
        {
            get => _canRedo;
            set => SetProperty(ref _canRedo, value);
        }

        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            set => SetProperty(ref _canUndo, value);
        }

        public CommandStack()
        {
            _redoStack = new RoundStack<ICommand>(1000);
            _undoStack = new RoundStack<ICommand>(1000);
        }

        public void Push(ICommand command)
        {
            _redoStack.Clear();

            command.Redo();
            _undoStack.Push(command);

            Refresh();
        }

        public void Redo()
        {
            if (CanRedo == false)
                return;

            var command = _redoStack.Pop();
            command.Redo();
            _undoStack.Push(command);

            Refresh();
        }

        public void Undo()
        {
            if (CanUndo == false)
                return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            Refresh();
        }

        private void Refresh()
        {
            CanUndo = _undoStack.Count > 0;
            CanRedo = _redoStack.Count > 0;
        }

        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
            Refresh();
        }
    }
}
