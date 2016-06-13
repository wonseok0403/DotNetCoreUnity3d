using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreWeb1.Modules.Score
{
    public class ScoreController : Controller
    {
        //Ref to our DB proxy
        protected ScoreContext Context = new ScoreContext();
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Context.Dispose();
        }

        // Note not using HTTP verbs correctly due to WWW limitation
        // Using action name convention instead.

        // GET api/Score/Get
        [HttpPost]
        [Route("api/Score/Get")]
        public async Task<ScoreModelContainer> Get()
        {
            // PROTIP : filter these results using http paramaters or something like ODATA 
            // ODATA exposes full lambda searches to the client

            return new ScoreModelContainer
            {
                Scores = await Context.Scores.OrderByDescending(o => o.Points).ToArrayAsync()
            };
        }

        // GET api/Score/Get/5
        [HttpPost("{id}")]
        [Route("api/Score/Get/{id}")]
        public Task<ScoreModel> Get(string id)
        {
            return Context.Scores.FirstOrDefaultAsync(o => o.UserName == id);
        }

        // POST api/Score/Post
        [HttpPost]
        [Route("api/Score/Post")]
        public async Task<IActionResult> Post([FromBody]ScoreModel model)
        {
            //Sanity
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Username or something");
            }

            //Check for new or update
            var old = await Context.Scores.FirstOrDefaultAsync(o => o.UserName == model.UserName);

            if (old == null)
            {
                //New
                Context.Scores.Add(model);
                await Context.SaveChangesAsync();

                return Ok(model);
            }
            else
            {
                //UPDATE
                old.Points = model.Points;
                await Context.SaveChangesAsync();
                return Ok(model);
            }
        }

        // DELETE api/Score/Delete/5
        [HttpPost("{id}")]
        [Route("api/Score/Delete/{id}")]
        public async Task Delete(string id)
        {
            //Check for new or update
            var old = await Context.Scores.FirstOrDefaultAsync(o => o.UserName == id);
            if (old != null)
            {
                Context.Scores.Remove(old);
                await Context.SaveChangesAsync();
            }
        }
    }
}
