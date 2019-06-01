using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Sample4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpellController : Controller
    {
        private readonly SpellCheckerService _service;

        public SpellController(SpellCheckerService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.Dictionary);
        }

        [HttpGet("{text}")]
        public IActionResult Get(string text)
        {
            var list = new List<Correction>();

            var words = _service.Tokenize(text.ToLower());
            foreach (var word in words)
            {
                if (!_service.Check(word.Value))
                {
                    var suggestions = _service.Suggest(word.Value);
                    list.Add(new Correction(word.Value, suggestions, word.Range));
                }
            }

            return Ok(list);
        }
    }

    public class SpellingRequest
    {
        public string Text { get; set; }
    }

    public struct Range
    {
        public Range(int from, int to) : this()
        {
            From = from;
            To = to;
        }

        public int From { get; }
        public int To { get; }
    }

    public class Correction
    {
        public Correction(string original, IReadOnlyList<string> suggestions, Range errorRange)
        {
            Original = original;
            Suggestions = suggestions;
            ErrorRange = errorRange;
        }

        public string Original { get; }
        public IReadOnlyList<string> Suggestions { get; }
        public Range ErrorRange { get; }
    }
}
