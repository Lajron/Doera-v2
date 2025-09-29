using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities {
    public class TodoItemTag {
        public Guid TodoItemId { get; set; }
        public TodoItem? TodoItem { get; set; }
        public Guid UserTagId { get; set; }
        public UserTag? UserTag { get; set; }
    }
}
