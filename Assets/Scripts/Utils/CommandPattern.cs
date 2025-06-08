using System;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// Интерфейс для команд
    /// Реализует паттерн Command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Выполняет команду
        /// </summary>
        void Execute();

        /// <summary>
        /// Отменяет выполненную команду, если это возможно
        /// </summary>
        /// <returns>True, если команда успешно отменена</returns>
        bool Undo();

        /// <summary>
        /// Проверяет, может ли команда быть отменена
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Возвращает описание команды для отображения пользователю
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Базовая реализация команды
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Описание команды
        /// </summary>
        public virtual string Description => GetType().Name;

        /// <summary>
        /// Проверяет, может ли команда быть отменена
        /// </summary>
        public virtual bool CanUndo => false;

        /// <summary>
        /// Выполняет команду
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Отменяет выполненную команду, если это возможно
        /// </summary>
        /// <returns>True, если команда успешно отменена</returns>
        public virtual bool Undo()
        {
            return false;
        }
    }

    /// <summary>
    /// Менеджер команд, который отслеживает выполненные команды и позволяет отменять их
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// Максимальное количество команд в истории
        /// </summary>
        private readonly int _maxHistorySize;

        /// <summary>
        /// Стек выполненных команд
        /// </summary>
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();

        /// <summary>
        /// Стек отмененных команд для возможности повторного выполнения (Redo)
        /// </summary>
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        /// <summary>
        /// Событие, вызываемое при изменении стека команд
        /// </summary>
        public event Action OnCommandStackChanged;

        /// <summary>
        /// Создает новый менеджер команд
        /// </summary>
        /// <param name="maxHistorySize">Максимальное количество команд в истории</param>
        public CommandManager(int maxHistorySize = 100)
        {
            _maxHistorySize = Math.Max(1, maxHistorySize);
        }

        /// <summary>
        /// Выполняет команду и добавляет ее в историю
        /// </summary>
        /// <param name="command">Команда для выполнения</param>
        public void ExecuteCommand(ICommand command)
        {
            if (command == null) return;

            try
            {
                // Выполняем команду
                command.Execute();

                // Добавляем команду в стек отмены, если она может быть отменена
                if (command.CanUndo)
                {
                    _undoStack.Push(command);

                    // Очищаем стек повторного выполнения, так как была выполнена новая команда
                    _redoStack.Clear();

                    // Проверяем, не превышен ли размер истории
                    if (_undoStack.Count > _maxHistorySize)
                    {
                        // Удаляем самую старую команду
                        Stack<ICommand> tempStack = new Stack<ICommand>();

                        for (int i = 0; i < _undoStack.Count - 1; i++)
                        {
                            tempStack.Push(_undoStack.Pop());
                        }

                        _undoStack.Clear();

                        while (tempStack.Count > 0)
                        {
                            _undoStack.Push(tempStack.Pop());
                        }
                    }

                    OnCommandStackChanged?.Invoke();
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Ошибка при выполнении команды {command.Description}: {ex.Message}");
            }
        }

        /// <summary>
        /// Отменяет последнюю выполненную команду
        /// </summary>
        /// <returns>True, если команда успешно отменена</returns>
        public bool Undo()
        {
            if (_undoStack.Count == 0) return false;

            ICommand command = _undoStack.Pop();

            try
            {
                bool result = command.Undo();

                if (result)
                {
                    _redoStack.Push(command);
                    OnCommandStackChanged?.Invoke();
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Не удалось отменить команду {command.Description}");
                }

                return result;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Ошибка при отмене команды {command.Description}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Повторяет последнюю отмененную команду
        /// </summary>
        /// <returns>True, если команда успешно повторена</returns>
        public bool Redo()
        {
            if (_redoStack.Count == 0) return false;

            ICommand command = _redoStack.Pop();

            try
            {
                command.Execute();
                _undoStack.Push(command);
                OnCommandStackChanged?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Ошибка при повторном выполнении команды {command.Description}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Проверяет, доступна ли отмена
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Проверяет, доступно ли повторение
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Очищает историю команд
        /// </summary>
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            OnCommandStackChanged?.Invoke();
        }

        /// <summary>
        /// Возвращает количество команд в стеке отмены
        /// </summary>
        public int UndoCount => _undoStack.Count;

        /// <summary>
        /// Возвращает количество команд в стеке повторения
        /// </summary>
        public int RedoCount => _redoStack.Count;

        /// <summary>
        /// Возвращает описание последней выполненной команды
        /// </summary>
        public string LastCommandDescription => _undoStack.Count > 0 ? _undoStack.Peek().Description : null;
    }
}
