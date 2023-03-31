using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NoSQl_Lb1_Kosinskiy_Kostikova.Models;

namespace NoSQl_Lb1_Kosinskiy_Kostikova.Repositories
{
    
    public class NoteRepository
    {
        private readonly IMongoCollection<Note> collection;
        public NoteRepository(IConfiguration configuration)
        {
            var connString =
                configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("notes_db")
                .GetCollection<Note>("notes");
        }
        public Note Insert(Note note)
        {
            note.Id = Guid.NewGuid();
            collection.InsertOne(note);
            return note;
        }
        public Note GetById(Guid id)
        {
            return collection
                .Find(x => x.Id == id)
                .FirstOrDefault();
        }
        public IReadOnlyCollection<Note> GetByUserId(Guid
            userId)
        {
            return collection
                .Find(x => x.UserId == userId)
                .ToList();
        }
        public void Delete(Guid noteId)
        {
            collection.DeleteOne((x) => x.Id == noteId);
        }
        public async void CreateIndexes()
        {
            await collection.Indexes
                .CreateOneAsync(new
                    CreateIndexModel<Note>(Builders<Note>.IndexKeys.Ascending(_ =>
                        _.Id)))
                .ConfigureAwait(false);
            await collection.Indexes
                .CreateOneAsync(new
                    CreateIndexModel<Note>(Builders<Note>.IndexKeys.Ascending(_ =>
                        _.UserId)))
                .ConfigureAwait(false);
        }

        public Task Edit(Note newNote)
        {
            return collection.ReplaceOneAsync(x => x.Id == newNote.Id, newNote);
        }

        public Task<List<Note>> Search(Guid userId, string searchQuery)
        {
            var lowerCaseQuery = searchQuery.ToLower();
            return collection.Find(x => (x.Text.ToLower().Contains(lowerCaseQuery) || x.Title.ToLower().Contains(lowerCaseQuery)) && x.UserId == userId).ToListAsync();
        }
    }
}
