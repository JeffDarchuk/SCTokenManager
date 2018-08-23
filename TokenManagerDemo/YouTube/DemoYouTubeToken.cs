using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.TokenExtensions;
using TokenManager.Data.Tokens;

namespace TokenManagerDemo.YouTube
{
	public class DemoYouTubeToken : AutoToken<YouTubeModel>
	{
		public DemoYouTubeToken() : base("Google", "Multimedia/32x32/film_clip_h.png", "YouTube Video")
		{
		}

		public override string TokenIdentifierText(YouTubeModel model)
		{
			return $"<span>Video ID <strong>{model.VideoId}</strong>, click to change</span>";
		}

		public override TokenButton TokenButton()
		{
			return new TokenButton("YouTube", "Multimedia/32x32/film_clip_h.png", 500);
		}

		public override string Render(YouTubeModel model)
		{
			return $"<iframe width='560' height='315' src='https://www.youtube.com/embed/{model.VideoId}' frameborder='0' allow='autoplay; encrypted-media' allowfullscreen></iframe>";
		}
	}
}
