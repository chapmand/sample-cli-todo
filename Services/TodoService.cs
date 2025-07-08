using sample_cli_todo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace sample_cli_todo.Services
{
    public class TodoService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "todoitems.json");
        private List<TodoItem> _todos;
        public TodoService()
        {
            Load();
        }
        public List<TodoItem> GetAll()
        {
            return _todos.OrderBy(t => t.Id).ToList();
        }
        public void AddTodo(string title, string description)
        {
            int nextId = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
            var todo = new TodoItem
            {
                Id = nextId,
                Title = title,
                Description = description,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                CompletedAt = null
            };
            _todos.Add(todo);
            Save();
        }
        public void MarkComplete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null && !todo.IsCompleted)
            {
                todo.IsCompleted = true;
                todo.CompletedAt = DateTime.Now;
                Save();
            }
        }
        public void ToggleComplete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todo.IsCompleted = !todo.IsCompleted;
                todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;
                Save();
            }
        }
        public void UpdateTodo(TodoItem updatedTodo)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == updatedTodo.Id);
            if (todo != null)
            {
                todo.Title = updatedTodo.Title;
                todo.Description = updatedTodo.Description;
                Save();
            }
        }
        public void DeleteTodo(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                _todos.Remove(todo);
                Save();
            }
        }
        private void Load()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
            else
            {
                _todos = new List<TodoItem>();
            }
        }
        private void Save()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var json = JsonSerializer.Serialize(_todos);
            File.WriteAllText(_filePath, json);
        }
    }
}