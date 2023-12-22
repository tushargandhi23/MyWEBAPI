using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JeavioTestCandidate.Models;

using System.Linq;

namespace JeavioTestCandidate.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private static List<Candidate> candidates = new List<Candidate>()
        {
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "Jimmy coder",
                skills = new List<string>
                {
                    "javascript", "es6", "nodejs", "express"
                }
            },
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "TG coder",
                skills = new List<string>
                {
                    "javascript", "express", "angular"
                }
            },
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "DG coder",
                skills = new List<string>
                {
                    "javascript", "express", "angular"
                }
            }
        };


        private readonly CandidateRepository _candidateRepository;
        public CandidatesController()
        {
            _candidateRepository = new CandidateRepository();
        }

        [HttpGet]
        public List<Candidate> GetAll()
        {
            return _candidateRepository.GetAll();
        }


        /// <summary>
        /// Filters candidates based on the coverage
        /// </summary>
        /// <param name="skills">comma separated skills for the filter</param>
        /// <returns>List of candidates based on the skills</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates(string skills)
        {
            if(string.IsNullOrEmpty(skills))
                return StatusCode(StatusCodes.Status400BadRequest);

            IEnumerable<Candidate> RequestedCandidates = _candidateRepository.GetCandidates(skills);
            if(RequestedCandidates == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                return Ok(RequestedCandidates);
            }
        }
        
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddCandidate([FromBody] Candidate candidate)
        {
            if(candidate == null)            
                return StatusCode(StatusCodes.Status400BadRequest);            
            _candidateRepository.AddCandidate(candidate);
            return Ok();
        }

    }

    public class CandidateRepository
    {
        private static List<Candidate> candidates = new List<Candidate>()
        {
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "Jimmy coder",
                skills = new List<string>
                {
                    "javascript", "es6", "nodejs", "express"
                }
            },
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "TG coder",
                skills = new List<string>
                {
                    "javascript", "express", "angular"
                }
            },
            new Candidate()
            {
                id = Guid.NewGuid(),
                name = "DG coder",
                skills = new List<string>
                {
                    "javascript", "express", "angular"
                }
            }
        };

        public List<Candidate> GetAll()
        {
            return candidates;
        }
        public IEnumerable<Candidate> GetCandidates(string skills)
        {          

            List<string> skillList = skills.Replace(" ", string.Empty).Split(',').ToList();

            int maxMatches = candidates.Select(candidate => new
            {
                Candidate = candidate,
                coverage = candidate.skills.Count(skill => skillList.Contains(skill))
            }).OrderByDescending(x => x.coverage).FirstOrDefault().coverage;

            if (maxMatches > 0)
            {
                var selectedCandidates = candidates.Select(candidate => new
                {
                    Candidate = candidate,
                    coverage = candidate.skills.Count(skill => skillList.Contains(skill))
                })
                 .OrderByDescending(result => result.coverage).Where(result => result.coverage == maxMatches).Select(result => result.Candidate);

                return selectedCandidates.ToList();
            }
            else
            {
                return null;
            }
        }

        public void AddCandidate(Candidate candidate)
        {            
            candidates.Add(candidate);            
        }

    }
}
