using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplicacionToDoList.Models;

namespace AplicacionToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private static int conteoEliminaciones = 0;
        private static int conteoCompletadas = 0;
        private static int conteoIncompletas = 0;
        public TodosController(ApiDbContext context)
        {
            _context = context;
            ActualizarConteoIncompletas();
        }
        private void ActualizarConteoIncompletas()
        {
            conteoIncompletas = _context.todos.Count(t => t.estado == false);
        }

        // GET: api/Todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todos>>> Gettodos()
        {

            var todosList = await _context.todos.ToListAsync();

            // Actualizar conteoElimi y conteoComple en cada tarea antes de enviarlas al frontend
            foreach (var todo in todosList)
            {
                todo.conteoElimi = conteoEliminaciones;
                todo.conteoComple = conteoCompletadas;
                todo.conteoIncomple = conteoIncompletas;
            }

            return todosList;


            //return await _context.todos.ToListAsync();
        }

        // GET: api/Todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Todos>> GetTodos(int id)
        {
            var todos = await _context.todos.FindAsync(id);

            if (todos == null)
            {
                return NotFound();
            }
            todos.conteoElimi = conteoEliminaciones;
            todos.conteoComple = conteoCompletadas;
            todos.conteoIncomple = conteoIncompletas;

            return todos;
        }

        // PUT: api/Todos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodos(int id, Todos todos)
        {
            if (id != todos.id)
            {
                return BadRequest();
            }

            // Verificar el cambio en el estado de la tarea
            var existingTodo = await _context.todos.AsNoTracking().FirstOrDefaultAsync(t => t.id == id);

            if (existingTodo != null)
            {
                if (existingTodo.estado == false && todos.estado == true)
                {
                    // Si el estado cambia a "true", incrementar conteo de completadas
                    conteoCompletadas++;
                    conteoIncompletas = Math.Max(0, conteoIncompletas - 1);
                }
                else if (existingTodo.estado == true && todos.estado == false)
                {
                    // Si el estado cambia a "false", restar pero asegurarse de que no sea menor a cero
                    conteoCompletadas = Math.Max(0, conteoCompletadas - 1);
                    conteoIncompletas++;
                }
            }

            _context.Entry(todos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            todos.conteoElimi = conteoEliminaciones;
            todos.conteoComple = conteoCompletadas;
            todos.conteoIncomple = conteoIncompletas;
            return NoContent();
        }

        // POST: api/Todos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Todos>> PostTodos(Todos todos)
        {
            if (todos.estado == false)
            {
                conteoIncompletas++;
            }
            todos.conteoComple = conteoCompletadas;
            todos.conteoElimi = conteoEliminaciones;
            todos.conteoIncomple = conteoIncompletas;

            _context.todos.Add(todos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodos", new { id = todos.id }, todos);
        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodos(int id)
        {
            var todos = await _context.todos.FindAsync(id);
            if (todos == null)
            {
                return NotFound();
            }

            conteoEliminaciones++;
            if (todos.estado == false)
            {
                conteoIncompletas = Math.Max(0, conteoIncompletas - 1);
            }
            todos.conteoElimi = conteoEliminaciones;
            todos.conteoComple = conteoCompletadas;
            todos.conteoIncomple = conteoIncompletas;

            _context.Entry(todos).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            _context.todos.Remove(todos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodosExists(int id)
        {
            return _context.todos.Any(e => e.id == id);
        }
    }
}
