using System.Security.Claims;
using LR1Backend.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQl_Lb1_Kosinskiy_Kostikova.Models;
using NoSQl_Lb1_Kosinskiy_Kostikova.Repositories;

namespace NoSQl_Lb1_Kosinskiy_Kostikova.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NoteController : Controller
    {
        private readonly NoteRepository noteRepository;
        private readonly UserRepository userRepository;
        public NoteController(UserRepository userRepository, NoteRepository noteRepository)
        {
            this.noteRepository = noteRepository;
            this.userRepository = userRepository;
        }

        [HttpGet]
        [Route("get")]
        public IActionResult Get(Guid id)
        {
            var note = noteRepository.GetById(id);
            return Ok(note);
        }


        [HttpGet]
        [Route("getByUserId")]
        public IActionResult GetByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var note = noteRepository.GetByUserId(Guid.Parse(userId));
            return Ok(note);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddNote([FromBody] NoteApiModel NAP)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            Note note = new Note()
            {
                Id = Guid.NewGuid(),
                Title = NAP.Title,
                Text = NAP.Text,
                UserId = Guid.Parse(userId)

            };
            var noten = noteRepository.Insert(note);
            return Ok(noten);
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult DeleteNote([FromBody] Note noten)
        {
            var noteId = noten.Id;
            noteRepository.Delete(noteId);
            return Ok();
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> EditNote(string id, [FromBody] EditNodeModel model)
        {
            var oldNote = noteRepository.GetById(Guid.Parse(id));

            oldNote.Title = model.NewTitle;
            oldNote.Text = model.NewText;
            oldNote.LastUpdate = DateTime.UtcNow;
            noteRepository.Edit(oldNote);
            return NoContent();
        }

        [HttpGet]
        [Route("search/{queryString}")]
        public async Task<IActionResult> Search(string queryString)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var source = await noteRepository.Search(Guid.Parse(userId), queryString);

            var notes = source.Select(x => new NoteApiModel()
            {
                Text = x.Text,
                Title = x.Title
            });

            return Ok(notes);
        }
    }
}
