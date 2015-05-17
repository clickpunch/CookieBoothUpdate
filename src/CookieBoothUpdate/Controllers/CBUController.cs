using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using CookieBoothUpdate.Models;
using System;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace CookieBoothUpdate.Controllers
{
//	[Route("api/[controller]")]
    [Route("api")]
    public class CBUController : Controller
	{
        /*
		static readonly List<CBUItem> _items = new List<CBUItem>()
		{
			new CBUItem { Id = 1, Title = "First Item" }
		};
		*/

        private readonly ICBURepository _repository;

        public CBUController(ICBURepository repository)
        {
            _repository = repository;
        }
        
		[HttpGet]
        public IEnumerable<CBUItem> GetAll()
        {
            return _repository.AllItems;
        }

        [BasicAuthenticationAttribute]
        [HttpGet("{zipcode}", Name = "GetByZipcodeRoute")]
        public IActionResult GetByZipcode (string zipcode)
        {
            var item = _repository.GetByZipcode(zipcode);
            if (item == null)
            {
                //return HttpNotFound();
                item = new CBUItem { Zipcode = "Not Found" };
            }
            
            return new ObjectResult(item);
        }

        [HttpPost]
        public void CreateCBUItem([FromBody] CBUItem item)
        {
            if (!ModelState.IsValid)
            {
                Context.Response.StatusCode = 400;
            }
            else
            {
                _repository.Add(item);

                string url = Url.RouteUrl("GetByZipcodeRoute", new { zipcode = item.Zipcode }, 
                    Request.Scheme, Request.Host.ToUriComponent());

                Context.Response.StatusCode = 201;
                Context.Response.Headers["Location"] = url;
            }
        }

        [HttpDelete("{zipcode}")]
        public IActionResult DeleteItem(string zipcode)
        {
            if (_repository.TryDelete(zipcode))
            {
                return new HttpStatusCodeResult(204); // 201 No Content
            }
            else
            {
                return HttpNotFound();
            }
        }
	}

    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        public string BasicRealm { get; set; }

        public BasicAuthenticationAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (!string.IsNullOrEmpty(auth))
            {
                var cred = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };

                var userIp = Convert.ToString(filterContext.HttpContext.GetFeature<IHttpConnectionFeature>()?.RemoteIpAddress);

                var configuration = new Configuration().AddJsonFile("config.json");
                var authorizedUserString = configuration.Get("Users");
                List<string> authorizedUsers = authorizedUserString.Split(';').ToList<string>();
                foreach (string curAuthorizedUserString in authorizedUsers)
                {
                    List<string> curAuthorizedUser = curAuthorizedUserString.Split(',').ToList<string>();
                    if (user.Name == curAuthorizedUser[0] && user.Pass == curAuthorizedUser[1] && userIp == curAuthorizedUser[2])
                    {
                        return;
                    }
                }
                    
            }
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.HttpContext.Response.WriteAsync("Unauthorized");
            
            // need to end the response here since no idea how to figure this out, 
            // I will redirect to root and this will prevent from executing API
            filterContext.Result = new RedirectResult("/");

            // asks for user/pass again - cannot have redirect with this.. 
            //res.Headers.Append("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", BasicRealm ?? "MyWebSite"));
        }
    }
}
